using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using PaintMV.Shapes;
using Rectangle = System.Drawing.Rectangle;

namespace PaintMV.Controls
{
    /// <summary>
    /// Class creates anchor points
    /// </summary>
    public class SupportPoints
    {
        private readonly ShapeSelection _shapeSelection;

        public SupportPoints(ShapeSelection shapeSelection)
        {
            _shapeSelection = shapeSelection;
        }

        /// <summary>
        /// Method draws squares around the selected shape
        /// </summary>
        /// <param name="value"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public Rectangle GetRect(int value, Shape shape)
        {
            int xValue = shape.StartOrigin.X;
            int yValue = shape.StartOrigin.Y;

            switch (value)
            {
                case 0:
                    return new Rectangle(xValue - 3 - _shapeSelection.FrmPaint.SizeNodeRect/2, yValue - 3 - _shapeSelection.FrmPaint.SizeNodeRect/2, _shapeSelection.FrmPaint.SizeNodeRect, _shapeSelection.FrmPaint.SizeNodeRect);

                case 1:
                    return new Rectangle(xValue - 4 - _shapeSelection.FrmPaint.SizeNodeRect/2, yValue + shape.Height/2 - _shapeSelection.FrmPaint.SizeNodeRect/2, _shapeSelection.FrmPaint.SizeNodeRect, _shapeSelection.FrmPaint.SizeNodeRect);

                case 2:
                    return new Rectangle(xValue - 3 - _shapeSelection.FrmPaint.SizeNodeRect/2, yValue + 3 + shape.Height - _shapeSelection.FrmPaint.SizeNodeRect/2, _shapeSelection.FrmPaint.SizeNodeRect, _shapeSelection.FrmPaint.SizeNodeRect);

                case 3:
                    return new Rectangle(xValue + shape.Width/2 - _shapeSelection.FrmPaint.SizeNodeRect/2, yValue + 3 + shape.Height - _shapeSelection.FrmPaint.SizeNodeRect/2, _shapeSelection.FrmPaint.SizeNodeRect, _shapeSelection.FrmPaint.SizeNodeRect);

                case 4:
                    return new Rectangle(xValue + 3 + shape.Width - _shapeSelection.FrmPaint.SizeNodeRect/2, yValue - 3 - _shapeSelection.FrmPaint.SizeNodeRect/2, _shapeSelection.FrmPaint.SizeNodeRect, _shapeSelection.FrmPaint.SizeNodeRect);

                case 5:
                    return new Rectangle(xValue + 3 + shape.Width - _shapeSelection.FrmPaint.SizeNodeRect/2, yValue + 3 + shape.Height - _shapeSelection.FrmPaint.SizeNodeRect/2, _shapeSelection.FrmPaint.SizeNodeRect, _shapeSelection.FrmPaint.SizeNodeRect);

                case 6:
                    return new Rectangle(xValue + 3 + shape.Width - _shapeSelection.FrmPaint.SizeNodeRect/2, yValue + shape.Height/2 - _shapeSelection.FrmPaint.SizeNodeRect/2, _shapeSelection.FrmPaint.SizeNodeRect, _shapeSelection.FrmPaint.SizeNodeRect);

                case 7:
                    return new Rectangle(xValue + shape.Width/2 - _shapeSelection.FrmPaint.SizeNodeRect/2, yValue - 4 - _shapeSelection.FrmPaint.SizeNodeRect/2, _shapeSelection.FrmPaint.SizeNodeRect, _shapeSelection.FrmPaint.SizeNodeRect);
                default:
                    return new Rectangle();
            }
        }

        /// <summary>
        /// Method of determining the selected node
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Enumerations.ResizePosition GetNodeSelectable(Point p)
        {
            foreach (Enumerations.ResizePosition r in Enum.GetValues(typeof (Enumerations.ResizePosition)))
            {
                if (GetRectangle(r).Contains(p))
                {
                    return r;
                }
            }
            return Enumerations.ResizePosition.None;
        }

