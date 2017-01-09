using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using PaintMV.Controls;
using PaintMV.Enumerations;
using PaintMV.Shapes;

namespace PaintMV.GUI
{
    public partial class FrmPaint : Form
    {
        private bool _paintMode;
        private int _shapeWidth, _shapeHeight;
        private int _shapeSize = 1;
        private bool _hasShapes;

        private bool _fillShape;
        private ColorsEnum _color = ColorsEnum.Black;
        private Color _chosenColor;

        private bool _mIsClick;
        private bool _selectionMode;
        private bool _isShapeWasSelected;

        private Point _startPoint;
        private Point _endPoint;
        private readonly ShapesList _redo;
        private Shape _figure;
        private bool _loadedFile;
        private bool _buttonDeletePressed;
        private ShapesEnum _shapesEnum;

        public ShapeSelectionByPoint ShapeSelectionByPoint { get; }
        public LineSelectionByPoint LineSelectionByPoint { get; }
        public int SizeNodeRect { set; get; } = 7;
        public int? IndexOfSelectedShape { set; get; }
        public ShapesList Doc { set; get; }
        public Panel PnlGraphic { get; }
        public ResizePosition ResizePosition { get; }
        public bool MMove { set; get; }
        public static int PanelWidth { get; set; } = 700;
        public static int PanelHeight { get; set; } = 500;
        public DashStyle PenStyle = DashStyle.Solid;

        public FrmPaint()
        {
            InitializeComponent();
            Doc = new ShapesList();
            _redo = new ShapesList();
            PnlGraphic = new MyPanel();
            Controls.Add(PnlGraphic);
            btnEllipse.BackColor = Color.White;

            PnlGraphic.BackColor = Color.White;
            PnlGraphic.Location = new Point(108, 32);
            PnlGraphic.Name = "pnlGraphic";
            PnlGraphic.Size = new Size(PanelWidth, PanelHeight);
            PnlGraphic.TabIndex = 2;
            PnlGraphic.Paint += pnlGraphic_Paint;
            PnlGraphic.MouseDown += pnlGraphic_MouseDown;
            PnlGraphic.MouseMove += pnlGraphic_MouseMove;
            PnlGraphic.MouseUp += pnlGraphic_MouseUp;
            ShapeSelectionByPoint = new ShapeSelectionByPoint(this);
            LineSelectionByPoint = new LineSelectionByPoint(this);
            ResizePosition = new ResizePosition(this);
        }

        public Point StartPoint
        {
            set { _startPoint = value; }
            get { return _startPoint; }
        }

        public Point EndPoint
        {
            set { _endPoint = value; }
            get { return _endPoint; }
        }

        public ShapesEnum ShapesEnum
        {
            get
            {
                return _shapesEnum;
            }

            set
            {
                _shapesEnum = value;
            }
        }

