using System;

public static class GameData
{
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


    public static Func<bool> PlayerMenuUp = () => KBInputUtils.KeyWClick() || KBInputUtils.KeyArrowUpClick();
    public static Func<bool> PlayerMenuDown = () => KBInputUtils.KeySClick() || KBInputUtils.KeyArrowDownClick();
    public static Func<bool> PlayerMenuLeft = () => KBInputUtils.KeyAClick() || KBInputUtils.KeyArrowLeftClick();
    public static Func<bool> PlayerMenuRight = () => KBInputUtils.KeyDClick() || KBInputUtils.KeyArrowRightClick();

}