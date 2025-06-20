using System;
using System.Drawing;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class yoyaku : DragDropForm
    {
        int panelCount = 1;
        public DateTime? ReservationCompletedTime { get; private set; }
        public yoyaku()
        {
            this.Text = "予約管理";
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
            if (obj is GuestPanel guestPanel)
            {
                bool isAlreadyOnThisForm = this.Controls.Contains(guestPanel);

                guestPanel.AddDragDropForm(this, PointToClient(new Point(e.X, e.Y)));

                Guest guest = guestPanel.GetGuest();

                if (isAlreadyOnThisForm)
                {
                    panelCount = 0;
                    MessageBox.Show(guest.Name + "さんは既に予約済みです。\n予約完了日時：" + UpdateTimeLabel(), 
                        "予約重複エラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    e.Effect = DragDropEffects.None;
                }
                else
                {
                    panelCount++;
                    try
                    {
                        MessageBox.Show(guest.Name + "さんの予約が完了しました。\n予約完了日時：" + UpdateTimeLabel());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(guest.Name + "さんの予約に失敗しました。" + ex.Message);
                    }
                }
               
            }
        }
        private string UpdateTimeLabel()
        {
            string date;
            if (ReservationCompletedTime == null || panelCount!=0)
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
