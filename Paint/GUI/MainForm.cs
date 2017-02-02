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
    public partial class MainForm : Form
    {
#region Properties 
        private bool _paintMode;
        private bool _hasShapes;
        private bool _mIsClick;
        private bool _selectionMode;
        private Point _startPoint;
        private Point _endPoint;
        private readonly ShapesList _redo;
        private bool _loadedFile;
        private bool _rectSelectionMode;
        private bool _moveSelectionMode;

        public bool FillShape;
        public int ShapeWidth;
        public int ShapeHeight;
        public int ShapeSize = 1;
        public Shape Figure;
        public ShapeSelection ShapeSelection { get; }
        public LineSelection LineSelection { get; }
        public ShapesEnum ShapesEnum { get; set; }
        public int SizeNodeRect { set; get; } = 10;
        public int? IndexOfSelectedShape { set; get; }
        public ShapesList Doc { set; get; }
        public ShapesList SelectedShapes { set; get; }
        public Panel PnlGraphic { get; }
        public MoveResize MoveResize { get; }
        public bool MMove { set; get; }
        public static int PanelWidth { get; set; } = 700;
        public static int PanelHeight { get; set; } = 500;
        public DashStyle PenStyle = DashStyle.Solid;
        public ColorChange ColorChange { get; }
        public Color ChosenColor { set; get; } = Color.Black;
        public LineStyleChose LineStyleChose { get; }
        public ChoseShape ChoseShape { get; }
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
        #endregion

#region Methods for drawing
        /// <summary>
        /// Form constructor. Initialize form parameters
        /// </summary>
        public MainForm()
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
            ShapeSelection = new ShapeSelection(this);
            LineSelection = new LineSelection(this);
            MoveResize = new MoveResize(this);
            ColorChange = new ColorChange(this);
            LineStyleChose = new LineStyleChose(this);
            ChoseShape = new ChoseShape(this);
        }

        /// <summary>
        /// Main method to draw on the panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        if (Doc.AllShapes[i].IsLine)
                        {
                            if (Doc.AllShapes == null) continue;
                            if (IndexOfSelectedShape != null) LineSelection.MakeSelectionOfLine(Doc.AllShapes[i], e.Graphics);
                        }
                        else
                        {
                            if (Doc.AllShapes == null) continue;
                            if (IndexOfSelectedShape != null) ShapeSelection.MakeSelectionOfShape(Doc.AllShapes[i], e.Graphics);
                        }
                    }
                }
            }
            if (_paintMode)
            {
                Figure?.Draw(e.Graphics);
            }
        }

        /// <summary>
        /// Method is called when you click on form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlGraphic_MouseDown(object sender, MouseEventArgs e)
        {
            _mIsClick = true;
            if (_selectionMode && Doc.AllShapes.Count == 0)
            {
                MessageBox.Show(@"No figures for selection!");
            }
            else if (_selectionMode && Doc.AllShapes.Count != 0)
            {
                bool selectionFlag = false;
                for (int i = Doc.AllShapes.Count - 1; i >= 0; i--)
                {
                    if (!Doc.AllShapes[i].ContainsPoint(e.Location)) continue;
                    selectionFlag = true;
                    ProccessOfSelectionFigure(i);
                }
                if (_moveSelectionMode && IndexOfSelectedShape != null && Doc.AllShapes[IndexOfSelectedShape.Value].GetShapeIsSelected())
                {
                    ShapeSelection.NodeSelected = Positions.None;
                    ShapeSelection.NodeSelected = ShapeSelection.SupportPoints.GetNodeSelectable(e.Location);
                }
                if (ShapeSelection.NodeSelected != Positions.None)
                {
                    selectionFlag = true;
                }
                if (!selectionFlag && !_moveSelectionMode)
                {
                    UnselectAllFigures();
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

        /// <summary>
        /// method is called when you unclick on form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlGraphic_MouseUp(object sender, MouseEventArgs e)
        {
            _paintMode = false;
            if (!_selectionMode && (e.X - _startPoint.X) != 0)
            {
                Doc.AllShapes.Add(Figure);
                _hasShapes = true;
            }
            if (_rectSelectionMode && Doc.AllShapes.Count != 0)
            {
                Doc.AllShapes.Remove(Figure);
            }
            _mIsClick = false;
            MMove = false;
            Figure = null;
            PnlGraphic.Invalidate();
        }

        /// <summary>
        /// Method is called when you move your mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlGraphic_MouseMove(object sender, MouseEventArgs e)
        {
            if (_paintMode)
            {
                _endPoint.X = e.X;
                _endPoint.Y = e.Y;
                ShapeWidth = e.X - _startPoint.X;
                ShapeHeight = e.Y - _startPoint.Y;
                ShapeSize = (int)numSize.Value;
                FillShape = chBoxFill.Checked;
                var absShapeWidth = Math.Abs(ShapeWidth);
                var absShapeHeight = Math.Abs(ShapeHeight);
                ChoseShape.CheckChosenShape(absShapeWidth, absShapeHeight);
            }
            else if (_moveSelectionMode && IndexOfSelectedShape != null && Doc.AllShapes[IndexOfSelectedShape.Value].GetShapeIsSelected())
            {
                ChangeCursor(e.Location);
                if (_mIsClick == false) { return; }
                if (Doc.AllShapes[IndexOfSelectedShape.Value].GetShapeIsSelected())
                {
                    for (int i = Doc.AllShapes.Count - 1; i >= 0; i--)
                    {
                        if (!Doc.AllShapes[i].GetShapeIsSelected()) continue;
                        if (IndexOfSelectedShape == null) continue;
                        Shape tempShape = Doc.AllShapes[i];
                        MoveResize.GetValueOfResizedPosition(e, tempShape);
                    }
                }
                _startPoint.X = e.X;
                _startPoint.Y = e.Y;
            }
            else if (_rectSelectionMode)
            {
                bool selectionFlag = false;
                for (var i = Doc.AllShapes.Count - 1; i >= 0; i--)
                {
                    if (!Doc.AllShapes[i].ContainsSelectedFigure(_startPoint, _endPoint)) continue;
                    selectionFlag = true;
                    ProccessOfSelectionFigure(i);
                }
                if (!selectionFlag)
                {
                    UnselectAllFigures();
                }
            }
            PnlGraphic.Invalidate();
        }

        /// <summary>
        /// The method that describes the shape selection process
        /// </summary>
        /// <param name="i"></param>
        private void ProccessOfSelectionFigure(int i)
        {
            if (!_moveSelectionMode)
            {
                Doc.AllShapes[i].SetShapeIsSelected(true);
            }
            Shape tempShape = Doc.AllShapes[i];
            if (Doc.AllShapes.Count == 2 && tempShape != Doc.AllShapes[Doc.AllShapes.Count - 1])
            {
                Doc.AllShapes[i] = Doc.AllShapes[i + 1];
                Doc.AllShapes[i + 1] = tempShape;
            }
            else if (Doc.AllShapes.Count > 2 && tempShape != Doc.AllShapes[Doc.AllShapes.Count - 1])
            {
                for (var j = i; j < Doc.AllShapes.Count - 1; j++)
                {
                    Doc.AllShapes[j] = Doc.AllShapes[j + 1];
                }
                Doc.AllShapes[Doc.AllShapes.Count - 1] = tempShape;
            }
            IndexOfSelectedShape = Doc.AllShapes.Count - 1;
            MMove = true;
        }

        /// <summary>
        /// Refresh coordinates
        /// </summary>
        private void RefreshPoints()
        {
            _startPoint.X = 0;
            _startPoint.Y = 0;
            _endPoint.X = 0;
            _endPoint.Y = 0;
        }

        /// <summary>
        /// Transmitting the coordinates of the cursor method
        /// </summary>
        /// <param name="p"></param>
        private void ChangeCursor(Point p)
        {
            PnlGraphic.Cursor = ShapeSelection.SupportPoints.GetCursor(ShapeSelection.SupportPoints.GetNodeSelectable(p));
        }

        /// <summary>
        /// Set flag IsSelected false of all shapes in list
        /// </summary>
        private void UnselectAllFigures()
        {
            for (int i = Doc.AllShapes.Count - 1; i >= 0; i--)
            {
                Doc.AllShapes[i].SetShapeIsSelected(false);
            }
        }
        #endregion

#region Control methods
        /// <summary>
        /// Selecting an ellipse rectangle method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRectangle_Click(object sender, EventArgs e)
        {
            string button = "rectangle";
            ShapesEnum = ShapesEnum.Rectangle;
            ChangeButtonColor(button);
            _selectionMode = true;
            _rectSelectionMode = false;
            btnSelection_Click(sender, e);
        }

        /// <summary>
        /// Selecting an ellipse shape method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEllipse_Click(object sender, EventArgs e)
        {
            ShapesEnum = ShapesEnum.Ellipse;
            string button = "ellipse";
            ChangeButtonColor(button);
            _selectionMode = true;
            _rectSelectionMode = false;
            btnSelection_Click(sender, e);
        }

        /// <summary>
        /// Selecting an line shape method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLine_Click(object sender, EventArgs e)
        {
            ShapesEnum = ShapesEnum.Line;
            string button = "line";
            ChangeButtonColor(button);
            _selectionMode = true;
            _rectSelectionMode = false;
            btnSelection_Click(sender, e);
        }

        /// <summary>
        /// Selecting an ellipse shape method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTriangle_Click(object sender, EventArgs e)
        {
            ShapesEnum = ShapesEnum.Triangle;
            string button = "triangle";
            ChangeButtonColor(button);
            _selectionMode = true;
            _rectSelectionMode = false;
            btnSelection_Click(sender, e);
        }

        /// <summary>
        /// Method of opening a new file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Dialog form
        /// </summary>
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

        /// <summary>
        /// Create a new file method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Method saves the file in the .pnt extension
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSave_Click(object sender, EventArgs e)
        {
            UnselectAllFigures();
            SaveFileDialog saveFileDialog = new SaveFileDialog {Filter = @"Paint Files | *.pnt"};
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (Stream file = saveFileDialog.OpenFile())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(file, Doc);
                }
            }
        }

        /// <summary>
        /// Method saves the file in the popular extensions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSaveLike_Click(object sender, EventArgs e)
        {
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

        /// <summary>
        /// New file dialog
        /// </summary>
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

        /// <summary>
        /// Method button changes the color of the selected shape
        /// </summary>
        /// <param name="button"></param>
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

        /// <summary>
        /// Point selection button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelection_Click(object sender, EventArgs e)
        {
            RefreshPoints();
            UnselectAllFigures();
            btnRectSelection.Text = @"OFF";
            groupBox6.Text = @"Rect Selection";
            _rectSelectionMode = false;
            
            btnMove.Text = @"OFF";
            _moveSelectionMode = false;
            if (_selectionMode)
            {
                _selectionMode = false;
                btnSelection.Text = @"OFF";
                groupBox2.Text = @"Point Selection";
            }
            else
            {
                _selectionMode = true;
                btnSelection.Text = @"ON";
                groupBox2.Text = @"Point Selection ON";
            }
            PnlGraphic.Invalidate();
        }

        /// <summary>
        /// Area selection button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRectSelection_Click(object sender, EventArgs e)
        {
            RefreshPoints();
            UnselectAllFigures();
            btnSelection.Text = @"OFF";
            groupBox2.Text = @"Point Selection";
            _selectionMode = false;
            btnMove.Text = @"OFF";
            _moveSelectionMode = false;
            if (_rectSelectionMode)
            {
                btnRectangle_Click(sender, e);
                btnRectSelection.Text = @"OFF";
                groupBox6.Text = @"Rect Selection";
            }
            else
            {
                ShapesEnum = ShapesEnum.SelectRectangle;
                _rectSelectionMode = true;
                btnRectSelection.Text = @"ON";
                groupBox6.Text = @"Rect Selection ON";
            }
            PnlGraphic.Invalidate();
        }

        /// <summary>
        /// Move/resize selected figure button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMoveResize_Click(object sender, EventArgs e)
        {
            RefreshPoints();
            groupBox2.Text = @"Point Selection";
            groupBox6.Text = @"Rect Selection";
            _rectSelectionMode = false;
            if (_moveSelectionMode)
            {
                btnRectangle_Click(sender, e);
                btnMove.Text = @"OFF";
            }
            else
            {
                btnSelection.Text = @"OFF";
                btnRectSelection.Text = @"OFF";
                _moveSelectionMode = true;
                _selectionMode = true;
                btnMove.Text = @"ON";
            }
            PnlGraphic.Invalidate();
        }

        /// <summary>
        /// Enable/disable undo redo buttons
        /// </summary>
        private void UpdateUndoRedo()
        {
            menuUndo.Enabled = (Doc.AllShapes.Count > 0);
            menuRedo.Enabled = (_redo.AllShapes.Count > 0);
        }

        /// <summary>
        /// Undo button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Redo button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Update selected shapes method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            for (int i = Doc.AllShapes.Count - 1; i >= 0; i--)
            {
                if (IndexOfSelectedShape != null && Doc.AllShapes[i].GetShapeIsSelected() && IndexOfSelectedShape != null)
                {
                    LineStyleChose.ChoseLineStyle();
                    Doc.AllShapes[i].ChosenColor = colorDialog1.Color;
                    Doc.AllShapes[i].ShapeSize = (int) numSize.Value;
                    Doc.AllShapes[i].PenStyle = PenStyle;
                }
            }
            PnlGraphic.Invalidate();
        }

        /// <summary>
        /// Copy button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            btnSelection_Click(sender, e);
            PnlGraphic.Invalidate();
        }

        /// <summary>
        /// Cut button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            btnSelection_Click(sender, e);
            UnselectAllFigures();
            PnlGraphic.Invalidate();
        }

        /// <summary>
        /// Method is called when the program is closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmPaint_FormClosing(object sender, FormClosingEventArgs e)
        {
            _selectionMode = false;
            _rectSelectionMode = false;
            UnselectAllFigures();
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

        /// <summary>
        /// Set color button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDefaultColor_Click(object sender, EventArgs e)
        {
            DialogResult d = colorDialog1.ShowDialog();
            if (d == DialogResult.OK)
            {
                ChosenColor = colorDialog1.Color;
                btnDefaultColor.BackColor = ChosenColor;
                ColorChange.ChangeColor();
            }
        }
        
        /// <summary>
        /// Clear button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuClear_Click(object sender, EventArgs e)
        {
            ShapesList newDoc = new ShapesList();
            Doc = newDoc;
            UnselectAllFigures();
            IndexOfSelectedShape = null;
            btnEllipse_Click(sender, e);
            PnlGraphic.Invalidate();
        }
        #endregion
    }
}
