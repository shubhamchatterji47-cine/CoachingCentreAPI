# ═══════════════════════════════════════
# Stage 1 — BUILD
# ═══════════════════════════════════════
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj first
COPY CoachingAPI.csproj ./

# ✅ Restore packages
RUN dotnet restore "CoachingAPI.csproj"

# Copy all source
COPY . .

# ✅ Remove --no-restore flag (let it restore again to be safe)
RUN dotnet publish "CoachingAPI.csproj" \
    -c Release \
    -o /app/publish

# ═══════════════════════════════════════
# Stage 2 — RUNTIME
# ═══════════════════════════════════════
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 10000
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "CoachingAPI.dll"]
