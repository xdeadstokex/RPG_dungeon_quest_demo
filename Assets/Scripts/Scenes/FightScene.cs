using Assets.Scripts.Core;
using Assets.Scripts.Infrastructure;
using UnityEngine;

public class FightScene : MonoBehaviour
{
    [SerializeField] private string _returnScene = "MainScene";
    private IInputService _input;
    private ISceneLoader _loader;

    private void Awake()
    {
        _input = new KeyboardInputService();
        _loader = SceneLoader.Shared;
    }

    private void Update()
    {
        if (_input.BackPressed)
            _loader.LoadWithTransition(_returnScene);
            
    }
}