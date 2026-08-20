FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["WCS.sln", "./"]
COPY ["src/Wcs.Api/Wcs.Api.csproj", "src/Wcs.Api/"]
COPY ["src/Wcs.Cms/Wcs.Cms.csproj", "src/Wcs.Cms/"]
COPY ["src/Wcs.Common/Wcs.Common.csproj", "src/Wcs.Common/"]
COPY ["src/Wcs.Infrastructure/Wcs.Infrastructure.csproj", "src/Wcs.Infrastructure/"]
COPY ["src/Wcs.OpcUa/Wcs.OpcUa.csproj", "src/Wcs.OpcUa/"]
COPY ["src/Wcs.Rcs/Wcs.Rcs.csproj", "src/Wcs.Rcs/"]

RUN dotnet restore "src/Wcs.Api/Wcs.Api.csproj"

COPY . .
WORKDIR "/src/src/Wcs.Api"
RUN dotnet publish "Wcs.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Wcs.Api.dll"]

