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
        public readonly VertexPositionColor[,][] Vertices;
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

            Vertices = new VertexPositionColor[RegionsCountX, RegionsCountZ][];
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
            Vertices = new VertexPositionColor[_regionsCountX, _regionsCountZ][];
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
                        if (y == 0)
                            Blocks[x, y, z] = WorldBlock.Wall;
                        else if (y == 1)
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
            List<VertexPositionColor> _vertices = new List<VertexPositionColor>();

            for (int x = 0; x < RegionSizeX; x++)
                for (int y = 0; y < RegionSizeY; y++)
                    for (int z = 0; z < RegionSizeZ; z++)
                    {
                        switch (_region.Blocks[x, y, z])
                        {
                            case WorldBlock.Wall:
                                Vector3 _leftDownFront = new Vector3(x, y, z);
                                Vector3 _leftUpFront = new Vector3(x, y + 1, z);
                                Vector3 _rightDownFront = new Vector3(x + 1, y, z);
                                Vector3 _rightUpFront = new Vector3(x + 1, y + 1, z);
                                Vector3 _leftDownBack = new Vector3(x, y, z + 1);
                                Vector3 _leftUpBack = new Vector3(x, y + 1, z + 1);
                                Vector3 _rightDownBack = new Vector3(x + 1, y, z + 1);
                                Vector3 _rightUpBack = new Vector3(x + 1, y + 1, z + 1);

                                VertexPositionColor ldf = new VertexPositionColor(_leftDownFront, Color.Red);
                                VertexPositionColor luf = new VertexPositionColor(_leftUpFront, Color.Blue);
                                VertexPositionColor rdf = new VertexPositionColor(_rightDownFront, Color.Purple);
                                VertexPositionColor ruf = new VertexPositionColor(_rightUpFront, Color.Yellow);
                                VertexPositionColor ldb = new VertexPositionColor(_leftDownBack, Color.Green);
                                VertexPositionColor lub = new VertexPositionColor(_leftUpBack, Color.Orange);
                                VertexPositionColor rdb = new VertexPositionColor(_rightDownBack, Color.Olive);
                                VertexPositionColor rub = new VertexPositionColor(_rightUpBack, Color.Navy);

                                // FRONT
                                _vertices.Add(luf);
                                _vertices.Add(ruf);
                                _vertices.Add(ldf);
                                _vertices.Add(ruf);
                                _vertices.Add(rdf);
                                _vertices.Add(ldf);

                                // BACK
                                _vertices.Add(lub);
                                _vertices.Add(ldb);
                                _vertices.Add(rub);
                                _vertices.Add(rub);
                                _vertices.Add(ldb);
                                _vertices.Add(rdb);

                                // LEFT
                                _vertices.Add(lub);
                                _vertices.Add(luf);
                                _vertices.Add(ldb);
                                _vertices.Add(luf);
                                _vertices.Add(ldf);
                                _vertices.Add(ldb);

                                // RIGHT
                                _vertices.Add(lub);
                                _vertices.Add(ldb);
                                _vertices.Add(luf);
                                _vertices.Add(luf);
                                _vertices.Add(ldb);
                                _vertices.Add(ldf);

                                // TOP
                                _vertices.Add(lub);
                                _vertices.Add(rub);
                                _vertices.Add(luf);
                                _vertices.Add(rub);
                                _vertices.Add(ruf);
                                _vertices.Add(luf);

                                // BOTTOM
                                _vertices.Add(lub);
                                _vertices.Add(luf);
                                _vertices.Add(rub);
                                _vertices.Add(rub);
                                _vertices.Add(luf);
                                _vertices.Add(ruf);
                                break;
                        }
                    }

            Vertices[_regionX, _regionZ] = _vertices.ToArray();
            if (Vertices[_regionX, _regionZ].Length != 0)
            {
                VertexBuffers[_regionX, _regionZ] = new VertexBuffer(_device, VertexPositionColor.VertexDeclaration, Vertices[_regionX, _regionZ].Length, BufferUsage.WriteOnly);
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
