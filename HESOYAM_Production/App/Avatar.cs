using System;
using Microsoft.Xna.Framework;

namespace App
{

    public class Avatar
    {
        Character character;
        String textureName;
        int x;
        int y;

        public Character Character {
            get { return character; }
            private set { }
        }

        public String TextureName {
            get { return textureName; }
            private set { }
        }

        public int X {
            get { return x; }
            private set { }
        }

        public int Y {
            get { return y; }
            private set { }
        }

        public Avatar(Character character, String textureName, int x, int y)
        {
            this.character = character;
            this.textureName = textureName;
            this.x = x;
            this.y = y;
        }

        public Rectangle GetAvatarRectangle()
        {
            return new Rectangle(x, y, 50, 50);
        }
    }
}

