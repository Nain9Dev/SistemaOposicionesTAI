using FluentValidation;
using Oposiciones.Api.DTOs;

namespace Oposiciones.Api.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .MaximumLength(255).WithMessage("El email no puede superar los 255 caracteres.")
            .EmailAddress().WithMessage("Formato de email invalido.");

        // En login no se validan reglas de complejidad: solo que venga algo. Rechazar por
        // formato revelaria la politica de contrasenas de las cuentas existentes.
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contrasena es obligatoria.")
            .MaximumLength(128).WithMessage("La contrasena no puede superar los 128 caracteres.");
    }
}

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    /// <summary>
    /// BCrypt trunca silenciosamente a 72 bytes. Se limita antes para que dos contrasenas
    /// distintas con el mismo prefijo no acaben siendo equivalentes.
    /// </summary>
    private const int MaxPasswordLength = 72;

    public RegisterDtoValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .MaximumLength(255).WithMessage("El email no puede superar los 255 caracteres.")
            .EmailAddress().WithMessage("Formato de email invalido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contrasena es obligatoria.")
            .MinimumLength(8).WithMessage("La contrasena debe tener al menos 8 caracteres.")
            .MaximumLength(MaxPasswordLength).WithMessage($"La contrasena no puede superar los {MaxPasswordLength} caracteres.")
            .Matches("[A-Za-z]").WithMessage("La contrasena debe contener al menos una letra.")
            .Matches("[0-9]").WithMessage("La contrasena debe contener al menos un numero.");
    }
}
