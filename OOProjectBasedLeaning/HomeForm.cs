using System;
using System.Drawing;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class HomeForm : DragDropForm
    {
        int panelCount = 1;
        public DateTime? ReservationCompletedTime { get; private set; }

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
                bool isAlreadyOnThisForm = this.Controls.Contains(guestPanel);

                guestPanel.AddDragDropForm(this, PointToClient(new Point(e.X, e.Y)));

                Guest guest = guestPanel.GetGuest();

                if (isAlreadyOnThisForm)
                {
                    panelCount = 0;
                    MessageBox.Show(guest.Name + "さんは既にチェックアウト済みです。\nチェックアウト完了日時：" + UpdateTimeLabel(),
                        "チェックアウト重複エラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    e.Effect = DragDropEffects.None;
                }
                else
                {
                    panelCount++;

                    try
                    {
                        hotel.CheckOut(guest);
                        MessageBox.Show($"{guest.Name} さんがホテルからチェックアウトしました。");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"チェックアウトに失敗しました: {ex.Message}");
                    }
                }
            }
        }
        private string UpdateTimeLabel()
        {
            string date;
            if (ReservationCompletedTime == null || panelCount != 0)
            {
                ReservationCompletedTime = DateTime.Now;
                date = ReservationCompletedTime.Value.ToString("yyyy年MM月dd日 HH:mm:ss");
            }
            else
            {
                date = ReservationCompletedTime.Value.ToString("yyyy年MM月dd日 HH:mm:ss");
            }
            return date;
        }

    }
}
