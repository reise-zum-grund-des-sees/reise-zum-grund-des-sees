﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
	struct GameState
	{
		public readonly Camera Camera;
		public readonly Player Player;
		public readonly World World;

		public GameState(World _world, Player _player, Camera _camera)
		{
			World = _world;
			Player = _player;
			Camera = _camera;
		}

		public struct View
		{
			private GameState baseState;
			public View(GameState s)
			{
				baseState = s;
			}

			public float CamX => baseState.Camera.Position.X;
			public float CamY => baseState.Camera.Position.Y;
			public float CamZ => baseState.Camera.Position.Z;
            public IPositionObject CameraCenter => baseState.Camera.Center;
            public float CamAngle => baseState.Camera.Angle;

            public float PlayerX => baseState.Player.Position.X;
			public float PlayerY => baseState.Player.Position.Y;
			public float PlayerZ => baseState.Player.Position.Z;

            public IReadonlyBlockWorld BlockWorld => baseState.World.Blocks;
		}
	}
}