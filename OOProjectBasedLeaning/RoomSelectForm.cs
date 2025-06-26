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

        public RoomSelectForm(List<Room> availableRooms, List<Room> reservedRooms, Guest guestLeader)
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

            var filteredRooms = availableRooms
            .Where(room => !reservedRooms.Contains(room))
            .ToList();


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
               var select = comboBox.SelectedItem as Room;

                //bool suite = select.RoomNumber >= 1000;
                //bool hasAuthority = guestLeader.IsVIP() || guestLeader.IsMember() || guestLeader.Any(c => c.IsVIP() || c.IsMember());

                //if (suite && !hasAuthority)
                //{
                //    MessageBox.Show("スイートルームはVIPまたは会員の同行者が必要です。", "予約不可", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return;
                //}

                if (reservedRooms.Contains(select))
                {
                    MessageBox.Show("この部屋はすでに予約されています。", "予約エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // 選択処理をなかったことにする
                }

                SelectedRoom = select;
                DialogResult = DialogResult.OK;
                Close();
            };

            Controls.Add(comboBox);
            Controls.Add(confirmBtn);
        }
    }
}
