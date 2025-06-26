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
    public partial class RoomSelectForm : Form
    {
        public Room SelectedRoom { get; private set; }

        public RoomSelectForm(List<Room> availableRooms)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(350, 450);

            ComboBox comboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 250,
                Left = 20,
                Top = 20
            };

            foreach (var room in availableRooms)
            {
                comboBox.Items.Add(room);
            }

            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;

            Button confirmBtn = new Button
            {
                Text = "決定",
                Top = 70,
                Left = 40,
                Width = 170,
                Height = 50
            };

            confirmBtn.Click += (s, e) =>
            {
                SelectedRoom = comboBox.SelectedItem as Room;
                DialogResult = DialogResult.OK;
                Close();
            };

            Controls.Add(comboBox);
            Controls.Add(confirmBtn);
        }
    }
}
