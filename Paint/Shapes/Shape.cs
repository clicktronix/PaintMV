using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PaintMV.Shapes
{
    /// <summary>
    /// Abstract shape class allows you to add different shapes in the list
    /// </summary>
    [Serializable]
    public abstract class Shape
    {
        public Point StartOrigin { get; set; }
        public Point EndOrigin { get; set; }
        public Point[] PointsArray { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ShapeSize { get; set; }
        public Color ChosenColor { get; set; }
        public Color FillColor { get; set; }
        public bool IsSelected { get; set; } = false;
        public bool IsLine { get; set; } = false; // переместить во внутрь фигур
        public bool IsPolygon { get; set; } = false; // переместить во внутрь фигур
        public bool DrawPolygon { get; set; } = false; // переместить во внутрь фигур
        public DashStyle PenStyle { get; set; }

        public abstract void Draw(Graphics g);
        public abstract Shape Clone();
        public abstract bool ContainsPoint(Point p); // поднять в абстрактный класс
        public abstract bool ContainsSelectedFigure(Point startPoint, Point endPoint); // поднять в абстрактный класс
        public abstract void SetShapeIsSelected(bool isSelected); // поднять в абстрактный класс
        public abstract bool GetShapeIsSelected(); // поднять в абстрактный класс
    }
}
