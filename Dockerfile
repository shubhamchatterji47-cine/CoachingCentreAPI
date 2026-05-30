# ═══════════════════════════════════════
# Stage 1 — BUILD
# ═══════════════════════════════════════
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# ✅ Copy csproj first for better caching
COPY CoachingAPI.csproj ./
RUN dotnet restore "CoachingAPI.csproj"

# ✅ Copy everything else
COPY . .

# ✅ Publish
RUN dotnet publish "CoachingAPI.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# ═══════════════════════════════════════
# Stage 2 — RUNTIME
# ═══════════════════════════════════════
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# ✅ Render port
EXPOSE 10000
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "CoachingAPI.dll"]
