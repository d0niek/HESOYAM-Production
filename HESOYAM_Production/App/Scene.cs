using Microsoft.Xna.Framework;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using HESOYAM_Production;

namespace App
{

    public class Scene: GameObject
    {
        private const float Shift = 100;

        public Scene(Engine game, string name, string bitmapPath, Model wall) : base(game, name)
        {
            Bitmap bmp = (Bitmap) Image.FromFile(bitmapPath);
            this.AddChild(new GameObject(game, "Walls"));

            this.buildWalls(bmp, wall);
            this.buildFloor(bmp.Width, bmp.Height, wall);

            this.AddChildrenToGame(true);
        }

        private void buildWalls(Bitmap bmp, Model model)
        {
            for (int i = 0; i < bmp.Height; i++) {
                for (int j = 0; j < bmp.Width; j++) {
                    System.Drawing.Color color = bmp.GetPixel(i, j);

                    if (color.R == 0 && color.G == 0 && color.B == 0) {
                        GameObject wall = new GameObject(
                                              game,
                                              "Wall_" + j + "x" + i,
                                              model,
                                              new Vector3(i * Shift, 0f, j * Shift)
                                          );
                        this.children["Walls"].AddChild(wall);
                    }
                }
            }
        }

        private void buildFloor(int Width, int Height, Model model)
        {
            float positionX = Height * Shift / 2;
            float positionZ = Width * Shift / 2;

            GameObject floor = new GameObject(
                                   this.game,
                                   "Floor",
                                   model,
                                   new Vector3(positionX, 0f, positionZ),
                                   Vector3.Zero,
                                   new Vector3((float) Height, 0f, (float) Width)
                               );

            this.AddChild(floor);
        }
    }
}