        private void pnlGraphic_Paint(object sender, PaintEventArgs e)
        {
            if (_hasShapes && Doc.AllShapes != null)
            {
                foreach (Shape shape in Doc.AllShapes)
                {
                    shape.Draw(e.Graphics);
                }
            }

            if (_loadedFile || _buttonDeletePressed || _selectionMode)
            {
                if (Doc.AllShapes != null)
                    foreach (Shape shape in Doc.AllShapes)
                    {
                        shape.Draw(e.Graphics);
                    }
            }

            if (_isShapeWasSelected)
            {
                if (ShapesEnum == ShapesEnum.Line)
                {
                    if (Doc.AllShapes != null)
                        if (IndexOfSelectedShape != null)
                            LineSelectionByPoint.MakeSelectionOfLine(Doc.AllShapes[IndexOfSelectedShape.Value], e.Graphics);
                }
                else
                {
                    if (Doc.AllShapes != null)
                        if (IndexOfSelectedShape != null)
                            ShapeSelectionByPoint.MakeSelectionOfShape(Doc.AllShapes[IndexOfSelectedShape.Value], e.Graphics);
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

            if (_selectionMode && Doc.AllShapes.Count == 0)
            {
                MessageBox.Show(@"Нет фигур для выбора!");
            }
            else if (_selectionMode && Doc.AllShapes.Count != 0)
            {
                for (int i = Doc.AllShapes.Count - 1; i >= 0; i--)
                {
                    if (Doc.AllShapes[i].ContainsPoint(e.Location))
                    {
                        _isShapeWasSelected = true;
                        Shape tempShape = Doc.AllShapes[i];
                        if (Doc.AllShapes.Count == 2 && tempShape != Doc.AllShapes[Doc.AllShapes.Count - 1])
                        {
                            Doc.AllShapes[i] = Doc.AllShapes[i + 1];
                            Doc.AllShapes[i + 1] = tempShape;
                        }
                        else if (Doc.AllShapes.Count > 2 && tempShape != Doc.AllShapes[Doc.AllShapes.Count - 1])
                        {
                            for (int j = i; j < Doc.AllShapes.Count - 1; j++)
                            {
                                Doc.AllShapes[j] = Doc.AllShapes[j + 1];
                            }
                            Doc.AllShapes[Doc.AllShapes.Count - 1] = tempShape;
                        }
                        IndexOfSelectedShape = Doc.AllShapes.Count - 1;
                        MMove = true;
                        break;
                    }
                }
                if (_isShapeWasSelected && IndexOfSelectedShape != null)
                {
                    btnDefaultColor.BackColor = Doc.AllShapes[IndexOfSelectedShape.Value].ChosenColor;
                    ShapeSelectionByPoint.NodeSelected = ShapeSelectionByPoint.ResizePosition.None;
                    ShapeSelectionByPoint.NodeSelected = ShapeSelectionByPoint.GetNodeSelectable(e.Location);
                }
            }
            else if (!_selectionMode)
            {
                _paintMode = true;
            }
            _startPoint.X = e.X;
            _startPoint.Y = e.Y;
            PnlGraphic.Invalidate();
        }

        private void ChangeColor(Color color)
        {
            if (_isShapeWasSelected && IndexOfSelectedShape != null)
            {
                Doc.AllShapes[IndexOfSelectedShape.Value].ChosenColor = color;
                Doc.AllShapes[IndexOfSelectedShape.Value].FilledShape = chBoxFill.Checked;
            }
            PnlGraphic.Invalidate();
        }

        private void pnlGraphic_MouseUp(object sender, MouseEventArgs e)
        {
            _paintMode = false;
            if (!_selectionMode && (e.X - _startPoint.X) != 0)
            {
                Doc.AllShapes.Add(_figure);
                _hasShapes = true;
            }
            _mIsClick = false;
            MMove = false;
            _figure = null;
            PnlGraphic.Invalidate();
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

                _fillShape = chBoxFill.Checked;
                
                CheckChosenColor();
                
                CheckChosenShape(absShapeWidth, absShapeHeight);
            }
            else if (_selectionMode && _isShapeWasSelected)
            {
                ChangeCursor(e.Location);
                if (_mIsClick == false)
                {
                    return;
                }
                if (IndexOfSelectedShape != null)
                {
                    Shape tempShape = Doc.AllShapes[IndexOfSelectedShape.Value];

                    ResizePosition.GetValueOfResizedPosition(e, tempShape);
                }

                _startPoint.X = e.X;
                _startPoint.Y = e.Y;
            }
            PnlGraphic.Invalidate();
        }

        private void CheckChosenColor()
        {
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
        }

        private void ChoseLineStyle()
        {
            if (radioSolid.Checked)
            {
                PenStyle = DashStyle.Solid;
            }
            else if (radioDash.Checked)
            {
                PenStyle = DashStyle.Dash;
            }
            else if (radioDot.Checked)
            {
                PenStyle = DashStyle.Dot;
            }
        }

        private void CheckChosenShape(int absShapeWidth, int absShapeHeight)
        {
            Point tempStartPoint;
            ChoseLineStyle();
            if (ShapesEnum == ShapesEnum.Ellipse || ShapesEnum == ShapesEnum.Rectangle ||
                ShapesEnum == ShapesEnum.Triangle)
            {
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
                if (ShapesEnum == ShapesEnum.Ellipse)
                {
                    _figure = new Ellipse(tempStartPoint, absShapeWidth, absShapeHeight, _chosenColor, _shapeSize, _fillShape, PenStyle);
                }
                if (ShapesEnum == ShapesEnum.Rectangle)
                {
                    _figure = new Shapes.Rectangle(tempStartPoint, absShapeWidth, absShapeHeight, _chosenColor,
                        _shapeSize, _fillShape, PenStyle);
                }
                if (ShapesEnum == ShapesEnum.Triangle)
                {
                    _figure = new Triangle(tempStartPoint, absShapeWidth, absShapeHeight, _chosenColor, _shapeSize, _fillShape, PenStyle);
                }
            }
            else if (ShapesEnum == ShapesEnum.Line)
            {
                tempStartPoint = _startPoint;
                var tempEndPoint = _endPoint;
                _figure = new Line(tempStartPoint, tempEndPoint, absShapeWidth, absShapeHeight, _chosenColor, _shapeSize, PenStyle);
            }
            else if (ShapesEnum == ShapesEnum.None)
            {
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        private void ChangeCursor(Point p)
        {
            PnlGraphic.Cursor = ShapeSelectionByPoint.GetCursor(ShapeSelectionByPoint.GetNodeSelectable(p));
        }

        // set figure for painting
        private void btnRectangle_Click(object sender, EventArgs e)
        {
            string button = "rectangle";
            ShapesEnum = ShapesEnum.Rectangle;
            ChangeButtonColor(button);
            _selectionMode = true;
            btnSelection_Click(sender, e);
        }

        private void btnEllipse_Click(object sender, EventArgs e)
        {
            ShapesEnum = ShapesEnum.Ellipse;
            string button = "ellipse";
            ChangeButtonColor(button);
            _selectionMode = true;
            btnSelection_Click(sender, e);
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            ShapesEnum = ShapesEnum.Line;
            string button = "line";
            ChangeButtonColor(button);
            _selectionMode = true;
            btnSelection_Click(sender, e);
        }

        private void btnTriangle_Click(object sender, EventArgs e)
        {
            ShapesEnum = ShapesEnum.Triangle;
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
            if (Doc.AllShapes.Count > 0)
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
            openFileDialog.Filter = @"Paint Files | *.pnt";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ShapesList newDoc = new ShapesList();
                Doc = newDoc;

                using (Stream file = openFileDialog.OpenFile())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    Doc = (ShapesList) formatter.Deserialize(file);
                }
                _loadedFile = true;
                PnlGraphic.Invalidate();
            }
        }

        private void menuNew_Click(object sender, EventArgs e)
        {
            if (Doc.AllShapes.Count > 0)
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
            saveFileDialog.Filter = @"Paint Files | *.pnt";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (Stream file = saveFileDialog.OpenFile())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(file, Doc);
                }
            }
        }

