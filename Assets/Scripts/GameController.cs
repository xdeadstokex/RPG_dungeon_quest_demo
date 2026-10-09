using UnityEngine;

public enum GameState { FreeRoam, Dialog, Battle}
public class GameController : MonoBehaviour
{

	[SerializeField] private LayerMask _solidObjectLayer;
    [SerializeField] private LayerMask _interactiveLayer;
	[SerializeField] private LayerMask _triggerZoneLayer;
    [SerializeField] private PlayerController _playerController;
    private GameState _state;


    private void Start()
    {
        DialogManager.Instance.OnShowDialog += HandleShowDialog;
        DialogManager.Instance.OnHideDialog += HandleHideDialog;
    }

    private void Update(){
        if(_state == GameState.FreeRoam){
			_playerController.CheckZone(_triggerZoneLayer);
			_playerController.HandleUpdate(_solidObjectLayer, _interactiveLayer);
		}
        else if (_state == GameState.Dialog){ DialogManager.Instance.HandleUpdate(); }
        else if (_state == GameState.Battle){ ;}
    }


    //handler
    private void HandleShowDialog()
    {
        _state = GameState.Dialog;
        _playerController.StopMoving();
    }

    private void HandleHideDialog()
    {
        if (_state == GameState.Dialog)
            _state = GameState.FreeRoam;
    }
}
