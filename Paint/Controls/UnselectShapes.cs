using PaintMV.GUI;

namespace PaintMV.Controls
{
    /// <summary>
    /// Class undo selection figures
    /// </summary>
    public class UnselectShapes
    {
        private readonly DrawHandlers _drawHandlers;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="drawHandlers"></param>
        public UnselectShapes(DrawHandlers drawHandlers)
        {
            _drawHandlers = drawHandlers;
        }

        /// <summary>
        /// Set flag IsSelected false of all shapes in list
        /// </summary>
        public void UnselectAllShapes()
        {
            for (int i = _drawHandlers.ShapesList.Count - 1; i >= 0; i--)
            {
                _drawHandlers.ShapesList[i].SetShapeIsSelected(false);
            }
        }
    }
}