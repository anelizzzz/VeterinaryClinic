FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/VeterinaryClinic.API
RUN dotnet restore "VeterinaryClinic.API.csproj"
RUN dotnet build "VeterinaryClinic.API.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR /src/VeterinaryClinic.API
RUN dotnet publish "VeterinaryClinic.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VeterinaryClinic.API.dll"]