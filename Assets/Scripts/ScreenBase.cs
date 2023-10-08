using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public abstract class ScreenBase : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] protected GameObject _root;
        [SerializeField] private List<GameStates> _showOnGameStates;

        [SerializeField] private bool _useFade;
        [SerializeField] private Color _fadeColor;
        [SerializeField] private float _fadeTime;
        [SerializeField] private float _unfadeTime;
#pragma warning restore CS0649

        private IDisposable _fadeDelayDisposable;

        private GameStates _prevGameState;
                
        private CanvasGroup _fadeCanvasGroup;
        private Coroutine _fadeCoroutine;

        protected Canvas _mainCanvas;
        protected CanvasScaler _canvasScaler;
        
        protected bool _isLocked;
        
        private readonly ReactiveProperty<bool> _isActive = new();

        protected readonly CompositeDisposable _compositeDisposable = new();
        
        protected readonly Subject<ScreenBase> _onShow = new Subject<ScreenBase>();

        protected readonly Subject<ScreenBase> _onHide = new Subject<ScreenBase>();
        

        protected bool IsActive
        {
            get
            {
                return _isActive.Value;
            }

            set
            {
                _isActive.Value = value;
                if(_isActive.Value)
                {
                    _onShow.OnNext(this);
                }
                else
                {
                    _onHide.OnNext(this);
                }
            }
        }
        
        protected virtual void Awake()
        {
            
            _root.SetActive(false);
            _mainCanvas = GetComponentInParent<Canvas>();
            _canvasScaler = GetComponentInParent<CanvasScaler>();

            if (_useFade)
            {
                var fadeGameObj = new GameObject("ScreenFade");
                var fadeTransform = fadeGameObj.transform;
                fadeTransform.SetParent(transform, false);
                fadeTransform.SetAsFirstSibling();
                
                var fadeRect = fadeGameObj.AddComponent<RectTransform>();
                var fadeImage = fadeGameObj.AddComponent<Image>();
                fadeImage.color = _fadeColor;
                
                fadeRect.anchorMin = new Vector2(0, 0);
                fadeRect.anchorMax = new Vector2(1, 1);
                fadeRect.pivot = new Vector2(0.5f, 0.5f);
                
                _fadeCanvasGroup = fadeGameObj.AddComponent<CanvasGroup>();
                
                _fadeCanvasGroup.gameObject.SetActive(false);
            }
        }

        protected virtual void Start()
        {
            var gameManager = FindObjectOfType<GameController>();
            gameManager.GameState.Subscribe(OnGameStateChanged).AddTo(_compositeDisposable);
            OnGameStateChanged(gameManager.GameState.Value);
        }


        protected void OnDestroy()
        {
            _fadeDelayDisposable?.Dispose();
            _compositeDisposable?.Dispose();
        }


        protected virtual void OnShow()
        {
            _root.SetActive(true);
            IsActive = true;
            _isLocked = false;
        }
        
        
        protected virtual void OnHide()
        {
            _isLocked = true;
            IsActive = false;
            _root.SetActive(false);
        }

        void OnGameStateChanged(GameStates gameState)
        {
            if(_prevGameState == gameState) return;
            
            
            bool show = _showOnGameStates.Contains(gameState);

            if (show)
            {
                if (!IsActive)
                {
                    if (_useFade)
                    {
                        Fade(_fadeTime);
                        _fadeDelayDisposable?.Dispose();
                        _fadeDelayDisposable = Observable
                            .Timer(TimeSpan.FromSeconds(_fadeTime), Scheduler.MainThreadIgnoreTimeScale)
                            .Subscribe(t => { OnShow(); });
                    }
                    else
                    {
                        OnShow();
                    }
                }
            }
            else
            {
                if (IsActive)
                {
                    if (_useFade)
                    {
                        Fade(_unfadeTime, true);
                    }
                    
                    OnHide();
                }
            }

            _prevGameState = gameState;
        }
        
        
        void Fade(float duration, bool inverceDuration = false)
        {
            FadeCancel();
            _fadeCoroutine = StartCoroutine(PlayFadeCorr(duration,inverceDuration));
        }


        void FadeCancel()
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
        }

        public void FadeEnabled(bool isEnabled)
        {
            FadeCancel();
            _fadeCanvasGroup.alpha = isEnabled ? 1f: 0f;

        }

        IEnumerator PlayFadeCorr(float duration, bool inverse = false)
        {
            float timeElapsed = 0;
            
            if (!inverse)
            {
                _fadeCanvasGroup.gameObject.SetActive(true);
            }

            while (timeElapsed <= duration)
            {
                float percent = Mathf.Clamp01(timeElapsed / duration);

                _fadeCanvasGroup.alpha = Mathf.Clamp01(inverse ? 1-percent : percent);

                timeElapsed += Time.unscaledDeltaTime;
                
                
                yield return null;
            }
            
            _fadeCanvasGroup.alpha = inverse ? 0f : 1f;

            if (inverse)
            {
                _fadeCanvasGroup.gameObject.SetActive(false);
            }
        }
    }
}

