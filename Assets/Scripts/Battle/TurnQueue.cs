using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Battle
{
    public class TurnQueue
    {
        private readonly Queue<BattleUnit> _queue = new();
        public int Round { get; private set; }

        
         //Get next unit in a round and build a new queue if end round
        public BattleUnit Next(IReadOnlyList<BattleUnit> all)
        {
            while (true)
            {
                //new round -> refill queue
                if (_queue.Count == 0)
                {
                    Round++;
                    var sortedAll = all.Where(unit => !unit.IsDead).OrderByDescending(unit => unit.Data.Speed);
                    foreach (var u in sortedAll)
                        _queue.Enqueue(u);

                    if (_queue.Count == 0)
                        return null;
                    //shot a delegate to UI
                }

                //get unit (not dead)
                var unit = _queue.Dequeue();
                if (!unit.IsDead)
                    return unit;
            }
        }
    }
}
