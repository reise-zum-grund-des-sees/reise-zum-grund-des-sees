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
    interface IBlockWorld : IReadonlyBlockWorld
    {
        new WorldBlock this[int x, int y, int z] { get; set; }
    }

    interface IReadonlyBlockWorld
    {
        WorldBlock this[int x, int y, int z] { get; }
    }

    class BaseWorld : IUpdateable
    {
        public readonly BlockWrapper Blocks;

        protected readonly WorldRegion[,] Regions;

        protected readonly int RegionSizeX, RegionSizeY, RegionSizeZ;
        protected readonly int RegionsCountX, RegionsCountZ;

        public readonly float SpawnPosX, SpawnPosY, SpawnPosZ;

        public BaseWorld(string _basePath)
        {
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

            Regions = new WorldRegion[RegionsCountX, RegionsCountZ];
            for (int x = 0; x < RegionsCountX; x++)
                for (int z = 0; z < RegionsCountZ; z++)
                    Regions[x, z] =
                        new WorldRegion(
                            File.OpenRead(Path.Combine(_basePath, $"{x}-{z}.region")),
                            RegionSizeX, RegionSizeY, RegionSizeZ);
        }

        public BaseWorld(int _regionSizeX, int _regionSizeY, int _regionSizeZ, int _regionsCountX, int _regionsCountZ, Vector3 _spawnPos)
        {
            Blocks = new BlockWrapper(this);

            RegionsCountX = _regionsCountX;
            RegionsCountZ = _regionsCountZ;
            RegionSizeX = _regionSizeX;
            RegionSizeY = _regionSizeY;
            RegionSizeZ = _regionSizeZ;

            SpawnPosX = _spawnPos.X;
            SpawnPosY = _spawnPos.Y;
            SpawnPosZ = _spawnPos.Z;

            Regions = new WorldRegion[RegionsCountX, RegionsCountZ];

            //for (int x = 0; x < RegionsCountX; x++)
                //for (int z = 0; z < RegionsCountZ; z++)
                    //Regions[x, z] = new WorldRegion();
        }

        public virtual UpdateDelegate Update(GameState.View _view, InputEventArgs _inputArgs, double _passedTime)
        {
            return (ref GameState _gameState) =>
            {

            };
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

        public virtual void Save(string _baseDir)
        {
            string[] lines = new string[2];
            lines[0] = $"region_size = { RegionSizeX }, { RegionSizeY }, { RegionSizeZ }";
            lines[1] = $"regions_count = { RegionsCountX }, { RegionsCountZ }";
            File.WriteAllLines(Path.Combine(_baseDir, "index.world"), lines);

            for (int x = 0; x < RegionsCountX; x++)
                for (int z = 0; z < RegionsCountZ; z++)
                    Regions[x, z].Save(File.OpenWrite(Path.Combine(_baseDir, $"{ x }-{ z }.region")));
        }

        public class BlockWrapper : IBlockWorld
        {
            private readonly BaseWorld world;

            public BlockWrapper(BaseWorld _world)
            {
                world = _world;
            }

            public WorldBlock this[int x, int y, int z]
            {
                get
                {
                    int rx = x / world.RegionSizeX;
                    int rz = z / world.RegionSizeZ;
                    int bx = x - rx * world.RegionSizeX;
                    int by = y;
                    int bz = z - rz * world.RegionSizeZ;

                    if (rx >= world.RegionsCountX | rx < 0 | rz >= world.RegionsCountZ | rz < 0 |
                        bx >= world.RegionSizeX | bx < 0 | by >= world.RegionSizeY | by < 0 | bz >= world.RegionSizeZ | bz < 0)
                        return WorldBlock.Wall;
                    else
                        return world.Regions[rx, rz].Blocks[bx, by, bz];
                }

                set
                {
                    int rx = x / world.RegionSizeX;
                    int rz = z / world.RegionSizeZ;
                    int bx = x - rx * world.RegionSizeX;
                    int by = y;
                    int bz = z - rz * world.RegionSizeZ;

                    world.Regions[rx, rz].Blocks[bx, by, bz] = value;
                }
            }
        }

    }
}
