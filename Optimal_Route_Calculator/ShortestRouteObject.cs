using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Xml.Schema;

namespace Optimal_Route_Calculator
{
    class ShortestRouteObject
    {
        // A* Pathfinding:
        // G_cost = Distance from starting node - following the route through parent nodes
        // H_cost = Straight distance from ending node
        // F_cost = G_cost + H_cost
     
        private List<List<double>> route_coords = new List<List<double>>();
        private List<double> start_pos = new List<double>();
        private double[] end_pos = new double[2];
        public ShortestRouteObject(double[] linePos)
        {
            start_pos.Add(linePos[0]);
            start_pos.Add(linePos[1]);
            start_pos.Add(0);
            start_pos.Add(0);

            end_pos[0] = linePos[2];
            end_pos[1] = linePos[3];

            GenerateRoute();
        }
        /// <summary>
        /// 1. Runs A* pathfinding from start_pos to end_pos with a step of 5 pixels between each node
        /// 2. Kills all the unessescary nodes
        /// 3. Repositions any remaining nodes away from land
        /// </summary>
        public void GenerateRoute()
        {
            // Node: 0 = X, 1 = Y, 2 = F_cost, 3 = G_cost, 4 = index of parent
            List<List<double>> closed_nodes = new List<List<double>>();
            List<List<double>> open_nodes = new List<List<double>>();

            open_nodes.Add(start_pos);
            ChooseNodes(open_nodes, closed_nodes);

            // Follows the chain of parent indexes backwards from the last node to the start node to get the route
            int node_index = closed_nodes.Count - 1;
            while (node_index != 0)
            {
                route_coords.Add(closed_nodes[node_index]);
                node_index = (int)closed_nodes[node_index][4];
            }
            route_coords.Reverse();

            // Positions nodes further away from any land detected in range
            //CorrectNodes();

            // Kills any unessecary nodes
            KillNodes();

            

        }
        public void KillNodes()
        {
            // Deletes All nodes with line of sight on the node before, reducing the list of nodes whilst still following the land
            for (int i = 0; i < route_coords.Count - 1; i++)
            {
                List<List<double>> nodes_to_kill = new List<List<double>>();
                for (int f = i + 1; f < route_coords.Count - 1; f++)
                {
                    double[] LinePos = { route_coords[i][0], route_coords[i][1], route_coords[f][0], route_coords[f][1] };
                    if (!LineIntersectsLand(LinePos))
                    {
                        nodes_to_kill.Add(route_coords[f]);
                    }
                }

                foreach (List<double> node in nodes_to_kill)
                {
                    route_coords.Remove(node);
                }
            }

            // Attempts to smooth the course by removing the first non-user generated nodes
            route_coords.RemoveAt(0);
            // route_coords.RemoveAt(route_coords.Count - 1);
        }
        public void CorrectNodes()
        {
            // theta(in radians) = 1 / radius
            foreach (List<double> node in route_coords)
            {
                double radius = 5;
                double[] centre = { node[0], node[1] };

                double angle_step = 1 / radius;
                List<bool> land_circle = new List<bool>();

                // Searches around the circle for land pixels
                for (double angle = 0; angle < 2 * Math.PI; angle += angle_step)
                {
                    // X = r * Cos(theta)
                    // Y = r * Sin(theta)
                    double X_Coord = centre[0] + Math.Cos(angle) * radius;
                    double Y_coord = centre[1] + Math.Sin(angle) * radius;
                    land_circle.Add(MainWindow.PixelIsLand((int)Y_coord, (int)X_Coord));
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
                    node[0] += nodeAdjustements[0];
                    node[1] += nodeAdjustements[1];
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
            average_centre_line = average_centre_line / centreLines.Count;
            double average_line_angle = average_centre_line * angleStep;

            //Adds [node_shift] number of pixels away from the average postion of the land
            double[] adjustments = { Math.Cos(average_line_angle) * -node_shift, Math.Sin(average_line_angle) * -node_shift };
            return adjustments;
        }
        public bool LineIntersectsLand(double[] line_pos)
        {
            // Gradient = Change in Y / Change in X
            double grad_to_node = (line_pos[1] - line_pos[3]) / (line_pos[0] - line_pos[2]);
            double X_dist_to_node = line_pos[0] - line_pos[2];
            double Y_intercept = grad_to_node * -line_pos[0] + line_pos[1];
            int LandPixelCount = 0;

            // If line is near-vertical Y_intercept will default to infinity
            // This ensures that it can never be infinity
            if (double.IsPositiveInfinity(Y_intercept))
            {
                Y_intercept = 99999;
            }
            if (double.IsNegativeInfinity(Y_intercept))
            {
                Y_intercept = -99999;
            }

            // At Grad_to_node = 0, step = 2 | As Grad_to_node tends towards infinity, step = 0.01
            double step = 0.001 + Math.Pow(2, -Math.Abs(grad_to_node));

            // Y = mX + c
            double Y;
            
            if (X_dist_to_node > 0)
            {
                step = step * -1;
            }
            
            for (double X = line_pos[0]; X > line_pos[2] + 1 || X < line_pos[2] - 1; X += step)
            {
                Y = grad_to_node * X + Y_intercept;
                if (MainWindow.PixelIsLand((int)Y, (int)X))
                {
                    LandPixelCount++;
                    // Defines how many pixels of "land" can be between two nodes before they can't see eachother

                    if (LandPixelCount > 3)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void ChooseNodes(List<List<double>> openNodes, List<List<double>> closedNodes)
        {
            while (openNodes.Count > 0)
            {
                List<double> current_node = PickCurrentNode(openNodes, closedNodes);
                // If curren_node != end position then calculate the neighbors
                if (!(current_node[0] <= end_pos[0] + 5 && current_node[0] >= end_pos[0] - 5 && current_node[1] <= end_pos[1] + 5 && current_node[1] >= end_pos[1] - 5))
                {
                    CheckNeighbor(5, 0, current_node, openNodes, closedNodes);
                    CheckNeighbor(0, 5, current_node, openNodes, closedNodes);
                    CheckNeighbor(-5, 0, current_node, openNodes, closedNodes);
                    CheckNeighbor(0, -5, current_node, openNodes, closedNodes);
                    CheckNeighbor(5, 5, current_node, openNodes, closedNodes);
                    CheckNeighbor(-5, 5, current_node, openNodes, closedNodes);
                    CheckNeighbor(5, -5, current_node, openNodes, closedNodes);
                    CheckNeighbor(-5, -5, current_node, openNodes, closedNodes);
                }
                else
                {
                    break;
                }
            }
        }
        public void CheckNeighbor(int x, int y, List<double> currentNode, List<List<double>> openNodes, List<List<double>> closedNodes)
        {
            List<double> neighbor = new List<double>() { currentNode[0] + x, currentNode[1] + y, currentNode[2], currentNode[3] + nodeDistance(x, y), closedNodes.IndexOf(currentNode) };
            // If neighbor is land or is already closed then return
            if (MainWindow.PixelIsLand((int)neighbor[1], (int)neighbor[0]) || GetNode(closedNodes, neighbor) != null)
            {
                return;
            }
            else
            {
                List<double> node = GetNode(openNodes, neighbor);
                double H_cost = MainWindow.Hypotenuse(neighbor[0] - end_pos[0], neighbor[1] - end_pos[1]);
                double G_cost = neighbor[3];
                if (node != null)
                {
                    if (neighbor[3] < node[3])
                    {
                        // If new path to neighbor is shorter then set the shorter path node as parent
                        node[3] = neighbor[3];
                        node[2] = H_cost + G_cost;
                    }
                }
                else
                {
                    neighbor[2] = H_cost + G_cost;
                    openNodes.Add(neighbor);
                }
            }
        }
        public double nodeDistance(int x, int y)
        {
            // If its a diagonal distance = 14 if not then distance = 10
            if (Math.Abs(x) == Math.Abs(y))
            {
                return 7;
            }
            else
            {
                return 5;
            }
        }

        public List<double> GetNode(List<List<double>> list, List<double> item)
        {
            // Checks if the node positions are the same
            foreach (List<double> node in list)
            {
                if (node[0] == item[0] && node[1] == item[1])
                {
                    return node;
                }
            }
            return null;
        }
        public List<double> PickCurrentNode(List<List<double>> openNodes, List<List<double>> closedNodes)
        {
            double max_F_cost = 10000;
            int node_index = 0;
            // Looks for the node with the lowest F_cost
            foreach (List<double> node in openNodes)
            {
                if (node[2] <= max_F_cost)
                {
                    node_index = openNodes.IndexOf(node);
                    max_F_cost = node[2];
                }
            }
            List<double> currentNode = openNodes[node_index];
            openNodes.RemoveAt(node_index);
            closedNodes.Add(currentNode);
            return currentNode;
        }
        public List<List<double>> GetRouteCoords
        {
            get { return route_coords; }
        }

        public void SetRouteCoords(List<double> coords, int index)
        {
            route_coords[index] = coords;
        }
    }
}
