FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["LLS.API/LLS.API.csproj", "LLS.API/"]
COPY ["LLS.DAL/LLS.DAL.csproj", "LLS.DAL/"]
COPY ["LLS.Common/LLS.Common.csproj", "LLS.Common/"]
COPY ["LLS.BLL/LLS.BLL.csproj", "LLS.BLL/"]
RUN dotnet restore "LLS.API/LLS.API.csproj"
COPY . .
WORKDIR "/src/LLS.API"
RUN dotnet build "LLS.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LLS.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet LLS.API.dll