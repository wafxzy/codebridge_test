FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Codebridge.api/Codebridge.API.csproj", "Codebridge.api/"]
COPY ["codebridge.common/codebridge.Common.csproj", "codebridge.common/"]
COPY ["codebridge.repository/codebridge.DAL.csproj", "codebridge.repository/"]
COPY ["codebridge.services/codebridge.BLL.csproj", "codebridge.services/"]

RUN dotnet restore "Codebridge.api/Codebridge.API.csproj"

COPY . .

WORKDIR "/src/Codebridge.api"
RUN dotnet build "Codebridge.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Codebridge.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "Codebridge.API.dll"]
