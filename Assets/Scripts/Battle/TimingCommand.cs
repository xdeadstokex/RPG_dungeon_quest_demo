using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Battle
{
    public class TimingCommand : IActionCommand
    {
        private readonly float _duration, _good, _miss, _perfect;
        public float GoodWindow => _good;
        public float MissWindow => _miss;
        public float PerfectWindow => _perfect;
        public Action<float> OnProgress; //draw view bar


        public TimingCommand(float good, float miss, float perfect)
        {
            _good = good;
            _miss = miss;
            _perfect = perfect;
            _duration = 2 * Mathf.Max(good, miss, perfect);
        }


        public IEnumerator Run(Action<CommandRanting> onDone)
        {
            float t = 0f;
            while (t < _duration)
            {
                t += TimeUtils.GetFrameTimeSec();
                float p = t / _duration;
                OnProgress?.Invoke(p);

                if (KBInputUtils.KeyEClick())
                {
                    float dist = Mathf.Abs(p - 0.5f);
                    if (dist <= _perfect)
                        onDone(CommandRanting.Perfect);
                    else if (dist <= _miss)
                        onDone(CommandRanting.Miss);
                    else if (dist <= _good)
                        onDone(CommandRanting.Good);
                    else
                        onDone(CommandRanting.Miss);

                    yield break;
                }
                yield return null;
            }
            onDone(CommandRanting.Miss);
        }
    }
}
