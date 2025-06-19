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

            new yoyaku().Show();

            new HotelForm().Show();

            new HomeForm().Show();

            new GuestCreatorForm().Show();

        }
    }
}
