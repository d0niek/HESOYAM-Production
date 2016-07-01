using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using HESOYAM_Production;
using App.Collisions;
using App.Animation;
using System.Collections.Generic;
using App.Models;


namespace App
{

    public class Player : Character, IInteractiveObject
    {
        float cameraAngle;
        bool isAttacking;
        List<string> bag;
        bool isHavingGun;
        bool isPlayerInteracting;
        private TimeSpan lastAttack;
        private TimeSpan attackDelay;
        protected TimeSpan lastShoot;
        protected TimeSpan shootDelay;
        Door doorToOpen;
        private float shootHeight = 120f;

        public Player(
            Engine game,
            string name,
            float cameraAngle,
            Vector3 position = default(Vector3),
            Vector3 rotation = default(Vector3)
        ) : base(game, name, position, rotation)
        {
            this.cameraAngle = cameraAngle;
            isAttacking = false;
            bag = new List<string>();
            isPlayerInteracting = false;
            isHavingGun = false;

            Vector3 newPosition = position;
            Vector3 newSize = new Vector3(35f, 190f, 35f);
            newPosition.Y += 95f;

            AddCollider("hitbox", new Collider(game, newPosition, newSize, Vector3.Zero));

            newSize = new Vector3(100f, 190f, 100f);
            AddCollider("main", new Collider(game, newPosition, newSize, Vector3.Zero));

            newPosition = position;
            newSize = new Vector3(5.0f, 10.0f, 40.0f);

            newPosition.X += 45;
            AddCollider("front", new Collider(game, newPosition, newSize, Vector3.Zero));

            newPosition.X -= 90;
            AddCollider("back", new Collider(game, newPosition, newSize, Vector3.Zero));

            newSize = new Vector3(40.0f, 10.0f, 5.0f);

            newPosition.X += 45;
            newPosition.Z += 45;
            AddCollider("right", new Collider(game, newPosition, newSize, Vector3.Zero));

            newPosition.Z -= 90;
            AddCollider("left", new Collider(game, newPosition, newSize, Vector3.Zero));

            AddCollidersToGame();

            lastAttack = TimeSpan.Zero;
            attackDelay = new TimeSpan(0, 0, 0, 0, 870);
            lastShoot = TimeSpan.Zero;
            shootDelay = new TimeSpan(0, 0, 1);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);


            if(IsDead())
            {
                OnDead();
                return;
            }
            if(isAttacking)
            {
                OnAttack();
                return;
            }

            foreach(IGameObject door in game.Scene.children["Doors"].children.Values)
            {
                if(this.colliders["main"].CollidesWith(door.colliders["main"]))
                {
                    if(((Door)(door)).IsMouseOverObject())
                    {
                        OnMouseLeftButtonPressed(() => OpenDoor(((Door)(door))));
                    }
                }

            }

            MouseState mouseState = new MouseState();
            if(game.Player.bag.Contains("weapon"))
            {
                if (game.InputState.IsNewRightMouseClick(out mouseState))
                {
                    game.Player.isHavingGun = !game.Player.isHavingGun;
                }
            }
            

            if(isPlayerInteracting)
            {
                OnOpenDoor();
                return;
            }

            if(!game.PlayMode)
            {
                return;
            }

            float angle = GetAngleFromMouse();
            Rotate(0, angle, 0);

            Vector3 vector = MovePlayer();

            ResetCollidersColor();
            vector = CheckCollisionsWithSceneObjects(vector);
            vector = CheckCollisionWithOpponents(vector, gameTime);

            Move(vector.X, 0, vector.Z);
        }

        private float GetAngleFromMouse()
        {
            float angle = rotation.Y;

            if(game.InputState.Mouse.isInGameWindow())
            {
                Vector2 mousePos = new Vector2(
                                       game.InputState.Mouse.CurrentMouseState.X,
                                       game.InputState.Mouse.CurrentMouseState.Y
                                   );

                mousePos.X -= Game.GraphicsDevice.Viewport.Width / 2;
                mousePos.Y -= Game.GraphicsDevice.Viewport.Height / 2;

                angle = (float)(Math.Atan2(mousePos.X, mousePos.Y)) + cameraAngle;
            }

            return angle;
        }

