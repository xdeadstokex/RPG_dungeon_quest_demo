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

    //private int _maxHp;
    //private int _maxMp;


    public void SetupActorStat(string name, Sprite icon)
    {
        //_maxHp = maxHp;
        //_maxMp = maxMp;
        _actorName.text = name;
        _actorIcon.sprite = icon;
        //_hpText.text = maxHp.ToString() + "/" + maxHp.ToString();
        //_mpText.text = maxMp.ToString() + "/" + maxMp.ToString();
    }


    public void SetHp(int hp, int maxHp)
    {
        _hpText.text = hp.ToString() + "/" + maxHp.ToString();
        float ratio = Mathf.Clamp01(hp / maxHp);
        _hpFill.rectTransform.anchorMax = new Vector2(ratio, 1f);
    }


    public void SetMp(int mp, int maxHp)
    {
        _mpText.text = mp.ToString() + "/" + maxHp.ToString();
        float ratio = Mathf.Clamp01(mp / maxHp);
        _mpFill.rectTransform.anchorMax = new Vector2(ratio, 1f);
    }
}
