using Assets.Scripts.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    public enum BattleState { Init, TurnStart, ChoooseAction, Execute, CheckEnd, Win, Lose }


    public struct ActionChoice
    {
        public SkillData Skill;
        public BattleUnit Target;
    }


    public interface IBattleMenu
    {
        public void RequestChoice(BattleUnit actor, IReadOnlyList<BattleUnit> all, Action<ActionChoice> onChosen);
    }


    public class BattleController : MonoBehaviour
    {
        public BattleState State { get; private set; }
        public IBattleMenu Menu { get; set; }
        public IReadOnlyList<BattleUnit> Units => _units;
        private bool _hasChosen;
        private ActionChoice _choice;
        private readonly List<BattleUnit> _units = new();
        private readonly TurnQueue _queue = new();


        //event for UI
        public event Action<IReadOnlyList<BattleUnit>> OnBattleStart;
        public event Action<BattleUnit> OnTurnStart;
        public event Action<BattleUnit, SkillData> OnSkillUsed;
        public event Action<BattleUnit, BattleUnit, int, bool, CommandRanting> OnDamaged;
        public event Action<BattleUnit, BattleUnit, int> OnHealed;
        public event Action<IActionCommand> OnCommandBegin;
        public event Action<CommandRanting> OnCommandEnd;
        public event Action<Team> OnBattleEnd;


        public Func<BattleUnit, IReadOnlyList<BattleUnit>, IEnumerator> ChooseActionRoutine;


        public void Start()
        {
            StartCoroutine(BattleLoop());
        }


        private IEnumerator BattleLoop()
        {
            State = BattleState.Init;
            BattleListInit();
            OnBattleStart?.Invoke(_units);
            yield return new WaitForSeconds(0.5f);

            while (true)
            {
                //step 1: choose actor
                State = BattleState.TurnStart;
                var actor = _queue.Next(_units);
                if (actor == null)
                {
                    EndBattle(Team.Enemy);
                    yield break;
                }
                OnTurnStart?.Invoke(actor);


                //step 2: choose skill and target
                State = BattleState.ChoooseAction;
                yield return GetChoice(actor);


                //step 3: execute
                State = BattleState.Execute;
                yield return ExecuteAction(actor, _choice);


                //step 4: check end
                State = BattleState.CheckEnd;
                yield return HandleIfBattleEnd();
            }
        }


        private IEnumerator ExecuteAction(BattleUnit actor, ActionChoice choice)
        {
            SkillData skill = choice.Skill;
            if (skill == null)
                yield break;

            List<BattleUnit> targets = BattleTargeting.HandleCastSkillOnTarget(actor, skill, choice.Target, _units);
            if (targets.Count == 0)
                yield break;

            if (skill.MpCost > 0 && !actor.TrySpendMp(skill.MpCost))
                yield break;

            OnSkillUsed?.Invoke(actor, skill);

            bool isPlayer = actor.Team == Team.Player;
            bool support = BattleTargeting.IsSupportSkill(skill);
            CommandRanting rating = CommandRanting.Good;
            float scale;

            if (isPlayer)
            {
                //player attacks / heals with a timing command
                IActionCommand cmd = CommandFactory.Create(skill);
                if (cmd != null)
                {
                    yield return new WaitForSeconds(0.25f);
                    OnCommandBegin?.Invoke(cmd);
                    yield return cmd.Run(r => rating = r);
                    OnCommandEnd?.Invoke(rating);
                }
                scale = DamageCalculator.AttackScale(rating);
                if (support)
                    scale = Mathf.Max(0.5f, scale); //missed heal still heal a little
            }
            else if (support)
            {
                //enemy buff/heal -> player do nothing
                yield return new WaitForSeconds(0.4f);
                scale = 1f;
            }
            else
            {
                //enemy attacks -> player parry with timming command
                var defend = new TimingCommand(0.8f, 0.0f, 0.8f, 0.06f); //just temp for now
                yield return new WaitForSeconds(0.4f);
                OnCommandBegin?.Invoke(defend);
                yield return defend.Run(r => rating = r);
                OnCommandEnd?.Invoke(rating);
                scale = DamageCalculator.DefendScale(rating);
            }

            foreach (var target in targets)
            {
                int raw = DamageCalculator.Raw(actor, skill, scale, out bool crit);
                if (support)
                {
                    int before = target.Hp;
                    target.Heal(raw);
                    OnHealed?.Invoke(actor, target, target.Hp - before);
                }
                else
                {
                    int dealt = target.TakeDamage(raw);
                    OnDamaged?.Invoke(actor, target, dealt, crit, rating);
                }
            }

            if (isPlayer)
                actor.GainMp(skill.MpGain);

            yield return new WaitForSeconds(0.7f);
        }


        //init
        private void BattleListInit()
        {
            foreach (var unit in BattleContext.PlayerParty)
                _units.Add(new BattleUnit(unit, Team.Player));
            foreach (var unit in BattleContext.Enemies)
                _units.Add(new BattleUnit(unit, Team.Enemy));
        }


        //handle battle end
        private IEnumerator HandleIfBattleEnd()
        {
            if (IsTeamAllCleared(Team.Enemy))
            {
                State = BattleState.Win;
                OnBattleEnd?.Invoke(Team.Player);
                yield break;
            }
            if (IsTeamAllCleared(Team.Player))
            {
                State = BattleState.Lose;
                OnBattleEnd?.Invoke(Team.Enemy);
                yield break;
            }
        }

        private void EndBattle(Team winner)
        {
            State = winner == Team.Player ? BattleState.Win : BattleState.Lose;
            OnBattleEnd?.Invoke(winner);
        }

        private bool IsTeamAllCleared(Team team)
        {
            return !_units.Any(u => u.Team == team && !u.IsDead);
        }


        //handle player's choose
        private void OnPlayerChose(ActionChoice choice)
        {
            _choice = choice;
            _hasChosen = true;
        }

        private IEnumerator GetChoice(BattleUnit actor)
        {
            _hasChosen = false;
            if (actor.Team == Team.Player)
            {
                Menu.RequestChoice(actor, _units, OnPlayerChose);
                yield return new WaitUntil(() => _hasChosen);
            }
            else
            {
                _choice = EnemyAI.Decide(actor, _units);
            }
        }
    }
}
