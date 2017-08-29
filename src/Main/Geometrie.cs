using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
    class Geometrie
    {
        public static bool IsChunkInViewRadius(Vector2 _chunkCenter, float _cameraRotation, float _viewAngle, Vector2 _playerPosition)
        {
            double difX = _chunkCenter.X - _playerPosition.X;
            double difY = _chunkCenter.Y - _playerPosition.Y;
            if (Math.Sqrt(difX * difX + difY * difY) < 16)
                return true;
            else
                try
                {
                    double vx = -Math.Sin(-_cameraRotation);
                    double vy = -Math.Cos(-_cameraRotation);

                    double wx = -vy;
                    double wy = vx;

                    double n1 = (_playerPosition.Y + vy * _chunkCenter.X / vx - vy * _playerPosition.X / vx - _chunkCenter.Y) /
                                (wy - vy * wx / vx);
                    double n2 = (_chunkCenter.X + wx * n1 - _playerPosition.X) / vx;

                    double abstCHKvLFP = Math.Sqrt(n1 * n1 * wx * wx + n1 * n1 * wy * wy);
                    double abstLFPvPlayer = Math.Sqrt(n2 * n2 * vx * vx + n2 * n2 * vy * vy);

                    if (n2 < 0)
                        return false;

                    if (abstCHKvLFP < 12)
                        return true;
                    else abstCHKvLFP -= 12;


                    double beta = Math.Atan2(abstCHKvLFP, abstLFPvPlayer);

                    //if (beta < _viewAngle / 2.0)
                    if (beta < _viewAngle / 2.0)
                    {
                        return true;
                    }
                    else return false;
                }
                catch (Exception e)
                {
                    return true;
                }
        }
    }
}