        private void menuSaveLike_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = @"Paint Files (*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (Bitmap bitmap = new Bitmap(PnlGraphic.ClientSize.Width,
                                  PnlGraphic.ClientSize.Height))
                {
                    PnlGraphic.DrawToBitmap(bitmap, PnlGraphic.ClientRectangle);
                    bitmap.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
                }
            }
        }

        private void NewBlankPage()
        {
            Form fileDialog = new NewFileForm();
            fileDialog.Text = @"New picture";
            fileDialog.ShowDialog();
            PnlGraphic.Size = new Size(PanelWidth, PanelHeight);
            ShapesList newDoc = new ShapesList();
            Doc = newDoc;
            _isShapeWasSelected = false;
            IndexOfSelectedShape = null;
            PnlGraphic.Invalidate();
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

        private void frmPaint_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && _isShapeWasSelected)
            {
                if (IndexOfSelectedShape != null)
                {
                    Doc.AllShapes.Remove(Doc.AllShapes[IndexOfSelectedShape.Value]);
                    _isShapeWasSelected = false;
                }
                IndexOfSelectedShape = null;
                _buttonDeletePressed = true;
                PnlGraphic.Invalidate();
            }
        }

        private void btnSelection_Click(object sender, EventArgs e)
        {
            if (_selectionMode)
            {
                _selectionMode = false;
                _isShapeWasSelected = false;
                btnSelection.Text = @"OFF";
            }
            else
            {
                _selectionMode = true;
                btnSelection.Text = @"ON";
            }
            PnlGraphic.Invalidate();
        }

        private void menuUndo_Click(object sender, EventArgs e)
        {
            if (Doc.AllShapes.Count < 1) return;
            var obj = Doc.AllShapes[Doc.AllShapes.Count - 1];
            Doc.AllShapes.Remove(obj);
            _redo.AllShapes.Add(obj);
            PnlGraphic.Invalidate();
        }

        private void menuRedo_Click(object sender, EventArgs e)
        {
            if (_redo.AllShapes.Count < 1) return;
            var obj = _redo.AllShapes[_redo.AllShapes.Count - 1];
            _redo.AllShapes.Remove(obj);
            Doc.AllShapes.Add(obj);
            PnlGraphic.Invalidate();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_isShapeWasSelected && IndexOfSelectedShape != null)
            {
                ChoseLineStyle();
                Doc.AllShapes[IndexOfSelectedShape.Value].ShapeSize = (int)numSize.Value;
                Doc.AllShapes[IndexOfSelectedShape.Value].PenStyle = PenStyle;
            }
            PnlGraphic.Invalidate();
        }

        private void menuCopy_Click(object sender, EventArgs e)
        {
            if (_isShapeWasSelected && IndexOfSelectedShape != null)
            {
                Shape copiedFigure = Doc.AllShapes[IndexOfSelectedShape.Value].Clone();
                Doc.AllShapes.Add(copiedFigure);
            }
            PnlGraphic.Invalidate();
        }

        private void menuCut_Click(object sender, EventArgs e)
        {
            if (_isShapeWasSelected && IndexOfSelectedShape != null)
            {
                var obj = Doc.AllShapes[IndexOfSelectedShape.Value];
                Doc.AllShapes.Remove(obj);
                _redo.AllShapes.Add(obj);
                _isShapeWasSelected = false;
            }
            PnlGraphic.Invalidate();
        }

        private void FrmPaint_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Doc.AllShapes.Count > 0)
            {
                const string message = "Do you want to save changes?";
                const string caption = "Save changes";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    menuSave_Click(sender, e);
                }
            }
        }
        
        private void menuClear_Click(object sender, EventArgs e)
        {
            ShapesList newDoc = new ShapesList();
            Doc = newDoc;
            _isShapeWasSelected = false;
            IndexOfSelectedShape = null;
            PnlGraphic.Invalidate();
        }
    }
}
