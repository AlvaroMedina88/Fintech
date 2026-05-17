FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore Presentacion/FinTech.Api/FinTech.Api.csproj
RUN dotnet publish Presentacion/FinTech.Api/FinTech.Api.csproj --no-restore -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 10000
CMD ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-10000} ./FinTech.Api"]
