namespace StudyToTech.Api.DTOs;

public record ErroDto(
    string Codigo,
    string Mensagem,
    object? Detalhes,
    DateTime Timestamp
);

public record PaginaDto<T>(
    int Pagina,
    int Tamanho,
    long TotalElementos,
    int TotalPaginas,
    IEnumerable<T> Conteudo
);

public record AtivoRequest(bool Ativo);
