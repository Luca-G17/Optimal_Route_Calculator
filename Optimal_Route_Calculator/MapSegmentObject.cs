using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Optimal_Route_Calculator
{

    class MapSegmentObject : MainObject
    {
        private readonly ImageBrush Skin = new ImageBrush();
        private readonly BitmapImage bitmapImage;

        private readonly List<MainObject> waypointsLines = new List<MainObject>(); // Waypoint -- Line -- Waypoint -- Line -- Waypoint

        private readonly string uri;

        public MapSegmentObject(int index, FullMapObject fullMap, double distanceScalar)
        {
            uri = ($"pack://application:,,,/Images/Map{index}.JPG");
            bitmapImage = new BitmapImage(new Uri(uri));
            Skin.ImageSource = bitmapImage;

            GetHeight = bitmapImage.Height;
            GetWidth = bitmapImage.Width;

            shape = new Rectangle { Width = GetWidth, Height = GetHeight, Fill = Skin };

            map_segment_index = index;
            GetScalar = distanceScalar;

            Canvas.SetLeft(shape, GetLeft);
            Canvas.SetTop(shape, GetTop);
            Panel.SetZIndex(shape, -1);

            fullMap.SetMapSegmentArr(index, this);
        }
        public List<MainObject> GetWaypointsAndLines()
        {
            return waypointsLines;
        }
        public void AddWaypointOrLine(int index, MainObject mainObject)
        {
            waypointsLines.Insert(index, mainObject);
        }
        public void DelWaypointOrLine(int index, Canvas MyCanvas)
        {
            waypointsLines[index].SetVisible(false, MyCanvas);
            waypointsLines.RemoveAt(index);
        }
        public void DelRouteLines(Canvas MyCanvas)
        {
            // If its a LineObject it will remove its RouteLines
            for (int i = 0; i < waypointsLines.Count - 1; i++)
            {
                if (i % 2 != 0)
                {
                    ((LineObject)waypointsLines[i]).ChangeRouteLineVisibility(MyCanvas);
                    ((LineObject)waypointsLines[i]).KillRouteLines();
                }
            }

        }
        public override void SetVisible(bool Visable, Canvas MyCanvas)
        {
            visible = Visable;
            if (visible)
            {
                MyCanvas.Children.Add(shape);
            }
            else
            {
                MyCanvas.Children.Remove(shape);
            }
            ChangeObjectVisibility(MyCanvas);
        }
        public void ChangeObjectVisibility(Canvas MyCanvas)
        {
            for (int i = 0; i < waypointsLines.Count(); i++)
            {
                MainObject mainObject = waypointsLines[i];
                mainObject.SetVisible(!mainObject.GetVisible(), MyCanvas);

                // If its a LineObject it will reverse the lines' route lines' visibility
                if (i % 2 != 0)
                {
                    ((LineObject)mainObject).ChangeRouteLineVisibility(MyCanvas);
                }
            }
        }
        public UIElement GetRectangle
        {
            get { return shape; }
        }
        public double GetHeight { get; }
        public double GetWidth { get; }
        public ShipObject GetShip { get; } = new ShipObject();
        public double GetScalar { get; }
    }
}
