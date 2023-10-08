using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Game
{
    public class InitialWindow : ScreenBase
    {
        [SerializeField] private Button _confirmButton;
        [SerializeField] private TMP_InputField _inputField;


        protected override void Awake()
        {
            base.Awake();

            var gameController = FindObjectOfType<GameController>();
            
            _inputField.onValueChanged.AddListener(value =>
            {
                _confirmButton.interactable = value.Length > 0;
            });
            
            if (_confirmButton != null)
            {
                _confirmButton.interactable = false;
                
                _confirmButton.onClick.AddListener(() =>
                {
                    if (_inputField.text.Length > 0)
                    {
                        PlayerPrefs.SetString(GameKeys.PlayerName, _inputField.text);

                        if (gameController != null)
                        {
                            gameController.SetGameState(GameStates.Search);
                        }
                    }
                });
            }
        }
    }
}

