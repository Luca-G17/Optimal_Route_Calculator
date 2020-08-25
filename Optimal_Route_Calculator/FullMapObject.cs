using System.Windows.Controls;

namespace Optimal_Route_Calculator
{
    class FullMapObject
    {
        private MapSegmentObject[] map_segment_arr = new MapSegmentObject[5];

        private int visible_segment_index = 2; 
        public FullMapObject()
        {

        }
        public void SetVisiblePos(Canvas MyCanvas, int index)
        {
            if (visible_segment_index + index > -1 && visible_segment_index + index < 5)
            {
                if (map_segment_arr[visible_segment_index + index] != null)
                {
                    visible_segment_index += index;

                    MapSegmentObject visibleMapSegment = map_segment_arr[visible_segment_index];
                    MapSegmentObject oldVisibleMapSegment = map_segment_arr[visible_segment_index - index];
                    visibleMapSegment.SetVisible(true, MyCanvas);
                    oldVisibleMapSegment.SetVisible(false, MyCanvas);


                    visibleMapSegment.ChangeObjectVisibility(MyCanvas);
                    oldVisibleMapSegment.ChangeObjectVisibility(MyCanvas);
                }
            }
        }

        public int GetVisibleSegmentIndex
        {
            get { return visible_segment_index; }
            set { visible_segment_index = value; }
        }
        public MapSegmentObject VisibleSegment() 
        {
            return map_segment_arr[visible_segment_index];
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
