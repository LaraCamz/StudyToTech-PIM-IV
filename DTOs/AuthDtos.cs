using System.ComponentModel.DataAnnotations;

namespace StudyToTech.Api.DTOs;

public class LoginRequest
{
    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Senha { get; set; } = string.Empty;
}

public class RegistroClienteRequest
{
    [Required, MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Senha { get; set; } = string.Empty;
}

public record LoginResponse(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    UsuarioDto Usuario
);

public record PerfilDto(int Id, string Nome);

public record UsuarioDto(
    int Id,
    string Nome,
    string Email,
    bool Ativo,
    PerfilDto Perfil,
    DateTime DataCadastro
);

public class UsuarioCreate
{
    [Required, MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Senha { get; set; } = string.Empty;

    [Required]
    public int PerfilId { get; set; }

    public bool Ativo { get; set; } = true;
}

public class UsuarioUpdate
{
    [Required, MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MinLength(8)]
    public string? Senha { get; set; }

    [Required]
    public int PerfilId { get; set; }
}
