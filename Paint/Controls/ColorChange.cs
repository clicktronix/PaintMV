using PaintMV.GUI;

namespace PaintMV.Controls
{
    public class ColorChange
    {
        private MainForm _mainForm;

        public ColorChange(MainForm mainForm)
        {
            _mainForm = mainForm;
        }

        /// <summary>
        /// Change current color method 
        /// </summary>
        public void ChangeColor()
        {
            for (int i = _mainForm.Doc.AllShapes.Count - 1; i >= 0; i--)
            {
                if (_mainForm.IndexOfSelectedShape != null && _mainForm.Doc.AllShapes[i].GetShapeIsSelected() && _mainForm.IndexOfSelectedShape != null)
                {
                    _mainForm.Doc.AllShapes[i].ChosenColor = _mainForm.ChosenColor;
                    _mainForm.Doc.AllShapes[i].FilledShape = _mainForm.chBoxFill.Checked;
                }
            }
            _mainForm.PnlGraphic.Invalidate();
        }
    }
}