#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["POSHWeb.Testing.Playground/POSHWeb.Testing.Playground.csproj", "POSHWeb.Testing.Playground/"]
RUN dotnet restore "POSHWeb.Testing.Playground/POSHWeb.Testing.Playground.csproj"
COPY . .
WORKDIR "/src/POSHWeb.Testing.Playground"
RUN dotnet build "POSHWeb.Testing.Playground.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "POSHWeb.Testing.Playground.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "POSHWeb.Testing.Playground.dll"]