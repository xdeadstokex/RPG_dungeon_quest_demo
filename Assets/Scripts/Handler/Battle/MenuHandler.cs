using Assets.Scripts.Battle;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentActorName;
    [SerializeField] private List<SkillHandler> _skillViews;


    public void AddSkillsData(BattleUnit actor, IReadOnlyList<SkillData> skills)
    {
        for (int i = 0;  i < skills.Count; i++)
        {
            SkillHandler skill = _skillViews[i];
            skill.gameObject.SetActive(true);
            skill.AddSkillData(skills[i].SkillName);
        }
    }


    public void SetPrePickSkill(int skillIndex)
    {
        if (!_skillViews[skillIndex].gameObject.activeSelf)
            return;

        _skillViews[skillIndex].PrePickSkill();
    }


    public void UnSetPrePickSkill(int skillIndex)
    {
        _skillViews[skillIndex].UnPrePickSkill();
    }
}
