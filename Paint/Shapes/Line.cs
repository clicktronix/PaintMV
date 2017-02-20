using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PaintMV.Shapes
{
    /// <summary>
    /// Class creates an line shape
    /// </summary>
    [Serializable]
    internal class Line : Shape
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="startOrigin"></param>
        /// <param name="endOrigin"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="chosenColor"></param>
        /// <param name="shapeSize"></param>
        /// <param name="penStyle"></param>
        /// <param name="isSelected"></param>
        /// <param name="isLine"></param>
        public Line(Point startOrigin, Point endOrigin, Color chosenColor, int shapeSize, DashStyle penStyle, 
            bool isSelected, bool isLine, int width, int height)
        {
            StartOrigin = startOrigin;
            EndOrigin = endOrigin;
            ChosenColor = chosenColor;
            ShapeSize = shapeSize;
            PenStyle = penStyle;
            IsSelected = isSelected;
            IsLine = isLine;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Drawing a line method
        /// </summary>
        /// <param name="g"></param>
        public override void Draw(Graphics g)
        {
            Pen pen = new Pen(ChosenColor, ShapeSize) {DashStyle = PenStyle};
            g.DrawLine(pen, StartOrigin, EndOrigin);
        }

        /// <summary>
        /// Method of determining the figure clicked on or not
        /// </summary>
        /// <param name="p"></param>
        public override bool ContainsPoint(Point p)
        {
            GraphicsPath myPath = new GraphicsPath();
            myPath.AddLine(StartOrigin, EndOrigin);
            bool pointWithinLine = myPath.IsOutlineVisible(p, new Pen(ChosenColor, ShapeSize));

            if (pointWithinLine)
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
        public override bool ContainsSelectedFigure(Point startPoint, Point endPoint)
        {
            var rect = new System.Drawing.Rectangle();
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

            bool pointWithinEllipse = myPath.IsVisible(StartOrigin.X, StartOrigin.Y);
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
        public override void SetShapeIsSelected(bool isSelected)
        {
            IsSelected = isSelected;
        }

        /// <summary>
        /// Get flag IsSelected
        /// </summary>
        /// <returns></returns>
        public override bool GetShapeIsSelected()
        {
            return IsSelected;
        }

        /// <summary>
        /// Copying the shape method
        /// </summary>
        /// <returns></returns>
        public override Shape Clone()
        {
            return new Line(StartOrigin, EndOrigin, ChosenColor, ShapeSize, PenStyle, IsSelected, IsLine, Width, Height);
        }
    }
}
