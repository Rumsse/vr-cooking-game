using System;

namespace Wastepunk.Core
{
    public static class GameEvents
    {
        public static event Action<GameState> OnGameStateChanged;
        public static event Action OnWormFed;
        public static event Action OnDishServed;
        public static event Action OnTimeChanged; 
        public static event Action OnGameOver;

        public static void TriggerGameStateChanged(GameState newState) => OnGameStateChanged?.Invoke(newState);
        public static void TriggerWormFed() => OnWormFed?.Invoke();
        public static void TriggerDishServed() => OnDishServed?.Invoke();
        public static void TriggerTimeChanged() => OnTimeChanged?.Invoke();
        public static void TriggerGameOver() => OnGameOver?.Invoke();
    }
}
