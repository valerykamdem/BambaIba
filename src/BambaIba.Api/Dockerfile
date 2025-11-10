# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
#USER $APP_UID
USER root
WORKDIR /app
EXPOSE 7000
#EXPOSE 7001

# ✅ Installation de FFmpeg
RUN apt-get update && \
    apt-get install -y ffmpeg && \
    rm -rf /var/lib/apt/lists/*

# Vérifier que FFmpeg est installé (optionnel, pour debug)
RUN which ffmpeg && ffmpeg -version

# ✅ Revenir à l'utilisateur non-root pour la sécurité
USER $APP_UID

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/BambaIba.Api/BambaIba.Api.csproj", "src/BambaIba.Api/"]
COPY ["src/BambaIba.Api/BambaIba.Api.csproj", "src/BambaIba.Api/"]
COPY ["src/BambaIba.Application/BambaIba.Application.csproj", "src/BambaIba.Application/"]
COPY ["src/BambaIba.Domain/BambaIba.Domain.csproj", "src/BambaIba.Domain/"]
COPY ["src/BambaIba.Infrastructure/BambaIba.Infrastructure.csproj", "src/BambaIba.Infrastructure/"]
COPY ["src/BambaIba.SharedKernel/BambaIba.SharedKernel.csproj", "src/BambaIba.SharedKernel/"]

RUN dotnet restore "./src/BambaIba.Api/BambaIba.Api.csproj"
COPY . .
WORKDIR "/src/src/BambaIba.Api"
RUN dotnet build "./BambaIba.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BambaIba.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BambaIba.Api.dll"]


## ============================================================
## 🟢 BASE (Runtime - PROD)
## ============================================================
#FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
#WORKDIR /app
#EXPOSE 7000
#EXPOSE 7001
#
## ✅ Créer un utilisateur non-root proprement
#RUN useradd -m -s /bin/bash appuser
#
## ✅ Installer FFmpeg pour le runtime
#RUN apt-get update && \
    #apt-get install -y ffmpeg && \
    #rm -rf /var/lib/apt/lists/*
#
#USER appuser
#
#
## ============================================================
## 🔨 BUILD (Compilation)
## ============================================================
#FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#
## ✅ Copier uniquement les fichiers projet pour un restore plus rapide
#COPY ["src/BambaIba.Api/BambaIba.Api.csproj", "src/BambaIba.Api/"]
#COPY ["src/BambaIba.Application/BambaIba.Application.csproj", "src/BambaIba.Application/"]
#COPY ["src/BambaIba.Domain/BambaIba.Domain.csproj", "src/BambaIba.Domain/"]
#COPY ["src/BambaIba.Infrastructure/BambaIba.Infrastructure.csproj", "src/BambaIba.Infrastructure/"]
#COPY ["src/BambaIba.SharedKernel/BambaIba.SharedKernel.csproj", "src/BambaIba.SharedKernel/"]
#
## ✅ Restore des dépendances
#RUN dotnet restore "./src/BambaIba.Api/./BambaIba.Api.csproj"
#
## ✅ Copier le reste du code source
#COPY . .
#
## ✅ Compiler
#WORKDIR "/src/src/BambaIba.Api"
#RUN dotnet build "BambaIba.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
#
## ============================================================
## 📦 PUBLISH (Publication - PROD)
## ============================================================
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "BambaIba.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
#
## ============================================================
## 🧑‍💻 DEV (Hot Reload)
## ============================================================
#FROM mcr.microsoft.com/dotnet/sdk:9.0 AS dev
#WORKDIR /src
#
## ✅ Installer FFmpeg aussi en mode DEV
#RUN apt-get update && \
    #apt-get install -y ffmpeg && \
    #rm -rf /var/lib/apt/lists/*
#
## ✅ Copier uniquement les projets pour restore rapide
#COPY ["src/BambaIba.Api/BambaIba.Api.csproj", "src/BambaIba.Api/"]
#COPY ["src/BambaIba.Application/BambaIba.Application.csproj", "src/BambaIba.Application/"]
#COPY ["src/BambaIba.Domain/BambaIba.Domain.csproj", "src/BambaIba.Domain/"]
#COPY ["src/BambaIba.Infrastructure/BambaIba.Infrastructure.csproj", "src/BambaIba.Infrastructure/"]
#COPY ["src/BambaIba.SharedKernel/BambaIba.SharedKernel.csproj", "src/BambaIba.SharedKernel/"]
#
## ✅ Restore
#RUN dotnet restore "./src/BambaIba.Api/./BambaIba.Api.csproj"
#
## ✅ Copier le code complet
#COPY . .
#
## ✅ Créer utilisateur non-root
#RUN useradd -m -s /bin/bash appuser
#USER appuser
#
#EXPOSE 7000
#
#ENV ASPNETCORE_ENVIRONMENT=Development
#ENV DOTNET_RUNNING_IN_CONTAINER=true
#ENV DOTNET_USE_POLLING_FILE_WATCHER=true
#ENV ASPNETCORE_URLS=http://+:7000
#
## ✅ Commande de démarrage pour hot reload
#WORKDIR /src/src/BambaIba.Api
#ENTRYPOINT ["dotnet", "watch", "run", "--non-interactive", "--project", "src/BambaIba.Api/BambaIba.Api.csproj"]
#
#
## ============================================================
## 🚀 FINAL (Image de production)
## ============================================================
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#
## ✅ Assurer la présence de FFmpeg
#RUN which ffmpeg && ffmpeg -version
#
#ENTRYPOINT ["dotnet", "BambaIba.Api.dll"]
#