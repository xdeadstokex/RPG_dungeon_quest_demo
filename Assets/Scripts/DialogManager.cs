using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private GameObject _dialogBox;
    [SerializeField] private Text _dialogText;
    [SerializeField] private int _letterPerSecond;
    private int _currentLine = 0;
    private Dialog _dialog;
    private bool _isTyping;
    public event Action OnShowDialog;
    public event Action OnHideDialog;

    public static DialogManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator ShowDialog(Dialog dialog)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();

        this._dialog = dialog;
        _dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E) && !_isTyping)
        {
            ++_currentLine;
            if (_currentLine < _dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(_dialog.Lines[_currentLine]));
            }
            else
            {
                _currentLine = 0;
                _dialogBox.SetActive(false);
                OnHideDialog?.Invoke();
            }
        }
    }

    public IEnumerator TypeDialog(string line)
    {
        _isTyping = true;
        _dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            _dialogText.text += letter;
            yield return new WaitForSeconds(1f / _letterPerSecond);
        }
        _isTyping = false;
    }
}