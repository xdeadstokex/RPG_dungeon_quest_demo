using Assets.Scripts.Battle;
using Assets.Scripts.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using static GameData;


//public class UnitView
//{
//    public GameObject gameObject;
//    public Coroutine popupCoroutine;
//}


public class BattleHandler : MonoBehaviour, IBattleMenu
{
    [SerializeField] private List<Transform> _alliesPosition;
    [SerializeField] private List<Transform> _enemiesPosition;
    [SerializeField] private GameObject _unitPrefab;
    [SerializeField] private Color _critDameColor;
    [SerializeField] private Color _normalDameColor;
    [SerializeField] private Color _missColor;
    [SerializeField] private Color _parryColor;
    [SerializeField] private Color _healColor;
    [SerializeField] private TimingPanelHandler _timingPanel;
    [SerializeField] private ResultPanelHandler _resultPanel;
    [SerializeField] private MenuHandler _menu;



    private BattleController _battle;
    private BattleUnit _currentActor;
    private Coroutine _ratingRoutine;
    private Dictionary<BattleUnit, CharacterHandler> _views = new();


    private void Start()
    {
        var battleController = GetComponent<BattleController>();
        Bind(battleController);
    }


    public void Bind(BattleController battle)
    {
        _battle = battle;
        _battle.OnBattleStart += HandleBattleStart;
        _battle.OnTurnStart += HandleTurnStart;
        _battle.OnSkillUsed += HandleSkillUsed;
        _battle.OnDamaged += HandleDamaged;
        _battle.OnHealed += HandleHealed;
        _battle.OnCommandBegin += HandleCommandBegin;
        _battle.OnCommandEnd += HandleCommandEnd;
        _battle.OnBattleEnd += HandleBattleEnd;
    }


    private void OnDestroy()
    {
        if (_battle == null)
            return;

        _battle.OnBattleStart -= HandleBattleStart;
        _battle.OnTurnStart -= HandleTurnStart;
        _battle.OnSkillUsed -= HandleSkillUsed;
        _battle.OnDamaged -= HandleDamaged;
        _battle.OnHealed -= HandleHealed;
        _battle.OnCommandBegin -= HandleCommandBegin;
        _battle.OnCommandEnd -= HandleCommandEnd;
        _battle.OnBattleEnd -= HandleBattleEnd;
    }


    private void HandleBattleStart(IReadOnlyList<BattleUnit> units)
    {
        var players = new List<BattleUnit>();
        var enemies = new List<BattleUnit>();

        foreach (var unit in units)
            (unit.Team == Team.Player ? players : enemies).Add(unit);

        SpawnTeam(players, true);
        SpawnTeam(enemies, false);
    }


    private void SpawnTeam(List<BattleUnit> team, bool isPlayer)
    {
        int n = team.Count;
        List<Transform> listPos = isPlayer ? _alliesPosition : _enemiesPosition;
        for (int i = 0; i < n; i++)
        {
            GameObject newCharacter = Instantiate(_unitPrefab, listPos[i]);
            CharacterHandler view = newCharacter.GetComponent<CharacterHandler>();
            view.SetupVisuals(team[i].Data.CharacterSprite);
            _views[team[i]] = view;
        }
    }


    private void HandleTurnStart(BattleUnit actor)
    {
        _currentActor = actor;
    }


    private void HandleSkillUsed(BattleUnit actor, SkillData skill)
    {
        //maybe play animation or sth 
    }


    private void HandleDamaged(BattleUnit actor, BattleUnit target, int dealt, bool crit, CommandRanting rating)
    {
        //show dame/parry on top of actor
        bool isPlayer = actor.Team == Team.Player;
        string popup;
        Color color;

        if (dealt > 0)
        {
            popup = crit ? dealt + "!" : dealt.ToString();
            color = crit ? _critDameColor : _normalDameColor;
        }
        else if (isPlayer && rating == CommandRanting.Miss)
        {
            popup = "MISS!";
            color = _missColor;
        }
        else if (!isPlayer && rating == CommandRanting.Miss)
        {
            popup = "PARRY!";
            color = _parryColor;
        }
        else
        {
            popup = "NO DAME!";
            color = _normalDameColor;
        }
        ShowPopup(target, popup, color);
    }


    private void ShowPopup(BattleUnit unit, string text, Color color)
    {
        if (!_views.TryGetValue(unit, out var view))
            return;

        view.StopCoroutineIfExist();
        view.AssignCoroutine(StartCoroutine(PopupRoutine(view, text, color)));

    }


    private IEnumerator PopupRoutine(CharacterHandler view, string text, Color c)
    {
        view.PopupEnable(text);
        yield return view.PopupRoutine(c);
        view.PopupDisable();
    }


    private void HandleHealed(BattleUnit actor, BattleUnit target, int amount)
    {
        string popup = "+" + amount;
        ShowPopup(target, popup, _healColor);
    }


