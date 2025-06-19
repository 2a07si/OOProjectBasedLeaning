using System;
using System.Drawing;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class yoyaku : DragDropForm
    {
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
                    MessageBox.Show(guest.Name + "‚³‚ñ‚ÍŠù‚É—\–ñÏ‚İ‚Å‚·B\n—\–ñŠ®—¹“úF" + PastDate(), 
                        "—\–ñd•¡ƒGƒ‰[",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    e.Effect = DragDropEffects.None;
                }
                else
                {
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
            string date = DateTime.Now.ToString("yyyy”NMMŒdd“ú HH:mm:ss");
            return date;
        }
        private string PastDate()
        {
            string past = UpdateTimeLabel();
            return past;
        }
    }
}
