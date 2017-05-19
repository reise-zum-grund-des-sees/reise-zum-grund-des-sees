using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees
{
    class World : IUpdateable, IWorld
    {
        public readonly BlockWrapper Blocks;

        private readonly WorldRegion[,] Regions;
        public readonly VertexPositionColorTexture[,][] Vertices;
        public readonly VertexBuffer[,] VertexBuffers;

        public readonly int RegionSizeX, RegionSizeY, RegionSizeZ;
        private readonly int RegionsCountX, RegionsCountZ;

        public readonly float SpawnPosX, SpawnPosY, SpawnPosZ;

        private readonly GraphicsDevice GraphicsDevice;

        public World(string _basePath, GraphicsDevice _device)
        {
            GraphicsDevice = _device;
            Blocks = new BlockWrapper(this);
            string _worldIndexFile = Path.Combine(_basePath, "index.world");

            Dictionary<string, string[]> _worldFile = ConfigFile.Load(_worldIndexFile);

            string[] _region_size = _worldFile["region_size"];
            RegionSizeX = int.Parse(_region_size[0]);
            RegionSizeY = int.Parse(_region_size[1]);
            RegionSizeZ = int.Parse(_region_size[2]);

            string[] _regions_count = _worldFile["regions_count"];
            RegionsCountX = int.Parse(_regions_count[0]);
            RegionsCountZ = int.Parse(_regions_count[1]);

            string[] _spawn_position = _worldFile["spawn"];
            SpawnPosX = int.Parse(_spawn_position[0]);
            SpawnPosY = int.Parse(_spawn_position[1]);
            SpawnPosZ = int.Parse(_spawn_position[2]);

            Vertices = new VertexPositionColorTexture[RegionsCountX, RegionsCountZ][];
            VertexBuffers = new VertexBuffer[RegionsCountX, RegionsCountZ];

            Regions = new WorldRegion[RegionsCountX, RegionsCountZ];
            for (int x = 0; x < RegionsCountX; x++)
                for (int z = 0; z < RegionsCountZ; z++)
                    Regions[x, z] =
                        new WorldRegion(
                            File.OpenRead(Path.Combine(_basePath, $"{x}-{z}.region")),
                            RegionSizeX, RegionSizeY, RegionSizeZ);
        }

        public World(int _regionSizeX, int _regionSizeY, int _regionSizeZ, int _regionsCountX, int _regionsCountZ, Vector3 _spawnPos, GraphicsDevice _device)
        {
            GraphicsDevice = _device;
            Blocks = new BlockWrapper(this);
            Vertices = new VertexPositionColorTexture[_regionsCountX, _regionsCountZ][];
            VertexBuffers = new VertexBuffer[_regionsCountX, _regionsCountZ];

            RegionsCountX = _regionsCountX;
            RegionsCountZ = _regionsCountZ;
            RegionSizeX = _regionSizeX;
            RegionSizeY = _regionSizeY;
            RegionSizeZ = _regionSizeZ;

            SpawnPosX = _spawnPos.X;
            SpawnPosY = _spawnPos.Y;
            SpawnPosZ = _spawnPos.Z;

            Regions = new WorldRegion[RegionsCountX, RegionsCountZ];

            /*for (int x = 0; x < RegionsCountX; x++)
                for (int z = 0; z < RegionsCountZ; z++)
                    Regions[x, z] = new WorldRegion();*/
        }

        public void GenerateTestWorld()
        {
            Random rnd = new Random();

            for (int x = 0; x < RegionsCountX; x++)
                for (int z = 0; z < RegionsCountZ; z++)
                {
                    Regions[x, z] = new WorldRegion();
                    Regions[x, z].Blocks = new WorldBlock[RegionSizeX, RegionSizeY, RegionSizeZ];
                }

            for (int x = 0; x < RegionSizeX * RegionsCountX; x++)
                for (int y = 0; y < RegionSizeY; y++)
                    for (int z = 0; z < RegionSizeZ * RegionsCountZ; z++)
                        if (y == 31)
                            Blocks[x, y, z] = WorldBlock.Wall;
                        else if (y == 32)
                            Blocks[x, y, z] = (rnd.Next(0, 20) == 1) ? WorldBlock.Wall : WorldBlock.None;
        }

        public void Save(string _baseDir)
        {
            string[] lines = new string[2];
            lines[0] = $"region_size = { RegionSizeX }, { RegionSizeY }, { RegionSizeZ }";
            lines[1] = $"regions_count = { RegionsCountX }, { RegionsCountZ }";
            File.WriteAllLines(Path.Combine(_baseDir, "index.world"), lines);

            for (int x = 0; x < RegionsCountX; x++)
                for (int z = 0; z < RegionsCountZ; z++)
                    Regions[x, z].Save(File.OpenWrite(Path.Combine(_baseDir, $"{ x }-{ z }.region")));
        }

        public void GenerateVertices(GraphicsDevice _device)
        {
            for (int x = 0; x < RegionsCountX; x++)
                for (int y = 0; y < RegionsCountZ; y++)
                    LoadVertices(x, y);
        }

        private void LoadVertices(int _regionX, int _regionZ)
        {
            WorldRegion _region = Regions[_regionX, _regionZ];
            List<VertexPositionColorTexture> _vertices = new List<VertexPositionColorTexture>();

            Vertices[_regionX, _regionZ] = VertexGenerator.GenerateVertices(this, _regionX * RegionSizeX, 0, _regionZ * RegionSizeZ, RegionSizeX, RegionSizeY, RegionSizeZ);
            if (Vertices[_regionX, _regionZ].Length != 0)
            {
                if (VertexBuffers[_regionX, _regionZ] != null)
                    VertexBuffers[_regionX, _regionZ].Dispose();
                VertexBuffers[_regionX, _regionZ] = new VertexBuffer(GraphicsDevice, VertexPositionColorTexture.VertexDeclaration, Vertices[_regionX, _regionZ].Length, BufferUsage.WriteOnly);
                VertexBuffers[_regionX, _regionZ].SetData(Vertices[_regionX, _regionZ]);
            }
        }
        private void UnloadVertices(int _regionX, int _regionZ)
        {
            Vertices[_regionX, _regionZ] = null;
            VertexBuffers[_regionX, _regionZ]?.Dispose();
            VertexBuffers[_regionX, _regionZ] = null;
        }

        public class BlockWrapper
        {
            private readonly World w;

            public BlockWrapper(World w)
            {
                this.w = w;
            }

            public WorldBlock this[int x, int y, int z]
            {
                get
                {
                    int rx = x / w.RegionSizeX;
                    int rz = z / w.RegionSizeZ;
                    int bx = x - rx * w.RegionSizeX;
                    int by = y;
                    int bz = z - rz * w.RegionSizeZ;

                    if (rx >= w.RegionsCountX | rx < 0 | rz >= w.RegionsCountZ | rz < 0 |
                        bx >= w.RegionSizeX | bx < 0 | by >= w.RegionSizeY | by < 0 | bz >= w.RegionSizeZ | bz < 0)
                        return WorldBlock.Wall;
                    else
                        return w.Regions[rx, rz].Blocks[bx, by, bz];
                }

                set
                {
                    int rx = x / w.RegionSizeX;
                    int rz = z / w.RegionSizeZ;
                    int bx = x - rx * w.RegionSizeX;
                    int by = y;
                    int bz = z - rz * w.RegionSizeZ;

                    w.Regions[rx, rz].Blocks[bx, by, bz] = value;
                }
            }
        }

        public WorldBlock GetBlock(int x, int y, int z) => Blocks[x, y, z];

        public UpdateDelegate Update(GameState.View _view, InputEventArgs _inputArgs, double _passedTime)
        {
            Vector3 _playerPosition = new Vector3(_view.PlayerX, _view.PlayerY, _view.PlayerZ);
            for (int x = 0; x < RegionsCountX; x++)
                for (int z = 0; z < RegionsCountZ; z++)
                {
                    float _distance = Vector2.Distance(new Vector2(_view.PlayerX, _view.PlayerZ), new Vector2((x + 0.5f) * RegionSizeX, (z + 0.5f) * RegionSizeZ));

                    if (_distance < 20 && Vertices[x, z] == null)
                    {
                        LoadVertices(x, z);
                    }
                    else if (_distance > 20 && Vertices[x, z] != null)
                    {
                        UnloadVertices(x, z);
                    }
                }

            return (ref GameState _gameState) =>
            {

            };
        }
    }
}
