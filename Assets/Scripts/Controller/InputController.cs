using Assets.Scripts.Battle;
using Assets.Scripts.SystemUtils;
using System.Collections.Generic; // List
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameData;

public class InputController : MonoBehaviour {

//===============================================
//==============  SETUP, DATA   =================
//===============================================
    private static InputController _instance = null;

    [SerializeField] private LayerMask _zoneLayer;
    [SerializeField] private List<CharacterData> _allies = new();

    private EntityHandler _player;

//===============================================
//==============    CORE FUNCS  =================
//===============================================
	private void Start(){
		if(_instance == this){ BindPlayer(); }
	}


    private void Awake(){
        SingletonInit();
    }


    private void Update(){
		FightSceneProgress();
		if(IsFightScene() || _player == null){ return; }
        MoveCharacter();
    }
	

    private void OnEnable(){	SceneManager.sceneLoaded += HandleSceneLoaded; }
    private void OnDisable(){	SceneManager.sceneLoaded -= HandleSceneLoaded; }

//===============================================
//==============  HELPER FUNCS  =================
//===============================================
    private void SingletonInit(){
        if(_instance != null && _instance != this){
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void BindPlayer(){
        _player = null;
        var all = FindObjectsByType<EntityHandler>(FindObjectsSortMode.None);
        foreach (var e in all)
            if (e.IsPlayer){
				Debug.Log("FOUNDDD!!!");
                _player = e;
                break;
            }
    }


    private void MoveCharacter(){
        _player.HandleMovement();
    }


    public static void SavePlayerPosition(){
        if (_instance == null || _instance._player == null){ return; }
        BattleContext.ReturnPosition = _instance._player.transform.position;
        BattleContext.HasReturnPosition = true;
    }


    private void RestorePlayerPosition(){
        if (!BattleContext.HasReturnPosition || _player == null){ return; }

        _player.transform.position = BattleContext.ReturnPosition;
        _player.Stop();
        BattleContext.HasReturnPosition = false;
    }


    public static void OnBeforeSceneChange(){
        if(_instance == null){ return; }
        Debug.Log("ffff");
    }


    //temp for now
    public static List<CharacterData> GetPartyCharacters(){
        return _instance._allies;
    }

    private static bool IsFightScene(){
        return SceneUtils.GetCurrentSceneName() == SceneNames.FightScene.ToString();
    }


    //temp for now
    private void FightSceneProgress(){
        if (SceneUtils.GetCurrentSceneName() != SceneNames.FightScene.ToString()){ return; }
        if (PlayerEscapeKey())
            SceneController.SwitchScene(SceneNames.MainScene.ToString());
        return;
    }


    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode){
        if (_instance != this){ return; }

        BindPlayer();

        if (scene.name == BattleContext.ReturnScene){
            BattleContext.FightCooldownUntil = Time.time + 2f;
            RestorePlayerPosition();
        }
    }


}
