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
using ResizePosition = PaintMV.Controls.ResizePosition;

namespace PaintMV.GUI
{
    public partial class FrmPaint : Form
    {
        private bool _paintMode;
        private int _shapeWidth, _shapeHeight;
        private int _shapeSize = 1;
        private bool _hasShapes;
        private bool _fillShape;
        private Color _chosenColor = Color.Black;
        private bool _mIsClick;
        private bool _selectionMode;
        private Point _startPoint;
        private Point _endPoint;
        private readonly ShapesList _redo;
        private Shape _figure;
        private bool _loadedFile;
        private bool _buttonDeletePressed;
        private bool _rectSelectionMode;

        public ShapeSelectionByPoint ShapeSelectionByPoint { get; }
        public LineSelectionByPoint LineSelectionByPoint { get; }
        public ShapesEnum ShapesEnum { get; set; }
        public int SizeNodeRect { set; get; } = 7;
        public int? IndexOfSelectedShape { set; get; }
        public ShapesList Doc { set; get; }
        public ShapesList SelectedShapes { set; get; }
        public Panel PnlGraphic { get; }
        public ResizePosition ResizePosition { get; }
        public bool MMove { set; get; }
        public static int PanelWidth { get; set; } = 700;
        public static int PanelHeight { get; set; } = 500;
        public DashStyle PenStyle = DashStyle.Solid;
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

