using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float moveSpeed;
    //private bool isMoving;
    private Vector2 input;
	
	public Animator p_ani;
	
	public float vx = 10;
	public float vy = 10;
	public float transition_time_sec = 1;

	public float last_inputx = 0;
	public float last_inputy = 0;
	public float last_active_inputx = 0;
	public float last_active_inputy = 0;	
    public bool last_moving = false;
    public bool active_moving_trasition = false;
	public float input_stop_dt_sec = 0;

	private Vector2 get_player_pos(){
		return transform.position;
	}

	public float get_dt_frame_sec(){
		return Time.deltaTime;
	}

	private void move_player_pos(float dx, float dy){
		var pos2d = transform.position;
		pos2d.x += dx;
		pos2d.y += dy;
		transform.position = pos2d;
		return;
	}

	private void Awake(){
		this.p_ani = GetComponent<Animator>();
		return;
	}




	// rework damn movement input to unified



    private void Update(){
        //if (isMoving){ return; }
		float frame_time = get_dt_frame_sec();


		float inputx = Input.GetAxisRaw("Horizontal");
		float inputy = Input.GetAxisRaw("Vertical");

		if(inputx == 0f && inputy == 0f){
			if(input_stop_dt_sec < 0.1){
				input_stop_dt_sec += get_dt_frame_sec();
			}
			else{
				input.x = inputx;
				input.y = inputy;
			}
		}
		else{
			this.input_stop_dt_sec = 0;
			input.x = inputx;
			input.y = inputy;
		}
		//Debug.Log("This is input.x" + input.x);
		//Debug.Log("This is input.y" + input.y);

		bool check_moving = false;
		if(input != Vector2.zero){
			check_moving = true;

			if(!this.last_moving){
				active_moving_trasition = false;	
			}

			this.p_ani.SetBool("is_moving", true);
			this.p_ani.SetFloat("mx", input.x);
			this.p_ani.SetFloat("my", input.y);

			//move_player_pos(input.x, input.y);
			//StartCoroutine(Move(targetPos)); // this line to make player move
			//StartCoroutine(handle_player_movement(input.x * 2, input.y * 2, 1)); // this line to make player move

			move_player_pos(input.x * vx * frame_time, input.y * vy * frame_time);
			this.last_active_inputx = input.x;
			this.last_active_inputy = input.y;
		}
		else{
			// will continue normal movement on not enough stop time, else true stop
			if(!active_moving_trasition){
				active_moving_trasition = true;
				StartCoroutine(handle_player_movement_transition(last_active_inputx * vx, last_active_inputy * vy, transition_time_sec));
				Debug.Log("is stopping");		
			}
		}
		
		this.last_inputx = input.x;
		this.last_inputy = input.y;
		this.last_moving = check_moving;
		return;
    }



	
	// buggy and outdated, still need to research sth here
	/*
    private IEnumerator Move(Vector3 targetPos){
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon){
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
		// end func
    }
	*/
	
	IEnumerator handle_player_movement_transition(float vx, float vy, float dt_sec){
		Debug.Log("is stopping start");
		//this.isMoving = true;
		
		float orig_vx = vx;
		float orig_vy = vy;
		float dt_current = 0;
		for(;dt_current < dt_sec;){
			if(!active_moving_trasition){ yield break; }
			float frame_time = get_dt_frame_sec();
			dt_current += frame_time;
			float t = 1f - (dt_current / dt_sec); // linear ease-out
			t = t * t;
			move_player_pos(orig_vx * t * frame_time, orig_vy * t * frame_time);
			Debug.Log("is stopping vx " + orig_vx * t + " vy " + orig_vy * t);
			if(t <= 0.1){ this.p_ani.SetBool("is_moving", false); }
			yield return null;
		}
        //isMoving = false;
	}


}
