using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Game
{
    public class InitialWindow : ScreenBase
    {
        [SerializeField] private TMP_Text _textTitle;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private TMP_Text _buttonText;
        [SerializeField] private TMP_InputField _inputField;


        protected override void Awake()
        {
            base.Awake();
            
            _inputField.onValueChanged.AddListener(value =>
            {
                if (value.Length > 0)
                {
                    _confirmButton.interactable = true;
                }
            });
            
            if (_confirmButton != null)
            {
                _confirmButton.interactable = false;
                
                _confirmButton.onClick.AddListener(() =>
                {
                    if (_inputField.text.Length > 0)
                    {
                        PlayerPrefs.SetString(GameKeys.PlayerName, _inputField.text);
                    }
                });
            }
        }
    }
}

