using System.Drawing;
using PaintMV.GUI;
using PaintMV.Shapes;

namespace PaintMV.Controls
{
    /// <summary>
    /// Class releasing the selected line
    /// </summary>
    public class LineSelection
    {
        public MainForm MainForm { get; }
        //public LinesSupportPoints LinesSupportPoints { get; }
        //public Enumerations.LinesPositions NodeSelected { set; get; } = Enumerations.LinesPositions.None;

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="mainForm"></param>
        public LineSelection(MainForm mainForm)
        {
            MainForm = mainForm;
            //LinesSupportPoints = new LinesSupportPoints(this);
        }

        /// <summary>
        /// Method releasing the selected line
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="g"></param>
        public void MakeSelectionOfLine(Shape shape, Graphics g)
        {
            for (int i = 0; i < 2; i++)
            {
                //g.DrawRectangle(new Pen(Color.Blue), LinesSupportPoints.GetRect(i, shape));
            }
            Pen tempPen = new Pen(Color.Blue) {DashStyle = System.Drawing.Drawing2D.DashStyle.Dash};
            g.DrawLine(tempPen, shape.StartOrigin.X, shape.StartOrigin.Y, shape.EndOrigin.X, shape.EndOrigin.Y);
            MainForm.PnlGraphic.Invalidate();
        }
    }
}