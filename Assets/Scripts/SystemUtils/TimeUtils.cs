using UnityEngine;

public static class TimeUtils{

    public static float GetFrameTimeSec(){	return Time.deltaTime; }	// seconds since last frame
    public static float GetRunTimeSec(){	return Time.time;		}	// seconds since game start
    public static float GetTimeSpeed(){	return Time.timeScale; }	// 0 = 0x speed/game paused, 1 = 1x speed

}