﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaintMV.GUI
{
    public partial class NewFileForm : Form
    {
        private bool inputerror = false;
        private bool notdigit = false;
        private bool emptyfield = false;

        public NewFileForm()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (usersSize.Checked)
            {
                if (notdigit || emptyfield || inputerror) MessageBox.Show(@"Введены неверные данные!");
                else
                {
                    FrmPaint.PanelWidth = Convert.ToInt32(numericUpDown1.Text);
                    FrmPaint.PanelHeight = Convert.ToInt32(numericUpDown2.Text);
                    Close();
                }
            }
            else
            {
                if (defaultSize1.Checked)
                {
                    FrmPaint.PanelWidth = 320;
                    FrmPaint.PanelHeight = 240;
                }
                else if (defaultSize2.Checked)
                {
                    FrmPaint.PanelWidth = 640;
                    FrmPaint.PanelHeight = 480;
                }
                else if (defaultSize3.Checked)
                {
                    FrmPaint.PanelWidth = 800;
                    FrmPaint.PanelHeight = 600;
                }
                else if (defaultSize4.Checked)
                {
                    FrmPaint.PanelWidth = 1024;
                    FrmPaint.PanelHeight = 768;
                }
                Close();
            }
            //MainForm.CreateNewFile = true;
        }
    }
}
