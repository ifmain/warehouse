FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["warehouse.Blazor/warehouse.Blazor.csproj", "warehouse.Blazor/"]
COPY ["warehouse.Shared/warehouse.Shared.csproj", "warehouse.Shared/"]
RUN dotnet restore "warehouse.Blazor/warehouse.Blazor.csproj"

COPY . .
WORKDIR "/src/warehouse.Blazor"
RUN dotnet publish "warehouse.Blazor.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

RUN rm -rf ./*

COPY --from=build /app/publish/wwwroot .

COPY warehouse.Blazor/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80