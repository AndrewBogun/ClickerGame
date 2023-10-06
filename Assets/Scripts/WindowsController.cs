using System;
using System.Diagnostics;
using UniRx;
using UnityEngine;

namespace Game
{
    public class WindowsController : MonoBehaviour
    {
        [SerializeField] private WindowsControllerConfig _config;
        
        private GameController _gameController;

        private readonly CompositeDisposable _compositeDisposable = new ();

        private void Awake()
        {
            _gameController = FindObjectOfType<GameController>();

            _gameController.GameState.Subscribe(ProcessGameState).AddTo(_compositeDisposable);
        }

        private void ProcessGameState(GameStates gameState)
        {
            switch (gameState)
            {
                case GameStates.Init:
                    var windowPrefab = _config.GetWindowPrefabForState(gameState);
                    if (windowPrefab != null)
                    {
                        var window = Instantiate(windowPrefab, transform);
                    }
                    break;
            }
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
        }
    }
}