using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PaintMV.Shapes
{
    [Serializable]
    internal class Polygon : Shape
    {
        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="pointsArray"></param>
        /// <param name="chosenColor"></param>
        /// <param name="shapeSize"></param>
        /// <param name="penStyle"></param>
        /// <param name="isSelected"></param>
        /// <param name="isPolygon"></param>
        public Polygon(Point[] pointsArray, Color chosenColor, int shapeSize, DashStyle penStyle, 
            bool isSelected, bool isPolygon, bool drawPolygon)
        {
            PointsArray = pointsArray;
            ChosenColor = chosenColor;
            ShapeSize = shapeSize;
            PenStyle = penStyle;
            IsSelected = isSelected;
            IsPolygon = isPolygon;
            DrawPolygon = drawPolygon;
        }

        /// <summary>
        /// Drawing a line method
        /// </summary>
        /// <param name="g"></param>
        public override void Draw(Graphics g)
        {
            Pen pen = new Pen(ChosenColor, ShapeSize) {DashStyle = PenStyle};
            SolidBrush tempBrush = new SolidBrush(FillColor);
            if (PointsArray.Length > 1)
            {
                g.DrawLines(pen, PointsArray);
                if (DrawPolygon)
                {
                    g.FillPolygon(tempBrush, PointsArray);
                    g.DrawPolygon(pen, PointsArray);
                }
            }
        }

        /// <summary>
        /// Method of determining the figure clicked on or not
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool ContainsPoint(Point p)
        {
            GraphicsPath myPath = new GraphicsPath();
            myPath.AddLines(PointsArray);
            bool pointWithinPolygon = myPath.IsVisible(p);

            if (pointWithinPolygon)
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

            bool pointWithinPolygon = false;
            for (var i = PointsArray.Length - 1; i > 0; i--)
            {
                pointWithinPolygon = myPath.IsVisible(PointsArray[i]);
            }
            if (pointWithinPolygon)
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
            return new Polygon(PointsArray, ChosenColor, ShapeSize, PenStyle, IsSelected, IsPolygon, DrawPolygon);
        }
    }
}