﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;

namespace Optimal_Route_Calculator
{

    class MapSegmentObject : MainObject
    {
        private ImageBrush Skin = new ImageBrush();
        private BitmapImage bitmapImage;

        private List<MainObject> waypointsLines = new List<MainObject>(); // Waypoint -- Line -- Waypoint -- Line -- Waypoint
        private ShipObject ship = new ShipObject();

        private string uri;
        private int mapNum;

        private double height;
        private double width;


        public MapSegmentObject(Canvas MyCanvas, int Row, int Col, FullMapObject fullMap)
        {
            uri = ($"pack://application:,,,/Images/Map {Col},{Row}.JPG");
            bitmapImage = new BitmapImage(new Uri(uri));
            Skin.ImageSource = bitmapImage;

            height = bitmapImage.Height;
            width = bitmapImage.Width;

            shape = new Rectangle { Width = width, Height = height, Fill = Skin };

            map_segment_index[0] = Row;
            map_segment_index[1] = Col;
             
            Canvas.SetLeft(shape, GetLeft);
            Canvas.SetTop(shape, GetTop);
            Canvas.SetZIndex(shape, -1);

            fullMap.SetMapSegmentArr(Row, Col, this);
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
            // If its a LineObject it will remove its RouteLines
            if (index % 2 != 0 && index != 0)
            {
                LineObject line = ((LineObject)waypointsLines[index]);
                line.ChangeRouteLineVisibility(MyCanvas);
            }
            waypointsLines[index].SetVisible(false, MyCanvas);
            waypointsLines.RemoveAt(index);
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
        public BitmapImage GetBitmap
        {
            get { return bitmapImage; }
        }
        public int[] GetSegmentIndex
        {
            get { return map_segment_index; }
            set { map_segment_index = value; }
        }

        public string GetUri
        {
            get { return uri; }
        }
        public int MapNum
        {
            get { return mapNum; }
            set { mapNum = value; }
        }
        public double GetHeight
        {
            get { return height; }
            set { height = value; }
        }
        public double GetWidth
        {
            get { return width; }
            set { width = value; }
        }

        public ShipObject GetShip
        {
            get { return ship; }
        }

    }
}
