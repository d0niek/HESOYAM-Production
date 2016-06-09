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
        List<Avatar> avatars;
        bool mouseLeftClicked = false;
        GameObject hoverTeammate;
        GameObject selectedTeammate;

        public HUD(Engine game)
        {
            this.game = game;
            this.teammates = game.Scene.children["Characters"].children["Teammates"].children;
            this.avatars = new List<Avatar>();
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
            const int padding = 10;
            int i = 1;

            foreach (GameObject teammate in teammates.Values) {
                Avatar avatar = new Avatar(teammate, "avatar_" + i, padding, 40 + (i - 1) * 50 + (i - 1) * padding);
                avatars.Add(avatar);
                DrawAvatar(teammate, avatar);
                i++;
            }
        }

        private void DrawAvatar(GameObject teammate, Avatar avatar)
        {
            if (teammate == selectedTeammate) {
                DrawAvatarBorder(avatar, "active");
            } else if (teammate == hoverTeammate) {
                DrawAvatarBorder(avatar, "hover");
            }

            game.spriteBatch.Draw(game.Textures[avatar.TextureName], avatar.GetAvatarRectangle(), Color.White);
        }

        private void DrawAvatarBorder(Avatar avatar, String avatarBorder)
        {
            Rectangle rec = new Rectangle(avatar.X - 3, avatar.Y - 3, 56, 56);
            game.spriteBatch.Draw(game.Textures["avatar_" + avatarBorder], rec, Color.White);
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

            if (rec.Contains(game.InputState.Mouse.GetMouseLocation())) {
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
                    HighlightTeammateAndCheckIfUserClickLeftButton(teammate);
                    return;
                }

                teammate.setHover(false);
            }

            foreach (Avatar avatar in avatars) {
                if (avatar.GetAvatarRectangle().Contains(game.InputState.Mouse.GetMouseLocation())) {
                    HighlightTeammateAndCheckIfUserClickLeftButton(avatar.Character);
                    return;
                }

                avatar.Character.setHover(false);
            }

            hoverTeammate = null;
        }

        private void HighlightTeammateAndCheckIfUserClickLeftButton(GameObject teammate)
        {
            teammate.setHover(true);
            hoverTeammate = teammate;

            if (IsMouseLeftButtonPressed() && !mouseLeftClicked) {
                UpdateSelectedTeammate(teammate);
                mouseLeftClicked = true;
            } else if (!IsMouseLeftButtonPressed()) {
                mouseLeftClicked = false;
            }
        }

        private void UpdateSelectedTeammate(GameObject teammate)
        {
            ResetSelectedTeammate();

            teammate.setActive(true);
            selectedTeammate = teammate;
        }

        public void ResetSelectedTeammate()
        {
            if (selectedTeammate != null) {
                selectedTeammate.setActive(false);
            }

            selectedTeammate = null;
        }

        private void SelectDoorOrInteractiveModel()
        {
            String[] sceneObjectsToLoop = { "Doors", "Interactive" };

            foreach (String objectsToLoop in sceneObjectsToLoop) {
                if (LoopObjectsAndHighlightObjectUnderMouse(game.Scene.children[objectsToLoop].children)) {
                    return;
                }
            }
        }

        private bool LoopObjectsAndHighlightObjectUnderMouse(Dictionary<String, IGameObject> gameObjects)
        {
            foreach (GameObject interactive in gameObjects.Values) {
                if (interactive.IsMouseOverObject()) {
                    interactive.setHover(true);
                    return true;
                }

                interactive.setHover(false);
            }

            return false;
        }
    }
}
