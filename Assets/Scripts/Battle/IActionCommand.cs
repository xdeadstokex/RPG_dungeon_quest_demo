using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Battle
{
    public interface IActionCommand
    {
        //run till end -> callback onDone
        public IEnumerator Run(Action<CommandRanting> onDone);
    }
}
