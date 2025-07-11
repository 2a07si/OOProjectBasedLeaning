﻿using System;
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

            var guestForm = new GuestCreatorForm()
            {
                StartPosition = FormStartPosition.Manual,
                Location = new Point(1348, 427)
            };
            guestForm.Show();
            var homeForm = new HomeForm()
            {
                StartPosition = FormStartPosition.Manual,
                Location = new Point(-5, 427)
            };
            homeForm.Show();
            var hotelForm = new HotelForm(homeForm)
            {
                StartPosition = FormStartPosition.Manual,
                Location = new Point(-5, 0)
            };
            hotelForm.Show();
            var yoyakuForm = new YoyakuForm(homeForm)
            {
                StartPosition = FormStartPosition.Manual,
                Location = new Point(630, 427)
            };
            yoyakuForm.Show();
            guestForm.Focus();
            this.Hide();
        }
    }
}