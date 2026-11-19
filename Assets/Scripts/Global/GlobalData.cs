using UnityEngine;
using System;
using static KBInputUtils;


	[System.Serializable]
	public class SignalSlot{
		public InputSignal data;
		[System.NonSerialized]
		public Func<bool> func;
	}


public class SignalPool{
    private SignalSlot[] _signalPool;

    public SignalPool(SignalSlot[] signalPool){
		_signalPool = signalPool;
    }

    //public InputSignal this[int idx]{
    //    get => _signalPool[idx].data;
    //    set => _signalPool[idx].data = value;
    //}

	public void Remap(){
		for (int i = 0; i < _signalPool.Length; i += 1){
			_signalPool[i].func = GlobalData.KeyPool[(int)_signalPool[i].data];
		}
		return;
	}

}


[CreateAssetMenu(fileName = "GlobalData", menuName = "Game/GlobalData")]
public class GlobalData : ScriptableObject{

//===============================================
//====================  SETUP   =================
//===============================================

private static GlobalData _instance; // do not call directly except for inside data() func

// call GlobalData.data().<anyfield>
public static GlobalData data(){
        if (_instance == null){
            _instance = Resources.Load<GlobalData>("Data/GlobalData");
			if(_instance == null){
				Debug.LogError($"GlobalData not found in Resources/");
			}
		}
        return _instance;
}


//===============================================
//===================  DATA   ===================
//===============================================
// NOTE: calling to all DATA should be GlobalData.<fieldname>

[Header("Const collision layer")]
    public LayerMask _globalLayer;


[Header("Player's signal mapping")]
	public SignalSlot _playerEscape = new SignalSlot{ data = InputSignal.A, func = null };
	public SignalSlot _playerMoveLeft = new SignalSlot{ data = InputSignal.None, func = null };
	public SignalSlot _playerMoveRight = new SignalSlot{ data = InputSignal.None, func = null };
	public SignalSlot _playerMoveUp = new SignalSlot{ data = InputSignal.None, func = null };
	public SignalSlot _playerMoveDown = new SignalSlot{ data = InputSignal.None, func = null };


	public SignalSlot _playerDialogEscape = new SignalSlot{ data = InputSignal.XClick, func = null };
	public SignalSlot _playerInteract = new SignalSlot{ data = InputSignal.EClick, func = null };

	public SignalSlot _playerMenuUp = new SignalSlot{ data = InputSignal.None, func = null };
	public SignalSlot _playerMenuDown = new SignalSlot{ data = InputSignal.None, func = null };
	public SignalSlot _playerMenuLeft = new SignalSlot{ data = InputSignal.None, func = null };
	public SignalSlot _playerMenuRight = new SignalSlot{ data = InputSignal.None, func = null };



// below fields are non serialized field

	// usage:	GlobalData.playerSignalPool[0];					// get
	//			GlobalData.playerSignalPool[0] = InputSignal.B; // set
	public static SignalPool _playerSignalPool = null;

	public static Func<bool>[] KeyPool = {
		KeyA,               // 0
		KeyW,               // 1
		KeyS,               // 2
		KeyD,               // 3
		KeyEscapeClick,     // 4
		KeyArrowUp,         // 5
		KeyArrowDown,       // 6
		KeyArrowLeft,      // 7
		KeyArrowRight,     // 8
		KeyE,               // 9
		KeyF,               // 10
		KeyAClick,          // 11
		KeyWClick,          // 12
		KeySClick,          // 13
		KeyDClick,          // 14
		KeyXClick,          // 15
		KeyEClick,          // 16
		KeyArrowUpClick,    // 17
		KeyArrowDownClick,  // 18
		KeyArrowLeftClick,  // 19
		KeyArrowRightClick, // 20
		KeyAUnclick         // 21
	};



//===============================================
//================  WRAPPER   ===================
//===============================================
/*
GOAL:
To make calling to field from:
GlobalData.data().<fieldname>
to:
GlobalData.<fieldname>
*/

