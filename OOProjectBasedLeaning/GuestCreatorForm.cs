using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class GuestCreatorForm : Form
    {
        private TextBox nameTextBox;

        public GuestCreatorForm()
        {
            InitializeComponent();
            nameTextBox = new TextBox()
            {
                Location = new Point(10, 20),
                Size = new Size(200, 31)
            };
            Controls.Add(nameTextBox);
        }

        private void CreateGuestEvent(object sender, EventArgs e)
        {
            string guestName = nameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(guestName))
            {
                MessageBox.Show("名前を入力してください。");
                return;
            }

            Guest guest = CreateGuest(guestName);
            int panelCount = Controls.OfType<GuestPanel>().Count();

            GuestPanel panel = new GuestPanel(guest)
            {
                Location = new Point(10, 60 + panelCount * 60),
                Size = new Size(300, 40)
            };

            Controls.Add(panel);
            nameTextBox.Clear();
        }

        private Guest CreateGuest(string guestName) => new GuestModel(guestName);
        private Guest CreateMember() => new MemberModel("Member");
    }
}