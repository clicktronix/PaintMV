using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using PaintMV.Enumerations;
using PaintMV.GUI;
using PaintMV.Shapes;

namespace PaintMV.Controls
{
    /// <summary>
    /// Figure drawing class
    /// </summary>
    public class DrawShape : ICommand
    {
        private readonly MainForm _mainForm;
        private readonly List<Shape> _redoShapes = new List<Shape>();
        private readonly List<Point> _newPolygon = new List<Point>();
        private readonly List<List<Shape>> _currentLists = new List<List<Shape>>();
        private readonly List<List<Shape>> _previousLists = new List<List<Shape>>();
        private string _operationName;
        private int _absShapeWidth;
        private int _absShapeHeight;
        private bool _drawPolygon;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="mainForm"></param>
        public DrawShape(MainForm mainForm)
        {
            _mainForm = mainForm;
        }

        /// <summary>
        /// Method of determining the selected shape
        /// </summary>
        /// <param name="absShapeWidth"></param>
        /// <param name="absShapeHeight"></param>
        public void SetShapeParameters(int absShapeWidth, int absShapeHeight)
        {
            _absShapeWidth = absShapeWidth;
            _absShapeHeight = absShapeHeight;
        }

        /// <summary>
        /// Method of drawing the selected shape
        /// </summary>
        /// <param name="g"></param>
        /// <param name="e"></param>
        /// <param name="tempShape"></param>
        public void Execute(Graphics g, MouseEventArgs e, Shape tempShape)
        {
            Point tempStartPoint;
            _mainForm.LineStyleChose.LineStyle();
            if (_mainForm.ShapesEnum == ShapesEnum.Ellipse || _mainForm.ShapesEnum == ShapesEnum.Rectangle || 
                _mainForm.ShapesEnum == ShapesEnum.Triangle || _mainForm.ShapesEnum == ShapesEnum.SelectRectangle)
            {
                if (_mainForm.ShapeWidth >= 0 || _mainForm.ShapeHeight >= 0)
                {
                    if (_mainForm.ShapeWidth < 0 && _mainForm.ShapeHeight > 0)
                    {
                        tempStartPoint = new Point(_mainForm.StartPoint.X - _absShapeWidth, _mainForm.StartPoint.Y);
                    }
                    else if (_mainForm.ShapeWidth > 0 && _mainForm.ShapeHeight < 0)
                    {
                        tempStartPoint = new Point(_mainForm.StartPoint.X, _mainForm.StartPoint.Y - _absShapeHeight);
                    }
                    else { tempStartPoint = new Point(_mainForm.StartPoint.X, _mainForm.StartPoint.Y); }
                }
                else
                {
                    tempStartPoint = new Point(_mainForm.StartPoint.X - _absShapeWidth, _mainForm.StartPoint.Y - _absShapeHeight);
                }
                if (_mainForm.ShapesEnum == ShapesEnum.Ellipse)
                {
                    _mainForm.Figure = new Ellipse(tempStartPoint, _absShapeWidth, _absShapeHeight, _mainForm.ChosenColor, 
                        _mainForm.FillColor, _mainForm.ShapeSize, _mainForm.PenStyle, false);
                    _operationName = "Add shape";
                }
                if (_mainForm.ShapesEnum == ShapesEnum.Rectangle)
                {
                    _mainForm.Figure = new Shapes.Rectangle(tempStartPoint, _absShapeWidth, _absShapeHeight, _mainForm.ChosenColor, 
                        _mainForm.FillColor, _mainForm.ShapeSize, _mainForm.PenStyle, false);
                    _operationName = "Add shape";
                }
                if (_mainForm.ShapesEnum == ShapesEnum.Triangle)
                {
                    _mainForm.Figure = new Triangle(tempStartPoint, _absShapeWidth, _absShapeHeight, _mainForm.ChosenColor,
                        _mainForm.FillColor, _mainForm.ShapeSize, _mainForm.PenStyle, false);
                    _operationName = "Add shape";
                }
                if (_mainForm.ShapesEnum == ShapesEnum.SelectRectangle)
                {
                    _mainForm.Figure = new Shapes.Rectangle(tempStartPoint, _absShapeWidth, _absShapeHeight, Color.Blue, 
                        Color.Empty, 1, DashStyle.Dash, false);
                }
            }
            else if (_mainForm.ShapesEnum == ShapesEnum.Line)
            {
                tempStartPoint = _mainForm.StartPoint;
                Point tempEndPoint = _mainForm.EndPoint;
                _mainForm.Figure = new Line(tempStartPoint, tempEndPoint, _mainForm.ChosenColor, _mainForm.ShapeSize, 
                    _mainForm.PenStyle, false, true, _absShapeWidth, _absShapeHeight);
                _operationName = "Add shape";
            }
            else if (_mainForm.ShapesEnum == ShapesEnum.Polygon)
            {
                if (_mainForm.MMove) return;
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        _newPolygon.Add(new Point(e.X, e.Y));
                        if (_newPolygon.Count > 0)
                        {
                            _mainForm.CompletedPolygon = true;
                            _drawPolygon = false;
                            _mainForm.Figure = new Polygon(_newPolygon.ToArray(), Color.Green, 1, DashStyle.Dash, false, true, _drawPolygon);
                            _mainForm.PolyLinesCount++;
                        }
                        break;

                    case MouseButtons.Right:
                        CompletePolygon();
                        break;
                }
            }
            else if (_mainForm.ShapesEnum == ShapesEnum.None) { }
            else { throw new ArgumentOutOfRangeException(); }
        }

