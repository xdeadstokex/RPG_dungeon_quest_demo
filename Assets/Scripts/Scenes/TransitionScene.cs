using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;


public class TransitionScene : MonoBehaviour
{
    [SerializeField] private float minLoadingDuration = 1f;
    [SerializeField] private string fallbackScene = "MainScene";
    public static string nextSceneName;


    private async UniTaskVoid Start()
    {
        //Debug.Log($"[Transition] id={GetInstanceID()} next='{nextSceneName}' frame={Time.frameCount}");
        string target = string.IsNullOrEmpty(nextSceneName) ? fallbackScene : nextSceneName;
        nextSceneName = null;
        //can add data loading and loading animation after
        var token = this.GetCancellationTokenOnDestroy();
        var op = SceneManager.LoadSceneAsync(target);
        op.allowSceneActivation = false;

        try
        {
            float elapsed = 0f;
            while (op.progress < 0.9f || elapsed < minLoadingDuration)
            {
                elapsed += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
        finally
        {
            op.allowSceneActivation = true;
        }
    }
}