        /// <summary>
        /// Method of determining the position of the cursor
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Cursor GetCursor(Enumerations.ResizePosition p)
        {
            switch (p)
            {
                case Enumerations.ResizePosition.LeftUp:
                    return Cursors.SizeNWSE;

                case Enumerations.ResizePosition.LeftMiddle:
                    return Cursors.SizeWE;

                case Enumerations.ResizePosition.LeftBottom:
                    return Cursors.SizeNESW;

                case Enumerations.ResizePosition.BottomMiddle:
                    return Cursors.SizeNS;

                case Enumerations.ResizePosition.RightUp:
                    return Cursors.SizeNESW;

                case Enumerations.ResizePosition.RightBottom:
                    return Cursors.SizeNWSE;

                case Enumerations.ResizePosition.RightMiddle:
                    return Cursors.SizeWE;

                case Enumerations.ResizePosition.UpMiddle:
                    return Cursors.SizeNS;
                default:
                    return Cursors.Default;
            }
        }

        /// <summary>
        /// Method of drawing a box around the selected shape
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Rectangle GetRectangle(Enumerations.ResizePosition value)
        {
            Debug.Assert(_shapeSelection.FrmPaint.IndexOfSelectedShape != null, "Нет нарисованных фигур!");
            Shape tempShape = _shapeSelection.FrmPaint.Doc.AllShapes[_shapeSelection.FrmPaint.IndexOfSelectedShape.Value];

            switch (value)
            {
                case Enumerations.ResizePosition.LeftUp:
                    return new Rectangle(tempShape.StartOrigin.X - 7, tempShape.StartOrigin.Y - 7, _shapeSelection.FrmPaint.SizeNodeRect, _shapeSelection.FrmPaint.SizeNodeRect);

                case Enumerations.ResizePosition.LeftMiddle:
                    return new Rectangle(tempShape.StartOrigin.X - 7, tempShape.StartOrigin.Y + tempShape.Height/2, _shapeSelection.FrmPaint.SizeNodeRect, _shapeSelection.FrmPaint.SizeNodeRect);

                case Enumerations.ResizePosition.LeftBottom:
                    return new Rectangle(tempShape.StartOrigin.X - 7, tempShape.StartOrigin.Y + 5 + tempShape.Height, _shapeSelection.FrmPaint.SizeNodeRect, _shapeSelection.FrmPaint.SizeNodeRect);

                case Enumerations.ResizePosition.BottomMiddle:
                    return new Rectangle(tempShape.StartOrigin.X + tempShape.Width/2, tempShape.StartOrigin.Y + 5 + tempShape.Height, _shapeSelection.FrmPaint.SizeNodeRect, _shapeSelection.FrmPaint.SizeNodeRect);

                case Enumerations.ResizePosition.RightUp:
                    return new Rectangle(tempShape.StartOrigin.X + 5 + tempShape.Width, tempShape.StartOrigin.Y - 7, _shapeSelection.FrmPaint.SizeNodeRect, _shapeSelection.FrmPaint.SizeNodeRect);

                case Enumerations.ResizePosition.RightBottom:
                    return new Rectangle(tempShape.StartOrigin.X + 5 + tempShape.Width, tempShape.StartOrigin.Y + 5 + tempShape.Height, _shapeSelection.FrmPaint.SizeNodeRect, _shapeSelection.FrmPaint.SizeNodeRect);

                case Enumerations.ResizePosition.RightMiddle:
                    return new Rectangle(tempShape.StartOrigin.X + 5 + tempShape.Width, tempShape.StartOrigin.Y + tempShape.Height/2, _shapeSelection.FrmPaint.SizeNodeRect, _shapeSelection.FrmPaint.SizeNodeRect);

                case Enumerations.ResizePosition.UpMiddle:
                    return new Rectangle(tempShape.StartOrigin.X + tempShape.Width/2, tempShape.StartOrigin.Y - 6, _shapeSelection.FrmPaint.SizeNodeRect, _shapeSelection.FrmPaint.SizeNodeRect);
                default:
                    return new Rectangle();
            }
        }
    }
}