using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnitHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _actorName;
    [SerializeField] private Image _actorIcon;
    [SerializeField] private Image _hpFill;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private Image _mpFill;
    [SerializeField] private TextMeshProUGUI _mpText;


    public void SetupActorStat(string name, Sprite icon)
    {
        _actorName.text = name;
        _actorIcon.sprite = icon;
        gameObject.SetActive(true);
    }


    public void SetHp(int hp, int maxHp)
    {
        _hpText.text = hp.ToString() + "/" + maxHp.ToString();
        float ratio = Mathf.Clamp01((float)hp / maxHp);
        _hpFill.rectTransform.anchorMax = new Vector2(ratio, 1f);
    }


    public void SetMp(int mp, int maxHp)
    {
        _mpText.text = mp.ToString() + "/" + maxHp.ToString();
        float ratio = Mathf.Clamp01((float)mp / maxHp);
        _mpFill.rectTransform.anchorMax = new Vector2(ratio, 1f);
    }
}
