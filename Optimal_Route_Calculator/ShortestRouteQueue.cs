using System.Collections.Generic;

namespace Optimal_Route_Calculator
{
    public class ShortestRouteQueue
    {
        private List<ShortestRouteObject> shortestRouteObjects = new List<ShortestRouteObject>();
        private int rear_ptr = 0;
        private int front_ptr = 0;
        public void Enqueue(ShortestRouteObject shortestRoute)
        {
            rear_ptr++;
            shortestRouteObjects.Add(shortestRoute);
        }
        public ShortestRouteObject Dequeue()
        {
            if (!IsEmpty())
            {
                int tmp_ptr = front_ptr;
                front_ptr++;
                return shortestRouteObjects[tmp_ptr];
            }
            return null;
        }
        public bool IsEmpty()
        {
            if (front_ptr >= rear_ptr)
            {
                return true;
            }
            return false;
        }
        public int Count()
        {
            return rear_ptr - front_ptr;
        }
    }
}
