namespace StudyToTech.Models;

public class Produto
{
    public int IdProduto { get; set; }
    public string Nome { get; set; } = "";
    public string? Descricao { get; set; }
    public int CategoriaIdCategoria { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public int EstoqueMinimo { get; set; }
    public bool Status { get; set; }
    public DateTime DataCadastro { get; set; }
}