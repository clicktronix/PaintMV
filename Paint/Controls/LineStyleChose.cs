using System.Drawing.Drawing2D;
using PaintMV.GUI;

namespace PaintMV.Controls
{
    public class LineStyleChose
    {
        private MainForm _mainForm;

        public LineStyleChose(MainForm mainForm)
        {
            _mainForm = mainForm;
        }

        /// <summary>
        /// Line style method
        /// </summary>
        public void ChoseLineStyle()
        {
            if (_mainForm.radioSolid.Checked)
            {
                _mainForm.PenStyle = DashStyle.Solid;
            }
            else if (_mainForm.radioDash.Checked)
            {
                _mainForm.PenStyle = DashStyle.Dash;
            }
            else if (_mainForm.radioDot.Checked)
            {
                _mainForm.PenStyle = DashStyle.Dot;
            }
        }
    }
}