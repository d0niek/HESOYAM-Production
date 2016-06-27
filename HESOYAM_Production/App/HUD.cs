using HESOYAM_Production;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using App.Models;

namespace App
{

    public class HUD
    {
        readonly Engine game;
        readonly Dictionary<String, IGameObject> teammates;
        List<Avatar> avatars;
        GameObject hoverTeammate;
        public GameObject selectedTeammate;
        GameObject objectToInteract;
        String message;
        TimeSpan messageStart;
        TimeSpan messageDelay;

        public String Message {
            private get { return ""; }
            set {
                message = value;
                messageStart = TimeSpan.Zero;
            }
        }

        public HUD(Engine game)
        {
            this.game = game;
            this.teammates = game.Scene.children["Teammates"].children;
            this.avatars = new List<Avatar>();
            this.message = "";
            this.messageStart = TimeSpan.Zero;
            this.messageDelay = new TimeSpan(0, 0, 5);
        }

        public void Draw(GameTime gameTime)
        {
            game.spriteBatch.Begin();
            DrawAvatars();
            DrawFotterBar();
            DrawPlayPauseButton();

            if (!game.PlayMode) {
                SelectTeammate();
            }

            SelectInteractiveObject();
            DrawMenuToInteractWithObject();

            DrawMessage(gameTime);

            game.spriteBatch.End();
        }

        public void ResetObjectToInteract()
        {
            objectToInteract = null;
        }

        private void DrawAvatars()
        {
            const int padding = 10;
            int i = 1;

            Avatar avatar = new Avatar(game.Player, "avatar_bohater", padding, padding);
            DrawAvatar(avatar);

            foreach (Teammate teammate in teammates.Values) {
                avatar = new Avatar(teammate, "avatar_" + i, padding, 40 + i * 50 + (i - 1) * padding);
                avatars.Add(avatar);
                DrawAvatar(avatar);
                i++;
            }
        }

        private void DrawAvatar(Avatar avatar)
        {
            if (avatar.Character == selectedTeammate) {
                DrawAvatarBorder(avatar, "active");
            } else if (avatar.Character == hoverTeammate) {
                DrawAvatarBorder(avatar, "hover");
            }

            game.spriteBatch.Draw(game.Textures[avatar.TextureName], avatar.GetAvatarRectangle(), Color.White);
            DrawLifeBar(avatar);
        }

        private void DrawAvatarBorder(Avatar avatar, String avatarBorder)
        {
            Rectangle rec = new Rectangle(0, avatar.Y - 3, 100, 56);
            game.spriteBatch.Draw(game.Textures["avatar_" + avatarBorder], rec, Color.White);
        }

        private void DrawLifeBar(Avatar avatar)
        {
            int x = avatar.X + 55;
            Rectangle rec = new Rectangle(x, avatar.Y, 5, 50);
            game.spriteBatch.Draw(game.Textures["life_background"], rec, Color.White);

            int y = (int) (avatar.Character.Life * 50 / avatar.Character.MaxLife);
            rec = new Rectangle(x, avatar.Y + (50 - y), 5, y);
            game.spriteBatch.Draw(game.Textures["life"], rec, Color.White);
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
                OnMouseLeftButtonClick(game.TogglePlayMode);
            }

            return game.Textures[buttonTexture];
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
                    DrawStringCloseToMouse(teammate.name);
                    return;
                }

