using System.ComponentModel.DataAnnotations;

namespace StudyToTech.Api.DTOs;

public record CategoriaDto(int Id, string Nome);

public record ProdutoDto(
    int Id,
    string Nome,
    string? Descricao,
    CategoriaDto Categoria,
    string Preco,
    int Estoque,
    int? EstoqueMinimo,
    bool Disponivel,
    bool Ativo,
    DateTime DataCadastro
);

public class ProdutoCreate
{
    [Required, MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Descricao { get; set; }

    [Required]
    public int CategoriaId { get; set; }

    [Required]
    public string Preco { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int EstoqueInicial { get; set; } = 0;

    [Range(0, int.MaxValue)]
    public int EstoqueMinimo { get; set; } = 0;

    public bool Ativo { get; set; } = true;
}

public class ProdutoUpdate
{
    [Required, MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Descricao { get; set; }

    [Required]
    public int CategoriaId { get; set; }

    [Required]
    public string Preco { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int EstoqueMinimo { get; set; }
}
