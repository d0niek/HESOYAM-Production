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
        private Dictionary<String, Texture2D> textures;

        public HUD(Engine game, Dictionary<String, Texture2D> textures)
        {
            this.game = game;
            this.textures = textures;
        }

        public void Draw()
        {
            this.game.spriteBatch.Begin();
            this.DrawPlayPauseButton();
            this.game.spriteBatch.End();
        }

        private void DrawPlayPauseButton()
        {
            Texture2D button = this.GetPlayOrPauseButton();
            int x = this.game.Graphics().Viewport.Width / 2 - (button.Width / 2);
            int y = this.game.Graphics().Viewport.Height - button.Height;

            Rectangle rec = new Rectangle(x, y, button.Width, button.Height);
            this.game.spriteBatch.Draw(button, rec, Color.White);
        }

        private Texture2D GetPlayOrPauseButton()
        {
            if (this.game.playMode) {
                return textures["pause_button"];
            } else {
                return textures["play_button"];
            }
        }
    }
}
