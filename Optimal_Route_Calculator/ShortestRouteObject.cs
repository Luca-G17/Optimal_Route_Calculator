using System;
using System.Collections.Generic;
using System.Windows;

namespace Optimal_Route_Calculator
{
    class ShortestRouteObject
    {
        // A* Pathfinding:
        // G_cost = Distance from starting node - following the route through parent nodes
        // H_cost = Straight distance from ending node
        // F_cost = G_cost + H_cost

        private List<GridNode> Route_coords = new List<GridNode>();
        private readonly GridNode start_pos = new GridNode();
        private readonly GridNode end_pos = new GridNode();
        private readonly double step;
        public ShortestRouteObject(double[] linePos, double Step, MainWindow mainWindow)
        {
            start_pos.X = linePos[0];
            start_pos.Y = linePos[1];

            end_pos.X = linePos[2];
            end_pos.Y = linePos[3];

            step = Step;

            GenerateRoute(mainWindow);
        }

        /// <summary>
        /// 1. Runs A* pathfinding from start_pos to end_pos with a step of 5 pixels between each node
        /// 2. Kills all the unessescary nodes
        /// 3. Repositions any remaining nodes away from land 
        /// </summary>
        public void GenerateRoute(MainWindow main_window)
        {
            // Node: 0 = X, 1 = Y, 2 = F_cost, 3 = G_cost, 4 = index of parent
            List<GridNode> closed_nodes = new List<GridNode>();
            List<GridNode> open_nodes = new List<GridNode> { start_pos };

            ChooseNodes(open_nodes, closed_nodes, main_window);

            // Follows the chain of parent indexes backwards from the last node to the start node to get the route
            int node_index = closed_nodes.Count - 1;
            while (node_index != 0)
            {
                Route_coords.Add(closed_nodes[node_index]);
                node_index = (int)closed_nodes[node_index].Parent;
            }
            Route_coords.Reverse();

            // TODO: Fix this - repositions nodes in the wrong directions
            // Positions nodes further away from any land detected in range
            //CorrectNodes();

            // TOOD: Make this process more efficeient
            // Kills any unessecary nodes
            KillNodes(main_window);

        }
        public void KillNodes(MainWindow main_window)
        {
            // Deletes All nodes with line of sight on the node before, reducing the list of nodes whilst still following the land
            for (int i = 0; i < Route_coords.Count - 1; i++)
            {
                List<GridNode> nodes_to_kill = new List<GridNode>();
                for (int f = i + 1; f < Route_coords.Count - 1; f++)
                {
                    double[] LinePos = { Route_coords[i].X, Route_coords[i].Y, Route_coords[f].X, Route_coords[f].Y };
                    if (!main_window.LineIntersectsLand(LinePos))
                    {
                        nodes_to_kill.Add(Route_coords[f]);
                    }
                }

                foreach (GridNode node in nodes_to_kill)
                {
                    Route_coords.Remove(node);
                }
            }

            Route_coords.RemoveAt(0);
            // route_coords.RemoveAt(route_coords.Count - 1);
        }

        public void CorrectNodes(MainWindow main_window)
        {
            // theta(in radians) = 1 / radius
            foreach (GridNode node in Route_coords)
            {
                double radius = 5;
                double[] centre = { node.X, node.Y };

                double angle_step = 1 / radius;
                List<bool> land_circle = new List<bool>();

                // Searches around the circle for land pixels
                for (double angle = 0; angle < 2 * Math.PI; angle += angle_step)
                {
                    // X = r * Cos(theta)
                    // Y = r * Sin(theta)
                    double X_Coord = centre[0] + Math.Cos(angle) * radius;
                    double Y_coord = centre[1] + Math.Sin(angle) * radius;
                    land_circle.Add(main_window.PixelIsLand((int)Y_coord, (int)X_Coord));
                }

                // Seaches for runs of land pixels, adds the index of the centre line of each run to a list
                int run_count = 0;
                List<int> centre_lines = new List<int>();
                foreach (bool pixel in land_circle)
                {
                    if (pixel)
                    {
                        run_count++;
                    }
                    else if (run_count > 0)
                    {
                        centre_lines.Add(land_circle.IndexOf(pixel) - (run_count / 2));
                        run_count = 0;
                    }
                }
                if (centre_lines.Count > 0)
                {
                    double[] nodeAdjustements = MoveNode(centre_lines, angle_step);
                    node.X += nodeAdjustements[0];
                    node.Y += nodeAdjustements[1];
                }
            }
        }
        public double[] MoveNode(List<int> centreLines, double angleStep)
        {
            double node_shift = 20;
            int average_centre_line = 0;
            foreach (int centre_line in centreLines)
            {
                average_centre_line += centre_line;
            }
            average_centre_line /= centreLines.Count;
            double average_line_angle = average_centre_line * angleStep;

            //Adds [node_shift] number of pixels away from the average postion of the land
            double[] adjustments = { Math.Cos(average_line_angle) * -node_shift, Math.Sin(average_line_angle) * -node_shift };
            return adjustments;
        }

