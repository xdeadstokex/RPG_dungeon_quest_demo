using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimatorView : MonoBehaviour
    {
        private Animator _anim;


        private void Awake()
        {
            _anim = GetComponent<Animator>();
        }

        public void OnMoveChanged(Vector2 direction, bool moving)
        {
            _anim.SetBool("is_moving", moving);
            if (moving && direction.sqrMagnitude > 0.01f)
            {
                _anim.SetFloat("mx", direction.x);
                _anim.SetFloat("my", direction.y);
            }
        }
    }
}
