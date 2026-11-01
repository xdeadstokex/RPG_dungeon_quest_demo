using System.Collections;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using System;

using UnityEngine.SceneManagement;
using static GameData;


[System.Serializable]
public class PlayerData //should be moved
{
	public LayerMask zoneLayer;

	public float popupOffsetX = 0;
	public float popupOffsetY = 0;

	public float maxSpeed = 50f;
	public float accel = 10f;
	public float decel = 20f;

	[System.NonSerialized]
	public Vector2 velocity;

	[System.NonSerialized]
	public Vector2 lastInput;
	
	[System.NonSerialized]
	public bool wasMoving;
	
	public Animator anim;

	[System.NonSerialized]
	public Action playerStop;

	[System.NonSerialized]
	public Transform transform; // ref
	
	[System.NonSerialized]
	public BoxCollider2D boxCollider;

	[System.NonSerialized]
	public GameObject obj;
}


[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class EntityHandler : MonoBehaviour
{
	[SerializeField] private PlayerData data;
    [SerializeField] private CharacterData _character;
    public CharacterData Character => _character;


    private void Awake()
    {
		data.obj = gameObject;
		data.boxCollider = GetComponent<BoxCollider2D>();
		data.transform = transform;
		data.playerStop = Stop;

		if (data.anim == null)
            data.anim = GetComponent<Animator>();
	}

	
    public void HandleMovement(){

        float dt = TimeUtils.GetFrameTimeSec();

        float x = 0f, y = 0f;

        if (PlayerMoveRight())     x =  1f;
        else if (PlayerMoveLeft()) x = -1f;
        if (PlayerMoveUp())        y =  1f;
        else if (PlayerMoveDown()) y = -1f;

        float angle;
        bool moving = true;

        if      (x ==  1 && y ==  0) angle = 0f;
        else if (x ==  1 && y ==  1) angle = Mathf.PI * 0.25f;
        else if (x ==  0 && y ==  1) angle = Mathf.PI * 0.5f;
        else if (x == -1 && y ==  1) angle = Mathf.PI * 0.75f;
        else if (x == -1 && y ==  0) angle = Mathf.PI;
        else if (x == -1 && y == -1) angle = Mathf.PI * 1.25f;
        else if (x ==  0 && y == -1) angle = Mathf.PI * 1.5f;
        else if (x ==  1 && y == -1) angle = Mathf.PI * 1.75f;
        else { angle = 0f; moving = false; }

        float s = Mathf.Sqrt(data.maxSpeed);
        Vector2 target = moving
            ? new Vector2(s * Mathf.Cos(angle), s * Mathf.Sin(angle))
            : Vector2.zero;

        float rate = moving ? data.accel : data.decel;

        data.velocity = Vector2.MoveTowards(data.velocity, target, rate * dt);

        moving = data.velocity.sqrMagnitude > 0.01f;

        Vector2 pos  = (Vector2)data.transform.position;
        Vector2 nextX = pos + new Vector2(data.velocity.x * dt, 0f);
        Vector2 nextY = pos + new Vector2(0f, data.velocity.y * dt);

        bool canMoveX = !CollisionHandler.IsInZoneType(nextX, data, data.zoneLayer, ZoneType.Solid);
        bool canMoveY = !CollisionHandler.IsInZoneType(nextY, data, data.zoneLayer, ZoneType.Solid);
        //Debug.Log(canMoveX + " " + canMoveY + " " + data.velocity);

        if (!canMoveX) data.velocity.x = 0f;
        if (!canMoveY) data.velocity.y = 0f;

        data.transform.position = pos
            + new Vector2(canMoveX ? data.velocity.x * dt : 0f,
                          canMoveY ? data.velocity.y * dt : 0f);

        moving = data.velocity.sqrMagnitude > 0.01f;

        data.wasMoving = moving;

        CollisionHandler.TriggerZoneFunction(this.data, data.zoneLayer);

        data.anim.SetBool("is_moving", moving);
        if (moving && data.velocity.sqrMagnitude > 0.01f){
            data.anim.SetFloat("mx", data.velocity.x);
            data.anim.SetFloat("my", data.velocity.y);
        }
    }


	public void Stop(){
		data.velocity = Vector2.zero;
		data.wasMoving = false;
		data.lastInput = Vector2.zero;
		data.anim.SetFloat("mx", 0);
        data.anim.SetFloat("my", 0);
	}

}