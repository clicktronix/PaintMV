using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PaintMV.Shapes
{
    [Serializable]
    internal class Ellipse : Shape
    {
        public Point MoveOriginStart { get; }

        public Ellipse(Point startOrigin, int width, int height, Color chosenColor, int shapeSize, bool fillShape, DashStyle penStyle)
        {
            StartOrigin = startOrigin;
            Width = width;
            Height = height;
            ChosenColor = chosenColor;
            ShapeSize = shapeSize;
            MoveOriginStart = startOrigin;
            FilledShape = fillShape;
            PenStyle = penStyle;
        }

        public override void Draw(Graphics g)
        {
            if (FilledShape)
            {
                SolidBrush tempBrush = new SolidBrush(ChosenColor);
                g.FillEllipse(tempBrush, StartOrigin.X, StartOrigin.Y, Width, Height);
            }
            else
            {
                Pen pen = new Pen(ChosenColor, ShapeSize);
                pen.DashStyle = PenStyle;
                g.DrawEllipse(pen, StartOrigin.X, StartOrigin.Y, Width, Height);
            }
        }

        public override bool ContainsPoint(Point p)
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

        public override Shape Clone()
        {
            return new Ellipse(StartOrigin, Width, Height, ChosenColor, ShapeSize, FilledShape, PenStyle);
        }
    }
}
