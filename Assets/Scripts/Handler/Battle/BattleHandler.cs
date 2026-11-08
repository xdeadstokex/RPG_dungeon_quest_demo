using Assets.Scripts.Battle;
using Assets.Scripts.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameData;


[RequireComponent(typeof(BattleController))]
public class BattleHandler : MonoBehaviour, IBattleMenu
{
    [SerializeField] private List<Transform> _alliesPosition;
    [SerializeField] private List<Transform> _enemiesPosition;
    [SerializeField] private GameObject _unitPrefab;
    [SerializeField] private Color _critDameColor = Color.darkRed;
    [SerializeField] private Color _normalDameColor = Color.white;
    [SerializeField] private Color _missColor = Color.grey;
    [SerializeField] private Color _parryColor = Color.yellow;
    [SerializeField] private Color _healColor = Color.green;
    [SerializeField] private TimingPanelHandler _timingPanel;
    [SerializeField] private ResultPanelHandler _resultPanel;
    [SerializeField] private MenuHandler _menu;
    [SerializeField] private TextMeshProUGUI _actorTurn;
    [SerializeField] private List<BattleUnitHandler> _battleStats;


    private BattleController _battle;
    private BattleUnit _currentActor;
    private Coroutine _ratingRoutine;
    private Dictionary<BattleUnit, CharacterHandler> _views = new();
    private Dictionary<BattleUnit, BattleUnitHandler> _stats = new();


    private void Awake()
    {
        _battle = GetComponent<BattleController>();
        _menu.gameObject.SetActive(false);
        _timingPanel.Hide();
        _resultPanel.HideResultText();
        Bind(_battle);
    }


    public void Bind(BattleController battle)
    {
        battle.Menu = this;
        battle.OnBattleStart += HandleBattleStart;
        battle.OnTurnStart += HandleTurnStart;
        battle.OnSkillUsed += HandleSkillUsed;
        battle.OnDamaged += HandleDamaged;
        battle.OnHealed += HandleHealed;
        battle.OnCommandBegin += HandleCommandBegin;
        battle.OnCommandEnd += HandleCommandEnd;
        battle.OnBattleEnd += HandleBattleEnd;
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

        SpawnTeam(players, _alliesPosition);
        SpawnTeam(enemies, _enemiesPosition);
    }


    private void SpawnTeam(List<BattleUnit> team, List<Transform> positions)
    {
        for (int i = 0; i < team.Count; i++)
        {
            if (positions == null && i >= positions.Count)
                continue;

            var newCharacter = Instantiate(_unitPrefab, positions[i]);
            CharacterHandler view = newCharacter.GetComponent<CharacterHandler>();
            view.SetupVisuals(team[i].Data.CharacterSprite);
            team[i].OnHpChanged += view.SetHp;
            team[i].OnMpChanged += view.SetMp;
            
            _views[team[i]] = view;

        }
    }


    private void HandleTurnStart(BattleUnit actor)
    {
        _currentActor = actor;
        _actorTurn.text = actor.DisplayName() + "'s turn";
        foreach (var view in _views)
            view.Value.SetHighight(view.Key == actor);
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
        else if (!isPlayer && rating == CommandRanting.Perfect)
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
        if (_views.TryGetValue(unit, out var view))
            view.ShowPopup(text, color);
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
        _timingPanel.SetText(text, Color.white);
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
        while (PlayerInteractKey())
            yield return null;

        BattleContext.Finish(winner);

        string target = BattleContext.IsDemo ? SceneNames.FightScene.ToString() : BattleContext.ReturnScene;
        if (SceneController.Exists)
            SceneController.SwitchScene(target);
        else
            SceneUtils.LoadScene(target);
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
            yield return null; //wait next frame to click again

            //choose target
            var candidates = BattleTargeting.GetEffectedCandidates(actor, skill, all);
            picked = candidates.Count > 0 ? candidates[0] : null;
            bool isBack = false;

            if (BattleTargeting.IsNeedPickingTarget(skill, candidates.Count))
            {
                int cur = 0;
                while (true)
                {
                    SetMarker(candidates[cur], true);
                    if (PlayerMenuUp() || PlayerMenuLeft())
                    {
                        SetMarker(candidates[cur], false);
                        cur = (cur - 1 + candidates.Count) % candidates.Count;
                    }
                    else if (PlayerMenuDown() || PlayerMenuRight())
                    {
                        SetMarker(candidates[cur], false);
                        cur = (cur + 1) % candidates.Count;
                    }
                    else if (PlayerDialogEscapeKey())
                    {
                        isBack = true;
                        break;
                    }
                    else if (PlayerInteractKey())
                    {
                        picked = candidates[cur];
                        break;
                    }
                    yield return null;
                }
                SetMarker(candidates[cur], false);
                yield return null;
            }

            if (isBack)
                continue;
            break;
        }

        _menu.gameObject.SetActive(false);
        onChosen(new ActionChoice { Skill = skill, Target = picked });
    }


    private void SetMarker(BattleUnit unit, bool show)
    {
        if (!_views.TryGetValue(unit, out var view)) 
            return;
        if (show) 
            view.ShowMarker(); 
        else 
            view.HideMarker();
    }
}
