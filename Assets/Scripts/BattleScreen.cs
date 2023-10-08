using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class BattleScreen : ScreenBase
    {
        [SerializeField] private RawImage _enemyAvatar;
        [SerializeField] private TMP_Text _enemyName;
        [SerializeField] private TMP_Text _enemyHp;
        [SerializeField] private Slider _enemyHpBar;
        [SerializeField] private Button _attackButton;

        private BattleManager _battleManager;

        private readonly CompositeDisposable _enemyCompositeDisposable = new();

        protected override void Awake()
        {
            base.Awake();

            _battleManager = FindObjectOfType<BattleManager>();

            _battleManager.CurrentEnemy.Subscribe(enemy =>
            {
                _enemyCompositeDisposable.Clear();
                if (enemy != null)
                {
                    _enemyAvatar.texture = enemy.Avatar;
                    _enemyName.text = enemy.Name;

                    enemy.HP.Subscribe(hp =>
                    {
                        _enemyHp.text = hp.ToString();
                    }).AddTo(_enemyCompositeDisposable);
                }
            }).AddTo(_compositeDisposable);

            if (_attackButton != null)
            {
                _attackButton.onClick.AddListener(() =>
                {
                    _battleManager.AttackCurrentEnemy();
                });
            }
        }
    }
}