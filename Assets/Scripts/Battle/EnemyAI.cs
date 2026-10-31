using Assets.Scripts.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Battle
{
    public static class EnemyAI
    {
        public static ActionChoice Decide(BattleUnit self, IReadOnlyList<BattleUnit> all)
        {
            var target = all.Where(u => u.Team == Team.Player && !u.IsDead);
            target.OrderBy(u => u.Hp).First();
            return new ActionChoice { Skill = self.Data.Skills[0], Target = target  };
        }
    }
}
