using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PaintMV.Shapes
{
    [Serializable]
    internal class Polygon : IShape
    {
        public bool DrawPolygon { get; set; }
        public Point StartOrigin { get; set; }
        public Point EndOrigin { get; set; }
        public Point[] PointsArray { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ShapeSize { get; set; }
        public Color ChosenColor { get; set; }
        public Color FillColor { get; set; }
        public bool IsSelected { get; set; }
        public string ShapeName { get; set; }
        public DashStyle PenStyle { get; set; }

        /// <summary>
        /// Create the instance of class <see cref="Polygon"/>
        /// </summary>
        /// <param name="pointsArray"></param>
        /// <param name="chosenColor"></param>
        /// <param name="fillColor"></param>
        /// <param name="shapeSize"></param>
        /// <param name="penStyle"></param>
        /// <param name="isSelected"></param>
        /// <param name="drawPolygon"></param>
        public Polygon(Point[] pointsArray, Color chosenColor, Color fillColor, int shapeSize, DashStyle penStyle, 
            bool isSelected, bool drawPolygon)
        {
            PointsArray = pointsArray;
            ChosenColor = chosenColor;
            FillColor = fillColor;
            ShapeSize = shapeSize;
            PenStyle = penStyle;
            IsSelected = isSelected;
            DrawPolygon = drawPolygon;
            ShapeName = "Polygon";
        }

        /// <summary>
        /// Drawing a line method
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g)
        {
            Pen pen = new Pen(ChosenColor, ShapeSize) {DashStyle = PenStyle};
            SolidBrush tempBrush = new SolidBrush(FillColor);
            if (PointsArray.Length > 1)
            {
                g.DrawLines(pen, PointsArray);
                if (DrawPolygon)
                {
                    g.FillPolygon(tempBrush, PointsArray);
                    g.DrawPolygon(pen, PointsArray);
                }
            }
        }

        /// <summary>
        /// Method of determining the figure clicked on or not
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool ContainsPoint(Point p)
        {
            GraphicsPath myPath = new GraphicsPath();
            myPath.AddLines(PointsArray);
            bool pointWithinPolygon = myPath.IsVisible(p);

            if (pointWithinPolygon)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method of determining whether the figure in the selected area or not
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public bool ContainsSelectedFigure(Point startPoint, Point endPoint)
        {
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle();
            if ((endPoint.Y > startPoint.Y) && (endPoint.X > startPoint.X))
            {
                rect.X = startPoint.X;
                rect.Y = startPoint.Y;
                rect.Height = endPoint.Y - startPoint.Y;
                rect.Width = endPoint.X - startPoint.X;
            }
            else if ((endPoint.Y < startPoint.Y) && (endPoint.X < startPoint.X))
            {
                rect.X = endPoint.X;
                rect.Y = endPoint.Y;
                rect.Height = startPoint.Y - endPoint.Y;
                rect.Width = startPoint.X - endPoint.X;
            }
            else if ((endPoint.Y > startPoint.Y) && (endPoint.X < startPoint.X))
            {
                rect.X = endPoint.X;
                rect.Y = startPoint.Y;
                rect.Height = endPoint.Y - startPoint.Y;
                rect.Width = startPoint.X - endPoint.X;
            }
            else if ((endPoint.Y < startPoint.Y) && (endPoint.X > startPoint.X))
            {
                rect.X = startPoint.X;
                rect.Y = endPoint.Y;
                rect.Height = startPoint.Y - endPoint.Y;
                rect.Width = endPoint.X - startPoint.X;
            }
            GraphicsPath myPath = new GraphicsPath();
            myPath.AddRectangle(rect);

            bool pointWithinPolygon = false;
            for (var i = PointsArray.Length - 1; i > 0; i--)
            {
                pointWithinPolygon = myPath.IsVisible(PointsArray[i]);
            }
            if (pointWithinPolygon)
            {
                IsSelected = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Set flag IsSelected 
        /// </summary>
        /// <param name="isSelected"></param>
        public void SetShapeIsSelected(bool isSelected)
        {
            IsSelected = isSelected;
        }

        /// <summary>
        /// Get flag IsSelected
        /// </summary>
        /// <returns></returns>
        public bool GetShapeIsSelected()
        {
            return IsSelected;
        }

        /// <summary>
        /// Copying the shape method
        /// </summary>
        /// <returns></returns>
        public IShape Clone()
        {
            return new Polygon(PointsArray, ChosenColor, FillColor, ShapeSize, PenStyle, IsSelected, DrawPolygon);
        }
    }
}