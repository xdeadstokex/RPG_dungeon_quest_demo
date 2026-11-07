namespace Assets.Scripts.Battle
{
    /// <summary>
    /// create the command base on the type 
    /// </summary>
    public static class CommandFactory
    {
        public static IActionCommand Create(SkillData skill)
        {
            switch (skill.Command)
            {
                case CommandKind.Timing:
                    return new TimingCommand(skill.CommandDuration, skill.GoodWindow, skill.MissWindow, skill.PerfectWindow);
                case CommandKind.None:
                    return null;
                case CommandKind.Mash:
                    return null;
                default:
                    return null;
            }
        }
    }
}
