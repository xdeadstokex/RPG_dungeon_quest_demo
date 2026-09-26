using System.Collections;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {
	[Header("Movement speed")]
    public float vx = 10;
    public float vy = 10;

	[Header("Trasition")]
    public bool activeMovingTransition = false;
    public float transitionTimeSec = 1;

	[Header("Collision")]
	[SerializeField] private LayerMask solidObjectLayer;
	[SerializeField] private float collisionCheckRadius;

    [Header("Input")]
    public float inputStopThreshold = 0.1f;
    private float inputStopTimer = 0;
	private Vector2 currentInput;
	private Vector2 lastActiveInput;
    private bool wasMoving = false;
    private bool isDecelerating;
    //private Vector2 input;

    private Animator playerAnimation;


    private void Awake(){
		playerAnimation = GetComponent<Animator>();
	}

    private void Update()
    {
        float dt = Time.deltaTime;
        currentInput = ReadBufferedInput(dt);
        bool isMoving = currentInput != Vector2.zero;

        if (isMoving)
            HandleActiveMovement(dt);
        else
            HandleStopMovement();

        wasMoving = isMoving;
    }


    //Input
    private Vector2 ReadBufferedInput(float dt)
    {
        float rawX = Input.GetAxis("Horizontal");
        float rawY = Input.GetAxis("Vertical");
        bool hasRawInput = rawX != 0f || rawY != 0f;

        //has input
        if (hasRawInput)
        {
            inputStopTimer = 0f;
            return new Vector2(rawX, rawY);
        }
        //transition before stop
        if (inputStopTimer < inputStopThreshold)
        {
            inputStopTimer += dt;
            return currentInput;
        }
        return Vector2.zero;
    }

    //Handler
    private void HandleActiveMovement(float dt)
    {
        if (!wasMoving)
            isDecelerating = false;

        lastActiveInput = currentInput;
        UpdateAnimation(currentInput, true);
        TryMove(currentInput.x * vx * dt, currentInput.y * vy * dt);
    }

    private void HandleStopMovement()
    {
        if (isDecelerating)
            return;

        isDecelerating = true;
        float startVx = lastActiveInput.x * vx;
        float startVy = lastActiveInput.y * vy;
        StartCoroutine(DecelerateCoroutine(startVx, startVy, transitionTimeSec));
    }


    //Slow down coroutine
    private IEnumerator DecelerateCoroutine(float startVx, float startVy, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (!isDecelerating)
                yield break;
            float dt = Time.deltaTime;
            elapsed += dt;

            float t = 1f - (elapsed / duration);
            t += t;

            bool moved = TryMove(startVx * t * dt, startVy * t * dt);

            if (t <= 0.1f || !moved)
            {
                UpdateAnimation(Vector2.zero, false);
                if (!moved)
                    yield break;
            }
               
            yield return null;
        }
	}

    //Move and Collision

    //check each exis
    private bool TryMove(float dx, float dy)
    {
        Vector3 targetPos = transform.position;
        bool moved = false;
        if (dx != 0f)
        {
            Vector3 targetX = targetPos;
            targetX.x += dx;
            if (IsWalkable(targetX))
            {
                targetPos.x = targetX.x;
                moved = true;
            }
        }
        if (dy != 0f)
        {
            Vector3 targetY = targetPos;
            targetY.y += dy;
            if (IsWalkable(targetY))
            {
                targetPos.y = targetY.y;
                moved = true;
            }
        }
        
        transform.position = targetPos;
        return moved;
    }


	private bool IsWalkable(Vector3 targetPos)
	{
		if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectLayer))
			return false;
		return true;
	}
    
    // Play animation
    private void UpdateAnimation(Vector2 direction, bool moving)
    {
        playerAnimation.SetBool("is_moving", moving);

        if (moving)
        {
            playerAnimation.SetFloat("mx", direction.x);
            playerAnimation.SetFloat("my", direction.y);
        }
    }


}
