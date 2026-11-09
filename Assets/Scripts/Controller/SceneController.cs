using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameData;


public class SceneController : MonoBehaviour
{
	private static SceneController _instance = null;

    [SerializeField] private float _minTransitionTime;
    [SerializeField] private float _currentTransitionTime;

    private bool _isSwitchingScene;
    private bool _isTransiting;
    private bool _isPreloading;
    private string _tagretScene;

    public static bool Exists => _instance != null;


    private void Awake()
    {
        SingletonInit();

        _currentTransitionTime = 0f;
        _isSwitchingScene = false;
        _isTransiting = false;
        _isPreloading = false;
        _tagretScene = null;
    }


    private void Update()
    {
        if (_instance == null)
            return;

        string transitionScene = SceneNames.TransitionScene.ToString();

        //case in the target scene
        if (SceneUtils.GetCurrentSceneName() == _tagretScene)
        {
            _tagretScene = null;
            _isTransiting = false;
        }

        //only just turn into the transitionscene
        if (!_isTransiting && SceneUtils.GetCurrentSceneName() == transitionScene)
        {
            _isSwitchingScene = true;
            _isTransiting = true;
        }

        //not in the transition scene
        if (_tagretScene != null && !_isTransiting)
        {
            SwitchingScene(transitionScene);
        }

        //turn into targetscene
        if (_tagretScene != null && _isTransiting)
        {
            SwitchingScene(_tagretScene);
        }

    }


    private void SingletonInit()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void SwitchingScene(string target)
    {
        if (!_isSwitchingScene)
            return;

        if (!_isPreloading)
        {
            _isPreloading = true;
            SceneUtils.CancelPreload();
            SceneUtils.PreloadScene(target);
        }

        _currentTransitionTime += TimeUtils.GetFrameTimeSec();

        if ((_currentTransitionTime >= _minTransitionTime || !_isTransiting) && SceneUtils.GetPreloadSceneStatus())
        //if (SceneUtils.GetPreloadSceneStatus())
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
