using System.Windows.Forms;

namespace PaintMV
{
    sealed class MyPanel : Panel
    {
        public MyPanel()
        {
            this.DoubleBuffered = true;
        }
    }
}
