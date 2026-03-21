FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5001
ENV ASPNETCORE_URLS=http://+:5001

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Library.Api/Library.Api.csproj", "src/Library.Api/"]
RUN dotnet restore "./src/Library.Api/Library.Api.csproj"
COPY . .
WORKDIR "/src/src/Library.Api"
RUN dotnet build "./Library.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Library.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Library.Api.dll"]