﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReiseZumGrundDesSees
{
    class Lever : IWorldObject
    {
        public static List<Lever> LeverList = new List<Lever>();
        public Model Model;
         
        public bool is_pressed;
        ContentManager ContentManager;
        public double Rotation;
        public bool alive;

        public Hitbox Hitbox
        {
            get;        
        }

        public Vector3Int Position
        {
            get;
        }

        public WorldBlock Type
        {
            get;
        }

        public Lever(ContentManager _contentManager,Vector3Int _position)
        {
            if (AtPosition(_position) == null) {
            alive = true;
            Position = _position;
            Hitbox= new Hitbox(_position.X+0.5f, _position.Y, _position.Z + 0.5f, 1f, 1f, 1f);//richtig schieben, im render mus auch Y+0.5f gesetzt werden
            Type = WorldBlock.Lever;
            //Position = _position + new Vector3(0.5f,0.5f,0.5f);
            is_pressed = false;
            ContentManager = _contentManager;
            Rotation = 0;
            Model = _contentManager.Load<Model>("schalter_oben");
            LeverList.Add(this);
            }
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

        public static Lever AtPosition(Vector3Int _position)
        {
            for (int i = 0; i < LeverList.Count; i++)
                if (LeverList[i].Position==_position) return LeverList[i];

            return null;
        }

        public UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            //throw new NotImplementedException();
            return (ref GameState _state) =>
            {

            };
        }    
    }
}
