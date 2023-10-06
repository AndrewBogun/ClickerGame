using UniRx;
using UnityEngine;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        private readonly ReactiveProperty<GameStates> _gameState = new();

        public IReadOnlyReactiveProperty<GameStates> GameState => _gameState;
        
        private void Awake()
        {
            if (!PlayerPrefs.HasKey(GameKeys.PlayerName))
            {
                SetGameState(GameStates.Init);
            }
        }

        private void SetGameState(GameStates gameState)
        {
            _gameState.Value = gameState;
        }
    }
    
    public static partial class GameKeys
    {
        public static string PlayerName;
    }

    public enum GameStates
    {
        None = 0,
        Init = 1,
        Search = 2,
        Fight = 3,
        Result = 4
    }
}