# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Lab.Coffe.sln", "./"]
COPY ["src/Lab.Coffe.Domain/Lab.Coffe.Domain.csproj", "src/Lab.Coffe.Domain/"]
COPY ["src/Lab.Coffe.Application/Lab.Coffe.Application.csproj", "src/Lab.Coffe.Application/"]
COPY ["src/Lab.Coffe.Infrastructure/Lab.Coffe.Infrastructure.csproj", "src/Lab.Coffe.Infrastructure/"]
COPY ["src/Lab.Coffe.API/Lab.Coffe.API.csproj", "src/Lab.Coffe.API/"]

# Restore dependencies
RUN dotnet restore "Lab.Coffe.sln"

# Copy all source files
COPY src/ ./src/

# Build the application
WORKDIR /src/src/Lab.Coffe.API
RUN dotnet build "Lab.Coffe.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Lab.Coffe.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published application
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p logs && chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "Lab.Coffe.API.dll"]