        public FrmPaint()
        {
            InitializeComponent();
            Doc = new ShapesList();
            SelectedShapes = new ShapesList();
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

        private void pnlGraphic_Paint(object sender, PaintEventArgs e)
        {
            UpdateUndoRedo();
            if (_hasShapes && Doc.AllShapes != null)
            {
                foreach (Shape shape in Doc.AllShapes)
                {
                    shape.Draw(e.Graphics);
                }
            }
            if (_loadedFile || _buttonDeletePressed || _selectionMode || _rectSelectionMode)
            {
                if (Doc.AllShapes != null)
                    foreach (Shape shape in Doc.AllShapes)
                    {
                        shape.Draw(e.Graphics);
                    }
            }
            if (Doc.AllShapes != null)
                for (int i = Doc.AllShapes.Count - 1; i >= 0; i--)
                {
                    if (Doc.AllShapes[i].GetShapeIsSelected())
                    {
                        ShapeSelectionByPoint.MakeSelectionOfShape(Doc.AllShapes[IndexOfSelectedShape.Value], e.Graphics);
                        //if (ShapesEnum == ShapesEnum.Line)
                        //{
                        //    if (Doc.AllShapes != null)
                        //        if (IndexOfSelectedShape != null)
                        //            LineSelectionByPoint.MakeSelectionOfLine(Doc.AllShapes[IndexOfSelectedShape.Value],
                        //                e.Graphics);
                        //}
                        //else
                        //{
                        //    if (Doc.AllShapes != null)
                        //        if (IndexOfSelectedShape != null)
                        //            ShapeSelectionByPoint.MakeSelectionOfShape(Doc.AllShapes[IndexOfSelectedShape.Value], e.Graphics);
                        //}
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
                        Doc.AllShapes[i].SetShapeIsSelected(true);
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
                if (IndexOfSelectedShape != null && (Doc.AllShapes[IndexOfSelectedShape.Value].GetShapeIsSelected() && IndexOfSelectedShape != null))
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
            if (IndexOfSelectedShape != null && (Doc.AllShapes[IndexOfSelectedShape.Value].GetShapeIsSelected() && IndexOfSelectedShape != null))
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
            if (_rectSelectionMode && Doc.AllShapes.Count != 0)
            {
                Doc.AllShapes.Remove(_figure);
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
                _fillShape = chBoxFill.Checked;
                var absShapeWidth = Math.Abs(_shapeWidth);
                var absShapeHeight = Math.Abs(_shapeHeight);
                CheckChosenShape(absShapeWidth, absShapeHeight);
            }
            else if (IndexOfSelectedShape != null && (_selectionMode && Doc.AllShapes[IndexOfSelectedShape.Value].GetShapeIsSelected()))
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
            else if (_rectSelectionMode)
            {
                for (int i = Doc.AllShapes.Count - 1; i >= 0; i--)
                {
                    if (Doc.AllShapes[i].ContainsSelectedFigure(_startPoint, _endPoint))
                    {
                        if (IndexOfSelectedShape != null)
                        {
                            Doc.AllShapes[i].SetShapeIsSelected(true);
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
                        }
                        IndexOfSelectedShape = Doc.AllShapes.Count - 1;
                        var obj = Doc.AllShapes[IndexOfSelectedShape.Value];
                        SelectedShapes.AllShapes.Add(obj);
                        MMove = true;
                    }
                }
            }
            if (IndexOfSelectedShape != null && (_hasShapes && Doc.AllShapes[IndexOfSelectedShape.Value].GetShapeIsSelected()))
            {
                TestIfRectInsideArea();
            }
            PnlGraphic.Invalidate();
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
                ShapesEnum == ShapesEnum.Triangle || ShapesEnum == ShapesEnum.SelectRectangle)
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
                    _figure = new Ellipse(tempStartPoint, absShapeWidth, absShapeHeight, _chosenColor, _shapeSize, _fillShape, PenStyle, false);
                }
                if (ShapesEnum == ShapesEnum.Rectangle)
                {
                    _figure = new Shapes.Rectangle(tempStartPoint, absShapeWidth, absShapeHeight, _chosenColor, _shapeSize, _fillShape, PenStyle, false);
                }
                if (ShapesEnum == ShapesEnum.Triangle)
                {
                    _figure = new Triangle(tempStartPoint, absShapeWidth, absShapeHeight, _chosenColor, _shapeSize, _fillShape, PenStyle, false);
                }
                if (ShapesEnum == ShapesEnum.SelectRectangle)
                {
                    _figure = new Shapes.Rectangle(tempStartPoint, absShapeWidth, absShapeHeight, Color.Blue, 1, false, DashStyle.Dash, false);
                }
            }
            else if (ShapesEnum == ShapesEnum.Line)
            {
                tempStartPoint = _startPoint;
                Point tempEndPoint = _endPoint;
                _figure = new Line(tempStartPoint, tempEndPoint, absShapeWidth, absShapeHeight, _chosenColor, _shapeSize, PenStyle, false);
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

        private void UnselectAllFigures()
        {
            for (int i = Doc.AllShapes.Count - 1; i >= 0; i--)
            {
                Doc.AllShapes[i].SetShapeIsSelected(false);
            }
        }

        private void btnRectangle_Click(object sender, EventArgs e)
        {
            string button = "rectangle";
            ShapesEnum = ShapesEnum.Rectangle;
            ChangeButtonColor(button);
            _selectionMode = true;
            _rectSelectionMode = false;
            btnSelection_Click(sender, e);
        }

        private void btnEllipse_Click(object sender, EventArgs e)
        {
            ShapesEnum = ShapesEnum.Ellipse;
            string button = "ellipse";
            ChangeButtonColor(button);
            _selectionMode = true;
            _rectSelectionMode = false;
            btnSelection_Click(sender, e);
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            ShapesEnum = ShapesEnum.Line;
            string button = "line";
            ChangeButtonColor(button);
            _selectionMode = true;
            _rectSelectionMode = false;
            btnSelection_Click(sender, e);
        }

        private void btnTriangle_Click(object sender, EventArgs e)
        {
            ShapesEnum = ShapesEnum.Triangle;
            string button = "triangle";
            ChangeButtonColor(button);
            _selectionMode = true;
            _rectSelectionMode = false;
            btnSelection_Click(sender, e);
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
            UnselectAllFigures();
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
            if (IndexOfSelectedShape != null && (e.KeyCode == Keys.Delete && Doc.AllShapes[IndexOfSelectedShape.Value].GetShapeIsSelected()))
            {
                if (IndexOfSelectedShape != null)
                {
                    Doc.AllShapes.Remove(Doc.AllShapes[IndexOfSelectedShape.Value]);
                    UnselectAllFigures();
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
                btnSelection.Text = @"OFF";
                UnselectAllFigures();
            }
            else
            {
                _selectionMode = true;
                btnSelection.Text = @"ON";
            }
            PnlGraphic.Invalidate();
        }

        private void UpdateUndoRedo()
        {
            menuUndo.Enabled = (Doc.AllShapes.Count > 0);
            menuRedo.Enabled = (_redo.AllShapes.Count > 0);
        }

        private void menuUndo_Click(object sender, EventArgs e)
        {
            UnselectAllFigures();
            if (Doc.AllShapes.Count < 1) return;
            var obj = Doc.AllShapes[Doc.AllShapes.Count - 1];
            Doc.AllShapes.Remove(obj);
            _redo.AllShapes.Add(obj);
            PnlGraphic.Invalidate();
        }

        private void menuRedo_Click(object sender, EventArgs e)
        {
            UnselectAllFigures();
            if (_redo.AllShapes.Count < 1) return;
            var obj = _redo.AllShapes[_redo.AllShapes.Count - 1];
            _redo.AllShapes.Remove(obj);
            Doc.AllShapes.Add(obj);
            PnlGraphic.Invalidate();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (IndexOfSelectedShape != null && (Doc.AllShapes[IndexOfSelectedShape.Value].GetShapeIsSelected() && IndexOfSelectedShape != null))
            {
                ChoseLineStyle();
                Doc.AllShapes[IndexOfSelectedShape.Value].ShapeSize = (int)numSize.Value;
                Doc.AllShapes[IndexOfSelectedShape.Value].PenStyle = PenStyle;
            }
            PnlGraphic.Invalidate();
        }

        private void menuCopy_Click(object sender, EventArgs e)
        {
            if (IndexOfSelectedShape != null && (Doc.AllShapes[IndexOfSelectedShape.Value].GetShapeIsSelected() && IndexOfSelectedShape != null))
            {
                Shape copiedFigure = Doc.AllShapes[IndexOfSelectedShape.Value].Clone();
                Doc.AllShapes.Add(copiedFigure);
            }
            PnlGraphic.Invalidate();
        }

        private void menuCut_Click(object sender, EventArgs e)
        {
            if (IndexOfSelectedShape != null && Doc.AllShapes[IndexOfSelectedShape.Value].GetShapeIsSelected())
            {
                var obj = Doc.AllShapes[IndexOfSelectedShape.Value];
                Doc.AllShapes.Remove(obj);
                _redo.AllShapes.Add(obj);
                UnselectAllFigures();
            }
            IndexOfSelectedShape = null;
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

        private void btnDefaultColor_Click(object sender, EventArgs e)
        {
            DialogResult d = colorDialog1.ShowDialog();
            if (d == DialogResult.OK)
            {
                _chosenColor = colorDialog1.Color;
                btnDefaultColor.BackColor = colorDialog1.Color;
                ChangeColor(colorDialog1.Color);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_rectSelectionMode)
            {
                string button = "rectangle";
                ShapesEnum = ShapesEnum.Rectangle;
                ChangeButtonColor(button);
                _rectSelectionMode = false;
                _selectionMode = false;
                UnselectAllFigures();
                IndexOfSelectedShape = null;
                button1.Text = @"Off";
            }
            else
            {
                UnselectAllFigures();
                IndexOfSelectedShape = null;
                ShapesEnum = ShapesEnum.SelectRectangle;
                _rectSelectionMode = true;
                _selectionMode = false;
                button1.Text = @"On";
            }
            PnlGraphic.Invalidate();
        }
        
        private void menuClear_Click(object sender, EventArgs e)
        {
            ShapesList newDoc = new ShapesList();
            Doc = newDoc;
            UnselectAllFigures();
            IndexOfSelectedShape = null;
            PnlGraphic.Invalidate();
        }

        private void TestIfRectInsideArea()
        {
            if (IndexOfSelectedShape != null)
            {
                Shape tempShape = Doc.AllShapes[IndexOfSelectedShape.Value];
                Point staticPoint;
                staticPoint = new Point();
                staticPoint.X = PnlGraphic.Width - tempShape.StartOrigin.X;
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
                if (tempShape.StartOrigin.X + tempShape.Width > PnlGraphic.Width)
                {
                    tempShape.StartOrigin = new Point(PnlGraphic.Width - tempShape.Width, tempShape.StartOrigin.Y);
                    tempShape.Width = PnlGraphic.Width - tempShape.StartOrigin.X;
                }
                if (tempShape.StartOrigin.Y + tempShape.Height > PnlGraphic.Height)
                {
                    tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, PnlGraphic.Height - tempShape.Height);
                    tempShape.Height = PnlGraphic.Height - tempShape.StartOrigin.Y;
                }
            }
            PnlGraphic.Invalidate();
        }
    }
}
