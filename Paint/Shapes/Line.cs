using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PaintMV.Shapes
{
    [Serializable]
    internal class Line : Shape
    {
        public Line(Point startOrigin, Point endOrigin, int width, int height, Color chosenColor, 
            int shapeSize, DashStyle penStyle, bool isSelected, bool isLine)
        {
            StartOrigin = startOrigin;
            EndOrigin = endOrigin;
            Width = width;
            Height = height;
            ChosenColor = chosenColor;
            ShapeSize = shapeSize;
            PenStyle = penStyle;
            IsSelected = isSelected;
            IsLine = isLine;
        }

        public override void Draw(Graphics g)
        {
            Pen pen = new Pen(ChosenColor, ShapeSize);
            pen.DashStyle = PenStyle;
            g.DrawLine(pen, StartOrigin, EndOrigin);
        }

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

        public override bool ContainsSelectedFigure(Point startPoint, Point endPoint)
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

        public override void SetShapeIsSelected(bool isSelected)
        {
            IsSelected = isSelected;
        }

        public override bool GetShapeIsSelected()
        {
            return IsSelected;
        }

        public override Shape Clone()
        {
            return new Line(StartOrigin, EndOrigin, Width, Height, ChosenColor, ShapeSize, PenStyle, IsSelected, IsLine);
        }
    }
}
