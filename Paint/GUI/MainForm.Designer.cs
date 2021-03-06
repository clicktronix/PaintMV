﻿namespace PaintMV.GUI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnDefaultColor = new System.Windows.Forms.Button();
            this.numSize = new System.Windows.Forms.NumericUpDown();
            this.grpColor = new System.Windows.Forms.GroupBox();
            this.menuOptions = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveLike = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCut = new System.Windows.Forms.ToolStripMenuItem();
            this.menuClear = new System.Windows.Forms.ToolStripMenuItem();
            this.saveDialog = new System.Windows.Forms.SaveFileDialog();
            this.openDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnPolygon = new System.Windows.Forms.Button();
            this.btnEllipse = new System.Windows.Forms.Button();
            this.btnLine = new System.Windows.Forms.Button();
            this.btnTriangle = new System.Windows.Forms.Button();
            this.btnRectangle = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnFillShape = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radioDot = new System.Windows.Forms.RadioButton();
            this.radioDash = new System.Windows.Forms.RadioButton();
            this.radioSolid = new System.Windows.Forms.RadioButton();
            this.btnRectSelection = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.btnMove = new System.Windows.Forms.Button();
            this.listUndo = new System.Windows.Forms.ListBox();
            this.listRedo = new System.Windows.Forms.ListBox();
            this.labelUndoHistory = new System.Windows.Forms.Label();
            this.labelRedoHistory = new System.Windows.Forms.Label();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            ((System.ComponentModel.ISupportInitialize)(this.numSize)).BeginInit();
            this.grpColor.SuspendLayout();
            this.menuOptions.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDefaultColor
            // 
            this.btnDefaultColor.BackColor = System.Drawing.Color.Black;
            this.btnDefaultColor.Location = new System.Drawing.Point(32, 19);
            this.btnDefaultColor.Name = "btnDefaultColor";
            this.btnDefaultColor.Size = new System.Drawing.Size(50, 30);
            this.btnDefaultColor.TabIndex = 9;
            this.btnDefaultColor.UseVisualStyleBackColor = false;
            this.btnDefaultColor.Click += new System.EventHandler(this.btnDefaultColor_Click);
            // 
            // numSize
            // 
            this.numSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.numSize.Location = new System.Drawing.Point(34, 19);
            this.numSize.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSize.Name = "numSize";
            this.numSize.ReadOnly = true;
            this.numSize.Size = new System.Drawing.Size(50, 23);
            this.numSize.TabIndex = 11;
            this.numSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSize.ValueChanged += new System.EventHandler(this.numSize_ValueChanged);
            // 
            // grpColor
            // 
            this.grpColor.Controls.Add(this.btnDefaultColor);
            this.grpColor.Location = new System.Drawing.Point(4, 474);
            this.grpColor.Name = "grpColor";
            this.grpColor.Size = new System.Drawing.Size(116, 55);
            this.grpColor.TabIndex = 13;
            this.grpColor.TabStop = false;
            this.grpColor.Text = "Set Color";
            // 
            // menuOptions
            // 
            this.menuOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.editToolStripMenuItem,
            this.menuClear});
            this.menuOptions.Location = new System.Drawing.Point(0, 0);
            this.menuOptions.Name = "menuOptions";
            this.menuOptions.Size = new System.Drawing.Size(1171, 24);
            this.menuOptions.TabIndex = 15;
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuNew,
            this.menuOpen,
            this.menuSave,
            this.menuSaveLike});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(37, 20);
            this.menuFile.Text = "File";
            // 
            // menuNew
            // 
            this.menuNew.Name = "menuNew";
            this.menuNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.menuNew.Size = new System.Drawing.Size(195, 22);
            this.menuNew.Text = "New";
            this.menuNew.Click += new System.EventHandler(this.menuNew_Click);
            // 
            // menuOpen
            // 
            this.menuOpen.Name = "menuOpen";
            this.menuOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuOpen.Size = new System.Drawing.Size(195, 22);
            this.menuOpen.Text = "Open";
            this.menuOpen.Click += new System.EventHandler(this.menuOpen_Click);
            // 
            // menuSave
            // 
            this.menuSave.Name = "menuSave";
            this.menuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuSave.Size = new System.Drawing.Size(195, 22);
            this.menuSave.Text = "Save";
            this.menuSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // menuSaveLike
            // 
            this.menuSaveLike.Name = "menuSaveLike";
            this.menuSaveLike.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.menuSaveLike.Size = new System.Drawing.Size(195, 22);
            this.menuSaveLike.Text = "Save As...";
            this.menuSaveLike.Click += new System.EventHandler(this.menuSaveLike_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuUndo,
            this.menuRedo,
            this.menuCopy,
            this.menuCut});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // menuUndo
            // 
            this.menuUndo.Name = "menuUndo";
            this.menuUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.menuUndo.Size = new System.Drawing.Size(174, 22);
            this.menuUndo.Text = "Undo";
            this.menuUndo.Click += new System.EventHandler(this.menuUndo_Click);
            // 
            // menuRedo
            // 
            this.menuRedo.Name = "menuRedo";
            this.menuRedo.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Z)));
            this.menuRedo.Size = new System.Drawing.Size(174, 22);
            this.menuRedo.Text = "Redo";
            this.menuRedo.Click += new System.EventHandler(this.menuRedo_Click);
            // 
            // menuCopy
            // 
            this.menuCopy.Name = "menuCopy";
            this.menuCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.menuCopy.Size = new System.Drawing.Size(174, 22);
            this.menuCopy.Text = "Copy";
            this.menuCopy.Click += new System.EventHandler(this.menuCopy_Click);
            // 
            // menuCut
            // 
            this.menuCut.Name = "menuCut";
            this.menuCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.menuCut.Size = new System.Drawing.Size(174, 22);
            this.menuCut.Text = "Cut";
            this.menuCut.Click += new System.EventHandler(this.menuCut_Click);
            // 
            // menuClear
            // 
            this.menuClear.Name = "menuClear";
            this.menuClear.Size = new System.Drawing.Size(46, 20);
            this.menuClear.Text = "Clear";
            this.menuClear.Click += new System.EventHandler(this.menuClear_Click);
            // 
            // openDialog
            // 
            this.openDialog.FileName = "openFileDialog1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnPolygon);
            this.groupBox1.Controls.Add(this.btnEllipse);
            this.groupBox1.Controls.Add(this.btnLine);
            this.groupBox1.Controls.Add(this.btnTriangle);
            this.groupBox1.Controls.Add(this.btnRectangle);
            this.groupBox1.Location = new System.Drawing.Point(4, 31);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(116, 143);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Shape";
            // 
            // btnPolygon
            // 
            this.btnPolygon.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnPolygon.FlatAppearance.BorderSize = 0;
            this.btnPolygon.Image = ((System.Drawing.Image)(resources.GetObject("btnPolygon.Image")));
            this.btnPolygon.Location = new System.Drawing.Point(16, 101);
            this.btnPolygon.Name = "btnPolygon";
            this.btnPolygon.Size = new System.Drawing.Size(35, 35);
            this.btnPolygon.TabIndex = 15;
            this.btnPolygon.UseVisualStyleBackColor = true;
            this.btnPolygon.Click += new System.EventHandler(this.btnPolygon_Click);
            // 
            // btnEllipse
            // 
            this.btnEllipse.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnEllipse.FlatAppearance.BorderSize = 0;
            this.btnEllipse.Image = ((System.Drawing.Image)(resources.GetObject("btnEllipse.Image")));
            this.btnEllipse.Location = new System.Drawing.Point(16, 19);
            this.btnEllipse.Name = "btnEllipse";
            this.btnEllipse.Size = new System.Drawing.Size(35, 35);
            this.btnEllipse.TabIndex = 1;
            this.btnEllipse.UseVisualStyleBackColor = true;
            this.btnEllipse.Click += new System.EventHandler(this.btnEllipse_Click);
            // 
            // btnLine
            // 
            this.btnLine.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnLine.FlatAppearance.BorderSize = 0;
            this.btnLine.Image = ((System.Drawing.Image)(resources.GetObject("btnLine.Image")));
            this.btnLine.Location = new System.Drawing.Point(16, 60);
            this.btnLine.Name = "btnLine";
            this.btnLine.Size = new System.Drawing.Size(35, 35);
            this.btnLine.TabIndex = 14;
            this.btnLine.UseVisualStyleBackColor = true;
            this.btnLine.Click += new System.EventHandler(this.btnLine_Click);
            // 
            // btnTriangle
            // 
            this.btnTriangle.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnTriangle.FlatAppearance.BorderSize = 0;
            this.btnTriangle.Image = ((System.Drawing.Image)(resources.GetObject("btnTriangle.Image")));
            this.btnTriangle.Location = new System.Drawing.Point(66, 60);
            this.btnTriangle.Name = "btnTriangle";
            this.btnTriangle.Size = new System.Drawing.Size(35, 35);
            this.btnTriangle.TabIndex = 11;
            this.btnTriangle.UseVisualStyleBackColor = true;
            this.btnTriangle.Click += new System.EventHandler(this.btnTriangle_Click);
            // 
            // btnRectangle
            // 
            this.btnRectangle.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnRectangle.FlatAppearance.BorderSize = 0;
            this.btnRectangle.Image = ((System.Drawing.Image)(resources.GetObject("btnRectangle.Image")));
            this.btnRectangle.Location = new System.Drawing.Point(66, 19);
            this.btnRectangle.Name = "btnRectangle";
            this.btnRectangle.Size = new System.Drawing.Size(35, 35);
            this.btnRectangle.TabIndex = 3;
            this.btnRectangle.UseVisualStyleBackColor = true;
            this.btnRectangle.Click += new System.EventHandler(this.btnRectangle_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numSize);
            this.groupBox3.Location = new System.Drawing.Point(4, 174);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(116, 48);
            this.groupBox3.TabIndex = 21;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Line Size";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnFillShape);
            this.groupBox4.Location = new System.Drawing.Point(4, 416);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(116, 52);
            this.groupBox4.TabIndex = 22;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Fill shape";
            // 
            // btnFillShape
            // 
            this.btnFillShape.BackColor = System.Drawing.Color.White;
            this.btnFillShape.Location = new System.Drawing.Point(32, 15);
            this.btnFillShape.Name = "btnFillShape";
            this.btnFillShape.Size = new System.Drawing.Size(50, 30);
            this.btnFillShape.TabIndex = 10;
            this.btnFillShape.UseVisualStyleBackColor = false;
            this.btnFillShape.Click += new System.EventHandler(this.btnFillShape_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radioDot);
            this.groupBox5.Controls.Add(this.radioDash);
            this.groupBox5.Controls.Add(this.radioSolid);
            this.groupBox5.Location = new System.Drawing.Point(4, 223);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(116, 81);
            this.groupBox5.TabIndex = 23;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Line Type";
            // 
            // radioDot
            // 
            this.radioDot.AutoSize = true;
            this.radioDot.Location = new System.Drawing.Point(34, 58);
            this.radioDot.Name = "radioDot";
            this.radioDot.Size = new System.Drawing.Size(42, 17);
            this.radioDot.TabIndex = 2;
            this.radioDot.Text = "Dot";
            this.radioDot.UseVisualStyleBackColor = true;
            this.radioDot.CheckedChanged += new System.EventHandler(this.radioDot_CheckedChanged);
            // 
            // radioDash
            // 
            this.radioDash.AutoSize = true;
            this.radioDash.Location = new System.Drawing.Point(34, 38);
            this.radioDash.Name = "radioDash";
            this.radioDash.Size = new System.Drawing.Size(50, 17);
            this.radioDash.TabIndex = 1;
            this.radioDash.Text = "Dash";
            this.radioDash.UseVisualStyleBackColor = true;
            this.radioDash.CheckedChanged += new System.EventHandler(this.radioDash_CheckedChanged);
            // 
            // radioSolid
            // 
            this.radioSolid.AutoSize = true;
            this.radioSolid.Checked = true;
            this.radioSolid.Location = new System.Drawing.Point(34, 19);
            this.radioSolid.Name = "radioSolid";
            this.radioSolid.Size = new System.Drawing.Size(48, 17);
            this.radioSolid.TabIndex = 0;
            this.radioSolid.TabStop = true;
            this.radioSolid.Text = "Solid";
            this.radioSolid.UseVisualStyleBackColor = true;
            // 
            // btnRectSelection
            // 
            this.btnRectSelection.Location = new System.Drawing.Point(8, 19);
            this.btnRectSelection.Name = "btnRectSelection";
            this.btnRectSelection.Size = new System.Drawing.Size(102, 28);
            this.btnRectSelection.TabIndex = 24;
            this.btnRectSelection.Text = "OFF";
            this.btnRectSelection.UseVisualStyleBackColor = true;
            this.btnRectSelection.Click += new System.EventHandler(this.btnRectSelection_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnRectSelection);
            this.groupBox6.Location = new System.Drawing.Point(4, 305);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(116, 54);
            this.groupBox6.TabIndex = 25;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Selection";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.btnMove);
            this.groupBox8.Location = new System.Drawing.Point(4, 361);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(116, 54);
            this.groupBox8.TabIndex = 27;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Move/Resize Mode";
            // 
            // btnMove
            // 
            this.btnMove.Location = new System.Drawing.Point(8, 19);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(102, 28);
            this.btnMove.TabIndex = 24;
            this.btnMove.Text = "OFF";
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.btnMoveResize_Click);
            // 
            // listUndo
            // 
            this.listUndo.FormatString = "N0";
            this.listUndo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.listUndo.Location = new System.Drawing.Point(4, 549);
            this.listUndo.Name = "listUndo";
            this.listUndo.Size = new System.Drawing.Size(116, 108);
            this.listUndo.TabIndex = 28;
            this.listUndo.SelectedIndexChanged += new System.EventHandler(this.listUndo_SelectedIndexChanged);
            // 
            // listRedo
            // 
            this.listRedo.FormatString = "N0";
            this.listRedo.FormattingEnabled = true;
            this.listRedo.Location = new System.Drawing.Point(4, 679);
            this.listRedo.Name = "listRedo";
            this.listRedo.Size = new System.Drawing.Size(116, 108);
            this.listRedo.TabIndex = 29;
            this.listRedo.SelectedIndexChanged += new System.EventHandler(this.listRedo_SelectedIndexChanged);
            // 
            // labelUndoHistory
            // 
            this.labelUndoHistory.AutoSize = true;
            this.labelUndoHistory.Location = new System.Drawing.Point(9, 532);
            this.labelUndoHistory.Name = "labelUndoHistory";
            this.labelUndoHistory.Size = new System.Drawing.Size(66, 13);
            this.labelUndoHistory.TabIndex = 30;
            this.labelUndoHistory.Text = "Undo history";
            // 
            // labelRedoHistory
            // 
            this.labelRedoHistory.AutoSize = true;
            this.labelRedoHistory.Location = new System.Drawing.Point(9, 662);
            this.labelRedoHistory.Name = "labelRedoHistory";
            this.labelRedoHistory.Size = new System.Drawing.Size(66, 13);
            this.labelRedoHistory.TabIndex = 31;
            this.labelRedoHistory.Text = "Redo history";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1171, 865);
            this.Controls.Add(this.labelRedoHistory);
            this.Controls.Add(this.labelUndoHistory);
            this.Controls.Add(this.listRedo);
            this.Controls.Add(this.listUndo);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpColor);
            this.Controls.Add(this.menuOptions);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuOptions;
            this.Name = "MainForm";
            this.Text = "Vector graphic editor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmPaint_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.numSize)).EndInit();
            this.grpColor.ResumeLayout(false);
            this.menuOptions.ResumeLayout(false);
            this.menuOptions.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnEllipse;
        private System.Windows.Forms.Button btnRectangle;
        private System.Windows.Forms.Button btnDefaultColor;
        public System.Windows.Forms.NumericUpDown numSize;
        private System.Windows.Forms.GroupBox grpColor;
        private System.Windows.Forms.Button btnTriangle;
        private System.Windows.Forms.Button btnLine;
        private System.Windows.Forms.MenuStrip menuOptions;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuNew;
        private System.Windows.Forms.ToolStripMenuItem menuOpen;
        private System.Windows.Forms.ToolStripMenuItem menuSave;
        private System.Windows.Forms.SaveFileDialog saveDialog;
        private System.Windows.Forms.OpenFileDialog openDialog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuUndo;
        private System.Windows.Forms.ToolStripMenuItem menuRedo;
        private System.Windows.Forms.ToolStripMenuItem menuCopy;
        private System.Windows.Forms.ToolStripMenuItem menuCut;
        private System.Windows.Forms.ToolStripMenuItem menuClear;
        private System.Windows.Forms.GroupBox groupBox5;
        public System.Windows.Forms.RadioButton radioDot;
        public System.Windows.Forms.RadioButton radioDash;
        public System.Windows.Forms.RadioButton radioSolid;
        private System.Windows.Forms.ToolStripMenuItem menuSaveLike;
        private System.Windows.Forms.Button btnRectSelection;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.Button btnPolygon;
        private System.Windows.Forms.Button btnFillShape;
        private System.Windows.Forms.Label labelUndoHistory;
        private System.Windows.Forms.Label labelRedoHistory;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.ListBox listRedo;
        private System.Windows.Forms.ListBox listUndo;
    }
}

