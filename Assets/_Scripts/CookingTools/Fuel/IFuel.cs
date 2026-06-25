namespace Wastepunk.Interfaces
{
    public interface IFuel
    {
        float FuelAmount { get; }
        void Consume();
    }
}