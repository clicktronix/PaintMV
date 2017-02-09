using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PaintMV.GUI;
using PaintMV.Shapes;

namespace PaintMV.Controls
{
    /// <summary>
    /// Copy figures class
    /// </summary>
    public class Copy : ICommand
    {
        private readonly MainForm _mainForm;
        private readonly List<Shape> _copiedShapes = new List<Shape>();
        private readonly List<Shape> _redoShapes = new List<Shape>();
        private Shape _copiedFigure;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="mainForm"></param>
        public Copy(MainForm mainForm)
        {
            _mainForm = mainForm;
        }

        /// <summary>
        /// Copy selected figure metod
        /// </summary>
        /// <param name="g"></param>
        /// <param name="e"></param>
        /// <param name="tempShape"></param>
        public void Execute(Graphics g, MouseEventArgs e, Shape tempShape)
        {
            for (int i = _mainForm.Doc.AllShapes.Count - 1; i >= 0; i--)
            {
                if (_mainForm.IndexOfSelectedShape != null && _mainForm.Doc.AllShapes[i].GetShapeIsSelected() && _mainForm.IndexOfSelectedShape != null)
                {
                    _mainForm.Doc.AllShapes[i].SetShapeIsSelected(false);
                    _copiedFigure = _mainForm.Doc.AllShapes[i].Clone();
                    _copiedFigure.SetShapeIsSelected(true);
                    _copiedShapes.Add(_copiedFigure);
                    _mainForm.Doc.AllShapes.Add(_copiedFigure);
                }
            }
        }

        /// <summary>
        /// Undo copy figure
        /// </summary>
        public void Undo()
        {
            _redoShapes.Clear();
            do
            {
                if (_copiedShapes.Count > 0)
                {
                    _mainForm.Doc.AllShapes.Remove(_copiedShapes[_copiedShapes.Count - 1]);
                    _redoShapes.Add(_copiedShapes[_copiedShapes.Count - 1]);
                    _copiedShapes.Remove(_copiedShapes[_copiedShapes.Count - 1]);
                }
            } while (_copiedShapes.Count > 0);
        }

        /// <summary>
        /// Redo copy figure
        /// </summary>
        public void Redo()
        {
            _mainForm.Doc.AllShapes.AddRange(_redoShapes);
            _copiedShapes.AddRange(_redoShapes);
        }
    }
}