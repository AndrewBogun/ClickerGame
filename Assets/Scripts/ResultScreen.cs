using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game
{
    public class ResultScreen : ScreenBase
    {
        [SerializeField] private TMP_Text _finalText;
        [SerializeField] private TMP_Text _rewardCount;
        [SerializeField] private Button _continueButton;

        private GameController _gameController;
        private BattleManager _battleManager;

        private int _reward;

        protected override void Awake()
        {
            base.Awake();

            _gameController = FindObjectOfType<GameController>();
            _battleManager = FindObjectOfType<BattleManager>();

            _onShow.Subscribe(_ =>
            {
                var enemy = _battleManager.CurrentEnemy.Value;

                if (enemy != null)
                {
                    _finalText.text = $"Бой с {enemy.Name}";
                    _reward = enemy.Reward;
                    _rewardCount.text = _reward.ToString();
                }
            }).AddTo(_compositeDisposable);

            if (_continueButton != null)
            {
                _continueButton.onClick.AddListener(() =>
                {
                    _gameController.ReceiveReward(_reward);
                });
            }
        }
    }
}

