using System;
using System.Collections.Generic;
using PaintMV.Shapes;

namespace PaintMV
{
    [Serializable]
    public class Document
    {
        public List<Shape> allShapes = new List<Shape>();
    }
}
