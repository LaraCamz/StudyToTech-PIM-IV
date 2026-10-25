namespace StudyToTech.Api.Entities;

public class Produto
{
    public int IdProduto { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int CategoriaIdCategoria { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public int EstoqueMinimo { get; set; }
    public bool Status { get; set; } = true; // true = ativo
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public Categoria Categoria { get; set; } = null!;
    public ICollection<ItemPedido> ItensPedido { get; set; } = new List<ItemPedido>();
    public ICollection<MovimentacaoEstoque> Movimentacoes { get; set; } = new List<MovimentacaoEstoque>();
}
