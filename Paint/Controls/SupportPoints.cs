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
        private int _sizeNodeRect { set; get; } = 10;

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
                    return new Rectangle(xValue - 3 - _sizeNodeRect/2, yValue - 3 - _sizeNodeRect/2, _sizeNodeRect, _sizeNodeRect);

                case 1:
                    return new Rectangle(xValue - 4 - _sizeNodeRect/2, yValue + shape.Height/2 - _sizeNodeRect/2, _sizeNodeRect, _sizeNodeRect);

                case 2:
                    return new Rectangle(xValue - 3 - _sizeNodeRect/2, yValue + 3 + shape.Height - _sizeNodeRect/2, _sizeNodeRect, _sizeNodeRect);

                case 3:
                    return new Rectangle(xValue + shape.Width/2 - _sizeNodeRect/2, yValue + 3 + shape.Height - _sizeNodeRect/2, _sizeNodeRect, _sizeNodeRect);

                case 4:
                    return new Rectangle(xValue + 3 + shape.Width - _sizeNodeRect/2, yValue - 3 - _sizeNodeRect/2, _sizeNodeRect, _sizeNodeRect);

                case 5:
                    return new Rectangle(xValue + 3 + shape.Width - _sizeNodeRect/2, yValue + 3 + shape.Height - _sizeNodeRect/2, _sizeNodeRect, _sizeNodeRect);

                case 6:
                    return new Rectangle(xValue + 3 + shape.Width - _sizeNodeRect/2, yValue + shape.Height/2 - _sizeNodeRect/2, _sizeNodeRect, _sizeNodeRect);

                case 7:
                    return new Rectangle(xValue + shape.Width/2 - _sizeNodeRect/2, yValue - 4 - _sizeNodeRect/2, _sizeNodeRect, _sizeNodeRect);
                default:
                    return new Rectangle();
            }
        }

        /// <summary>
        /// Method of determining the selected node
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Enumerations.Positions GetNodeSelectable(Point p)
        {
            foreach (Enumerations.Positions r in Enum.GetValues(typeof (Enumerations.Positions)))
            {
                if (GetRectangle(r).Contains(p))
                {
                    return r;
                }
            }
            return Enumerations.Positions.None;
        }

        /// <summary>
        /// Method of determining the position of the cursor
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Cursor GetCursor(Enumerations.Positions p)
        {
            switch (p)
            {
                case Enumerations.Positions.LeftUp:
                    return Cursors.SizeNWSE;

                case Enumerations.Positions.LeftMiddle:
                    return Cursors.SizeWE;

                case Enumerations.Positions.LeftBottom:
                    return Cursors.SizeNESW;

                case Enumerations.Positions.BottomMiddle:
                    return Cursors.SizeNS;

                case Enumerations.Positions.RightUp:
                    return Cursors.SizeNESW;

                case Enumerations.Positions.RightBottom:
                    return Cursors.SizeNWSE;

                case Enumerations.Positions.RightMiddle:
                    return Cursors.SizeWE;

                case Enumerations.Positions.UpMiddle:
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
        private Rectangle GetRectangle(Enumerations.Positions value)
        {
            Debug.Assert(_shapeSelection.MainForm.IndexOfSelectedShape != null, "No selected figures!");
            Shape tempShape = _shapeSelection.MainForm.Doc.AllShapes[_shapeSelection.MainForm.IndexOfSelectedShape.Value];
            switch (value)
            {
                case Enumerations.Positions.LeftUp:
                    return new Rectangle(tempShape.StartOrigin.X - 7, tempShape.StartOrigin.Y - 7, _sizeNodeRect, _sizeNodeRect);

                case Enumerations.Positions.LeftMiddle:
                    return new Rectangle(tempShape.StartOrigin.X - 7, tempShape.StartOrigin.Y + tempShape.Height/2, _sizeNodeRect, _sizeNodeRect);

                case Enumerations.Positions.LeftBottom:
                    return new Rectangle(tempShape.StartOrigin.X - 7, tempShape.StartOrigin.Y + 5 + tempShape.Height, _sizeNodeRect, _sizeNodeRect);

                case Enumerations.Positions.BottomMiddle:
                    return new Rectangle(tempShape.StartOrigin.X + tempShape.Width/2,
                        tempShape.StartOrigin.Y + 5 + tempShape.Height, _sizeNodeRect, _sizeNodeRect);

                case Enumerations.Positions.RightUp:
                    return new Rectangle(tempShape.StartOrigin.X + 5 + tempShape.Width, tempShape.StartOrigin.Y - 7,_sizeNodeRect, _sizeNodeRect);

                case Enumerations.Positions.RightBottom:
                    return new Rectangle(tempShape.StartOrigin.X + 5 + tempShape.Width,
                        tempShape.StartOrigin.Y + 5 + tempShape.Height, _sizeNodeRect, _sizeNodeRect);

                case Enumerations.Positions.RightMiddle:
                    return new Rectangle(tempShape.StartOrigin.X + 5 + tempShape.Width,
                        tempShape.StartOrigin.Y + tempShape.Height/2, _sizeNodeRect, _sizeNodeRect);

                case Enumerations.Positions.UpMiddle:
                    return new Rectangle(tempShape.StartOrigin.X + tempShape.Width/2, tempShape.StartOrigin.Y - 6, _sizeNodeRect, _sizeNodeRect);
                default:
                    return new Rectangle();
            }
        }
    }
}