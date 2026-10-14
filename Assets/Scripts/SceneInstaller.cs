using Assets.Scripts.Core;
using Assets.Scripts.Infrastructure;
using Assets.Scripts.Scenes;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class SceneInstaller : MonoBehaviour
    {
        [SerializeField] private LayerMask _solidLayer, _interactiveLayer;
        [SerializeField] private GameController _game;
        [SerializeField] private PlayerController _player;
        [SerializeField] private DialogManager _dialog;
        [SerializeField] private PopupIconView _popup;


        private void Awake()
        {
            IInputService input = new KeyboardInputService();
            IGameStateService state = new GameStateService();
            ICollisionQuery collision = new PhysicsCollisionQuery(_solidLayer, _interactiveLayer);
            ISceneLoader sceneLoader = SceneLoader.Shared;

            _player.Construct(input, collision);
            _dialog.Construct(input);
            _game.Construct(state);
            _popup.Bind(_player.Zones);

            foreach (var npc in FindObjectsByType<NPCController>(FindObjectsSortMode.None))
                npc.Construct(_dialog);
            foreach (var c in FindObjectsByType<ChallengerController>(FindObjectsSortMode.None))
                c.Construct(sceneLoader);
            foreach (var z in FindObjectsByType<SceneChangeZone>(FindObjectsSortMode.None))
                z.Construct(sceneLoader);

            _dialog.OnOpened += () => { state.Set(GameState.Dialog); _player.Freeze(); };
            _dialog.OnClosed += () => state.Set(GameState.FreeRoam);

            _game.Register(GameState.FreeRoam, _player);
            _game.Register(GameState.Dialog, _dialog);

        }
    }
}
