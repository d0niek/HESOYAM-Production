using Microsoft.Xna.Framework;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using HESOYAM_Production;
using System.Diagnostics;

namespace App
{

    public class Scene: GameObject
    {
        public Scene(Engine game, string name, string bitmapPath, Model wall) : base(game, name)
        {
            Bitmap bmp = (Bitmap) Bitmap.FromFile(bitmapPath);
            this.AddChild(new GameObject(game, "Walls"));

            for (int i = 0; i < bmp.Height; i++) {
                for (int j = 0; j < bmp.Width; j++) {
                    System.Drawing.Color color = bmp.GetPixel(i, j);

                    if (color.R == 0 && color.G == 0 && color.B == 0) {
                        GameObject newChild = new GameObject(
                                                  game,
                                                  "Wall" + j + "x" + i,
                                                  wall,
                                                  new Vector3(i * 200f, 0f, j * 200f),
                                                  Vector3.Zero,
                                                  new Vector3(1f, 4f, 1f)
                                              );
                        this.children["Walls"].AddChild(newChild);
                    }
                }
            }

            GameObject floor = new GameObject(
                                   game,
                                   "Floor",
                                   wall,
                                   new Vector3((float) bmp.Height * 100f, -400f, (float) bmp.Width * 100f),
                                   Vector3.Zero,
                                   new Vector3((float) bmp.Height, 0.1f, (float) bmp.Width)
                               );

            this.AddChild(floor);
            this.AddChildrenToGame(true);
        }

        public override void Draw(GameTime gameTime)
        {
        }
    }
}

