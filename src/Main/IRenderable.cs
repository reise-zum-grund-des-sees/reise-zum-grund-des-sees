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
    interface IRenderable
    {
        void Initialize(GraphicsDevice _graphicsDevice, ContentManager _contentManager);
        void Render(GameFlags _flags, IEffect _effect, GraphicsDevice _grDevice);
    }
}