        private Vector3 MovePlayer()
        {
            Vector3 vector = ReadInputAndMovePlayer();

            if(this.IsDead())
            {
                this.OnDead();
            }
            else {
                if(vector != Vector3.Zero)
                {
                    OnMove(vector);
                }
                else {
                    OnIdle();
                }
            }

            Matrix rotationMatrixY = Matrix.CreateRotationY(rotation.Y + cameraAngle);
            vector = Vector3.Transform(vector, rotationMatrixY);

            vector = FixSpeedOfMovingDiagonally(vector);
            vector = Vector3.Multiply(vector, isHavingGun ? 0.6f : 1.0f);
            return vector;
        }

        private Vector3 ReadInputAndMovePlayer()
        {
            PlayerIndex playerIndex;
            Vector3 vector = Vector3.Zero;

            if(game.InputState.IsKeyPressed(Keys.W, PlayerIndex.One, out playerIndex))
            {
                vector.Z -= 10;
            }

            if(game.InputState.IsKeyPressed(Keys.S, PlayerIndex.One, out playerIndex))
            {
                vector.Z += 10;
            }

            if(game.InputState.IsKeyPressed(Keys.A, PlayerIndex.One, out playerIndex))
            {
                vector.X -= 10;
            }

            if(game.InputState.IsKeyPressed(Keys.D, PlayerIndex.One, out playerIndex))
            {
                vector.X += 10;
            }

            return vector;
        }

        protected void OnMove(Vector3 moveVector)
        {
            AnimatedObject playerModel = (AnimatedObject)children["playerModel"];
            float rotationY = (float)Math.Atan2(moveVector.X, moveVector.Z);
            rotationY = (float)((GetAngleFromMouse() - rotationY) % (Math.PI * 2) + (Math.PI / 4));

            if(rotationY < 0)
            {
                rotationY += (float)(Math.PI * 2);
            }

            if(!isHavingGun)
            {
                if(rotationY >= (1.75 * Math.PI) || rotationY < Math.PI / 4)
                {
                    playerModel.PlayClip("bieg_przod").Looping = true;
                }
                else if(rotationY <= (1.75 * Math.PI) && rotationY >= (1.25 * Math.PI))
                {
                    playerModel.PlayClip("bieg_lewo").Looping = true;
                }
                else if(rotationY <= (0.75 * Math.PI) && rotationY > Math.PI / 4)
                {
                    playerModel.PlayClip("bieg_prawo").Looping = true;
                }
                else
                {
                    playerModel.PlayClip("bieg_tyl").Looping = true;
                }
            }
            else
            {
                if(rotationY >= (1.75 * Math.PI) || rotationY < Math.PI / 4)
                {
                    playerModel.PlayClip("bron_chod_przod").Looping = true;
                }
                else if(rotationY <= (1.75 * Math.PI) && rotationY >= (1.25 * Math.PI))
                {
                    playerModel.PlayClip("bron_chod_lewo").Looping = true;
                }
                else if(rotationY <= (0.75 * Math.PI) && rotationY > Math.PI / 4)
                {
                    playerModel.PlayClip("bron_chod_prawo").Looping = true;
                }
                else
                {
                    playerModel.PlayClip("bron_chod_tyl").Looping = true;
                }
            }

        }

        new protected void OnIdle()
        {
            if(!isHavingGun)
            {
                AnimatedObject playerModel = (AnimatedObject)children["playerModel"];
                playerModel.PlayClip("postawa").Looping = true;
            }
            else
            {
                AnimatedObject playerModel = (AnimatedObject)children["playerModel"];
                playerModel.PlayClip("celowanie").Looping = true;
            }

            
        }

        new protected void OnDead()
        {
            AnimatedObject playerModel = (AnimatedObject)children["playerModel"];
            playerModel.PlayClip("smierc").Looping = false;
        }

