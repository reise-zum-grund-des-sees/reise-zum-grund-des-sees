using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ReiseZumGrundDesSees
{
    static class WaterSimmulation
    {
        public static IEnumerable<KeyValuePair<Vector3Int, WorldBlock>> Simmulate(IReadonlyBlockWorld _blockWorld, CancellationToken _cancelToken)
        {
            Dictionary<Vector3Int, int> _list = new Dictionary<Vector3Int, int>();

            int[,] _adjacentWaterLevels = new int[7, 7];

            for (int x = 0; x < _blockWorld.Size.X; x++)
                for (int y = 0; y < _blockWorld.Size.Y; y++)
                    for (int z = 0; z < _blockWorld.Size.Z; z++)
                    {
                        if (_cancelToken.IsCancellationRequested)
                            return null;

                        WorldBlock b = _blockWorld[x, y, z];
                        if (!b.IsWater())
                            continue;

                        Vector3Int _position = new Vector3Int(x, y, z);

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
                        }
                    }
            return _list.Select(_kvp => new KeyValuePair<Vector3Int, WorldBlock>(_kvp.Key, WorldBlockHelper.GetWaterFromLevel(_kvp.Value / 10)));
        }
    }
}
