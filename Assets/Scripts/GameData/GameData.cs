using System;
using UnityEngine;
//using UnityEngine.InputSystem;

public static class GameData{

	// use .ToString() to get scene name string
	public enum SceneNames{
		FightScene,
		MainScene,
		TransitionScene,
		None
	}

	public static Func<bool> PlayerEscapeKey =			KBInputUtils.KeyEscapeClick;	// default key, can be remap
	public static Func<bool> PlayerDialogEscapeKey =	KBInputUtils.KeyXClick;			// default key, can be remap
	public static Func<bool> PlayerInteractKey =		KBInputUtils.KeyEClick;			// default key, can be remap

	public static Func<Vector2> PlayerMovementInputVetor = PlayerMove;

	private static Vector2 PlayerMove(){
		float x = 0f, y = 0f;
		if (KBInputUtils.KeyD() || KBInputUtils.KeyArrowRight()){ x = 1f; }
		else if (KBInputUtils.KeyA() || KBInputUtils.KeyArrowLeft()){ x = -1f; }
		if (KBInputUtils.KeyW() || KBInputUtils.KeyArrowUp()){ y = 1f; }
		else if (KBInputUtils.KeyS() || KBInputUtils.KeyArrowDown()){ y = -1f; }

		return new Vector2(x, y);
	}


    // Enums
    //public enum GameState { Menu, Fight, Transition }
    //public enum Team      { Red, Blue }

	/*
using System;

Func<int, bool>           // 1 input, 1 return
Func<int, int, bool>      // 2 input, 1 return
Action<int, int>          // 2 input, void return
Action<int>               // 1 input, void return
Action                    // no input, void return

*/

}