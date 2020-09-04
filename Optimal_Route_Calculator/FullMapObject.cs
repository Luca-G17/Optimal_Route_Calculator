using System.Windows.Controls;

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
