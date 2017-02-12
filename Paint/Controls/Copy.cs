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
        private readonly List<List<Shape>> _redoLists = new List<List<Shape>>();
        private readonly List<List<Shape>> _undoLists = new List<List<Shape>>();

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
            List<Shape> copiedShapes = new List<Shape>();
            for (int i = _mainForm.Doc.AllShapes.Count - 1; i >= 0; i--)
            {
                if (_mainForm.IndexOfSelectedShape != null && _mainForm.Doc.AllShapes[i].GetShapeIsSelected() && _mainForm.IndexOfSelectedShape != null)
                {
                    _mainForm.Doc.AllShapes[i].SetShapeIsSelected(false);
                    var copiedShape = _mainForm.Doc.AllShapes[i].Clone();
                    Point a = Point.Empty;
                    a.X = copiedShape.StartOrigin.X + 15;
                    a.Y = copiedShape.StartOrigin.Y + 15;
                    copiedShape.StartOrigin = a;
                    copiedShapes.Add(copiedShape);
                }
            }
            _mainForm.Doc.AllShapes.AddRange(copiedShapes);
            _undoLists.Add(copiedShapes);
        }

        /// <summary>
        /// Undo copy figure
        /// </summary>
        public void Undo()
        {
            var shapes = new List<Shape>(_undoLists[_undoLists.Count - 1]);
            do
            {
                if (shapes.Count > 0)
                {
                    _mainForm.Doc.AllShapes.Remove(shapes[shapes.Count - 1]);
                    shapes.Remove(shapes[shapes.Count - 1]);
                }
            } while (shapes.Count > 0);
            _redoLists.Add(_undoLists[_undoLists.Count - 1]);
            _undoLists.Remove(_undoLists[_undoLists.Count - 1]);
        }

        /// <summary>
        /// Redo copy figure
        /// </summary>
        public void Redo()
        {
            if (_redoLists.Count > 0)
            {
                _mainForm.Doc.AllShapes.AddRange(_redoLists[_redoLists.Count - 1]);
            }
            _undoLists.Add(_redoLists[_redoLists.Count - 1]);
            _redoLists.Remove(_redoLists[_redoLists.Count - 1]);
        }
    }
}