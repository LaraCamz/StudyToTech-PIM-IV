using MySql.Data.MySqlClient;
using StudyToTech.Data;
using StudyToTech.Models;

namespace StudyToTech.Repositories;

public class ProdutoRepository
{
    private readonly Banco banco;

    public ProdutoRepository()
    {
        banco = new Banco();
    }

    public void CadastrarProduto(Produto produto)
    {
        using MySqlConnection conexao = banco.CriarConexao();

        conexao.Open();

        string sql = """
            INSERT INTO produto
            (
                nome,
                descricao,
                categoria_idcategoria,
                preco,
                estoque,
                estoque_minimo,
                status,
                data_cadastro
            )
            VALUES
            (
                @nome,
                @descricao,
                @categoria,
                @preco,
                @estoque,
                @estoque_minimo,
                @status,
                @data_cadastro
            );
            """;

        using MySqlCommand comando = new MySqlCommand(sql, conexao);

        comando.Parameters.AddWithValue("@nome", produto.Nome);
        comando.Parameters.AddWithValue("@descricao", produto.Descricao);
        comando.Parameters.AddWithValue("@categoria", produto.CategoriaIdCategoria);
        comando.Parameters.AddWithValue("@preco", produto.Preco);
        comando.Parameters.AddWithValue("@estoque", produto.Estoque);
        comando.Parameters.AddWithValue("@estoque_minimo", produto.EstoqueMinimo);
        comando.Parameters.AddWithValue("@status", produto.Status);
        comando.Parameters.AddWithValue("@data_cadastro", produto.DataCadastro);

        comando.ExecuteNonQuery();
    }

    public List<Produto> ListarProdutos()
    {
        List<Produto> produtos = new();

        using MySqlConnection conexao = banco.CriarConexao();

        conexao.Open();

        string sql = """
            SELECT
                idproduto,
                nome,
                descricao,
                categoria_idcategoria,
                preco,
                estoque,
                estoque_minimo,
                status,
                data_cadastro
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
                Descricao = leitor.IsDBNull(leitor.GetOrdinal("descricao"))
                    ? ""
                    : leitor.GetString("descricao"),
                CategoriaIdCategoria = leitor.GetInt32("categoria_idcategoria"),
                Preco = leitor.GetDecimal("preco"),
                Estoque = leitor.GetInt32("estoque"),
                EstoqueMinimo = leitor.GetInt32("estoque_minimo"),
                Status = Convert.ToBoolean(leitor["status"]),
                DataCadastro = leitor.GetDateTime("data_cadastro")
            };

            produtos.Add(produto);
        }

        return produtos;
    }

    public Produto? BuscarProduto(int id)
    {
        using MySqlConnection conexao = banco.CriarConexao();

        conexao.Open();

        string sql = """
            SELECT
                idproduto,
                nome,
                descricao,
                categoria_idcategoria,
                preco,
                estoque,
                estoque_minimo,
                status,
                data_cadastro
            FROM produto
            WHERE idproduto = @id;
            """;

        using MySqlCommand comando = new MySqlCommand(sql, conexao);

        comando.Parameters.AddWithValue("@id", id);

        using MySqlDataReader leitor = comando.ExecuteReader();

        if (!leitor.Read())
        {
            return null;
        }

        return new Produto
        {
            IdProduto = leitor.GetInt32("idproduto"),
            Nome = leitor.GetString("nome"),
            Descricao = leitor.IsDBNull(leitor.GetOrdinal("descricao"))
                ? ""
                : leitor.GetString("descricao"),
            CategoriaIdCategoria = leitor.GetInt32("categoria_idcategoria"),
            Preco = leitor.GetDecimal("preco"),
            Estoque = leitor.GetInt32("estoque"),
            EstoqueMinimo = leitor.GetInt32("estoque_minimo"),
            Status = Convert.ToBoolean(leitor["status"]),
            DataCadastro = leitor.GetDateTime("data_cadastro")
        };
    }
}