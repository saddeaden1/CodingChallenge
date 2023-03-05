using FluentValidation;

namespace CodingChallenge;

public class ArgsValidator : AbstractValidator<string[]>
{
    public ArgsValidator()
    {
        RuleFor(x => x).NotNull().WithMessage("No road inputted please enter a road");
        RuleFor(x => x).NotEmpty().WithMessage("No road inputted please enter a road");
        RuleFor(x => x).Must(x=>x.Length < 2).WithMessage("Please enter roads one at a time");
        RuleForEach(x => x).ChildRules(roads =>
        {
            roads.RuleFor(road => road).NotNull().WithMessage("No road inputted please enter a road");
            roads.RuleFor(road => road).NotEmpty().WithMessage("No road inputted please enter a road");
        });
    }
}