// Input.cs
using UnityEngine;
using UnityEngine.InputSystem;

public static class KBInputUtils{


public static bool KeyA(){ return Keyboard.current.aKey.isPressed; }
public static bool KeyW(){ return Keyboard.current.wKey.isPressed; }
public static bool KeyS(){ return Keyboard.current.sKey.isPressed; }
public static bool KeyD(){ return Keyboard.current.dKey.isPressed; }
public static bool KeyE(){ return Keyboard.current.eKey.isPressed; }
public static bool KeyF(){ return Keyboard.current.fKey.isPressed; }




public static bool KeyAClick(){
    return Keyboard.current.aKey.wasPressedThisFrame;
}

public static bool KeyAUnclick(){
    return Keyboard.current.aKey.wasReleasedThisFrame;
}




public static bool KeyArrowUp(){
    return Keyboard.current.upArrowKey.isPressed;
}


public static bool KeyArrowDown(){
    return Keyboard.current.downArrowKey.isPressed;
}

public static bool KeyArrowLeft(){
    return Keyboard.current.leftArrowKey.isPressed;
}

public static bool KeyArrowRight(){
    return Keyboard.current.rightArrowKey.isPressed;
}

/*
// Arrow keys
Keyboard.current.upArrowKey
Keyboard.current.downArrowKey
Keyboard.current.leftArrowKey
Keyboard.current.rightArrowKey

// Modifiers
Keyboard.current.shiftKey
Keyboard.current.ctrlKey
Keyboard.current.altKey

// Numbers
Keyboard.current.digit1Key
Keyboard.current.digit2Key

// Others
Keyboard.current.spaceKey
Keyboard.current.enterKey
Keyboard.current.escapeKey
*/


/*
old system 1f or -1f output

        //float rawX = Input.GetAxis("Horizontal");
        //float rawY = Input.GetAxis("Vertical");

Input.GetKeyDown(KeyCode.E)
*/

}