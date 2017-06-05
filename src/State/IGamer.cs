using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ReiseZumGrundDesSees.State
{
    class IGamer
    {
        
        public Texture2D vollesHerz;
        public Texture2D leeresHerz;
        public Texture2D Herz;

        public Texture2D skill1;
        public Texture2D skill2;
        public Texture2D skill3;

        private Rectangle RVollesHerz = new Rectangle(300, 70, 100, 70);

        private Rectangle RSkill1 = new Rectangle(300, 640, 80, 80);
        private Rectangle RSkill2 = new Rectangle(390, 640, 80, 80);
        private Rectangle RSkill3 = new Rectangle(480, 640, 80, 80);

        private SpriteFont S1;
        private SpriteFont S2;
        private SpriteFont S3;

        private Vector2 VS1 = new Vector2(310, 640);
        private Vector2 VS2 = new Vector2(400, 640);
        private Vector2 VS3 = new Vector2(490, 640);



        public IGamer(ContentManager Content)
        {
           
            vollesHerz = Content.Load<Texture2D>("Herz_ausgefüllt");
            leeresHerz = Content.Load<Texture2D>("Herz_unausgefüllt");

            skill1 = Content.Load<Texture2D>("wuerfel_hell_alt");
            skill2 = Content.Load<Texture2D>("wuerfel_grau_alt");
            skill3 = Content.Load<Texture2D>("wuerfel_sw_alt");

            S1 = Content.Load<SpriteFont>("S1");
            S2 = Content.Load<SpriteFont>("S2");
            S3 = Content.Load<SpriteFont>("S3");
        }
        public void Update(InputEventArgs _args, Point _windowSize, GameState _gameState)
        {
            
            Herz = vollesHerz;
            //_gameState.Player.Healthcd für Zeit nach dem Leben verloren wurde, also =0 wenn geradde Leben verloren
            if (_gameState.Player.Health==1)
                Herz = leeresHerz;
            // _gameState.Player.Blocks;


        }
        public void Render(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Begin();
           
            _spriteBatch.Draw(Herz, RVollesHerz, Color.White);

            _spriteBatch.Draw(skill1, RSkill1, Color.White);
            _spriteBatch.Draw(skill2, RSkill2, Color.White);
            _spriteBatch.Draw(skill3, RSkill3, Color.White);
            _spriteBatch.DrawString(S1, "1", VS1, Color.White);
            _spriteBatch.DrawString(S2, "2", VS2, Color.White);
            _spriteBatch.DrawString(S3, "3", VS3, Color.White);

           


            _spriteBatch.End();



        }
       // throw new NotImplementedException();
    }
}
