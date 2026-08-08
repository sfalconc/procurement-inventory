using MediatR;
using Microsoft.AspNetCore.Mvc;
using Procurement.Application.Products.Commands.CreateProduct;
using Procurement.Application.Products.Commands.DeleteProduct;
using Procurement.Application.Products.Commands.UpdateProduct;
using Procurement.Application.Products.DTOs;
using Procurement.Application.Products.Queries.GetProductById;
using Procurement.Application.Products.Queries.GetProducts;
using Microsoft.AspNetCore.Authorization;
using Procurement.Application.Common.Constants;

namespace Procurement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = PolicyConstants.RequirePurchaser)]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(
        IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var products = await _mediator.Send(
            new GetProductsQuery(),
            cancellationToken);

        return Ok(products);
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _mediator.Send(
            new GetProductByIdQuery(id),
            cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }


    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await _mediator.Send(
            new CreateProductCommand(
                request.Code,
                request.Name,
                request.Description,
                request.CategoryId),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new UpdateProductCommand(
                id,
                request.Code,
                request.Name,
                request.Description,
                request.CategoryId),
            cancellationToken);

        return NoContent();
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new DeleteProductCommand(id),
            cancellationToken);

        return NoContent();
    }
}