using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        public bool EndOfPolygon;
        private readonly MainForm _mainForm;
        private readonly List<Shape> _redoShapes = new List<Shape>();
        private readonly List<Point> _newPolygon = new List<Point>();
        private string _operationName;
        private int _absShapeWidth;
        private int _absShapeHeight;

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
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        _newPolygon.Add(new Point(e.X, e.Y));
                        if (_newPolygon.Count > 0)
                        {
                            EndOfPolygon = false;
                            _mainForm.Figure = new Polygon(_newPolygon, Color.Green,
                                1, DashStyle.Dash, false, true, EndOfPolygon);
                        }
                        break;

                    case MouseButtons.Right:
                        if (_newPolygon.Count > 1)
                        {
                            EndOfPolygon = true;
                            _mainForm.Figure = new Polygon(_newPolygon, _mainForm.ChosenColor,
                                _mainForm.ShapeSize, _mainForm.PenStyle, false, true, EndOfPolygon);
                            _newPolygon.Clear();
                            _operationName = "Add shape";
                        }
                        break;
                }
            }
            else if (_mainForm.ShapesEnum == ShapesEnum.None) { }
            else { throw new ArgumentOutOfRangeException(); }
        }

        /// <summary>
        /// Undo drawing the selected shape
        /// </summary>
        public void Undo()
        {
            if (_mainForm.Doc.AllShapes.Count > 0)
            {
                var item = _mainForm.Doc.AllShapes[_mainForm.Doc.AllShapes.Count - 1];
                _mainForm.Doc.AllShapes.Remove(_mainForm.Doc.AllShapes[_mainForm.Doc.AllShapes.Count - 1]);
                _redoShapes.Add(item);
            }
        }

        /// <summary>
        /// Redo drawing the selected shape
        /// </summary>
        public void Redo()
        {
            if (_redoShapes.Count > 0)
            {
                var item = _redoShapes[_redoShapes.Count - 1];
                _mainForm.Doc.AllShapes.Add(item);
                _redoShapes.Remove(_redoShapes[_redoShapes.Count - 1]);
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
    }
}