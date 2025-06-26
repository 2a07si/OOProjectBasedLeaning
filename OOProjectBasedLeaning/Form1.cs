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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Shown += Form1_Shown;


        }
        private void Form1_Shown(object? sender, EventArgs e)
        {
            var hotelForm = new HotelForm()
            {
                StartPosition = FormStartPosition.Manual,
                Location = new Point(-15, 0)
            };
            hotelForm.Show();

            var guestForm = new GuestCreatorForm()
            {
                StartPosition = FormStartPosition.Manual,
                Location = new Point(1348, 427)
            };
            guestForm.Show();
            var yoyakuForm = new yoyaku()
            {
                StartPosition = FormStartPosition.Manual,
                Location = new Point(450, 427)
            };
            yoyakuForm.Show();

            var homeForm = new HomeForm()
            {
                StartPosition = FormStartPosition.Manual,
                Location = new Point(0, 427)
            };
            homeForm.Show();
            this.Hide();
        }
    }
}