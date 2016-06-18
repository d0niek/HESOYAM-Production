using Microsoft.Xna.Framework;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using HESOYAM_Production;
using System;
using App.Collisions;
using App.Models;
using App.Animation;
using System.Collections.Generic;

namespace App
{

    public class Scene: GameObject
    {
        const float wallShift = 100;
        AnimatedObject player;
        public Movement movement;

        public AnimatedObject Player {
            get { return player; } 
            private set { }
        }

        public Scene(
            Engine game,
            string name,
            string bitmapPath
        ) : base(game, name)
        {
            Bitmap bmp = (Bitmap) Image.FromFile(bitmapPath + ".bmp");

            buildMap(bmp);
            drawFloor(bmp.Width, bmp.Height);

            AddChildrenToGame(true, true);
        }

        private void buildMap(Bitmap bmp)
        {
            AddChild(new GameObject(game, "Player"));
            AddChild(new GameObject(game, "Teammates"));
            AddChild(new GameObject(game, "Opponents"));
            AddChild(new GameObject(game, "Walls"));
            AddChild(new GameObject(game, "Windows"));
            AddChild(new GameObject(game, "Doors"));
            AddChild(new GameObject(game, "ExitDoors"));
            AddChild(new GameObject(game, "Interactive"));
            AddChild(new GameObject(game, "Others"));

            movement = new Movement(bmp.Width, bmp.Height, wallShift);

            for (int i = 0; i < bmp.Width; i++) {
                for (int j = 0; j < bmp.Height; j++) {
                    System.Drawing.Color color = bmp.GetPixel(i, j);

                    buildMapObject(color, new Vector2(i, j));
                }
            }
        }

        private void buildMapObject(System.Drawing.Color color, Vector2 pos)
        {
            if (color.R == 0 && color.G == 0) {
                buildWall(pos);
            } else if (color.R == 255 && color.G == 0) {
                buildWindow(pos, (int) color.B);
            } else if (color.R == 0 && color.G == 255) {
                buildDoor(pos, (int) color.B, false);
            } else if (color.R == 1 && color.G == 255) {
                buildDoor(pos, (int) color.B, true);
            } else if (color.R == 164 && color.G == 255) {
                buildExit(pos, (int) color.B);
            } else if (color.R == 100 && color.G == 100) {
                buildOther(game.Models["lozko"], pos, (int) color.B);
            } else if (color.R == 100 && color.G == 50) {
                buildOther(game.Models["lampa"], pos, (int) color.B);
            } else if (color.R == 185 && color.G == 61) {
                buildCupboard(pos, (int) color.B, "key");
            } else if (color.R == 185 && color.G == 99) {
                buildCupboard(pos, (int) color.B, "");
            } else if (color.R == 185 && color.G == 163) {
                buildOther(game.Models["biurko"], pos, (int) color.B);
            } else if (color.R == 46 && color.G == 163) {
                buildOther(game.Models["krzeslo"], pos, (int) color.B);
            } else if (color.R == 250 && color.G == 250) {
                insertPlayerCharacter(pos, (int) color.B);
            } else if (color.R == 250 && color.G == 200) {
                insertTeammateCharacter(game.Models["chudzielec"], pos, (int) color.B, "chudzielec");
            } else if (color.R == 200 && color.G == 250) {
                insertTeammateCharacter(game.Models["grubas"], pos, (int) color.B, "grubas");
            } else if (color.R == 200 && color.G == 200) {
                insertTeammateCharacter(game.Models["miesniak"], pos, (int) color.B, "miesniak");
            } else if (color.R == 75 && color.G == 25) {
                insertOpponentCharacter(game.Models["zolnierz"], pos, (int) color.B, "zolnierz");
            } else if (color.R == 25 && color.G == 57) {
                insertOpponentCharacter(game.Models["lekarz"], pos, (int) color.B, "lekarz");
            }
        }

