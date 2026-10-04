using System;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] private Dialog _dialog;
    public void Interact()
    {
        StartCoroutine(DialogManager.Instance.ShowDialog(_dialog));
    }
}
