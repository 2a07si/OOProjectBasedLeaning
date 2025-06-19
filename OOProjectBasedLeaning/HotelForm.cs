using System;
using System.Drawing;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class HotelForm : DragDropForm
    {
        private Hotel hotel = new Hotel();

        public HotelForm()
        {
            InitializeComponent();
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
                    hotel.CheckIn(guest);
                    MessageBox.Show($"{guest.Name} さんがホテルにチェックインしました。");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"チェックインに失敗しました: {ex.Message}");
                }
            }
        }
    }
}