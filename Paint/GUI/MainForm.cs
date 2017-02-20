using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using PaintMV.Controls;
using PaintMV.Enumerations;
using PaintMV.Shapes;

namespace PaintMV.GUI
{
    /// <summary>
    /// Main form application class
    /// </summary>
    public partial class MainForm : Form
    {

#region Properties 

        private readonly List<ICommand> _redoCommands = new List<ICommand>();
        private readonly List<string> _commandsHistory = new List<string>();
        private readonly List<string> _commandsRedoHistory = new List<string>();

        public bool SelectionMode { set; get; }
        public bool LoadedFile { set; get; }
        public bool RectSelectionMode { set; get; }
        public bool MoveSelectionMode { set; get; }
        public Panel GraphicPanel { get; }
        public static int PanelWidth { get; set; } = 700;
        public static int PanelHeight { get; set; } = 500;
        public Copy Copy { get; }
        public Cut Cut { get; }
        public UpdateShape UpdateShape { get; }
        public UnselectShapes UnselectShapes { get; }
        public DrawHandlers DrawHandlers { get; }

#endregion
        
        /// <summary>
        /// Application constructor. Initialize form parameters
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            GraphicPanel = new GraphicPanel();
            Controls.Add(GraphicPanel);
            btnEllipse.BackColor = Color.White;
            GraphicPanel.BackColor = Color.White;
            GraphicPanel.Location = new Point(130, 32);
            GraphicPanel.Name = "pnlGraphic";
            GraphicPanel.Size = new Size(PanelWidth, PanelHeight);
            DrawHandlers = new DrawHandlers(this);
            GraphicPanel.Paint += DrawHandlers.pnlGraphic_Paint;
            GraphicPanel.MouseDown += DrawHandlers.pnlGraphic_MouseDown;
            GraphicPanel.MouseMove += DrawHandlers.pnlGraphic_MouseMove;
            GraphicPanel.MouseUp += DrawHandlers.pnlGraphic_MouseUp;
            Copy = new Copy(DrawHandlers);
            Cut = new Cut(DrawHandlers);
            UpdateShape = new UpdateShape(DrawHandlers, numSize);
            UnselectShapes = new UnselectShapes(DrawHandlers);
        }

#region Control methods
        
        /// <summary>
        /// Selecting an rectangle shape method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRectangle_Click(object sender, EventArgs e)
        {
            string button = "rectangle";
            DrawHandlers.ShapesEnum = ShapesEnum.Rectangle;
            ChangeButtonColor(button);
            RectSelectionMode = true;
            btnRectSelection_Click(sender, e);
        }

        /// <summary>
        /// Selecting an ellipse shape method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEllipse_Click(object sender, EventArgs e)
        {
            DrawHandlers.ShapesEnum = ShapesEnum.Ellipse;
            string button = "ellipse";
            ChangeButtonColor(button);
            RectSelectionMode = true;
            btnRectSelection_Click(sender, e);
        }

        /// <summary>
        /// Selecting an line shape method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLine_Click(object sender, EventArgs e)
        {
            DrawHandlers.ShapesEnum = ShapesEnum.Line;
            string button = "line";
            ChangeButtonColor(button);
            RectSelectionMode = true;
            btnRectSelection_Click(sender, e);
        }

        /// <summary>
        /// Selecting an triangle shape method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTriangle_Click(object sender, EventArgs e)
        {
            DrawHandlers.ShapesEnum = ShapesEnum.Triangle;
            string button = "triangle";
            ChangeButtonColor(button);
            RectSelectionMode = true;
            btnRectSelection_Click(sender, e);
        }

        /// <summary>
        /// Selecting an polygon shape method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPolygon_Click(object sender, EventArgs e)
        {
            DrawHandlers.ShapesEnum = ShapesEnum.Polygon;
            string button = "polygon";
            ChangeButtonColor(button);
            RectSelectionMode = true;
            btnRectSelection_Click(sender, e);
        }

