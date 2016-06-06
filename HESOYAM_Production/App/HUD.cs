using HESOYAM_Production;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Graphics;


namespace App
{

    public class HUD
    {
        private Engine game;
        private InputState inputState;
        private Dictionary<String, Texture2D> textures;

        public HUD(Engine game, InputState inputState, Dictionary<String, Texture2D> textures)
        {
            this.game = game;
            this.textures = textures;
            this.inputState = inputState;
        }

        public void Draw()
        {
            this.game.spriteBatch.Begin();
            this.DrawPlayPauseButton();
            this.game.spriteBatch.End();
        }

        private void DrawPlayPauseButton()
        {
            int x = this.game.Graphics().Viewport.Width / 2 - 25;
            int y = this.game.Graphics().Viewport.Height - 50;
            Rectangle rec = new Rectangle(x, y, 50, 50);

            Texture2D button = this.GetPlayOrPauseButtonTexture(rec);

            this.game.spriteBatch.Draw(button, rec, Color.White);
        }

        private Texture2D GetPlayOrPauseButtonTexture(Rectangle rec)
        {
            String buttonTexture = "";
            var mousePoint = new Point(
                                 this.inputState.Mouse.CurrentMouseState.X,
                                 this.inputState.Mouse.CurrentMouseState.Y
                             );

            if (this.game.playMode) {
                buttonTexture = "pause_button";
            } else {
                buttonTexture = "play_button";
            }

            if (rec.Contains(mousePoint)) {
                buttonTexture += "_hover";
            }

            return textures[buttonTexture];
        }
    }
}
