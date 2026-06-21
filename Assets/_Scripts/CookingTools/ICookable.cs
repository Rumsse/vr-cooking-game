namespace Wastepunk.Interfaces
{
    public interface ICookable
    {
        float CookingProgress { get; set; }
        float TimeToCook { get; }
        float TimeToBurn { get; }

        void Cook(Wastepunk.Ingredients.CookingType cookingType);
        void Burn();
        bool CanBeCooked(Wastepunk.Ingredients.CookingType cookingType);
    }
}