using PaintMV.GUI;

namespace PaintMV.Controls
{
    /// <summary>
    /// Class undo selection figures
    /// </summary>
    public class UnselectShapes
    {
        private readonly MainForm _mainForm;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="mainForm"></param>
        public UnselectShapes(MainForm mainForm)
        {
            _mainForm = mainForm;
        }

        /// <summary>
        /// Set flag IsSelected false of all shapes in list
        /// </summary>
        public void UnselectAllShapes()
        {
            for (int i = _mainForm.Doc.AllShapes.Count - 1; i >= 0; i--)
            {
                _mainForm.Doc.AllShapes[i].SetShapeIsSelected(false);
            }
        }
    }
}