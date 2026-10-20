using UnityEngine;
using static GameData;

public class SceneController : MonoBehaviour{

	private static SceneController _instance = null;
	public float _minTransitionTime;
	public float _currentTransitionTime;
	public bool _isSwitchingScene;
	public bool _isTransiting;
	public bool _isPreloading;
	public string _tagretScene;


	void Awake(){
		if(_instance != null && _instance != this){
			Destroy(gameObject);
			return;
		}
		_instance = this;
		DontDestroyOnLoad(gameObject);

		this._minTransitionTime = 2f; // 2s
		this._currentTransitionTime = 0f;
		this._isSwitchingScene = false;
		this._isTransiting = false;
		this._isPreloading = false;
		this._tagretScene = null;
		return;
	}


    private void Update(){
		if(_instance == null){ return; }
		string transitionScene = SceneNames.TransitionScene.ToString();

		if(SceneUtils.GetCurrentSceneName() == _tagretScene){
			_tagretScene = null;
			_isTransiting = false;
		}

		if(!_isTransiting && SceneUtils.GetCurrentSceneName() == transitionScene){
			_isSwitchingScene = true;
			_isTransiting = true;
		}

		if(_tagretScene != null && !_isTransiting){
			SwitchingScene(transitionScene);
		}

		if(_tagretScene != null && _isTransiting){
			SwitchingScene(_tagretScene);
		}
	
	}


	private void SwitchingScene(string target){
		if(!_isSwitchingScene){ return; }
		
		if(!_isPreloading){
			_isPreloading = true;
			SceneUtils.CancelPreload();
			SceneUtils.PreloadScene(target);
		}

		_currentTransitionTime += TimeUtils.GetFrameTimeSec();
		if(_currentTransitionTime >= _minTransitionTime && SceneUtils.GetPreloadSceneStatus()){
			_isPreloading = false;
			_isSwitchingScene = false;
			_currentTransitionTime = 0f;
			
			InputController.OnBeforeSceneChange();
			SceneUtils.SwitchToPreloadScene();
		}
	}






	public static void SwitchScene(string target){
		if(_instance == null){ return; }
		if(_instance._isSwitchingScene){ return; }
		_instance._tagretScene = target;
		_instance._currentTransitionTime = 0f;
		_instance._isSwitchingScene = true;
		_instance._isTransiting = false;
		return;
	}


			//SceneController _scriptA = GameObject.Find("SceneController").GetComponent<SceneController>();
			//_scriptA.SwitchScene("FightScene");




/*
    [SerializeField] private float minLoadingDuration = 1f;
    [SerializeField] private string fallbackScene = "MainScene";

    private async UniTaskVoid Start(){
        //Debug.Log($"[Transition] id={GetInstanceID()} next='{nextSceneName}' frame={Time.frameCount}");
        string target = string.IsNullOrEmpty(_tagretScene) ? fallbackScene : _tagretScene;
        _tagretScene = null;
        //can add data loading and loading animation after
        var token = this.GetCancellationTokenOnDestroy();
        var op = SceneManager.LoadSceneAsync(target);
        op.allowSceneActivation = false;

        try{
            float elapsed = 0f;
            while (op.progress < 0.9f || elapsed < minLoadingDuration)
            {
                elapsed += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
        finally{
            op.allowSceneActivation = true;
        }
    }
*/



}
