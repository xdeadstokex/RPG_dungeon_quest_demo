using System;
using UnityEngine;
//using UnityEngine.InputSystem;

public static class GameData
{

    // use .ToString() to get scene name string
    public enum SceneNames
    {
        FightScene,
        MainScene,
        TransitionScene,
        None
    }

    public enum ZoneType
    {
        Portal,
        Teleport,
        Solid,
        Dialog,
        Fight
    }

    public static Func<bool> PlayerEscapeKey = KBInputUtils.KeyEscapeClick; // default key, can be remap
    public static Func<bool> PlayerDialogEscapeKey = KBInputUtils.KeyXClick;            // default key, can be remap
    public static Func<bool> PlayerInteractKey = KBInputUtils.KeyEClick;            // default key, can be remap


    public static Func<bool> PlayerMoveLeft = () => KBInputUtils.KeyA() || KBInputUtils.KeyArrowLeft();
    public static Func<bool> PlayerMoveRight = () => KBInputUtils.KeyD() || KBInputUtils.KeyArrowRight();
    public static Func<bool> PlayerMoveUp = () => KBInputUtils.KeyW() || KBInputUtils.KeyArrowUp();
    public static Func<bool> PlayerMoveDown = () => KBInputUtils.KeyS() || KBInputUtils.KeyArrowDown();


    // menu navigation (pressed this frame, not held). confirm = PlayerInteractKey, back = PlayerDialogEscapeKey
    public static Func<bool> PlayerMenuUp = () => KBInputUtils.KeyWClick() || KBInputUtils.KeyArrowUpClick();
    public static Func<bool> PlayerMenuDown = () => KBInputUtils.KeySClick() || KBInputUtils.KeyArrowDownClick();
    public static Func<bool> PlayerMenuLeft = () => KBInputUtils.KeyAClick() || KBInputUtils.KeyArrowLeftClick();
    public static Func<bool> PlayerMenuRight = () => KBInputUtils.KeyDClick() || KBInputUtils.KeyArrowRightClick();




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