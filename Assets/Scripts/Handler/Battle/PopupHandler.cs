using UnityEngine;
using static GameData;

public class PopupHandler : MonoBehaviour{

	public enum PopupType{
		FollowPlayer,
		FollowObj,
		ZonePopup
	}


	[SerializeField] public GameObject popup = null;
	[SerializeField] public bool initSignal = false;
	[SerializeField] public bool invertedSignal = false;
	[SerializeField] public PopupType type = PopupType.ZonePopup;
	[SerializeField] public float offsetX = 0;
	[SerializeField] public float offsetY = 0;
	private Transform _target = null;
	private float PlayerPopupoffsetX = 0;
	private float PlayerPopupoffsetY = 0;
	
	private void Awake(){
		if(popup == null){ return; }
		popup.SetActive(initSignal);
	}
	
	void Update(){
		if(_target == null){ return; }
		if(type == PopupType.FollowObj){ 
			Vector3 pos = popup.transform.position;
			pos.x = _target.position.x + offsetX;
			pos.y = _target.position.y + offsetY;
			popup.transform.position = pos;
			return;
		}

		if(type == PopupType.FollowPlayer){ 
			Vector3 pos = popup.transform.position;
			pos.x = _target.position.x + PlayerPopupoffsetX;
			pos.y = _target.position.y + PlayerPopupoffsetY;
			popup.transform.position = pos;
			return;
		}

	}
	
	public void Show(bool signal){
		if(popup == null){ return; }
		popup.SetActive((invertedSignal) ? !signal : signal);
	}
	
	public void FollowObj(Transform target, bool signal){
		if(popup == null){ return; }
		if(type != PopupType.FollowObj){ return; }
		_target = signal ? target : null;
	}

	public void FollowPlayer(PlayerData data, bool signal){
		if(popup == null){ return; }
		if(type != PopupType.FollowPlayer){ return; }
		PlayerPopupoffsetX = data.popupOffsetX;
		PlayerPopupoffsetY = data.popupOffsetY;
		_target = signal ? data.transform : null;
	}

}