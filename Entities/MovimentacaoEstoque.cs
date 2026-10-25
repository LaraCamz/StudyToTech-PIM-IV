namespace StudyToTech.Api.Entities;

public class MovimentacaoEstoque
{
    public int IdMovimentacaoEstoque { get; set; }
    public int ProdutoIdProduto { get; set; }
    public int UsuarioIdUsuario { get; set; }
    public string Tipo { get; set; } = string.Empty; // ENTRADA | SAIDA
    public int Quantidade { get; set; }
    public DateTime DataHora { get; set; } = DateTime.UtcNow;

    public Produto Produto { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
