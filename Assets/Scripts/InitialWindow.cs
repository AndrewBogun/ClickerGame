using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Game
{
    public class InitialWindow : Window
    {
        [SerializeField] private GameObject _visualRoot;
        [SerializeField] private TMP_Text _textTitle;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private TMP_Text _buttonText;
        [SerializeField] private GameObject _buttonBlock;
        [SerializeField] private InputField _inputField;


        private void Awake()
        {
            if (_confirmButton != null)
            {
                _confirmButton.onClick.AddListener(() =>
                {
                    if (_inputField.text.Length > 0)
                    {
                        PlayerPrefs.SetString(GameKeys.PlayerName, _inputField.text);
                    }
                });
            }
        }

        private void OnEnable()
        {
            throw new NotImplementedException();
        }
    }
}

