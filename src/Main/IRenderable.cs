using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees
{
    interface IRenderable
    {
        void Initialize(GraphicsDevice _graphicsDevice);
        void Render(GameFlags _flags, Matrix _viewMatrix, Matrix _perspectiveMatrix);
    }
}
