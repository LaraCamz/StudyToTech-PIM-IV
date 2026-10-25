using System.ComponentModel.DataAnnotations;

namespace StudyToTech.Api.DTOs;

public record EstoqueItemDto(
    int ProdutoId,
    string Nome,
    string Categoria,
    int Estoque,
    int EstoqueMinimo,
    bool AbaixoDoMinimo,
    bool Ativo
);

public class MovimentacaoCreate
{
    [Required]
    public int ProdutoId { get; set; }

    [Required]
    public string Tipo { get; set; } = string.Empty; // ENTRADA | SAIDA

    [Required, Range(1, int.MaxValue)]
    public int Quantidade { get; set; }
}

public record MovimentacaoDto(
    int Id,
    object Produto,
    string Tipo,
    int Quantidade,
    DateTime DataHora,
    object Usuario,
    int? SaldoAtual
);
