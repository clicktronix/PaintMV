using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PaintMV.GUI;
using PaintMV.Shapes;

namespace PaintMV.Controls
{
    /// <summary>
    /// Class updates the parameters of the figure
    /// </summary>
    public class UpdateShape : ICommand
    {
#region Properties
        private readonly MainForm _mainForm;
        private readonly List<List<Shape>> _currentLists = new List<List<Shape>>();
        private readonly List<List<Shape>> _previousLists = new List<List<Shape>>();
        private readonly List<List<Shape>> _redoLists = new List<List<Shape>>();
        private readonly List<List<Shape>> _undoLists = new List<List<Shape>>();
        private Shape _a;
        private Shape _b;
#endregion

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="mainForm"></param>
        public UpdateShape(MainForm mainForm)
        {
            _mainForm = mainForm;
        }

        /// <summary>
        /// Method updates the parameters of the figure
        /// </summary>
        /// <param name="g"></param>
        /// <param name="e"></param>
        /// <param name="tempShape"></param>
        public void Execute(Graphics g, MouseEventArgs e, Shape tempShape)
        {
            var redoShapes = new List<Shape>();
            var undoShapes = new List<Shape>();
            for (int i = _mainForm.Doc.AllShapes.Count - 1; i >= 0; i--)
            {
                if (_mainForm.IndexOfSelectedShape != null && _mainForm.Doc.AllShapes[i].GetShapeIsSelected() && _mainForm.IndexOfSelectedShape != null)
                {
                    _mainForm.LineStyleChose.LineStyle();
                    _a = _mainForm.Doc.AllShapes[i].Clone();
                    _b = _mainForm.Doc.AllShapes[i].Clone();
                    _mainForm.Doc.AllShapes.Remove(_mainForm.Doc.AllShapes[i]);
                    _a.ChosenColor = _mainForm.ChosenColor;
                    _a.FilledShape = _mainForm.chBoxFill.Checked;
                    _a.ShapeSize = (int) _mainForm.numSize.Value;
                    _a.PenStyle = _mainForm.PenStyle;

                    _mainForm.Doc.AllShapes.Add(_a);
                    undoShapes.Add(_b);
                    redoShapes.Add(_a);
                }
            }
            _currentLists.Add(redoShapes);
            _previousLists.Add(undoShapes);
        }

        /// <summary>
        /// Undo new parameters
        /// </summary>
        public void Undo()
        {
            if (_mainForm.Doc.AllShapes.Count >= _currentLists[_currentLists.Count - 1].Count)
            {
                _mainForm.Doc.AllShapes.RemoveRange(_mainForm.Doc.AllShapes.Count - _currentLists[_currentLists.Count - 1].Count, _currentLists[_currentLists.Count - 1].Count);
            }
            _mainForm.Doc.AllShapes.AddRange(_previousLists[_previousLists.Count - 1]);
            _undoLists.Add(_previousLists[_previousLists.Count - 1]);
            _redoLists.Add(_currentLists[_currentLists.Count - 1]);
            _previousLists.Remove(_previousLists[_previousLists.Count - 1]);
            _currentLists.Remove(_currentLists[_currentLists.Count - 1]);
        }

        /// <summary>
        /// Redo new parameters
        /// </summary>
        public void Redo()
        {
            _previousLists.Add(_undoLists[_undoLists.Count - 1]);
            _currentLists.Add(_redoLists[_redoLists.Count - 1]);
            _undoLists.Remove(_undoLists[_undoLists.Count - 1]);
            _redoLists.Remove(_redoLists[_redoLists.Count - 1]);
            if (_mainForm.Doc.AllShapes.Count >= _previousLists[_previousLists.Count - 1].Count)
            {
                _mainForm.Doc.AllShapes.RemoveRange(_mainForm.Doc.AllShapes.Count - _previousLists[_previousLists.Count - 1].Count, _previousLists[_previousLists.Count - 1].Count);
            }
            _mainForm.Doc.AllShapes.AddRange(_currentLists[_currentLists.Count - 1]);
        }
    }
}