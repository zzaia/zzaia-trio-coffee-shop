# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["CoffeeShop.Wasm/CoffeeShop.Wasm.csproj", "CoffeeShop.Wasm/"]

# Restore dependencies
RUN dotnet restore "CoffeeShop.Wasm/CoffeeShop.Wasm.csproj"

# Copy source code
COPY . .

# Build and publish
WORKDIR "/src/CoffeeShop.Wasm"
RUN dotnet publish "CoffeeShop.Wasm.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage - use nginx to serve static files
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Copy published Blazor WASM files
COPY --from=build /app/publish/wwwroot .

# Copy custom nginx configuration
COPY --from=build /src/CoffeeShop.Wasm/nginx.conf /etc/nginx/nginx.conf

# Expose port
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD wget --quiet --tries=1 --spider http://localhost:80/ || exit 1

# Entry point
CMD ["nginx", "-g", "daemon off;"]
