using Microsoft.Xna.Framework;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using HESOYAM_Production;
using System;
using App.Collisions;
using App.Animation;
using System.Collections.Generic;
using System.Linq;

namespace App
{

    public class Scene: GameObject
    {
        const float wallShift = 100;
        AnimatedObject player;

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

            this.buildMap(bmp);
            this.drawFloor(bmp.Width, bmp.Height);

            this.AddChildrenToGame(true, true);
        }

        private void buildMap(Bitmap bmp)
        {
            this.AddChild(new GameObject(game, "Characters"));
            this.AddChild(new GameObject(game, "Walls"));
            this.AddChild(new GameObject(game, "Windows"));
            this.AddChild(new GameObject(game, "Doors"));
            this.AddChild(new GameObject(game, "Others"));

            for (int i = 0; i < bmp.Width; i++) {
                for (int j = 0; j < bmp.Height; j++) {
                    System.Drawing.Color color = bmp.GetPixel(i, j);

                    this.buildMapObject(color, new Vector2(i, j));
                }
            }
        }

        private void buildMapObject(System.Drawing.Color color, Vector2 pos)
        {
            if (color.R == 0 && color.G == 0) {
                this.buildWall(pos);
            } else if (color.R == 255 && color.G == 0) {
                this.buildWindow(pos, (int) color.B);
            } else if (color.R == 0 && color.G == 255) {
                this.buildDoor(pos, (int) color.B);
            } else if (color.R == 164 && color.G == 255) {
                this.buildExit(pos, (int) color.B);
            } else if (color.R == 100 && color.G == 100) {
                this.buildOther(this.game.Models["lozko"], pos, (int) color.B);
            } else if (color.R == 100 && color.G == 50) {
                this.buildOther(this.game.Models["lampa"], pos, (int) color.B);
            } else if (color.R == 185 && color.G == 67) {
                this.buildOther(this.game.Models["szafka"], pos, (int) color.B);
            } else if (color.R == 185 && color.G == 163) {
                this.buildOther(this.game.Models["biurko"], pos, (int) color.B);
            } else if (color.R == 46 && color.G == 163) {
                this.buildOther(this.game.Models["krzeslo"], pos, (int) color.B);
            } else if (color.R == 250 && color.G == 250) {
                this.insertMainCharacter(pos, (int) color.B);
            } else if (color.R == 250 && color.G == 200) {
                this.insertCharacter(this.game.Models["chudzielec"], pos, (int) color.B, "chudzielec");
            } else if (color.R == 200 && color.G == 250) {
                this.insertCharacter(this.game.Models["grubas"], pos, (int) color.B, "grubas");
            } else if (color.R == 200 && color.G == 200) {
                this.insertCharacter(this.game.Models["miesniak"], pos, (int) color.B, "miesniak");
            } else if (color.R == 75 && color.G == 25) {
                this.insertCharacter(this.game.Models["zolnierz"], pos, (int) color.B, "zolnierz");
            } else if (color.R == 25 && color.G == 57) {
                this.insertCharacter(this.game.Models["lekarz"], pos, (int) color.B, "lekarz");
            }
        }

        private void buildWall(Vector2 pos)
        {
            Wall wall = new Wall(
                            game,
                            "Wall_" + pos.X + "x" + pos.Y,
                            this.game.Models["sciana"],
                            this.game.Models["modul_przyciete"],
                            new Vector3(pos.X * wallShift, 0f, pos.Y * wallShift)
                        );
            wall.setTextureNormal(this.game.Textures["sciana_tekstura"]);
            wall.setTextureCut(this.game.Textures["modul_przyciete_tekstura"]);

            this.addColider(wall);

            this.children["Walls"].AddChild(wall);
        }

        private void buildWindow(Vector2 pos, int rotationY)
        {
            Wall window = new Wall(
                              game,
                              "Window_" + pos.X + "x" + pos.Y,
                              this.game.Models["okno"],
                              this.game.Models["modul_przyciete"],
                              new Vector3(pos.X * wallShift, 0f, pos.Y * wallShift),
                              new Vector3(0f, (float) (rotationY * Math.PI / 2), 0f)
                          );
            window.setTextureNormal(this.game.Textures["okno_tekstura"]);
            window.setTextureCut(this.game.Textures["modul_przyciete_tekstura"]);

            this.addColider(window);

            this.children["Windows"].AddChild(window);
        }

        private void buildDoor(Vector2 pos, int rotationY)
        {
            Wall door = new Wall(
                            game,
                            "Door_" + pos.X + "x" + pos.Y,
                            this.game.Models["drzwi"],
                            this.game.Models["drzwi_przyciete"],
                            new Vector3(pos.X * wallShift, 0f, pos.Y * wallShift),
                            new Vector3(0f, (float) (rotationY * Math.PI / 2), 0f)
                        );
            door.setTextureNormal(this.game.Textures["drzwi_tekstura"]);
            door.setTextureCut(this.game.Textures["modul_przyciete_tekstura"]);

            this.children["Doors"].AddChild(door);
        }

        private void buildExit(Vector2 pos, int rotationY)
        {
            Wall exit = new Wall(
                            game,
                            "Door_" + pos.X + "x" + pos.Y,
                            this.game.Models["drzwi_duze"],
                            this.game.Models["drzwi_duze"],
                            new Vector3(pos.X * wallShift, 0f, pos.Y * wallShift),
                            new Vector3(0f, (float) (rotationY * Math.PI / 2), 0f)
                        );
            exit.setTextureNormal(this.game.Textures["drzwi_duze_tekstura"]);
            exit.setTextureCut(this.game.Textures["drzwi_duze_tekstura"]);

            this.children["Doors"].AddChild(exit);
        }

        private void buildOther(Model model, Vector2 pos, int rotationY)
        {
            GameObject other = this.buildObject(model, pos, "Other_", rotationY);

            this.children["Others"].AddChild(other);
        }

        private void insertMainCharacter(Vector2 pos, int rotationY)
        {
            this.player = this.buildAnimatedObject(this.game.Models["bohater"], pos, "Player_", rotationY);
            loadAnimationsToCharacter(this.player, "bohater");
            this.player.setTexture(this.game.Textures["bohater"]);
            this.children["Characters"].AddChild(this.player);
        }

        private void loadAnimationsToCharacter(AnimatedObject character, String name)
        {
            Dictionary<String, Model> models = new Dictionary<string, Model>();
            this.game.LoadModels("Animation/" + name, models);

            foreach (String modelName in models.Keys) {
                Model model = models[modelName];
                ModelExtra modelExtra = model.Tag as ModelExtra;
                String clipName = modelName.Replace(name + "_", "");

                foreach (AnimationClip clip in modelExtra.Clips) {
                    character.ClipsToPlay.Add(clipName, clip);
                }
            }

            character.PlayClip("postawa").Looping = true;

        }

        private void insertCharacter(Model model, Vector2 pos, int rotationY, string texture)
        {
            GameObject character = this.buildObject(model, pos, "Player_", rotationY);
            character.setTexture(this.game.Textures[texture]);

            this.children["Characters"].AddChild(character);
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

            this.addColider(gameObject);

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

            this.addColider(gameObject);

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
                                   this.game,
                                   "Floor",
                                   this.game.Models["sciana"],
                                   new Vector3(positionX, 0f, positionZ),
                                   Vector3.Zero,
                                   new Vector3((float) Width, 0f, (float) Height)
                               );

            this.AddChild(floor);
        }
    }
}
