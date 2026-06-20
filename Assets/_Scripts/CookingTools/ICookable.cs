namespace Wastepunk.Interfaces
{
    public interface ICookable
    {
        float CookingProgress { get; set; }
        float TimeToCook { get; }
        float TimeToBurn { get; }

        void Cook();
        void Burn();
        bool CanBeCooked();
    }
}