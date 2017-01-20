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
        private Point _startPointOfSelect;
        private readonly ShapesList _redo;
        private Shape _figure;
        private bool _loadedFile;
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
            PnlGraphic.Location = new Point(130, 32);
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
            if (_loadedFile || _selectionMode || _rectSelectionMode)
            {
                if (Doc.AllShapes != null)
                    foreach (Shape shape in Doc.AllShapes)
                    {
                        shape.Draw(e.Graphics);
                    }
            }
            if (Doc.AllShapes != null)
            {
                for (int i = Doc.AllShapes.Count - 1; i >= 0; i--)
                {
                    if ((_selectionMode && Doc.AllShapes[i].GetShapeIsSelected()) || (Doc.AllShapes[i].GetShapeIsSelected() && _rectSelectionMode))
                    {
                        if (ShapesEnum == ShapesEnum.Line)
                        {
                            if (Doc.AllShapes == null) continue;
                            if (IndexOfSelectedShape != null) LineSelectionByPoint.MakeSelectionOfLine(Doc.AllShapes[i], e.Graphics);
                        }
                        else
                        {
                            if (Doc.AllShapes == null) continue;
                            if (IndexOfSelectedShape != null) ShapeSelectionByPoint.MakeSelectionOfShape(Doc.AllShapes[i], e.Graphics);
                        }
                    }
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
                        if (!_rectSelectionMode)
                        {
                            Doc.AllShapes[IndexOfSelectedShape.Value - 1].SetShapeIsSelected(false);
                        }
                        MMove = true;
                        break;
                    }
                }
                if (IndexOfSelectedShape != null && (Doc.AllShapes[IndexOfSelectedShape.Value].GetShapeIsSelected() && IndexOfSelectedShape != null))
                {
                    ShapeSelectionByPoint.NodeSelected = Enumerations.ResizePosition.None;
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
                _startPointOfSelect = _endPoint;
                var absShapeWidth = Math.Abs(_shapeWidth);
                var absShapeHeight = Math.Abs(_shapeHeight);
                CheckChosenShape(absShapeWidth, absShapeHeight);
            }
            else if (IndexOfSelectedShape != null && _selectionMode && Doc.AllShapes[IndexOfSelectedShape.Value].GetShapeIsSelected())
            {
                ChangeCursor(e.Location);
                if (_mIsClick == false) { return; }
                if (_rectSelectionMode)
                {
                    for (int i = Doc.AllShapes.Count - 1; i >= 0; i--)
                    {
                        if (!Doc.AllShapes[i].GetShapeIsSelected()) continue;
                        if (IndexOfSelectedShape == null) continue;
                        Shape tempShape = Doc.AllShapes[i];
                        ResizePosition.GetValueOfResizedPosition(e, tempShape);
                    }
                }
                else if (Doc.AllShapes[IndexOfSelectedShape.Value].GetShapeIsSelected())
                {
                    if (IndexOfSelectedShape != null)
                    {
                        Shape tempShape = Doc.AllShapes[IndexOfSelectedShape.Value];
                        ResizePosition.GetValueOfResizedPosition(e, tempShape);
                    }
                }
                _startPoint.X = e.X;
                _startPoint.Y = e.Y;
            }
            else if (_rectSelectionMode)
            {
                for (int i = Doc.AllShapes.Count - 1; i >= 0; i--)
                {
                    if (Doc.AllShapes[i].ContainsSelectedFigure(_startPointOfSelect, _endPoint))
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
                        MMove = true;
                    }
                }
            }
            for (int i = Doc.AllShapes.Count - 1; i >= 0; i--)
            {
                if (IndexOfSelectedShape != null && Doc.AllShapes[i].GetShapeIsSelected() && IndexOfSelectedShape != null)
                {
                    TestIfRectInsideArea();
                }
            }
            PnlGraphic.Invalidate();
        }

        private void ChangeColor()
        {
            for (int i = Doc.AllShapes.Count - 1; i >= 0; i--)
            {
                if (IndexOfSelectedShape != null && Doc.AllShapes[i].GetShapeIsSelected() && IndexOfSelectedShape != null)
                {
                    Doc.AllShapes[i].ChosenColor = _chosenColor;
                    Doc.AllShapes[i].FilledShape = chBoxFill.Checked;
                }
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
            if (ShapesEnum == ShapesEnum.Ellipse || ShapesEnum == ShapesEnum.Rectangle || ShapesEnum == ShapesEnum.Triangle || ShapesEnum == ShapesEnum.SelectRectangle)
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
                    else { tempStartPoint = new Point(_startPoint.X, _startPoint.Y); }
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
                    _startPointOfSelect = tempStartPoint;
                }
            }
            else if (ShapesEnum == ShapesEnum.Line)
            {
                tempStartPoint = _startPoint;
                Point tempEndPoint = _endPoint;
                _figure = new Line(tempStartPoint, tempEndPoint, absShapeWidth, absShapeHeight, _chosenColor, _shapeSize, PenStyle, false);
            }
            else if (ShapesEnum == ShapesEnum.None) { }
            else { throw new ArgumentOutOfRangeException(); }
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
            _selectionMode = false;
            _rectSelectionMode = false;
            UnselectAllFigures();
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
            _selectionMode = false;
            _rectSelectionMode = false;
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
            _selectionMode = true;
            _rectSelectionMode = false;
            btnSelection_Click(sender, e);
            UnselectAllFigures();
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
            _selectionMode = true;
            _rectSelectionMode = false;
            btnSelection_Click(sender, e);
            UnselectAllFigures();
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
            _selectionMode = false;
            _rectSelectionMode = false;
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

        private void btnSelection_Click(object sender, EventArgs e)
        {
            button1.Text = @"Select OFF";
            groupBox6.Text = @"Rect Selection";
            if (_selectionMode)
            {
                _selectionMode = false;
                _rectSelectionMode = false;
                btnSelection.Text = @"Move/Resize OFF";
                groupBox2.Text = @"Point Selection";
                UnselectAllFigures();
            }
            else
            {
                _selectionMode = true;
                btnSelection.Text = @"Move/Resize ON";
                groupBox2.Text = @"Point Selection ON";
            }
            PnlGraphic.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UnselectAllFigures();
            btnSelection.Text = @"Move/Resize OFF";
            groupBox2.Text = @"Point Selection";
            if (_rectSelectionMode)
            {
                btnRectangle_Click(sender, e);
                button1.Text = @"Select OFF";
                groupBox6.Text = @"Rect Selection";
                
            }
            else
            {
                ShapesEnum = ShapesEnum.SelectRectangle;
                _rectSelectionMode = true;
                _selectionMode = false;
                button1.Text = @"Select ON";
                groupBox6.Text = @"Rect Selection ON";
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
            if (Doc.AllShapes.Count < 1) return;
            var obj = Doc.AllShapes[Doc.AllShapes.Count - 1];
            obj.SetShapeIsSelected(false);
            Doc.AllShapes.Remove(obj);
            _redo.AllShapes.Add(obj);
            btnEllipse_Click(sender, e);
            UnselectAllFigures();
            PnlGraphic.Invalidate();
        }

        private void menuRedo_Click(object sender, EventArgs e)
        {
            if (_redo.AllShapes.Count < 1) return;
            var obj = _redo.AllShapes[_redo.AllShapes.Count - 1];
            obj.SetShapeIsSelected(false);
            _redo.AllShapes.Remove(obj);
            Doc.AllShapes.Add(obj);
            btnEllipse_Click(sender, e);
            UnselectAllFigures();
            PnlGraphic.Invalidate();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            for (int i = Doc.AllShapes.Count - 1; i >= 0; i--)
            {
                if (IndexOfSelectedShape != null && Doc.AllShapes[i].GetShapeIsSelected() && IndexOfSelectedShape != null)
                {
                    ChoseLineStyle();
                    Doc.AllShapes[i].ChosenColor = colorDialog1.Color;
                    Doc.AllShapes[i].ShapeSize = (int) numSize.Value;
                    Doc.AllShapes[i].PenStyle = PenStyle;
                }
            }
            PnlGraphic.Invalidate();
        }

        private void menuCopy_Click(object sender, EventArgs e)
        {
            for (int i = Doc.AllShapes.Count - 1; i >= 0; i--)
            {
                if (IndexOfSelectedShape != null && Doc.AllShapes[i].GetShapeIsSelected() && IndexOfSelectedShape != null)
                {
                    Doc.AllShapes[i].SetShapeIsSelected(false);
                    Shape copiedFigure = Doc.AllShapes[i].Clone();
                    copiedFigure.SetShapeIsSelected(false);
                    Doc.AllShapes.Add(copiedFigure);
                }
            }
            _selectionMode = false;
            _rectSelectionMode = false;
            _startPointOfSelect = _endPoint;
            btnSelection_Click(sender, e);
            PnlGraphic.Invalidate();
        }

        private void menuCut_Click(object sender, EventArgs e)
        {
            for (int i = Doc.AllShapes.Count - 1; i >= 0; i--)
            {
                if (IndexOfSelectedShape != null && Doc.AllShapes[i].GetShapeIsSelected() && IndexOfSelectedShape != null)
                {
                    var obj = Doc.AllShapes[Doc.AllShapes.Count - 1];
                    obj.SetShapeIsSelected(false);
                    Doc.AllShapes[i].SetShapeIsSelected(false);
                    Doc.AllShapes.Remove(Doc.AllShapes[i]);
                    _redo.AllShapes.Add(obj);
                }
            }
            _selectionMode = true;
            _rectSelectionMode = false;
            _startPointOfSelect = _endPoint;
            btnSelection_Click(sender, e);
            UnselectAllFigures();
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
                btnDefaultColor.BackColor = _chosenColor;
                ChangeColor();
            }
        }
        
        private void menuClear_Click(object sender, EventArgs e)
        {
            ShapesList newDoc = new ShapesList();
            Doc = newDoc;
            UnselectAllFigures();
            IndexOfSelectedShape = null;
            btnEllipse_Click(sender, e);
            PnlGraphic.Invalidate();
        }

        private void TestIfRectInsideArea()
        {
            for (int i = Doc.AllShapes.Count - 1; i >= 0; i--)
            {
                if (IndexOfSelectedShape != null && Doc.AllShapes[i].GetShapeIsSelected() && IndexOfSelectedShape != null)
                {
                    Shape tempShape = Doc.AllShapes[i];
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
            }
            PnlGraphic.Invalidate();
        }
    }
}
