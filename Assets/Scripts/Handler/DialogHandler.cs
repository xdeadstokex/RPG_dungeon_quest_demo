using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameData;

public class DialogHandler : MonoBehaviour {
    [SerializeField] private GameObject _dialogBox;
    [SerializeField] private Text _dialogText;
    [SerializeField] private int _letterPerSecond;
    [SerializeField] private List<string> _lines;

    private int _currentLine = 0;
    private bool _isTyping;
    public bool _isOpening;
    public bool _isStoping;

    public Action OnOpened;
    public Action OnClosed;

    private void Update() {
        if (!_isOpening) { return; }

        if (!_isStoping) {
            OnOpened?.Invoke();
        }

        if (PlayerDialogEscapeKey()) {
            _isTyping = false;
            _isOpening = false;
            _currentLine = 0;
            _dialogBox.SetActive(false);
            OnClosed?.Invoke();
            return;
        }

        if (!_isStoping && PlayerInteractKey() && !_isTyping) {
            if (++_currentLine < _lines.Count) {
                StartCoroutine(TypeDialog(_lines[_currentLine]));
                return;
            } else {
                _isStoping = true;
                _currentLine = 0;
                _dialogBox.SetActive(false);
                OnClosed?.Invoke();
                StopAllCoroutines(); // temp
                return;
            }
        }

        if (_isStoping && PlayerInteractKey()) {
            _isStoping = false;
            _isOpening = false;
            _isTyping = false;
        }
    }

    public void Show(List<string> lines){
        if (_isStoping || _isOpening || _dialogBox.activeSelf) { return; }
        if (lines == null || lines.Count == 0) { return; }

        _isStoping = false;
        _isOpening = true;
        _lines = lines;
        _currentLine = 0;
        _dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(_lines[0]));
    }

    public void Show(PlayerData data){
        if (_isStoping || _isOpening || _dialogBox.activeSelf) { return; }
        if (_lines == null || _lines.Count == 0) { return; }
		OnOpened = data.playerStop;
        _isStoping = false;
        _isOpening = true;
        _currentLine = 0;
        _dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(_lines[0]));
    }

    private IEnumerator TypeDialog(string line) {
        _isTyping = true;
        _dialogText.text = "";
        foreach (var letter in line.ToCharArray()) {
            _dialogText.text += letter;
            yield return new WaitForSeconds(1f / _letterPerSecond);
        }
        _isTyping = false;
    }
}