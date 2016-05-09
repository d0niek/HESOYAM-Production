using Microsoft.Xna.Framework;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using HESOYAM_Production;

namespace App
{

    public class Scene: GameObject
    {
        public Scene(Engine game, string name, string bitmapPath, Model wall) : base(game, name)
        {
            Bitmap bmp = (Bitmap) Image.FromFile(bitmapPath);
            this.AddChild(new GameObject(game, "Walls"));

            this.buildWalls(bmp, wall);

            GameObject floor = new GameObject(
                                   game,
                                   "Floor",
                                   wall,
                                   new Vector3((float) bmp.Height * 100f, -400f, (float) bmp.Width * 100f),
                                   Vector3.Zero,
                                   new Vector3((float) bmp.Height, 0.1f, (float) bmp.Width)
                               );

            this.AddChild(floor);
            this.children["Walls"].AddChildrenToGame(true);
        }

        private void buildWalls(Bitmap bmp, Model model)
        {
            for (int i = 0; i < bmp.Height; i++) {
                for (int j = 0; j < bmp.Width; j++) {
                    System.Drawing.Color color = bmp.GetPixel(i, j);

                    if (color.R == 0 && color.G == 0 && color.B == 0) {
                        GameObject wall = new GameObject(
                                              game,
                                              "Wall" + j + "x" + i,
                                              model,
                                              new Vector3(i * 100f, 0f, j * 100f)
                                          );
                        this.children["Walls"].AddChild(wall);
                    }
                }
            }
        }
    }
}
