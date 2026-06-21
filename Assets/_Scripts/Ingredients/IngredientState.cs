using System;

namespace Wastepunk.Ingredients
{
    [Flags]
    public enum IngredientState
    {
        None = 0,
        Dirty = 1 << 0,
        NotPeeled = 1 << 1,
        Raw = 1 << 2,
        Sliced = 1 << 3,
        Boiled = 1 << 4,
        Fried = 1 << 5,
        Seasoned = 1 << 6,
        Burned = 1 << 7
    }

    public enum CookingType
    {
        Boiling,
        Frying
    }
}