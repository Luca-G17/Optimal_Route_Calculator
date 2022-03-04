namespace Optimal_Route_Calculator
{
    /// <summary>
    /// Grid nodes used in A* pathfinding
    /// </summary>
    public class GridNode
    {
        public double F_Cost { get; set; }
        public double G_Cost { get; set; }
        public double H_Cost { get; set; }
        public int Parent { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
}
