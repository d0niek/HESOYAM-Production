using HESOYAM_Production;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace App
{

    public class HUD
    {
        Engine game;
        readonly Dictionary<String, IGameObject> teammates;
        bool mouseLeftClicked = false;

        public HUD(Engine game)
        {
            this.game = game;
            this.teammates = game.Scene.children["Characters"].children["Teammates"].children;
        }

        public void Draw()
        {
            game.spriteBatch.Begin();
            DrawAvatars();
            DrawFotterBar();
            DrawPlayPauseButton();

            if (!game.PlayMode) {
                SelectTeammate();
                SelectDoorOrInteractiveModel();
            }

            game.spriteBatch.End();
        }

        private void DrawAvatars()
        {
            const int move = 10;
            DrawAvatar("avatar_1", move, 40);
            DrawAvatar("avatar_2", move, 90 + move);
            DrawAvatar("avatar_3", move, 140 + 2 * move);
        }

        private void DrawAvatar(String avatarName, int x, int y)
        {
            Rectangle rec = new Rectangle(x, y, 50, 50);

            game.spriteBatch.Draw(game.Textures[avatarName], rec, Color.White);
        }

        private void DrawPlayPauseButton()
        {
            int x = game.GraphicsDevice.Viewport.Width / 2 - 25;
            int y = game.GraphicsDevice.Viewport.Height - 50;
            Rectangle rec = new Rectangle(x, y, 50, 50);

            Texture2D button = GetPlayOrPauseButtonTexture(rec);

            game.spriteBatch.Draw(button, rec, Color.White);
        }

        private Texture2D GetPlayOrPauseButtonTexture(Rectangle rec)
        {
            String buttonTexture = game.PlayMode ? "pause_button" : "play_button";
            Point mousePoint = new Point(
                                   game.InputState.Mouse.CurrentMouseState.X,
                                   game.InputState.Mouse.CurrentMouseState.Y
                               );
            if (rec.Contains(mousePoint)) {
                buttonTexture += "_hover";

                CheckIfUserClickMouseAndTogglePlayMode();
            }

            return game.Textures[buttonTexture];
        }

        private void CheckIfUserClickMouseAndTogglePlayMode()
        {
            if (IsMouseLeftButtonPressed() && !mouseLeftClicked) {
                game.PlayMode = !game.PlayMode;
                mouseLeftClicked = true;
            } else if (!IsMouseLeftButtonPressed()) {
                mouseLeftClicked = false;
            }
        }

        private bool IsMouseLeftButtonPressed()
        {
            return game.InputState.Mouse.CurrentMouseState.LeftButton == ButtonState.Pressed;
        }

        private void DrawFotterBar()
        {
            Texture2D bar = game.Textures["bar"];
            int x = 0;
            int y = game.GraphicsDevice.Viewport.Height - bar.Height;
            Rectangle rec = new Rectangle(x, y, game.GraphicsDevice.Viewport.Width, bar.Height);

            game.spriteBatch.Draw(bar, rec, Color.White);
        }

        private void SelectTeammate()
        {
            foreach (GameObject teammate in teammates.Values) {
                if (teammate.IsMouseOverObject()) {
                    teammate.setHover(true);

                    break;
                }
            }
        }

        private void SelectDoorOrInteractiveModel()
        {
            Dictionary<String, IGameObject> models = game.Scene.children["Doors"].children;

            foreach (GameObject door in game.Scene.children["Doors"].children.Values) {
                if (door.IsMouseOverObject()) {
                    door.setHover(true);

                    return;
                }
            }

            foreach (GameObject door in game.Scene.children["Interactive"].children.Values) {
                if (door.IsMouseOverObject()) {
                    door.setHover(true);

                    return;
                }
            }
        }
    }
}
