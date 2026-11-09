using Assets.Scripts.Battle;
using Assets.Scripts.SystemUtils;
using System.Collections.Generic; // List
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameData;

public class InputController : MonoBehaviour{

	private static InputController _instance = null;

	[SerializeField] private LayerMask _zoneLayer;
    [SerializeField] private EntityHandler _player;
    [SerializeField] private List<CharacterData> _allies = new();
	//[SerializeField] private Camera mainCam = null;


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
		Init();
		//MatchCamera();
	}


    private void Update()
    {
        FightSceneProgress();
        if (IsFightScene())
            return;
        MoveCharacter();
		//ChooseAPartyMember();
		//MoveAllPartyMembers();
    }


    private void MoveCharacter()
    {
        _player.HandleMovement();
    }


    //static funcs
    public static void OnBeforeSceneChange()
    {
        if (_instance == null)
            return;
        Debug.Log("ffff");

        //if (_instance.mainCam != null)
        //{
        //    _instance.mainCam.transform.SetParent(null);
        //    _instance.mainCam = null;
        //}


        //temp for now
        //for (int i = 0; i < _instance.controlledParty.Count; i++)
        //    DontDestroyOnLoad(_instance.controlledParty[i]);
    }


    //temp for now
    public static List<CharacterData> GetPartyCharacters()
    {

        return _instance._allies;
    }


    //helper funcs
    private static bool IsFightScene()
    {
        return SceneUtils.GetCurrentSceneName() == SceneNames.FightScene.ToString();
    }


    private void Init()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }


    //private void MatchCamera()
    //{
    //    if (_instance.mainCam != null)
    //    {
    //        _instance.mainCam = Camera.main;
    //        if (controlledParty.Count == 1)
    //            CameraUtils.SetCameraParent(controlledParty[0], mainCam);
    //    }
    //}


    //temp for now
    private void FightSceneProgress()
    {
        if (SceneUtils.GetCurrentSceneName() != SceneNames.FightScene.ToString())
            return;
        if (PlayerEscapeKey())
            SceneController.SwitchScene(SceneNames.MainScene.ToString());
        return;
    }


    //temp for now
    //private void ChooseAPartyMember()
    //{
    //    if (controlledParty.Count == 0 && MouseInputUtils.MouseLeftClick())
    //    {
    //        GameObject hitEntity = CollisionHandler.GetMouseTarget(zoneLayer);
    //        if (hitEntity != null && hitEntity.TryGetComponent<EntityHandler>(out _))
    //        {
    //            controlledParty.Add(hitEntity);
    //            if (controlledParty.Count == 1)
    //                CameraUtils.SetCameraParent(controlledParty[0], mainCam);
    //        }
    //    }
    //}


    //private void MoveAllPartyMembers()
    //{
    //    foreach (var participant in controlledParty)
    //    {
    //        if (participant.TryGetComponent<EntityHandler>(out var player))
    //            player.HandleMovement();
    //    }
    //}


    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_instance != this)
            return;

        bool fight = scene.name == SceneNames.FightScene.ToString();

        //for (int i = 0; i < _instance.controlledParty.Count; i++)
        //{
        //    SceneUtils.UnPersistObj(_instance.controlledParty[i]);
        //    _instance.controlledParty[i].SetActive(!fight);   
        //}

        //if (fight)
        //{
        //    mainCam = Camera.main;
        //    return;
        //}

        if (scene.name == BattleContext.ReturnScene)
            BattleContext.FightCooldownUntil = Time.time + 2f;

        //Debug.Log("cam after load: " + mainCam);
        //if (mainCam == null)
        //{
        //    mainCam = Camera.main;
        //    if (controlledParty.Count == 1)
        //        CameraUtils.SetCameraParent(controlledParty[0], mainCam);
        //}
    }
}
