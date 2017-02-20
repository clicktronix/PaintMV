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

        public bool LineSelection;
        public bool PolygonSelection;
        public SupportPoints SupportPoints { get; }
        public MainForm MainForm { get; }
        public Enumerations.Positions NodeSelected { set; get; } = Enumerations.Positions.None;
        
#endregion

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="mainForm"></param>
        public ShapeSelection(MainForm mainForm)
        {
            MainForm = mainForm;
            SupportPoints = new SupportPoints(this, MainForm);
        }

        /// <summary>
        /// Method releasing the selected shape
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="g"></param>
        public void MakeSelectionOfShape(Shape shape, Graphics g)
        {
            Pen tempPen = new Pen(Color.Blue) { DashStyle = DashStyle.Dash };
            if (LineSelection)
            {
                SupportPoints.PolygonSelection = false;
                g.DrawRectangle(new Pen(Color.Blue), SupportPoints.GetRect(8, shape));
                g.DrawRectangle(new Pen(Color.Blue), SupportPoints.GetRect(9, shape));
                g.DrawLine(tempPen, shape.StartOrigin.X, shape.StartOrigin.Y, shape.EndOrigin.X, shape.EndOrigin.Y);
            }
            else if (PolygonSelection)
            {
                SupportPoints.PolygonSelection = true;
                var points = shape.PointsArray;
                for (var i = points.Length - 1; i > -1; i--)
                {
                    var supportShape = shape.Clone();
                    supportShape.EndOrigin = points[i];
                    g.DrawRectangle(new Pen(Color.Blue), SupportPoints.GetRect(8, supportShape));
                }
                g.DrawPolygon(tempPen, shape.PointsArray);
            }
            else
            {
                SupportPoints.PolygonSelection = false;
                for (int i = 0; i < 8; i++)
                {
                    g.DrawRectangle(new Pen(Color.Blue), SupportPoints.GetRect(i, shape));
                }
                g.DrawRectangle(tempPen, shape.StartOrigin.X - 3, shape.StartOrigin.Y - 3, shape.Width + 6,
                    shape.Height + 6);
            }
        }
    }
}