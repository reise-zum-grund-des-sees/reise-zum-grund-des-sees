using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
    struct GameState
    {
        public readonly ICamera Camera;
        public readonly Player Player;
        public readonly RenderableWorld World;
        public readonly CollisionDetector CollisionDetector;

        public readonly List<Enemy> Enemies;
        public readonly List<Geschoss> Geschosse;

        public GameState(RenderableWorld _world, Player _player, Camera _camera)
        {
            World = _world;
            Player = _player;
            Camera = _camera;
            CollisionDetector = new CollisionDetector(_world.Blocks);
            Enemies = new List<Enemy>();
            Geschosse = new List<Geschoss>();
        }

        public void Save(string _baseDir)
        {
            ConfigFile f = new ConfigFile();

            ObjectIDMapper _idMapper = new ObjectIDMapper();
            f.Nodes["world"] = World.GetState(_idMapper);
            f.Nodes["player"] = Player.GetState(_idMapper);


            //Enemy
            ConfigFile.ConfigNode _enemyNode = new ConfigFile.ConfigNode();

            for (int i = 0; i < Enemies.Count; i++)
                _enemyNode.Nodes[i.IdAsString()] = Enemies[i].GetState();

            if (Enemies.Count > 0)
                f.Nodes["enemies"] = _enemyNode;

            f.Write(System.IO.Path.Combine(_baseDir, "state.conf"));
            World.SaveRegions(_baseDir);
        }

        public static GameState Load(string _baseDir)
        {
            ConfigFile _config = ConfigFile.Load(System.IO.Path.Combine(_baseDir, "state.conf"));

            ObjectIDMapper _idMapper = new ObjectIDMapper();
            RenderableWorld w = new RenderableWorld(_config.Nodes["world"], _idMapper, _baseDir);
            Player p = new Player(_config.Nodes["player"]);
            Camera c = new Camera();
            c.CenterOn(p);

            var _gs = new GameState(w, p, c);

            if (_config.Nodes.ContainsKey("enemies"))
                foreach (var _node in _config.Nodes["enemies"].Nodes)
                {
                    Enemy e = new Enemy(_node.Value);
                    _gs.Enemies.Add(e);
                }

            return _gs;
        }

        public struct View
        {
            private GameState baseState;
            public View(GameState s)
            {
                baseState = s;
            }

            public IReadonlyCollisionDetector CollisionDetector => baseState.CollisionDetector;
            public IReadonlyPlayer Player => baseState.Player;
            public IReadonlyCamera Camera => baseState.Camera;

            public float CamX => baseState.Camera.Position.X;
            public float CamY => baseState.Camera.Position.Y;
            public float CamZ => baseState.Camera.Position.Z;
            public IReadonlyPositionObject CameraCenter => baseState.Camera.Center;
            public float CamAngle => baseState.Camera.Azimuth;

            public float PlayerX => baseState.Player.Position.X;
            public float PlayerY => baseState.Player.Position.Y;
            public float PlayerZ => baseState.Player.Position.Z;
            public int Dialog => baseState.Player.Dialog;

            public List<Enemy> Enemies => baseState.Enemies;
            public List<Geschoss> Geschosse => baseState.Geschosse;

            public IReadonlyBlockWorld BlockWorld => baseState.World.Blocks;
            public IReadonlyWorldObjectContainer WorldObjects => baseState.World;
        }
    }
}
