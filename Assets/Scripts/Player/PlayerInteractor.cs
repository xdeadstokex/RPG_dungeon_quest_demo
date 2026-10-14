using Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerInteractor
    {
        private readonly Transform _transform;
        private readonly ICollisionQuery _collision;


        public PlayerInteractor(ICollisionQuery collision, Transform transform)
        {
            _collision = collision;
            _transform = transform;
        }

        public void TryInteract(Vector2 facing)
        {
            var position = (Vector2)_transform.position + facing;
            _collision.FindInteractable(position)?.Interact();
        }
    }
}
