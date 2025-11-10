# =========================================
# 🟢 BASE (pour PROD)
# =========================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
#USER $APP_UID
WORKDIR /app
EXPOSE 7000
EXPOSE 7001

# Installer FFmpeg si besoin (root temporaire)
USER root
RUN apt-get update && \
    apt-get install -y ffmpeg && \
    rm -rf /var/lib/apt/lists/*
USER appuser

# =========================================
# 🔨 BUILD (pour PROD)
# =========================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/BambaIba.Api/BambaIba.Api.csproj", "src/BambaIba.Api/"]
COPY ["src/BambaIba.Application/BambaIba.Application.csproj", "src/BambaIba.Application/"]
COPY ["src/BambaIba.Domain/BambaIba.Domain.csproj", "src/BambaIba.Domain/"]
COPY ["src/BambaIba.Infrastructure/BambaIba.Infrastructure.csproj", "src/BambaIba.Infrastructure/"]
COPY ["src/BambaIba.SharedKernel/BambaIba.SharedKernel.csproj", "src/BambaIba.SharedKernel/"]

RUN dotnet restore "./src/BambaIba.Api/BambaIba.Api.csproj"

COPY . .
WORKDIR "/src/src/BambaIba.Api"
RUN dotnet build "./BambaIba.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# =========================================
# 📦 PUBLISH (pour PROD)
# =========================================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BambaIba.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# =========================================
# 🟡 DEV (Hot Reload)
# =========================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS dev
WORKDIR /app

# Installer FFmpeg si nécessaire
USER root
RUN apt-get update && \
    apt-get install -y ffmpeg && \
    rm -rf /var/lib/apt/lists/*

# Copier uniquement le csproj pour restore rapide
COPY ["src/BambaIba.Api/BambaIba.Api.csproj", "./"]
RUN dotnet restore "BambaIba.Api.csproj"

# Exposer les ports pour Docker
EXPOSE 7000
EXPOSE 7001

# Écoute sur toutes les interfaces
ENV ASPNETCORE_URLS="http://+:7000;https://+:7001"

# Pour DEV, utiliser root pour éviter les problèmes avec volumes montés
USER root

# Hot Reload : commande définie par docker-compose.override.yml
# ENTRYPOINT optionnel, on peut mettre dans override
# ENTRYPOINT ["dotnet", "watch", "run", "--project", "BambaIba.Api.csproj"]

# =========================================
# 🔴 FINAL (PROD)
# =========================================
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Créer un utilisateur non-root pour PROD
RUN useradd -m appuser
USER appuser

ENTRYPOINT ["dotnet", "BambaIba.Api.dll"]
