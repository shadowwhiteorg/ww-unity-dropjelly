using System;

namespace ww.DropJelly
{
    internal static class EventManager
    {
        public static event Action OnLevelCompleted;
        public static event Action OnLevelFailed;

        public static void LevelComplete()
        {
            OnLevelCompleted?.Invoke();
        }

        public static void LevelFailed()
        {
            OnLevelFailed?.Invoke();
        }
    }
}
