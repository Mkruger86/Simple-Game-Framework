using System.Numerics;

namespace Simple_Game_Framework.Worlds
{
    public class Node
    {
        public Vector2 Position { get; set; }
        public bool Walkable { get; set; }

        public Node(Vector2 position, bool walkable)
        {
            Position = position;
            Walkable = walkable;
        }
    }
}

