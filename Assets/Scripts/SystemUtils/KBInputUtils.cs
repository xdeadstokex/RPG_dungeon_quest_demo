// Input.cs
using UnityEngine;
using UnityEngine.InputSystem;

public static class KBInputUtils{


    //KEY PRESSED
    //WASD
    public static bool KeyA(){ return Keyboard.current.aKey.isPressed; }
    public static bool KeyW(){ return Keyboard.current.wKey.isPressed; }
    public static bool KeyS(){ return Keyboard.current.sKey.isPressed; }
    public static bool KeyD(){ return Keyboard.current.dKey.isPressed; }

    //Arrows
    public static bool KeyArrowUp() { return Keyboard.current.upArrowKey.isPressed; }
    public static bool KeyArrowDown() { return Keyboard.current.downArrowKey.isPressed; }
    public static bool KeyArrowLeft() { return Keyboard.current.leftArrowKey.isPressed; }
    public static bool KeyArrowRight() { return Keyboard.current.rightArrowKey.isPressed; }

    //Others
    public static bool KeyE(){ return Keyboard.current.eKey.isPressed; }
    public static bool KeyF(){ return Keyboard.current.fKey.isPressed; }





    //KEY CLICKED
    // WASD
    public static bool KeyAClick() { return Keyboard.current.aKey.wasPressedThisFrame; }
    public static bool KeyWClick() { return Keyboard.current.wKey.wasPressedThisFrame; }
    public static bool KeySClick() { return Keyboard.current.sKey.wasPressedThisFrame; }
    public static bool KeyDClick() { return Keyboard.current.dKey.wasPressedThisFrame; }

    //Arrows
    public static bool KeyArrowUpClick() { return Keyboard.current.upArrowKey.wasPressedThisFrame; }
    public static bool KeyArrowDownClick() { return Keyboard.current.downArrowKey.wasPressedThisFrame; }
    public static bool KeyArrowLeftClick() { return Keyboard.current.leftArrowKey.wasPressedThisFrame; }
    public static bool KeyArrowRightClick() { return Keyboard.current.rightArrowKey.wasPressedThisFrame; }

    //Others
    public static bool KeyXClick(){ return Keyboard.current.xKey.wasPressedThisFrame; }
    public static bool KeyEClick(){ return Keyboard.current.eKey.wasPressedThisFrame; }
    public static bool KeyEscapeClick(){ return Keyboard.current.escapeKey.wasPressedThisFrame; }





    //KEY UNCLICK
    public static bool KeyAUnclick() { return Keyboard.current.aKey.wasReleasedThisFrame; }

}