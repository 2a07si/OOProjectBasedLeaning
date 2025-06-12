using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOProjectBasedLeaning
{

    public class GuestPanel : Panel
    {

        private Guest guest;

        public GuestPanel(Guest guest)
        {

            this.guest = guest;

            InitializeComponent();

        }

        private void InitializeComponent()
        {

            Label guestNameLabel = new Label
            {
                Text = guest.Name,
                AutoSize = true,
                Location = new Point(10, 10)
            };

            TextBox guestNameTextBox = new TextBox
            {
                Text = guest.Name,
                Location = new Point(120, 6),
                Width = 160
            };

            Controls.Add(guestNameLabel);
            Controls.Add(guestNameTextBox);

        }

    }

}
