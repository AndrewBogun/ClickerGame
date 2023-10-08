using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private BattleManagerConfig _config;

        private GameController _gameController;

        private readonly ReactiveProperty<Enemy> _curEnemy = new();
        public IReadOnlyReactiveProperty<Enemy> CurrentEnemy => _curEnemy;

        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly CompositeDisposable _enemyCompositeDisposable = new();

        private void Awake()
        {
            _gameController = FindObjectOfType<GameController>();

            _curEnemy.Subscribe(enemy =>
            {
                _enemyCompositeDisposable.Clear();
                if (enemy != null)
                {
                    enemy.IsDead.Subscribe(isDead =>
                    {
                        if (isDead)
                        {
                            _gameController.SetGameState(GameStates.Result);
                        }
                    }).AddTo(_enemyCompositeDisposable);
                }
            }).AddTo(_compositeDisposable);
        }

        private void OnDestroy()
        {
            _enemyCompositeDisposable?.Dispose();
            _compositeDisposable?.Dispose();
        }

        public async Task GetEnemy()
        {
            Enemy enemy = new();
            
            var response = await MakeRequest();
            await ProcessResult(response, enemy);

            enemy.SetHp(_config.GetEnemyHp()); 
            enemy.Reward = _config.GetEnemyReward();

            _curEnemy.Value = enemy;
        }
        
        private async Task<string> MakeRequest()
        {
            using (HttpClient client = new HttpClient())
            {
                var url = "https://randomuser.me/api/";
                HttpResponseMessage response = await client.GetAsync(url);
            
                string responseBody = await response.Content.ReadAsStringAsync();
            
                return responseBody;
            }
        }
        
        private async Task<Texture2D> LoadImage(string url)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                var asyncOperation = www.SendWebRequest();

                while (!asyncOperation.isDone)
                {
                    await Task.Yield();
                }

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(www.error);
                    return default;
                }
                else
                {
                    Texture2D texture2D = DownloadHandlerTexture.GetContent(www);
                    return texture2D;
                }
            }
        }



        
        private async Task ProcessResult(string webJson, Enemy enemy)
        {
            if (!string.IsNullOrWhiteSpace(webJson))
            {
                var result = JsonConvert.DeserializeObject<EnemyInfo>(webJson);

                if (result != null)
                {
                    enemy.Name = result.results[0].name.first;
                    enemy.Avatar = await LoadImage(result.results[0].picture.large);
                }
            }
        }

        public void AttackCurrentEnemy()
        {
            if (_curEnemy.Value != null)
            {
                _curEnemy.Value.TakeDamage(_config.GetAttackDamage());
            }
        }
    }

    public class Enemy
    {
        private ReactiveProperty<int> _hp = new();
        public IReadOnlyReactiveProperty<int> HP => _hp;

        private readonly ReactiveProperty<bool> _isDead = new(false);
        
        public IReadOnlyReactiveProperty<bool> IsDead => _isDead;
        public string Name { get; set; }
        public Texture2D Avatar { get; set; }
        public int Reward { get; set; }


        public void TakeDamage(int damage)
        {
            if (damage > 0 && _hp.Value > 0)
            {
                if (damage >= _hp.Value)
                {
                    _hp.Value = 0;
                }
                else
                {
                    _hp.Value -= damage; 
                }
            }

            if (_hp.Value <= 0)
            {
                _isDead.Value = true;
            }
        }

        public void SetHp(int healsPoints)
        {
            _hp.Value = healsPoints;
        }
    }
    
    public class Name
    {
        public string title { get; set; }
        public string first { get; set; }
        public string last { get; set; }
    }

    public class Picture
    {
        public string large { get; set; }
        public string medium { get; set; }
        public string thumbnail { get; set; }
    }

    public class Result
    {
        public Name name { get; set; }
        public Picture picture { get; set; }
    }

    public class EnemyInfo
    {
        public Result[] results { get; set; }
    }
}