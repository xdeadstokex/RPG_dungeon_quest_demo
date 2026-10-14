using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Core
{
    public interface ISceneLoader
    {
        public string PendingScene { get; }
        public void Load(string sceneName);
        public void LoadWithTransition(string sceneName);
    }
}
