using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ReiseZumGrundDesSees
{
    class RenderableWorld : World, IUpdateable, IRenderable
    {
        private readonly VertexPositionColorTexture[,][] Vertices;
        private readonly VertexBuffer[,] VertexBuffers;

        private BasicEffect effect;
        private GraphicsDevice graphicsDevice;

        private readonly Texture2D blockTexture;
        private const string BLOCKTEXTURE_NAME = "blocktexture";

        private List<Point> invalidatedChunks = new List<Point>();

        public RenderableWorld(string _basePath, ContentManager _content) : base(_basePath)
        {
            Vertices = new VertexPositionColorTexture[RegionsCountX, RegionsCountZ][];
            VertexBuffers = new VertexBuffer[RegionsCountX, RegionsCountZ];

            blockTexture = _content.Load<Texture2D>(BLOCKTEXTURE_NAME);
            Blocks.OnBlockChanged += (WorldBlock _, WorldBlock __, int x, int y, int z) =>
                invalidatedChunks.Add(new Point(x / RegionSizeX, z / RegionSizeZ));
        }

        public RenderableWorld(int _regionSizeX, int _regionSizeY, int _regionSizeZ, int _regionsCountX, int _regionsCountZ, Vector3 _spawnPos, ContentManager _content)
            : base(_regionSizeX, _regionSizeY, _regionSizeZ, _regionsCountX, _regionsCountZ, _spawnPos)
        {
            Vertices = new VertexPositionColorTexture[_regionsCountX, _regionsCountZ][];
            VertexBuffers = new VertexBuffer[_regionsCountX, _regionsCountZ];

            blockTexture = _content.Load<Texture2D>(BLOCKTEXTURE_NAME);
            Blocks.OnBlockChanged += (WorldBlock _, WorldBlock __, int x, int y, int z) =>
                invalidatedChunks.Add(new Point(x / RegionSizeX, z / RegionSizeZ));
        }

        private void LoadVertices(int _regionX, int _regionZ)
        {
            WorldRegion _region = Regions[_regionX, _regionZ];
            List<VertexPositionColorTexture> _vertices = new List<VertexPositionColorTexture>();

            Vertices[_regionX, _regionZ] = VertexGenerator.GenerateVertices(this.Blocks, _regionX * RegionSizeX, 0, _regionZ * RegionSizeZ, RegionSizeX, RegionSizeY, RegionSizeZ);
            if (Vertices[_regionX, _regionZ].Length != 0)
            {
                if (VertexBuffers[_regionX, _regionZ] != null)
                    VertexBuffers[_regionX, _regionZ].Dispose();
                VertexBuffers[_regionX, _regionZ] = new VertexBuffer(graphicsDevice, VertexPositionColorTexture.VertexDeclaration, Vertices[_regionX, _regionZ].Length, BufferUsage.WriteOnly);
                VertexBuffers[_regionX, _regionZ].SetData(Vertices[_regionX, _regionZ]);
            }
        }
        private void UnloadVertices(int _regionX, int _regionZ)
        {
            Vertices[_regionX, _regionZ] = null;
            VertexBuffers[_regionX, _regionZ]?.Dispose();
            VertexBuffers[_regionX, _regionZ] = null;
        }

        public override UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            foreach (Point v in invalidatedChunks)
            {
                Vertices[v.X, v.Y] = null;
                VertexBuffers[v.X, v.Y]?.Dispose();
                VertexBuffers[v.X, v.Y] = null;
            }
            invalidatedChunks.Clear();
            
            for (int x = 0; x < RegionsCountX; x++)
                for (int z = 0; z < RegionsCountZ; z++)
                {
                    float _distance = Vector2.Distance(new Vector2(_view.CameraCenter.Position.X, _view.CameraCenter.Position.Z), new Vector2((x + 0.5f) * RegionSizeX, (z + 0.5f) * RegionSizeZ));

                    if (_distance < 20 && Vertices[x, z] == null)
                    {
                        LoadVertices(x, z);
                    }
                    else if (_distance > 30 && Vertices[x, z] != null)
                    {
                        UnloadVertices(x, z);
                    }
                }

            return base.Update(_view, _flags, _inputArgs, _passedTime);
        }

        public void Initialize(GraphicsDevice _graphicsDevice)
        {
            graphicsDevice = _graphicsDevice;

            foreach (var _obj in specialBlocks)
                _obj.Value.Initialize(graphicsDevice);

            effect = new BasicEffect(graphicsDevice);
            effect.TextureEnabled = true;
            effect.Texture = blockTexture;
            effect.VertexColorEnabled = true;
        }

        public override void AddObject(ISpecialBlock _object)
        {
            base.AddObject(_object);
            _object.Initialize(graphicsDevice);
        }

        public void Render(GameFlags _flags, Matrix _viewMatrix, Matrix _perspectiveMatrix)
        {
            foreach (var _obj in specialBlocks)
                _obj.Value.Render(_flags, _viewMatrix, _perspectiveMatrix);
            DebugHelper.Information.RenderedWorldChunks = 0;
            DebugHelper.Information.RenderedWorldVertices = 0;

            int maxX = Vertices.GetLength(0);
            int maxZ = Vertices.GetLength(1);

            for (int x = 0; x < maxX; x++)
                for (int z = 0; z < maxZ; z++)
                {
                    if (Vertices[x, z]?.Length > 0)
                    {
                        DebugHelper.Information.RenderedWorldChunks++;
                        DebugHelper.Information.RenderedWorldVertices += (uint)(Vertices[x, z]?.Length ?? 0);

                        effect.View = _viewMatrix;
                        effect.World = Matrix.CreateTranslation(x * RegionSizeX, 0, z * RegionSizeZ);
                        effect.Projection = _perspectiveMatrix;

                        graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

                        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                            pass.Apply();

                        graphicsDevice.SetVertexBuffer(VertexBuffers[x, z]);
                        graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, Vertices[x, z].Length / 3);
                    }
                }
        }
    }
}