    private void HandleCommandBegin(IActionCommand cmd)
    {
        if (cmd is not TimingCommand timing)
            return;

        _timingPanel.Setup(timing.GoodWindow, timing.MissWindow, timing.PerfectWindow);
        timing.OnProgress = _timingPanel.SetCursorOffset;

        bool isParry = _currentActor != null && _currentActor.Team == Team.Enemy;
        string text = isParry ? "Parry! Press E" : "Press E";
        if (_ratingRoutine != null)
        {
            StopCoroutine(_ratingRoutine);
            _ratingRoutine = null;
        }
        _timingPanel.HideText();
    }


    private IEnumerator RatingRoutine(string label, Color color)
    {
        _timingPanel.SetText(label, color);
        yield return new WaitForSeconds(0.6f);
        _timingPanel.HideText();
        _ratingRoutine = null;
    }


    private void HandleCommandEnd(CommandRanting rating)
    {
        _timingPanel.Hide();
        bool isParry = _currentActor != null && _currentActor.Team == Team.Enemy;
        string label;
        Color color;

        switch (rating)
        {
            case CommandRanting.Perfect:
                label = isParry ? "PARRY!" : "PERFECT!";
                color = _parryColor;
                break;
            case CommandRanting.Good:
                label = "GOOD";
                color = _normalDameColor;
                break;
            default:
                label = "MISS";
                color = _missColor;
                break;
        }

        if (_ratingRoutine != null)
            StopCoroutine(_ratingRoutine);
        _ratingRoutine = StartCoroutine(RatingRoutine(label, color));
    }


    private void HandleBattleEnd(Team winner)
    {
        StartCoroutine(ResultRoutine(winner));
    }


    private IEnumerator ResultRoutine(Team winner)
    {
        yield return new WaitForSeconds(0.8f);
        _menu.gameObject.SetActive(false);
        _timingPanel.gameObject.SetActive(false);

        bool isWin = winner == Team.Player;
        string text = isWin ? "VICTORY!" : "DEFEAT";
        _resultPanel.SetResultText(text);

        yield return null;
        while (!KBInputUtils.KeyEClick())
            yield return null;

        BattleContext.Finish(winner);
        SceneController.SwitchScene(BattleContext.ReturnScene);
    }


    public void RequestChoice(BattleUnit actor, IReadOnlyList<BattleUnit> all, Action<ActionChoice> onChosen)
    {
        StartCoroutine(MenuRoutine(actor, all, onChosen));
    }


    private IEnumerator MenuRoutine(BattleUnit actor, IReadOnlyList<BattleUnit> all, Action<ActionChoice> onChosen)
    {
        var skills = actor.Skills;
        int skillIndex = 0;
        SkillData skill = null;
        BattleUnit picked = null;

        _menu.AddSkillsData(actor, skills);
        _menu.gameObject.SetActive(true);

        while (true)    
        {
            //choose skill
            while (true)
            {
                _menu.SetPrePickSkill(skillIndex);
                if (PlayerMenuUp())
                {
                    _menu.UnSetPrePickSkill(skillIndex);
                    skillIndex = (skillIndex - 1 + skills.Count) % skills.Count;
                }
                else if (PlayerMenuDown())
                {
                    _menu.UnSetPrePickSkill(skillIndex);
                    skillIndex = (skillIndex + 1) % skills.Count;
                }
                else if (PlayerInteractKey() && actor.Mp >= skills[skillIndex].MpCost)
                    break;

                yield return null;
            }

            skill = skills[skillIndex];
            yield return null;

            //choose target
            var candidates = BattleTargeting.GetEffectedCandidates(actor, skill, all);
            picked = candidates.Count > 0 ? candidates[0] : null;
            bool isBack = false;

            if (BattleTargeting.IsNeedPickingTarget(skill, candidates.Count))
            {
                int currentTarget = 0;
                while (true)
                {
                    _views.TryGetValue(candidates[currentTarget], out CharacterHandler currentHandler);
                    currentHandler.ShowMarker();
                    if (PlayerMenuUp() || PlayerMenuLeft())
                    {
                        currentHandler.HideMarker();
                        currentTarget = (currentTarget - 1 + candidates.Count) % candidates.Count;
                    }
                    else if (PlayerMenuDown() || PlayerMenuRight())
                    {
                        currentHandler.HideMarker();
                        currentTarget = (currentTarget + 1) % candidates.Count;
                    }
                    else if (PlayerDialogEscapeKey())
                    {
                        isBack = true;
                        break;
                    }
                    else if (PlayerInteractKey())
                    {
                        picked = candidates[currentTarget];
                        break;
                    }
                    yield return null;
                }
                _views.TryGetValue(candidates[currentTarget], out CharacterHandler current);
                current.HideMarker();
                yield return null;
            }

            if (isBack)
                continue;
            break;
        }

        _menu.gameObject.SetActive(false);
        onChosen(new ActionChoice { Skill = skill, Target = picked });
    }
}
