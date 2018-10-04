using System;
using Microsoft.Xna.Framework;

namespace CGCCPlatformer.UI.Common
{
    public abstract class InteractiveWindow : GameWindow
    {
        protected InteractiveWindow(Rectangle bounds) : base(bounds)
        {
        }

        protected InteractiveWindow()
        {
        }

        //protected InteractiveWindow(GameWindow template) : base(template) { }

        public abstract void Update(GameTime gameTime, Input input);

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException("Use Input argument for " + ToString() + " update");
        }

        public abstract override void Draw(GameTime gameTime, Point mousePos);

        public override void Draw(GameTime gameTime)
        {
            throw new NotImplementedException("Use mousePos argument for " + ToString() + " draw");
        }
    }
}