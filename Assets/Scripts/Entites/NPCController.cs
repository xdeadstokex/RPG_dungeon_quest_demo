using Assets.Scripts.Core;
using System;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    [SerializeField] private Dialog _dialog;
    private IDialogService _dialogService;

    public void Construct(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public void Interact()
    {
        _dialogService.Show(_dialog);
    }
}