   public static int globalLayer{
        get => GlobalData.data()._globalLayer;
        set => GlobalData.data()._globalLayer = value;
    }


// SIGNAL
public static InputSignal playerEscape{
    get => GlobalData.data()._playerEscape.data;
    set => GlobalData.data()._playerEscape.data = value;
}

public static InputSignal playerMoveLeft{
    get => GlobalData.data()._playerMoveLeft.data;
    set => GlobalData.data()._playerMoveLeft.data = value;
}

public static InputSignal playerMoveRight{
    get => GlobalData.data()._playerMoveRight.data;
    set => GlobalData.data()._playerMoveRight.data = value;
}

public static InputSignal playerMoveUp{
    get => GlobalData.data()._playerMoveUp.data;
    set => GlobalData.data()._playerMoveUp.data = value;
}

public static InputSignal playerMoveDown{
    get => GlobalData.data()._playerMoveDown.data;
    set => GlobalData.data()._playerMoveDown.data = value;
}

public static InputSignal playerDialogEscape{
    get => GlobalData.data()._playerDialogEscape.data;
    set => GlobalData.data()._playerDialogEscape.data = value;
}

public static InputSignal playerInteract{
    get => GlobalData.data()._playerInteract.data;
    set => GlobalData.data()._playerInteract.data = value;
}

public static InputSignal playerMenuUp{
    get => GlobalData.data()._playerMenuUp.data;
    set => GlobalData.data()._playerMenuUp.data = value;
}

public static InputSignal playerMenuDown{
    get => GlobalData.data()._playerMenuDown.data;
    set => GlobalData.data()._playerMenuDown.data = value;
}

public static InputSignal playerMenuLeft{
    get => GlobalData.data()._playerMenuLeft.data;
    set => GlobalData.data()._playerMenuLeft.data = value;
}

public static InputSignal playerMenuRight{
    get => GlobalData.data()._playerMenuRight.data;
    set => GlobalData.data()._playerMenuRight.data = value;
}


// FUNCTIONS
public static Func<bool> PlayerEscapeKey{
    get => GlobalData.data()._playerEscape.func;
    set => GlobalData.data()._playerEscape.func = value;
}

public static Func<bool> PlayerMoveLeftKey{
    get => GlobalData.data()._playerMoveLeft.func;
    set => GlobalData.data()._playerMoveLeft.func = value;
}

public static Func<bool> PlayerMoveRightKey{
    get => GlobalData.data()._playerMoveRight.func;
    set => GlobalData.data()._playerMoveRight.func = value;
}

public static Func<bool> PlayerMoveUpKey{
    get => GlobalData.data()._playerMoveUp.func;
    set => GlobalData.data()._playerMoveUp.func = value;
}

public static Func<bool> PlayerMoveDownKey{
    get => GlobalData.data()._playerMoveDown.func;
    set => GlobalData.data()._playerMoveDown.func = value;
}

public static Func<bool> PlayerDialogEscapeKey{
    get => GlobalData.data()._playerDialogEscape.func;
    set => GlobalData.data()._playerDialogEscape.func = value;
}

public static Func<bool> PlayerInteractKey{
    get => GlobalData.data()._playerInteract.func;
    set => GlobalData.data()._playerInteract.func = value;
}

public static Func<bool> PlayerMenuUpKey{
    get => GlobalData.data()._playerMenuUp.func;
    set => GlobalData.data()._playerMenuUp.func = value;
}

public static Func<bool> PlayerMenuDownKey{
    get => GlobalData.data()._playerMenuDown.func;
    set => GlobalData.data()._playerMenuDown.func = value;
}

public static Func<bool> PlayerMenuLeftKey{
    get => GlobalData.data()._playerMenuLeft.func;
    set => GlobalData.data()._playerMenuLeft.func = value;
}

public static Func<bool> PlayerMenuRightKey{
    get => GlobalData.data()._playerMenuRight.func;
    set => GlobalData.data()._playerMenuRight.func = value;
}


	public static SignalPool playerSignalPool{
		get{
			if(_playerSignalPool == null){
				_playerSignalPool = new SignalPool(
					new SignalSlot[]{
						GlobalData.data()._playerEscape,
						GlobalData.data()._playerMoveLeft,
						GlobalData.data()._playerMoveRight,
						GlobalData.data()._playerMoveUp,
						GlobalData.data()._playerMoveDown,
						GlobalData.data()._playerDialogEscape,
						GlobalData.data()._playerInteract,
						GlobalData.data()._playerMenuUp,
						GlobalData.data()._playerMenuDown,
						GlobalData.data()._playerMenuLeft,
						GlobalData.data()._playerMenuRight
					}
				);
			}
			return _playerSignalPool;
		}
	}

}