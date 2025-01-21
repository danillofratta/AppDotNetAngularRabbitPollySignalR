# Imagem base para execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Imagem de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar os arquivos .csproj para o contêiner
COPY ["ApiSale/ApiSale.csproj", "ApiSale/"]
COPY ["SharedDatabase/SharedDatabase.csproj", "SharedDatabase/"]
COPY ["SharedRabbitMq/SharedRabbitMq.csproj", "SharedRabbitMq/"]

# Restaurar as dependências
RUN dotnet restore "ApiSale/ApiSale.csproj"

# Copiar o restante dos arquivos para o contêiner
COPY ApiSale/ ApiSale/
COPY SharedDatabase/ SharedDatabase/
COPY SharedRabbitMq/ SharedRabbitMq/

# Compilar o projeto
WORKDIR "/src/ApiSale"
RUN dotnet build -c Release -o /app/build

# Publicar o aplicativo
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Imagem final para execução
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiSale.dll"]
