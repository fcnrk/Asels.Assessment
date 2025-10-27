FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Platform/Asels.Assessment.Platform.WebApi/Asels.Assessment.Platform.WebApi.csproj", "src/Platform/Asels.Assessment.Platform.WebApi/"]
RUN dotnet restore "src/Platform/Asels.Assessment.Platform.WebApi/Asels.Assessment.Platform.WebApi.csproj"
COPY . .
WORKDIR "/src/src/Platform/Asels.Assessment.Platform.WebApi"
RUN dotnet build "./Asels.Assessment.Platform.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Asels.Assessment.Platform.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Asels.Assessment.Platform.WebApi.dll"]
