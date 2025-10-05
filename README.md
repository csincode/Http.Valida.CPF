# 🧾 Azure Function — HttpValidaCpf

O projeto **HttpValidaCpf** é uma **Azure Function** desenvolvida em **.NET (C#)** que tem como objetivo **validar números de CPF** enviados via requisição HTTP.

A função expõe um **endpoint HTTP POST**, que recebe um corpo JSON contendo o campo `cpf`, executa a validação seguindo as **regras oficiais de cálculo dos dígitos verificadores** e retorna uma resposta estruturada indicando se o CPF é válido ou não.

---

## ⚙️ Funcionamento

1. A função é acionada via **HTTP Trigger** (`AuthorizationLevel.Function`).
2. O corpo da requisição deve conter um JSON com o campo `cpf`, por exemplo:

   ```json
   { "cpf": "12345678909" }
   ```

3. O código valida o formato, remove caracteres especiais (`.`, `-`), e aplica o **algoritmo de validação de CPF**.
4. Retorna um JSON de resposta:

   ✅ **CPF válido**
   ```json
   {
     "success": true,
     "message": "CPF válido",
     "cpf": "12345678909"
   }
   ```

   ❌ **CPF inválido**
   ```json
   {
     "success": false,
     "message": "CPF inválido",
     "cpf": "12345678909"
   }
   ```

   ⚠️ **Erro de requisição**
   ```json
   {
     "success": false,
     "message": "Por favor, forneça um CPF no corpo da requisição.",
     "cpf": null
   }
   ```

---

## 🧮 Lógica de Validação

A função `ValidaCpf()` realiza as seguintes etapas:

- Remove pontos e traços;
- Verifica se há **11 dígitos numéricos**;
- Calcula os **dois dígitos verificadores** conforme a regra oficial da Receita Federal;
- Retorna `true` se o CPF for válido e `false` caso contrário.

---

## 🧰 Stack Técnica

| Componente | Descrição |
|-------------|------------|
| **Linguagem** | C# (.NET 8 ou superior) |
| **Plataforma** | Azure Functions (Isolated Worker Model) |
| **Bibliotecas** | `Microsoft.Azure.Functions.Worker`, `Microsoft.Extensions.Logging`, `Newtonsoft.Json` |

---

## 🧩 Finalidade

Este projeto é útil em cenários de:

- **Validação de cadastros** (APIs de onboarding, CRM, ERP);
- **Serviços de integração** que precisam validar CPFs antes de persistir dados;
- **Funções serverless** para verificações leves e escaláveis na nuvem.

---

## 🚀 Exemplo de Uso (via cURL)

```bash
curl -X POST https://<sua-function-app>.azurewebsites.net/api/HttpValidaCpf      -H "Content-Type: application/json"      -d "{\"cpf\":\"12345678909\"}"
```

---

## 📝 Exemplo de Código Simplificado

```csharp
[Function("HttpValidaCpf")]
public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
{
    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic data = JsonConvert.DeserializeObject(requestBody);

    string cpf = data?.cpf;
    if (string.IsNullOrEmpty(cpf))
        return new BadRequestObjectResult("Por favor, forneça um CPF.");

    bool valido = ValidaCpf(cpf);
    return valido
        ? new OkObjectResult("CPF válido")
        : new BadRequestObjectResult("CPF inválido");
}
```

---

## 📜 Licença

Este projeto é distribuído sob a licença **MIT**.  
Sinta-se à vontade para usar, modificar e distribuir conforme necessário.
