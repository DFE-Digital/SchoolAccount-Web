# Build from the repository root: docker build -f tests/SchoolAccount.MockIdentityProvider/Dockerfile .
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY [".editorconfig", "."]
COPY ["tests/SchoolAccount.MockIdentityProvider/SchoolAccount.MockIdentityProvider.csproj", "tests/SchoolAccount.MockIdentityProvider/"]
RUN dotnet restore "tests/SchoolAccount.MockIdentityProvider/SchoolAccount.MockIdentityProvider.csproj"
COPY ["tests/SchoolAccount.MockIdentityProvider/", "tests/SchoolAccount.MockIdentityProvider/"]
RUN dotnet publish "tests/SchoolAccount.MockIdentityProvider/SchoolAccount.MockIdentityProvider.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SchoolAccount.MockIdentityProvider.dll"]
