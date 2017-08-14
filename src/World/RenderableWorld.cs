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
        private ContentManager contentManager;

        private double viewDistance = 30.0;

        private Texture2D blockTexture;
        private const string BLOCKTEXTURE_NAME = "blocktexture";

        private List<Point> invalidatedChunks = new List<Point>();

        private GameState.View lastGameState;

        public RenderableWorld(ConfigFile.ConfigNode _config, ObjectIDMapper _idMapper, string _baseDir) : base(_config, _idMapper, _baseDir)
        {
            Vertices = new VertexPositionColorTexture[RegionsCount.X, RegionsCount.Y][];
            VertexBuffers = new VertexBuffer[RegionsCount.X, RegionsCount.Y];

            Blocks.OnBlockChanged += (WorldBlock _, WorldBlock __, int x, int y, int z) =>
                invalidatedChunks.Add(new Point(x / RegionSize.X, z / RegionSize.Z));
        }

        public RenderableWorld(int _regionSizeX, int _regionSizeY, int _regionSizeZ, int _regionsCountX, int _regionsCountZ, Vector3 _spawnPos, ContentManager _content)
            : base(_regionSizeX, _regionSizeY, _regionSizeZ, _regionsCountX, _regionsCountZ, _spawnPos)
        {
            Vertices = new VertexPositionColorTexture[_regionsCountX, _regionsCountZ][];
            VertexBuffers = new VertexBuffer[_regionsCountX, _regionsCountZ];

            Blocks.OnBlockChanged += (WorldBlock _, WorldBlock __, int x, int y, int z) =>
            {
                int a = x / RegionSize.X;
                int b = z / RegionSize.Z;
                for (int i = Math.Max(a - 1, 0); i < Math.Min(a + 2, RegionsCount.X); i++)
                    for (int j = Math.Max(b - 1, 0); j <= Math.Min(b + 2, RegionsCount.Y); j++)
                        if(VertexBuffers[a,b]!=null | Vertices[a,b] !=null)
                        invalidatedChunks.Add(new Point(a, b));
            };
        }

        private void LoadVertices(int _regionX, int _regionZ)
        {
            WorldRegion _region = Regions[_regionX, _regionZ];
            List<VertexPositionColorTexture> _vertices = new List<VertexPositionColorTexture>();

            Vertices[_regionX, _regionZ] = VertexGenerator.GenerateVertices(this.Blocks, _regionX * RegionSize.X, 0, _regionZ * RegionSize.Z, RegionSize.X, RegionSize.Y, RegionSize.Z);
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
            lastGameState = _view;

            if (_inputArgs.Events.HasFlag(InputEventList.IncreaseViewDistance))
            {
                viewDistance += 0.1f;
                DebugHelper.Log(viewDistance + "");
            }
            else if (_inputArgs.Events.HasFlag(InputEventList.DecreaseViewDistance))
            {
                viewDistance -= 0.1f;
                DebugHelper.Log(viewDistance + "");
            }


            if (graphicsDevice != null)
            {
                foreach (Point v in invalidatedChunks)
                {
                    Vertices[v.X, v.Y] = null;
                    VertexBuffers[v.X, v.Y]?.Dispose();
                    VertexBuffers[v.X, v.Y] = null;
                }
                invalidatedChunks.Clear();

                for (int x = 0; x < RegionsCount.X; x++)
                    for (int z = 0; z < RegionsCount.Y; z++)
                    {
                        float _distance = Vector2.Distance(new Vector2(_view.CameraCenter.Position.X, _view.CameraCenter.Position.Z), new Vector2((x + 0.5f) * RegionSize.X, (z + 0.5f) * RegionSize.Z));

                        if (_distance < viewDistance && Vertices[x, z] == null)
                        {
                            LoadVertices(x, z);
                        }
                        else if (_distance > viewDistance * 1.2 && Vertices[x, z] != null)
                        {
                            UnloadVertices(x, z);
                        }
                    }
            }

            return base.Update(_view, _flags, _inputArgs, _passedTime);
        }

        public void Initialize(GraphicsDevice _graphicsDevice, ContentManager _contentManager)
        {
            graphicsDevice = _graphicsDevice;
            contentManager = _contentManager;

            blockTexture = contentManager.Load<Texture2D>(BLOCKTEXTURE_NAME);

            foreach (var _blocks in specialBlocks)
                _blocks.Value.Initialize(graphicsDevice, contentManager);
            foreach (var _obj in objects)
                _obj.Initialize(graphicsDevice, contentManager);

            effect = new BasicEffect(graphicsDevice);
            effect.TextureEnabled = true;
            effect.Texture = blockTexture;
            effect.VertexColorEnabled = true;
        }

        protected override void AddSpecialBlock(ISpecialBlock _object)
        {
            base.AddSpecialBlock(_object);

            if (graphicsDevice != null && contentManager != null)
                _object.Initialize(graphicsDevice, contentManager);
        }

        public override void AddObject(IWorldObject _object)
        {
            base.AddObject(_object);

            if (graphicsDevice != null && contentManager != null)
                _object.Initialize(graphicsDevice, contentManager);
        }

        public void Render(GameFlags _flags, Matrix _viewMatrix, Matrix _perspectiveMatrix, GraphicsDevice _grDevice)
        {
            if (lastGameState.Camera != null)
            {
                foreach (var _obj in specialBlocks)
                    if (Vector2.Distance(
                            new Vector2(_obj.Key.X, _obj.Key.Z),
                            new Vector2(lastGameState.Camera.Center.Position.X, lastGameState.Camera.Center.Position.Z))
                        < viewDistance)
                        _obj.Value.Render(_flags, _viewMatrix, _perspectiveMatrix, _grDevice);

                foreach (var _obj in objects)
                    if (Vector2.Distance(
                            new Vector2(_obj.Position.X, _obj.Position.Z),
                            new Vector2(lastGameState.Camera.Position.X, lastGameState.Camera.Position.Z))
                        < viewDistance)
                        _obj.Render(_flags, _viewMatrix, _perspectiveMatrix, _grDevice);
            }

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
                        effect.World = Matrix.CreateTranslation(x * RegionSize.X, 0, z * RegionSize.Z);
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
