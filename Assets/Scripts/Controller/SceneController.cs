using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameData;


public class SceneController : MonoBehaviour{

	private static SceneController _instance = null;
    public float _minTransitionTime;
    public float _currentTransitionTime;
    public bool _isSwitchingScene;
    public bool _isTransiting;
    public bool _isPreloading;
    public string _tagretScene;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
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


    private void Update()
    {
        if (_instance == null) { return; }
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




    //[SerializeField] private CanvasGroup _fade;
    //[SerializeField] private float _fadeTime = 0.3f;
    //[SerializeField] private float _minLoading = 2f;
    //[SerializeField] private string _transitionScene = "TransitionScene";

    //private bool _isSwitching;
    //public static bool IsSwitching => _instance != null && _instance._isSwitching;


    //   private void Awake()
    //   {
    //       if (_instance != null && _instance != this)
    //       {
    //           Destroy(gameObject);
    //           return;
    //       }
    //       _instance = this;
    //       DontDestroyOnLoad(gameObject);
    //       _fade.alpha = 0f;
    //       _fade.blocksRaycasts = false;
    //   }


    //   public static void SwitchScene(string target)
    //   {
    //       if (_instance == null || _instance._isSwitching)
    //           return;
    //       _instance.StartCoroutine(_instance.SwitchRoutine(target));
    //   }

    //   private IEnumerator SwitchRoutine(string target)
    //   {
    //       _isSwitching = true;
    //       _fade.blocksRaycasts = true;

    //       yield return Fade(0f, 1f);
    //       InputController.OnBeforeSceneChange();

    //       AsyncOperation toTrasition = SceneManager.LoadSceneAsync(_transitionScene);
    //       yield return toTrasition;
    //       yield return null;

    //       yield return Fade(1f, 0f);

    //       TransitionView view = TransitionView.Instance;
    //       Application.backgroundLoadingPriority = ThreadPriority.High;

    //       AsyncOperation op = SceneManager.LoadSceneAsync(target);
    //       op.allowSceneActivation = false;

    //       float elapsed = 0f;
    //       while (op.progress < 0.9f || elapsed < _minLoading)
    //       {
    //           elapsed += Time.unscaledDeltaTime;
    //           float real = Mathf.Clamp01(op.progress / 0.9f);
    //           float time = Mathf.Clamp01(elapsed / _minLoading);
    //           if (view != null)
    //               view.SetProgress(Mathf.Min(real, time));
    //           yield return null;
    //       }

    //       if (view != null)
    //           view.SetProgress(1f);

    //       yield return Fade(0f, 1f);
    //       op.allowSceneActivation = true;
    //       yield return op;
    //       yield return null;

    //       yield return Resources.UnloadUnusedAssets();
    //       Application.backgroundLoadingPriority = ThreadPriority.Low;

    //       yield return Fade(1f, 0f);
    //       _fade.blocksRaycasts = false;
    //       _isSwitching = false;

    //   }

    //   private IEnumerator Fade(float from, float to)
    //   {
    //       float time = 0f;
    //       while (time < _fadeTime)
    //       {
    //           time += Time.unscaledDeltaTime;
    //           _fade.alpha = Mathf.Lerp(from, to, time / _fadeTime);
    //           yield return null;
    //       }
    //       _fade.alpha = to;
    //   }


}