        /// <summary>
        /// Method of opening a new file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuOpen_Click(object sender, EventArgs e)
        {
            if (DrawHandlers.ShapesList.Count > 0)
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
            var newDoc = new List<IShape>();
            DrawHandlers.ShapesList = newDoc;
            using (Stream file = openFileDialog.OpenFile())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                DrawHandlers.ShapesList = (List<IShape>) formatter.Deserialize(file);
            }
            LoadedFile = true;
            GraphicPanel.Invalidate();
        }

        /// <summary>
        /// Create a new file method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuNew_Click(object sender, EventArgs e)
        {
            if (DrawHandlers.ShapesList.Count > 0)
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
                formatter.Serialize(file, DrawHandlers.ShapesList);
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
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = @"Paint Files (*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
            using (Bitmap bitmap = new Bitmap(GraphicPanel.ClientSize.Width,
                GraphicPanel.ClientSize.Height))
            {
                GraphicPanel.DrawToBitmap(bitmap, GraphicPanel.ClientRectangle);
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
            GraphicPanel.Size = new Size(PanelWidth, PanelHeight);
            List<IShape> newDoc = new List<IShape>();
            DrawHandlers.ShapesList = newDoc;
            UnselectShapes.UnselectAllShapes();
            DrawHandlers.IndexOfSelectedShape = null;
            SelectionMode = false;
            RectSelectionMode = false;
            GraphicPanel.Invalidate();
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
            DrawHandlers.RefreshPoints();
            UnselectShapes.UnselectAllShapes();
            SelectionMode = false;
            btnMove.Text = @"OFF";
            MoveSelectionMode = false;
            if (RectSelectionMode)
            {
                RectSelectionMode = false;
                btnRectSelection.Text = @"OFF";
                groupBox6.Text = @"Selection";
            } else
            {
                DrawHandlers.ShapesEnum = ShapesEnum.SelectRectangle;
                RectSelectionMode = true;
                btnRectSelection.Text = @"ON";
                groupBox6.Text = @"Selection ON";
            }
            GraphicPanel.Invalidate();
        }

        /// <summary>
        /// Move/resize selected figure button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMoveResize_Click(object sender, EventArgs e)
        {
            if (DrawHandlers.IndexOfSelectedShape != null && DrawHandlers.IndexOfSelectedShape.Value > -1 && DrawHandlers.ShapesList.Count > -1)
            {
                DrawHandlers.RefreshPoints();
                groupBox6.Text = @"Selection";
                RectSelectionMode = false;
                if (MoveSelectionMode)
                {
                    btnRectangle_Click(sender, e);
                    btnMove.Text = @"OFF";
                }
                else
                {
                    btnRectSelection.Text = @"OFF";
                    MoveSelectionMode = true;
                    SelectionMode = true;
                    btnMove.Text = @"ON";
                }
                GraphicPanel.Invalidate();
            }
        }

        /// <summary>
        /// Undo button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuUndo_Click(object sender, EventArgs e)
        {
            if (DrawHandlers.Commands.Count > 0)
            {
                var item = DrawHandlers.Commands[DrawHandlers.Commands.Count - 1];
                _redoCommands.Add(item);
                item.Undo();
                DrawHandlers.Commands.Remove(DrawHandlers.Commands[DrawHandlers.Commands.Count - 1]);
            }
            btnEllipse_Click(sender, e);
            UndoListAddElements();
            RedoListAddElements();
            GraphicPanel.Invalidate();
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
                DrawHandlers.Commands.Add(item);
                item.Redo();
                _redoCommands.Remove(_redoCommands[_redoCommands.Count - 1]);
            }
            btnEllipse_Click(sender, e);
            RedoListAddElements();
            UndoListAddElements();
            GraphicPanel.Invalidate();
        }

        /// <summary>
        /// Copy button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuCopy_Click(object sender, EventArgs e)
        {
            Copy.Execute(null,null,null);
            DrawHandlers.Commands.Add(Copy);
            btnMoveResize_Click(sender, e);
            UndoListAddElements();
            GraphicPanel.Invalidate();
        }

        /// <summary>
        /// Cut button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuCut_Click(object sender, EventArgs e)
        {
            Cut.Execute(null, null, null);
            DrawHandlers.Commands.Add(Cut);
            btnEllipse_Click(sender, e);
            UnselectShapes.UnselectAllShapes();
            UndoListAddElements();
            GraphicPanel.Invalidate();
        }

        /// <summary>
        /// Method is called when the program is closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmPaint_FormClosing(object sender, FormClosingEventArgs e)
        {
            RectSelectionMode = false;
            UnselectShapes.UnselectAllShapes();
            if (DrawHandlers.ShapesList.Count <= 0) return;
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
            DrawHandlers.ChosenColor = colorDialog.Color;
            btnDefaultColor.BackColor = DrawHandlers.ChosenColor;
            DrawHandlers.UpdateSelectedShapes();
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
            DrawHandlers.FillColor = colorDialog.Color;
            btnFillShape.BackColor = DrawHandlers.FillColor;
            DrawHandlers.UpdateSelectedShapes();
        }

        /// <summary>
        /// Clear button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuClear_Click(object sender, EventArgs e)
        {
            List<IShape> newDoc = new List<IShape>();
            DrawHandlers.ShapesList = newDoc;
            UnselectShapes.UnselectAllShapes();
            DrawHandlers.IndexOfSelectedShape = null;
            btnEllipse_Click(sender, e);
            GraphicPanel.Invalidate();
            DrawHandlers.Commands.Clear();
            _redoCommands.Clear();
        }

        /// <summary>
        /// Undo commands history
        /// </summary>
        public void UndoListAddElements()
        {
            listUndo.Items.Clear();
            _commandsHistory.Clear();
            for (var i = DrawHandlers.Commands.Count - 1; i >= 0; i--)
            {
                listUndo.Items.Add(@i + " " + DrawHandlers.Commands[i].Operation());
                _commandsHistory.Add(DrawHandlers.Commands[i].Operation());
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
        private void radioDash_CheckedChanged(object sender, EventArgs e)
        {
            DrawHandlers.UpdateSelectedShapes();
        }

        /// <summary>
        /// Update figures when radio button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioDot_CheckedChanged(object sender, EventArgs e)
        {
            DrawHandlers.UpdateSelectedShapes();
        }

        /// <summary>
        /// Update figures when numeric button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numSize_ValueChanged(object sender, EventArgs e)
        {
            DrawHandlers.UpdateSelectedShapes();
        }
        #endregion
    }
}
