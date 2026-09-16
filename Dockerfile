# =========================
# Build stage
# =========================

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["src/Procurement.Api/Procurement.Api.csproj", "src/Procurement.Api/"]
COPY ["src/Procurement.Application/Procurement.Application.csproj", "src/Procurement.Application/"]
COPY ["src/Procurement.Domain/Procurement.Domain.csproj", "src/Procurement.Domain/"]
COPY ["src/Procurement.Infrastructure/Procurement.Infrastructure.csproj", "src/Procurement.Infrastructure/"]

RUN dotnet restore "src/Procurement.Api/Procurement.Api.csproj"

COPY . .

WORKDIR "/src/src/Procurement.Api"

RUN dotnet publish "Procurement.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore


# =========================
# Runtime stage
# =========================

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Procurement.Api.dll"]