using UnityEngine;
using UnityEngine.UI;

public class TransitionView : MonoBehaviour
{
    public static TransitionView Instance { get; private set; }
    [SerializeField] private Text _percent;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void SetProgress(float value)
    {
        _percent.text = Mathf.RoundToInt(value * 100f) + "%";
    }
}
