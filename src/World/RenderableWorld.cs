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
        private readonly VertexPositionTexture[,][] Vertices;
        private readonly VertexBuffer[,] VertexBuffers;

        private BasicEffect effect;
        private GraphicsDevice graphicsDevice;
        private ContentManager contentManager;

        public double viewDistance = 70.0;

        private Texture2D blockTexture;
        private const string BLOCKTEXTURE_NAME = "blocktexture";

        private List<Point> invalidatedChunks = new List<Point>();

        private GameState.View lastGameState;

        public RenderableWorld(ConfigFile.ConfigNode _config, ObjectIDMapper _idMapper, string _baseDir) : base(_config, _idMapper, _baseDir)
        {
            Vertices = new VertexPositionTexture[RegionsCount.X, RegionsCount.Y][];
            VertexBuffers = new VertexBuffer[RegionsCount.X, RegionsCount.Y];

            Blocks.OnBlockChanged += (WorldBlock _, WorldBlock __, int x, int y, int z) =>
            {
                if (_ != __)
                {
                    int a = x / RegionSize.X;
                    int b = z / RegionSize.Z;
                    for (int i = Math.Max(a - 1, 0); i < Math.Min(a + 2, RegionsCount.X); i++)
                        for (int j = Math.Max(b - 1, 0); j < Math.Min(b + 2, RegionsCount.Y); j++)
                            if ((a == i || b == j) && (VertexBuffers[i, j] != null | Vertices[i, j] != null))
                                invalidatedChunks.Add(new Point(i, j));
                }
            };
        }

        public RenderableWorld(int _regionSizeX, int _regionSizeY, int _regionSizeZ, int _regionsCountX, int _regionsCountZ, Vector3 _spawnPos, ContentManager _content)
            : base(_regionSizeX, _regionSizeY, _regionSizeZ, _regionsCountX, _regionsCountZ, _spawnPos)
        {
            Vertices = new VertexPositionTexture[_regionsCountX, _regionsCountZ][];
            VertexBuffers = new VertexBuffer[_regionsCountX, _regionsCountZ];

            Blocks.OnBlockChanged += (WorldBlock _, WorldBlock __, int x, int y, int z) =>
            {
                if (_ != __)
                {
                    int a = x / RegionSize.X;
                    int b = z / RegionSize.Z;
                    for (int i = Math.Max(a - 1, 0); i < Math.Min(a + 2, RegionsCount.X); i++)
                        for (int j = Math.Max(b - 1, 0); j < Math.Min(b + 2, RegionsCount.Y); j++)
                            if ((a == i || b == j) && (VertexBuffers[i, j] != null | Vertices[i, j] != null))
                                invalidatedChunks.Add(new Point(i, j));
                }
            };
        }

        private void LoadVertices(int _regionX, int _regionZ)
        {
            WorldRegion _region = Regions[_regionX, _regionZ];
            //List<VertexPositionTexture> _vertices = new List<VertexPositionTexture>();

            Vertices[_regionX, _regionZ] = VertexGenerator.GenerateVertices(this.Blocks, _regionX * RegionSize.X, 0, _regionZ * RegionSize.Z, RegionSize.X, RegionSize.Y, RegionSize.Z);
            if (Vertices[_regionX, _regionZ].Length != 0)
            {
                if (VertexBuffers[_regionX, _regionZ] != null)
                    VertexBuffers[_regionX, _regionZ].Dispose();
                VertexBuffers[_regionX, _regionZ] = new VertexBuffer(graphicsDevice, VertexPositionTexture.VertexDeclaration, Vertices[_regionX, _regionZ].Length, BufferUsage.WriteOnly);
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

            blockTexture = contentManager.Load<Texture2D>(ContentRessources.TEXTURE_BLOCKS);

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

        public void Render(GameFlags _flags, IEffect _effect, GraphicsDevice _grDevice)
        {
            DebugHelper.Information.RenderedWorldChunks = 0;
            DebugHelper.Information.RenderedWorldVertices = 0;

            if (lastGameState.Camera != null)
            {
                int maxX = Vertices.GetLength(0);
                int maxZ = Vertices.GetLength(1);

                _effect.Texture = blockTexture;
                _effect.VertexFormat = VertexFormat.World;

                List<Vector2Int> renderList = new List<Vector2Int>(256);

                DebugHelper.Information.CameraPosition = lastGameState.Camera.Position;

                for (int x = 0; x < maxX; x++)
                    for (int z = 0; z < maxZ; z++)
                    {
                        if (Vertices[x, z]?.Length > 0 && Geometrie.IsChunkInViewRadius(new Vector2(x * 16 + 8, z * 16 + 8),
                            lastGameState.Camera.Azimuth, MathHelper.PiOver2, new Vector2(lastGameState.Camera.Position.X, lastGameState.Camera.Position.Z)))
                        {
                            DebugHelper.Information.RenderedWorldChunks++;
                            DebugHelper.Information.RenderedWorldVertices += (uint)(Vertices[x, z]?.Length ?? 0);

                            renderList.Add(new Vector2Int(x, z));
                        }
                    }

                renderList.Sort(new Comparison<Vector2Int>((v1, v2) =>
                {
                    int v1x = v1.X * 16 - (int)lastGameState.Player.Position.X;
                    int v1y = v1.Y * 16 - (int)lastGameState.Player.Position.Z;
                    int v2x = v2.X * 16 - (int)lastGameState.Player.Position.X;
                    int v2y = v2.Y * 16 - (int)lastGameState.Player.Position.Z;

                    int _result = v1x * v1x + v1y * v1y - v2x * v2x - v2y * v2y;
                    return _result;
                }));

                foreach (Vector2Int v in renderList)
                {
                    int x = v.X;
                    int z = v.Y;

                    _effect.WorldMatrix = Matrix.CreateTranslation(x * RegionSize.X, 0, z * RegionSize.Z);

                    graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                    graphicsDevice.SetVertexBuffer(VertexBuffers[x, z]);

                    Effect _baseEffect = _effect.Effect;
                    foreach (EffectPass pass in _baseEffect.CurrentTechnique.Passes)
                        pass.Apply();

                    graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, Vertices[x, z].Length / 3);
                }
            }
        }
    }
}
