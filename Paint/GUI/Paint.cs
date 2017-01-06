using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using PaintMV.Enumerations;
using PaintMV.Shapes;
using Rectangle = System.Drawing.Rectangle;

namespace PaintMV.GUI
{
    public partial class FrmPaint : Form
    {
        private bool _paintMode = false;
        private int _shapeWidth, _shapeHeight;
        private int _shapeSize = 1;
        private bool _hasShapes = false;

        private bool _fillShape = false;
        private ShapesEnum _shapesEnum;
        private ColorsEnum _color = ColorsEnum.Black;
        private Color _chosenColor;

        private int _sizeNodeRect = 7;
        private bool _mIsClick = false;
        private bool _selectionMode = false;
        private bool _isShapeWasSelected = false;
        private bool _mMove = false;
        private ResizePosition _nodeSelected = ResizePosition.None;
        private int? _indexOfSelectedShape;

        private Point _startPoint;
        private Point _endPoint;
        private ShapesList _doc;
        private readonly ShapesList _redo;
        private Shape _figure;
        private readonly Panel _pnlGraphic;
        private bool _loadedFile = false;
        private bool _buttonDeletePressed = false;

        public FrmPaint()
        {
            InitializeComponent();
            _doc = new ShapesList();
            _redo = new ShapesList();
            _pnlGraphic = new MyPanel();
            Controls.Add(_pnlGraphic);
            btnEllipse.BackColor = Color.White;

            _pnlGraphic.BackColor = Color.White;
            _pnlGraphic.Location = new Point(108, 32);
            _pnlGraphic.Name = "pnlGraphic";
            _pnlGraphic.Size = new Size(715, 520);
            _pnlGraphic.TabIndex = 2;
            _pnlGraphic.Paint += pnlGraphic_Paint;
            _pnlGraphic.MouseClick += pnlGraphic_MouseClick;
            _pnlGraphic.MouseDown += pnlGraphic_MouseDown;
            _pnlGraphic.MouseMove += pnlGraphic_MouseMove;
            _pnlGraphic.MouseUp += pnlGraphic_MouseUp;
        }

        private void pnlGraphic_Paint(object sender, PaintEventArgs e)
        {
            if (_hasShapes && _doc.AllShapes != null)
            {
                foreach (Shape shape in _doc.AllShapes)
                {
                    shape.Draw(e.Graphics);
                }
            }

            if (_loadedFile || _buttonDeletePressed || _selectionMode)
            {
                if (_doc.AllShapes != null)
                    foreach (Shape shape in _doc.AllShapes)
                    {
                        shape.Draw(e.Graphics);
                    }
            }

            if (_isShapeWasSelected)
            {
                if (_shapesEnum == ShapesEnum.Line)
                {
                    if (_doc.AllShapes != null)
                        if (_indexOfSelectedShape != null)
                            MakeSelectionOfLine(_doc.AllShapes[_indexOfSelectedShape.Value], e.Graphics);
                }
                else
                {
                    if (_doc.AllShapes != null)
                        if (_indexOfSelectedShape != null)
                            MakeSelectionOfShape(_doc.AllShapes[_indexOfSelectedShape.Value], e.Graphics);
                }
            }
            if (_paintMode)
            {
                _figure?.Draw(e.Graphics);
            }
        }

