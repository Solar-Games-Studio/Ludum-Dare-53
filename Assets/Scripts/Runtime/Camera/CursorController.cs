using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Game.Scene;

namespace Game.Runtime.Camera
{
    public static class CursorManager
    {
        public static List<CursorLock> Locks { get; private set; } = new List<CursorLock>();
        public static bool IsLocked { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            SceneManager.OnSceneLoaded += SceneManager_OnSceneLoaded;
        }

        private static void SceneManager_OnSceneLoaded(string scene)
        {
            Refresh();
        }

        internal static void AddLock(CursorLock cursorLock)
        {
            Locks.Add(cursorLock);
        }  

        internal static void Refresh()
        {
            Locks = Locks
                .Where(x => x != null)
                .Distinct()
                .ToList();

            bool locked = true;

            foreach (var cursorLock in Locks)
                if (!cursorLock.Locked)
                    locked = false;

            IsLocked = locked;
            Cursor.lockState = locked ?
                CursorLockMode.Locked :
                CursorLockMode.None;
        }
    }
}