using UnityEngine;
using System.Collections.Generic; // List

using static GameData;

public class InputController : MonoBehaviour{

	private static InputController _instance = null;
	[SerializeField] private LayerMask zoneLayer;
	[SerializeField] List<GameObject> controlledParty = new List<GameObject>();
	public Camera mainCam = null;


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


	// this group is for setting when a scene is loaded
	void OnEnable(){ UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded; }
	void OnDisable(){ UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded; }
	
	
	void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode){
    if(_instance != this) return;


	for(int i = 0; i < _instance.controlledParty.Count; i++){
		SceneUtils.BringObjToScene(_instance.controlledParty[i]);
	}


		Debug.Log("cam after load: " + mainCam);
		if(mainCam == null){
			mainCam = Camera.main;
			if(controlledParty.Count == 1){
				SetCamParent(controlledParty[0], mainCam);
			}
		}
	}


    private void Update(){
		FightSceneProgress();
		if(controlledParty.Count == 0){

			if (MouseInputUtils.MouseLeftClick()){
				GameObject hitEntity = CollisionHandler.GetMouseTarget(zoneLayer);
				if(hitEntity != null){
					EntityHandler player = hitEntity.GetComponent<EntityHandler>();
					if(player != null){
						controlledParty.Add(hitEntity); 
						if(controlledParty.Count == 1){
							SetCamParent(controlledParty[0], mainCam);
						}
					}
				}
			}

		}
		

		for (int i = 0; i < controlledParty.Count; i++){
			EntityHandler player = controlledParty[i].GetComponent<EntityHandler>();
			if(player != null){
				player.HandleMovement();
			}
		}
	}




	private void SetCamParent(GameObject target, Camera cam){
		cam.transform.SetParent(target.transform);
		//mainCam.transform.localPosition = Vector3.zero; // or your offset
		cam.transform.localPosition = new Vector3(0f, 0f, cam.transform.localPosition.z);
	}
	


	private void FightSceneProgress(){
		if(SceneUtils.GetCurrentSceneName() != SceneNames.FightScene.ToString()){ return; }
		if(PlayerEscapeKey()){
			SceneController.SwitchScene(SceneNames.MainScene.ToString());
		}
		return;
	}
	

	public static void OnBeforeSceneChange(){
		if(_instance == null) return;
		Debug.Log("ffff");


    if(_instance.mainCam != null){
		_instance.mainCam.transform.SetParent(null); // to not let inherit dontdestroyonload
		_instance.mainCam = null; // to let go for available next slot
    }


		for(int i = 0; i < _instance.controlledParty.Count; i++){
			DontDestroyOnLoad(_instance.controlledParty[i]);
			//_instance.controlledParty[i].SetActive(false); // dont show in transition
		}
		

		//Destroy(_instance.mainCam.gameObject); // kill the whole object
	}









}