        public void ChooseNodes(List<GridNode> openNodes, List<GridNode> closedNodes, MainWindow main_window)
        {
            while (openNodes.Count > 0)
            {
                GridNode current_node = PickCurrentNode(openNodes, closedNodes);
                // If current_node != end position then calculate the neighbors
                if (!(current_node.X <= end_pos.X + step && current_node.X >= end_pos.X - step && current_node.Y <= end_pos.Y + step && current_node.Y >= end_pos.Y - step))
                {
                    CheckNeighbor(step, 0, current_node, openNodes, closedNodes, main_window);
                    CheckNeighbor(0, step, current_node, openNodes, closedNodes, main_window);
                    CheckNeighbor(-step, 0, current_node, openNodes, closedNodes, main_window);
                    CheckNeighbor(0, -step, current_node, openNodes, closedNodes, main_window);
                    CheckNeighbor(step, step, current_node, openNodes, closedNodes, main_window);
                    CheckNeighbor(-step, step, current_node, openNodes, closedNodes, main_window);
                    CheckNeighbor(step, -step, current_node, openNodes, closedNodes, main_window);
                    CheckNeighbor(-step, -step, current_node, openNodes, closedNodes, main_window);
                }
                else
                {
                    break;
                }
            }
        }
        public void CheckNeighbor(double x, double y, GridNode currentNode, List<GridNode> openNodes, List<GridNode> closedNodes, MainWindow main_window)
        {
            GridNode neighbor = new GridNode()
            {
                X = currentNode.X + x,
                Y = currentNode.Y + y,
                F_Cost = currentNode.F_Cost,
                G_Cost = currentNode.G_Cost + NodeDistance(x, y),
                Parent = closedNodes.IndexOf(currentNode)
            };
            // If neighbor is land or is already closed then return

            if (main_window.PixelIsLand((int)neighbor.Y, (int)neighbor.X) || GetNode(closedNodes, neighbor) != null)
            {
                return;
            }
            else
            {
                // If node is already in open nodes, the path from the shorter parent must be taken
                GridNode node = GetNode(openNodes, neighbor);
                double H_cost = MainWindow.Hypotenuse(neighbor.X - end_pos.X, neighbor.Y - end_pos.Y);
                double G_cost = neighbor.G_Cost;
                if (node != null)
                {
                    if (neighbor.G_Cost < node.G_Cost)
                    {
                        // If new path to neighbor is shorter then set the shorter path node as parent
                        node.G_Cost = neighbor.G_Cost;
                        node.F_Cost = H_cost + G_cost;
                        node.Parent = neighbor.Parent;
                    }
                }
                else
                {
                    neighbor.F_Cost = H_cost + G_cost;
                    openNodes.Add(neighbor);
                }
            }
        }
        public double NodeDistance(double x, double y)
        {
            // If its a diagonal distance = 1.4 if not then distance = 1
            if (Math.Abs(x) == Math.Abs(y))
            {
                return 1.4 * step;
            }
            else
            {
                return 1 * step;
            }
        }

        public GridNode GetNode(List<GridNode> list, GridNode item)
        {
            // Checks if the node positions are the same
            foreach (GridNode node in list)
            {
                if (node.X == item.X && node.Y == item.Y)
                {
                    return node;
                }
            }
            return null;
        }
        public GridNode PickCurrentNode(List<GridNode> openNodes, List<GridNode> closedNodes)
        {
            double max_F_cost = 10000;
            int node_index = 0;
            // Looks for the node with the lowest F_cost
            foreach (GridNode node in openNodes)
            {
                if (node.F_Cost <= max_F_cost)
                {
                    node_index = openNodes.IndexOf(node);
                    max_F_cost = node.F_Cost;
                }
            }
            GridNode currentNode = openNodes[node_index];
            openNodes.RemoveAt(node_index);
            closedNodes.Add(currentNode);
            return currentNode;
        }
        public List<GridNode> GetRouteCoords
        {
            get { return Route_coords; }
        }
    }
}
