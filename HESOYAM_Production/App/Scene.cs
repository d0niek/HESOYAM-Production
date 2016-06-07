using Microsoft.Xna.Framework;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using HESOYAM_Production;
using System;
using App.Collisions;
using System.Collections.Generic;

namespace App
{

    public class Scene: GameObject
    {
        private Dictionary<String, Model> models;
        private Dictionary<String, Texture2D> textures;
        private const float WallShift = 100;

        public GameObject Player { get; private set; }

        public Scene(
            Engine game,
            string name,
            string bitmapPath,
            Dictionary<String, Model> models,
            Dictionary<String, Texture2D> textures
        ) : base(game, name)
        {
            this.models = models;
            this.textures = textures;

            Bitmap bmp = (Bitmap) Image.FromFile(bitmapPath + ".bmp");

            this.buildMap(bmp);
            this.drawFloor(bmp.Width, bmp.Height);

            this.AddChildrenToGame(true, true);
        }

        private void buildMap(Bitmap bmp)
        {
            this.AddChild(new GameObject(game, "Characters"));
            this.children["Characters"].AddChild(new GameObject(game, "Player"));
            this.children["Characters"].AddChild(new GameObject(game, "Teammate"));
            this.children["Characters"].AddChild(new GameObject(game, "Opponents"));
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
                this.buildOther(this.models["lozko"], pos, (int) color.B);
            } else if (color.R == 100 && color.G == 50) {
                this.buildOther(this.models["lampa"], pos, (int) color.B);
            } else if (color.R == 185 && color.G == 67) {
                this.buildOther(this.models["szafka"], pos, (int) color.B);
            } else if (color.R == 185 && color.G == 163) {
                this.buildOther(this.models["biurko"], pos, (int) color.B);
            } else if (color.R == 46 && color.G == 163) {
                this.buildOther(this.models["krzeslo"], pos, (int) color.B);
            } else if (color.R == 250 && color.G == 250) {
                this.insertPlayerCharacter(pos, (int) color.B);
            } else if (color.R == 250 && color.G == 200) {
                this.insertTeammateCharacter(this.models["chudzielec"], pos, (int) color.B, "chudzielec");
            } else if (color.R == 200 && color.G == 250) {
                this.insertTeammateCharacter(this.models["grubas"], pos, (int) color.B, "grubas");
            } else if (color.R == 200 && color.G == 200) {
                this.insertTeammateCharacter(this.models["miesniak"], pos, (int) color.B, "miesniak");
            } else if (color.R == 75 && color.G == 25) {
                this.insertOpponentsCharacter(this.models["zolnierz"], pos, (int) color.B, "zolnierz");
            } else if (color.R == 25 && color.G == 57) {
                this.insertOpponentsCharacter(this.models["lekarz"], pos, (int) color.B, "lekarz");
            }
        }

        private void buildWall(Vector2 pos)
        {
            Wall wall = new Wall(
                            game,
                            "Wall_" + pos.X + "x" + pos.Y,
                            this.models["sciana"],
                            this.models["modul_przyciete"],
                            new Vector3(pos.X * WallShift, 0f, pos.Y * WallShift)
                        );
            wall.setTextureNormal(this.textures["sciana_tekstura"]);
            wall.setTextureCut(this.textures["modul_przyciete_tekstura"]);

            this.addColider(wall);

            this.children["Walls"].AddChild(wall);
        }

        private void buildWindow(Vector2 pos, int rotationY)
        {
            Wall window = new Wall(
                              game,
                              "Window_" + pos.X + "x" + pos.Y,
                              this.models["okno"],
                              this.models["modul_przyciete"],
                              new Vector3(pos.X * WallShift, 0f, pos.Y * WallShift),
                              new Vector3(0f, (float) (rotationY * Math.PI / 2), 0f)
                          );
            window.setTextureNormal(this.textures["okno_tekstura"]);
            window.setTextureCut(this.textures["modul_przyciete_tekstura"]);

            this.addColider(window);

            this.children["Windows"].AddChild(window);
        }

        private void buildDoor(Vector2 pos, int rotationY)
        {
            Wall door = new Wall(
                            game,
                            "Door_" + pos.X + "x" + pos.Y,
                            this.models["drzwi"],
                            this.models["drzwi_przyciete"],
                            new Vector3(pos.X * WallShift, 0f, pos.Y * WallShift),
                            new Vector3(0f, (float) (rotationY * Math.PI / 2), 0f)
                        );
            door.setTextureNormal(this.textures["drzwi_tekstura"]);
            door.setTextureCut(this.textures["modul_przyciete_tekstura"]);

            this.children["Doors"].AddChild(door);
        }

        private void buildExit(Vector2 pos, int rotationY)
        {
            Wall exit = new Wall(
                            game,
                            "Door_" + pos.X + "x" + pos.Y,
                            this.models["drzwi_duze"],
                            this.models["drzwi_duze"],
                            new Vector3(pos.X * WallShift, 0f, pos.Y * WallShift),
                            new Vector3(0f, (float) (rotationY * Math.PI / 2), 0f)
                        );
            exit.setTextureNormal(this.textures["drzwi_duze_tekstura"]);
            exit.setTextureCut(this.textures["drzwi_duze_tekstura"]);

            this.children["Doors"].AddChild(exit);
        }

        private void buildOther(Model model, Vector2 pos, int rotationY)
        {
            GameObject other = this.buildObject(model, pos, "Other_", rotationY);

            this.children["Others"].AddChild(other);
        }

        private void insertPlayerCharacter(Vector2 pos, int rotationY)
        {
            this.Player = this.buildObject(this.models["bohater"], pos, "Player_", rotationY);
            this.Player.setTexture(this.textures["bohater"]);

            this.children["Characters"].children["Player"].AddChild(this.Player);
        }

        private void insertTeammateCharacter(Model model, Vector2 pos, int rotationY, string texture)
        {
            GameObject character = this.buildObject(model, pos, "Team_", rotationY);
            character.setTexture(this.textures[texture]);

            this.children["Characters"].children["Teammate"].AddChild(character);
        }

        private void insertOpponentsCharacter(Model model, Vector2 pos, int rotationY, string texture)
        {
            GameObject character = this.buildObject(model, pos, "Opponent_", rotationY);
            character.setTexture(this.textures[texture]);

            this.children["Characters"].children["Opponents"].AddChild(character);
        }

        private GameObject buildObject(Model model, Vector2 pos, string prefix, int rotationY)
        {
            GameObject gameObject = new GameObject(
                                        game,
                                        prefix + pos.X + "x" + pos.Y,
                                        model,
                                        new Vector3(pos.X * WallShift, 0f, pos.Y * WallShift),
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

            float positionX = Width * WallShift / 2 - modelWidth;
            float positionZ = Height * WallShift / 2 - modelWidth;

            GameObject floor = new GameObject(
                                   this.game,
                                   "Floor",
                                   this.models["sciana"],
                                   new Vector3(positionX, 0f, positionZ),
                                   Vector3.Zero,
                                   new Vector3((float) Width, 0f, (float) Height)
                               );

            this.AddChild(floor);
        }
    }
}
