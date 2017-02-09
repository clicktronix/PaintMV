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
        public readonly List<Shape> CutedShapes = new List<Shape>();
        private int _listLenght;

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
            for (int i = _mainForm.Doc.AllShapes.Count - 1; i >= 0; i--)
            {
                if (_mainForm.IndexOfSelectedShape != null && _mainForm.Doc.AllShapes[i].GetShapeIsSelected() && _mainForm.IndexOfSelectedShape != null)
                {
                    _listLenght++;
                    var obj = _mainForm.Doc.AllShapes[_mainForm.Doc.AllShapes.Count - 1];
                    obj.SetShapeIsSelected(false);
                    _mainForm.Doc.AllShapes[i].SetShapeIsSelected(false);
                    CutedShapes.Add(obj);
                    _mainForm.Doc.AllShapes.Remove(_mainForm.Doc.AllShapes[i]);
                }
            }
        }

        /// <summary>
        /// Undo cut figure
        /// </summary>
        public void Undo()
        {
            _mainForm.Doc.AllShapes.AddRange(CutedShapes);
        }

        /// <summary>
        /// Redo cut figure
        /// </summary>
        public void Redo()
        {
            if (_mainForm.Doc.AllShapes.Count > _listLenght)
            {
                _mainForm.Doc.AllShapes.RemoveRange(_mainForm.Doc.AllShapes.Count - _listLenght, _listLenght);
            }
        }
    }
}