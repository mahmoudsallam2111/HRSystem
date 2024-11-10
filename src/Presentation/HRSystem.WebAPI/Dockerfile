#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Presentation/HRSystem.WebAPI/HRSystem.WebAPI.csproj", "src/Presentation/HRSystem.WebAPI/"]
COPY ["src/Core/HRSystem.Application/HRSystem.Application.csproj", "src/Core/HRSystem.Application/"]
COPY ["src/Core/HRSystem.Domain/HRSystem.Domain.csproj", "src/Core/HRSystem.Domain/"]
COPY ["src/Infrastrucure/HRSystem.Infrastructure.Persistence/HRSystem.Infrastructure.Persistence.csproj", "src/Infrastrucure/HRSystem.Infrastructure.Persistence/"]
COPY ["src/BuildingBlocks/HRSystem.Common/HRSystem.Common.csproj", "src/BuildingBlocks/BuildingBlocks/"]
RUN dotnet restore "./src/Presentation/HRSystem.WebAPI/./HRSystem.WebAPI.csproj"
COPY . .
WORKDIR "/src/src/Presentation/HRSystem.WebAPI"
RUN dotnet build "./HRSystem.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HRSystem.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HRSystem.WebAPI.dll"]