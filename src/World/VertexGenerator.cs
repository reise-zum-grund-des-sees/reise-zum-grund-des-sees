using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees
{
    class VertexGenerator
    {
        public static VertexPositionTexture[] GenerateVertices(IReadonlyBlockWorld _world, int _startX, int _startY, int _startZ, int _sizeX, int _sizeY, int _sizeZ)
        {
            /*int _sizeX = _region.Blocks.GetLength(0);
            int _sizeY = _region.Blocks.GetLength(1);
            int _sizeZ = _region.Blocks.GetLength(2);*/

            VertexPositionTexture[] _vertices = new VertexPositionTexture[_sizeX * _sizeY * _sizeZ * 6 * 6 / 8];
            int i = 0;

            for (int x = _startX; x < _startX + _sizeX; x++)
                for (int z = _startZ; z < _startZ + _sizeZ; z++)
                    for (int y = _startY; y < _startY + _sizeY; y++)
                    {
                        WorldBlock b = _world[x, y, z];
                        if (b.IsVisible())
                        {
                            Vector3 _bounds = b.GetBounds();
                            float _xPos = x - _startX;
                            float _yPos = y - _startY;
                            float _zPos = z - _startZ;
                            Vector3 ldb = new Vector3(_xPos, _yPos, _zPos);
                            Vector3 lub = new Vector3(_xPos, _yPos + _bounds.Y, _zPos);
                            Vector3 rdb = new Vector3(_xPos + _bounds.X, _yPos, _zPos);
                            Vector3 rub = new Vector3(_xPos + _bounds.X, _yPos + _bounds.Y, _zPos);
                            Vector3 ldf = new Vector3(_xPos, _yPos, _zPos + _bounds.Z);
                            Vector3 luf = new Vector3(_xPos, _yPos + _bounds.Y, _zPos + _bounds.Z);
                            Vector3 rdf = new Vector3(_xPos + _bounds.X, _yPos, _zPos + _bounds.Z);
                            Vector3 ruf = new Vector3(_xPos + _bounds.X, _yPos + _bounds.Y, _zPos + _bounds.Z);

                            Vector2[] _textureOffsets = b.GetTextureOffsets();

                            Random rnd = new Random(x * 16 * 16 + y * 16 + z);

                            // FRONT
                            if (!_world[x, y, z + 1].IsFullBlock())
                            {
                                int r = rnd.Next(0, 5);
                                _vertices[i++] = new VertexPositionTexture(luf, texture(b, 0, r));
                                _vertices[i++] = new VertexPositionTexture(ruf, texture(b, 1, r));
                                _vertices[i++] = new VertexPositionTexture(ldf, texture(b, 2, r));
                                _vertices[i++] = new VertexPositionTexture(ruf, texture(b, 1, r));
                                _vertices[i++] = new VertexPositionTexture(rdf, texture(b, 3, r));
                                _vertices[i++] = new VertexPositionTexture(ldf, texture(b, 2, r));
                            }
                            else rnd.Next();

                            // BACK
                            if (!_world[x, y, z - 1].IsFullBlock())
                            {
                                int r = rnd.Next(0, 5);
                                _vertices[i++] = new VertexPositionTexture(lub, texture(b, 1, r));
                                _vertices[i++] = new VertexPositionTexture(ldb, texture(b, 3, r));
                                _vertices[i++] = new VertexPositionTexture(rub, texture(b, 0, r));
                                _vertices[i++] = new VertexPositionTexture(rub, texture(b, 0, r));
                                _vertices[i++] = new VertexPositionTexture(ldb, texture(b, 3, r));
                                _vertices[i++] = new VertexPositionTexture(rdb, texture(b, 2, r));
                            }
                            else rnd.Next();

                            // LEFT
                            if (!_world[x - 1, y, z].IsFullBlock())
                            {
                                int r = rnd.Next(0, 5);
                                _vertices[i++] = new VertexPositionTexture(lub, texture(b, 0, r));
                                _vertices[i++] = new VertexPositionTexture(luf, texture(b, 1, r));
                                _vertices[i++] = new VertexPositionTexture(ldb, texture(b, 2, r));
                                _vertices[i++] = new VertexPositionTexture(luf, texture(b, 1, r));
                                _vertices[i++] = new VertexPositionTexture(ldf, texture(b, 3, r));
                                _vertices[i++] = new VertexPositionTexture(ldb, texture(b, 2, r));
                            }
                            else rnd.Next();

                            // RIGHT
                            if (!_world[x + 1, y, z].IsFullBlock())
                            {
                                int r = rnd.Next(0, 5);
                                _vertices[i++] = new VertexPositionTexture(rub, texture(b, 1, r));
                                _vertices[i++] = new VertexPositionTexture(rdb, texture(b, 3, r));
                                _vertices[i++] = new VertexPositionTexture(ruf, texture(b, 0, r));
                                _vertices[i++] = new VertexPositionTexture(ruf, texture(b, 0, r));
                                _vertices[i++] = new VertexPositionTexture(rdb, texture(b, 3, r));
                                _vertices[i++] = new VertexPositionTexture(rdf, texture(b, 2, r));
                            }
                            else rnd.Next();

                            // TOP
                            if (!(b.IsFullBlock() && _world[x, y + 1, z].IsFullBlock()))
                            {
                                int r = rnd.Next(0, 5);
                                _vertices[i++] = new VertexPositionTexture(lub, texture(b, 0, r));
                                _vertices[i++] = new VertexPositionTexture(rub, texture(b, 1, r));
                                _vertices[i++] = new VertexPositionTexture(luf, texture(b, 2, r));
                                _vertices[i++] = new VertexPositionTexture(rub, texture(b, 1, r));
                                _vertices[i++] = new VertexPositionTexture(ruf, texture(b, 3, r));
                                _vertices[i++] = new VertexPositionTexture(luf, texture(b, 2, r));
                            }
                            else rnd.Next();

                            // BOTTOM
                            if (!_world[x, y - 1, z].IsFullBlock())
                            {
                                int r = rnd.Next(0, 5);
                                _vertices[i++] = new VertexPositionTexture(ldb, texture(b, 2, r));
                                _vertices[i++] = new VertexPositionTexture(ldf, texture(b, 0, r));
                                _vertices[i++] = new VertexPositionTexture(rdb, texture(b, 3, r));
                                _vertices[i++] = new VertexPositionTexture(rdb, texture(b, 3, r));
                                _vertices[i++] = new VertexPositionTexture(ldf, texture(b, 0, r));
                                _vertices[i++] = new VertexPositionTexture(rdf, texture(b, 1, r));
                            }
                            else rnd.Next();
                        }
                    }

            VertexPositionTexture[] _finalArray = new VertexPositionTexture[i];
            Array.Copy(_vertices, _finalArray, i);
            return _finalArray;
        }

        private static Vector2[,] vertices = new Vector2[4, 5]
        {
            { new Vector2(0, 0), new Vector2(1 / 4f, 0), new Vector2(2 / 4f, 0), new Vector2(3 / 4f, 0), new Vector2(1f, 0) },
            { new Vector2(0, 1 / 3f), new Vector2(1 / 4f, 1 / 3f), new Vector2(2 / 4f, 1 / 3f), new Vector2(3 / 4f, 1 / 3f), new Vector2(1f, 1 / 3f) },
            { new Vector2(0, 2 / 3f), new Vector2(1 / 4f, 2 / 3f), new Vector2(2 / 4f, 2 / 3f), new Vector2(3 / 4f, 2 / 3f), new Vector2(1f, 2 / 3f) },
            { new Vector2(0, 3 / 3f), new Vector2(1 / 4f, 3 / 3f), new Vector2(2 / 4f, 3 / 3f), new Vector2(3 / 4f, 3 / 3f), new Vector2(1f, 3 / 3f) }
        };
        private static Vector2 texture(WorldBlock _block, int _position, int _randomness)
        {
            switch (_block)
            {
                case WorldBlock.Wall:
                    switch (_randomness)
                    {
                        case 0:
                            switch (_position)
                            {
                                case 0: return vertices[0, 0];
                                case 1: return vertices[0, 1];
                                case 2: return vertices[1, 0];
                                case 3: return vertices[1, 1];
                            }
                            break;
                        case 1:
                            switch (_position)
                            {
                                case 0: return vertices[1, 0];
                                case 1: return vertices[1, 1];
                                case 2: return vertices[2, 0];
                                case 3: return vertices[2, 1];
                            }
                            break;
                        case 2:
                            switch (_position)
                            {
                                case 0: return vertices[2, 0];
                                case 1: return vertices[2, 1];
                                case 2: return vertices[3, 0];
                                case 3: return vertices[3, 1];
                            }
                            break;
                        case 3:
                            switch (_position)
                            {
                                case 0: return vertices[0, 1];
                                case 1: return vertices[0, 2] - new Vector2(0.001f, 0);
                                case 2: return vertices[1, 1];
                                case 3: return vertices[1, 2] - new Vector2(0.001f, 0);
                            }
                            break;
                        case 4:
                            switch (_position)
                            {
                                case 0: return vertices[1, 1];
                                case 1: return vertices[1, 2] - new Vector2(0.001f, 0);
                                case 2: return vertices[2, 1];
                                case 3: return vertices[2, 2] - new Vector2(0.001f, 0);
                            }
                            break;
                        case 5:
                            switch (_position)
                            {
                                case 0: return vertices[2, 1];
                                case 1: return vertices[2, 2] - new Vector2(0.001f, 0);
                                case 2: return vertices[3, 1];
                                case 3: return vertices[3, 2] - new Vector2(0.001f, 0);
                            }
                            break;
                    }
                    break;

                case WorldBlock.Water1:
                case WorldBlock.Water2:
                case WorldBlock.Water3:
                case WorldBlock.Water4:
                case WorldBlock.Water4Infinite:
                    
                    switch (_randomness)
                    {
                        case 0:
                            switch (_position)
                            {
                                case 0: return vertices[0, 2];
                                case 1: return vertices[0, 3];
                                case 2: return vertices[1, 2];
                                case 3: return vertices[1, 3];
                            }
                            break;
                        case 1:
                            switch (_position)
                            {
                                case 0: return vertices[1, 2];
                                case 1: return vertices[1, 3];
                                case 2: return vertices[2, 2];
                                case 3: return vertices[2, 3];
                            }
                            break;
                        case 2:
                            switch (_position)
                            {
                                case 0: return vertices[2, 2];
                                case 1: return vertices[2, 3];
                                case 2: return vertices[3, 2];
                                case 3: return vertices[3, 3];
                            }
                            break;
                        case 3:
                            switch (_position)
                            {
                                case 0: return vertices[0, 2];
                                case 1: return vertices[0, 3];
                                case 2: return vertices[1, 2];
                                case 3: return vertices[1, 3];
                            }
                            break;
                        case 4:
                            switch (_position)
                            {
                                case 0: return vertices[1, 3];
                                case 1: return vertices[1, 4];
                                case 2: return vertices[2, 3];
                                case 3: return vertices[2, 4];
                            }
                            break;
                        case 5:
                            switch (_position)
                            {
                                case 0: return vertices[2, 3];
                                case 1: return vertices[2, 4];
                                case 2: return vertices[3, 3];
                                case 3: return vertices[3, 4];
                            }
                            break;
                    }
                    break;

            }

            return Vector2.Zero;
        }
    }
}
