FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["PReq/PReq.csproj", "PReq/"]
RUN dotnet restore "PReq/PReq.csproj"
COPY . .
WORKDIR "/src/PReq"
RUN dotnet build "PReq.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "PReq.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "PReq.dll"]