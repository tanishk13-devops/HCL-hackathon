# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FoodDeliveryAPI.csproj", "."]
RUN dotnet restore "FoodDeliveryAPI.csproj"

COPY . .
RUN dotnet build "FoodDeliveryAPI.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "FoodDeliveryAPI.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 7001
EXPOSE 80

ENTRYPOINT ["dotnet", "FoodDeliveryAPI.dll"]