        private void pnlGraphic_MouseDown(object sender, MouseEventArgs e)
        {
            _mIsClick = true;

            if (_selectionMode && _doc.AllShapes.Count == 0)
            {
                MessageBox.Show(@"Нет фигур для выбора!");
            }
            else if (_selectionMode && _doc.AllShapes.Count != 0)
            {
                for (int i = _doc.AllShapes.Count - 1; i >= 0; i--)
                {
                    if (_doc.AllShapes[i].ContainsPoint(e.Location))
                    {
                        _isShapeWasSelected = true;
                        Shape tempShape = _doc.AllShapes[i];
                        if (_doc.AllShapes.Count == 2 && tempShape != _doc.AllShapes[_doc.AllShapes.Count - 1])
                        {
                            _doc.AllShapes[i] = _doc.AllShapes[i + 1];
                            _doc.AllShapes[i + 1] = tempShape;
                        }
                        else if (_doc.AllShapes.Count > 2 && tempShape != _doc.AllShapes[_doc.AllShapes.Count - 1])
                        {
                            for (int j = i; j < _doc.AllShapes.Count - 1; j++)
                            {
                                _doc.AllShapes[j] = _doc.AllShapes[j + 1];
                            }
                            _doc.AllShapes[_doc.AllShapes.Count - 1] = tempShape;
                        }
                        _indexOfSelectedShape = _doc.AllShapes.Count - 1;
                        _mMove = true;
                        break;
                    }
                }
                if (_isShapeWasSelected && _indexOfSelectedShape != null)
                {
                    btnDefaultColor.BackColor = _doc.AllShapes[_indexOfSelectedShape.Value].ChosenColor;
                    _nodeSelected = ResizePosition.None;
                    _nodeSelected = GetNodeSelectable(e.Location);
                }
            }
            else if (!_selectionMode)
            {
                _paintMode = true;
            }
            _startPoint.X = e.X;
            _startPoint.Y = e.Y;

            _pnlGraphic.Invalidate();
        }

        private void ChangeColor(Color color)
        {
            if (_isShapeWasSelected && _indexOfSelectedShape != null)
            {
                _doc.AllShapes[_indexOfSelectedShape.Value].ChosenColor = color;
                _doc.AllShapes[_indexOfSelectedShape.Value].FilledShape = chBoxFill.Checked == true;
                _doc.AllShapes[_indexOfSelectedShape.Value].ShapeSize = (int)numSize.Value;
            }
            _pnlGraphic.Invalidate();
        }

        private void pnlGraphic_MouseUp(object sender, MouseEventArgs e)
        {
            _paintMode = false;
            if (!_selectionMode && (e.X - _startPoint.X) != 0)
            {
                _doc.AllShapes.Add(_figure);
                _hasShapes = true;
            }
            _mIsClick = false;
            _mMove = false;
            _figure = null;
            _pnlGraphic.Invalidate();
        }

