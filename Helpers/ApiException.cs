namespace StudyToTech.Api.Helpers;

public class ApiException : Exception
{
    public string Codigo { get; }
    public int StatusCode { get; }
    public object? Detalhes { get; }

    public ApiException(string codigo, string mensagem, int statusCode = 400, object? detalhes = null)
        : base(mensagem)
    {
        Codigo = codigo;
        StatusCode = statusCode;
        Detalhes = detalhes;
    }
}
