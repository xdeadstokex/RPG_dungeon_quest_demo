using UnityEngine;
using UnityEngine.SceneManagement;

public class ChallengerController : MonoBehaviour, Interactable
{
    private bool _isLoading;
    public void Interact()
    {
        if (_isLoading) return;
        _isLoading = true;

        Debug.Log("You will start a battle");
        TransitionScene.nextSceneName = "FightScene";
        SceneManager.LoadScene("TransitionScene");
    }
}
