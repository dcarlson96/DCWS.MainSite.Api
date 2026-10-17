FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/DCWS.MainSite.Api.Web/DCWS.MainSite.Api.Web.csproj src/DCWS.MainSite.Api.Web/
COPY src/DCWS.MainSite.Api.Domain/DCWS.MainSite.Api.Domain.csproj src/DCWS.MainSite.Api.Domain/
RUN dotnet restore src/DCWS.MainSite.Api.Web/DCWS.MainSite.Api.Web.csproj
COPY . .
RUN dotnet publish src/DCWS.MainSite.Api.Web/DCWS.MainSite.Api.Web.csproj \
    --configuration Release --no-restore --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "DCWS.MainSite.Api.Web.dll"]
