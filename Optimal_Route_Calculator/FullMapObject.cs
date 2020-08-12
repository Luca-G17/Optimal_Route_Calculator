using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Optimal_Route_Calculator
{
    class FullMapObject
    {
        private MapSegmentObject[,] map_segment_arr = new MapSegmentObject[5,3];

        private int[] visible_segment = new int[2] {2, 2}; 
        public FullMapObject()
        {

        }
        public void SetVisiblePos(Canvas MyCanvas, int row, int col, List<Waypoint> waypoints)
        {
            if (visible_segment[0] + row > -1 && visible_segment[0] + row < 5 && visible_segment[1] + col < 3 && visible_segment[1] + col > -1)
            {
                if (map_segment_arr[visible_segment[0] + row, visible_segment[1] + col] != null)
                {
                    visible_segment[0] += row;
                    visible_segment[1] += col;
                    map_segment_arr[visible_segment[0], visible_segment[1]].SetVisible(true, MyCanvas);
                    map_segment_arr[visible_segment[0] - row, visible_segment[1] - col].SetVisible(false, MyCanvas);

                    foreach (Waypoint waypoint in waypoints)
                    {
                        if (waypoint.GetMapSegment[0] == visible_segment[0] && waypoint.GetMapSegment[1] == visible_segment[1])
                        {
                            waypoint.SetVisible(true, MyCanvas);
                        }
                        else if (waypoint.GetMapSegment[0] == visible_segment[0] - row && waypoint.GetMapSegment[1] == visible_segment[1] - col)
                        {
                            waypoint.SetVisible(false, MyCanvas);
                        }
                    }
                }
            }
        }
        public int[] GetVisibleSegment
        {
            get { return visible_segment; }
            set { visible_segment = value; }
        }
        public MapSegmentObject[,] GetMapSegmentArr()
        {
            return map_segment_arr; 
        }
        public void SetMapSegmentArr(int row, int col, MapSegmentObject mapSegment)
        {
            map_segment_arr[row,col] = mapSegment;
        }
    }
}
