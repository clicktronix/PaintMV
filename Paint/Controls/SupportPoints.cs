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
                    return new Rectangle(xValue - 3 - _shapeSelection.MainForm.SizeNodeRect/2, yValue - 3 - _shapeSelection.MainForm.SizeNodeRect/2, _shapeSelection.MainForm.SizeNodeRect, _shapeSelection.MainForm.SizeNodeRect);

                case 1:
                    return new Rectangle(xValue - 4 - _shapeSelection.MainForm.SizeNodeRect/2, yValue + shape.Height/2 - _shapeSelection.MainForm.SizeNodeRect/2, _shapeSelection.MainForm.SizeNodeRect, _shapeSelection.MainForm.SizeNodeRect);

                case 2:
                    return new Rectangle(xValue - 3 - _shapeSelection.MainForm.SizeNodeRect/2, yValue + 3 + shape.Height - _shapeSelection.MainForm.SizeNodeRect/2, _shapeSelection.MainForm.SizeNodeRect, _shapeSelection.MainForm.SizeNodeRect);

                case 3:
                    return new Rectangle(xValue + shape.Width/2 - _shapeSelection.MainForm.SizeNodeRect/2, yValue + 3 + shape.Height - _shapeSelection.MainForm.SizeNodeRect/2, _shapeSelection.MainForm.SizeNodeRect, _shapeSelection.MainForm.SizeNodeRect);

                case 4:
                    return new Rectangle(xValue + 3 + shape.Width - _shapeSelection.MainForm.SizeNodeRect/2, yValue - 3 - _shapeSelection.MainForm.SizeNodeRect/2, _shapeSelection.MainForm.SizeNodeRect, _shapeSelection.MainForm.SizeNodeRect);

                case 5:
                    return new Rectangle(xValue + 3 + shape.Width - _shapeSelection.MainForm.SizeNodeRect/2, yValue + 3 + shape.Height - _shapeSelection.MainForm.SizeNodeRect/2, _shapeSelection.MainForm.SizeNodeRect, _shapeSelection.MainForm.SizeNodeRect);

                case 6:
                    return new Rectangle(xValue + 3 + shape.Width - _shapeSelection.MainForm.SizeNodeRect/2, yValue + shape.Height/2 - _shapeSelection.MainForm.SizeNodeRect/2, _shapeSelection.MainForm.SizeNodeRect, _shapeSelection.MainForm.SizeNodeRect);

                case 7:
                    return new Rectangle(xValue + shape.Width/2 - _shapeSelection.MainForm.SizeNodeRect/2, yValue - 4 - _shapeSelection.MainForm.SizeNodeRect/2, _shapeSelection.MainForm.SizeNodeRect, _shapeSelection.MainForm.SizeNodeRect);
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
            Debug.Assert(_shapeSelection.MainForm.IndexOfSelectedShape != null, "No figures for selection!");

            Shape tempShape =
                _shapeSelection.MainForm.Doc.AllShapes[_shapeSelection.MainForm.IndexOfSelectedShape.Value];

            switch (value)
            {
                case Enumerations.Positions.LeftUp:
                    return new Rectangle(tempShape.StartOrigin.X - 7, tempShape.StartOrigin.Y - 7,
                        _shapeSelection.MainForm.SizeNodeRect, _shapeSelection.MainForm.SizeNodeRect);

                case Enumerations.Positions.LeftMiddle:
                    return new Rectangle(tempShape.StartOrigin.X - 7, tempShape.StartOrigin.Y + tempShape.Height/2,
                        _shapeSelection.MainForm.SizeNodeRect, _shapeSelection.MainForm.SizeNodeRect);

                case Enumerations.Positions.LeftBottom:
                    return new Rectangle(tempShape.StartOrigin.X - 7, tempShape.StartOrigin.Y + 5 + tempShape.Height,
                        _shapeSelection.MainForm.SizeNodeRect, _shapeSelection.MainForm.SizeNodeRect);

                case Enumerations.Positions.BottomMiddle:
                    return new Rectangle(tempShape.StartOrigin.X + tempShape.Width/2,
                        tempShape.StartOrigin.Y + 5 + tempShape.Height, _shapeSelection.MainForm.SizeNodeRect,
                        _shapeSelection.MainForm.SizeNodeRect);

                case Enumerations.Positions.RightUp:
                    return new Rectangle(tempShape.StartOrigin.X + 5 + tempShape.Width, tempShape.StartOrigin.Y - 7,
                        _shapeSelection.MainForm.SizeNodeRect, _shapeSelection.MainForm.SizeNodeRect);

                case Enumerations.Positions.RightBottom:
                    return new Rectangle(tempShape.StartOrigin.X + 5 + tempShape.Width,
                        tempShape.StartOrigin.Y + 5 + tempShape.Height, _shapeSelection.MainForm.SizeNodeRect,
                        _shapeSelection.MainForm.SizeNodeRect);

                case Enumerations.Positions.RightMiddle:
                    return new Rectangle(tempShape.StartOrigin.X + 5 + tempShape.Width,
                        tempShape.StartOrigin.Y + tempShape.Height/2, _shapeSelection.MainForm.SizeNodeRect,
                        _shapeSelection.MainForm.SizeNodeRect);

                case Enumerations.Positions.UpMiddle:
                    return new Rectangle(tempShape.StartOrigin.X + tempShape.Width/2, tempShape.StartOrigin.Y - 6,
                        _shapeSelection.MainForm.SizeNodeRect, _shapeSelection.MainForm.SizeNodeRect);
                default:
                    return new Rectangle();
            }
        }
    }
}