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
        private const float WallShift = 100;

        public Scene(Engine game, string name, string bitmapPath, Dictionary<String, Model> models) : base(game, name)
        {
            this.models = models;

            Bitmap bmp = (Bitmap) Image.FromFile(bitmapPath);

            this.buildMap(bmp);
            this.drawFloor(bmp.Width, bmp.Height);

            this.AddChildrenToGame(true, true);
        }

        private void buildMap(Bitmap bmp)
        {
            this.AddChild(new GameObject(game, "Walls"));
            this.AddChild(new GameObject(game, "Windows"));
            this.AddChild(new GameObject(game, "Doors"));

            for (int i = 0; i < bmp.Width; i++) {
                for (int j = 0; j < bmp.Height; j++) {
                    System.Drawing.Color color = bmp.GetPixel(i, j);

                    this.buildMapObject(color, new Vector2(i, j));
                }
            }
        }

        private void buildMapObject(System.Drawing.Color color, Vector2 pos)
        {
            if (color.R == 0 && color.G == 0 && color.B == 0) {
                this.buildWall(this.models["sciana"], pos);
            } else if (color.R == 255 && color.B == 0) {
                this.buildWindow(this.models["okno"], pos, (int) color.G);
            } else if (color.R == 0 && color.G == 255) {
                this.buildDoor(this.models["drzwi"], pos, (int) color.B);
            }
        }

        private void buildWall(Model model, Vector2 pos)
        {
            GameObject wall = this.buildObject(model, pos, "Wall_");

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

        private GameObject buildObject(Model model, Vector2 pos, string prefix = "Object_", int rotationY = 0)
        {
            GameObject gameObject = new GameObject(
                                        game,
                                        prefix + pos.X + "x" + pos.Y,
                                        model,
                                        new Vector3(pos.X * WallShift, 0f, pos.Y * WallShift),
                                        new Vector3(0f, (float) (rotationY * Math.PI / 2), 0f)
                                    );

            this.addColider(gameObject, model);

            return gameObject;
        }

        private void addColider(GameObject gameObject, Model model)
        {
            if (model != this.models["drzwi"]) {
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
        }

        private void drawFloor(int Width, int Height)
        {
            int modelWidth = 50;

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
