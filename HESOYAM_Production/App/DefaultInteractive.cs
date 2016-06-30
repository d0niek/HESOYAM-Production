using App;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HESOYAM_Production.App
{
    class DefaultInteractive : GameObject, IInteractiveObject
    {
        public DefaultInteractive(Engine game, Vector3 position) : base(game, "defaultInteractive", position)
        { }

        public List<String> GetOptionsToInteract()
        {
            List<String> options = new List<String>();
            options.Add("Move here");
            return options;
        }
    }
}
