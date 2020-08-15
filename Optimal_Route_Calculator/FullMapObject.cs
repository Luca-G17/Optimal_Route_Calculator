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

        private int[] visible_segment_index = new int[2] {2, 2}; 
        public FullMapObject()
        {

        }
        public void SetVisiblePos(Canvas MyCanvas, int row, int col)
        {
            if (visible_segment_index[0] + row > -1 && visible_segment_index[0] + row < 5 && visible_segment_index[1] + col < 3 && visible_segment_index[1] + col > -1)
            {
                if (map_segment_arr[visible_segment_index[0] + row, visible_segment_index[1] + col] != null)
                {
                    visible_segment_index[0] += row;
                    visible_segment_index[1] += col;

                    MapSegmentObject visibleMapSegment = map_segment_arr[visible_segment_index[0], visible_segment_index[1]];
                    MapSegmentObject oldVisibleMapSegment = map_segment_arr[visible_segment_index[0] - row, visible_segment_index[1] - col];
                    visibleMapSegment.SetVisible(true, MyCanvas);
                    oldVisibleMapSegment.SetVisible(false, MyCanvas);


                    visibleMapSegment.ChangeObjectVisibility(MyCanvas);
                    oldVisibleMapSegment.ChangeObjectVisibility(MyCanvas);
                }
            }
        }

        public int[] GetVisibleSegmentIndex
        {
            get { return visible_segment_index; }
            set { visible_segment_index = value; }
        }
        public MapSegmentObject VisibleSegment() 
        {
            return map_segment_arr[visible_segment_index[0], visible_segment_index[1]];
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
