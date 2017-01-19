using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PaintMV.GUI;
using PaintMV.Shapes;
using Rectangle = System.Drawing.Rectangle;

namespace PaintMV.Controls
{
    public class ShapeSelectionByPoint
    {
        private readonly FrmPaint _frmPaint;
        public Point StartOrigin { get; set; }
        public Point EndOrigin { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public ResizePosition NodeSelected { set; get; } = ResizePosition.None;

        public ShapeSelectionByPoint(FrmPaint frmPaint)
        {
            _frmPaint = frmPaint;
        }

        public void MakeSelectionOfShape(Shape shape, Graphics g)
        {
            for (int i = 0; i < 8; i++)
            {
                g.DrawRectangle(new Pen(Color.Blue), GetRect(i, shape));
            }

            Pen tempPen = new Pen(Color.Blue);
            tempPen.DashStyle = DashStyle.Dash;
            g.DrawRectangle(tempPen, shape.StartOrigin.X - 3, shape.StartOrigin.Y - 3, shape.Width + 6, shape.Height + 6);
        }

        private Rectangle GetRect(int value, Shape shape)
        {
            int xValue = shape.StartOrigin.X;
            int yValue = shape.StartOrigin.Y;

            switch (value)
            {
                case 0:
                    return new Rectangle(xValue - 3 - _frmPaint.SizeNodeRect/2, yValue - 3 - _frmPaint.SizeNodeRect/2, _frmPaint.SizeNodeRect, _frmPaint.SizeNodeRect);

                case 1:
                    return new Rectangle(xValue - 4 - _frmPaint.SizeNodeRect/2, yValue + shape.Height/2 - _frmPaint.SizeNodeRect/2, _frmPaint.SizeNodeRect, _frmPaint.SizeNodeRect);

                case 2:
                    return new Rectangle(xValue - 3 - _frmPaint.SizeNodeRect/2, yValue + 3 + shape.Height - _frmPaint.SizeNodeRect/2, _frmPaint.SizeNodeRect, _frmPaint.SizeNodeRect);

                case 3:
                    return new Rectangle(xValue + shape.Width/2 - _frmPaint.SizeNodeRect/2, yValue + 3 + shape.Height - _frmPaint.SizeNodeRect/2, _frmPaint.SizeNodeRect, _frmPaint.SizeNodeRect);

                case 4:
                    return new Rectangle(xValue + 3 + shape.Width - _frmPaint.SizeNodeRect/2, yValue - 3 - _frmPaint.SizeNodeRect/2, _frmPaint.SizeNodeRect, _frmPaint.SizeNodeRect);

                case 5:
                    return new Rectangle(xValue + 3 + shape.Width - _frmPaint.SizeNodeRect/2, yValue + 3 + shape.Height - _frmPaint.SizeNodeRect/2, _frmPaint.SizeNodeRect, _frmPaint.SizeNodeRect);

                case 6:
                    return new Rectangle(xValue + 3 + shape.Width - _frmPaint.SizeNodeRect/2, yValue + shape.Height/2 - _frmPaint.SizeNodeRect/2, _frmPaint.SizeNodeRect, _frmPaint.SizeNodeRect);

                case 7:
                    return new Rectangle(xValue + shape.Width/2 - _frmPaint.SizeNodeRect/2, yValue - 4 - _frmPaint.SizeNodeRect/2, _frmPaint.SizeNodeRect, _frmPaint.SizeNodeRect);
                default:
                    return new Rectangle();
            }
        }

        public ResizePosition GetNodeSelectable(Point p)
        {
            foreach (ResizePosition r in Enum.GetValues(typeof (ResizePosition)))
            {
                if (GetRectangle(r).Contains(p))
                {
                    return r;
                }
            }
            return ResizePosition.None;
        }

        public Cursor GetCursor(ResizePosition p)
        {
            switch (p)
            {
                case ResizePosition.LeftUp:
                    return Cursors.SizeNWSE;

                case ResizePosition.LeftMiddle:
                    return Cursors.SizeWE;

                case ResizePosition.LeftBottom:
                    return Cursors.SizeNESW;

                case ResizePosition.BottomMiddle:
                    return Cursors.SizeNS;

                case ResizePosition.RightUp:
                    return Cursors.SizeNESW;

                case ResizePosition.RightBottom:
                    return Cursors.SizeNWSE;

                case ResizePosition.RightMiddle:
                    return Cursors.SizeWE;

                case ResizePosition.UpMiddle:
                    return Cursors.SizeNS;
                default:
                    return Cursors.Default;
            }
        }

        private Rectangle GetRectangle(ResizePosition value)
        {
            Debug.Assert(_frmPaint.IndexOfSelectedShape != null, "Нет нарисованных фигур!");
            Shape tempShape = _frmPaint.Doc.AllShapes[_frmPaint.IndexOfSelectedShape.Value];

            switch (value)
            {
                case ResizePosition.LeftUp:
                    return new Rectangle(tempShape.StartOrigin.X - 7, tempShape.StartOrigin.Y - 7, _frmPaint.SizeNodeRect, _frmPaint.SizeNodeRect);

                case ResizePosition.LeftMiddle:
                    return new Rectangle(tempShape.StartOrigin.X - 7, tempShape.StartOrigin.Y + tempShape.Height/2, _frmPaint.SizeNodeRect, _frmPaint.SizeNodeRect);

                case ResizePosition.LeftBottom:
                    return new Rectangle(tempShape.StartOrigin.X - 7, tempShape.StartOrigin.Y + 5 + tempShape.Height, _frmPaint.SizeNodeRect, _frmPaint.SizeNodeRect);

                case ResizePosition.BottomMiddle:
                    return new Rectangle(tempShape.StartOrigin.X + tempShape.Width/2, tempShape.StartOrigin.Y + 5 + tempShape.Height, _frmPaint.SizeNodeRect, _frmPaint.SizeNodeRect);

                case ResizePosition.RightUp:
                    return new Rectangle(tempShape.StartOrigin.X + 5 + tempShape.Width, tempShape.StartOrigin.Y - 7, _frmPaint.SizeNodeRect, _frmPaint.SizeNodeRect);

                case ResizePosition.RightBottom:
                    return new Rectangle(tempShape.StartOrigin.X + 5 + tempShape.Width, tempShape.StartOrigin.Y + 5 + tempShape.Height, _frmPaint.SizeNodeRect, _frmPaint.SizeNodeRect);

                case ResizePosition.RightMiddle:
                    return new Rectangle(tempShape.StartOrigin.X + 5 + tempShape.Width, tempShape.StartOrigin.Y + tempShape.Height/2, _frmPaint.SizeNodeRect, _frmPaint.SizeNodeRect);

                case ResizePosition.UpMiddle:
                    return new Rectangle(tempShape.StartOrigin.X + tempShape.Width/2, tempShape.StartOrigin.Y - 6, _frmPaint.SizeNodeRect, _frmPaint.SizeNodeRect);
                default:
                    return new Rectangle();
            }
        }

        public enum ResizePosition
        {
            UpMiddle,
            LeftMiddle,
            LeftBottom,
            LeftUp,
            RightUp,
            RightMiddle,
            RightBottom,
            BottomMiddle,
            None
        }
    }
}