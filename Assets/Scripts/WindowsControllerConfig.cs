using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "WindowsControllerConfig", menuName = "Configs/WindowsControllerConfig", order = 1)]
    public class WindowsControllerConfig : ScriptableObject
    {
        [SerializeField] private List<WindowsConfiguration> _windowsConfigurations;

        public Window GetWindowPrefabForState(GameStates gameState)
        {
            foreach (var configuration in _windowsConfigurations)
            {
                if (configuration.GameState == gameState)
                {
                    return configuration.WindowPrefab;
                }
            }

            return default;
        }
    }

    [Serializable]
    public class WindowsConfiguration
    {
        public GameStates GameState;
        public Window WindowPrefab;
    }
}

