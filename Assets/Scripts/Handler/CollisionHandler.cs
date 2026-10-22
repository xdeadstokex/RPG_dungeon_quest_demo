using UnityEngine;
using static GameData;

public class CollisionHandler : MonoBehaviour{
	//[SerializeField] public string _targetScene = "FightScene";

	// have custom detection radius (maybe change to not layer type)

	public enum TriggerType{
		Click,
		Enter
	}



	[SerializeField] private SceneNames _targetScene = SceneNames.None;
	[SerializeField] private ZoneType _zoneType = ZoneType.Portal;
	[SerializeField] private TriggerType _triggerType = TriggerType.Enter;

	static public Collider2D _lastHit;


	public static GameObject GetMouseTarget(LayerMask layer){
		Collider2D hit = Physics2D.OverlapPoint(MouseInputUtils.MouseWorldPos2D(), layer);
		if(hit == null){ return null; }
		Debug.Log("ol");
		//if(hit.GetComponent<CollisionHandler>() == null){ return null; } // not interactable
		return hit.gameObject;
	}


	public static bool IsInZoneType(Vector2 playerPos, PlayerData data, LayerMask layer, ZoneType type){
		//Collider2D hitObject = Physics2D.OverlapBox(playerPos, data.boxCollider.bounds.size, 0f, layer);
		
		Collider2D[] hits = Physics2D.OverlapBoxAll(playerPos, data.boxCollider.bounds.size, 0f, layer);
		Collider2D hitObject = System.Array.Find(hits, h => h.gameObject != data.obj);

		if(hitObject == null){ return false; }
		CollisionHandler dataScript = hitObject.GetComponent<CollisionHandler>();
		if(dataScript == null){ return false; }
		return type == dataScript._zoneType;
	}

	public static string GetZoneName(PlayerData data, LayerMask layer){
		Collider2D hitObject = Physics2D.OverlapCircle(data.transform.position, 0.5f, layer);
		if(hitObject == null){ return null; }
		return hitObject.gameObject.name;
	}

	public static void TriggerZoneFunction(PlayerData data, LayerMask layer){
		//Collider2D hitObject = Physics2D.OverlapCircle(data.transform.position, 0.5f, layer);

		Collider2D[] hits = Physics2D.OverlapBoxAll(data.transform.position, data.boxCollider.bounds.size, 0f, layer);
		Collider2D hitObject = System.Array.Find(hits, h => h.gameObject != data.obj);


		if(hitObject == null){ goto end; }
		bool inside = hitObject != null;
		
		CollisionHandler dataScript = hitObject.GetComponent<CollisionHandler>();
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


	private static void HandleDialog(PlayerData data, CollisionHandler dataScript, Collider2D obj){
		if(obj == null){ return; }
		if(dataScript._zoneType != ZoneType.Dialog){ return; }
		if(!PlayerInteractKey()){ return; }
		
		DialogHandler dialog = obj.GetComponent<DialogHandler>();
		if(dialog != null){
			dialog.Show(data);
		}
	}



	public static void CheckChangeScene(CollisionHandler dataScript){
		string sceneName = dataScript._targetScene.ToString();
		if(sceneName == "None"){ return; }
		SceneController.SwitchScene(sceneName);
		return;
	}


}