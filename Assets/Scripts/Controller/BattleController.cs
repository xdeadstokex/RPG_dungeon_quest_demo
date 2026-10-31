using Assets.Scripts.Battle;
using Assets.Scripts.Data;
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

        private readonly List<BattleUnit> _units = new();
        private readonly TurnQueue _queue = new();

        //event for UI
        public event Action<IReadOnlyList<BattleUnit>> OnBattleStart;
        public event Action<BattleUnit> OnTurnStart;
        public event Action<BattleUnit, BattleUnit, int, bool, CommandRanting> OnDamaged;
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
            //add all battle unit
            foreach (var unit in BattleContext.PlayerParty)
                _units.Add(new BattleUnit(unit, Team.Player));
            foreach (var unit in BattleContext.Enemies)
                _units.Add(new BattleUnit(unit, Team.Enemy));
            OnBattleStart?.Invoke(_units);
            yield return null;

            while (true)
            {
                State = BattleState.TurnStart;
                var actor = _queue.Next(_units);
                OnTurnStart?.Invoke(actor);

                State = BattleState.ChoooseAction;
                ActionChoice choice = default;
                if (actor.Team == Team.Player && Menu != null)
                {
                    bool chosen = false;
                    Menu.RequestChoice(actor, _units, c => { choice = c; chosen = true; });
                    yield return new WaitUntil(() => chosen);
                }
                else
                {
                    choice = EnemyAI.Decide(actor, _units);
                }

                State = BattleState.Execute;
                yield return ExecuteAction(actor, choice);

                State = BattleState.CheckEnd;
                if (!_units.Any(u => u.Team == Team.Enemy && !u.IsDead))
                {
                    State = BattleState.Win;
                    OnBattleEnd?.Invoke(Team.Player);
                    yield break;
                }
                if (!_units.Any(u => u.Team == Team.Player && !u.IsDead))
                {
                    State = BattleState.Lose;
                    OnBattleEnd?.Invoke(Team.Enemy);
                    yield break;
                }
            }
        }

        private IEnumerator ExecuteAction(BattleUnit actor, ActionChoice choice)
        {
            if (choice.Skill == null || choice.Target == null)
                yield break;

            if (choice.Skill.MpCost > 0 && !actor.TrySpendMp(choice.Skill.MpCost))
                yield break;

            CommandRanting rating = CommandRanting.Good;
            float scale;

            if (actor.Team == Team.Player)
            {
                IActionCommand cmd = CommandFactory.Create(choice.Skill);
                if (cmd != null)
                {
                    yield return new WaitForSeconds(0.25f);
                    OnCommandBegin?.Invoke(cmd);
                    yield return cmd.Run(r => rating = r);
                    OnCommandEnd?.Invoke(rating);
                }
                scale = DamageCalculator.AttackScale(rating);
            }
            else
            {
                var defend = new TimingCommand(0.8f, 0.2f, 0.06f); //just temp for now
                yield return new WaitForSeconds(0.4f);
                OnCommandBegin?.Invoke(defend);
                yield return defend.Run(r => rating = r);
                scale = DamageCalculator.AttackScale(rating);
            }

            int rawDamage = DamageCalculator.Raw(actor, choice.Skill, scale, out bool crit);
            int dealt = choice.Target.TakeDamage(rawDamage);

            if (actor.Team == Team.Player)
                actor.GainMp(choice.Skill.MpGain);
            OnDamaged?.Invoke(actor, choice.Target, dealt, crit, rating);

            yield return new WaitForSeconds(0.5f);
        }
    }
}
