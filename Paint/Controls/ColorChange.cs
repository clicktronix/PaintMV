using PaintMV.GUI;

namespace PaintMV.Controls
{
    /// <summary>
    /// Class color changer
    /// </summary>
    public class ColorChange
    {
        private readonly MainForm _mainForm;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="mainForm"></param>
        public ColorChange(MainForm mainForm)
        {
            _mainForm = mainForm;
        }

        /// <summary>
        /// Change current color method 
        /// </summary>
        public void Color()
        {
            for (int i = _mainForm.Doc.AllShapes.Count - 1; i >= 0; i--)
            {
                if (_mainForm.IndexOfSelectedShape != null && _mainForm.Doc.AllShapes[i].GetShapeIsSelected() && _mainForm.IndexOfSelectedShape != null)
                {
                    _mainForm.Doc.AllShapes[i].ChosenColor = _mainForm.ChosenColor;
                    _mainForm.Doc.AllShapes[i].FilledShape = _mainForm.chBoxFill.Checked;
                }
            }
        }
    }
}