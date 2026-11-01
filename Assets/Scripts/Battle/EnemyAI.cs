using Assets.Scripts.Controller;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Battle
{
    //basic ai for now
    public static class EnemyAI
    {
        public static ActionChoice Decide(BattleUnit self, IReadOnlyList<BattleUnit> all)
        {
            var target = all.Where(u => u.Team == Team.Player && !u.IsDead).OrderBy(u => u.Hp).First();
            var skill = self.Data.Skills != null && self.Data.Skills.Length > 0 ? self.Data.Skills[0] : null;
            return new ActionChoice { Skill = skill, Target = target };
        }
    }
}
