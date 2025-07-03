using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class RoomSelectForm : Form
    {
        // 選択された部屋を保持するプロパティ
        public Room SelectedRoom { get; private set; }

        public RoomSelectForm(List<Room> availableRooms, List<Room> reservedRooms, Guest guestLeader)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(350, 450);

            // コンボボックスのセットアップ
            ComboBox comboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 250,
                Left = 20,
                Top = 20
            };

            // 利用可能な部屋リストから予約済みを除外し、
            // スイートルームは会員またはVIP、もしくは同行者に会員/VIPがいる場合のみ表示
            var filteredRooms = availableRooms
                .Where(room => !reservedRooms.Contains(room))
                .Where(room =>
                    !(room is SuiteRoom)
                    || guestLeader.IsMember()
                    || guestLeader.IsVIP()
                    || guestLeader.Companions.Any(c => c.IsMember() || c.IsVIP())
                )
                .ToList();

            // フィルタリング後の部屋をコンボに追加
            foreach (var room in filteredRooms)
            {
                comboBox.Items.Add(room);
            }

            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;

            // 決定ボタンのセットアップ
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
                if (select == null)
                {
                    MessageBox.Show("部屋が選択されていません。", "予約エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 最終的な権限チェック（念のため）
                bool isSuite = select is SuiteRoom;
                bool hasAuthority = guestLeader.IsMember()
                                     || guestLeader.IsVIP()
                                     || guestLeader.Companions.Any(c => c.IsMember() || c.IsVIP());
                if (isSuite && !hasAuthority)
                {
                    MessageBox.Show("スイートルームはVIPまたは会員、もしくは同行者に権限者が必要です。", "予約不可", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 追加で予約済みチェック
                if (reservedRooms.Contains(select))
                {
                    MessageBox.Show("この部屋はすでに予約されています。", "予約エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
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
