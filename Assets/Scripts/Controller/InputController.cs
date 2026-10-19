using UnityEngine;
using System.Collections.Generic; // List

using static GameData;

public class InputController : MonoBehaviour{

	private static InputController _instance = null;
	[SerializeField] public LayerMask zoneLayer;
	[SerializeField] List<GameObject> controlledParty = new List<GameObject>();



	void Awake(){
		if(_instance != null && _instance != this){
			Destroy(gameObject);
			return;
		}
		_instance = this;
		DontDestroyOnLoad(gameObject);

		return;
	}

    private void Update(){
		FightSceneProgress();
		if(controlledParty.Count == 0){

			if (MouseInputUtils.MouseLeftClick()){
				GameObject hitEntity = ChangeSceneObjs.GetMouseTarget(zoneLayer);
				if(hitEntity != null){
					EntityHandler player = hitEntity.GetComponent<EntityHandler>();
					if(player != null){ controlledParty.Add(hitEntity); }
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






	private void FightSceneProgress(){
		if(SceneUtils.GetCurrentSceneName() != SceneNames.FightScene.ToString()){ return; }
		if(PlayerEscapeKey()){
			SceneController.SwitchScene(SceneNames.MainScene.ToString());
		}
		return;
	}

}
