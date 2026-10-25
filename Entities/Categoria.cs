namespace StudyToTech.Api.Entities;

public class Categoria
{
    public int IdCategoria { get; set; }
    public string Nome { get; set; } = string.Empty;

    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
}
