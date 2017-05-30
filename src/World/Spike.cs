using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
    class Spike : ISpecialBlock
    {
        ContentManager ContentManager;
        public static List<Spike> SpikeList = new List<Spike>();
        public Model Model;

        public Vector3Int Position
        {
            get;
        }

        public WorldBlock Type
        {
            get;
        }
        public Spike(ContentManager _contentManager, Vector3Int _position)
        {
            if (AtPosition(_position) == null)
            {
                Position = _position;
                Type = WorldBlock.Spikes;
                ContentManager = _contentManager;
                Model = _contentManager.Load<Model>("leichter_Block");
                SpikeList.Add(this);
            }
        }
        public static Spike AtPosition(Vector3Int _position)
        {
            for (int i = 0; i < SpikeList.Count; i++)
                if (SpikeList[i].Position == _position) return SpikeList[i];

            return null;
        }
        public UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            return (ref GameState _state) =>
            {

            };
        }

        public void Initialize(GraphicsDevice _graphicsDevice)
        {
            throw new NotImplementedException();
        }

        public void Render(GameFlags _flags, Matrix _viewMatrix, Matrix _perspectiveMatrix)
        {
            throw new NotImplementedException();
        }
    }
}
