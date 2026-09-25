# NoPrumo

Sistema de Gestão de Obras para Pequenas Empresas de Construção.

## Configuração do Ambiente de Desenvolvimento

### 1. Configurar User Secrets da API (`back/01-Presentation`)

A API necessita de chaves criptográficas (AES-256-GCM e HMAC-SHA-256) registradas em User Secrets para cifrar e gerar hashes de CPF/CNPJ de forma segura.

Execute os comandos a seguir no terminal (PowerShell) a partir da raiz do projeto para gerar e definir as chaves no ambiente local:

```powershell
# Navegar até a pasta da API se necessário, ou especificar --project
cd back/01-Presentation

# 1. Gerar e configurar a chave de criptografia de documentos (32 bytes em Base64)
$encKey = [Convert]::ToBase64String((1..32 | ForEach-Object { [byte](Get-Random -Max 256) }))
dotnet user-secrets set "Documents:EncryptionKey" $encKey

# 2. Gerar e configurar a chave HMAC de documentos (32 bytes em Base64)
$hmacKey = [Convert]::ToBase64String((1..32 | ForEach-Object { [byte](Get-Random -Max 256) }))
dotnet user-secrets set "Documents:HmacKey" $hmacKey
```

> **Nota:** Certifique-se de ter configurado também a string de conexão (`ConnectionStrings:DefaultConnection`) e as configurações de JWT (`Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key`) nos seus User Secrets locais.