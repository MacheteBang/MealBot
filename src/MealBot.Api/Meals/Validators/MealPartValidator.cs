namespace MealBot.Api.Meals.Validators;

public class MealPartValidator : AbstractValidator<MealPart>
{
    public MealPartValidator()
    {
        RuleFor(y => y.Category)
            .IsInEnum()
            .NotEqual(MealPartCategory.Unknown.ToString());

        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}