using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PaintMV.Shapes
{
    /// <summary>
    /// Class creates an rectangle shape
    /// </summary>
    [Serializable]
    internal class Rectangle : Shape
    {
        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="startOrigin"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="chosenColor"></param>
        /// <param name="shapeSize"></param>
        /// <param name="fillShape"></param>
        /// <param name="penStyle"></param>
        /// <param name="isSelected"></param>
        public Rectangle(Point startOrigin, int width, int height, Color chosenColor, int shapeSize, bool fillShape, DashStyle penStyle, bool isSelected)
        {
            StartOrigin = startOrigin;
            Width = width;
            Height = height;
            ChosenColor = chosenColor;
            ShapeSize = shapeSize;
            FilledShape = fillShape;
            PenStyle = penStyle;
            IsSelected = isSelected;
        }

        /// <summary>
        /// Drawing a rectangle method
        /// </summary>
        /// <param name="g"></param>
        public override void Draw(Graphics g)
        {
            Pen pen = new Pen(ChosenColor, ShapeSize);
            pen.DashStyle = PenStyle;
            if (FilledShape)
            {
                SolidBrush tempBrush = new SolidBrush(ChosenColor);
                g.FillRectangle(tempBrush, StartOrigin.X, StartOrigin.Y, Width, Height);
            }
            else
            {
                g.DrawRectangle(pen, StartOrigin.X, StartOrigin.Y, Width, Height);
            }
        }

        /// <summary>
        /// Method of determining the figure clicked on or not
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool ContainsPoint(Point p)
        {
            if (p.X > StartOrigin.X - 5 && p.X < StartOrigin.X + Width + 10 && 
                p.Y > StartOrigin.Y - 5 && p.Y < StartOrigin.Y + Height + 10)
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
        public override bool ContainsSelectedFigure(Point startPoint, Point endPoint, Point p)
        {
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle();
            if ((endPoint.Y > startPoint.Y) && (endPoint.X > startPoint.X))
            {
                rect.X = startPoint.X;
                rect.Y = startPoint.Y;
                rect.Height = endPoint.Y - startPoint.Y;
                rect.Width = endPoint.X - startPoint.X;
            }
            else if ((endPoint.Y < p.Y) && (endPoint.X < p.X))
            {
                rect.X = endPoint.X;
                rect.Y = endPoint.Y;
                rect.Height = p.Y - endPoint.Y;
                rect.Width = p.X - endPoint.X;
            }
            else if ((endPoint.Y > p.Y) && (endPoint.X < p.X))
            {
                rect.X = endPoint.X;
                rect.Y = p.Y;
                rect.Height = endPoint.Y - p.Y;
                rect.Width = p.X - endPoint.X;
            }
            else if ((endPoint.Y < p.Y) && (endPoint.X > p.X))
            {
                rect.X = p.X;
                rect.Y = endPoint.Y;
                rect.Height = p.Y - endPoint.Y;
                rect.Width = endPoint.X - p.X;
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
            return new Rectangle(StartOrigin, Width, Height, ChosenColor, ShapeSize, FilledShape, PenStyle, IsSelected);
        }
    }
}
