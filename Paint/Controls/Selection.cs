using System;
using System.Drawing;
using System.Windows.Forms;

namespace PaintMV
{
    class RectSelection
    {
        private PictureBox _mPictureBox;
        public Rectangle Rect;
        public bool AllowDeformingDuringMovement = false;
        private bool _mIsClick = false;
        private bool _mMove = false;
        private int _oldX;
        private int _oldY;
        private int _sizeNodeRect = 5;
        private Bitmap _mBmp = null;
        private PosSizableRect _nodeSelected = PosSizableRect.None;
        
        private enum PosSizableRect
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

        public RectSelection(Rectangle r)
        {
            Rect = r;
            _mIsClick = false;
        }

        public void Draw(Graphics g)
        {
            g.DrawRectangle(new Pen(Color.Red), Rect);

            foreach (PosSizableRect pos in Enum.GetValues(typeof(PosSizableRect)))
            {
                g.DrawRectangle(new Pen(Color.Blue), GetRect(pos));
            }
        }

        public void Draw(Graphics g, Rectangle rectangle)
        {
            g.DrawRectangle(new Pen(Color.Red), rectangle);

            foreach (PosSizableRect pos in Enum.GetValues(typeof(PosSizableRect)))
            {
                g.DrawRectangle(new Pen(Color.Blue), GetRect(pos));
            }
        }

        public void SetBitmapFile(string filename)
        {
            this._mBmp = new Bitmap(filename);
        }

        public void SetBitmap(Bitmap bmp)
        {
            this._mBmp = bmp;
        }

        public void SetPictureBox(PictureBox p)
        {
            this._mPictureBox = p;
            _mPictureBox.MouseDown += new MouseEventHandler(mPictureBox_MouseDown);
            _mPictureBox.MouseUp += new MouseEventHandler(mPictureBox_MouseUp);
            _mPictureBox.MouseMove += new MouseEventHandler(mPictureBox_MouseMove);
            _mPictureBox.Paint += new PaintEventHandler(mPictureBox_Paint);
        }

        private void mPictureBox_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Draw(e.Graphics);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }

        private void mPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            _mIsClick = true;

            _nodeSelected = PosSizableRect.None;
            _nodeSelected = GetNodeSelectable(e.Location);

