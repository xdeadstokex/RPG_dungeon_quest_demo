public enum SceneNames{
	FightScene,
	MainScene,
	TransitionScene,
	None
}

public enum ZoneType{
	Portal,
	Teleport,
	Solid,
	Dialog,
	Fight
}

public enum TriggerType { Click, Enter }


// follow the order of the list inside GlobalData
public enum InputSignal{
    A = 0,
    W = 1,
    S = 2,
    D = 3,
    Esc = 4,

    ArrowUp = 5,
    ArrowDown = 6,
    ArrowLeft = 7,
    ArrowRight = 8,

    E = 9,
    F = 10,

    AClick = 11,
    WClick = 12,
    SClick = 13,
    DClick = 14,
    XClick = 15,
    EClick = 16,

    ArrowUpClick = 17,
    ArrowDownClick = 18,
    ArrowLeftClick = 19,
    ArrowRightClick = 20,

    AUnclick = 21,

    None = 9999
}


public enum PopupType{
	FollowPlayer,
	FollowObj,
	ZonePopup
}