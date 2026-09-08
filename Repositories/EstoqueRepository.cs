using MySql.Data.MySqlClient;
using StudyToTech.Data;
using StudyToTech.Models;

namespace StudyToTech.Repositories;

public class EstoqueRepository
{
    private readonly Banco banco;

    public EstoqueRepository()
    {
        banco = new Banco();
    }

    public List<Produto> ConsultarEstoque()
    {
        List<Produto> produtos = new();

        using MySqlConnection conexao = banco.CriarConexao();

        conexao.Open();

        string sql = """
            SELECT
                idproduto,
                nome,
                estoque,
                estoque_minimo
            FROM produto;
            """;

        using MySqlCommand comando = new MySqlCommand(sql, conexao);

        using MySqlDataReader leitor = comando.ExecuteReader();

        while (leitor.Read())
        {
            Produto produto = new Produto
            {
                IdProduto = leitor.GetInt32("idproduto"),
                Nome = leitor.GetString("nome"),
                Estoque = leitor.GetInt32("estoque"),
                EstoqueMinimo = leitor.GetInt32("estoque_minimo")
            };

            produtos.Add(produto);
        }

        return produtos;
    }

    public void RegistrarEntrada(
        int produtoId,
        int usuarioId,
        int quantidade)
    {
        using MySqlConnection conexao = banco.CriarConexao();

        conexao.Open();

        using MySqlTransaction transacao = conexao.BeginTransaction();

        try
        {
            string atualizarEstoque = """
                UPDATE produto
                SET estoque = estoque + @quantidade
                WHERE idproduto = @produto;
                """;

            using MySqlCommand comandoEstoque =
                new MySqlCommand(atualizarEstoque, conexao, transacao);

            comandoEstoque.Parameters.AddWithValue("@quantidade", quantidade);
            comandoEstoque.Parameters.AddWithValue("@produto", produtoId);

            comandoEstoque.ExecuteNonQuery();

            string registrarMovimentacao = """
                INSERT INTO movimentacao_estoque
                (
                    produto_idproduto,
                    usuario_idusuario,
                    tipo,
                    quantidade,
                    data_hora
                )
                VALUES
                (
                    @produto,
                    @usuario,
                    'ENTRADA',
                    @quantidade,
                    @data_hora
                );
                """;

            using MySqlCommand comandoMovimentacao =
                new MySqlCommand(registrarMovimentacao, conexao, transacao);

            comandoMovimentacao.Parameters.AddWithValue("@produto", produtoId);
            comandoMovimentacao.Parameters.AddWithValue("@usuario", usuarioId);
            comandoMovimentacao.Parameters.AddWithValue("@quantidade", quantidade);
            comandoMovimentacao.Parameters.AddWithValue("@data_hora", DateTime.Now);

            comandoMovimentacao.ExecuteNonQuery();

            transacao.Commit();
        }
        catch
        {
            transacao.Rollback();
            throw;
        }
    }

    public void RegistrarSaida(
        int produtoId,
        int usuarioId,
        int quantidade)
    {
        using MySqlConnection conexao = banco.CriarConexao();

        conexao.Open();

        using MySqlTransaction transacao = conexao.BeginTransaction();

        try
        {
            string atualizarEstoque = """
                UPDATE produto
                SET estoque = estoque - @quantidade
                WHERE idproduto = @produto
                AND estoque >= @quantidade;
                """;

            using MySqlCommand comandoEstoque =
                new MySqlCommand(atualizarEstoque, conexao, transacao);

            comandoEstoque.Parameters.AddWithValue("@quantidade", quantidade);
            comandoEstoque.Parameters.AddWithValue("@produto", produtoId);

            int linhasAfetadas = comandoEstoque.ExecuteNonQuery();

            if (linhasAfetadas == 0)
            {
                throw new Exception("Estoque insuficiente ou produto não encontrado.");
            }

            string registrarMovimentacao = """
                INSERT INTO movimentacao_estoque
                (
                    produto_idproduto,
                    usuario_idusuario,
                    tipo,
                    quantidade,
                    data_hora
                )
                VALUES
                (
                    @produto,
                    @usuario,
                    'SAIDA',
                    @quantidade,
                    @data_hora
                );
                """;

            using MySqlCommand comandoMovimentacao =
                new MySqlCommand(registrarMovimentacao, conexao, transacao);

            comandoMovimentacao.Parameters.AddWithValue("@produto", produtoId);
            comandoMovimentacao.Parameters.AddWithValue("@usuario", usuarioId);
            comandoMovimentacao.Parameters.AddWithValue("@quantidade", quantidade);
            comandoMovimentacao.Parameters.AddWithValue("@data_hora", DateTime.Now);

            comandoMovimentacao.ExecuteNonQuery();

            transacao.Commit();
        }
        catch
        {
            transacao.Rollback();
            throw;
        }
    }
}