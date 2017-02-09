using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PaintMV.GUI;
using PaintMV.Shapes;

namespace PaintMV.Controls
{
    /// <summary>
    /// Changing class sizes shapes and moving it on the form
    /// </summary>
    public class MoveResize : ICommand
    {
        private readonly MainForm _mainForm;

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="mainForm"></param>
        public MoveResize(MainForm mainForm)
        {
            _mainForm = mainForm;
        }

        /// <summary>
        /// Method changing sizes shapes and moving it on the form
        /// </summary>
        /// <param name="g"></param>
        /// <param name="e"></param>
        /// <param name="tempShape"></param>
        public void Execute(Graphics g, MouseEventArgs e, Shape tempShape)
        {
            switch (_mainForm.ShapeSelection.NodeSelected)
            {
                case Enumerations.Positions.LeftUp:
                    tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - _mainForm.StartPoint.X, tempShape.StartOrigin.Y);
                    tempShape.Width -= e.X - _mainForm.StartPoint.X;
                    tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y + e.Y - _mainForm.StartPoint.Y);
                    tempShape.Height -= e.Y - _mainForm.StartPoint.Y;
                    break;
                case Enumerations.Positions.LeftMiddle:
                    tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - _mainForm.StartPoint.X, tempShape.StartOrigin.Y);
                    tempShape.Width -= (e.X - _mainForm.StartPoint.X);
                    break;
                case Enumerations.Positions.LeftBottom:
                    tempShape.Width -= e.X - _mainForm.StartPoint.X;
                    tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - _mainForm.StartPoint.X, tempShape.StartOrigin.Y);
                    tempShape.Height += e.Y - _mainForm.StartPoint.Y;
                    break;
                case Enumerations.Positions.BottomMiddle:
                    tempShape.Height += e.Y - _mainForm.StartPoint.Y;
                    break;
                case Enumerations.Positions.RightUp:
                    tempShape.Width += e.X - _mainForm.StartPoint.X;
                    tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y + e.Y - _mainForm.StartPoint.Y);
                    tempShape.Height -= e.Y - _mainForm.StartPoint.Y;
                    break;
                case Enumerations.Positions.RightBottom:
                    tempShape.Width += e.X - _mainForm.StartPoint.X;
                    tempShape.Height += e.Y - _mainForm.StartPoint.Y;
                    break;
                case Enumerations.Positions.RightMiddle:
                    tempShape.Width += e.X - _mainForm.StartPoint.X;
                    break;
                case Enumerations.Positions.UpMiddle:
                    tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y + e.Y - _mainForm.StartPoint.Y);
                    tempShape.Height -= e.Y - _mainForm.StartPoint.Y;
                    tempShape.EndOrigin = new Point(tempShape.EndOrigin.X, tempShape.EndOrigin.Y + e.Y - _mainForm.StartPoint.Y);
                    break;
                default:
                    if (_mainForm.MMove)
                    {
                        tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - _mainForm.StartPoint.X, tempShape.StartOrigin.Y);
                        tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y + e.Y - _mainForm.StartPoint.Y);
                        tempShape.EndOrigin = new Point(tempShape.EndOrigin.X + e.X - _mainForm.StartPoint.X, tempShape.EndOrigin.Y);
                        tempShape.EndOrigin = new Point(tempShape.EndOrigin.X, tempShape.EndOrigin.Y + e.Y - _mainForm.StartPoint.Y);
                        _mainForm.PnlGraphic.Cursor = Cursors.SizeAll;
                    }
                    break;
            }
        }

        /// <summary>
        /// Undo changing shape
        /// </summary>
        public void Undo()
        {

        }

        /// <summary>
        /// Redo changing shape
        /// </summary>
        public void Redo()
        {

        }
    }
}