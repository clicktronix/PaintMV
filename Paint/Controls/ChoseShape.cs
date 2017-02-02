using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using PaintMV.Enumerations;
using PaintMV.GUI;
using PaintMV.Shapes;

namespace PaintMV.Controls
{
    public class ChoseShape
    {
        private readonly MainForm _mainForm;

        public ChoseShape(MainForm mainForm)
        {
            _mainForm = mainForm;
        }

        /// <summary>
        /// Method of determining the selected shape
        /// </summary>
        /// <param name="absShapeWidth"></param>
        /// <param name="absShapeHeight"></param>
        public void CheckChosenShape(int absShapeWidth, int absShapeHeight)
        {
            Point tempStartPoint;
            _mainForm.LineStyleChose.ChoseLineStyle();
            if (_mainForm.ShapesEnum == ShapesEnum.Ellipse || _mainForm.ShapesEnum == ShapesEnum.Rectangle || _mainForm.ShapesEnum == ShapesEnum.Triangle || _mainForm.ShapesEnum == ShapesEnum.SelectRectangle)
            {
                if (_mainForm.ShapeWidth >= 0 || _mainForm.ShapeHeight >= 0)
                {
                    if (_mainForm.ShapeWidth < 0 && _mainForm.ShapeHeight > 0)
                    {
                        tempStartPoint = new Point(_mainForm.StartPoint.X - absShapeWidth, _mainForm.StartPoint.Y);
                    }
                    else if (_mainForm.ShapeWidth > 0 && _mainForm.ShapeHeight < 0)
                    {
                        tempStartPoint = new Point(_mainForm.StartPoint.X, _mainForm.StartPoint.Y - absShapeHeight);
                    }
                    else { tempStartPoint = new Point(_mainForm.StartPoint.X, _mainForm.StartPoint.Y); }
                }
                else
                {
                    tempStartPoint = new Point(_mainForm.StartPoint.X - absShapeWidth, _mainForm.StartPoint.Y - absShapeHeight);
                }
                if (_mainForm.ShapesEnum == ShapesEnum.Ellipse)
                {
                    _mainForm.Figure = new Ellipse(tempStartPoint, absShapeWidth, absShapeHeight, _mainForm.ChosenColor, _mainForm.ShapeSize, _mainForm.FillShape, _mainForm.PenStyle, false);
                }
                if (_mainForm.ShapesEnum == ShapesEnum.Rectangle)
                {
                    _mainForm.Figure = new Shapes.Rectangle(tempStartPoint, absShapeWidth, absShapeHeight, _mainForm.ChosenColor, _mainForm.ShapeSize, _mainForm.FillShape, _mainForm.PenStyle, false);
                }
                if (_mainForm.ShapesEnum == ShapesEnum.Triangle)
                {
                    _mainForm.Figure = new Triangle(tempStartPoint, absShapeWidth, absShapeHeight, _mainForm.ChosenColor, _mainForm.ShapeSize, _mainForm.FillShape, _mainForm.PenStyle, false);
                }
                if (_mainForm.ShapesEnum == ShapesEnum.SelectRectangle)
                {
                    _mainForm.Figure = new Shapes.Rectangle(tempStartPoint, absShapeWidth, absShapeHeight, Color.Blue, 1, false, DashStyle.Dash, false);
                }
            }
            else if (_mainForm.ShapesEnum == ShapesEnum.Line)
            {
                tempStartPoint = _mainForm.StartPoint;
                Point tempEndPoint = _mainForm.EndPoint;
                _mainForm.Figure = new Line(tempStartPoint, tempEndPoint, absShapeWidth, absShapeHeight, _mainForm.ChosenColor, _mainForm.ShapeSize, _mainForm.PenStyle, false, true);
            }
            else if (_mainForm.ShapesEnum == ShapesEnum.None) { }
            else { throw new ArgumentOutOfRangeException(); }
        }
    }
}