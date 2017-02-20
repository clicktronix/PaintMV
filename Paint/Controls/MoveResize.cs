using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        private string _operationName;
        public int PolygonPoint;

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
            _operationName = "Move/Resize figure";
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
                    break;

                case Enumerations.Positions.LeftLinePoint:
                    tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - _mainForm.StartPoint.X, tempShape.StartOrigin.Y);
                    tempShape.Width -= e.X - _mainForm.StartPoint.X;
                    tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y + e.Y - _mainForm.StartPoint.Y);
                    tempShape.Height -= e.Y - _mainForm.StartPoint.Y;
                    break;

                case Enumerations.Positions.RightLinePoint:
                    tempShape.EndOrigin = new Point(tempShape.EndOrigin.X + e.X - _mainForm.StartPoint.X, tempShape.EndOrigin.Y);
                    tempShape.Width -= e.X - _mainForm.StartPoint.X;
                    tempShape.EndOrigin = new Point(tempShape.EndOrigin.X, tempShape.EndOrigin.Y + e.Y - _mainForm.StartPoint.Y);
                    tempShape.Height -= e.Y - _mainForm.StartPoint.Y;
                    break;

                case Enumerations.Positions.PolygonPoint:
                    var pointsArrayList = new List<Point>(tempShape.PointsArray.ToList());
                    if (tempShape.PointsArray.Length > PolygonPoint)
                    {
                        pointsArrayList[PolygonPoint] = new Point(tempShape.PointsArray[PolygonPoint].X + e.X - _mainForm.StartPoint.X,
                            tempShape.PointsArray[PolygonPoint].Y + e.Y - _mainForm.StartPoint.Y);
                    }
                    tempShape.PointsArray = new List<Point>(pointsArrayList).ToArray();
                    break;

                default:
                    if (_mainForm.MMove)
                    {
                        tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - _mainForm.StartPoint.X, tempShape.StartOrigin.Y + e.Y - _mainForm.StartPoint.Y);
                        tempShape.EndOrigin = new Point(tempShape.EndOrigin.X + e.X - _mainForm.StartPoint.X, tempShape.EndOrigin.Y + e.Y - _mainForm.StartPoint.Y);
                        if (tempShape.PointsArray != null)
                        {
                            var points = new List<Point>();
                            for (var i = tempShape.PointsArray.Length - 1; i > - 1; i--)
                            {
                                points.Add(new Point(tempShape.PointsArray[i].X + e.X - _mainForm.StartPoint.X,
                                    tempShape.PointsArray[i].Y + e.Y - _mainForm.StartPoint.Y));
                            }
                            tempShape.PointsArray = new List<Point>(points).ToArray();
                        }
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
            if (_previousLists.Count > 0)
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
            if (_currentLists.Count > 0)
            {
                _previousLists.Add(_currentLists[_currentLists.Count - 1]);
                _currentLists.Remove(_currentLists[_currentLists.Count - 1]);
                _mainForm.Doc.AllShapes = new List<Shape>(_currentLists[_currentLists.Count - 1]);
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