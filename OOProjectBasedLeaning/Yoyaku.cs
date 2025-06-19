using System;
using System.Drawing;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class yoyaku : DragDropForm
    {
        public yoyaku()
        {
            this.Text = "ó\ñÒä«óù";
            this.Size = new Size(800, 600);

            new GuestCreatorForm().Show();
            new HomeForm().Show();
            new HotelForm().Show();
        }

        protected override void OnFormDragEnterSerializable(DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        protected override void OnFormDragDropSerializable(object? obj, DragEventArgs e)
        {
            if (obj is DragDropPanel panel)
                panel.AddDragDropForm(this, PointToClient(new Point(e.X, e.Y)));
            MessageBox.Show("ó\ñÒÇ™äÆóπÇµÇ‹ÇµÇΩ");
        }
    }
}