        private void buildWall(Vector2 pos)
        {
            Wall wall = new Wall(
                            game,
                            "Wall_" + pos.X + "x" + pos.Y,
                            game.Models["sciana"],
                            game.Models["modul_przyciete"],
                            new Vector3(pos.X * wallShift, 0f, pos.Y * wallShift)
                        );
            wall.TextureNormal = game.Textures["sciana_tekstura"];
            wall.TextureCut = game.Textures["modul_przyciete_tekstura"];

            addColider(wall);

            children["Walls"].AddChild(wall);
            movement.addObstacle((int) pos.X, (int) pos.Y);
        }

        private void buildWindow(Vector2 pos, int rotationY)
        {
            Wall window = new Wall(
                              game,
                              "Window_" + pos.X + "x" + pos.Y,
                              game.Models["okno"],
                              game.Models["modul_przyciete"],
                              new Vector3(pos.X * wallShift, 0f, pos.Y * wallShift),
                              new Vector3(0f, (float) (rotationY * Math.PI / 2), 0f)
                          );
            window.TextureNormal = game.Textures["okno_tekstura"];
            window.TextureCut = game.Textures["modul_przyciete_tekstura"];

            addColider(window);

            children["Windows"].AddChild(window);
            movement.addObstacle((int) pos.X, (int) pos.Y);
        }

        private void buildDoor(Vector2 pos, int rotationY, bool isLock)
        {
            Door door = new Door(
                            game,
                            "Door_" + pos.X + "x" + pos.Y,
                            game.Models["drzwi"],
                            game.Models["drzwi_przyciete"],
                            isLock,
                            new Vector3(pos.X * wallShift, 0f, pos.Y * wallShift),
                            new Vector3(0f, (float) (rotationY * Math.PI / 2), 0f)
                        );
            door.TextureNormal = game.Textures["drzwi_tekstura"];
            door.TextureCut = game.Textures["modul_przyciete_tekstura"];

            addColider(door);

            children["Doors"].AddChild(door);
        }

        void buildLamp(Model model, Vector2 pos, int rotationY)
        {
            Lamp door = new Lamp(
                            game,
                            "Door_" + pos.X + "x" + pos.Y,
                            this.game.Models["lampa"],
                            new Vector3(pos.X * wallShift, 0f, pos.Y * wallShift),
                            new Vector3(0f, (float) (rotationY * Math.PI / 2), 0f)
                        );

            this.children["Others"].AddChild(door);
        }

        private void buildExit(Vector2 pos, int rotationY)
        {
            Wall exit = new Wall(
                            game,
                            "ExitDoor_" + pos.X + "x" + pos.Y,
                            game.Models["drzwi_duze"],
                            game.Models["drzwi_duze_przyciete"],
                            new Vector3(pos.X * wallShift, 0f, pos.Y * wallShift),
                            new Vector3(0f, (float) (rotationY * Math.PI / 2), 0f)
                        );
            exit.TextureNormal = game.Textures["drzwi_duze_tekstura"];
            exit.TextureCut = game.Textures["drzwi_duze_tekstura"];

            children["ExitDoors"].AddChild(exit);

        }

        private void buildCupboard(Vector2 pos, int rotationY, String item)
        {
            Cupboard cupboard = new Cupboard(
                                    game,
                                    "Cupboard_" + pos.X + "x" + pos.Y,
                                    game.Models["szafka"],
                                    item,
                                    new Vector3(pos.X * wallShift, 0f, pos.Y * wallShift),
                                    new Vector3(0f, (float) (rotationY * Math.PI / 2), 0f)
                                );

            addColider(cupboard);

            String childrensList = item != "" ? "Interactive" : "Others";

            children[childrensList].AddChild(cupboard);
            movement.addObstacle((int) pos.X, (int) pos.Y);
        }

        private void buildOther(Model model, Vector2 pos, int rotationY)
        {
            GameObject other = buildObject(model, pos, "Other_", rotationY);

            children["Others"].AddChild(other);
        }

        private void insertPlayerCharacter(Vector2 pos, int rotationY)
        {
            player = buildAnimatedObject(game.Models["bohater"], pos, "Player_", rotationY);
            loadAnimationsToCharacter(player, "bohater");
            player.setTexture(game.Textures["bohater"]);
            
            children["Player"].AddChild(player);
        }

