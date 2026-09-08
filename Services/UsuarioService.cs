using StudyToTech.Models;
using StudyToTech.Repositories;

namespace StudyToTech.Services;

public class UsuarioService
{
    private readonly UsuarioRepository usuarioRepository;

    public UsuarioService()
    {
        usuarioRepository = new UsuarioRepository();
    }

    public void CadastrarUsuario()
    {
        Usuario usuario = new Usuario();

        Console.Write("Nome: ");
        usuario.Nome = Console.ReadLine()!;

        Console.Write("Email: ");
        usuario.Email = Console.ReadLine()!;

        Console.Write("Senha: ");
        usuario.SenhaHash = Console.ReadLine()!;

        Console.Write("ID do perfil: ");
        usuario.PerfilIdPerfil = int.Parse(Console.ReadLine()!);

        usuario.Status = true;
        usuario.DataCadastro = DateTime.Now;

        usuarioRepository.CadastrarUsuario(usuario);

        Console.WriteLine("Usuário cadastrado com sucesso!");
    }

    public void ListarUsuarios()
    {
        List<Usuario> usuarios = usuarioRepository.ListarUsuarios();

        if (usuarios.Count == 0)
        {
            Console.WriteLine("Nenhum usuário cadastrado.");
            return;
        }

        foreach (Usuario usuario in usuarios)
        {
            Console.WriteLine($"ID: {usuario.IdUsuario}");
            Console.WriteLine($"Nome: {usuario.Nome}");
            Console.WriteLine($"Email: {usuario.Email}");
            Console.WriteLine($"Perfil: {usuario.PerfilIdPerfil}");
            Console.WriteLine($"Status: {(usuario.Status ? "Ativo" : "Inativo")}");
            Console.WriteLine($"Data de cadastro: {usuario.DataCadastro}");
            Console.WriteLine("-----------------------------");
        }
    }

    public Usuario? BuscarUsuario(int id)
    {
        return usuarioRepository.BuscarUsuario(id);
    }

    public List<Usuario> ObterUsuarios()
    {
        return usuarioRepository.ListarUsuarios();
    }
}