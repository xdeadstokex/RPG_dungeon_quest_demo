using UnityEngine;
using UnityEngine.SceneManagement;

public class FightScene : MonoBehaviour
{
    private void Update()
    {
        if (KBInputUtils.KeyEscapeClick())
        {
            TransitionScene.nextSceneName = "MainScene";
            SceneManager.LoadSceneAsync("TransitionScene");
        }
    }
}
