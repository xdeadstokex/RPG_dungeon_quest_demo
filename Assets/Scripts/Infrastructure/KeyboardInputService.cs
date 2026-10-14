using Assets.Scripts.Core;
using UnityEngine;


namespace Assets.Scripts.Infrastructure
{
    public class KeyboardInputService : IInputService
    {
        public Vector2 Move
        {
            get
            {
                float x = 0f, y = 0f;
                if (KBInputUtils.KeyD() || KBInputUtils.KeyArrowRight())
                    x = 1f;
                else if (KBInputUtils.KeyA() || KBInputUtils.KeyArrowLeft())
                    x = -1f;

                if (KBInputUtils.KeyW() || KBInputUtils.KeyArrowUp())
                    y = 1f;
                else if (KBInputUtils.KeyS() || KBInputUtils.KeyArrowDown())
                    y = -1f;

                return new Vector2(x, y);
            }
        }

        public bool InteractPressed => KBInputUtils.KeyEClick();

        public bool CancelPressed => KBInputUtils.KeyXClick();

        public bool BackPressed => KBInputUtils.KeyEscapeClick();
    }
}
