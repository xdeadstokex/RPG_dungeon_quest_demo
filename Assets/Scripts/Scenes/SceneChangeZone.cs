using Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Scenes
{
    public class SceneChangeZone : MonoBehaviour, ITriggerZone
    {
        [SerializeField] private string _targetScene;
        private ISceneLoader _loader;
        private bool _isLoading;


        public void Construct(ISceneLoader loader)
        {
            _loader = loader;
        }

        public void OnEnter()
        {
            if (_isLoading)
                return;
            _isLoading = true;
            _loader.LoadWithTransition(_targetScene);
        }

        public void OnExit()
        {
            //just mock for now (i really dont no to do with this ;))) )
        }
    }
}
