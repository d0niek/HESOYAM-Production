using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace App.Util
{

    public class Mouse
    {
        private const int borderWidth = 10;
        private GraphicsDevice GraphicsDevice;

        public MouseState CurrentMouseState {
            get;
            private set;
        }

        public MouseState LastMouseState {
            get;
            private set;
        }

        public Mouse(GraphicsDevice GraphicsDevice) : base()
        {
            this.GraphicsDevice = GraphicsDevice;
            this.CurrentMouseState = new MouseState();
            this.LastMouseState = new MouseState();
        }

        public void Update()
        {
            this.LastMouseState = this.CurrentMouseState;
            this.CurrentMouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
        }

        public bool isInGameWindow()
        {
            bool leftBorder = this.CurrentMouseState.X >= 0;
            bool topBorder = this.CurrentMouseState.Y >= 0;
            bool rightBorder = this.CurrentMouseState.X <= this.GraphicsDevice.Viewport.Width;
            bool bottomBorder = this.CurrentMouseState.Y <= this.GraphicsDevice.Viewport.Height;

            return leftBorder && topBorder && rightBorder && bottomBorder;
        }

        public bool isCloseToLeftBorder()
        {
            return this.CurrentMouseState.X >= 0 && this.CurrentMouseState.X < borderWidth;
        }

        public bool isCloseToRightBorder()
        {
            return this.CurrentMouseState.X <= this.GraphicsDevice.Viewport.Width &&
            this.CurrentMouseState.X > this.GraphicsDevice.Viewport.Width - borderWidth;
        }

        public bool isCloseToTopBorder()
        {
            return this.CurrentMouseState.Y >= 0 && this.CurrentMouseState.Y < borderWidth;
        }

        public bool isCloseToBottomBorder()
        {
            return this.CurrentMouseState.Y <= this.GraphicsDevice.Viewport.Height &&
            this.CurrentMouseState.Y > this.GraphicsDevice.Viewport.Height - borderWidth;
        }

        public bool isCloseToTopLeftCorner()
        {
            return this.isCloseToLeftBorder() && this.isCloseToTopBorder();
        }

        public bool isCloseToTopRightCorner()
        {
            return this.isCloseToRightBorder() && this.isCloseToTopBorder();
        }

        public bool isCloseToBottomLeftCorner()
        {
            return this.isCloseToLeftBorder() && this.isCloseToBottomBorder();
        }

        public bool isCloseToBottomRightCorner()
        {
            return this.isCloseToRightBorder() && this.isCloseToBottomBorder();
        }
    }
}
