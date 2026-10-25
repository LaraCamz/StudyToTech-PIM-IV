namespace StudyToTech.Api.Entities;

public class Perfil
{
    public int IdPerfil { get; set; }
    public string Nome { get; set; } = string.Empty;

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
