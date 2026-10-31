using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Battle
{
    public class TimingCommand : IActionCommand
    {
        private readonly float _duration, _good, _perfect;
        public float GoodWindow => _good;
        public float PerfectWindow => _perfect;
        public Action<float> OnProgress; //draw view bar


        public TimingCommand(float duration, float good, float perfect)
        {
            _duration = duration;
            _good = good;
            _perfect = perfect;
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
