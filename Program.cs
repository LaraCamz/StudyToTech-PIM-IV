using StudyToTech.Services;

ProdutoService produtoService = new ProdutoService();
UsuarioService usuarioService = new UsuarioService();
PedidoService pedidoService = new PedidoService();
ItemPedidoService itemPedidoService = new ItemPedidoService();
EstoqueService estoqueService = new EstoqueService();

int opcao;

do
{
    //Console.Clear();

    Console.WriteLine("===== SISTEMA DE ESTOQUE =====");
    Console.WriteLine("1 - Cadastrar produto");
    Console.WriteLine("2 - Listar produtos");
    Console.WriteLine("3 - Cadastrar usuário");
    Console.WriteLine("4 - Listar usuários");
    Console.WriteLine("5 - Criar pedido");
    Console.WriteLine("6 - Adicionar item ao pedido");
    Console.WriteLine("7 - Consultar estoque");
    Console.WriteLine("8 - Registrar entrada/saída de estoque");
    Console.WriteLine("9 - Finalizar pedido");
    Console.WriteLine("0 - Sair");
    Console.WriteLine("==============================");

    Console.Write("Escolha uma opção: ");
    opcao = int.Parse(Console.ReadLine()!);

    Console.Clear();

    switch (opcao)
    {
        case 1:
            produtoService.CadastrarProduto();
            break;

        case 2:
            produtoService.ListarProdutos();
            break;

        case 3:
            usuarioService.CadastrarUsuario();
            break;

        case 4:
            usuarioService.ListarUsuarios();
            break;

        case 5:
            pedidoService.CriarPedido();
            break;

        case 6:
            itemPedidoService.AdicionarItemAoPedido();
            break;

        case 7:
            estoqueService.ConsultarEstoque();
            break;

        case 8:
            Console.WriteLine("===== MOVIMENTAÇÃO DE ESTOQUE =====");
            Console.WriteLine("1 - Registrar entrada");
            Console.WriteLine("2 - Registrar saída");
            Console.Write("Escolha uma opção: ");

            int tipoMovimentacao = int.Parse(Console.ReadLine()!);

            Console.Clear();

            if (tipoMovimentacao == 1)
            {
                estoqueService.RegistrarEntrada();
            }
            else if (tipoMovimentacao == 2)
            {
                estoqueService.RegistrarSaida();
            }
            else
            {
                Console.WriteLine("Opção inválida.");
            }

            break;

        case 9:
            pedidoService.FinalizarPedido();
            break;

        case 0:
            Console.WriteLine("Encerrando o sistema...");
            break;

        default:
            Console.WriteLine("Opção inválida.");
            break;
    }

    if (opcao != 0)
    {
        Console.WriteLine();
        Console.WriteLine("Pressione ENTER para continuar...");
        Console.ReadLine();
    }

} while (opcao != 0);