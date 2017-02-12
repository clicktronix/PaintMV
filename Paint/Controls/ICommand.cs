using System.Drawing;
using System.Windows.Forms;
using PaintMV.Shapes;

namespace PaintMV.Controls
{
    public interface ICommand
    {
        void Execute(Graphics g, MouseEventArgs e, Shape tempShape);

        void Undo();

        void Redo();
    }
}
