using TMPro;
using UnityEngine;

public class TimingPanelHandler : MonoBehaviour
{
    [SerializeField] private GameObject _bar;
    [SerializeField] private RectTransform _good;
    [SerializeField] private RectTransform _miss;
    [SerializeField] private RectTransform _perfect;
    [SerializeField] private RectTransform _cursor;
    [SerializeField] private TextMeshProUGUI _text;

    private float _baseWidth;


    public void Setup(float good, float miss, float perfect)
    {
        gameObject.SetActive(true);
        if (_bar != null) 
            _bar.SetActive(true);

        _baseWidth = ((RectTransform)transform).rect.width;

        float total = 2f * Mathf.Max(good, miss, perfect);
        SetBarWidth(_good, _baseWidth * 2f * good / total);
        SetBarWidth(_miss, _baseWidth * 2f * miss / total);
        SetBarWidth(_perfect, _baseWidth * 2f * perfect / total);

        SetCursorOffset(0);
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
        if (_bar != null)
            _bar.SetActive(false);
        else
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


    private void SetBarWidth(RectTransform bar, float width)
    {
        if (bar == null)
            return;

        var s = bar.sizeDelta;
        s.x = Mathf.Clamp(width, 0f, _baseWidth);
        bar.sizeDelta = s;
    }
}
