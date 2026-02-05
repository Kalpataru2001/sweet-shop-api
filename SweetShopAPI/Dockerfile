# 1. Build Phase
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SweetShopAPI.csproj", "./"]
RUN dotnet restore "SweetShopAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "SweetShopAPI.csproj" -c Release -o /app/build
RUN dotnet publish "SweetShopAPI.csproj" -c Release -o /app/publish

# 2. Run Phase
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SweetShopAPI.dll"]
