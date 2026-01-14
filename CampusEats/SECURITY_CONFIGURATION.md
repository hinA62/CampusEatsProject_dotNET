# Configurare Securitate JWT & Secrets

## ⚠️ IMPORTANT: Această configurație rezolvă vulnerabilitățile de securitate

### Problema identificată:
- **CRITICAL**: Cheia JWT era hardcodată în `appsettings.json` (comis în Git)
- **CRITICAL**: Secret-uri Stripe expuse în repository

### Soluția implementată:

## 1. Development (Local)

### Setup inițial:
1. Copiază fișierul template:
   ```bash
   cp appsettings.Development.json.example appsettings.Development.json
   ```

2. Editează `appsettings.Development.json` cu cheile tale locale:
   ```json
   {
     "Jwt": {
       "Key": "DEV_ONLY_KEY_DO_NOT_USE_IN_PRODUCTION_[minim_32_caractere]"
     },
     "Stripe": {
       "SecretKey": "sk_test_...",
       "PublishableKey": "pk_test_...",
       "WebhookSecret": "whsec_..."
     }
   }
   ```

⚠️ **NOTĂ**: Fișierul `appsettings.Development.json` este acum în `.gitignore` și NU va fi comis în Git.

## 2. Production (Docker/Cloud)

### Opțiunea A: Environment Variables

Setează următoarele variabile de mediu:

```bash
# JWT Configuration
export Jwt__Key="your-super-secure-production-key-min-32-chars"
export Jwt__Issuer="CampusEats"
export Jwt__Audience="CampusEatsClients"
export Jwt__ExpiryMinutes="60"

# Stripe Configuration
export Stripe__SecretKey="sk_live_..."
export Stripe__PublishableKey="pk_live_..."
export Stripe__WebhookSecret="whsec_..."
export Stripe__ClientBaseUrl="https://your-production-domain.com"

# Database
export ConnectionStrings__DefaultConnection="Host=db;Database=CampusEatsDB;Username=postgres;Password=SECURE_PASSWORD"
```

### Opțiunea B: Docker Secrets (Recomandat)

În `docker-compose.yml`:
```yaml
services:
  app:
    environment:
      - Jwt__Key=${JWT_KEY}
      - Stripe__SecretKey=${STRIPE_SECRET_KEY}
    env_file:
      - .env.production  # Nu comite acest fișier!
```

Creează `.env.production`:
```
JWT_KEY=your-production-key-here
STRIPE_SECRET_KEY=sk_live_...
```

### Opțiunea C: Azure/AWS Secrets Manager

Pentru Azure App Service:
```bash
az webapp config appsettings set --name YourAppName \
  --resource-group YourResourceGroup \
  --settings Jwt__Key="your-key" Stripe__SecretKey="sk_live_..."
```

## 3. Validare

Codul în `JwtService.cs` validează automat:
```csharp
var jwtKey = config["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key is not configured!");
}
```

Aplicația **NU VA PORNI** dacă cheia JWT nu este configurată corect.

## 4. Generare Cheie Securizată

Pentru producție, generează o cheie criptografic securizată:

### PowerShell (Windows):
```powershell
[Convert]::ToBase64String((1..64 | ForEach-Object { Get-Random -Minimum 0 -Maximum 256 }))
```

### Bash (Linux/Mac):
```bash
openssl rand -base64 64
```

### Online (folosește cu atenție):
- https://www.random.org/strings/ (64 caractere, alfanumeric)

## 5. Checklist Securitate

- [x] `appsettings.json` - Doar placeholders (Key: "")
- [x] `appsettings.Development.json` - În .gitignore
- [x] `JwtService.cs` - Validare că key-ul există
- [x] Environment Variables - Pentru producție
- [ ] Rotate keys periodic (recomandat la 90 zile)
- [ ] Monitorizare accese neautorizate

## 6. Ce NU trebuie să faci niciodată:

❌ Commit `appsettings.Development.json` în Git
❌ Hardcode chei în cod
❌ Share chei de producție pe Slack/Email
❌ Folosește chei de development în producție
❌ Expune chei în logs

## 7. Dacă ai comis deja secret-uri în Git:

```bash
# 1. Regenerează TOATE cheile (JWT, Stripe, etc.)
# 2. Șterge istoricul Git (folosește BFG Repo-Cleaner)
git filter-branch --force --index-filter \
  "git rm --cached --ignore-unmatch appsettings.Development.json" \
  --prune-empty --tag-name-filter cat -- --all

# 3. Force push (ATENȚIE!)
git push origin --force --all
```

## 8. Testing

Testează că aplicația pornește corect:

### Development:
```bash
dotnet run --environment Development
# Ar trebui să vadă în logs: "JWT Key loaded from: appsettings.Development.json"
```

### Production simulation:
```bash
export Jwt__Key="test-production-key-1234567890123456789012345678901234567890"
dotnet run --environment Production
# Ar trebui să vadă: "JWT Key loaded from: Environment Variable or Production Config"
```

---

## Support

Pentru întrebări despre securitate, contactează echipa DevOps.

