using System.Drawing;
using System.Windows.Forms;
using PaintMV.GUI;
using PaintMV.Shapes;

namespace PaintMV.Controls
{
    public class ResizePosition
    {
        private readonly FrmPaint _frmPaint;

        public ResizePosition(FrmPaint frmPaint)
        {
            _frmPaint = frmPaint;
        }

        public void GetValueOfResizedPosition(MouseEventArgs e, Shape tempShape)
        {
            switch (_frmPaint.ShapeSelectionByPoint.NodeSelected)
            {
                case Enumerations.ResizePosition.LeftUp:
                    tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - _frmPaint.StartPoint.X, tempShape.StartOrigin.Y);
                    tempShape.Width -= e.X - _frmPaint.StartPoint.X;
                    tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y + e.Y - _frmPaint.StartPoint.Y);
                    tempShape.Height -= e.Y - _frmPaint.StartPoint.Y;
                    break;
                case Enumerations.ResizePosition.LeftMiddle:
                    tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - _frmPaint.StartPoint.X, tempShape.StartOrigin.Y);
                    tempShape.Width -= (e.X - _frmPaint.StartPoint.X);
                    break;
                case Enumerations.ResizePosition.LeftBottom:
                    tempShape.Width -= e.X - _frmPaint.StartPoint.X;
                    tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - _frmPaint.StartPoint.X, tempShape.StartOrigin.Y);
                    tempShape.Height += e.Y - _frmPaint.StartPoint.Y;
                    break;
                case Enumerations.ResizePosition.BottomMiddle:
                    tempShape.Height += e.Y - _frmPaint.StartPoint.Y;
                    break;
                case Enumerations.ResizePosition.RightUp:
                    tempShape.Width += e.X - _frmPaint.StartPoint.X;
                    tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y + e.Y - _frmPaint.StartPoint.Y);
                    tempShape.Height -= e.Y - _frmPaint.StartPoint.Y;
                    break;
                case Enumerations.ResizePosition.RightBottom:
                    tempShape.Width += e.X - _frmPaint.StartPoint.X;
                    tempShape.Height += e.Y - _frmPaint.StartPoint.Y;
                    break;
                case Enumerations.ResizePosition.RightMiddle:
                    tempShape.Width += e.X - _frmPaint.StartPoint.X;
                    break;
                case Enumerations.ResizePosition.UpMiddle:
                    tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y + e.Y - _frmPaint.StartPoint.Y);
                    tempShape.Height -= e.Y - _frmPaint.StartPoint.Y;
                    break;
                default:
                    if (_frmPaint.MMove)
                    {
                        tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - _frmPaint.StartPoint.X, tempShape.StartOrigin.Y);
                        tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y + e.Y - _frmPaint.StartPoint.Y);
                        _frmPaint.PnlGraphic.Cursor = Cursors.SizeAll;
                    }
                    break;
            }
        }
    }
}