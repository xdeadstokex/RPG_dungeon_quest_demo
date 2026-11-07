using TMPro;
using UnityEngine;

public class ResultPanelHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resultText;

    public void SetResultText(string text)
    {
        _resultText.text = text;
        gameObject.SetActive(true);
    }

    public void HideResultText()
    {
        gameObject.SetActive(false);
    }
}
