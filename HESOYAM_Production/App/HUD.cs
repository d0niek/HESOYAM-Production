using HESOYAM_Production;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using App.Models;
using Microsoft.Xna.Framework.Input;
using HESOYAM_Production.App;

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
        GameObject menuForObject;
        Rectangle menuFramePos;
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
            DrawAvatars();
            DrawFotterBar();
            DrawPlayPauseButton();

            if (!game.PlayMode) {
                SelectTeammate();
            }

            if(objectToInteract == null) SelectInteractiveObject();
            DrawMenuToInteractWithObject();

            DrawMessage(gameTime);
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

            GameObject highlightObject = null;

            foreach (String interactiveObjectsToLoop in sceneInteractiveObjectsToLoop) {
                highlightObject = LoopObjectsAndHighlightObjectUnderMouse(
                                                 game.Scene.children[interactiveObjectsToLoop].children
                                             );
                if(highlightObject != null)
                {
                    break;
                }
            }
            if(highlightObject == null)
            {
                highlightObject = new DefaultInteractive(game, FindWhereClicked());
            }
            if(highlightObject != null)
            {
                DrawStringCloseToMouse(highlightObject.name);
                OnMouseLeftButtonClick(() => SetObjectToInteractForDrawMenu(highlightObject));
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

        public Vector3 FindWhereClicked()
        {
            MouseState ms = this.game.InputState.Mouse.CurrentMouseState;
            Vector3 nearScreenPoint = new Vector3(ms.X, ms.Y, 0);
            Vector3 farScreenPoint = new Vector3(ms.X, ms.Y, 1);
            Vector3 nearWorldPoint = game.GraphicsDevice.Viewport.Unproject(nearScreenPoint, game.Camera.ProjectionMatrix, game.Camera.ViewMatrix, Matrix.Identity);
            Vector3 farWorldPoint = game.GraphicsDevice.Viewport.Unproject(farScreenPoint, game.Camera.ProjectionMatrix, game.Camera.ViewMatrix, Matrix.Identity);

            Vector3 direction = farWorldPoint - nearWorldPoint;

            float zFactor = -nearWorldPoint.Y / direction.Y;
            Vector3 zeroWorldPoint = nearWorldPoint + direction * zFactor;

            return zeroWorldPoint;
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
                Rectangle framePosition = DrawMenuFrameWithTitle();
                DrawOptionsInMenu(framePosition);

                game.TimeToInteract = true;
            } else {
                menuFramePos = Rectangle.Empty;
            }
        }

        private Rectangle DrawMenuFrameWithTitle()
        {
            Rectangle menuPosition = GetMenuPosition();
            game.spriteBatch.Draw(game.Textures["frame"], menuPosition, Color.White);

            DrawFrameTitle(menuPosition, "Interact with " + objectToInteract.name.Split('_')[0]);

            return menuPosition;
        }

        private Rectangle GetMenuPosition()
        {
            if (menuFramePos == Rectangle.Empty || objectToInteract != menuForObject) {
                menuFramePos = new Rectangle(
                    game.InputState.Mouse.GetMouseLocation().X + 20,
                    game.InputState.Mouse.GetMouseLocation().Y + 20,
                    game.Textures["frame"].Width,
                    game.Textures["frame"].Height
                );

                menuForObject = objectToInteract;
            }

            return menuFramePos;
        }

        private void DrawFrameTitle(Rectangle framePosition, String title)
        {
            Vector2 pos = new Vector2(framePosition.X + 18, framePosition.Y + 13);
            game.spriteBatch.DrawString(
                game.Fonts["Open Sans"],
                title,
                pos,
                Color.DarkOrange
            );
        }

        private void DrawOptionsInMenu(Rectangle menuFramePosition)
        {
            IInteractiveObject interactiveObject = (IInteractiveObject) objectToInteract;
            List<String> options = interactiveObject.GetOptionsToInteract();
            options.Add("Cancel");
            const int shift = 25;
            int loop = 1;

            foreach (String option in options) {
                Vector2 pos = new Vector2(menuFramePosition.X + 25, menuFramePosition.Y + 13 + (loop * shift));
                DrawBackgroundUnderMenuOption(pos, option);
                game.spriteBatch.DrawString(
                    game.Fonts["Open Sans"],
                    option,
                    pos,
                    Color.DarkOrange
                );

                loop++;
            }
        }

        private void DrawBackgroundUnderMenuOption(Vector2 optionPosition, String option)
        {
            Rectangle rec = new  Rectangle(
                                (int) optionPosition.X,
                                (int) optionPosition.Y,
                                200,
                                20
                            );

            if (rec.Contains(game.InputState.Mouse.GetMouseLocation())) {
                game.spriteBatch.Draw(game.Textures["option_background"], rec, Color.White);
                OnMouseLeftButtonClick(() => ClickOnMenuOption(option));
            }
        }

        private void ClickOnMenuOption(String option)
        {
            if(!option.Equals("Cancel"))
                (selectedTeammate as Teammate).onMoveToCommand(objectToInteract, option);
            objectToInteract = null;
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
