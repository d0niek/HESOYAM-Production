using HESOYAM_Production;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace App
{

    public class HUD
    {
        private Engine game;
        bool mouseLeftClicked = false;

        public HUD(Engine game)
        {
            this.game = game;
        }

        public void Draw()
        {
            this.game.SpriteBatch.Begin();
            this.DrawAvatars();
            this.DrawFotterBar();
            this.DrawPlayPauseButton();
            this.game.SpriteBatch.End();
        }

        private void DrawAvatars()
        {
            const int move = 10;
            this.DrawAvatar("avatar_1", move, 40);
            this.DrawAvatar("avatar_2", move, 90 + move);
            this.DrawAvatar("avatar_3", move, 140 + 2 * move);
        }

        private void DrawAvatar(String avatarName, int x, int y)
        {
            Rectangle rec = new Rectangle(x, y, 50, 50);

            this.game.SpriteBatch.Draw(this.game.Textures[avatarName], rec, Color.White);
        }

        private void DrawPlayPauseButton()
        {
            int x = this.game.GraphicsDevice.Viewport.Width / 2 - 25;
            int y = this.game.GraphicsDevice.Viewport.Height - 50;
            Rectangle rec = new Rectangle(x, y, 50, 50);

            Texture2D button = this.GetPlayOrPauseButtonTexture(rec);

            this.game.SpriteBatch.Draw(button, rec, Color.White);
        }

        private Texture2D GetPlayOrPauseButtonTexture(Rectangle rec)
        {
            String buttonTexture = "";
            var mousePoint = new Point(
                                 this.game.InputState.Mouse.CurrentMouseState.X,
                                 this.game.InputState.Mouse.CurrentMouseState.Y
                             );

            if (this.game.PlayMode) {
                buttonTexture = "pause_button";
            } else {
                buttonTexture = "play_button";
            }

            if (rec.Contains(mousePoint)) {
                buttonTexture += "_hover";

                if (this.game.InputState.Mouse.CurrentMouseState.LeftButton == ButtonState.Pressed &&
                    this.mouseLeftClicked == false) {
                    this.game.PlayMode = !this.game.PlayMode;
                    this.mouseLeftClicked = true;
                } else if (this.game.InputState.Mouse.CurrentMouseState.LeftButton == ButtonState.Released) {
                    this.mouseLeftClicked = false;
                }
            }

            return game.Textures[buttonTexture];
        }

        private void DrawFotterBar()
        {
            Texture2D bar = game.Textures["bar"];
            int x = 0;
            int y = this.game.GraphicsDevice.Viewport.Height - bar.Height;
            Rectangle rec = new Rectangle(x, y, this.game.GraphicsDevice.Viewport.Width, bar.Height);

            this.game.SpriteBatch.Draw(bar, rec, Color.White);
        }
    }
}
