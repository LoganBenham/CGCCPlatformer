using Microsoft.Xna.Framework;

namespace CGCCPlatformer.UI.Common
{
    public abstract class GameWindow
    {
        protected Rectangle bounds;
        public static TheGame Game => TheGame.Game;

        public virtual Rectangle Bounds
        {
            get { return bounds; }
            set { bounds = value; }
        }

        protected GameWindow(Rectangle _bounds)
        {
            bounds = _bounds;
        }

        protected GameWindow()
        {
            bounds = TheGame.Bounds;
        }

        //protected GameWindow(GameWindow template)
        //{
        //    Bounds = template.Bounds;
        //}

        public virtual void Draw(GameTime gameTime, Point mousePos)
        {
            Draw(gameTime);
        }

        public abstract void Draw(GameTime gameTime);

        public abstract void Update(GameTime gameTime);

        public virtual void LoadContent()
        {
        }
    }
}