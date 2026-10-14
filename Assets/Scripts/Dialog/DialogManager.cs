using Assets.Scripts.Core;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour, IDialogService, ITickable
{
    [SerializeField] private GameObject _dialogBox;
    [SerializeField] private Text _dialogText;
    [SerializeField] private int _letterPerSecond;

    private IInputService _input;
    private Dialog _dialog;
    private int _currentLine = 0;
    private bool _isTyping;

    public event Action OnOpened;
    public event Action OnClosed;


    public void Construct(IInputService input)
    {
        _input = input;
    }

    public void Show(Dialog dialog)
    {
        StartCoroutine(ShowRoutine(dialog));
    }

    public IEnumerator ShowRoutine(Dialog dialog)
    {
        yield return new WaitForEndOfFrame();
        _dialog = dialog;
        _currentLine = 0;
        _dialogBox.SetActive(true);
        OnOpened?.Invoke();
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public void Tick()
    {
        if (_input.CancelPressed)
        {
            Close();
            return;
        }
        if (_input.InteractPressed && !_isTyping)
            Next();
    }

    private void Next()
    {
        if (++_currentLine < _dialog.Lines.Count)
            StartCoroutine(TypeDialog(_dialog.Lines[_currentLine]));
        else
            Close();
    }

    private void Close()
    {
        _currentLine = 0;
        _dialogBox.SetActive(false);
        OnClosed?.Invoke();
    }

    private IEnumerator TypeDialog(string line)
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

    //public static DialogManager Instance { get; private set; }

    //private void Awake()
    //{
    //    Instance = this;
    //}

    //public void HandleUpdate()
    //{
    //    HandleSkipDialog();
    //    HandleNextLine();

    //}


    //private void HandleNextLine()
    //{
    //    if (IsKeyEPressed() && !_isTyping)
    //    {
    //        ++_currentLine;
    //        if (_currentLine < _dialog.Lines.Count)
    //        {
    //            StartCoroutine(TypeDialog(_dialog.Lines[_currentLine]));
    //        }
    //        else
    //        {
    //            _currentLine = 0;
    //            _dialogBox.SetActive(false);
    //            OnHideDialog?.Invoke();
    //        }
    //    }
    //}

    //private void HandleSkipDialog()
    //{
    //    if (IsKeyXPressed() && _dialogBox.activeSelf)
    //    {
    //        _currentLine = 0;
    //        _dialogBox.SetActive(false);
    //        OnHideDialog?.Invoke();
    //        return;
    //    }
    //}

    //public IEnumerator ShowDialog(Dialog dialog)
    //{
    //    yield return new WaitForEndOfFrame();
    //    OnShowDialog?.Invoke();

    //    this._dialog = dialog;
    //    _dialogBox.SetActive(true);
    //    StartCoroutine(TypeDialog(dialog.Lines[0]));
    //}



    //private bool IsKeyEPressed() { return KBInputUtils.KeyEClick(); }
    //private bool IsKeyXPressed() { return KBInputUtils.KeyXClick();}
}