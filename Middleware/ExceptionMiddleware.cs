using System.Net;
using System.Text.Json;
using StudyToTech.Api.DTOs;
using StudyToTech.Api.Helpers;

namespace StudyToTech.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApiException ex)
        {
            await WriteError(context, ex.StatusCode, ex.Codigo, ex.Message, ex.Detalhes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado");
            await WriteError(context, 500, "ERRO_INTERNO", "Ocorreu um erro interno.", null);
        }
    }

    private static async Task WriteError(HttpContext ctx, int status, string codigo, string mensagem, object? detalhes)
    {
        ctx.Response.ContentType = "application/json";
        ctx.Response.StatusCode = status;

        var body = new ErroDto(codigo, mensagem, detalhes, DateTime.UtcNow);
        var json = JsonSerializer.Serialize(body, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        await ctx.Response.WriteAsync(json);
    }
}
