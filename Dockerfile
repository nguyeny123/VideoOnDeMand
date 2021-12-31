#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["VideoOnDemand.UI/VideoOnDemand.UI.csproj", "VideoOnDemand.UI/"]
COPY ["VideoOnDemand.Admin/VideoOnDemand.Admin.csproj", "VideoOnDemand.Admin/"]
COPY ["VideoOnDemand.Data/VideoOnDemand.Data.csproj", "VideoOnDemand.Data/"]
RUN dotnet restore "VideoOnDemand.UI/VideoOnDemand.UI.csproj"
COPY . .
WORKDIR "/src/VideoOnDemand.UI"
RUN dotnet build "VideoOnDemand.UI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VideoOnDemand.UI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "VideoOnDemand.UI.dll"]
CMD ASPNETCORE_URLS=http://*:$PORT dotnet VideoOnDemand.UI.dll