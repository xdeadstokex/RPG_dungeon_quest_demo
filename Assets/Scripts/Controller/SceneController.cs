using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameData;


public class SceneController : MonoBehaviour
{
	private static SceneController _instance = null;
    public float _minTransitionTime;
    public float _currentTransitionTime;
    public bool _isSwitchingScene;
    public bool _isTransiting;
    public bool _isPreloading;
    public string _tagretScene;

    public static bool Exists => _instance != null;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        this._minTransitionTime = 2f;
        this._currentTransitionTime = 0f;
        this._isSwitchingScene = false;
        this._isTransiting = false;
        this._isPreloading = false;
        this._tagretScene = null;
        return;
    }


    private void Update()
    {
        if (_instance == null)
            return;

        string transitionScene = SceneNames.TransitionScene.ToString();

        if (SceneUtils.GetCurrentSceneName() == _tagretScene)
        {
            _tagretScene = null;
            _isTransiting = false;
        }

        if (!_isTransiting && SceneUtils.GetCurrentSceneName() == transitionScene)
        {
            _isSwitchingScene = true;
            _isTransiting = true;
        }

        if (_tagretScene != null && !_isTransiting)
        {
            SwitchingScene(transitionScene);
        }

        if (_tagretScene != null && _isTransiting)
        {
            SwitchingScene(_tagretScene);
        }

    }


    private void SwitchingScene(string target)
    {
        if (!_isSwitchingScene) { return; }

        if (!_isPreloading)
        {
            _isPreloading = true;
            SceneUtils.CancelPreload();
            SceneUtils.PreloadScene(target);
        }

        _currentTransitionTime += TimeUtils.GetFrameTimeSec();
        if (_currentTransitionTime >= _minTransitionTime && SceneUtils.GetPreloadSceneStatus())
        {
            _isPreloading = false;
            _isSwitchingScene = false;
            _currentTransitionTime = 0f;

            InputController.OnBeforeSceneChange();
            SceneUtils.SwitchToPreloadScene();
        }
    }


    public static void SwitchScene(string target)
    {
        if (_instance == null) { return; }
        if (_instance._isSwitchingScene) { return; }
        _instance._tagretScene = target;
        _instance._currentTransitionTime = 0f;
        _instance._isSwitchingScene = true;
        _instance._isTransiting = false;
        return;
    }
}
