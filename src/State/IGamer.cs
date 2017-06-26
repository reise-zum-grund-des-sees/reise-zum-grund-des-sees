using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ReiseZumGrundDesSees
{
    class IGamer : IOverlay
    {
        private Texture2D vollesHerz;
        private Texture2D leeresHerz;
        private Texture2D Herz1;
        private Texture2D Herz2;
        private Texture2D Herz3;

        private Texture2D skill1;
        private Texture2D skill2;
        private Texture2D skill3;

        private Rectangle RHerz1 = new Rectangle(250, 70, 100, 70);
        private Rectangle RHerz2 = new Rectangle(350, 70, 100, 70);
        private Rectangle RHerz3 = new Rectangle(450, 70, 100, 70);

        private Rectangle RSkill1 = new Rectangle(300, 640, 80, 80);
        private Rectangle RSkill2 = new Rectangle(390, 640, 80, 80);
        private Rectangle RSkill3 = new Rectangle(480, 640, 80, 80);

        private SpriteFont S1;
        private SpriteFont S2;
        private SpriteFont S3;

        private Vector2 VS1 = new Vector2(310, 640);
        private Vector2 VS2 = new Vector2(400, 640);
        private Vector2 VS3 = new Vector2(490, 640);

        private Vector2 scalingFactor = new Vector2(1f, 1f);

        public IGamer(ContentManager _content)
        {
            vollesHerz = _content.Load<Texture2D>(Content.ICON_HERZ_AUSGEFUELLT);
            leeresHerz = _content.Load<Texture2D>(Content.ICON_HERZ_UNAUSGEFUELLT);

            skill1 = _content.Load<Texture2D>(Content.ICON_WUERFEL_HELL);
            skill2 = _content.Load<Texture2D>(Content.ICON_WUERFEL_GRAU);
            skill3 = _content.Load<Texture2D>(Content.ICON_WUERFEL_SCHWARZ);

            S1 = _content.Load<SpriteFont>(Content.FONT_ARIAL_12);
            S2 = _content.Load<SpriteFont>(Content.FONT_ARIAL_12);
            S3 = _content.Load<SpriteFont>(Content.FONT_ARIAL_12);
        }

        public void Update(InputEventArgs _args, GameState _gameState, GameFlags _flags, Point _windowSize)
        {
            scalingFactor = _windowSize.ToVector2() * 0.001f;
            Herz1 = vollesHerz;
            Herz2 = vollesHerz;
            Herz3 = vollesHerz;
            //_gameState.Player.Healthcd für Zeit nach dem Leben verloren wurde, also =0 wenn geradde Leben verloren
            if (_gameState.Player.Health < 1)
                Herz1 = leeresHerz;
            if (_gameState.Player.Health < 2)
                Herz2 = leeresHerz;
            if (_gameState.Player.Health < 3)
                Herz3 = leeresHerz;
            // _gameState.Player.Blocks;
        }

        public void Render(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Begin();
           
            _spriteBatch.Draw(Herz1, RHerz1.Scale(scalingFactor), Color.White);
            _spriteBatch.Draw(Herz2, RHerz2.Scale(scalingFactor), Color.White);
            _spriteBatch.Draw(Herz3, RHerz3.Scale(scalingFactor), Color.White);

            _spriteBatch.Draw(skill1, RSkill1.Scale(scalingFactor), Color.White);
            _spriteBatch.Draw(skill2, RSkill2.Scale(scalingFactor), Color.White);
            _spriteBatch.Draw(skill3, RSkill3.Scale(scalingFactor), Color.White);
            _spriteBatch.DrawString(S1, "1", VS1 * scalingFactor, Color.White);
            _spriteBatch.DrawString(S2, "2", VS2 * scalingFactor, Color.White);
            _spriteBatch.DrawString(S3, "3", VS3 * scalingFactor, Color.White);

            _spriteBatch.End();
        }
    }
}