using System.Drawing;
using PaintMV.GUI;
using PaintMV.Shapes;

namespace PaintMV.Controls
{
    public class LineSelectionByPoint
    {
        private readonly FrmPaint _frmPaint;

        public LineSelectionByPoint(FrmPaint frmPaint)
        {
            _frmPaint = frmPaint;
        }

        public void MakeSelectionOfLine(Shape shape, Graphics g)
        {
            int xValue = shape.StartOrigin.X;
            int yValue = shape.StartOrigin.Y;

            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    g.DrawEllipse(new Pen(Color.Blue), xValue - _frmPaint.SizeNodeRect / 2, yValue - _frmPaint.SizeNodeRect / 2, _frmPaint.SizeNodeRect, _frmPaint.SizeNodeRect);
                }
                else
                {
                    g.DrawEllipse(new Pen(Color.Blue), shape.EndOrigin.X - 3, shape.EndOrigin.Y - 3, _frmPaint.SizeNodeRect, _frmPaint.SizeNodeRect);
                }
            }
            Pen tempPen = new Pen(Color.Blue);
            tempPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            g.DrawLine(tempPen, shape.StartOrigin.X, shape.StartOrigin.Y, shape.EndOrigin.X, shape.EndOrigin.Y);
            _frmPaint.PnlGraphic.Invalidate();
        }
    }
}