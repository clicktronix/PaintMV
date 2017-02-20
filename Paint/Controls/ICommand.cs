using System.Drawing;
using System.Windows.Forms;
using PaintMV.Shapes;

namespace PaintMV.Controls
{
    /// <summary>
    /// Commands interface
    /// </summary>
    public interface ICommand
    {
        void Execute(Graphics g, MouseEventArgs e, IShape tempShape);

        void Undo();

        void Redo();

        string Operation();
    }
}
