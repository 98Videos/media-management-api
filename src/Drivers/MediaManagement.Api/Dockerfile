FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Drivers/MediaManagement.Api/MediaManagement.Api.csproj", "src/Drivers/MediaManagement.Api/"]
COPY ["src/Core/MediaManagementApi.Domain/MediaManagementApi.Domain.csproj", "src/Core/MediaManagementApi.Domain/"]
COPY ["src/Core/MediaManagement.Application/MediaManagement.Application.csproj", "src/Core/MediaManagement.Application/"]
RUN dotnet restore "src/Drivers/MediaManagement.Api/MediaManagement.Api.csproj"
COPY . .
WORKDIR "/src/src/Drivers/MediaManagement.Api"
RUN dotnet build "MediaManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MediaManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MediaManagement.Api.dll"]
