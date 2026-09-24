# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/DemoApp.csproj ./
RUN dotnet restore DemoApp.csproj
COPY src/ ./
RUN dotnet publish DemoApp.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

# The aspnet image already runs as a non-root user; port 8080 avoids needing privileges.
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "DemoApp.dll"]
