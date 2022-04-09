FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app 
#
# copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore 
WORKDIR /app/POSHWeb
RUN dotnet publish -c Release -o /app/publish
COPY Samples/. /app/publish/Scripts/.
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app 
EXPOSE 80
EXPOSE 443

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "POSHWeb.dll"]