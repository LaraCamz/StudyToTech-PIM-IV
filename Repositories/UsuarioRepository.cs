using MySql.Data.MySqlClient;
using StudyToTech.Data;
using StudyToTech.Models;

namespace StudyToTech.Repositories;

public class UsuarioRepository
{
    private readonly Banco banco;

    public UsuarioRepository()
    {
        banco = new Banco();
    }

    public void CadastrarUsuario(Usuario usuario)
    {
        using MySqlConnection conexao = banco.CriarConexao();

        conexao.Open();

        string sql = """
            INSERT INTO usuario
            (
                nome,
                email,
                senha_hash,
                status,
                data_cadastro,
                perfil_idperfil
            )
            VALUES
            (
                @nome,
                @email,
                @senha,
                @status,
                @data_cadastro,
                @perfil
            );
            """;

        using MySqlCommand comando = new MySqlCommand(sql, conexao);

        comando.Parameters.AddWithValue("@nome", usuario.Nome);
        comando.Parameters.AddWithValue("@email", usuario.Email);
        comando.Parameters.AddWithValue("@senha", usuario.SenhaHash);
        comando.Parameters.AddWithValue("@status", usuario.Status);
        comando.Parameters.AddWithValue("@data_cadastro", usuario.DataCadastro);
        comando.Parameters.AddWithValue("@perfil", usuario.PerfilIdPerfil);

        comando.ExecuteNonQuery();
    }

    public List<Usuario> ListarUsuarios()
    {
        List<Usuario> usuarios = new();

        using MySqlConnection conexao = banco.CriarConexao();

        conexao.Open();

        string sql = """
            SELECT
                idusuario,
                nome,
                email,
                senha_hash,
                status,
                data_cadastro,
                perfil_idperfil
            FROM usuario;
            """;

        using MySqlCommand comando = new MySqlCommand(sql, conexao);

        using MySqlDataReader leitor = comando.ExecuteReader();

        while (leitor.Read())
        {
            Usuario usuario = new Usuario
            {
                IdUsuario = leitor.GetInt32("idusuario"),
                Nome = leitor.GetString("nome"),
                Email = leitor.GetString("email"),
                SenhaHash = leitor.GetString("senha_hash"),
                Status = Convert.ToBoolean(leitor["status"]),
                DataCadastro = leitor.GetDateTime("data_cadastro"),
                PerfilIdPerfil = leitor.GetInt32("perfil_idperfil")
            };

            usuarios.Add(usuario);
        }

        return usuarios;
    }

    public Usuario? BuscarUsuario(int id)
    {
        using MySqlConnection conexao = banco.CriarConexao();

        conexao.Open();

        string sql = """
            SELECT
                idusuario,
                nome,
                email,
                senha_hash,
                status,
                data_cadastro,
                perfil_idperfil
            FROM usuario
            WHERE idusuario = @id;
            """;

        using MySqlCommand comando = new MySqlCommand(sql, conexao);

        comando.Parameters.AddWithValue("@id", id);

        using MySqlDataReader leitor = comando.ExecuteReader();

        if (!leitor.Read())
        {
            return null;
        }

        return new Usuario
        {
            IdUsuario = leitor.GetInt32("idusuario"),
            Nome = leitor.GetString("nome"),
            Email = leitor.GetString("email"),
            SenhaHash = leitor.GetString("senha_hash"),
            Status = Convert.ToBoolean(leitor["status"]),
            DataCadastro = leitor.GetDateTime("data_cadastro"),
            PerfilIdPerfil = leitor.GetInt32("perfil_idperfil")
        };
    }
}