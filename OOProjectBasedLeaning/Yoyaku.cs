using System;
using System.Drawing;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class yoyaku : DragDropForm
    {
        public yoyaku()
        {
            this.Text = "�\��Ǘ�";
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
                    MessageBox.Show(guest.Name + "����͊��ɗ\��ς݂ł��B\n�\�񊮗������F" + PastDate(), 
                        "�\��d���G���[",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    e.Effect = DragDropEffects.None;
                }
                else
                {
                    try
                    {
                        MessageBox.Show(guest.Name + "����̗\�񂪊������܂����B\n�\�񊮗������F" + UpdateTimeLabel());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(guest.Name + "����̗\��Ɏ��s���܂����B" + ex.Message);
                    }
                }
               
            }
        }
        private string UpdateTimeLabel()
        {
            string date = DateTime.Now.ToString("yyyy�NMM��dd�� HH:mm:ss");
            return date;
        }
        private string PastDate()
        {
            string past = UpdateTimeLabel();
            return past;
        }
    }
}
