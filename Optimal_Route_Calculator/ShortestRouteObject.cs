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
            ChooseNode(open_nodes, closed_nodes);

            int node_index = closed_nodes.Count - 1;
            while (node_index != 0)
            {
                route_coords.Add(closed_nodes[node_index]);
                node_index = (int)closed_nodes[node_index][4];
            }
            route_coords.Reverse();
        }
        public void ChooseNode(List<List<double>> openNodes, List<List<double>> closedNodes)
        {
            List<double> current_node = PickCurrentNode(openNodes, closedNodes);
            if (!(current_node[0] <= end_pos[0] + 1 && current_node[0] >= end_pos[0] - 1 && current_node[1] <= end_pos[1] + 1 && current_node[1] >= end_pos[0] - 1))
            {
                CheckNeighbor(1, 0, current_node, openNodes, closedNodes);
                CheckNeighbor(0, 1, current_node, openNodes, closedNodes);
                CheckNeighbor(-1, 0, current_node, openNodes, closedNodes);
                CheckNeighbor(0, -1, current_node, openNodes, closedNodes);
                CheckNeighbor(1, 1, current_node, openNodes, closedNodes);
                CheckNeighbor(-1, 1, current_node, openNodes, closedNodes);
                CheckNeighbor(1, -1, current_node, openNodes, closedNodes);
                CheckNeighbor(-1, -1, current_node, openNodes, closedNodes);

                ChooseNode(openNodes, closedNodes);
            }

        }
        public void CheckNeighbor(int x, int y, List<double> currentNode, List<List<double>> openNodes, List<List<double>> closedNodes)
        {
            List<double> neighbor = new List<double>() { currentNode[0] + x, currentNode[1] + y, currentNode[2], currentNode[3] + nodeDistance(x, y), closedNodes.IndexOf(currentNode) };
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
        public int nodeDistance(int x, int y)
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
            double max_F_cost = 10001;
            int node_index = 0;
            foreach (List<double> node in openNodes)
            {
                if (node[2] < max_F_cost)
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
