using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PaintMV.Shapes
{
    [Serializable]
    class Line : Shape
    {
        public Line(Point startOrigin, Point endOrigin, int width, int height, Color chosenColor, int shapeSize)
        {
            StartOrigin = startOrigin;
            EndOrigin = endOrigin;
            Width = width;
            Height = height;
            ChosenColor = chosenColor;
            ShapeSize = shapeSize;
        }

        public override void Draw(Graphics g)
        {
            g.DrawLine(new Pen(ChosenColor, ShapeSize), StartOrigin, EndOrigin);
           
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
    }
}
