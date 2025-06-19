using System;
using System.Drawing;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class HomeForm : DragDropForm
    {
        public HomeForm()
        {
            Text = "Home Form";
            Size = new Size(400, 400);
            BackColor = Color.White;
        }

        protected override void OnFormDragEnterSerializable(DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        protected override void OnFormDragDropSerializable(object? obj, DragEventArgs e)
        {
            if (obj is DragDropPanel panel)
            {
                panel.AddDragDropForm(this, PointToClient(new Point(e.X, e.Y)));
            }
        }
    }
}
