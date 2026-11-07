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
        if (_currentActorName != null)
            _currentActorName.text = actor.DisplayName();

        for (int i = 0;  i < skills.Count; i++)
        {
            bool has = i < _skillViews.Count;
            _skillViews[i].gameObject.SetActive(has);
            if (!has)
                continue;

            _skillViews[i].AddSkillData(skills[i].SkillName);
            _skillViews[i].UnPrePickSkill();
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
