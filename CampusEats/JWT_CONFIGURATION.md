# Configurarea JWT Key

## ⚠️ IMPORTANT - SECURITATE

Cheia JWT **NU trebuie** să fie hardcodată în `appsettings.json` care este commitată în Git!

## Development

Pentru development local, cheia este configurată în `appsettings.Development.json`:
```json
"Jwt": {
  "Key": "DEV_ONLY_KEY_DO_NOT_USE_IN_PRODUCTION_1234567890123456789012345678901234567890"
}
```

⚠️ Această cheie este DOAR pentru development și **NU** trebuie folosită în producție!

## Producție

### Opțiune 1: Environment Variables (recomandat)

Setează environment variable-ul:
```bash
export Jwt__Key="your-secure-production-key-here"
```

Sau pentru Windows:
```powershell
$env:Jwt__Key="your-secure-production-key-here"
```

### Opțiune 2: Azure App Service

În Azure Portal:
1. Mergi la App Service → Configuration → Application settings
2. Adaugă un nou setting:
   - **Name:** `Jwt:Key` sau `Jwt__Key`
   - **Value:** cheia ta sigură de producție

### Opțiune 3: Docker

În `docker-compose.yml`:
```yaml
environment:
  - Jwt__Key=${JWT_KEY}
```

Apoi creează un fișier `.env`:
```
JWT_KEY=your-secure-production-key-here
```

⚠️ **NU commita fișierul `.env` în Git!** Adaugă-l în `.gitignore`

## Generarea unei chei sigure

Pentru a genera o cheie JWT sigură, poți folosi:

### PowerShell:
```powershell
$bytes = New-Object byte[] 64
[System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
[Convert]::ToBase64String($bytes)
```

### Bash/Linux:
```bash
openssl rand -base64 64
```

### Online:
Sau folosește: https://generate-random.org/api-key-generator (64 caractere)

## Verificare

Aplicația va arunca o excepție `InvalidOperationException` dacă cheia JWT nu este configurată:
```
JWT Key is not configured! Please set Jwt:Key in appsettings or as environment variable.
```

Acest mecanism de validare previne pornirea accidentală a aplicației fără o cheie configurată.

## Checklist Securitate

- [ ] `appsettings.json` are `"Key": ""` (gol)
- [ ] `appsettings.Development.json` are o cheie pentru development
- [ ] Cheia de producție este setată ca environment variable
- [ ] Cheia de producție are minim 64 caractere aleatorii
- [ ] Cheia de producție NU este commitată în Git
- [ ] `.env` fișierul este în `.gitignore`

