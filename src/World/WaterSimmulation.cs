using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ReiseZumGrundDesSees
{
    class WaterSimmulation
    {
        class Pool
        {
            public float waterLevel => waterLevelSum * 0.25f / waterBlockCount;
            public int waterLevelSum;
            public int waterBlockCount;
            public bool holes = false;
        }

        private Dictionary<Vector3Int, Pool> _waterBlockPools = new Dictionary<Vector3Int, Pool>();
        private Dictionary<Vector3Int, int> _list = new Dictionary<Vector3Int, int>();
        private List<Pool> _poolList = new List<Pool>();

        public IEnumerable<KeyValuePair<Vector3Int, WorldBlock>> Result => _list?.Select(_kvp => new KeyValuePair<Vector3Int, WorldBlock>(_kvp.Key, WorldBlockHelper.GetWaterFromLevel(_kvp.Value)));

        public void Simmulate(IReadonlyBlockWorld _blockWorld, CancellationToken _cancelToken, Vector3Int _start, Vector3Int _length)
        {
            //int[,] _adjacentWaterLevels = new int[7, 7];

            for (int x = _start.X; x < _start.X + _length.X; x++)
                for (int y = _start.Y; y < _start.Y + _length.Y; y++)
                    for (int z = _start.Z; z < _start.Z + _length.Z; z++)
                    {
                        if (_cancelToken.IsCancellationRequested)
                        {
                            _list = null;
                            return;
                        }

                        WorldBlock b = _blockWorld[x, y, z];
                        if (!b.IsWater())
                            continue;

                        Vector3Int _position = new Vector3Int(x, y, z);

                        if (_waterBlockPools.ContainsKey(_position))
                        {
                            continue;
                        }
                        else
                        {
                            // start flood fill to find all blocks in pool
                            Pool _pool = new Pool();
                            _poolList.Add(_pool);
                            _waterBlockPools.Add(_position, _pool);
                            _pool.waterBlockCount = 1;
                            _pool.waterLevelSum = b.GetWaterLevel();

                            WorldBlock _underneath = _blockWorld[x, y - 1, z];
                            if (_underneath == WorldBlock.None ||
                                _underneath == WorldBlock.Water1 ||
                                _underneath == WorldBlock.Water2 ||
                                _underneath == WorldBlock.Water3)
                            {
                                _pool.waterLevelSum -= 4 - _underneath.GetWaterLevel();
                                _list.Add(new Vector3Int(x, y - 1, z), 4);
                            }

                            _floodFill(x, y, z);

                            void _floodFill(int __x, int __y, int __z)
                            {
                                for (int j = -1; j <= 1; j++)
                                    for (int k = -1; k <= 1; k++)
                                    {
                                        if (Math.Abs(j) + Math.Abs(k) > 1.1)
                                            continue;
                                        Vector3Int __position = new Vector3Int(__x + j, __y, __z + k);
                                        if (!_waterBlockPools.ContainsKey(__position))
                                        {
                                            WorldBlock __b = _blockWorld[__position.X, __position.Y, __position.Z];
                                            if (__b.IsWater())
                                            {
                                                _pool.waterBlockCount++;
                                                _pool.waterLevelSum += __b.GetWaterLevel();
                                                _waterBlockPools.Add(__position, _pool);

                                                WorldBlock __underneath = _blockWorld[__x + j, __y - 1, __z + k];
                                                if (__underneath == WorldBlock.None ||
                                                    __underneath == WorldBlock.Water1 ||
                                                    __underneath == WorldBlock.Water2 ||
                                                    __underneath == WorldBlock.Water3)
                                                {
                                                    _pool.holes = true;
                                                    _list.Add(new Vector3Int(__x + j, __y - 1, __z + k), 4);
                                                }
                                                _floodFill(__position.X, __position.Y, __position.Z);
                                            }
                                            else if (__b == WorldBlock.None)
                                            {
                                                _pool.waterBlockCount++;
                                                _waterBlockPools.Add(__position, _pool);
                                            }
                                        }
                                    }
                            }

                            if (_pool.holes)
                                _pool.waterLevelSum = (int)Math.Round(_pool.waterLevel - 0.25f) * _pool.waterBlockCount;
                            // apply pool values
                            foreach (KeyValuePair<Vector3Int, Pool> _kvp in _waterBlockPools)
                            {
                                if (_kvp.Value == _pool)
                                    if (!_list.ContainsKey(_kvp.Key))
                                        _list[_kvp.Key] = (int)Math.Round(_pool.waterLevel * 4f);
                            }
                        }

                        /*
                                                if (_blockWorld[x, y - 1, z] == WorldBlock.Water1 ||
                                                    _blockWorld[x, y - 1, z] == WorldBlock.Water2 ||
                                                    _blockWorld[x, y - 1, z] == WorldBlock.Water3 ||
                                                    _blockWorld[x, y - 1, z] == WorldBlock.None)
                                                {
                                                    if (_blockWorld[x, y - 1, z] != WorldBlock.Water4)
                                                    {
                                                        if (b != WorldBlock.Water4Infinite)
                                                            _list[_position] = Math.Max(0, 10 * (b.GetWaterLevel() - (4 - _blockWorld[x, y - 1, z].GetWaterLevel())));
                                                        _list[new Vector3Int(x, y - 1, z)] = 40;
                                                    }
                                                }
                                                else
                                                {
                                                    int _water1Count = 0;
                                                    for (int v = -1; v <= 1; v++)
                                                        for (int w = -1; w <= 1; w++)
                                                        {
                                                            if (v == 0 && w == 0)
                                                                continue;
                                                            if (_blockWorld[x + v, y - 1, z + w] == WorldBlock.None &&
                                                                _blockWorld[x, y - 1, z].IsWater())
                                                                continue;

                                                            Vector3Int _nxPos = new Vector3Int(x + v, y, z + w);
                                                            switch (b)
                                                            {
                                                                case WorldBlock.Water4Infinite:
                                                                    if (_blockWorld[x + v, y, z + w] == WorldBlock.None ||
                                                                        _blockWorld[x + v, y, z + w] == WorldBlock.Water1 ||
                                                                        _blockWorld[x + v, y, z + w] == WorldBlock.Water2 ||
                                                                        _blockWorld[x + v, y, z + w] == WorldBlock.Water3)
                                                                        _list[_nxPos] = 40;

                                                                    break;
                                                                case WorldBlock.Water4:
                                                                    if (_blockWorld[x + v, y, z + w] == WorldBlock.Water2)
                                                                    {
                                                                        _list[_nxPos] = 30;
                                                                        _list[_position] = 30;
                                                                    }
                                                                    else if (_blockWorld[x + v, y, z + w] == WorldBlock.Water1)
                                                                    {
                                                                        _list[_nxPos] = 20;
                                                                        _list[_position] = 30;
                                                                    }
                                                                    else if (_blockWorld[x + v, y, z + w] == WorldBlock.None)
                                                                    {
                                                                        if (_blockWorld[x + v, y - 1, z + w] != WorldBlock.None ||
                                                                            _blockWorld[x, y - 1, z] != WorldBlock.Water4)
                                                                        {
                                                                            _list[_nxPos] = 20;
                                                                            _list[_position] = 30;
                                                                        }
                                                                    }
                                                                    break;
                                                                case WorldBlock.Water3:
                                                                    if (_blockWorld[x + v, y, z + w] == WorldBlock.Water1)
                                                                    {
                                                                        _list[_nxPos] = 20;
                                                                        _list[_position] = 20;
                                                                    }
                                                                    else if (_blockWorld[x + v, y, z + w] == WorldBlock.None)
                                                                    {
                                                                        if (_blockWorld[x + v, y - 1, z + w] != WorldBlock.None ||
                                                                            _blockWorld[x, y - 1, z] != WorldBlock.Water4)
                                                                        {
                                                                            _list[_nxPos] = 10;
                                                                            _list[_position] = 20;
                                                                        }
                                                                    }
                                                                    break;
                                                                case WorldBlock.Water2:
                                                                    if (_blockWorld[x + v, y, z + w] == WorldBlock.None)
                                                                    {
                                                                        if (_blockWorld[x + v, y - 1, z + w] != WorldBlock.None ||
                                                                            _blockWorld[x, y - 1, z] != WorldBlock.Water4)
                                                                        {
                                                                            _list[_nxPos] = 10;
                                                                            _list[_position] = 10;
                                                                        }
                                                                    }
                                                                    break;
                                                                case WorldBlock.Water1:
                                                                    if (_blockWorld[x + v, y, z + w].IsWater())
                                                                    {
                                                                        _water1Count++;
                                                                    }
                                                                    if (_blockWorld[x + v, y - 1, z + w] == WorldBlock.Water3)
                                                                    {
                                                                        _list[new Vector3Int(x + v, y - 1, z + w)] = 40;
                                                                        _list[_position] = 0;
                                                                    }
                                                                    break;
                                                            }
                                                        }
                                                    if (b == WorldBlock.Water1 && _water1Count < 5)
                                                        _list[new Vector3Int(x, y, z)] = 0;
                                                }*/
                    }
        }
    }
}
