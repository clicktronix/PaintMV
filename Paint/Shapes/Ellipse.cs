using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PaintMV.Shapes
{
    /// <summary>
    /// Class creates an ellipse shape
    /// </summary>
    [Serializable]
    internal class Ellipse : IShape
    {
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
        /// Create the instance of class <see cref="Ellipse"/>
        /// </summary>
        /// <param name="startOrigin"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="chosenColor"></param>
        /// <param name="fillColor"></param>
        /// <param name="shapeSize"></param>
        /// <param name="penStyle"></param>
        /// <param name="isSelected"></param>
        public Ellipse(Point startOrigin, int width, int height, Color chosenColor, Color fillColor, int shapeSize, 
            DashStyle penStyle, bool isSelected)
        {
            StartOrigin = startOrigin;
            Width = width;
            Height = height;
            ChosenColor = chosenColor;
            FillColor = fillColor;
            ShapeSize = shapeSize;
            PenStyle = penStyle;
            IsSelected = isSelected;
        }

        /// <summary>
        /// Drawing an ellipse method
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g)
        {
            Pen pen = new Pen(ChosenColor, ShapeSize) {DashStyle = PenStyle};
            SolidBrush tempBrush = new SolidBrush(FillColor);
            g.FillEllipse(tempBrush, StartOrigin.X, StartOrigin.Y, Width, Height);
            g.DrawEllipse(pen, StartOrigin.X, StartOrigin.Y, Width, Height);
        }

        /// <summary>
        /// Method of determining the figure clicked on or not
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool ContainsPoint(Point p)
        {
            GraphicsPath myPath = new GraphicsPath();
            myPath.AddEllipse(StartOrigin.X - 6, StartOrigin.Y - 6, Width + 15, Height + 15);
            bool pointWithinEllipse = myPath.IsVisible(p);

            if (pointWithinEllipse)
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

            bool pointWithinEllipse = myPath.IsVisible(StartOrigin.X + 15, StartOrigin.Y + 15);
            if (pointWithinEllipse)
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
            return new Ellipse(StartOrigin, Width, Height, ChosenColor, FillColor, ShapeSize, PenStyle, IsSelected);
        }
    }
}
