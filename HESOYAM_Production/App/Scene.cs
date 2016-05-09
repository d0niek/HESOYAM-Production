using Microsoft.Xna.Framework;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using HESOYAM_Production;

namespace App
{

    public class Scene: GameObject
    {
        private const float WallShift = 100;

        public Scene(Engine game, string name, string bitmapPath, Model wall) : base(game, name)
        {
            Bitmap bmp = (Bitmap) Image.FromFile(bitmapPath);

            this.buildMap(bmp, wall);
            this.drawFloor(bmp.Width, bmp.Height, wall);

            this.AddChildrenToGame(true);
        }

        private void buildMap(Bitmap bmp, Model model)
        {
            this.AddChild(new GameObject(game, "Walls"));

            for (int i = 0; i < bmp.Width; i++) {
                for (int j = 0; j < bmp.Height; j++) {
                    System.Drawing.Color color = bmp.GetPixel(i, j);

                    this.buildMapObject(color, model, i, j);
                }
            }
        }

        private void buildMapObject(System.Drawing.Color color, Model model, int x, int y)
        {
            if (color.R == 0 && color.G == 0 && color.B == 0) {
                this.buildWall(model, x, y);
            }
        }

        private void buildWall(Model model, int x, int y)
        {
            GameObject wall = new GameObject(
                                  game,
                                  "Wall_" + x + "x" + y,
                                  model,
                                  new Vector3(x * WallShift, 0f, y * WallShift)
                              );

            this.children["Walls"].AddChild(wall);
        }

        private void drawFloor(int Width, int Height, Model model)
        {
            int modelWidth = 50;

            float positionX = Width * WallShift / 2 - modelWidth;
            float positionZ = Height * WallShift / 2 - modelWidth;

            GameObject floor = new GameObject(
                                   this.game,
                                   "Floor",
                                   model,
                                   new Vector3(positionX, 0f, positionZ),
                                   Vector3.Zero,
                                   new Vector3((float) Width, 0f, (float) Height)
                               );

            this.AddChild(floor);
        }
    }
}
