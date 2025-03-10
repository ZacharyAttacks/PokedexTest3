
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080 


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Pokedex.csproj .
RUN dotnet restore "Pokedex.csproj"
COPY . .
RUN dotnet build "Pokedex.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "Pokedex.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY Pokedex.db /app/Pokedex.db 
ENTRYPOINT ["dotnet", "Pokedex.dll"]
