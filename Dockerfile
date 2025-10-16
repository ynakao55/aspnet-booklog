# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ./src/*.csproj ./ 
RUN dotnet restore
COPY ./src/ .
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
WORKDIR /app
COPY --from=build /app/publish ./
EXPOSE 8080
# Render Ç™ PORT ÇìnÇ∑ÅBñ≥ÇØÇÍÇŒ 8080 Ç≈ë“ÇøéÛÇØ
ENV PORT=8080
ENTRYPOINT ["dotnet", "aspnet-booklog.dll"]
