﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
	class Camera
	{
		public float xPos, yPos, zPos;
		public Matrix CalculateViewMatrix => throw new NotImplementedException();
	}
}