﻿using System;
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
        public static VertexPositionColorTexture[] GenerateVertices(IReadonlyBlockWorld _world, int _startX, int _startY, int _startZ, int _sizeX, int _sizeY, int _sizeZ, bool _bottomOptimizations = true)
        {
            /*int _sizeX = _region.Blocks.GetLength(0);
            int _sizeY = _region.Blocks.GetLength(1);
            int _sizeZ = _region.Blocks.GetLength(2);*/

            VertexPositionColorTexture[] _vertices = new VertexPositionColorTexture[_sizeX * _sizeY * _sizeZ * 6 * 6 / 8];
            int i = 0;

            for (int x = _startX; x < _startX + _sizeX; x++)
                for (int z = _startZ; z < _startZ + _sizeZ; z++)
                    for (int y = _startY; y < _startY + _sizeY; y++)
                    {
                        bool _optimizationFlag = _bottomOptimizations;
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

                            int r;
                            Random rnd = new Random(x * 16 * 16 + y * 16 + z);

                            // FRONT
                            if (_world[x, y, z + 1] != WorldBlock.Wall)
                            {
                                _vertices[i++] = new VertexPositionColorTexture(luf, Color.White, new Vector2(0, 0));
                                _vertices[i++] = new VertexPositionColorTexture(ruf, Color.White, new Vector2(1, 0));
                                _vertices[i++] = new VertexPositionColorTexture(ldf, Color.White, new Vector2(0, 1));
                                _vertices[i++] = new VertexPositionColorTexture(ruf, Color.White, new Vector2(1, 0));
                                _vertices[i++] = new VertexPositionColorTexture(rdf, Color.White, new Vector2(1, 1));
                                _vertices[i++] = new VertexPositionColorTexture(ldf, Color.White, new Vector2(0, 1));
                                for (int j = i - 6; j < i; j++) _vertices[j].TextureCoordinate *= new Vector2(0.5f, 1f / 3);
                                r = rnd.Next(0, _textureOffsets.Length);
                                for (int j = i - 6; j < i; j++) _vertices[j].TextureCoordinate += _textureOffsets[r];
                            }

                            // BACK
                            if (_world[x, y, z - 1] != WorldBlock.Wall)
                            {
                                _vertices[i++] = new VertexPositionColorTexture(lub, Color.White, new Vector2(1, 0));
                                _vertices[i++] = new VertexPositionColorTexture(ldb, Color.White, new Vector2(1, 1));
                                _vertices[i++] = new VertexPositionColorTexture(rub, Color.White, new Vector2(0, 0));
                                _vertices[i++] = new VertexPositionColorTexture(rub, Color.White, new Vector2(0, 0));
                                _vertices[i++] = new VertexPositionColorTexture(ldb, Color.White, new Vector2(1, 1));
                                _vertices[i++] = new VertexPositionColorTexture(rdb, Color.White, new Vector2(0, 1));
                                for (int j = i - 6; j < i; j++) _vertices[j].TextureCoordinate *= new Vector2(0.5f, 1f / 3);
                                r = rnd.Next(0, _textureOffsets.Length);
                                for (int j = i - 6; j < i; j++) _vertices[j].TextureCoordinate += _textureOffsets[r];
                            }

                            // LEFT
                            if (_world[x - 1, y, z] != WorldBlock.Wall)
                            {
                                _vertices[i++] = new VertexPositionColorTexture(lub, Color.White, new Vector2(0, 0));
                                _vertices[i++] = new VertexPositionColorTexture(luf, Color.White, new Vector2(1, 0));
                                _vertices[i++] = new VertexPositionColorTexture(ldb, Color.White, new Vector2(0, 1));
                                _vertices[i++] = new VertexPositionColorTexture(luf, Color.White, new Vector2(1, 0));
                                _vertices[i++] = new VertexPositionColorTexture(ldf, Color.White, new Vector2(1, 1));
                                _vertices[i++] = new VertexPositionColorTexture(ldb, Color.White, new Vector2(0, 1));
                                for (int j = i - 6; j < i; j++) _vertices[j].TextureCoordinate *= new Vector2(0.5f, 1f / 3);
                                r = rnd.Next(0, _textureOffsets.Length);
                                for (int j = i - 6; j < i; j++) _vertices[j].TextureCoordinate += _textureOffsets[r];
                            }

                            // RIGHT
                            if (_world[x + 1, y, z] != WorldBlock.Wall)
                            {
                                _vertices[i++] = new VertexPositionColorTexture(rub, Color.White, new Vector2(1, 0));
                                _vertices[i++] = new VertexPositionColorTexture(rdb, Color.White, new Vector2(1, 1));
                                _vertices[i++] = new VertexPositionColorTexture(ruf, Color.White, new Vector2(0, 0));
                                _vertices[i++] = new VertexPositionColorTexture(ruf, Color.White, new Vector2(0, 0));
                                _vertices[i++] = new VertexPositionColorTexture(rdb, Color.White, new Vector2(1, 1));
                                _vertices[i++] = new VertexPositionColorTexture(rdf, Color.White, new Vector2(0, 1));
                                for (int j = i - 6; j < i; j++) _vertices[j].TextureCoordinate *= new Vector2(0.5f, 1f / 3);
                                r = rnd.Next(0, _textureOffsets.Length);
                                for (int j = i - 6; j < i; j++) _vertices[j].TextureCoordinate += _textureOffsets[r];
                            }

                            // TOP
                            if (_world[x, y + 1, z] != WorldBlock.Wall)
                            {
                                _vertices[i++] = new VertexPositionColorTexture(lub, Color.White, new Vector2(0, 0));
                                _vertices[i++] = new VertexPositionColorTexture(rub, Color.White, new Vector2(1, 0));
                                _vertices[i++] = new VertexPositionColorTexture(luf, Color.White, new Vector2(0, 1));
                                _vertices[i++] = new VertexPositionColorTexture(rub, Color.White, new Vector2(1, 0));
                                _vertices[i++] = new VertexPositionColorTexture(ruf, Color.White, new Vector2(1, 1));
                                _vertices[i++] = new VertexPositionColorTexture(luf, Color.White, new Vector2(0, 1));
                                for (int j = i - 6; j < i; j++) _vertices[j].TextureCoordinate *= new Vector2(0.5f, 1f / 3);
                                r = rnd.Next(0, _textureOffsets.Length);
                                for (int j = i - 6; j < i; j++) _vertices[j].TextureCoordinate += _textureOffsets[r];
                            }

                            // BOTTOM
                            if (!_optimizationFlag && _world[x, y - 1, z] != WorldBlock.Wall)
                            {
                                _vertices[i++] = new VertexPositionColorTexture(ldb, Color.White, new Vector2(0, 1));
                                _vertices[i++] = new VertexPositionColorTexture(ldf, Color.White, new Vector2(0, 0));
                                _vertices[i++] = new VertexPositionColorTexture(rdb, Color.White, new Vector2(1, 1));
                                _vertices[i++] = new VertexPositionColorTexture(rdb, Color.White, new Vector2(1, 1));
                                _vertices[i++] = new VertexPositionColorTexture(ldf, Color.White, new Vector2(0, 0));
                                _vertices[i++] = new VertexPositionColorTexture(rdf, Color.White, new Vector2(1, 0));
                                for (int j = i - 6; j < i; j++) _vertices[j].TextureCoordinate *= new Vector2(0.5f, 1f / 3);
                                r = rnd.Next(0, _textureOffsets.Length);
                                for (int j = i - 6; j < i; j++) _vertices[j].TextureCoordinate += _textureOffsets[r];
                            }

                            if (_optimizationFlag)
                                _optimizationFlag = false;
                        }
                    }

            VertexPositionColorTexture[] _finalArray = new VertexPositionColorTexture[i];
            Array.Copy(_vertices, _finalArray, i);
            return _finalArray;
        }

        /*private static Vector2[] TextureAtlasLookup(WorldBlock _block)
        {
            switch (_block)
            {
                case WorldBlock.Wall:
                    return new Vector2[] {
                        new Vector2()
                    };
            }
        }*/
    }
}