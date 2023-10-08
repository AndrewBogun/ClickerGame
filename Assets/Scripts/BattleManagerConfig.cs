using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "BattleManagerConfig", menuName = "Configs/BattleManagerConfig", order = 1)]
    public class BattleManagerConfig : ScriptableObject
    {
        [SerializeField, Range(50, 100)] private int _minEnemyHp;
        [SerializeField, Range(50, 100)] private int _maxEnemyHp;
        [SerializeField, Range(50, 100)] private int _minAttackDamage;
        [SerializeField, Range(50, 100)] private int _maxAttackDamage;
        [SerializeField] private int _minReward;
        [SerializeField] private int _maxReward;

        public int GetEnemyHp()
        {
            return Random.Range(_minEnemyHp, _maxEnemyHp);
        }

        public int GetEnemyReward()
        {
            return Random.Range(_minReward, _maxReward);
        }

        public int GetAttackDamage()
        {
            return Random.Range(_minAttackDamage, _maxAttackDamage);
        }

        private void OnValidate()
        {
            if (_minEnemyHp > _maxEnemyHp)
            {
                _minEnemyHp = _maxEnemyHp - 1;
            }
        }
    }
}