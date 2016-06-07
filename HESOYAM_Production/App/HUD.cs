using HESOYAM_Production;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace App
{

    public class HUD
    {
        private Engine game;
        private InputState inputState;
        private Dictionary<String, Texture2D> textures;
        bool mouseLeftClicked = false;

        public HUD(Engine game, InputState inputState, Dictionary<String, Texture2D> textures)
        {
            this.game = game;
            this.textures = textures;
            this.inputState = inputState;
        }

        public void Draw()
        {
            this.game.spriteBatch.Begin();
            this.DrawAvatars();
            this.DrawFotterBar();
            this.DrawPlayPauseButton();
            this.game.spriteBatch.End();
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

            this.game.spriteBatch.Draw(this.textures[avatarName], rec, Color.White);
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

                if (this.inputState.Mouse.CurrentMouseState.LeftButton == ButtonState.Pressed &&
                    this.mouseLeftClicked == false) {
                    this.game.playMode = !this.game.playMode;
                    this.mouseLeftClicked = true;
                } else if (this.inputState.Mouse.CurrentMouseState.LeftButton == ButtonState.Released) {
                    this.mouseLeftClicked = false;
                }
            }

            return textures[buttonTexture];
        }

        private void DrawFotterBar()
        {
            Texture2D bar = textures["bar"];
            int x = 0;
            int y = this.game.Graphics().Viewport.Height - bar.Height;
            Rectangle rec = new Rectangle(x, y, this.game.Graphics().Viewport.Width, bar.Height);

            this.game.spriteBatch.Draw(bar, rec, Color.White);
        }
    }
}
