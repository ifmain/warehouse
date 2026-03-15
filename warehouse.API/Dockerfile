FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src


COPY ["warehouse.API/warehouse.API.csproj", "warehouse.API/"]
COPY ["warehouse.Shared/warehouse.Shared.csproj", "warehouse.Shared/"]
RUN dotnet restore "warehouse.API/warehouse.API.csproj"


COPY . .
WORKDIR "/src/warehouse.API"
RUN dotnet publish "warehouse.API.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .


RUN mkdir -p /app/wwwroot/uploads

ENTRYPOINT ["dotnet", "warehouse.API.dll"]