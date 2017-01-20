using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PaintMV.Shapes
{
    [Serializable]
    internal class Rectangle : Shape
    {
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

        public override bool ContainsPoint(Point p)
        {
            if (p.X > StartOrigin.X - 5 && p.X < StartOrigin.X + Width + 10 && 
                p.Y > StartOrigin.Y - 5 && p.Y < StartOrigin.Y + Height + 10)
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
            return new Rectangle(StartOrigin, Width, Height, ChosenColor, ShapeSize, FilledShape, PenStyle, IsSelected);
        }
    }
}
