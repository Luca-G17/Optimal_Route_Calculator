using System;
using System.Collections.Generic;
using System.Windows.Shapes;

namespace Optimal_Route_Calculator
{
    class ShortestRouteObject
    {
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
        public bool LineIntersectsLand(double[] line_pos)
        {
            // Gradient = Change in Y / Change in X
            double grad_to_node = (line_pos[1] - line_pos[3]) / (line_pos[0] - line_pos[2]);
            double X_dist_to_node = line_pos[0] - line_pos[2];
            double Y_intercept = grad_to_node * -line_pos[0] + line_pos[1];
            int LandPixelCount = 0;

            double step;
            double Y;
            // Y = mX + c
            if (X_dist_to_node > 0)
            {
                step = -2;
            }
            else
            {
                step = 2;
            }
            for (double i = line_pos[0]; i > line_pos[2] + 2 || i < line_pos[2] - 2; i += step)
            {
                Y = grad_to_node * i + Y_intercept;
                if(MainWindow.PixelIsLand((int)Y, (int)i))
                {
                    LandPixelCount++;
                }
            }
            if (LandPixelCount > 20)
            {
                return true;
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
                    CheckNeighbor(10, 0, current_node, openNodes, closedNodes);
                    CheckNeighbor(0, 10, current_node, openNodes, closedNodes);
                    CheckNeighbor(-10, 0, current_node, openNodes, closedNodes);
                    CheckNeighbor(0, -10, current_node, openNodes, closedNodes);
                    CheckNeighbor(10, 10, current_node, openNodes, closedNodes);
                    CheckNeighbor(-10, 10, current_node, openNodes, closedNodes);
                    CheckNeighbor(10, -10, current_node, openNodes, closedNodes);
                    CheckNeighbor(-10, -10, current_node, openNodes, closedNodes);
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
                return 14;
            }
            else
            {
                return 10;
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
