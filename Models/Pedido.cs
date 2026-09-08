namespace StudyToTech.Models;

public class Pedido
{
    public int IdPedido { get; set; }
    public int UsuarioIdUsuario { get; set; }
    public int StatusIdStatusPedido { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataFinalizacao { get; set; }
}