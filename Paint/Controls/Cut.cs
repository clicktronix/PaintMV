using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PaintMV.GUI;
using PaintMV.Shapes;

namespace PaintMV.Controls
{
    /// <summary>
    /// Cut figures class
    /// </summary>
    public class Cut : ICommand
    {
        private readonly MainForm _mainForm;
        private readonly List<List<Shape>> _currentLists = new List<List<Shape>>();
        private readonly List<List<Shape>> _previousLists = new List<List<Shape>>();
        private readonly List<List<Shape>> _redoLists = new List<List<Shape>>();
        private readonly List<List<Shape>> _undoLists = new List<List<Shape>>();

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="mainForm"></param>
        public Cut(MainForm mainForm)
        {
            _mainForm = mainForm;
        }

        /// <summary>
        /// Cut selected figure metod
        /// </summary>
        /// <param name="g"></param>
        /// <param name="e"></param>
        /// <param name="tempShape"></param>
        public void Execute(Graphics g, MouseEventArgs e, Shape tempShape)
        {
            List<Shape> ñutedShapes = new List<Shape>();
            for (int i = _mainForm.Doc.AllShapes.Count - 1; i >= 0; i--)
            {
                if (_mainForm.IndexOfSelectedShape != null && _mainForm.Doc.AllShapes[i].GetShapeIsSelected() && _mainForm.IndexOfSelectedShape != null)
                {
                    var obj = _mainForm.Doc.AllShapes[_mainForm.Doc.AllShapes.Count - 1];
                    obj.SetShapeIsSelected(false);
                    _mainForm.Doc.AllShapes[i].SetShapeIsSelected(false);
                    ñutedShapes.Add(obj);
                    _mainForm.Doc.AllShapes.Remove(_mainForm.Doc.AllShapes[i]);
                }
            }
            _undoLists.Add(ñutedShapes);
        }

        /// <summary>
        /// Undo cut figure
        /// </summary>
        public void Undo()
        {
            _mainForm.Doc.AllShapes.AddRange(_undoLists[_undoLists.Count - 1]);
            _redoLists.Add(_undoLists[_undoLists.Count - 1]);
            _undoLists.Remove(_undoLists[_undoLists.Count - 1]);
        }

        /// <summary>
        /// Redo cut figure
        /// </summary>
        public void Redo()
        {
            if (_mainForm.Doc.AllShapes.Count >= _redoLists[_redoLists.Count - 1].Count)
            {
                _mainForm.Doc.AllShapes.RemoveRange(_mainForm.Doc.AllShapes.Count - _redoLists[_redoLists.Count - 1].Count, _redoLists[_redoLists.Count - 1].Count);
                _undoLists.Add(_redoLists[_redoLists.Count - 1]);
            }
        }
    }
}