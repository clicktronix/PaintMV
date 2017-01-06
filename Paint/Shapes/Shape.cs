using System;
using System.Drawing;

namespace PaintMV.Shapes
{
    [Serializable]
    public abstract class Shape
    {
        public Point StartOrigin { get; set; }
        public Point EndOrigin { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ShapeSize { get; set; }
        public Color ChosenColor { get; set; }
        public bool FilledShape { get; set; }

        public abstract void Draw(Graphics g);
        public abstract Shape Clone();
        public abstract bool ContainsPoint(Point p);
    }
}
