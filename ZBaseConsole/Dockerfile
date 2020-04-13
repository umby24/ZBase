#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ZBaseConsole/ZBaseConsole.csproj", "ZBaseConsole/"]
COPY ["ZBase/ZBase.csproj", "ZBase/"]
COPY ["ClientSocketCore/ClientSocketCore.csproj", "ClientSocketCore/"]
COPY ["fNbtCore/fNbtCore.csproj", "fNbtCore/"]
COPY ["ClassicWorldCore/ClassicWorldCore.csproj", "ClassicWorldCore/"]
RUN dotnet restore "ZBaseConsole/ZBaseConsole.csproj"
COPY . .
WORKDIR "/src/ZBaseConsole"
RUN dotnet build "ZBaseConsole.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ZBaseConsole.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZBaseConsole.dll"]