﻿using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PaintMV.GUI;
using PaintMV.Shapes;

namespace PaintMV.Controls
{
    /// <summary>
    /// Changing class sizes shapes and moving it on the form
    /// </summary>
    public class MoveResize : ICommand
    {
        private readonly MainForm _mainForm;
        private readonly List<List<Shape>> _currentLists = new List<List<Shape>>();
        private readonly List<List<Shape>> _previousLists = new List<List<Shape>>();

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="mainForm"></param>
        public MoveResize(MainForm mainForm)
        {
            _mainForm = mainForm;
        }

        /// <summary>
        /// Method changing sizes shapes and moving it on the form
        /// </summary>
        /// <param name="g"></param>
        /// <param name="e"></param>
        /// <param name="tempShape"></param>
        public void Execute(Graphics g, MouseEventArgs e, Shape tempShape)
        {
            switch (_mainForm.ShapeSelection.NodeSelected)
            {
                case Enumerations.Positions.LeftUp:
                    tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - _mainForm.StartPoint.X, tempShape.StartOrigin.Y);
                    tempShape.Width -= e.X - _mainForm.StartPoint.X;
                    tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y + e.Y - _mainForm.StartPoint.Y);
                    tempShape.Height -= e.Y - _mainForm.StartPoint.Y;
                    break;
                case Enumerations.Positions.LeftMiddle:
                    tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - _mainForm.StartPoint.X, tempShape.StartOrigin.Y);
                    tempShape.Width -= (e.X - _mainForm.StartPoint.X);
                    break;
                case Enumerations.Positions.LeftBottom:
                    tempShape.Width -= e.X - _mainForm.StartPoint.X;
                    tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - _mainForm.StartPoint.X, tempShape.StartOrigin.Y);
                    tempShape.Height += e.Y - _mainForm.StartPoint.Y;
                    break;
                case Enumerations.Positions.BottomMiddle:
                    tempShape.Height += e.Y - _mainForm.StartPoint.Y;
                    break;
                case Enumerations.Positions.RightUp:
                    tempShape.Width += e.X - _mainForm.StartPoint.X;
                    tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y + e.Y - _mainForm.StartPoint.Y);
                    tempShape.Height -= e.Y - _mainForm.StartPoint.Y;
                    break;
                case Enumerations.Positions.RightBottom:
                    tempShape.Width += e.X - _mainForm.StartPoint.X;
                    tempShape.Height += e.Y - _mainForm.StartPoint.Y;
                    break;
                case Enumerations.Positions.RightMiddle:
                    tempShape.Width += e.X - _mainForm.StartPoint.X;
                    break;
                case Enumerations.Positions.UpMiddle:
                    tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y + e.Y - _mainForm.StartPoint.Y);
                    tempShape.Height -= e.Y - _mainForm.StartPoint.Y;
                    tempShape.EndOrigin = new Point(tempShape.EndOrigin.X, tempShape.EndOrigin.Y + e.Y - _mainForm.StartPoint.Y);
                    break;
                default:
                    if (_mainForm.MMove)
                    {
                        tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - _mainForm.StartPoint.X, tempShape.StartOrigin.Y);
                        tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y + e.Y - _mainForm.StartPoint.Y);
                        tempShape.EndOrigin = new Point(tempShape.EndOrigin.X + e.X - _mainForm.StartPoint.X, tempShape.EndOrigin.Y);
                        tempShape.EndOrigin = new Point(tempShape.EndOrigin.X, tempShape.EndOrigin.Y + e.Y - _mainForm.StartPoint.Y);
                        _mainForm.PnlGraphic.Cursor = Cursors.SizeAll;
                    }
                    break;
            }
        }

        /// <summary>
        /// Undo changing shape
        /// </summary>
        public void Undo()
        {
            _mainForm.Doc.AllShapes.Clear();
            if (_previousLists[_previousLists.Count - 1].Count > 0)
            {
                _mainForm.Doc.AllShapes = new List<Shape>(_previousLists[_previousLists.Count - 1]);
                _currentLists.Add(_previousLists[_previousLists.Count - 1]);
                _previousLists.Remove(_previousLists[_previousLists.Count - 1]);
            }
        }

        /// <summary>
        /// Redo changing shape
        /// </summary>
        public void Redo()
        {
            _mainForm.Doc.AllShapes.Clear();
            if (_currentLists[_currentLists.Count - 1].Count > 0)
            {
                _mainForm.Doc.AllShapes = new List<Shape>(_currentLists[_currentLists.Count - 1]);
                _previousLists.Add(_currentLists[_currentLists.Count - 1]);
                _currentLists.Remove(_currentLists[_currentLists.Count - 1]);
            }
        }

        /// <summary>
        /// Update undo lists
        /// </summary>
        public void ExecuteUndo()
        {
            var undoShapesList = new List<Shape>(_mainForm.CopiedShapes);
            _previousLists.Add(undoShapesList);
        }

        /// <summary>
        /// Update redo lists
        /// </summary>
        public void ExecuteRedo()
        {
            var redoShapesList = new List<Shape>(_mainForm.Doc.AllShapes);
            _currentLists.Add(redoShapesList);
        }

        /// <summary>
        /// Clear added lists, when MouseMove method called
        /// </summary>
        public void IncrementCurrentLists()
        {
            if (_currentLists.Count > 1)
            {
                _currentLists.Remove(_currentLists[_currentLists.Count - 2]);
            }
        }
    }
}