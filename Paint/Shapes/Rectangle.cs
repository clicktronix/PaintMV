﻿using System;
using System.Drawing;

namespace PaintMV.Shapes
{
    [Serializable]
    internal class Rectangle : Shape
    {
        public Rectangle(Point startOrigin, int width, int height, Color chosenColor, int shapeSize, bool fillShape)
        {
            StartOrigin = startOrigin;
            Width = width;
            Height = height;
            ChosenColor = chosenColor;
            ShapeSize = shapeSize;
            FilledShape = fillShape;
        }

        public override void Draw(Graphics g)
        {
            Pen penColor = new Pen(ChosenColor, ShapeSize);

            if (FilledShape)
            {
                SolidBrush tempBrush = new SolidBrush(ChosenColor);
                g.FillRectangle(tempBrush, StartOrigin.X, StartOrigin.Y, Width, Height);
            }
            else
            {
                g.DrawRectangle(penColor, StartOrigin.X, StartOrigin.Y, Width, Height);
            }
         
        }

        public override bool ContainsPoint(Point p)
        {
            if (p.X > StartOrigin.X - 5 && p.X < StartOrigin.X + Width + 10 && p.Y > StartOrigin.Y - 5 && p.Y < StartOrigin.Y + Height + 10)
            {
                return true;
            }
            return false;
        }

        public override Shape Clone()
        {
            return new Rectangle(StartOrigin, Width, Height, ChosenColor, ShapeSize, FilledShape);
        }
    }
}
