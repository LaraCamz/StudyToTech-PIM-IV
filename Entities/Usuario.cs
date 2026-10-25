namespace StudyToTech.Api.Entities;

public class Usuario
{
    public int IdUsuario { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public bool Status { get; set; } = true; // true = ativo
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    public int PerfilIdPerfil { get; set; }

    public Perfil Perfil { get; set; } = null!;
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    public ICollection<MovimentacaoEstoque> Movimentacoes { get; set; } = new List<MovimentacaoEstoque>();
}
