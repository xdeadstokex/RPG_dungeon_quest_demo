using Assets.Scripts.Core;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	private IGameStateService _state;
	private readonly Dictionary<GameState, ITickable> _handlers = new();


	public void Construct(IGameStateService state)
	{
		_state = state;
	}

	public void Register(GameState state, ITickable handler)
	{
		_handlers[state] = handler;
	}

    private void Update()
    {
        if (_handlers.TryGetValue((GameState)_state.Current, out var handler))
            handler.Tick();
    }

    //[SerializeField] private LayerMask _solidObjectLayer;
    //   [SerializeField] private LayerMask _interactiveLayer;
    //[SerializeField] private LayerMask _triggerZoneLayer;
    //   [SerializeField] private PlayerController _playerController;
    //   private GameState _state;


    //   private void Start()
    //   {
    //       DialogManager.Instance.OnShowDialog += HandleShowDialog;
    //       DialogManager.Instance.OnHideDialog += HandleHideDialog;
    //   }

    //   private void Update(){
    //       if(_state == GameState.FreeRoam){
    //		_playerController.CheckZone(_triggerZoneLayer);
    //		_playerController.HandleUpdate(_solidObjectLayer, _interactiveLayer);
    //		_playerController.ChangeScene(_triggerZoneLayer);
    //	}
    //       else if (_state == GameState.Dialog){ DialogManager.Instance.HandleUpdate(); }
    //       else if (_state == GameState.Battle){ ;}
    //   }


    //   //handler
    //   private void HandleShowDialog()
    //   {
    //       _state = GameState.Dialog;
    //       _playerController.StopMoving();
    //   }

    //   private void HandleHideDialog()
    //   {
    //       if (_state == GameState.Dialog)
    //           _state = GameState.FreeRoam;
    //   }
}
