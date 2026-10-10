using System.Collections;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;


using UnityEngine.SceneManagement;



[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {
	[Header("Movement speed")]
    public float vx = 10;
    public float vy = 10;

	[Header("Trasition")]
    public bool activeMovingTransition = false;
    public float transitionTimeSec = 1;

	[Header("Collision")]
	[SerializeField] private float _collisionCheckRadius;

    [Header("Input")]
    public float inputStopThreshold = 0.1f;
    private float _inputStopTimer = 0;
	private Vector2 _currentInput; 
	private Vector2 _lastActiveInput;
    private bool _wasMoving = false;
    private bool _isDecelerating;
    private Vector2 _facingDir = Vector2.down;

    private Animator _playerAnimation;


    private void Awake(){
		_playerAnimation = GetComponent<Animator>();
	}


//	private void Update(){
//		HandleUpdate();
//	}


    public void HandleUpdate(LayerMask solidObjectLayer, LayerMask interactiveLayer)
    {
        float dt = Time.deltaTime;
        _currentInput = ReadBufferedInput(dt);
        bool isMoving = _currentInput != Vector2.zero;

        if (isMoving)
            HandleActiveMovement(dt, solidObjectLayer, interactiveLayer);
        else
            HandleStopMovement(solidObjectLayer, interactiveLayer);

        _wasMoving = isMoving;

        if (Input.GetKeyDown(KeyCode.E))
            Interact(interactiveLayer);
		//CheckZone();
		return;
    }

    private void Interact(LayerMask interactiveLayer)
    {
        Vector3 interactPos = transform.position + (Vector3)_facingDir;
        //Debug.DrawLine(transform.position, interactPos, Color.red, 1f);
        var collider = Physics2D.OverlapCircle(interactPos, 0.2f, interactiveLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
        }
    }

    public void StopMoving()
    {
        StopAllCoroutines();
        _currentInput = Vector2.zero;
        _lastActiveInput = Vector2.zero;
        _inputStopTimer = inputStopThreshold;
        _wasMoving = false;
        _isDecelerating = true;

        _playerAnimation.SetBool("is_moving", false);
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
            _inputStopTimer = 0f;
            return new Vector2(rawX, rawY);
        }
        //transition before stop
        if (_inputStopTimer < inputStopThreshold)
        {
            _inputStopTimer += dt;
            return _currentInput;
        }
        return Vector2.zero;
    }

    //Handler
    private void HandleActiveMovement(float dt, LayerMask solidObjectLayer, LayerMask interactiveLayer)
    {
        if (!_wasMoving)
            _isDecelerating = false;

        _lastActiveInput = _currentInput;
        UpdateAnimation(_currentInput, true);
        TryMove(_currentInput.x * vx * dt, _currentInput.y * vy * dt, solidObjectLayer, interactiveLayer);
    }

    private void HandleStopMovement(LayerMask solidObjectLayer, LayerMask interactiveLayer)
    {
        if (_isDecelerating)
            return;

        _isDecelerating = true;
        float startVx = _lastActiveInput.x * vx;
        float startVy = _lastActiveInput.y * vy;
        StartCoroutine(DecelerateCoroutine(startVx, startVy, transitionTimeSec, solidObjectLayer, interactiveLayer));
    }


    //Slow down coroutine
    private IEnumerator DecelerateCoroutine(float startVx, float startVy, float duration, LayerMask solidObjectLayer, LayerMask interactiveLayer)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (!_isDecelerating)
                yield break;
            float dt = Time.deltaTime;
            elapsed += dt;

            float t = 1f - (elapsed / duration);

            bool moved = TryMove(startVx * t * dt, startVy * t * dt, solidObjectLayer, interactiveLayer);

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
    //check each axis
    private bool TryMove(float dx, float dy, LayerMask solidObjectLayer, LayerMask interactiveLayer)
    {
        Vector3 targetPos = transform.position;
        bool moved = false;
        if (dx != 0f)
        {
            Vector3 targetX = targetPos;
            targetX.x += dx;
            if (IsWalkable(targetX, solidObjectLayer, interactiveLayer))
            {
                targetPos.x = targetX.x;
                moved = true;
            }
        }
        if (dy != 0f)
        {
            Vector3 targetY = targetPos;
            targetY.y += dy;
            if (IsWalkable(targetY, solidObjectLayer, interactiveLayer))
            {
                targetPos.y = targetY.y;
                moved = true;
            }
        }
        
        transform.position = targetPos;
        return moved;
    }


	private bool IsWalkable(Vector3 targetPos, LayerMask solidObjectLayer, LayerMask interactiveLayer)
	{
		if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectLayer | interactiveLayer))
			return false;
		return true;
	}
    
    // Play animation
    private void UpdateAnimation(Vector2 direction, bool moving)
    {
        _playerAnimation.SetBool("is_moving", moving);

        if (moving && direction.sqrMagnitude > 0.01f)
        {
            _facingDir = GetFacingDir(direction);
            _playerAnimation.SetFloat("mx", direction.x);
            _playerAnimation.SetFloat("my", direction.y);
        }
    }

    private Vector2 GetFacingDir(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            return new Vector2(Mathf.Sign(direction.x), 0);
        else
            return new Vector2(0, Mathf.Sign(direction.y));
    }


	[SerializeField] private GameObject popupIcon;

	bool _inZone = false;
	string oldZoneName = "";

	public void CheckZone(LayerMask triggerZone){
		Collider2D hitObject = Physics2D.OverlapCircle(transform.position, 0.5f, triggerZone);
		bool inside = hitObject != null;
		
		if (inside && !_inZone){
			_inZone = true;
			popupIcon.SetActive(true);
			Debug.Log("entered");
			Debug.Log(hitObject.gameObject.name);
			oldZoneName = hitObject.gameObject.name;
		}
		else if (!inside && _inZone){
			_inZone = false;
			popupIcon.SetActive(false);
			Debug.Log("fuck you, get out!");
			Debug.Log(oldZoneName);
		}

		return;
	}

	public void ChangeScene(LayerMask triggerZone){
		Collider2D hitObject = Physics2D.OverlapCircle(transform.position, 0.5f, triggerZone);
		bool inside = hitObject != null;
		
		if(inside && hitObject.gameObject.name == "SceneChangingTestZone"){
			SceneManager.LoadScene("Test");
		}
		
		return;
	}
}