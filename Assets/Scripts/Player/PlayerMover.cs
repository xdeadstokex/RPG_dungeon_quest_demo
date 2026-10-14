using Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerMover
    {
        private readonly Transform _transform;
        private readonly ICollisionQuery _collision;
        private readonly Vector2 _speed;
        private readonly float _accel, _decel;
        private Vector2 _velocity;
        private Vector2 _lastInput;
        private bool _wasMoving;
        public Vector2 Facing { get; private set; } = Vector2.down;
        public event Action<Vector2, bool> MoveChanged;


        public PlayerMover(ICollisionQuery collision, Transform transform, Vector2 speed, float accel, float decel)
        {
            _collision = collision;
            _transform = transform;
            _speed = speed;
            _accel = accel;
            _decel = decel;
        }

        public void Tick(Vector2 input, float dt) 
        {
            if (input.sqrMagnitude > 1)
                input.Normalize();

            var target = Vector2.Scale(input, _speed);
            float rate = input == Vector2.zero ? _decel : _accel;
            _velocity = Vector2.MoveTowards(_velocity, target, rate * dt);
            bool moving = _velocity.sqrMagnitude > 0.01f;

            if (moving && !TryMove(_velocity * dt)) //block -> stop animation
            { 
                _velocity = Vector2.zero;
                moving = false;
            }

            if (input != Vector2.zero)
                Facing = ToCardinal(input);

            if (moving != _wasMoving || input != _lastInput)
                MoveChanged?.Invoke(input, moving);

            _wasMoving = moving;
            _lastInput = input;
        }

        public void Stop()
        {
            _velocity = Vector2.zero;
            _wasMoving = false;
            _lastInput = Vector2.zero;
            MoveChanged?.Invoke(Vector2.zero, false);
        }

        private static Vector2 ToCardinal(Vector2 distance)
        {
            if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
                return new Vector2(Mathf.Sign(distance.x), 0);
            return new Vector2(0, Mathf.Sign(distance.y));
        }

        private bool TryMove(Vector2 distance)
        {
            Vector2 pos = _transform.position;
            bool moved = false;
            Vector2 moveX = pos + new Vector2(distance.x, 0);
            Vector2 moveY = pos + new Vector2(0, distance.y);

            if (distance.x != 0 && _collision.IsWalkable(moveX))
            {
                pos.x += distance.x;
                moved = true;
            }

            if (distance.y != 0 && _collision.IsWalkable(moveY))
            {
                pos.y += distance.y;
                moved = true;
            }
            _transform.position = pos;
            return moved;
        }
    }
}
