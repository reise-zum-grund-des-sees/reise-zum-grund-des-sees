using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
    class Octree<T>
    {
        public T Value;

        protected Octree<T>[] subtrees;

        public readonly Vector3 LeftFrontLowerEdge;
        public readonly float Size;
        public readonly int Depth;
        public readonly int MaximumDepth;

        public enum Edges
        {
            FrontLeftUpper,
            FrontLeftLower,
            FrontRightUpper,
            FrontRightLower,
            BackLeftUpper,
            BackLeftLower,
            BackRightUpper,
            BackRightLower
        }

        public Octree<T> this[Edges _edge]
        {
            get
            {
                return subtrees[(int)_edge];
            }
            set
            {
                subtrees[(int)_edge] = value;
            }
        }

        public bool IsLeaf => subtrees == null;

        public Octree(T _value) : this(_value, Vector3.Zero)
        { }

        public Octree(T _value, Vector3 _leftFrontLowerEdge, float _size = 1,
            int _depth = 0, int _maxDepth = 8)
        {
            Value = _value;
            LeftFrontLowerEdge = _leftFrontLowerEdge;
            Size = _size;
            Depth = _depth;
            MaximumDepth = _maxDepth;
        }

        public void ModifyValueAt(Func<T, T> _value, float x, float y, float z)
        {
            if (Depth < MaximumDepth)
            {
                if (IsLeaf)
                {
                    subtrees = new Octree<T>[8];
                    this[Edges.BackLeftLower] = new Octree<T>(Value, LeftFrontLowerEdge + new Vector3(0, 0, Size / 2),
                        Size / 2, Depth + 1, MaximumDepth);
                    this[Edges.BackLeftUpper] = new Octree<T>(Value, LeftFrontLowerEdge + new Vector3(0, Size / 2, Size / 2),
                        Size / 2, Depth + 1, MaximumDepth);
                    this[Edges.BackRightLower] = new Octree<T>(Value, LeftFrontLowerEdge + new Vector3(Size / 2, 0, Size / 2),
                        Size / 2, Depth + 1, MaximumDepth);
                    this[Edges.BackRightUpper] = new Octree<T>(Value, LeftFrontLowerEdge + new Vector3(Size / 2, Size / 2, Size / 2),
                        Size / 2, Depth + 1, MaximumDepth);
                    this[Edges.FrontLeftLower] = new Octree<T>(Value, LeftFrontLowerEdge,
                        Size / 2, Depth + 1, MaximumDepth);
                    this[Edges.FrontLeftUpper] = new Octree<T>(Value, LeftFrontLowerEdge + new Vector3(0, Size / 2, 0),
                        Size / 2, Depth + 1, MaximumDepth);
                    this[Edges.FrontRightLower] = new Octree<T>(Value, LeftFrontLowerEdge + new Vector3(Size / 2, 0, 0),
                        Size / 2, Depth + 1, MaximumDepth);
                    this[Edges.FrontRightUpper] = new Octree<T>(Value, LeftFrontLowerEdge + new Vector3(Size / 2, Size / 2, 0),
                        Size / 2, Depth + 1, MaximumDepth);
                }

                if (x < LeftFrontLowerEdge.X + Size / 2)
                    // Left
                    if (y < LeftFrontLowerEdge.Y + Size / 2)
                        // Lower
                        if (z < LeftFrontLowerEdge.Z + Size / 2)
                            // Front
                            this[Edges.FrontLeftLower].ModifyValueAt(_value, x, y, z);
                        else
                            // Back
                            this[Edges.BackLeftLower].ModifyValueAt(_value, x, y, z);
                    else
                        // Upper
                        if (z < LeftFrontLowerEdge.Z + Size / 2)
                        // Front
                        this[Edges.FrontLeftUpper].ModifyValueAt(_value, x, y, z);
                    else
                        // Back
                        this[Edges.BackLeftUpper].ModifyValueAt(_value, x, y, z);
                else
                    // Right
                    if (y < LeftFrontLowerEdge.Y + Size / 2)
                    // Lower
                    if (z < LeftFrontLowerEdge.Z + Size / 2)
                        // Front
                        this[Edges.FrontRightLower].ModifyValueAt(_value, x, y, z);
                    else
                        // Back
                        this[Edges.BackRightLower].ModifyValueAt(_value, x, y, z);
                else
                        // Upper
                        if (z < LeftFrontLowerEdge.Z + Size / 2)
                    // Front
                    this[Edges.FrontRightUpper].ModifyValueAt(_value, x, y, z);
                else
                    // Back
                    this[Edges.BackRightUpper].ModifyValueAt(_value, x, y, z);

                bool _canMerge = true;
                T _lastValue = subtrees[0].Value;
                for (int i = 0; i < 8 & _canMerge; i++)
                    _canMerge &= subtrees[i].Value.Equals(_lastValue) & subtrees[i].IsLeaf;
                if (_canMerge)
                {
                    subtrees = null;
                    Value = _lastValue;
                }
            }
            else // maximum depth reached
                Value = _value(Value);
        }

        public IEnumerable<KeyValuePair<Vector3, float>> AllNodesWhere(Predicate<T> _predicate)
        {
            var _list = new List<KeyValuePair<Vector3, float>>();
            allNodesWhere(_predicate, _list);
            return _list;
        }
        private void allNodesWhere(Predicate<T> _pred, List<KeyValuePair<Vector3, float>> _list)
        {
            if (IsLeaf)
            {
                if (_pred(Value)) _list.Add(new KeyValuePair<Vector3, float>(
                    this.LeftFrontLowerEdge, this.Size));
            }
            else for (int i = 0; i < 8; i++)
                    subtrees[i].allNodesWhere(_pred, _list);
        }
    }
}
