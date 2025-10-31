# 🐳 Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["CampusEats/CampusEats.csproj", "CampusEats/"]
RUN dotnet restore "CampusEats/CampusEats.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/CampusEats"
RUN dotnet build "CampusEats.csproj" -c Release -o /app/build

# 🚀 Publish Stage
FROM build AS publish
RUN dotnet publish "CampusEats.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 🏃 Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=publish /app/publish .

# Create non-root user for security
RUN useradd -m -u 1000 appuser && chown -R appuser:appuser /app
USER appuser

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "CampusEats.dll"]
