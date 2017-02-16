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
        private string _operationName;
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
            var undoShapes = new List<Shape>();
            var redoShapes = new List<Shape>();
            for (int i = _mainForm.Doc.AllShapes.Count - 1; i >= 0; i--)
            {
                var shapeA = _mainForm.Doc.AllShapes[i].Clone();
                undoShapes.Add(shapeA);
                if (_mainForm.IndexOfSelectedShape != null && _mainForm.Doc.AllShapes[i].GetShapeIsSelected() && _mainForm.IndexOfSelectedShape != null)
                {
                    _mainForm.LineStyleChose.LineStyle();
                    _mainForm.Doc.AllShapes[i].ChosenColor = _mainForm.ChosenColor;
                    _mainForm.Doc.AllShapes[i].FillColor = _mainForm.FillColor;
                    _mainForm.Doc.AllShapes[i].ShapeSize = (int) _mainForm.numSize.Value;
                    _mainForm.Doc.AllShapes[i].PenStyle = _mainForm.PenStyle;
                }
                var shapeB = _mainForm.Doc.AllShapes[i].Clone();
                redoShapes.Add(shapeB);
            }
            _currentLists.Add(redoShapes);
            _previousLists.Add(undoShapes);
            _operationName = "Update shapes";
        }

        /// <summary>
        /// Undo new parameters
        /// </summary>
        public void Undo()
        {
            _mainForm.Doc.AllShapes.Clear();
            _mainForm.Doc.AllShapes = new List<Shape>(_previousLists[_previousLists.Count - 1]);
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
            _mainForm.Doc.AllShapes.Clear();
            _mainForm.Doc.AllShapes = new List<Shape>(_currentLists[_currentLists.Count - 1]);
        }

        /// <summary>
        /// Name of the operation
        /// </summary>
        /// <returns></returns>
        public string Operation()
        {
            return _operationName;
        }
    }
}