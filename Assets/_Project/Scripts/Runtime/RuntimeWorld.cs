using System;
using System.Collections.Generic;
using UnityEngine;

public static class RuntimeWorld
{
    public static class Bases
    {
        private static readonly List<PlayerBase> PlayerBasesList = new();

        public static event Action<List<PlayerBase>> OnChanged;

        public static void Register(PlayerBase playerBaseGameObject)
        {
            if (playerBaseGameObject == null) return;
            if (PlayerBasesList.Contains(playerBaseGameObject)) return;

            PlayerBasesList.Add(playerBaseGameObject);
            OnChanged?.Invoke(PlayerBasesList);
        }

        public static void Unregister(PlayerBase baseObj)
        {
            if (baseObj == null) return;
            if (PlayerBasesList.Remove(baseObj))
            {
                OnChanged?.Invoke(PlayerBasesList);
            }
        }

        public static List<PlayerBase> GetAll()
        {
            return PlayerBasesList;
        }

        public static List<PlayerBase> GetValidTargets()
        {
            return PlayerBasesList.FindAll(playerBase => playerBase && playerBase.State == BaseStateEnum.Active);
        }

        public static void Clear()
        {
            PlayerBasesList.Clear();
            OnChanged?.Invoke(PlayerBasesList);
        }
    }
}
