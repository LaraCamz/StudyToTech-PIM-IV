namespace StudyToTech.Models;

public class Usuario
{
    public int IdUsuario { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public string SenhaHash { get; set; } = "";
    public bool Status { get; set; }
    public DateTime DataCadastro { get; set; }
    public int PerfilIdPerfil { get; set; }
}