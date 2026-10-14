using Assets.Scripts.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChallengerController : MonoBehaviour, IInteractable
{
    [SerializeField] private string _battleScene = "FightScene";
    private ISceneLoader _loader;
    private bool _isLoading;


    public void Construct(ISceneLoader loader)
    {
        _loader = loader;
    }

    public void Interact()
    {
        if (_isLoading) 
            return;
        _isLoading = true;

        _loader.LoadWithTransition(_battleScene);
    }
}
