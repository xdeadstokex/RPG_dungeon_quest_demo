using UnityEngine;

public enum GameState { FreeRoam, Dialog, Battle}
public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    private GameState _state;


    private void Start()
    {
        DialogManager.Instance.OnShowDialog += () => { _state = GameState.Dialog; };
        DialogManager.Instance.OnHideDialog += () =>
        {
            if (_state == GameState.Dialog)
                _state = GameState.FreeRoam;
        };
    }

    private void Update()
    {
        if (_state == GameState.FreeRoam)
            _playerController.HandleUpdate();
        else if (_state == GameState.Dialog)
            DialogManager.Instance.HandleUpdate();
        else if (_state == GameState.Battle)
        {

        }
    }
}