                teammate.Hover = false;
            }

            foreach (Avatar avatar in avatars) {
                if (avatar.GetAvatarRectangle().Contains(game.InputState.Mouse.GetMouseLocation())) {
                    HighlightTeammateAndCheckIfUserClickLeftButton(avatar.Character);
                    DrawStringCloseToMouse(avatar.Character.name);
                    return;
                }

                avatar.Character.Hover = false;
            }

            hoverTeammate = null;
        }

        private void HighlightTeammateAndCheckIfUserClickLeftButton(GameObject teammate)
        {
            teammate.Hover = true;
            hoverTeammate = teammate;

            OnMouseLeftButtonClick(() => UpdateSelectedTeammateAndSetCameraOnHim(teammate));
        }

        private void UpdateSelectedTeammateAndSetCameraOnHim(GameObject teammate)
        {
            if (selectedTeammate != teammate) {
                ResetSelectedTeammate();

                teammate.setActive(true);
                selectedTeammate = teammate;
            } else {
                SetCameraOnTeammate(teammate);
            }
        }

        public void ResetSelectedTeammate()
        {
            if (selectedTeammate != null) {
                selectedTeammate.setActive(false);
            }

            selectedTeammate = null;
        }

        private void SetCameraOnTeammate(GameObject teammate)
        {
            Vector3 cameraMove = game.Camera.position - game.Camera.CameraLookAt;
            game.Camera.position = Vector3.Add(teammate.position, cameraMove);
            game.Camera.CameraLookAt = teammate.position;
        }

        private void SelectInteractiveObject()
        {
            String[] sceneInteractiveObjectsToLoop = { "Doors", "Interactive", "Opponents", "ExitDoors" };

            foreach (String interactiveObjectsToLoop in sceneInteractiveObjectsToLoop) {
                GameObject highlightObject = LoopObjectsAndHighlightObjectUnderMouse(
                                                 game.Scene.children[interactiveObjectsToLoop].children
                                             );

                if (highlightObject != null) {
                    DrawStringCloseToMouse(highlightObject.name);
                    OnMouseLeftButtonClick(() => SetObjectToInteractForDrawMenu(highlightObject));
                    return;
                }
            }
        }

        private GameObject LoopObjectsAndHighlightObjectUnderMouse(Dictionary<String, IGameObject> gameObjects)
        {
            foreach (GameObject interactive in gameObjects.Values) {
                if (interactive.IsMouseOverObject()) {
                    interactive.Hover = true;
                    return interactive;
                }

                interactive.Hover = false;
            }

            return null;
        }

        private void SetObjectToInteractForDrawMenu(GameObject gameObject)
        {
            if (selectedTeammate != null) {
                objectToInteract = gameObject;
            }
        }

        private void DrawMenuToInteractWithObject()
        {
            if (objectToInteract != null) {
                Rectangle rec = new Rectangle(
                                    game.GraphicsDevice.Viewport.Width / 2,
                                    game.GraphicsDevice.Viewport.Height / 2,
                                    game.Textures["frame"].Width,
                                    game.Textures["frame"].Height
                                );
                game.spriteBatch.Draw(game.Textures["frame"], rec, Color.White);

                Vector2 pos = new Vector2(rec.X + 18, rec.Y + 13);
                game.spriteBatch.DrawString(
                    game.Fonts["Open Sans"],
                    "Interact with " + objectToInteract.name,
                    pos,
                    Color.DarkOrange
                );
            }
        }

        private void DrawMessage(GameTime gameTime)
        {
            SetMessageStartTime(gameTime);

            if (message != "" && messageStart + messageDelay > gameTime.TotalGameTime) {
                Vector2 pos = DrawBackgroundForMessage();
                game.spriteBatch.DrawString(game.Fonts["Open Sans"], message, pos, Color.White);
            } else {
                message = "";
                messageStart = TimeSpan.Zero;
            }
        }

        private void SetMessageStartTime(GameTime gameTime)
        {
            if (message != "" && messageStart == TimeSpan.Zero) {
                messageStart = gameTime.TotalGameTime;
            }
        }

        private Vector2 DrawBackgroundForMessage()
        {
            Vector2 messageSize = game.Fonts["Open Sans"].MeasureString(message);
            const int padding = 15;
            int x = (game.GraphicsDevice.Viewport.Width / 2) - (int) (messageSize.X / 2) - padding;
            const int y = 50;
            Rectangle rec = new Rectangle(
                                x,
                                y,
                                (int) messageSize.X + padding * 2,
                                (int) messageSize.Y + padding * 2
                            );
            game.spriteBatch.Draw(game.Textures["message_background"], rec, Color.White);

            return new Vector2(x + 15, y + 15);
        }

        private void OnMouseLeftButtonClick(Action action)
        {
            game.InputState.Mouse.OnMouseLeftButtonClick(action);
        }

        private void DrawStringCloseToMouse(String text)
        {
            Vector2 pos = new Vector2(
                              game.InputState.Mouse.GetMouseLocation().X + 20,
                              game.InputState.Mouse.GetMouseLocation().Y + 20
                          );

            game.spriteBatch.DrawString(game.Fonts["Open Sans"], text, pos, Color.DarkOrange);
        }
    }
}
