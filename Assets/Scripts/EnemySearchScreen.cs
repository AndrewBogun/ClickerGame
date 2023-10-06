using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
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

        private readonly CompositeDisposable _compositeDisposable = new();


        protected override void Awake()
        {
            base.Awake();
        
            _onShow.Subscribe(async _ =>
            {
                var response = await MakeRequest();
                ProcessResult(response);
                Debug.Log(response);
            }).AddTo(_compositeDisposable);
        }

        private void ProcessResult(string webJson)
        {
            if (!string.IsNullOrWhiteSpace(webJson))
            {
                var result = JsonConvert.DeserializeObject<EnemyInfo>(webJson);

                if (result != null)
                {
                    _enemyName.text = result.results[0].name.first;
                    StartCoroutine(LoadImage(result.results[0].picture.large));
                }
            }
        }
        
        private IEnumerator LoadImage(string url)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    // Создаем текстуру из загруженных данных
                    Texture2D texture = DownloadHandlerTexture.GetContent(www);

                    // Помещаем текстуру в RawImage
                    _enemyAvatar.texture = texture;
                }
            }
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
