﻿using Microsoft.Xna.Framework;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using HESOYAM_Production;
using System;

namespace App
{
    class Door : Wall
    {
        bool isOpen;

        public Door(
             Engine game,
             string name,
             Model model,
             Model transparentModel,
             bool isOpen,
             Vector3 position = default(Vector3),
             Vector3 rotation = default(Vector3),
             Vector3? scale = null
         ) : base(game, name, model, transparentModel, position, rotation, scale)
        {
            this.isOpen = isOpen;
        }



       public void update()
       {
           if (colliders.ContainsKey("main") && colliders["main"].CollidesWith(game.Scene.Player.colliders["main"]))
           {
            if (game.player.getKeyInfo("key"))
               {
                   this.colliders.Remove("main");
                   this.isOpen = true;
                    System.Console.WriteLine("Drzwi otwarte - użyto klucza");
               }
           }
       }
    }
}