        private void pnlGraphic_MouseMove(object sender, MouseEventArgs e)
        {
            if (_paintMode)
            {
                _endPoint.X = e.X;
                _endPoint.Y = e.Y;
                _shapeWidth = e.X - _startPoint.X;
                _shapeHeight = e.Y - _startPoint.Y;
                _shapeSize = (int)numSize.Value;
                var absShapeWidth = Math.Abs(_shapeWidth);
                var absShapeHeight = Math.Abs(_shapeHeight);
                Point tempStartPoint;

                _fillShape = chBoxFill.Checked == true;

                // check selected color
                switch (_color)
                {
                    case ColorsEnum.Black:
                        _chosenColor = Color.Black;
                        break;
                    case ColorsEnum.Yellow:
                        _chosenColor = Color.Yellow;
                        break;
                    case ColorsEnum.Red:
                        _chosenColor = Color.Red;
                        break;
                    case ColorsEnum.Blue:
                        _chosenColor = Color.Blue;
                        break;
                    case ColorsEnum.White:
                        _chosenColor = Color.White;
                        break;
                    case ColorsEnum.Green:
                        _chosenColor = Color.Green;
                        break;
                    case ColorsEnum.Orange:
                        _chosenColor = Color.Orange;
                        break;
                    case ColorsEnum.Purple:
                        _chosenColor = Color.Violet;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // check chosen shape
                switch (_shapesEnum)
                {
                    case ShapesEnum.Ellipse:
                    case ShapesEnum.Rectangle:
                    case ShapesEnum.Triangle:
                        if (_shapeWidth >= 0 || _shapeHeight >= 0)
                        {
                            if (_shapeWidth < 0 && _shapeHeight > 0)
                            {
                                tempStartPoint = new Point(_startPoint.X - absShapeWidth, _startPoint.Y);
                            }
                            else if (_shapeWidth > 0 && _shapeHeight < 0)
                            {
                                tempStartPoint = new Point(_startPoint.X, _startPoint.Y - absShapeHeight);
                            }
                            else
                            {
                                tempStartPoint = new Point(_startPoint.X, _startPoint.Y);
                            }
                        }
                        else
                        {
                            tempStartPoint = new Point(_startPoint.X - absShapeWidth, _startPoint.Y - absShapeHeight);
                        }
                        if (_shapesEnum == ShapesEnum.Ellipse)
                        {
                            _figure = new Shapes.Ellipse(tempStartPoint, absShapeWidth, absShapeHeight, _chosenColor,
                                _shapeSize, _fillShape);
                        }
                        if (_shapesEnum == ShapesEnum.Rectangle)
                        {
                            _figure = new Shapes.Rectangle(tempStartPoint, absShapeWidth, absShapeHeight, _chosenColor, _shapeSize, _fillShape);
                        }
                        if (_shapesEnum == ShapesEnum.Triangle)
                        {
                            _figure = new Shapes.Triangle(tempStartPoint, absShapeWidth, absShapeHeight, _chosenColor, _shapeSize, _fillShape);
                        }
                        break;
                    case ShapesEnum.Line:
                        tempStartPoint = _startPoint;
                        var tempEndPoint = _endPoint;

                        _figure = new Shapes.Line(tempStartPoint, tempEndPoint, absShapeWidth, absShapeHeight, _chosenColor, _shapeSize);
                        break;
                    case ShapesEnum.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (_selectionMode && _isShapeWasSelected)
            {
                ChangeCursor(e.Location);
                if (_mIsClick == false)
                {
                    return;
                }
                if (_indexOfSelectedShape != null)
                {
                    Shape tempShape = _doc.AllShapes[_indexOfSelectedShape.Value];

                    switch (_nodeSelected)
                    {
                        case ResizePosition.LeftUp:
                            tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - _startPoint.X, tempShape.StartOrigin.Y);
                            tempShape.Width -= e.X - _startPoint.X;
                            tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y + e.Y - _startPoint.Y);
                            tempShape.Height -= e.Y - _startPoint.Y;
                            break;
                        case ResizePosition.LeftMiddle:
                            tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - _startPoint.X, tempShape.StartOrigin.Y);
                            tempShape.Width -= (e.X - _startPoint.X);
                            break;
                        case ResizePosition.LeftBottom:
                            tempShape.Width -= e.X - _startPoint.X;
                            tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - _startPoint.X, tempShape.StartOrigin.Y);
                            tempShape.Height += e.Y - _startPoint.Y;
                            break;
                        case ResizePosition.BottomMiddle:
                            tempShape.Height += e.Y - _startPoint.Y;
                            break;
                        case ResizePosition.RightUp:
                            tempShape.Width += e.X - _startPoint.X;
                            tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y + e.Y - _startPoint.Y);
                            tempShape.Height -= e.Y - _startPoint.Y;
                            break;
                        case ResizePosition.RightBottom:
                            tempShape.Width += e.X - _startPoint.X;
                            tempShape.Height += e.Y - _startPoint.Y;
                            if (_shapesEnum == ShapesEnum.Line)
                            {
                                tempShape.EndOrigin = new Point(e.X - tempShape.EndOrigin.X + _startPoint.X, e.Y - tempShape.EndOrigin.Y + _endPoint.Y);
                            }
                            break;
                        case ResizePosition.RightMiddle:
                            tempShape.Width += e.X - _startPoint.X;
                            break;
                        case ResizePosition.UpMiddle:
                            tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y + e.Y - _startPoint.Y);
                            tempShape.Height -= e.Y - _startPoint.Y;
                            break;
                        default:
                            if (_mMove)
                            {
                                tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - _startPoint.X, tempShape.StartOrigin.Y);
                                tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y + e.Y - _startPoint.Y);
                                _pnlGraphic.Cursor = Cursors.SizeAll;
                            }
                            break;
                    }
                    int qwe = tempShape.Width - (e.X - _startPoint.X);
                }
                int qwe2 = e.X - _startPoint.X;
                int qwe3 = e.X;
                int qwe4 = _startPoint.X;

                _startPoint.X = e.X;
                _startPoint.Y = e.Y;
            }
            if (_hasShapes && _isShapeWasSelected)
            {
                TestIfRectInsideArea();
            }
            _pnlGraphic.Invalidate();
        }

        private void MakeSelectionOfShape(Shape shape, Graphics g)
        {
            for (int i = 0; i < 8; i++)
            {
                g.DrawRectangle(new Pen(Color.Blue), GetRect(i, shape));
            }

            Pen tempPen = new Pen(Color.Blue);
            tempPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            g.DrawRectangle(tempPen, shape.StartOrigin.X - 3, shape.StartOrigin.Y - 3, shape.Width + 6, shape.Height + 6);
        }

        private Rectangle GetRect(int value, Shape shape)
        {
            int xValue = shape.StartOrigin.X;
            int yValue = shape.StartOrigin.Y;

            switch (value)
            {
                case 0:
                    return new Rectangle(xValue - 3 - _sizeNodeRect/2, yValue - 3 - _sizeNodeRect/2, _sizeNodeRect, _sizeNodeRect);

                case 1:
                    return new Rectangle(xValue - 4 - _sizeNodeRect/2, yValue + shape.Height/2 - _sizeNodeRect/2, _sizeNodeRect, _sizeNodeRect);

                case 2:
                    return new Rectangle(xValue - 3 - _sizeNodeRect/2, yValue + 3 + shape.Height - _sizeNodeRect/2, _sizeNodeRect, _sizeNodeRect);

                case 3:
                    return new Rectangle(xValue + shape.Width/2 - _sizeNodeRect/2, yValue + 3 + shape.Height - _sizeNodeRect/2, _sizeNodeRect, _sizeNodeRect);

                case 4:
                    return new Rectangle(xValue + 3 + shape.Width - _sizeNodeRect/2, yValue - 3 - _sizeNodeRect/2, _sizeNodeRect, _sizeNodeRect);

                case 5:
                    return new Rectangle(xValue + 3 + shape.Width - _sizeNodeRect/2, yValue + 3 + shape.Height - _sizeNodeRect/2, _sizeNodeRect, _sizeNodeRect);

                case 6:
                    return new Rectangle(xValue + 3 + shape.Width - _sizeNodeRect/2, yValue + shape.Height/2 - _sizeNodeRect/2, _sizeNodeRect, _sizeNodeRect);

                case 7:
                    return new Rectangle(xValue + shape.Width/2 - _sizeNodeRect/2, yValue - 4 - _sizeNodeRect/2, _sizeNodeRect, _sizeNodeRect);
                default:
                    return new Rectangle();
            }
        }

        private void MakeSelectionOfLine(Shape shape, Graphics g)
        {
            int xValue = shape.StartOrigin.X;
            int yValue = shape.StartOrigin.Y;

            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    g.DrawEllipse(new Pen(Color.Blue), xValue - _sizeNodeRect/2, yValue - _sizeNodeRect/2, _sizeNodeRect, _sizeNodeRect);
                }
                else
                {
                    g.DrawEllipse(new Pen(Color.Blue), shape.EndOrigin.X - 3, shape.EndOrigin.Y - 3, _sizeNodeRect, _sizeNodeRect);
                }
            }
            Pen tempPen = new Pen(Color.Blue);
            tempPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            g.DrawLine(tempPen, shape.StartOrigin.X, shape.StartOrigin.Y, shape.EndOrigin.X, shape.EndOrigin.Y);
            _pnlGraphic.Invalidate();
        }
        
        private ResizePosition GetNodeSelectable(Point p)
        {
            foreach (ResizePosition r in Enum.GetValues(typeof (ResizePosition)))
            {
                if (GetRectangle(r).Contains(p))
                {
                    return r;
                }
            }
            return ResizePosition.None;
        }

        private void ChangeCursor(Point p)
        {
            _pnlGraphic.Cursor = GetCursor(GetNodeSelectable(p));
        }

        private Cursor GetCursor(ResizePosition p)
        {
            switch (p)
            {
                case ResizePosition.LeftUp:
                    return Cursors.SizeNWSE;

                case ResizePosition.LeftMiddle:
                    return Cursors.SizeWE;

                case ResizePosition.LeftBottom:
                    return Cursors.SizeNESW;

                case ResizePosition.BottomMiddle:
                    return Cursors.SizeNS;

                case ResizePosition.RightUp:
                    return Cursors.SizeNESW;

                case ResizePosition.RightBottom:
                    return Cursors.SizeNWSE;

                case ResizePosition.RightMiddle:
                    return Cursors.SizeWE;

                case ResizePosition.UpMiddle:
                    return Cursors.SizeNS;
                default:
                    return Cursors.Default;
            }
        }

        private Rectangle GetRectangle(ResizePosition value)
        {
            Debug.Assert(_indexOfSelectedShape != null, "Нет нарисованных фигур!");
            Shape tempShape = _doc.AllShapes[_indexOfSelectedShape.Value];

            switch (value)
            {
                case ResizePosition.LeftUp:
                    return new Rectangle(tempShape.StartOrigin.X - 7, tempShape.StartOrigin.Y - 7, _sizeNodeRect, _sizeNodeRect);

                case ResizePosition.LeftMiddle:
                    return new Rectangle(tempShape.StartOrigin.X - 7, tempShape.StartOrigin.Y + tempShape.Height/2, _sizeNodeRect, _sizeNodeRect);

                case ResizePosition.LeftBottom:
                    return new Rectangle(tempShape.StartOrigin.X - 7, tempShape.StartOrigin.Y + 5 + tempShape.Height, _sizeNodeRect, _sizeNodeRect);

                case ResizePosition.BottomMiddle:
                    return new Rectangle(tempShape.StartOrigin.X + tempShape.Width/2, tempShape.StartOrigin.Y + 5 + tempShape.Height, _sizeNodeRect, _sizeNodeRect);

                case ResizePosition.RightUp:
                    return new Rectangle(tempShape.StartOrigin.X + 5 + tempShape.Width, tempShape.StartOrigin.Y - 7, _sizeNodeRect, _sizeNodeRect);

                case ResizePosition.RightBottom:
                    return new Rectangle(tempShape.StartOrigin.X + 5 + tempShape.Width, tempShape.StartOrigin.Y + 5 + tempShape.Height, _sizeNodeRect, _sizeNodeRect);

                case ResizePosition.RightMiddle:
                    return new Rectangle(tempShape.StartOrigin.X + 5 + tempShape.Width, tempShape.StartOrigin.Y + tempShape.Height/2, _sizeNodeRect, _sizeNodeRect);

                case ResizePosition.UpMiddle:
                    return new Rectangle(tempShape.StartOrigin.X + tempShape.Width/2, tempShape.StartOrigin.Y - 6, _sizeNodeRect, _sizeNodeRect);
                default:
                    return new Rectangle();
            }
        }

        private enum ResizePosition
        {
            UpMiddle,
            LeftMiddle,
            LeftBottom,
            LeftUp,
            RightUp,
            RightMiddle,
            RightBottom,
            BottomMiddle,
            None
        }

        private void btnRectangle_Click(object sender, EventArgs e)
        {
            string button = "rectangle";
            _shapesEnum = ShapesEnum.Rectangle;
            ChangeButtonColor(button);
            _selectionMode = true;
            btnSelection_Click(sender, e);
        }

        private void btnEllipse_Click(object sender, EventArgs e)
        {
            _shapesEnum = ShapesEnum.Ellipse;
            string button = "ellipse";
            ChangeButtonColor(button);
            _selectionMode = true;
            btnSelection_Click(sender, e);
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            _shapesEnum = ShapesEnum.Line;
            string button = "line";
            ChangeButtonColor(button);
            _selectionMode = true;
            btnSelection_Click(sender, e);
        }

        private void btnTriangle_Click(object sender, EventArgs e)
        {
            _shapesEnum = ShapesEnum.Triangle;
            string button = "triangle";
            ChangeButtonColor(button);
            _selectionMode = true;
            btnSelection_Click(sender, e);
        }

        // set current color
        private void btnRed_Click(object sender, EventArgs e)
        {
            _color = ColorsEnum.Red;
            btnDefaultColor.BackColor = Color.Red;
            ChangeColor(Color.Red);
        }

        private void btnYellow_Click(object sender, EventArgs e)
        {
            _color = ColorsEnum.Yellow;
            btnDefaultColor.BackColor = Color.Yellow;
            ChangeColor(Color.Yellow);
        }

        private void btnBlack_Click(object sender, EventArgs e)
        {
            _color = ColorsEnum.Black;
            btnDefaultColor.BackColor = Color.Black;
            ChangeColor(Color.Black);
        }

        private void btnBlue_Click(object sender, EventArgs e)
        {
            _color = ColorsEnum.Blue;
            btnDefaultColor.BackColor = Color.Blue;
            ChangeColor(Color.Blue);
        }

        private void btnWhite_Click(object sender, EventArgs e)
        {
            _color = ColorsEnum.White;
            btnDefaultColor.BackColor = Color.White;
            ChangeColor(Color.White);
        }

        private void btnGreen_Click(object sender, EventArgs e)
        {
            _color = ColorsEnum.Green;
            btnDefaultColor.BackColor = Color.Green;
            ChangeColor(Color.Green);
        }

        private void btnOrange_Click(object sender, EventArgs e)
        {
            _color = ColorsEnum.Orange;
            btnDefaultColor.BackColor = Color.Orange;
            ChangeColor(Color.Orange);
        }

        private void btnPurple_Click(object sender, EventArgs e)
        {
            _color = ColorsEnum.Purple;
            btnDefaultColor.BackColor = Color.Violet;
            ChangeColor(Color.Violet);
        }

        private void menuOpen_Click(object sender, EventArgs e)
        {
            if (_doc.AllShapes.Count > 0)
            {
                const string message = "Do you want to save changes?";
                const string caption = "Paint";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    menuSave_Click(sender, e);
                    OpenDialogForm();
                }
                else if (result == DialogResult.No)
                {
                    OpenDialogForm();
                }
            }
            else
            {
                OpenDialogForm();
            }
        }

        private void OpenDialogForm()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Paint Files | *.pnt";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ShapesList newDoc = new ShapesList();
                _doc = newDoc;

                using (Stream file = openFileDialog.OpenFile())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    _doc = (ShapesList) formatter.Deserialize(file);
                }
                _loadedFile = true;
                _pnlGraphic.Invalidate();
            }
        }

        private void menuNew_Click(object sender, EventArgs e)
        {
            if (_doc.AllShapes.Count > 0)
            {
                const string message = "Do you want to save changes?";
                const string caption = "Save changes";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    menuSave_Click(sender, e);
                    NewBlankPage();
                }
                else if (result == DialogResult.No)
                {
                    NewBlankPage();
                }
            }
            else
            {
                NewBlankPage();
            }
        }

        private void menuSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Paint Files | *.pnt";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (Stream file = saveFileDialog.OpenFile())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(file, _doc);
                }
            }
        }

        private void NewBlankPage()
        {
            ShapesList newDoc = new ShapesList();
            _doc = newDoc;
            _isShapeWasSelected = false;
            _indexOfSelectedShape = null;
            _pnlGraphic.Invalidate();
        }

        private void ChangeButtonColor(string button)
        {
            if (button == "rectangle")
            {
                btnRectangle.BackColor = Color.White;
                btnEllipse.BackColor = Color.Transparent;
                btnLine.BackColor = Color.Transparent;
                btnTriangle.BackColor = Color.Transparent;
            }
            else if (button == "line")
            {
                btnEllipse.BackColor = Color.Transparent;
                btnRectangle.BackColor = Color.Transparent;
                btnLine.BackColor = Color.White;
                btnTriangle.BackColor = Color.Transparent;
            }
            else if (button == "triangle")
            {
                btnEllipse.BackColor = Color.Transparent;
                btnRectangle.BackColor = Color.Transparent;
                btnLine.BackColor = Color.Transparent;
                btnTriangle.BackColor = Color.White;
            }
            else if (button == "ellipse")
            {
                btnEllipse.BackColor = Color.White;
                btnRectangle.BackColor = Color.Transparent;
                btnLine.BackColor = Color.Transparent;
                btnTriangle.BackColor = Color.Transparent;
            }
        }

        private void pnlGraphic_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void frmPaint_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && _isShapeWasSelected)
            {
                if (_indexOfSelectedShape != null)
                {
                    _doc.AllShapes.Remove(_doc.AllShapes[_indexOfSelectedShape.Value]);
                    _isShapeWasSelected = false;
                }
                _indexOfSelectedShape = null;
                _buttonDeletePressed = true;
                _pnlGraphic.Invalidate();
            }
        }

        private void btnSelection_Click(object sender, EventArgs e)
        {
            if (_selectionMode)
            {
                _selectionMode = false;
                _isShapeWasSelected = false;
                btnSelection.Text = "OFF";
            }
            else
            {
                _selectionMode = true;
                btnSelection.Text = "ON";
            }
            _pnlGraphic.Invalidate();
        }

        private void menuUndo_Click(object sender, EventArgs e)
        {
            if (_doc.AllShapes.Count < 1) return;
            var obj = _doc.AllShapes[_doc.AllShapes.Count - 1];
            _doc.AllShapes.Remove(obj);
            _redo.AllShapes.Add(obj);
            _pnlGraphic.Invalidate();
        }

        private void menuRedo_Click(object sender, EventArgs e)
        {
            if (_redo.AllShapes.Count < 1) return;
            var obj = _redo.AllShapes[_redo.AllShapes.Count - 1];
            _redo.AllShapes.Remove(obj);
            _doc.AllShapes.Add(obj);
            _pnlGraphic.Invalidate();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {

        }

        private void menuCopy_Click(object sender, EventArgs e)
        {
            if (_isShapeWasSelected && _indexOfSelectedShape != null)
            {
                Shape copiedFigure = _doc.AllShapes[_indexOfSelectedShape.Value].Clone();
                _doc.AllShapes.Add(copiedFigure);
            }
            _pnlGraphic.Invalidate();
        }

        private void TestIfRectInsideArea()
        {
            Debug.Assert(_indexOfSelectedShape != null, "Фигура не выбрана!");
            Shape tempShape = _doc.AllShapes[_indexOfSelectedShape.Value];
            Point staticPoint;
            staticPoint = new Point();
            staticPoint.X = _pnlGraphic.Width - tempShape.StartOrigin.X;
            staticPoint.Y = tempShape.StartOrigin.Y;

            if (tempShape.StartOrigin.X < 0) tempShape.StartOrigin = new Point(0, tempShape.StartOrigin.Y);
            if (tempShape.StartOrigin.Y < 0) tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, 0);
            if (tempShape.Width <= 1)
            {
                tempShape.Width = 1;
                tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y);
            }
            if (tempShape.Height <= 1)
            {
                tempShape.Height = 1;
                tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y);
            }

            if (tempShape.StartOrigin.X + tempShape.Width > _pnlGraphic.Width)
            {
                tempShape.StartOrigin = new Point(_pnlGraphic.Width - tempShape.Width, tempShape.StartOrigin.Y);
                tempShape.Width = _pnlGraphic.Width - tempShape.StartOrigin.X;
            }
            if (tempShape.StartOrigin.Y + tempShape.Height > _pnlGraphic.Height)
            {
                tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, _pnlGraphic.Height - tempShape.Height);
                tempShape.Height = _pnlGraphic.Height - tempShape.StartOrigin.Y;
            }
            _pnlGraphic.Invalidate();
        }
    }
}
