FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Food_Delivery_API/Food_Delivery_API.csproj", "Food_Delivery_API/"]
COPY ["Clean.Application/Clean.Application.csproj", "Clean.Application/"]
COPY ["Clean.Domain/Clean.Domain.csproj", "Clean.Domain/"]
COPY ["Clean.Infrastructure/Clean.Infrastructure.csproj", "Clean.Infrastructure/"]
RUN dotnet restore "Food_Delivery_API/Food_Delivery_API.csproj"
COPY . .
WORKDIR "/src/Food_Delivery_API"
RUN dotnet build "./Food_Delivery_API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Food_Delivery_API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Food_Delivery_API.dll"]
