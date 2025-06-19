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
            this.Text = "—\–ñŠÇ—";
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
                    MessageBox.Show(guest.Name + "‚³‚ñ‚ÍŠù‚É—\–ñÏ‚İ‚Å‚·B\n—\–ñŠ®—¹“úF" + UpdateTimeLabel(), 
                        "—\–ñd•¡ƒGƒ‰[",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    e.Effect = DragDropEffects.None;
                }
                else
                {
                    panelCount++;
                    try
                    {
                        MessageBox.Show(guest.Name + "‚³‚ñ‚Ì—\–ñ‚ªŠ®—¹‚µ‚Ü‚µ‚½B\n—\–ñŠ®—¹“úF" + UpdateTimeLabel());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(guest.Name + "‚³‚ñ‚Ì—\–ñ‚É¸”s‚µ‚Ü‚µ‚½B" + ex.Message);
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
                date = ReservationCompletedTime.Value.ToString("yyyy”NMMŒdd“ú HH:mm:ss");
            }
            else
            {
                date = ReservationCompletedTime.Value.ToString("yyyy”NMMŒdd“ú HH:mm:ss");
            }
            return date;
        }
    }
}