        new protected void OnAttack()
        {
            if(!isAttacking)
                isAttacking = true;
            AnimatedObject playerModel = (AnimatedObject)children["playerModel"];
            if(!isHavingGun)
            {
                AnimationPlayer player = playerModel.PlayClip("cios_piesc");
                player.Looping = false;
                if(player.Position >= player.Duration)
                    isAttacking = false;
            }
            else
            {
                AnimationPlayer player = playerModel.PlayClip("bron_piesc");
                player.Looping = false;
            }

        }

        protected void OnOpenDoor()
        {
            AnimatedObject playerModel = (AnimatedObject)children["playerModel"];
            AnimationPlayer player = playerModel.PlayClip("interakcja");
            player.Looping = false;
            if (player.Position >= player.Duration / 2.0f)
            {
                isPlayerInteracting = false;
                doorToOpen.TryToOpenDoor();
            }
        }

        private Vector3 FixSpeedOfMovingDiagonally(Vector3 vector)
        {
            if(vector.X == 0f)
            {
                vector.Z /= (float)Math.Sqrt(2);
            }
            else if(vector.Z == 0f)
            {
                vector.X /= (float)Math.Sqrt(2);
            }
            return vector;
        }

        private void ResetCollidersColor()
        {
            foreach(Collider collider in colliders.Values)
            {
                collider.drawColor = Color.GreenYellow;
            }
        }

        private Vector3 CheckCollisionsWithSceneObjects(Vector3 vector)
        {
            String[] objectsListInTheScene = { "Walls", "Interactive", "Windows", "Others", "Teammates" };

            foreach(String objectsList in objectsListInTheScene)
            {
                vector = CheckCollisionsWithObjects(objectsList, vector);
            }

            vector = CheckCollisionWithDoors(vector);

            return vector;
        }

        private Vector3 CheckCollisionsWithObjects(String objectsList, Vector3 vector)
        {
            foreach(IGameObject objectOnScene in game.Scene.children[objectsList].children.Values)
            {
                foreach(Collider collider in objectOnScene.colliders.Values)
                {
                    vector = CheckSensors(collider, vector);
                }
            }

            return vector;
        }

        private Vector3 CheckCollisionWithDoors(Vector3 vector)
        {
            String[] doors = { "Doors", "ExitDoors" };

            foreach(String door in doors)
            {
                foreach(Door d in game.Scene.children[door].children.Values)
                {
                    if(!d.IsOpen)
                    {
                        foreach(Collider collider in d.colliders.Values)
                        {
                            vector = CheckSensors(collider, vector);
                        }
                    }
                }
            }

            return vector;
        }

        private Vector3 CheckCollisionWithOpponents(Vector3 vector, GameTime gameTime)
        {
            //List<String> opponentsToRemove = new List<String>();
            foreach(Opponent opponent in game.Scene.children["Opponents"].children.Values)
            {
                if(opponent.colliders.ContainsKey("main"))
                    vector = CheckSensors(opponent.colliders["main"], vector);

                if(!isHavingGun)
                {
                    if(IsCollisionWithOpponent(opponent) && opponent.IsMouseOverObject())
                    {
                        OnMouseLeftButtonPressed(() => AttackOpponent(opponent, gameTime));

                    }
                }
                else OnMouseLeftButtonPressed(() => AttackOpponent(opponent, gameTime));



            }

            //RemoveOpponentsFromScene(opponentsToRemove);

            return vector;
        }


        private Vector3 CheckSensors(Collider collider, Vector3 vector)
        {
            if(colliders["right"].CollidesWith(collider))
            {
                colliders["right"].drawColor = Color.OrangeRed;
                vector.Z = (vector.Z > 0 ? 0 : vector.Z);
            }

            if(colliders["left"].CollidesWith(collider))
            {
                colliders["left"].drawColor = Color.OrangeRed;
                vector.Z = (vector.Z < 0 ? 0 : vector.Z);
            }

            if(colliders["front"].CollidesWith(collider))
            {
                colliders["front"].drawColor = Color.OrangeRed;
                vector.X = (vector.X > 0 ? 0 : vector.X);
            }

            if(colliders["back"].CollidesWith(collider))
            {
                colliders["back"].drawColor = Color.OrangeRed;
                vector.X = (vector.X < 0 ? 0 : vector.X);
            }

            return vector;
        }

