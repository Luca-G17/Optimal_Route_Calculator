<<<<<<< HEAD
﻿using System.Collections.Generic;
using System.Windows.Controls;

namespace Optimal_Route_Calculator
{
    /// <summary>
    /// Acts as a collection for all the map segments
    /// </summary>
    class FullMapObject
    {
        private readonly MapSegmentObject[] map_segment_arr = new MapSegmentObject[5];
        const int MAX_MAP_SEGMENTS = 5;
        public FullMapObject()
        {

        }
        public double SetScalar
        {
            set { GetScalar.Add(value); }
        }
        public List<double> GetScalar { get; } = new List<double>();

        /// <summary>
        /// Changes the currently displayed map segment
        /// </summary>
        /// <param name="MyCanvas"></param>
        /// <param name="index"></param>
        public void SetVisiblePos(Canvas MyCanvas, int index)
        {
            if (GetVisibleSegmentIndex + index > -1 && GetVisibleSegmentIndex + index < MAX_MAP_SEGMENTS)
            {
                if (map_segment_arr[GetVisibleSegmentIndex + index] != null)
                {
                    GetVisibleSegmentIndex += index;
                    // Changes the visibility of next map segment and previous map segment + all its contained objects
                    MapSegmentObject visibleMapSegment = map_segment_arr[GetVisibleSegmentIndex];
                    MapSegmentObject oldVisibleMapSegment = map_segment_arr[GetVisibleSegmentIndex - index];
                    visibleMapSegment.SetVisible(true, MyCanvas);
                    oldVisibleMapSegment.SetVisible(false, MyCanvas);
                }
            }
        }

        public int GetVisibleSegmentIndex { get; set; } = 2;
        /// <summary>
        /// Returns the Currently displayed segment
        /// </summary>
        /// <returns>Displayed Map Segment</returns>
        public MapSegmentObject VisibleSegment()
        {
            return map_segment_arr[GetVisibleSegmentIndex];
        }

        public MapSegmentObject[] GetMapSegmentArr()
        {
            return map_segment_arr;
        }
        public void SetMapSegmentArr(int index, MapSegmentObject mapSegment)
        {
            map_segment_arr[index] = mapSegment;
        }
    }
}
=======
﻿using System.Windows.Controls;

namespace Optimal_Route_Calculator
{
    class FullMapObject
    {
        private readonly MapSegmentObject[] map_segment_arr = new MapSegmentObject[5];

        public FullMapObject()
        {

        }
        public void SetVisiblePos(Canvas MyCanvas, int index)
        {
            if (GetVisibleSegmentIndex + index > -1 && GetVisibleSegmentIndex + index < 5)
            {
                if (map_segment_arr[GetVisibleSegmentIndex + index] != null)
                {
                    GetVisibleSegmentIndex += index;

                    MapSegmentObject visibleMapSegment = map_segment_arr[GetVisibleSegmentIndex];
                    MapSegmentObject oldVisibleMapSegment = map_segment_arr[GetVisibleSegmentIndex - index];
                    visibleMapSegment.SetVisible(true, MyCanvas);
                    oldVisibleMapSegment.SetVisible(false, MyCanvas);
                }
            }
        }

        public int GetVisibleSegmentIndex { get; set; } = 2;
        public MapSegmentObject VisibleSegment()
        {
            return map_segment_arr[GetVisibleSegmentIndex];
        }

        public MapSegmentObject[] GetMapSegmentArr()
        {
            return map_segment_arr;
        }
        public void SetMapSegmentArr(int index, MapSegmentObject mapSegment)
        {
            map_segment_arr[index] = mapSegment;
        }
    }
}
>>>>>>> 3da34f7792296f9183bb3aefb50d77191f829b09
