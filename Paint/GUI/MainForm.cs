using System;
using System.Collections.Generic;
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
        private bool _loadedFile;
        private bool _rectSelectionMode;
        private bool _moveSelectionMode;
        private int _moveCount;
        private readonly List<ICommand> _commands = new List<ICommand>();
        private readonly List<ICommand> _redoCommands = new List<ICommand>();
        private readonly List<string> _commandsHistory = new List<string>();
        private readonly List<string> _commandsRedoHistory = new List<string>();

        public int ShapeWidth;
        public int ShapeHeight;
        public int ShapeSize = 1;
        public int PolyLinesCount;
        public bool CompletedPolygon;
        public Shape Figure;
        public List<Shape> CopiedShapes = new List<Shape>();
        public ShapeSelection ShapeSelection { get; }
        public ShapesEnum ShapesEnum { get; set; }
        public int? IndexOfSelectedShape { set; get; }
        public ShapesList Doc { set; get; }
        public Panel PnlGraphic { get; }
        public MoveResize MoveResize { get; }
        public bool MMove { set; get; }
        public static int PanelWidth { get; set; } = 700;
        public static int PanelHeight { get; set; } = 500;
        public DashStyle PenStyle = DashStyle.Solid;
        public Color ChosenColor { set; get; } = Color.Black;
        public Color FillColor { set; get; } = Color.Empty;
        public LineStyleChose LineStyleChose { get; }
        public DrawShape DrawShape { get; }
        public Copy Copy { get; }
        public Cut Cut { get; }
        public ShapesList Redo { get; }
        public UpdateShape UpdateShape { get; }
        public UnselectShapes UnselectShapes { get; }
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
            Redo = new ShapesList();
            PnlGraphic = new GraphicPanel();
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
            ShapeSelection = new ShapeSelection(this); //передавать параметры сюда
            MoveResize = new MoveResize(this);
            LineStyleChose = new LineStyleChose(this);
            DrawShape = new DrawShape(this);
            Copy = new Copy(this);
            Cut = new Cut(this);
            UpdateShape = new UpdateShape(this);
            UnselectShapes = new UnselectShapes(this);
        }

        /// <summary>
        /// Main method to draw on the panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlGraphic_Paint(object sender, PaintEventArgs e)
        {
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
                    if ((!_selectionMode || !Doc.AllShapes[i].GetShapeIsSelected()) && 
                        (!Doc.AllShapes[i].GetShapeIsSelected() || !_rectSelectionMode)) continue;
                    if (Doc.AllShapes[i].IsLine)
                    {
                        ShapeSelection.LineSelection = true;
                        ShapeSelection.PolygonSelection = false;
                    }
                    else if (Doc.AllShapes[i].IsPolygon)
                    {
                        ShapeSelection.LineSelection = false;
                        ShapeSelection.PolygonSelection = true;
                    }
                    else
                    {
                        ShapeSelection.LineSelection = false;
                        ShapeSelection.PolygonSelection = false;
                    }
                    if (Doc.AllShapes == null) continue;
                    if (IndexOfSelectedShape != null) ShapeSelection.MakeSelectionOfShape(Doc.AllShapes[i], e.Graphics);
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
            if (_rectSelectionMode && Doc.AllShapes.Count == 0)
            {
                MessageBox.Show(@"No figures for selection!");
            }
            else if (_selectionMode && Doc.AllShapes.Count != 0)
            {
                for (int i = Doc.AllShapes.Count - 1; i >= 0; i--)
                {
                    if (!Doc.AllShapes[i].ContainsPoint(e.Location)) continue;
                    ProccessOfSelectionFigure(i);
                }
                if (IndexOfSelectedShape != null && (_moveSelectionMode && Doc.AllShapes[IndexOfSelectedShape.Value].GetShapeIsSelected()))
                {
                    ShapeSelection.NodeSelected = Positions.None;
                    ShapeSelection.NodeSelected = ShapeSelection.SupportPoints.GetNodeSelectable(e.Location);
                }
            }
            else if (!_selectionMode)
            {
                _paintMode = true;
                if (ShapesEnum == ShapesEnum.Polygon)
                {
                    MMove = false;
                    DrawShape.Execute(null, e, null);
                } else if (CompletedPolygon)
                {
                    DrawShape.CompletePolygon();
                }
            }
            _startPoint.X = e.X;
            _startPoint.Y = e.Y;
            PnlGraphic.Invalidate();
        }

        /// <summary>
        /// Method is called when you unclick on form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlGraphic_MouseUp(object sender, MouseEventArgs e)
        {
            _paintMode = false;
            _moveCount = 0;
            if (!_selectionMode && !_rectSelectionMode && !_moveSelectionMode && (e.X - _startPoint.X != 0 
                || e.X - _startPoint.X == 0 && ShapesEnum == ShapesEnum.Polygon || !CompletedPolygon))
            {
                CopyListOfFigures();
                DrawShape.ExecuteUndo();
                _commands.Add(DrawShape);
                Doc.AllShapes.Add(Figure);
                if (ShapesEnum == ShapesEnum.SelectRectangle)
                {
                    Doc.AllShapes.Remove(Doc.AllShapes[Doc.AllShapes.Count - 1]);
                    _commands.Remove(_commands[_commands.Count - 1]);
                }
                _hasShapes = true;
            }
            if (_rectSelectionMode && Doc.AllShapes.Count != 0)
            {
                bool selectionFlag = false;
                for (int i = Doc.AllShapes.Count - 1; i >= 0; i--)
                {
                    if (!Doc.AllShapes[i].ContainsPoint(e.Location)) continue;
                    selectionFlag = true;
                    ProccessOfSelectionFigure(i);
                }
                if (!selectionFlag && !_moveSelectionMode)
                {
                    UnselectShapes.UnselectAllShapes();
                }
            }
            _mIsClick = false;
            MMove = false;
            Figure = null;
            UndoListAddElements();
            PnlGraphic.Invalidate();
        }

        /// <summary>
        /// Method is called when you move your mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlGraphic_MouseMove(object sender, MouseEventArgs e)
        {
            MMove = true;
            if (_paintMode)
            {
                _endPoint.X = e.X;
                _endPoint.Y = e.Y;
                ShapeWidth = e.X - _startPoint.X;
                ShapeHeight = e.Y - _startPoint.Y;
                ShapeSize = (int)numSize.Value;
                var absShapeWidth = Math.Abs(ShapeWidth);
                var absShapeHeight = Math.Abs(ShapeHeight);
                DrawShape.SetShapeParameters(absShapeWidth, absShapeHeight);
                DrawShape.Execute(null, e, null);
                DrawShape.IncrementCurrentLists();
            }
            else if (_moveSelectionMode)
            {
                if (IndexOfSelectedShape != null && !Doc.AllShapes[IndexOfSelectedShape.Value].IsPolygon
                    && Doc.AllShapes[IndexOfSelectedShape.Value].GetShapeIsSelected())
                {
                    PnlGraphic.Cursor = ShapeSelection.SupportPoints.GetCursor(ShapeSelection.SupportPoints.GetNodeSelectable(e.Location));
                } else
                {
                    PnlGraphic.Cursor = ShapeSelection.SupportPoints.GetCursor(ShapeSelection.SupportPoints.GetCursorOfPolygonPoint(e.Location));
                }
                if (_mIsClick == false) { return; }
                if (IndexOfSelectedShape != null && (Doc.AllShapes[IndexOfSelectedShape.Value].GetShapeIsSelected()))
                {
                    for (int i = Doc.AllShapes.Count - 1; i >= 0; i--)
                    {
                        if (!Doc.AllShapes[i].GetShapeIsSelected()) continue;
                        if (IndexOfSelectedShape == null) continue;
                        MoveResize.Execute(null, e, Doc.AllShapes[i]);
                        MoveResize.IncrementCurrentLists();
                        MoveResize.ExecuteRedo();
                        if (_moveCount == 0)
                        {
                            _commands.Add(MoveResize);
                            CopyListOfFigures();
                            MoveResize.ExecuteUndo();
                        }
                        _commands.Remove(_commands[_commands.Count - 1]);
                        _commands.Add(MoveResize);
                        _moveCount++;
                    }
                }
                _startPoint.X = e.X;
                _startPoint.Y = e.Y;
            }
            else if (_rectSelectionMode && _startPoint.X != 0 && _startPoint.Y != 0 && _endPoint.X != 0 && _endPoint.Y != 0 )
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
                    UnselectShapes.UnselectAllShapes();
                    RefreshPoints();
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
        /// Copy list of current figures
        /// </summary>
        public void CopyListOfFigures()
        {
            CopiedShapes.Clear();
            for (int i = Doc.AllShapes.Count - 1; i >= 0; i--)
            {
                var a = Doc.AllShapes[i].Clone();
                CopiedShapes.Add(a);
            }
        }

        /// <summary>
        /// Update selected shapes
        /// </summary>
        public void UpdateSelectedShapes()
        {
            if (IndexOfSelectedShape != null && Doc.AllShapes[IndexOfSelectedShape.Value].GetShapeIsSelected())
            {
                UpdateShape.Execute(null,null,null);
                _commands.Add(UpdateShape);
                PnlGraphic.Invalidate();
                UndoListAddElements();
            }
            PnlGraphic.Invalidate();
        }

        /// <summary>
        /// Delete supported polylines
        /// </summary>
        public void DeletePolyLines()
        {
            if (PolyLinesCount == 0) return;
            do
            {
                PolyLinesCount--;
                if (_commands.Count > 0)
                {
                    Doc.AllShapes.Remove(Doc.AllShapes[Doc.AllShapes.Count - 1]);
                    _commands.Remove(_commands[_commands.Count - 1]);
                }
            } while (PolyLinesCount > 0);
        }

        #endregion

#region Control methods
        /// <summary>
        /// Selecting an rectangle shape method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRectangle_Click(object sender, EventArgs e)
        {
            string button = "rectangle";
            ShapesEnum = ShapesEnum.Rectangle;
            ChangeButtonColor(button);
            _rectSelectionMode = true;
            btnRectSelection_Click(sender, e);
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
            _rectSelectionMode = true;
            btnRectSelection_Click(sender, e);
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
            _rectSelectionMode = true;
            btnRectSelection_Click(sender, e);
        }

        /// <summary>
        /// Selecting an triangle shape method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTriangle_Click(object sender, EventArgs e)
        {
            ShapesEnum = ShapesEnum.Triangle;
            string button = "triangle";
            ChangeButtonColor(button);
            _rectSelectionMode = true;
            btnRectSelection_Click(sender, e);
        }

        /// <summary>
        /// Selecting an polygon shape method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPolygon_Click(object sender, EventArgs e)
        {
            ShapesEnum = ShapesEnum.Polygon;
            string button = "polygon";
            ChangeButtonColor(button);
            _rectSelectionMode = true;
            btnRectSelection_Click(sender, e);
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
                } else if (result == DialogResult.No)
                {
                    OpenDialogForm();
                }
            } else
            {
                OpenDialogForm();
            }
        }

        /// <summary>
        /// Dialog form
        /// </summary>
        private void OpenDialogForm()
        {
            var openFileDialog = new OpenFileDialog {Filter = @"Paint Files | *.pnt"};
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            var newDoc = new ShapesList();
            Doc = newDoc;
            using (Stream file = openFileDialog.OpenFile())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                Doc = (ShapesList) formatter.Deserialize(file);
            }
            _loadedFile = true;
            PnlGraphic.Invalidate();
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
                switch (result)
                {
                    case DialogResult.Yes:
                        menuSave_Click(sender, e);
                        NewBlankPage();
                        break;
                    case DialogResult.No:
                        NewBlankPage();
                        break;
                }
            } else
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
            UnselectShapes.UnselectAllShapes();
            SaveFileDialog saveFileDialog = new SaveFileDialog {Filter = @"Paint Files | *.pnt"};
            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
            using (Stream file = saveFileDialog.OpenFile())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(file, Doc);
            }
        }

        /// <summary>
        /// Method saves the file in the popular extensions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSaveLike_Click(object sender, EventArgs e)
        {
            UnselectShapes.UnselectAllShapes();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = @"Paint Files (*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
            using (Bitmap bitmap = new Bitmap(PnlGraphic.ClientSize.Width,
                PnlGraphic.ClientSize.Height))
            {
                PnlGraphic.DrawToBitmap(bitmap, PnlGraphic.ClientRectangle);
                bitmap.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
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
            UnselectShapes.UnselectAllShapes();
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
            switch (button)
            {
                case "rectangle":
                    btnRectangle.BackColor = Color.White;
                    btnEllipse.BackColor = Color.Transparent;
                    btnLine.BackColor = Color.Transparent;
                    btnTriangle.BackColor = Color.Transparent;
                    btnPolygon.BackColor = Color.Transparent;
                    break;
                case "line":
                    btnEllipse.BackColor = Color.Transparent;
                    btnRectangle.BackColor = Color.Transparent;
                    btnLine.BackColor = Color.White;
                    btnTriangle.BackColor = Color.Transparent;
                    btnPolygon.BackColor = Color.Transparent;
                    break;
                case "triangle":
                    btnEllipse.BackColor = Color.Transparent;
                    btnRectangle.BackColor = Color.Transparent;
                    btnLine.BackColor = Color.Transparent;
                    btnTriangle.BackColor = Color.White;
                    btnPolygon.BackColor = Color.Transparent;
                    break;
                case "ellipse":
                    btnEllipse.BackColor = Color.White;
                    btnRectangle.BackColor = Color.Transparent;
                    btnLine.BackColor = Color.Transparent;
                    btnTriangle.BackColor = Color.Transparent;
                    btnPolygon.BackColor = Color.Transparent;
                    break;
                case "polygon":
                    btnPolygon.BackColor = Color.White;
                    btnEllipse.BackColor = Color.Transparent;
                    btnRectangle.BackColor = Color.Transparent;
                    btnLine.BackColor = Color.Transparent;
                    btnTriangle.BackColor = Color.Transparent;
                    break;
            }
        }

        /// <summary>
        /// Area selection button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRectSelection_Click(object sender, EventArgs e)
        {
            RefreshPoints();
            UnselectShapes.UnselectAllShapes();
            _selectionMode = false;
            btnMove.Text = @"OFF";
            _moveSelectionMode = false;
            if (_rectSelectionMode)
            {
                _rectSelectionMode = false;
                btnRectSelection.Text = @"OFF";
                groupBox6.Text = @"Selection";
            } else
            {
                ShapesEnum = ShapesEnum.SelectRectangle;
                _rectSelectionMode = true;
                btnRectSelection.Text = @"ON";
                groupBox6.Text = @"Selection ON";
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
            if (IndexOfSelectedShape != null && IndexOfSelectedShape.Value > -1 && Doc.AllShapes.Count > -1)
            {
                RefreshPoints();
                groupBox6.Text = @"Selection";
                _rectSelectionMode = false;
                if (_moveSelectionMode)
                {
                    btnRectangle_Click(sender, e);
                    btnMove.Text = @"OFF";
                }
                else
                {
                    btnRectSelection.Text = @"OFF";
                    _moveSelectionMode = true;
                    _selectionMode = true;
                    btnMove.Text = @"ON";
                }
                PnlGraphic.Invalidate();
            }
        }

        /// <summary>
        /// Undo button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuUndo_Click(object sender, EventArgs e)
        {
            if (_commands.Count > 0)
            {
                var item = _commands[_commands.Count - 1];
                _redoCommands.Add(item);
                item.Undo();
                _commands.Remove(_commands[_commands.Count - 1]);
            }
            btnEllipse_Click(sender, e);
            UndoListAddElements();
            RedoListAddElements();
            PnlGraphic.Invalidate();
        }

        /// <summary>
        /// Redo button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuRedo_Click(object sender, EventArgs e)
        {
            if (_redoCommands.Count > 0)
            {
                var item = _redoCommands[_redoCommands.Count - 1];
                _commands.Add(item);
                item.Redo();
                _redoCommands.Remove(_redoCommands[_redoCommands.Count - 1]);
            }
            btnEllipse_Click(sender, e);
            RedoListAddElements();
            UndoListAddElements();
            PnlGraphic.Invalidate();
        }

        /// <summary>
        /// Copy button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuCopy_Click(object sender, EventArgs e)
        {
            Copy.Execute(null,null,null);
            _commands.Add(Copy);
            btnMoveResize_Click(sender, e);
            UndoListAddElements();
            PnlGraphic.Invalidate();
        }

        /// <summary>
        /// Cut button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuCut_Click(object sender, EventArgs e)
        {
            Cut.Execute(null, null, null);
            _commands.Add(Cut);
            btnEllipse_Click(sender, e);
            UnselectShapes.UnselectAllShapes();
            UndoListAddElements();
            PnlGraphic.Invalidate();
        }

        /// <summary>
        /// Method is called when the program is closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmPaint_FormClosing(object sender, FormClosingEventArgs e)
        {
            _rectSelectionMode = false;
            UnselectShapes.UnselectAllShapes();
            if (Doc.AllShapes.Count <= 0) return;
            const string message = "Do you want to save changes?";
            const string caption = "Save changes";
            var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                menuSave_Click(sender, e);
            }
        }

        /// <summary>
        /// Set color button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDefaultColor_Click(object sender, EventArgs e)
        {
            DialogResult d = colorDialog.ShowDialog();
            if (d != DialogResult.OK) return;
            ChosenColor = colorDialog.Color;
            btnDefaultColor.BackColor = ChosenColor;
            UpdateSelectedShapes();
        }

        /// <summary>
        /// Set fill button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFillShape_Click(object sender, EventArgs e)
        {
            DialogResult d = colorDialog.ShowDialog();
            if (d != DialogResult.OK) return;
            FillColor = colorDialog.Color;
            btnFillShape.BackColor = FillColor;
            UpdateSelectedShapes();
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
            UnselectShapes.UnselectAllShapes();
            IndexOfSelectedShape = null;
            btnEllipse_Click(sender, e);
            PnlGraphic.Invalidate();
            _commands.Clear();
            _redoCommands.Clear();
        }

        /// <summary>
        /// Undo commands history
        /// </summary>
        private void UndoListAddElements()
        {
            listUndo.Items.Clear();
            _commandsHistory.Clear();
            for (var i = _commands.Count - 1; i >= 0; i--)
            {
                listUndo.Items.Add(@i + " " + _commands[i].Operation());
                _commandsHistory.Add(_commands[i].Operation());
            }
        }

        /// <summary>
        /// Redo commands history
        /// </summary>
        private void RedoListAddElements()
        {
            listRedo.Items.Clear();
            _commandsRedoHistory.Clear();
            for (var i = _redoCommands.Count - 1; i >= 0; i--)
            {
                listRedo.Items.Add(@i + " " + _redoCommands[i].Operation());
                _commandsRedoHistory.Add(_redoCommands[i].Operation());
            }
        }

        /// <summary>
        /// Get the index of the selected command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listUndo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listUndo.SelectedIndex > -1)
            {
                var commandIndex = listUndo.SelectedIndex;
                do
                {
                    menuUndo_Click(sender, e);
                    commandIndex--;
                } while (commandIndex > -1);
            }
        }

        /// <summary>
        /// Get the index of the selected command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listRedo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listRedo.SelectedIndex > -1)
            {
                var commandIndex = listRedo.SelectedIndex;
                do
                {
                    menuRedo_Click(sender, e);
                    commandIndex--;
                } while (commandIndex > -1);
            }
        }
        
        /// <summary>
        /// Update figures when radio button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioSolid_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSelectedShapes();
        }

        /// <summary>
        /// Update figures when radio button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioDash_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSelectedShapes();
        }

        /// <summary>
        /// Update figures when radio button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioDot_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSelectedShapes();
        }

        /// <summary>
        /// Update figures when numeric button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numSize_ValueChanged(object sender, EventArgs e)
        {
            UpdateSelectedShapes();
        }
        #endregion
    }
}