        private bool IsCollisionWithOpponent(Opponent opponent)
        {
            if(opponent.colliders.ContainsKey("main") && game.Scene.Player.colliders.ContainsKey("main"))
            {
                return game.Scene.Player.colliders["main"].CollidesWith(opponent.colliders["main"]);
            }
            else
                return false;
        }

        private bool IsCollisionWithDoors(Door door)
        {
            if(door.colliders.ContainsKey("main") && game.Scene.Player.colliders.ContainsKey("main"))
            {
                return game.Scene.Player.colliders["main"].CollidesWith(door.colliders["main"]);
            }
            else
                return false;
        }

        private void shoot(Vector3 direction, TimeSpan time)
        {
            if(lastShoot + shootDelay < time)
            {
                lastShoot = time;
                new Projectile(game, new Vector3(position.X, position.Y + shootHeight, position.Z), direction, 15f, true);
            }
        }

        private void AttackOpponent(Opponent opponent, GameTime gameTime)
        {
            if(!isHavingGun)
            {
                rotateInDirection(Vector3.Subtract(opponent.position, this.position), false);
                if(lastAttack + attackDelay < gameTime.TotalGameTime)
                {
                    OnAttack();
                    opponent.ReduceLife(34f);
                    lastAttack = gameTime.TotalGameTime;
                }
            }
            else
            {
                Vector3 cursorPosition = game.Hud.FindWhereClicked(shootHeight);
                Vector3 opponentDelta = Vector3.Subtract(cursorPosition, position);
                opponentDelta.Normalize();
                shoot(opponentDelta, gameTime.TotalGameTime);

            }


        }

        private void OpenDoor(Door door)
        {
            doorToOpen = door;
            rotateInDirection(Vector3.Subtract(doorToOpen.Position, this.position), false);
            isPlayerInteracting = true;
        }

        private void RemoveOpponentsFromScene(List<string> opponentsToRemove)
        {
            foreach(String opponentToRemove in opponentsToRemove)
            {
                IGameComponent opponent = game.Scene.children["Opponents"].RemoveChild(opponentToRemove) as IGameComponent;
                game.Components.Remove(opponent);
            }
        }

        public new void Move(float x, float y, float z)
        {
            Vector3 delta = new Vector3(x, y, z);
            position = Vector3.Add(delta, position);

            foreach(IGameElement child in children.Values)
            {
                child.Move(x, y, z);
            }

            foreach(Collider collider in colliders.Values)
            {
                collider.Move(x, y, z);
            }
        }

        public new void Rotate(float x, float y, float z)
        {
            foreach(IGameElement child in children.Values)
            {
                if(!(child is Camera))
                {
                    child.SetRotation(x, y, z);
                }
            }
        }

        protected new void rotateInDirection(Vector3 direction, bool lerp)
        {
            float rotationY;
            if(lerp)
                rotationY = MathHelper.Lerp(this.rotation.Y, (float)Math.Atan2(direction.X, direction.Z), 0.1f);
            else
                rotationY = (float)Math.Atan2(direction.X, direction.Z);

            if(Math.Abs(this.rotation.Y - rotationY) > 0.01f)
            {
                this.Rotate(0, rotationY, 0);
            }
        }

        public void addItemToBag(String item)
        {
            bag.Add(item);
            game.Hud.Message = "You have obtained a \"" + item + "\"";
        }

        public bool hasItemInBag(String item)
        {
            return bag.Contains(item);
        }

        public void checkIfFirstAidKit()
        {
            if(hasItemInBag("first aid kit"))
            {
                IncreaseLife(50f);
                bag.Remove("first aid kit");
            }
        }

        public List<string> GetOptionsToInteract()
        {
            List<string> options = new List<string>();
            options.Add("Give items");
            options.Add("Follow");
            return options;
        }

        public string performAction(string action)
        {
            if(action != null)
            {
                if(action.Equals("Give items"))
                    return "Accepted";
                else if(action.Equals("Follow"))
                    return "Chase";
            }
            return null;
        }
    }
}
