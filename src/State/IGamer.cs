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

        private Rectangle RHerz1 = new Rectangle(100, 70, 100, 70);
        private Rectangle RHerz2 = new Rectangle(200, 70, 100, 70);
        private Rectangle RHerz3 = new Rectangle(300, 70, 100, 70);

        private Rectangle RSkill1 = new Rectangle(100, 740, 80, 80);
        private Rectangle RSkill2 = new Rectangle(190, 740, 80, 80);
        private Rectangle RSkill3 = new Rectangle(280, 740, 80, 80);

        private SpriteFont S1;
        private SpriteFont S2;
        private SpriteFont S3;
        private SpriteFont S4;

        private Vector2 VS1 = new Vector2(110, 740);
        private Vector2 VS2 = new Vector2(200, 740);
        private Vector2 VS3 = new Vector2(290, 740);

        private Vector2 CD1 = new Vector2(140, 740);
        private Vector2 CD2 = new Vector2(230, 740);
        private Vector2 CD3 = new Vector2(320, 740);

        private Rectangle RTextbox = new Rectangle(640, 730, 350, 240);
   

        private Vector2 Textboxtext1 = new Vector2(650, 740);
        private Vector2 Textboxtext2 = new Vector2(650, 780);
        private Vector2 Textboxtext3 = new Vector2(650, 820);
        private Vector2 Textboxtext4 = new Vector2(650, 860);

        private int Dialog;

        private Vector2 scalingFactor = new Vector2(1f, 1f);

       

        public IGamer(ContentManager _content)
        {
            vollesHerz = _content.Load<Texture2D>(ContentRessources.ICON_HERZ_AUSGEFUELLT);
            leeresHerz = _content.Load<Texture2D>(ContentRessources.ICON_HERZ_UNAUSGEFUELLT);

            skill1 = _content.Load<Texture2D>(ContentRessources.ICON_WUERFEL_HELL);
            skill2 = _content.Load<Texture2D>(ContentRessources.ICON_WUERFEL_GRAU);
            skill3 = _content.Load<Texture2D>(ContentRessources.ICON_WUERFEL_SCHWARZ);

            S1 = _content.Load<SpriteFont>(ContentRessources.FONT_ARIAL_12);
            S2 = _content.Load<SpriteFont>(ContentRessources.FONT_ARIAL_12);
            S3 = _content.Load<SpriteFont>(ContentRessources.FONT_ARIAL_12);
            S4 = _content.Load<SpriteFont>(ContentRessources.FONT_ARIAL_32);
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

            Dialog = _gameState.Player.Dialog;
          
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

            //Anzahl Blöcke, die bereit sind
            _spriteBatch.DrawString(S1, Player.AnzahlBlockReadyL.ToString(), CD1 * scalingFactor, Color.White);
            _spriteBatch.DrawString(S2, Player.AnzahlBlockReadyM.ToString(), CD2 * scalingFactor, Color.White);
            _spriteBatch.DrawString(S3, Player.AnzahlBlockReadyS.ToString(), CD3 * scalingFactor, Color.White);

            //Tipps
            if (Dialog != -1)
            {
                _spriteBatch.Draw(skill1, RTextbox.Scale(scalingFactor), Color.White);

            }

            switch (Dialog) {
                case 0:
                    _spriteBatch.DrawString(S4, "Bewegen      -> W/A/S/D", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "2x Spacebar -> Doppelsprung", Textboxtext2 * scalingFactor, Color.Black);
                    break;
                case 1:
                    _spriteBatch.DrawString(S4, "Dieser Block speichert", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "Spawnposition und setzt", Textboxtext2 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "Bloecke zurueck", Textboxtext3 * scalingFactor, Color.Black);                   
                    break;
                case 2:
                    _spriteBatch.DrawString(S4, "Springe auf Gegner um", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "sie zu vernichten.", Textboxtext2 * scalingFactor, Color.Black);
                    break;
                case 3:
                    _spriteBatch.DrawString(S4, "Mit 1,2 und 3 koennen gefundene", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "Bloecke gesetzt werden.", Textboxtext2 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "Druecke Q um sie aufzusammeln,", Textboxtext3 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "wenn du davor stehst.", Textboxtext4 * scalingFactor, Color.Black);
                    break;
                case 4:
                    _spriteBatch.DrawString(S4, "Bloecke verschwinden auch wenn", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "du Speicherst oder dich zu weit", Textboxtext2 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "entfernst. Leichte Blöcke (1)", Textboxtext3 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "schweben in der Luft.", Textboxtext4 * scalingFactor, Color.Black);
                    break;
                case 5:
                    _spriteBatch.DrawString(S4, "Schalter lassen Bloecke ", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "erscheinen,verschwinden", Textboxtext2 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "oder bewegen Bloecke.", Textboxtext3 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "Akiviere ihn mit E.", Textboxtext4 * scalingFactor, Color.Black);
                    break;
                case 6:
                    _spriteBatch.DrawString(S4, "Mittelschwere Blöcke (2)", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "fallen zu Boden und", Textboxtext2 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "schwimmen im Wasser.", Textboxtext3 * scalingFactor, Color.Black);
                    break;
                case 7:
                    _spriteBatch.DrawString(S4, "Diese Schalter senkt", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "den Wasserstand.", Textboxtext2 * scalingFactor, Color.Black);
                    break;
                case 8:
                    _spriteBatch.DrawString(S4, "Lasse ein mittelschweren (2)", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "oder schweren (3) Block auf", Textboxtext2 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "die Bodenplatte fallen um ", Textboxtext3 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "die Bodenplatte z u aktivieren.", Textboxtext4 * scalingFactor, Color.Black);
                    break;
                case 9:
                    _spriteBatch.DrawString(S4, "Vergiss nicht, dass du mit Q", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "Blöcke wieder aufsammeln kannst.", Textboxtext2 * scalingFactor, Color.Black);
                    break;
                case 10:
                    _spriteBatch.DrawString(S4, "Schwere Blöcke (3) fallen", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "zu Boden und gehen", Textboxtext2 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "im Wasser unter.", Textboxtext3 * scalingFactor, Color.Black);
                    break;
                case 11:
                    _spriteBatch.DrawString(S4, "Diese Schalter senkt den", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "Wasserstand. Doch ein weiterer", Textboxtext2 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "Schalter wird für Rückweg", Textboxtext3 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "benötigt", Textboxtext4 * scalingFactor, Color.Black);
                    break;
                case 12:
                    _spriteBatch.DrawString(S4, "Hier scheint kein Schalter zur", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "Wasserstandssenkung zu sein.", Textboxtext2 * scalingFactor, Color.Black);
                    break;
                case 13:
                    _spriteBatch.DrawString(S4, "Diese Schalter senkt", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "den Wasserstand.", Textboxtext2 * scalingFactor, Color.Black);
                    break;
                case 14:
                    _spriteBatch.DrawString(S4, "Diese Schalter senkt", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "den Wasserstand.", Textboxtext2 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "Erkunde den Grund des Sees!", Textboxtext3 * scalingFactor, Color.Black);
                    break;
                case 15:
                    _spriteBatch.DrawString(S4, "Glückwunsch! Du hast", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(S4, "den Schatz geborgen!", Textboxtext2 * scalingFactor, Color.Black);
                    break;
                default:
                    break;
        }
            _spriteBatch.End();
        }
    }
}