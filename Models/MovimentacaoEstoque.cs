namespace StudyToTech.Models;

public class MovimentacaoEstoque
{
    public int IdMovimentacaoEstoque { get; set; }
    public int ProdutoIdProduto { get; set; }
    public int UsuarioIdUsuario { get; set; }
    public string Tipo { get; set; } = "";
    public int Quantidade { get; set; }
    public DateTime DataHora { get; set; }
}