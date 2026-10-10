// Input.cs
using UnityEngine;
using UnityEngine.InputSystem;

public static class MouseInputUtils{

	// screen x,y from top left
	public static float MouseX(){ return Mouse.current.position.ReadValue().x; }
	public static float MouseY(){ return Mouse.current.position.ReadValue().y; }

	// might be -120 -> 120, it depend, test before usage
	public static float ScrollX(){ return Mouse.current.scroll.ReadValue().x; }
	public static float ScrollY(){ return Mouse.current.scroll.ReadValue().y; }


	// mouse hold
	public static bool MouseLeft(){ return Mouse.current.leftButton.isPressed; }
	public static bool MouseRight(){ return Mouse.current.rightButton.isPressed; }
	public static bool MouseMiddle(){ return Mouse.current.middleButton.isPressed; }

	// mouse click
	public static bool MouseLeftClick(){ return Mouse.current.leftButton.wasPressedThisFrame; }
	public static bool MouseRightClick(){ return Mouse.current.rightButton.wasPressedThisFrame; }
	public static bool MouseMiddleClick(){ return Mouse.current.middleButton.wasPressedThisFrame; }

	// mouse unclick
	public static bool MouseLeftUnclick(){ return Mouse.current.leftButton.wasReleasedThisFrame; }
	public static bool MouseRightUnclick(){ return Mouse.current.rightButton.wasReleasedThisFrame; }
	public static bool MouseMiddleUnclick(){ return Mouse.current.middleButton.wasReleasedThisFrame; }

}