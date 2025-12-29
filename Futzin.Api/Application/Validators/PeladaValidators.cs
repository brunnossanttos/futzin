using FluentValidation;
using Futzin.Api.Application.DTOs;
using Futzin.Api.Domain.Enums;

namespace Futzin.Api.Application.Validators;

public class CreatePeladaDtoValidator : AbstractValidator<CreatePeladaDto>
{
    public CreatePeladaDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Data é obrigatória")
            .GreaterThan(DateTime.UtcNow.AddMinutes(-30))
            .WithMessage("Data deve ser no futuro ou no máximo 30 minutos no passado");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Localização é obrigatória")
            .MaximumLength(200).WithMessage("Localização deve ter no máximo 200 caracteres");

        RuleFor(x => x.FieldType)
            .IsInEnum().WithMessage("Tipo de campo inválido")
            .Must(BeAValidFieldType).WithMessage("Tipo de campo deve ser: Campo, Quadra, Society ou Terrão");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Preço deve ser maior ou igual a zero")
            .LessThanOrEqualTo(1000).WithMessage("Preço deve ser menor ou igual a R$ 1000");

        RuleFor(x => x.MaxPlayers)
            .GreaterThanOrEqualTo(2).WithMessage("Número mínimo de jogadores é 2")
            .LessThanOrEqualTo(100).WithMessage("Número máximo de jogadores é 100")
            .Must(BeAnEvenNumber).WithMessage("Número de jogadores deve ser par para dividir em times");
    }

    private bool BeAValidFieldType(FieldType fieldType)
    {
        return Enum.IsDefined(typeof(FieldType), fieldType);
    }

    private bool BeAnEvenNumber(int maxPlayers)
    {
        return maxPlayers % 2 == 0;
    }
}

public class UpdatePeladaDtoValidator : AbstractValidator<UpdatePeladaDto>
{
    public UpdatePeladaDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome não pode ser vazio")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres")
            .When(x => x.Name != null);

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres")
            .When(x => x.Description != null);

        RuleFor(x => x.Date)
            .GreaterThan(DateTime.UtcNow.AddMinutes(-30))
            .WithMessage("Data deve ser no futuro ou no máximo 30 minutos no passado")
            .When(x => x.Date.HasValue);

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Localização não pode ser vazia")
            .MaximumLength(200).WithMessage("Localização deve ter no máximo 200 caracteres")
            .When(x => x.Location != null);

        RuleFor(x => x.FieldType)
            .IsInEnum().WithMessage("Tipo de campo inválido")
            .Must(ft => Enum.IsDefined(typeof(FieldType), ft!.Value))
            .WithMessage("Tipo de campo deve ser: Campo, Quadra, Society ou Terrão")
            .When(x => x.FieldType.HasValue);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Preço deve ser maior ou igual a zero")
            .LessThanOrEqualTo(1000).WithMessage("Preço deve ser menor ou igual a R$ 1000")
            .When(x => x.Price.HasValue);

        RuleFor(x => x.MaxPlayers)
            .GreaterThanOrEqualTo(2).WithMessage("Número mínimo de jogadores é 2")
            .LessThanOrEqualTo(100).WithMessage("Número máximo de jogadores é 100")
            .Must(mp => mp!.Value % 2 == 0)
            .WithMessage("Número de jogadores deve ser par para dividir em times")
            .When(x => x.MaxPlayers.HasValue);
    }
}
