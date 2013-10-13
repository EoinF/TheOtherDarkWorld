using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheOtherDarkWorld
{
    /// <summary>
    /// Contains the actions to be performed when trigger conditions are met
    /// </summary>
    public interface IObjectEffect
    {
        void OnKill();
        void OnClick();
    }
}
