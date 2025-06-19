using System.Drawing;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public class GuestPanel : DragDropPanel
    {
        private Guest guest;
        private Label guestNameLabel;

        public GuestPanel(Guest guest)
        {
            this.guest = guest;
            Size = new Size(300, 40);
            BackColor = Color.LightYellow;
            InitializeComponent();
        }

        public Guest GetGuest() => guest;

        private void InitializeComponent()
        {
            guestNameLabel = new Label
            {
                Text = $"ゲスト名： {guest.Name}",
                AutoSize = true,
                Location = new Point(10, 10)
            };
            Controls.Add(guestNameLabel);
        }

        protected override void OnPanelMouseDown()
        {
            DoDragDropMove();
        }
    }
}