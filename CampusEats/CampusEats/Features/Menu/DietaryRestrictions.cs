namespace CampusEats.Features.Menu;

[Flags]
public enum DietaryRestrictions
{
    None = 0,
    LactoseFree = 1,
    GlutenFree = 2,
    NutFree = 4,
    DairyFree = 8,
    NoSeafood = 16
}