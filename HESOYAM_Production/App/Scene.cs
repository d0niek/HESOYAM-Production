﻿using Microsoft.Xna.Framework;
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
                this.buildWall(this.models["sciana"], pos);
            } else if (color.R == 255 && color.G == 0) {
                this.buildWindow(this.models["okno"], pos, (int) color.B);
            } else if (color.R == 0 && color.G == 255) {
                this.buildDoor(this.models["drzwi"], pos, (int) color.B);
            } else if (color.R == 100 && color.G == 100) {
                this.buildOther(this.models["lozko"], pos, (int) color.B);
            } else if (color.R == 100 && color.G == 50) {
                this.buildOther(this.models["lampa"], pos, (int) color.B);
            } else if (color.R == 250 && color.G == 250) {
                this.insertMainCharacter(this.models["bohater"], pos, (int) color.B);
            } else if (color.R == 250 && color.G == 200) {
                this.insertCharacter(this.models["chudzielec"], pos, (int) color.B, "chudzielec");
            } else if (color.R == 200 && color.G == 250) {
                this.insertCharacter(this.models["grubas"], pos, (int) color.B, "grubas");
            } else if (color.R == 200 && color.G == 200) {
                this.insertCharacter(this.models["miesniak"], pos, (int) color.B, "miesniak");
            }
        }

        private void buildWall(Model model, Vector2 pos)
        {
            GameObject wall = this.buildObject(model, pos, "Wall_", 0);

            this.children["Walls"].AddChild(wall);
        }

        private void buildWindow(Model model, Vector2 pos, int rotationY)
        {
            GameObject window = this.buildObject(model, pos, "Window_", rotationY);

            this.children["Windows"].AddChild(window);
        }

        private void buildDoor(Model model, Vector2 pos, int rotationY)
        {
            GameObject door = this.buildObject(model, pos, "Door_", rotationY);

            this.children["Doors"].AddChild(door);
        }

        private void buildOther(Model model, Vector2 pos, int rotationY)
        {
            GameObject other = this.buildObject(model, pos, "Other_", rotationY);

            this.children["Others"].AddChild(other);
        }

        private void insertMainCharacter(Model model, Vector2 pos, int rotationY)
        {
            this.Player = this.buildObject(model, pos, "Player_", rotationY);
            this.Player.setTexture(this.textures["bohater"]);

            this.children["Characters"].AddChild(this.Player);
        }

        private void insertCharacter(Model model, Vector2 pos, int rotationY, string texture)
        {
            GameObject character = this.buildObject(model, pos, "Player_", rotationY);
            character.setTexture(this.textures[texture]);

            this.children["Characters"].AddChild(character);
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

            if (model != this.models["drzwi"]) {
                this.addColider(gameObject);
            }

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
