using Assets.Scripts.Core;
using Assets.Scripts.Infrastructure;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightScene : MonoBehaviour
{
    private readonly IInputService _input = new KeyboardInputService();
    private bool _isLeaving;


    private void Update()
    {
        if (_isLeaving) 
            return;

        if (_input.BackPressed)
        {
            _isLeaving = true;
            TransitionScene.nextSceneName = "MainScene";
            SceneManager.LoadScene("TransitionScene");
        }
    }
}
