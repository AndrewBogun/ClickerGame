using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class EnemySearchScreen : ScreenBase
    {
        [SerializeField] private GameObject _loadingRoot;
        [SerializeField] private GameObject _mainRoot;
        [SerializeField] private TMP_Text _playerName;
        [SerializeField] private TMP_Text _enemyName;
        [SerializeField] private RawImage _enemyAvatar;
        [SerializeField] private TMP_Text _playerSoft;
        [SerializeField] private Button _searchButton;
        [SerializeField] private Button _battleButton;

        private BattleManager _battleManager;
        private GameController _gameController;

        protected override void Awake()
        {
            base.Awake();

            _battleManager = FindObjectOfType<BattleManager>();
            _gameController = FindObjectOfType<GameController>();

            _battleManager.CurrentEnemy.Subscribe(enemy =>
            {
                if (enemy != null)
                {
                    _enemyName.text = enemy.Name;
                    _enemyAvatar.texture = enemy.Avatar;
                }
            }).AddTo(_compositeDisposable);
        
            _onShow.Subscribe(async _ =>
            {
                SearchEnemy();
                _playerName.text = PlayerPrefs.GetString(GameKeys.PlayerName);
                _playerSoft.text = PlayerPrefs.GetInt(GameKeys.PlayerSoft).ToString();
            }).AddTo(_compositeDisposable);

            if (_searchButton != null)
            {
                _searchButton.onClick.AddListener(SearchEnemy);
            }

            if (_battleButton != null)
            {
                _battleButton.onClick.AddListener(() =>
                {
                    _gameController.SetGameState(GameStates.Fight);
                });
            }
        }

        private async void SearchEnemy()
        {
            _loadingRoot.SetActive(true);
            await _battleManager.GetEnemy();
            _loadingRoot.SetActive(false);
        }

        private void OnDisable()
        {
            _compositeDisposable.Clear();
        }
    }
}
