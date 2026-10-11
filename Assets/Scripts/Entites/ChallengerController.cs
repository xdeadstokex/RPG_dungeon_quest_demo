using UnityEngine;
using UnityEngine.SceneManagement;

public class ChallengerController : MonoBehaviour, Interactable
{
    public void Interact()
    {
        Debug.Log("You will start a battle");
        TransitionScene.nextSceneName = "FightScene";
        SceneManager.LoadSceneAsync("TransitionScene");
    }
}
