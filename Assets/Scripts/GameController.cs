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
            else
            {
                SetGameState(GameStates.Search);
            }
        }

        public void SetGameState(GameStates gameState)
        {
            _gameState.Value = gameState;
        }

        public void ReceiveReward(int rewardAmount)
        {
            var playerSoftCount = PlayerPrefs.GetInt(GameKeys.PlayerSoft);
            PlayerPrefs.SetInt(GameKeys.PlayerSoft, playerSoftCount + rewardAmount);
            
            SetGameState(GameStates.Search);
        }
    }
    
    public static partial class GameKeys
    {
        public static readonly string PlayerName = "PlayerName";
        public static readonly string PlayerSoft = "PlayerSoft";
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