            if (Rect.Contains(new Point(e.X, e.Y)))
            {
                _mMove = true;
            }
            _oldX = e.X;
            _oldY = e.Y;
        }

        private void mPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            _mIsClick = false;
            _mMove = false;
        }

        private void mPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            ChangeCursor(e.Location);
            if (_mIsClick == false)
            {
                return;
            }
            
            Rectangle backupRect = Rect;

            switch (_nodeSelected)
            {
                case PosSizableRect.LeftUp:
                    Rect.X += e.X - _oldX;
                    Rect.Width -= e.X - _oldX;
                    Rect.Y += e.Y - _oldY;
                    Rect.Height -= e.Y - _oldY;
                    break;
                case PosSizableRect.LeftMiddle:
                    Rect.X += e.X - _oldX;
                    Rect.Width -= e.X - _oldX;
                    break;
                case PosSizableRect.LeftBottom:
                    Rect.Width -= e.X - _oldX;
                    Rect.X += e.X - _oldX;
                    Rect.Height += e.Y - _oldY;
                    break;
                case PosSizableRect.BottomMiddle:
                    Rect.Height += e.Y - _oldY;
                    break;
                case PosSizableRect.RightUp:
                    Rect.Width += e.X - _oldX;
                    Rect.Y += e.Y - _oldY;
                    Rect.Height -= e.Y - _oldY;
                    break;
                case PosSizableRect.RightBottom:
                    Rect.Width += e.X - _oldX;
                    Rect.Height += e.Y - _oldY;
                    break;
                case PosSizableRect.RightMiddle:
                    Rect.Width += e.X - _oldX;
                    break;

                case PosSizableRect.UpMiddle:
                    Rect.Y += e.Y - _oldY;
                    Rect.Height -= e.Y - _oldY;
                    break;

                default:
                    if (_mMove)
                    {
                        Rect.X = Rect.X + e.X - _oldX;
                        Rect.Y = Rect.Y + e.Y - _oldY;
                        _mPictureBox.Cursor = Cursors.SizeAll;
                    }
                    break;
            }
            _oldX = e.X;
            _oldY = e.Y;

            if (Rect.Width < 5 || Rect.Height < 5)
            {
                Rect = backupRect;
            }

            TestIfRectInsideArea();

            _mPictureBox.Invalidate();
        }

        private void TestIfRectInsideArea()
        {
            // Test if rectangle still inside the area.
            if (Rect.X < 0) Rect.X = 0;
            if (Rect.Y < 0) Rect.Y = 0;
            if (Rect.Width <= 0) Rect.Width = 1;
            if (Rect.Height <= 0) Rect.Height = 1;

            if (Rect.X + Rect.Width > _mPictureBox.Width)
            {
                Rect.Width = _mPictureBox.Width - Rect.X - 1; // -1 to be still show 
                if (AllowDeformingDuringMovement == false)
                {
                    _mIsClick = false;
                }
            }
            if (Rect.Y + Rect.Height > _mPictureBox.Height)
            {
                Rect.Height = _mPictureBox.Height - Rect.Y - 1;// -1 to be still show 
                if (AllowDeformingDuringMovement == false)
                {
                    _mIsClick = false;
                }
            }
        }

        private Rectangle CreateRectSizableNode(int x, int y)
        {
            return new Rectangle(x - _sizeNodeRect / 2, y - _sizeNodeRect / 2, _sizeNodeRect, _sizeNodeRect);
        }

        private Rectangle GetRect(PosSizableRect p)
        {
            switch (p)
            {
                case PosSizableRect.LeftUp:
                    return CreateRectSizableNode(Rect.X, Rect.Y);

                case PosSizableRect.LeftMiddle:
                    return CreateRectSizableNode(Rect.X, Rect.Y + Rect.Height / 2);

                case PosSizableRect.LeftBottom:
                    return CreateRectSizableNode(Rect.X, Rect.Y + Rect.Height);

                case PosSizableRect.BottomMiddle:
                    return CreateRectSizableNode(Rect.X + Rect.Width / 2, Rect.Y + Rect.Height);

                case PosSizableRect.RightUp:
                    return CreateRectSizableNode(Rect.X + Rect.Width, Rect.Y);

                case PosSizableRect.RightBottom:
                    return CreateRectSizableNode(Rect.X + Rect.Width, Rect.Y + Rect.Height);

                case PosSizableRect.RightMiddle:
                    return CreateRectSizableNode(Rect.X + Rect.Width, Rect.Y + Rect.Height / 2);

                case PosSizableRect.UpMiddle:
                    return CreateRectSizableNode(Rect.X + Rect.Width / 2, Rect.Y);
                default:
                    return new Rectangle();
            }
        }

        private PosSizableRect GetNodeSelectable(Point p)
        {
            foreach (PosSizableRect r in Enum.GetValues(typeof(PosSizableRect)))
            {
                if (GetRect(r).Contains(p))
                {
                    return r;
                }
            }
            return PosSizableRect.None;
        }

        private void ChangeCursor(Point p)
        {
            _mPictureBox.Cursor = GetCursor(GetNodeSelectable(p));
        }

        private Cursor GetCursor(PosSizableRect p)
        {
            switch (p)
            {
                case PosSizableRect.LeftUp:
                    return Cursors.SizeNWSE;

                case PosSizableRect.LeftMiddle:
                    return Cursors.SizeWE;

                case PosSizableRect.LeftBottom:
                    return Cursors.SizeNESW;

                case PosSizableRect.BottomMiddle:
                    return Cursors.SizeNS;

                case PosSizableRect.RightUp:
                    return Cursors.SizeNESW;

                case PosSizableRect.RightBottom:
                    return Cursors.SizeNWSE;

                case PosSizableRect.RightMiddle:
                    return Cursors.SizeWE;

                case PosSizableRect.UpMiddle:
                    return Cursors.SizeNS;
                default:
                    return Cursors.Default;
            }
        }
    }
}
