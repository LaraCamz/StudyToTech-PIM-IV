using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace StudyToTech.Data;

public class Banco
{
    private readonly string conexao;

    public Banco()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        conexao = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "A string de conexão não foi encontrada.");
    }

    public MySqlConnection CriarConexao()
    {
        return new MySqlConnection(conexao);
    }
}