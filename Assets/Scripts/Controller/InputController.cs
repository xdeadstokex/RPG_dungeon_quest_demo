using UnityEngine;
using System.Collections.Generic; // List

using static GameData;
using UnityEngine.SceneManagement;

public class InputController : MonoBehaviour{

	private static InputController _instance = null;

	[SerializeField] private LayerMask zoneLayer;
	[SerializeField] private List<GameObject> controlledParty = new();
	[SerializeField] private Camera mainCam = null;


    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }


    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }


    private void Awake()
	{
		if(_instance != null && _instance != this)
		{
			Destroy(gameObject);
			return;
		} 
		_instance = this;
		DontDestroyOnLoad(gameObject);
		       
		if(_instance.mainCam == null)
		{
			_instance.mainCam = Camera.main;
			if(controlledParty.Count == 1)
				SetCamParent(controlledParty[0], mainCam);
		}

	}


    private void Update()
    {
        FightSceneProgress();
        if (controlledParty.Count == 0 && MouseInputUtils.MouseLeftClick())
        {
            GameObject hitEntity = CollisionHandler.GetMouseTarget(zoneLayer);
            if (hitEntity != null && hitEntity.TryGetComponent<EntityHandler>(out _))
            {
                controlledParty.Add(hitEntity);
                if (controlledParty.Count == 1)
                    SetCamParent(controlledParty[0], mainCam);
            }
        }

        foreach (var participant in controlledParty)
        {
            if (participant.TryGetComponent<EntityHandler>(out var player))
                player.HandleMovement();
        }
    }

    
	
	
	private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if(_instance != this) 
			return;

		for(int i = 0; i < _instance.controlledParty.Count; i++)
			SceneUtils.UnPersistObj(_instance.controlledParty[i]);

		Debug.Log("cam after load: " + mainCam);
		if(mainCam == null)
		{
			mainCam = Camera.main;
			if(controlledParty.Count == 1)
				SetCamParent(controlledParty[0], mainCam);
		}
	}


	private void SetCamParent(GameObject target, Camera cam)
	{
		cam.transform.SetParent(target.transform);
		//mainCam.transform.localPosition = Vector3.zero; // or your offset
		cam.transform.localPosition = new Vector3(0f, 0f, cam.transform.localPosition.z);
	}
	


	private void FightSceneProgress()
	{
		if (SceneUtils.GetCurrentSceneName() != SceneNames.FightScene.ToString())
			return;

		if(PlayerEscapeKey())
			SceneController.SwitchScene(SceneNames.MainScene.ToString());

		return;
	}
	

	public static void OnBeforeSceneChange()
	{
		if (_instance == null)
			return;
		Debug.Log("ffff");

		if(_instance.mainCam != null)
			{
			_instance.mainCam.transform.SetParent(null); // to not let inherit dontdestroyonload
			_instance.mainCam = null; // to let go for available next slot
		}

		for(int i = 0; i < _instance.controlledParty.Count; i++)
		{
			DontDestroyOnLoad(_instance.controlledParty[i]);
			//_instance.controlledParty[i].SetActive(false); // dont show in transition
		}
		//Destroy(_instance.mainCam.gameObject); // kill the whole object
	}









}
