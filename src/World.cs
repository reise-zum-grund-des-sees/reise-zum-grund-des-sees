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
    class World : IUpdateable
    {
        public readonly BlockWrapper Blocks;
        public readonly VertexPositionColorTexture[,][] Vertices;
        public readonly VertexBuffer[,] VertexBuffers;

        private readonly WorldRegion[,] Regions;
        public readonly int RegionSizeX, RegionSizeY, RegionSizeZ;
        private readonly int RegionsCountX, RegionsCountZ;

        public World(string _basePath)
        {
            Blocks = new BlockWrapper(this);
            string _worldIndexFile = Path.Combine(_basePath, "index.world");

            string[] _lines = File.ReadAllLines(_worldIndexFile);
            foreach (string _line in _lines)
            {
                string[] _items = _line.Split('=', ',', ';');
                for (int i = 0; i < _items.Length; i++) _items[i] = _items[i].Trim();

                switch (_items[0])
                {
                    case "region_size":
                        RegionSizeX = int.Parse(_items[1]);
                        RegionSizeY = int.Parse(_items[2]);
                        RegionSizeZ = int.Parse(_items[3]);
                        break;
                    case "regions_count":
                        RegionsCountX = int.Parse(_items[1]);
                        RegionsCountZ = int.Parse(_items[2]);
                        break;
                    default: throw new FormatException("World file has an invalid format.");

                }
            }

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

        public World(int _regionSizeX, int _regionSizeY, int _regionSizeZ, int _regionsCountX, int _regionsCountZ)
        {
            Blocks = new BlockWrapper(this);
            Vertices = new VertexPositionColorTexture[_regionsCountX, _regionsCountZ][];
            VertexBuffers = new VertexBuffer[_regionsCountX, _regionsCountZ];

            RegionsCountX = _regionsCountX;
            RegionsCountZ = _regionsCountZ;
            RegionSizeX = _regionSizeX;
            RegionSizeY = _regionSizeY;
            RegionSizeZ = _regionSizeZ;

            Regions = new WorldRegion[RegionsCountX, RegionsCountZ];
            for (int x = 0; x < RegionsCountX; x++)
                for (int z = 0; z < RegionsCountZ; z++)
                    Regions[x, z] = new WorldRegion(RegionSizeX, RegionSizeY, RegionSizeZ);

        }

        public void GenerateTestWorld()
        {
            Random rnd = new Random();
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
                    LoadVertices(x, y, _device);
        }

        private void LoadVertices(int _regionX, int _regionZ, GraphicsDevice _device)
        {
            WorldRegion _region = Regions[_regionX, _regionZ];
            List<VertexPositionColorTexture> _vertices = new List<VertexPositionColorTexture>();

            Vertices[_regionX, _regionZ] = VertexGenerator.GenerateVertices(ref _region);
            if (Vertices[_regionX, _regionZ].Length != 0)
            {
                if (VertexBuffers[_regionX, _regionZ] != null)
                    VertexBuffers[_regionX, _regionZ].Dispose();
                VertexBuffers[_regionX, _regionZ] = new VertexBuffer(_device, VertexPositionColorTexture.VertexDeclaration, Vertices[_regionX, _regionZ].Length, BufferUsage.WriteOnly);
                VertexBuffers[_regionX, _regionZ].SetData(Vertices[_regionX, _regionZ]);
            }
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

        public UpdateDelegate Update(GameState.View _view, InputEventArgs _inputArgs, double _passedTime)
        {
            throw new NotImplementedException();
        }
    }
}
