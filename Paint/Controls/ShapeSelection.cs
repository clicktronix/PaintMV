using System.Drawing;
using System.Drawing.Drawing2D;
using PaintMV.GUI;
using PaintMV.Shapes;

namespace PaintMV.Controls
{
    /// <summary>
    /// Class releasing the selected shape
    /// </summary>
    public class ShapeSelection
    {
#region Properties 
        public Point StartOrigin { get; set; }
        public Point EndOrigin { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public SupportPoints SupportPoints { get; }
        public FrmPaint FrmPaint { get; }
        public Enumerations.ResizePosition NodeSelected { set; get; } = Enumerations.ResizePosition.None;
#endregion

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="frmPaint"></param>
        public ShapeSelection(FrmPaint frmPaint)
        {
            FrmPaint = frmPaint;
            SupportPoints = new SupportPoints(this);
        }

        /// <summary>
        /// Method releasing the selected shape
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="g"></param>
        public void MakeSelectionOfShape(Shape shape, Graphics g)
        {
            for (int i = 0; i < 8; i++)
            {
                g.DrawRectangle(new Pen(Color.Blue), SupportPoints.GetRect(i, shape));
            }

            Pen tempPen = new Pen(Color.Blue);
            tempPen.DashStyle = DashStyle.Dash;
            g.DrawRectangle(tempPen, shape.StartOrigin.X - 3, shape.StartOrigin.Y - 3, shape.Width + 6, shape.Height + 6);
        }
    }
}