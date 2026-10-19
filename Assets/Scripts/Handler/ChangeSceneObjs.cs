using UnityEngine;
using static GameData;

public class ChangeSceneObjs : MonoBehaviour{
	//[SerializeField] public string _targetScene = "FightScene";

	// have custom detection radius (maybe change to not layer type)

	public enum TriggerType{
		Click,
		Enter
	}

	public enum ZoneType{
		Portal,
		Teleport,
		Solid,
		Dialog,
		Fight
	}

	[SerializeField] public SceneNames _targetScene = SceneNames.None;
	[SerializeField] public ZoneType _zoneType = ZoneType.Portal;
	[SerializeField] public TriggerType _triggerType = TriggerType.Enter;

	static public Collider2D _lastHit;


		public static GameObject GetMouseTarget(LayerMask layer){
			Collider2D hit = Physics2D.OverlapPoint(MouseInputUtils.MouseWorldPos2D(), layer);
			if(hit == null){ return null; }
			Debug.Log("ol");
			//if(hit.GetComponent<ChangeSceneObjs>() == null){ return null; } // not interactable
			return hit.gameObject;
		}

		public static bool IsInZone(Vector2 playerPos, LayerMask layer){
			Collider2D hitObject = Physics2D.OverlapCircle(playerPos, 0.5f, layer);
			return (hitObject != null);
		}

	public static string GetZoneName(PlayerData data, LayerMask layer){
		Collider2D hitObject = Physics2D.OverlapCircle(data.transform.position, 0.5f, layer);
		if(hitObject == null){ return null; }
		return hitObject.gameObject.name;
	}

	public static void TriggerZoneFunction(PlayerData data, LayerMask layer){
		Collider2D hitObject = Physics2D.OverlapCircle(data.transform.position, 0.5f, layer);
		if(hitObject == null){ goto end; }
		bool inside = hitObject != null;
		
		ChangeSceneObjs dataScript = hitObject.GetComponent<ChangeSceneObjs>();
		if(dataScript == null){ goto end; }
		
		HandlePopup(data, hitObject, true);
		HandleDialog(data, dataScript, hitObject);

		switch(dataScript._zoneType){

			case ZoneType.Portal:
			if(inside){
				if(dataScript._triggerType == TriggerType.Click && !PlayerInteractKey()){
					break;
				}
				CheckChangeScene(dataScript);
			}
			break;



			case ZoneType.Fight:

			break;


		}

		end:
		if(_lastHit != hitObject){
			HandlePopup(data, _lastHit, false);
			_lastHit = hitObject;
		}
		return;
	}

	
	private static void HandlePopup(PlayerData data, Collider2D obj, bool signal){
		if(obj == null){ return; }
		PopupHandler popup = obj.GetComponent<PopupHandler>();
		if(popup != null){
			popup.FollowObj(data.transform, signal);
			popup.FollowPlayer(data, signal);
			popup.Show(signal);
		}
	}


	private static void HandleDialog(PlayerData data, ChangeSceneObjs dataScript, Collider2D obj){
		if(obj == null){ return; }
		if(dataScript._zoneType != ZoneType.Dialog){ return; }
		if(!PlayerInteractKey()){ return; }
		
		DialogHandler dialog = obj.GetComponent<DialogHandler>();
		if(dialog != null){
			dialog.Show(data);
		}
	}














	public static void CheckChangeScene(ChangeSceneObjs dataScript){
		string sceneName = dataScript._targetScene.ToString();
		if(sceneName == "None"){ return; }
		SceneController.SwitchScene(sceneName);
		return;
	}


}