        public void CompletePolygon()
        {
            if (_newPolygon.Count > 1)
            {
                _drawPolygon = true;
                _mainForm.CompletedPolygon = false;
                _mainForm.DeletePolyLines();
                _mainForm.Figure = new Polygon(_newPolygon.ToArray(), _mainForm.ChosenColor,
                    _mainForm.ShapeSize, _mainForm.PenStyle, false, true, _drawPolygon);
                _newPolygon.Clear();
                _mainForm.PolyLinesCount = 0;
                _operationName = "Add shape";
            }
            else
            {
                MessageBox.Show(@"Need more points");
            }
        }

        /// <summary>
        /// Undo drawing the selected shape
        /// </summary>
        public void Undo()
        {
            //if (_mainForm.Doc.AllShapes.Count > 0)
            //{
            //    var item = _mainForm.Doc.AllShapes[_mainForm.Doc.AllShapes.Count - 1];
            //    _mainForm.Doc.AllShapes.Remove(_mainForm.Doc.AllShapes[_mainForm.Doc.AllShapes.Count - 1]);
            //    _redoShapes.Add(item);
            //}
            if (_previousLists.Count > 0)
            {
                _currentLists.Add(_mainForm.Doc.AllShapes);
                _mainForm.Doc.AllShapes = new List<Shape>(_previousLists[_previousLists.Count - 1]);
                _previousLists.Remove(_previousLists[_previousLists.Count - 1]);
            }
        }

        /// <summary>
        /// Redo drawing the selected shape
        /// </summary>
        public void Redo()
        {
            //if (_redoShapes.Count > 0)
            //{
            //    var item = _redoShapes[_redoShapes.Count - 1];
            //    _mainForm.Doc.AllShapes.Add(item);
            //    _redoShapes.Remove(_redoShapes[_redoShapes.Count - 1]);
            //}
            if (_currentLists.Count > 0)
            {
                _previousLists.Add(_mainForm.Doc.AllShapes);
                _mainForm.Doc.AllShapes = new List<Shape>(_currentLists[_currentLists.Count - 1]);
                _currentLists.Remove(_currentLists[_currentLists.Count - 1]);
            }
        }

        /// <summary>
        /// Name of the operation
        /// </summary>
        /// <returns></returns>
        public string Operation()
        {
            return _operationName;
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