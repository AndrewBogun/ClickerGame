using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "BattleManagerConfig", menuName = "Configs/BattleManagerConfig", order = 1)]
    public class BattleManagerConfig : ScriptableObject
    {
        [SerializeField] private int _minEnemyHp = 50;
        [SerializeField] private int _maxEnemyHp = 100;
        [SerializeField] private int _minAttackDamage = 5;
        [SerializeField] private int _maxAttackDamage = 10;
        [SerializeField] private int _minReward;
        [SerializeField] private int _maxReward;

        public int GetEnemyHp()
        {
            return Random.Range(_minEnemyHp, _maxEnemyHp + 1);
        }

        public int GetEnemyReward()
        {
            return Random.Range(_minReward, _maxReward + 1);
        }

        public int GetAttackDamage()
        {
            return Random.Range(_minAttackDamage, _maxAttackDamage + 1);
        }
    }
}