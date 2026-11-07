using System.Collections;
using TMPro;
using UnityEngine;

public class CharacterHandler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _mainCharRenderer;
    [SerializeField] private SpriteRenderer _highlightRenderer;
    [SerializeField] private SpriteRenderer _targetedRenderer;
    [SerializeField] private TextMeshProUGUI _popupText;
    [SerializeField] private float _popupDuration = 1f;
    [SerializeField] private float _popupRiseDelta = 20f;

    private Coroutine _popupCoroutine;


    public void SetupVisuals(Sprite charSprite)
    {
        if (_mainCharRenderer != null)
            _mainCharRenderer.sprite = charSprite;
    }


    public void PopupEnable(string text)
    {
        _popupText.text = text;
        _popupText.gameObject.SetActive(true);
    }


    public IEnumerator PopupRoutine(Color c)
    {
        float t = 0f;
        while (t < _popupDuration)
        {
            t += TimeUtils.GetFrameTimeSec();
            float p = Mathf.Clamp01(t / _popupDuration);
            _popupText.rectTransform.anchoredPosition = new Vector2(0f, _popupRiseDelta * p);
            _popupText.color = new Color(c.r, c.g, c.b, 1f - p);
            yield return null;
        }
    }


    public void PopupDisable()
    {
        _popupText.gameObject.SetActive(false);
        _popupCoroutine = null;
    }


    public void StopCoroutineIfExist()
    {
        if (_popupCoroutine != null)
            StopCoroutine(_popupCoroutine);
    }


    public void AssignCoroutine(Coroutine popupCoroutine)
    {
        _popupCoroutine = popupCoroutine;
    }


    public void ShowMarker()
    {
        _popupText.gameObject.SetActive(true);
    }


    public void HideMarker()
    {
        _popupText.gameObject.SetActive(false);
    }
}
