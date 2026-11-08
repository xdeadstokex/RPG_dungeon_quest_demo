using System.Collections;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHandler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _mainCharRenderer;
    [SerializeField] private SpriteRenderer _highlightRenderer;
    [SerializeField] private SpriteRenderer _targetedRenderer;
    [SerializeField] private TextMeshProUGUI _popupText;
    [SerializeField] private float _popupDuration = 1f;
    [SerializeField] private float _popupRiseDelta = 20f;
    [SerializeField] private Image _hpFill;
    [SerializeField] private Image _mpFill;


    private Coroutine _popupCoroutine;


    private void Awake()
    {
        if (_popupText != null)
            _popupText.gameObject.SetActive(false);
        if (_targetedRenderer != null)
            _targetedRenderer.gameObject.SetActive(false);
        if (_highlightRenderer != null)
            _highlightRenderer.gameObject.SetActive(false);
    }

    public void SetupVisuals(Sprite charSprite)
    {
        if (_mainCharRenderer != null)
            _mainCharRenderer.sprite = charSprite;
    }


    public void SetHighight(bool on)
    {
        if (_highlightRenderer != null)
            _highlightRenderer.gameObject.SetActive(on);
    }


    public void ShowMarker()
    {
        _targetedRenderer.gameObject.SetActive(true);
    }


    public void HideMarker()
    {
        _targetedRenderer.gameObject.SetActive(false);
    }


    public void ShowPopup(string text, Color c)
    {
        if (_popupText == null)
            return;

        if (_popupCoroutine != null)
            StopCoroutine(_popupCoroutine);

        _popupCoroutine = StartCoroutine(PopupRoutine(text, c));
    }


    private IEnumerator PopupRoutine(string text, Color c)
    {
        _popupText.text = text;
        _popupText.color = c;
        _popupText.gameObject.SetActive(true);

        float t = 0f;
        while (t < _popupDuration)
        {
            t += TimeUtils.GetFrameTimeSec();
            float p = Mathf.Clamp01(t / _popupDuration);
            _popupText.rectTransform.anchoredPosition = new Vector2(0f, _popupRiseDelta * p);
            _popupText.color = new Color(c.r, c.g, c.b, 1f - p);
            yield return null;
        }

        _popupText.gameObject.SetActive(false);
        _popupCoroutine = null;
    }


    public void SetHp(int hp, int maxHp)
    {
        float ratio = Mathf.Clamp01((float)hp / maxHp);
        _hpFill.rectTransform.anchorMax = new Vector2(ratio, 1f);
        
    }


    public void SetMp(int mp, int maxHp)
    {
        float ratio = Mathf.Clamp01((float)mp / maxHp);
        _mpFill.rectTransform.anchorMax = new Vector2(ratio, 1f);
    }
}
