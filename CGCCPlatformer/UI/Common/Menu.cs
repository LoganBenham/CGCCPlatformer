using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

//Written by Logan Benham

namespace CGCCPlatformer.UI.Common
{
    public abstract class Menu : InteractiveWindow
    {
        public virtual Option[] Options { get; protected set; }
        public IEnumerable<Option> ActiveOptions => Options.Where(opt => opt.Opt().Activatable);

        protected Menu(Rectangle bounds) : base(bounds)
        {
        }

        protected Menu()
        {
        }

        protected bool KeyboardSelect(Input input)
        {
            int i = -1;
            if (input.KeyPress(Keys.D1)) i = 0;
            else if (input.KeyPress(Keys.D2)) i = 1;
            else if (input.KeyPress(Keys.D3)) i = 2;
            else if (input.KeyPress(Keys.D4)) i = 3;
            else if (input.KeyPress(Keys.D5)) i = 4;
            else if (input.KeyPress(Keys.D6)) i = 5;
            else if (input.KeyPress(Keys.D7)) i = 6;
            else if (input.KeyPress(Keys.D8)) i = 7;
            else if (input.KeyPress(Keys.D9)) i = 8;

            if (i < ActiveOptions.Count() && i > -1)
            {
                ActiveOptions.ElementAt(i).Execute();
                return true;
            }
            return false;
        }
    }
}