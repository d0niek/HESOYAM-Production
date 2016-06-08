﻿using HESOYAM_Production;
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
                DrawAvatarBorder(avatar);
            }

            game.spriteBatch.Draw(game.Textures[avatar.TextureName], avatar.GetAvatarRectangle(), Color.White);
        }

        private void DrawAvatarBorder(Avatar avatar)
        {
            Rectangle rec = new Rectangle(avatar.X - 3, avatar.Y - 3, 56, 56);
            game.spriteBatch.Draw(game.Textures["avatar_active"], rec, Color.White);
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
                    teammate.setHover(true);
                    selectedTeammate = teammate;
                    return;
                }

                teammate.setHover(false);
            }

            foreach (Avatar avatar in avatars) {
                if (avatar.GetAvatarRectangle().Contains(game.InputState.Mouse.GetMouseLocation())) {
                    avatar.Character.setHover(true);
                    selectedTeammate = avatar.Character;
                    return;
                }

                avatar.Character.setHover(false);
            }

            selectedTeammate = null;
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