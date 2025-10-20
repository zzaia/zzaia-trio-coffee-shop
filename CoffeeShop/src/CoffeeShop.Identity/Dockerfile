# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["CoffeeShop.Identity/CoffeeShop.Identity.csproj", "CoffeeShop.Identity/"]
COPY ["CoffeeShop.ServiceDefaults/CoffeeShop.ServiceDefaults.csproj", "CoffeeShop.ServiceDefaults/"]

# Restore dependencies
RUN dotnet restore "CoffeeShop.Identity/CoffeeShop.Identity.csproj"

# Copy source code
COPY . .

# Build application
WORKDIR "/src/CoffeeShop.Identity"
RUN dotnet build "CoffeeShop.Identity.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "CoffeeShop.Identity.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published application
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "CoffeeShop.Identity.dll"]
