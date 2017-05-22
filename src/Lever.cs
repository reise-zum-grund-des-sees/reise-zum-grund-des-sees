using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReiseZumGrundDesSees
{
    class Lever 
    {
        public static List<Lever> LeverList = new List<Lever>();
        public Model Model;
        public Vector3 Position;    
        public bool is_pressed;
        ContentManager ContentManager;
        public double Rotation;
        public bool alive;
        public Lever(ContentManager _contentManager,Vector3 _position)
        {
            alive = true;
            Position = _position + new Vector3(0.5f,0.5f,0.5f);
            is_pressed = false;
            ContentManager = _contentManager;
            Rotation = 0;
            Model = _contentManager.Load<Model>("schalter_oben");
            LeverList.Add(this);
         
        }
        public void press()
        {
            if (alive == true) { 
            if (is_pressed == false) {
            Model = ContentManager.Load<Model>("schalter_unten");
                is_pressed = true;
            }
            else
            {
            Model = ContentManager.Load<Model>("schalter_oben");
                is_pressed = false;
            }
            }
        }
        
     public static Lever AtPosition(Vector3 _position)
        {
            for (int i = 0; i < LeverList.Count; i++)
                if (LeverList[i].Position.Equals(_position)) return LeverList[i];

            return null;
        }
     
    }
}
