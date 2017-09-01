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

        private Rectangle RHerz1 = new Rectangle(30, 30, 100, 100);
        private Rectangle RHerz2 = new Rectangle(130, 30, 100, 100);
        private Rectangle RHerz3 = new Rectangle(230, 30, 100, 100);

        private Rectangle RSkill1 = new Rectangle(370, 40, 80, 80);
        private Rectangle RSkill2 = new Rectangle(470, 40, 80, 80);
        private Rectangle RSkill3 = new Rectangle(570, 40, 80, 80);

        private SpriteFont arial_12;
        private SpriteFont arial_32;

        private Vector2 VS1 = new Vector2(110, 740);
        private Vector2 VS2 = new Vector2(200, 740);
        private Vector2 VS3 = new Vector2(290, 740);

        private Vector2 CD1 = new Vector2(410, 80);
        private Vector2 CD2 = new Vector2(510, 80);
        private Vector2 CD3 = new Vector2(610, 80);

        private Rectangle RTextbox = new Rectangle(640, 730, 350, 240);
   
        private Vector2 Textboxtext1 = new Vector2(650, 740);
        private Vector2 Textboxtext2 = new Vector2(650, 780);
        private Vector2 Textboxtext3 = new Vector2(650, 820);
        private Vector2 Textboxtext4 = new Vector2(650, 860);

        private int Dialog;

        private Vector2 scalingFactor = new Vector2(1f, 1f);

        private Rectangle herz1 = new Rectangle(602, 735, 196, 195);
        private Rectangle herz2 = new Rectangle(808, 746, 193, 179);
        private Rectangle herz3 = new Rectangle(809, 543, 193, 191);

        private Rectangle block_leicht = new Rectangle(812, 6, 209, 208);
        private Rectangle block_medium = new Rectangle(810, 208, 206, 207);
        private Rectangle block_schwer = new Rectangle(203, 203, 206, 212);

        private Texture2D texture;

        private int playerHealth = 0;
       

        public IGamer(ContentManager _content)
        {
            vollesHerz = _content.Load<Texture2D>(ContentRessources.ICON_HERZ_AUSGEFUELLT);
            leeresHerz = _content.Load<Texture2D>(ContentRessources.ICON_HERZ_UNAUSGEFUELLT);

            skill1 = _content.Load<Texture2D>(ContentRessources.ICON_WUERFEL_HELL);
            skill2 = _content.Load<Texture2D>(ContentRessources.ICON_WUERFEL_GRAU);
            skill3 = _content.Load<Texture2D>(ContentRessources.ICON_WUERFEL_SCHWARZ);

            texture = _content.Load<Texture2D>(ContentRessources.TEXTURE);

            arial_12 = _content.Load<SpriteFont>(ContentRessources.FONT_ARIAL_12);
            arial_32 = _content.Load<SpriteFont>(ContentRessources.FONT_ARIAL_32);
        }

        public void Update(InputEventArgs _args, GameState _gameState, GameFlags _flags, Point _windowSize, GameTime _gameTime)
        {
            scalingFactor = _windowSize.ToVector2() * 0.001f;
            playerHealth = _gameState.Player.Health;
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

            /*_spriteBatch.Draw(Herz1, RHerz1.Scale(scalingFactor), Color.White);
            _spriteBatch.Draw(Herz2, RHerz2.Scale(scalingFactor), Color.White);
            _spriteBatch.Draw(Herz3, RHerz3.Scale(scalingFactor), Color.White);*/

            Color _red = new Color(0.9f, 0.5f, 0.5f);
            Color _transparent = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            _spriteBatch.Draw(texture, RHerz1.Scale(scalingFactor), herz1, (playerHealth == 0)? _transparent : _red);
            _spriteBatch.Draw(texture, RHerz2.Scale(scalingFactor), herz2, (playerHealth <= 1)? _transparent : _red);
            _spriteBatch.Draw(texture, RHerz3.Scale(scalingFactor), herz3, (playerHealth <= 2)? _transparent : _red);

            Color t = new Color(0.7f, 0.7f, 0.7f, 0.5f);
            _spriteBatch.Draw(texture, RSkill1.Scale(scalingFactor), block_leicht, t);
            t = new Color(1f, 1f, 1f, 0.5f);
            _spriteBatch.Draw(texture, RSkill2.Scale(scalingFactor), block_medium, t);
            _spriteBatch.Draw(texture, RSkill3.Scale(scalingFactor), block_schwer, t);
            /*_spriteBatch.DrawString(S1, "1", VS1 * scalingFactor, Color.White);
            _spriteBatch.DrawString(S2, "2", VS2 * scalingFactor, Color.White);
            _spriteBatch.DrawString(S3, "3", VS3 * scalingFactor, Color.White);*/

            //Anzahl Blöcke, die bereit sinod
            string _count1 = Player.AnzahlBlockReadyL.ToString();
            _spriteBatch.DrawString(arial_32, _count1, CD1 * scalingFactor - arial_32.MeasureString(_count1) * 0.5f, Color.Black);

            string _count2 = Player.AnzahlBlockReadyM.ToString();
            _spriteBatch.DrawString(arial_32, _count2, CD2 * scalingFactor - arial_32.MeasureString(_count2) * 0.5f, Color.Black);

            string _count3 = Player.AnzahlBlockReadyS.ToString();
            _spriteBatch.DrawString(arial_32, _count3, CD3 * scalingFactor - arial_32.MeasureString(_count3) * 0.5f, Color.Black);

            //Tipps
            if (Dialog != -1 && Dialog!= 100)
            {
                _spriteBatch.Draw(skill1, RTextbox.Scale(scalingFactor), Color.White);

            }

            switch (Dialog)
            {
             
                case 0:
                    _spriteBatch.DrawString(arial_32, "Bewegen      -> W/A/S/D", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "2x Spacebar -> Doppelsprung", Textboxtext2 * scalingFactor, Color.Black);
                    break;
                case 1:
                    _spriteBatch.DrawString(arial_32, "Dieser Block speichert", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "Spawnposition und setzt", Textboxtext2 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "Bloecke zurueck", Textboxtext3 * scalingFactor, Color.Black);
                    break;
                case 2:
                    _spriteBatch.DrawString(arial_32, "Springe auf Gegner um", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "sie zu vernichten.", Textboxtext2 * scalingFactor, Color.Black);
                    break;             
                case 3://neu
                    _spriteBatch.DrawString(arial_32, "Auf dem Grund des Sees", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "soll ein Schatz versteckt", Textboxtext2 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "sein. Hol ihn dir!", Textboxtext3 * scalingFactor, Color.Black);
                    break;
                case 4:
                    _spriteBatch.DrawString(arial_32, "Mit 1,2 und 3 koennen gefundene", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "Bloecke gesetzt werden.", Textboxtext2 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "Druecke Q um sie aufzusammeln,", Textboxtext3 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "wenn du davor stehst.", Textboxtext4 * scalingFactor, Color.Black);
                    break;
                case 5:
                    _spriteBatch.DrawString(arial_32, "Bloecke verschwinden auch wenn", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "du Speicherst oder dich zu weit", Textboxtext2 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "entfernst. Leichte Bloecke (1)", Textboxtext3 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "schweben in der Luft.", Textboxtext4 * scalingFactor, Color.Black);
                    break;
                case 6://neu
                    _spriteBatch.DrawString(arial_32, "Mit ESC wird das Menue", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "geoeffnet. Dort kann", Textboxtext2 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "gespeichert,geladen", Textboxtext3 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "und beendet werden.", Textboxtext4 * scalingFactor, Color.Black);
                    break;
                case 7: //neu
                    _spriteBatch.DrawString(arial_32, "Um an den Schatz zu kommen,", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "muss das Wasser abgelassen", Textboxtext2 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "werden. Suche die noetigen", Textboxtext3 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "Schalter.", Textboxtext4 * scalingFactor, Color.Black);
                    break;
                case 8:
                    _spriteBatch.DrawString(arial_32, "Lasse ein mittelschweren (2)", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "oder schweren (3) Block auf", Textboxtext2 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "die Bodenplatte fallen um ", Textboxtext3 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "die Bodenplatte zu aktivieren.", Textboxtext4 * scalingFactor, Color.Black);
                    break;
                case 9:
                    _spriteBatch.DrawString(arial_32, "Vergiss nicht, dass du mit", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "der Taste Q Bloecke wieder", Textboxtext2 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "aufsammeln kannst.", Textboxtext3 * scalingFactor, Color.Black);
                    break;
                case 10:
                    _spriteBatch.DrawString(arial_32, "Schwere Bloecke (3) fallen", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "zu Boden und gehen", Textboxtext2 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "im Wasser unter.", Textboxtext3 * scalingFactor, Color.Black);
                    break;
                case 11:
                    _spriteBatch.DrawString(arial_32, "Diese Schalter senkt den", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "Wasserstand. Doch ein weiterer", Textboxtext2 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "Schalter wird fuer Rueckweg", Textboxtext3 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "benoetigt", Textboxtext4 * scalingFactor, Color.Black);
                    break;
                case 12:
                    _spriteBatch.DrawString(arial_32, "Hier scheint kein Schalter zur", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "Wasserstandssenkung zu sein.", Textboxtext2 * scalingFactor, Color.Black);
                    break;
                case 13:
                    _spriteBatch.DrawString(arial_32, "Diese Schalter senkt", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "den Wasserstand.", Textboxtext2 * scalingFactor, Color.Black);
                    break;
                case 14:
                    _spriteBatch.DrawString(arial_32, "Diese Schalter senkt", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "den Wasserstand.", Textboxtext2 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "Erkunde den Grund des Sees!", Textboxtext3 * scalingFactor, Color.Black);
                    break;
                    /*
                case 15:
                    _spriteBatch.DrawString(arial_32, "Glueckwunsch! Du hast", Textboxtext1 * scalingFactor, Color.Black);
                    _spriteBatch.DrawString(arial_32, "den Schatz geborgen!", Textboxtext2 * scalingFactor, Color.Black);
                    break;
                case 100:
                    break;
                    */
                default:
                    break;
            }
            _spriteBatch.End();
        }
    }
}