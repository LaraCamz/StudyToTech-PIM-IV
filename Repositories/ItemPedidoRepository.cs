using MySql.Data.MySqlClient;
using StudyToTech.Data;
using StudyToTech.Models;

namespace StudyToTech.Repositories;

public class ItemPedidoRepository
{
    private readonly Banco banco;

    public ItemPedidoRepository()
    {
        banco = new Banco();
    }

    public void AdicionarItemAoPedido(ItemPedido item)
    {
        using MySqlConnection conexao = banco.CriarConexao();

        conexao.Open();

        string sql = """
            INSERT INTO item_pedido
            (
                pedido_idpedido,
                produto_idproduto,
                preco_unitario,
                quantidade
            )
            VALUES
            (
                @pedido,
                @produto,
                @preco,
                @quantidade
            );
            """;

        using MySqlCommand comando = new MySqlCommand(sql, conexao);

        comando.Parameters.AddWithValue("@pedido", item.PedidoIdPedido);
        comando.Parameters.AddWithValue("@produto", item.ProdutoIdProduto);
        comando.Parameters.AddWithValue("@preco", item.PrecoUnitario);
        comando.Parameters.AddWithValue("@quantidade", item.Quantidade);

        comando.ExecuteNonQuery();
    }

    public List<ItemPedido> ListarItensDoPedido(int pedidoId)
    {
        List<ItemPedido> itens = new();

        using MySqlConnection conexao = banco.CriarConexao();

        conexao.Open();

        string sql = """
            SELECT
                iditem_pedido,
                pedido_idpedido,
                produto_idproduto,
                preco_unitario,
                quantidade
            FROM item_pedido
            WHERE pedido_idpedido = @pedido;
            """;

        using MySqlCommand comando = new MySqlCommand(sql, conexao);

        comando.Parameters.AddWithValue("@pedido", pedidoId);

        using MySqlDataReader leitor = comando.ExecuteReader();

        while (leitor.Read())
        {
            ItemPedido item = new ItemPedido
            {
                IdItemPedido = leitor.GetInt32("iditem_pedido"),
                PedidoIdPedido = leitor.GetInt32("pedido_idpedido"),
                ProdutoIdProduto = leitor.GetInt32("produto_idproduto"),
                PrecoUnitario = leitor.GetDecimal("preco_unitario"),
                Quantidade = leitor.GetInt32("quantidade")
            };

            itens.Add(item);
        }

        return itens;
    }
}