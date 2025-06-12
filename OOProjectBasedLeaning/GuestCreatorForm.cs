using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{

    public partial class GuestCreatorForm : Form
    {

        public GuestCreatorForm()
        {

            InitializeComponent();

        }

        private void CreateGuestEvent(object sender, EventArgs e)
        {

            Controls.Add(new GuestPanel(CreateGuest())
            {
                Location = new Point(10, 10 + Controls.Count * 30),
                Width = 300,
            });

        }

        private Guest CreateGuest()
        {

            return new GuestModel("Guest");

        }

        private Guest CreateMember()
        {

            return new MemberModel("Member");

        }

    }

}
