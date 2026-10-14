using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Core
{
    public interface IDialogService
    {
        public event Action OnOpened;
        public event Action OnClosed;
        public void Show(Dialog dialog);
    }
}
