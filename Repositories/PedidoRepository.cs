using MySql.Data.MySqlClient;
using StudyToTech.Data;
using StudyToTech.Models;

namespace StudyToTech.Repositories;

public class PedidoRepository
{
    private readonly Banco banco;

    public PedidoRepository()
    {
        banco = new Banco();
    }

    public void CriarPedido(Pedido pedido)
    {
        using MySqlConnection conexao = banco.CriarConexao();

        conexao.Open();

        string sql = """
            INSERT INTO pedido
            (
                usuario_idusuario,
                status_idstatus_pedido,
                data_criacao
            )
            VALUES
            (
                @usuario,
                (
                    SELECT idstatus_pedido
                    FROM status_pedido
                    WHERE nome = 'Em elaboração'
                ),
                @data_criacao
            );
            """;

        using MySqlCommand comando = new MySqlCommand(sql, conexao);

        comando.Parameters.AddWithValue("@usuario", pedido.UsuarioIdUsuario);
        comando.Parameters.AddWithValue("@data_criacao", pedido.DataCriacao);

        comando.ExecuteNonQuery();
    }

    public Pedido? BuscarPedido(int id)
    {
        using MySqlConnection conexao = banco.CriarConexao();

        conexao.Open();

        string sql = """
            SELECT
                idpedido,
                usuario_idusuario,
                status_idstatus_pedido,
                data_criacao,
                data_finalizacao
            FROM pedido
            WHERE idpedido = @id;
            """;

        using MySqlCommand comando = new MySqlCommand(sql, conexao);

        comando.Parameters.AddWithValue("@id", id);

        using MySqlDataReader leitor = comando.ExecuteReader();

        if (!leitor.Read())
        {
            return null;
        }

        return new Pedido
        {
            IdPedido = leitor.GetInt32("idpedido"),
            UsuarioIdUsuario = leitor.GetInt32("usuario_idusuario"),
            StatusIdStatusPedido = leitor.GetInt32("status_idstatus_pedido"),
            DataCriacao = leitor.GetDateTime("data_criacao"),
            DataFinalizacao = leitor.IsDBNull(leitor.GetOrdinal("data_finalizacao"))
                ? null
                : leitor.GetDateTime("data_finalizacao")
        };
    }

    public void FinalizarPedido(int id)
    {
        using MySqlConnection conexao = banco.CriarConexao();

        conexao.Open();

        string sql = """
            UPDATE pedido
            SET
                status_idstatus_pedido = (
                    SELECT idstatus_pedido
                    FROM status_pedido
                    WHERE nome = 'Finalizado'
                ),
                data_finalizacao = @data_finalizacao
            WHERE idpedido = @id;
            """;

        using MySqlCommand comando = new MySqlCommand(sql, conexao);

        comando.Parameters.AddWithValue("@id", id);
        comando.Parameters.AddWithValue("@data_finalizacao", DateTime.Now);

        comando.ExecuteNonQuery();
    }
}