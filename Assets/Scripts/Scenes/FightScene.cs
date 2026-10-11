using UnityEngine;
using UnityEngine.SceneManagement;

public class FightScene : MonoBehaviour
{
    private bool _isLeaving;

    private void Update()
    {
        if (_isLeaving) return;

        if (KBInputUtils.KeyEscapeClick())
        {
            _isLeaving = true;
            TransitionScene.nextSceneName = "MainScene";
            SceneManager.LoadScene("TransitionScene");
        }
    }
}
