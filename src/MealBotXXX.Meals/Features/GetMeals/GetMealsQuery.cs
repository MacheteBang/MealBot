namespace MealBot.Meals.Features.GetMeals;

public record GetMealsQuery : IRequest<ErrorOr<List<Meal>>>;