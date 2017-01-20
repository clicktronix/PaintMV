using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PaintMV.Shapes
{
    [Serializable]
    internal class Triangle : Shape
    {
        public Triangle(Point startOrigin, int width, int height, Color chosenColor, int shapeSize, bool fillShape, DashStyle penStyle, bool isSelected)
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
            Point[] trianglePoints = new Point[] {
                new Point(StartOrigin.X + Width, StartOrigin.Y + Height), 
                new Point(StartOrigin.X + Width / 2, StartOrigin.Y),
                new Point(StartOrigin.X, StartOrigin.Y + Height),
                new Point(StartOrigin.X + Width, StartOrigin.Y + Height)
            };

            Pen pen = new Pen(ChosenColor, ShapeSize);
            pen.DashStyle = PenStyle;
            if (FilledShape)
            {
                SolidBrush tempBrush = new SolidBrush(ChosenColor);
                g.FillPolygon(tempBrush, trianglePoints);
            }
            else
            {
                g.DrawLine(pen, StartOrigin.X + Width, StartOrigin.Y + Height, StartOrigin.X + Width / 2, StartOrigin.Y);
                g.DrawLine(pen, StartOrigin.X + Width / 2, StartOrigin.Y, StartOrigin.X, StartOrigin.Y + Height);
                g.DrawLine(pen, StartOrigin.X, StartOrigin.Y + Height, StartOrigin.X + Width, StartOrigin.Y + Height);
            }
        }

        public override bool ContainsPoint(Point p)
        {
            Point[] trianglePoints = {
                new Point(StartOrigin.X + Width + 7, StartOrigin.Y + Height + 7), 
                new Point(StartOrigin.X + Width / 2, StartOrigin.Y - 7),
                new Point(StartOrigin.X - 7, StartOrigin.Y + Height + 7)
            };
            GraphicsPath myPath = new GraphicsPath();
            myPath.AddLines(trianglePoints);
            bool pointWithinTriangle = myPath.IsVisible(p);

            if (pointWithinTriangle)
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
            return new Triangle(StartOrigin, Width, Height, ChosenColor, ShapeSize, FilledShape, PenStyle, IsSelected);
        }
    }
}
