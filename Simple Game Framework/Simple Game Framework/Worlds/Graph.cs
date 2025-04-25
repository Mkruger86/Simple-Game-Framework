using System.Collections.Generic;

namespace Simple_Game_Framework.Worlds
{
    public class Graph
    {
        public List<List<Node>> Grid { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Graph"/> class with the specified dimensions.
        /// </summary>
        /// <param name="width">
        /// The width of the graph, representing the number of columns in the grid.
        /// </param>
        /// <param name="height">
        /// The height of the graph, representing the number of rows in the grid.
        /// </param>
        /// <remarks>
        /// This constructor creates a 2D grid of <see cref="Node"/> objects, where each node is initialized as walkable by default.
        /// The grid is represented as a list of rows, with each row containing a list of nodes.
        /// </remarks>
        public Graph(int width, int height)
        {
            Grid = new List<List<Node>>();
            for (int y = 0; y < height; y++)
            {
                var row = new List<Node>();
                for (int x = 0; x < width; x++)
                {                   
                    row.Add(new Node(new System.Numerics.Vector2(x, y), true));
                }
                Grid.Add(row);
            }
        }

        public Node GetNode(int x, int y)
        {
            return Grid[y][x];
        }
    }
}