        private void insertTeammateCharacter(Model model, Vector2 pos, int rotationY, string texture)
        {
            GameObject teammate = new Teammate(
                                      game,
                                      "Teammate_" + pos.X + "x" + pos.Y,
                                      model,
                                      new Vector3(pos.X * wallShift, 0f, pos.Y * wallShift),
                                      new Vector3(0f, (float) (rotationY * Math.PI / 2), 0f)
                                  );
            teammate.setTexture(game.Textures[texture]);

            addColider(teammate);

            children["Teammates"].AddChild(teammate);
        }

        private void insertOpponentCharacter(Model model, Vector2 pos, int rotationY, string texture)
        {
            Opponent opponent = new Opponent(
                                    game,
                                    "Opponent_" + pos.X + "x" + pos.Y,
                                    model,
                                    new Vector3(pos.X * wallShift, 0f, pos.Y * wallShift),
                                    new Vector3(0f, (float) (rotationY * Math.PI / 2), 0f)
                                );
            opponent.setTexture(game.Textures[texture]);

            addColider(opponent);

            children["Opponents"].AddChild(opponent);
        }

        private void loadAnimationsToCharacter(AnimatedObject character, String name)
        {
            Dictionary<String, Model> models = new Dictionary<string, Model>();
            game.LoadModels("Animation/" + name, models);

            foreach (String modelName in models.Keys) {
                Model model = models[modelName];
                ModelExtra modelExtra = model.Tag as ModelExtra;
                String clipName = modelName.Replace(name + "_", "");

                foreach (AnimationClip clip in modelExtra.Clips) {
                    character.Clips.Add(clipName, clip);
                }
            }

            character.PlayClip("postawa").Looping = true;
        }

        private AnimatedObject buildAnimatedObject(Model model, Vector2 pos, string prefix, int rotationY)
        {
            AnimatedObject gameObject = new AnimatedObject(
                                            game,
                                            prefix + pos.X + "x" + pos.Y,
                                            model,
                                            new Vector3(pos.X * wallShift, 0f, pos.Y * wallShift),
                                            new Vector3(0f, (float) (rotationY * Math.PI / 2), 0f)
                                        );

            addColider(gameObject);

            return gameObject;
        }

        private GameObject buildObject(Model model, Vector2 pos, string prefix, int rotationY)
        {
            GameObject gameObject = new GameObject(
                                        game,
                                        prefix + pos.X + "x" + pos.Y,
                                        model,
                                        new Vector3(pos.X * wallShift, 0f, pos.Y * wallShift),
                                        new Vector3(0f, (float) (rotationY * Math.PI / 2), 0f)
                                    );

            addColider(gameObject);

            return gameObject;
        }

        private InteractiveObject buildInteractiveObject(
            Model model,
            Vector2 pos,
            string prefix,
            string itemName,
            int rotationY
        )
        {
            InteractiveObject gameObject = new InteractiveObject(
                                               game,
                                               prefix + pos.X + "x" + pos.Y,
                                               model,
                                               itemName,
                                               new Vector3(pos.X * wallShift, 0f, pos.Y * wallShift),
                                               new Vector3(0f, (float) (rotationY * Math.PI / 2), 0f)
                                           );

            addColider(gameObject);

            return gameObject;
        }

        private void addColider(GameObject gameObject)
        {
            const float height = 250;

            Vector3 shift = new Vector3(0f, height / 2, 0f);

            gameObject.AddCollider(
                "main",
                new Collider(
                    game,
                    Vector3.Add(gameObject.position, shift),
                    new Vector3(100f, height, 100f),
                    Vector3.Zero
                )
            );
        }

        private void drawFloor(int Width, int Height)
        {
            const int modelWidth = 50;

            float positionX = Width * wallShift / 2 - modelWidth;
            float positionZ = Height * wallShift / 2 - modelWidth;

            GameObject floor = new GameObject(
                                   game,
                                   "Floor",
                                   game.Models["sciana"],
                                   new Vector3(positionX, 0f, positionZ),
                                   Vector3.Zero,
                                   new Vector3((float) Width, 0f, (float) Height)
                               );

            AddChild(floor);
        }
    }
}
