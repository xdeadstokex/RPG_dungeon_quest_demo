using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillHandler : MonoBehaviour
{
    [SerializeField] private GameObject _hollow;
    [SerializeField] private Image _background;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _name;


    /// <summary>
    /// just change the name now(need change icon, maybe bg, hollow)
    /// </summary>
    public void AddSkillData(string name)
    {
        _name.text = name;
    }

    
    public void PrePickSkill()
    {
        _hollow.SetActive(true);
    }


    public void UnPrePickSkill()
    {
        _hollow.SetActive(false);
    }
}
