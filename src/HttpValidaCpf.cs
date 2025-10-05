using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Learn.Function;

public class HttpValidaCpf
{
    private readonly ILogger<HttpValidaCpf> _logger;

    public HttpValidaCpf(ILogger<HttpValidaCpf> logger)
    {
        _logger = logger;
    }

    [Function("HttpValidaCpf")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        _logger.LogInformation("Iniciando a validação do CPF.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic? data = JsonConvert.DeserializeObject(requestBody);

        if (data == null && string.IsNullOrEmpty((string?)data?.cpf))
        {
            _logger.LogWarning("Requisição inválida: corpo ou CPF ausente.");
            var errorResponse = new
            {
                success = false,
                message = "Por favor, forneça um CPF no corpo da requisição.",
                cpf = (string?)null
            };
            return new BadRequestObjectResult(JsonConvert.SerializeObject(errorResponse));
        }

        string? cpf = data?.cpf;
        _logger.LogInformation($"CPF recebido: {cpf}");

        if (ValidaCpf(cpf))
        {
            _logger.LogInformation("CPF válido.");
            var successResponse = new
            {
                success = true,
                message = "CPF válido",
                cpf
            };
            return new OkObjectResult(JsonConvert.SerializeObject(successResponse));
        }
        
        _logger.LogInformation("CPF inválido.");
        var failResponse = new
        {
            success = false,
            message = "CPF inválido",
            cpf = cpf
        };
        return new BadRequestObjectResult(JsonConvert.SerializeObject(failResponse));
    }

    public static bool ValidaCpf(string? cpf)
    {
        if (string.IsNullOrEmpty(cpf))
            return false;

        cpf = cpf.Replace(".", "").Replace("-", "");

        if (cpf.Length != 11 || !long.TryParse(cpf, out _))
            return false;

        int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCpf = cpf.Substring(0, 9);
        int soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        int resto = soma % 11;
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;

        string digito = resto.ToString();
        tempCpf += digito;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;

        digito += resto.ToString();

        return cpf.EndsWith(digito);
    }
}