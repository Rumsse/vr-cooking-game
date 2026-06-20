namespace Wastepunk.Interfaces
{
    public interface IFillable
    {
        float MaxVolume { get; }
        float CurrentVolume { get; }
        void Fill(float amount);
        bool IsFull();
    }
}