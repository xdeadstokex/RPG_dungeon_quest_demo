using TMPro;
using UnityEngine;

public class TimingPanelHandler : MonoBehaviour
{
    [SerializeField] private RectTransform _good;
    [SerializeField] private RectTransform _miss;
    [SerializeField] private RectTransform _perfect;
    [SerializeField] private RectTransform _cursor;
    [SerializeField] private TextMeshProUGUI _text;

    private float _baseWidth;
    private float _goodWindow;


    public void Setup(float good, float miss, float perfect)
    {
        _baseWidth = ((RectTransform)transform).rect.width;
        _goodWindow = good;

        SetBarWidth(_miss, _baseWidth * miss / good);
        SetBarWidth(_perfect, _baseWidth * perfect / good);

        SetCursorOffset(0);
        gameObject.SetActive(true);
    }


    public void SetCursorOffset(float p)
    {
        float x = p * _baseWidth;
        var pos = _cursor.anchoredPosition;
        pos.x = x;
        _cursor.anchoredPosition = pos;
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }


    public void SetText(string text, Color color)
    {
        _text.text = text;
        _text.color = color;
        _text.gameObject.SetActive(true);
    }


    public void HideText()
    {
        _text.gameObject.SetActive(false);
    }


    private static void SetBarWidth(RectTransform bar, float width)
    {
        var s = bar.sizeDelta;
        s.x = width;
        bar.sizeDelta = s;
    }
}
