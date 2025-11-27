# ----------------------------------------------------
# FAZA 1: BUILD - Utilizează imaginea .NET SDK pentru a compila proiectul
# ----------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Modificarea 1: Copiază fișierul de proiect direct din directorul curent (.).
# Calea de destinație este punctul (.) pentru a-l pune direct în WORKDIR /src
COPY CampusEats.Frontend.csproj . 
RUN dotnet restore

# Copiază restul fișierelor și construiește aplicația
COPY . .
RUN dotnet publish -c Release -o /app/publish

# ----------------------------------------------------
# FAZA 2: FINAL - Utilizează imaginea de runtime mai mică pentru a rula aplicația
# ----------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Copiază fișierele publicate din faza de build
COPY --from=build /app/publish .

EXPOSE 8080

# Modificarea 2: Numele DLL-ului este cel al proiectului
ENTRYPOINT ["dotnet", "CampusEats.Frontend.dll"]