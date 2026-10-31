using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Battle
{
    public static class CommandFactory
    {
        public static IActionCommand Create(SkillData skill)
        {
            switch (skill.Command)
            {
                case CommandKind.Timing:
                    return new TimingCommand(skill.CommandDuration, skill.GoodWindow, skill.PerfectWindow);
                default:
                    return null;
            }
        }
    }
}
