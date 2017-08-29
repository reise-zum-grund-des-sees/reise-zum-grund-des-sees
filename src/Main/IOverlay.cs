using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees
{
    interface IOverlay
    {
        void Update(InputEventArgs _inputEvents, GameState _gameState, GameFlags _flags, Point _windowSize);
        void Render(SpriteBatch _spriteBatch);
    }
}
