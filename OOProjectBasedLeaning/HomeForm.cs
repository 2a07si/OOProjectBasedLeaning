using System;
using System.Drawing;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class HomeForm : DragDropForm
    {
        private Hotel hotel = new Hotel();

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
            if (obj is GuestPanel guestPanel)
            {
                guestPanel.AddDragDropForm(this, PointToClient(new Point(e.X, e.Y)));

                Guest guest = guestPanel.GetGuest();
                try
                {
                    hotel.CheckOut(guest);
                    MessageBox.Show($"{guest.Name} さんがホテルにチェックアウトしました。");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"チェックアウトに失敗しました: {ex.Message}");
                }
            }
        }
    }
}
