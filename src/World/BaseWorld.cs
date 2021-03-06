using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees
{
    interface IBlockWorld : IReadonlyBlockWorld
    {
        new WorldBlock this[int x, int y, int z] { get; set; }
    }

    interface IReadonlyBlockWorld
    {
        Vector3Int Size { get; }
        WorldBlock this[int x, int y, int z] { get; }
    }

    class BaseWorld : IUpdateable, IStateObject
    {
        public readonly BlockWrapper Blocks;

        protected readonly WorldRegion[,] Regions;

        protected readonly Vector3Int RegionSize;
        protected readonly Vector2Int RegionsCount;

        private readonly Octree<int> ContainsWaterTree = new Octree<int>(0);

        public Vector3 SpawnPos;

        public BaseWorld(ConfigFile.ConfigNode _config, string _regionPath)
        {
            Blocks = new BlockWrapper(this);
            Blocks.OnBlockChanged += (_oldBlock, _newBlock, x, y, z) =>
            {
                if (_oldBlock.IsWater() ^ _newBlock.IsWater())
                {
                    ContainsWaterTree.ModifyValueAt(
                        _old => _newBlock.IsWater() ? _old + 1 : _old - 1,
                        (float)x / (RegionSize.X * RegionsCount.X), (float)y / RegionSize.Y, (float)z / (RegionSize.Z * RegionsCount.Y));
                }
            };

            string _region_size = _config["region_size"];
            RegionSize = Vector3Int.Parse(_region_size);

            string _regions_count = _config["regions_count"];
            RegionsCount = Vector2Int.Parse(_regions_count);

            string _spawn_position = _config["spawn"];
            SpawnPos = _spawn_position.ToVector3();

            Regions = new WorldRegion[RegionsCount.X, RegionsCount.Y];
            for (int x = 0; x < RegionsCount.X; x++)
                for (int z = 0; z < RegionsCount.Y; z++)
                    Regions[x, z] =
                        new WorldRegion(
                            File.OpenRead(Path.Combine(_regionPath, $"{x}-{z}.region")),
                            RegionSize.X, RegionSize.Y, RegionSize.Z);
            for (int x = 0; x < Blocks.Size.X; x++)
                for (int z = 0; z < Blocks.Size.Z; z++)
                    for (int y = 0; y < Blocks.Size.Y; y++)
                        if (Blocks[x, y, z].IsWater())
                            ContainsWaterTree.ModifyValueAt(_old => _old + 1, (float)x / Blocks.Size.X,
                                (float)y / Blocks.Size.Y, (float)z / Blocks.Size.Z);
            waterWatch.Start();
        }

        public BaseWorld(int _regionSizeX, int _regionSizeY, int _regionSizeZ, int _regionsCountX, int _regionsCountZ, Vector3 _spawnPos)
        {
            Blocks = new BlockWrapper(this);
            Blocks.OnBlockChanged += (_oldBlock, _newBlock, x, y, z) =>
            {
                if (_oldBlock.IsWater() ^ _newBlock.IsWater())
                {
                    ContainsWaterTree.ModifyValueAt(
                        _old => _newBlock.IsWater() ? _old + 1 : _old - 1,
                        (float)x / (RegionSize.X * RegionsCount.X), (float)y / RegionSize.Y, (float)z / (RegionSize.Z * RegionsCount.Y));
                }
            };

            RegionsCount.X = _regionsCountX;
            RegionsCount.Y = _regionsCountZ;
            RegionSize.X = _regionSizeX;
            RegionSize.Y = _regionSizeY;
            RegionSize.Z = _regionSizeZ;

            SpawnPos.X = _spawnPos.X;
            SpawnPos.Y = _spawnPos.Y;
            SpawnPos.Z = _spawnPos.Z;

            Regions = new WorldRegion[RegionsCount.X, RegionsCount.Y];

            for (int x = 0; x < RegionsCount.X; x++)
                for (int z = 0; z < RegionsCount.Y; z++)
                {
                    Regions[x, z] = new WorldRegion();
                    Regions[x, z].Blocks = new WorldBlock[RegionSize.X, RegionSize.Y, RegionSize.Z];
                }
            waterWatch.Start();
        }

        private Task<IEnumerable<KeyValuePair<Vector3Int, WorldBlock>>> blockWorldUpdateTask;
        private System.Threading.CancellationTokenSource blockWorldUpdateCancelToken;
        //private List<KeyValuePair<Vector3Int, WorldBlock>> blockWorldUpdateChangeList = new List<KeyValuePair<Vector3Int, WorldBlock>>();
        private Stopwatch waterWatch = new Stopwatch();
        public virtual UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            return (ref GameState _gameState) =>
            {
                if (blockWorldUpdateTask != null &&
                    blockWorldUpdateTask.IsCompleted &&
                    blockWorldUpdateTask.Result != null)
                {
                    foreach (var _kvp in blockWorldUpdateTask.Result)
                        Blocks[_kvp.Key.X, _kvp.Key.Y, _kvp.Key.Z] = _kvp.Value;

                    //blockWorldUpdateChangeList = null;
                    blockWorldUpdateTask = null;
                }
                else
                {
                    bool _continue = false;
                    if (blockWorldUpdateTask == null && waterWatch.ElapsedMilliseconds > 500)
                    {
                        waterWatch.Restart();
                        _continue = true;
                    }
                    else if (blockWorldUpdateTask != null && 
                             (blockWorldUpdateTask.IsCanceled ||
                             blockWorldUpdateTask.IsCompleted ||
                             blockWorldUpdateTask.IsFaulted))
                        _continue = true;

                    if (_continue)
                    {
                        blockWorldUpdateCancelToken = new System.Threading.CancellationTokenSource();
                        var _waterChunks = ContainsWaterTree.AllNodesWhere(x => x > 0);
                        blockWorldUpdateTask = new Task<IEnumerable<KeyValuePair<Vector3Int, WorldBlock>>>(() =>
                        {
                            WaterSimmulation _waterSimmulation = new WaterSimmulation();
                            foreach (KeyValuePair<Vector3, float> _item in _waterChunks)
                            {
                                _waterSimmulation.Simmulate(Blocks, blockWorldUpdateCancelToken.Token,
                                    new Vector3Int((int)(_item.Key.X * RegionSize.X * RegionsCount.X),
                                                   (int)(_item.Key.Y * RegionSize.Y),
                                                   (int)(_item.Key.Z * RegionSize.Z * RegionsCount.Y)),
                                    new Vector3Int((int)Math.Ceiling(_item.Value * RegionSize.X * RegionsCount.X),
                                                   (int)Math.Ceiling(_item.Value * RegionSize.Y),
                                                   (int)Math.Ceiling(_item.Value * RegionSize.Z * RegionsCount.Y)));
                            }

                            return _waterSimmulation.Result;
                        });
                        blockWorldUpdateTask.Start();
                    }
                }
            };
        }

        public void GenerateTestWorld()
        {
            Random rnd = new Random();

            for (int x = 0; x < RegionSize.X * RegionsCount.X; x++)
                for (int y = 0; y < RegionSize.Y; y++)
                    for (int z = 0; z < RegionSize.Z * RegionsCount.Y; z++)
                        if (y == 31 && x % RegionSize.X != 0 && z % RegionSize.Z != 0)
                            Blocks[x, y, z] = WorldBlock.Wall;
        }

        public virtual ConfigFile.ConfigNode GetState(ObjectIDMapper _idMapper)
        {
            ConfigFile.ConfigNode f = new ConfigFile.ConfigNode();

            f.Items["region_size"] = RegionSize.ToString();
            f.Items["regions_count"] = RegionsCount.ToString();
            f.Items["spawn"] = SpawnPos.ToNiceString();

            return f;
        }

        public virtual void SaveRegions(string _baseDir)
        {
            for (int x = 0; x < RegionsCount.X; x++)
                for (int z = 0; z < RegionsCount.Y; z++)
                    Regions[x, z].Save(File.OpenWrite(Path.Combine(_baseDir, $"{ x }-{ z }.region")));
        }

        public class BlockWrapper : IBlockWorld
        {
            private readonly BaseWorld world;

            public Vector3Int Size => world.RegionSize * new Vector3Int(world.RegionsCount.X, 1, world.RegionsCount.Y);

            public delegate void OnBlockChangedEventHandler(WorldBlock _oldBlock, WorldBlock _newBlock, int x, int y, int z);
            public event OnBlockChangedEventHandler OnBlockChanged;

            public BlockWrapper(BaseWorld _world)
            {
                world = _world;
            }

            public WorldBlock this[int x, int y, int z]
            {
                get
                {
                    int rx = x / world.RegionSize.X;
                    int rz = z / world.RegionSize.Z;
                    int bx = x - rx * world.RegionSize.X;
                    int by = y;
                    int bz = z - rz * world.RegionSize.Z;

                    if (rx >= world.RegionsCount.X | rx < 0 | rz >= world.RegionsCount.Y | rz < 0 |
                        bx >= world.RegionSize.X | bx < 0 | by >= world.RegionSize.Y | by < 0 | bz >= world.RegionSize.Z | bz < 0)
                        return WorldBlock.Wall;
                    else
                        return world.Regions[rx, rz].Blocks[bx, by, bz];
                }

                set
                {
                    int rx = x / world.RegionSize.X;
                    int rz = z / world.RegionSize.Z;
                    int bx = x - rx * world.RegionSize.X;
                    int by = y;
                    int bz = z - rz * world.RegionSize.Z;

                    world.blockWorldUpdateCancelToken?.Cancel();

                    WorldBlock _oldBlock = world.Regions[rx, rz].Blocks[bx, by, bz];
                    world.Regions[rx, rz].Blocks[bx, by, bz] = value;
                    OnBlockChanged?.Invoke(_oldBlock, value, x, y, z);
                }
            }
        }

    }
}
