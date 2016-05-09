﻿using Microsoft.Xna.Framework;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using HESOYAM_Production;

namespace App
{

    public class Scene: GameObject
    {
        private const float WallShift = 100;
        private Model wall;
        private Model door;
        private Model window;

        public Scene(
            Engine game,
            string name,
            string bitmapPath,
            Model wall,
            Model door,
            Model window
        ) : base(game, name)
        {
            this.wall = wall;
            this.door = door;
            this.window = window;

            Bitmap bmp = (Bitmap) Image.FromFile(bitmapPath);

            this.buildMap(bmp);
            this.drawFloor(bmp.Width, bmp.Height);

            this.AddChildrenToGame(true);
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
                this.buildWall(this.wall, pos);
            } else if (color.R == 255 && color.G == 0 && color.B == 0) {
                this.buildWindow(this.window, pos);
            } else if (color.R == 0 && color.G == 255 && color.B == 0) {
                this.buildDoor(this.door, pos);
            }
        }

        private void buildWall(Model model, Vector2 pos)
        {
            GameObject wall = this.buildObject(model, pos, "Wall_");

            this.children["Walls"].AddChild(wall);
        }

        private void buildWindow(Model model, Vector2 pos)
        {
            GameObject window = this.buildObject(model, pos, "Window_");

            this.children["Windows"].AddChild(window);
        }

        private void buildDoor(Model model, Vector2 pos)
        {
            GameObject door = this.buildObject(model, pos, "Door_");

            this.children["Doors"].AddChild(door);
        }

        private GameObject buildObject(Model model, Vector2 pos, string prefix = "Object_")
        {
            GameObject gameObject = new GameObject(
                                        game,
                                        prefix + pos.X + "x" + pos.Y,
                                        model,
                                        new Vector3(pos.X * WallShift, 0f, pos.Y * WallShift)
                                    );

            return gameObject;
        }

        private void drawFloor(int Width, int Height)
        {
            int modelWidth = 50;

            float positionX = Width * WallShift / 2 - modelWidth;
            float positionZ = Height * WallShift / 2 - modelWidth;

            GameObject floor = new GameObject(
                                   this.game,
                                   "Floor",
                                   this.wall,
                                   new Vector3(positionX, 0f, positionZ),
                                   Vector3.Zero,
                                   new Vector3((float) Width, 0f, (float) Height)
                               );

            this.AddChild(floor);
        }
    }
}
