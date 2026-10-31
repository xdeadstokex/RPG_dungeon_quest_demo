using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Battle
{
    public enum Team { Player, Enemy }
    public enum CommandRanting { Miss, Good, Perfect }
    public enum CommandKind { None, Timing, Mash }
    public enum TargetType { SingleEnemy, AllEnemies, Self, SingleAlly }
}
