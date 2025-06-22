using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class HotelForm : DragDropForm
    {
        // ホテルのロジックを保持
        private readonly Hotel hotel = new Hotel();
        // GroupBox ↔ Room の対応マップ
        private readonly Dictionary<GroupBox, Room> roomBoxes = new();

        public HotelForm()
        {
            InitializeComponent();
            InitializeRoomBoxes();
        }

        // フォーム直下へのドラッグは無効化
        protected override void OnFormDragEnterSerializable(DragEventArgs e) => e.Effect = DragDropEffects.None;
        protected override void OnFormDragDropSerializable(object? obj, DragEventArgs e) { }

        /// <summary>
        /// 各号室 GroupBox に対応する Room をマッピングし、
        /// 初期色設定と DragDrop ハンドラを登録
        /// </summary>
        private void InitializeRoomBoxes()
        {
            var boxes = new[]
            {
                gbx_5_1, gbx_5_2, gbx_5_3,
                gbx_6_1, gbx_6_2, gbx_6_3,
                gbx_7_1, gbx_7_2, gbx_7_3,
                gbx_8_1, gbx_8_2,
                gbx_10_1, gbx_10_2
            };

            foreach (var gbx in boxes)
            {
                int num = int.Parse(gbx.Text.Replace("号室", ""));
                var room = hotel.GetRoomByNumber(num);

                roomBoxes[gbx] = room;
                UpdateRoomColor(gbx, room);

                // ドラッグの許可とイベント登録
                gbx.AllowDrop = true;
                gbx.DragEnter += RoomBox_DragEnterOrOver;
                gbx.DragOver += RoomBox_DragEnterOrOver;
                gbx.DragDrop += RoomBox_DragDrop;
                gbx.ControlRemoved += RoomBox_ControlRemoved;
            }
        }

        private void RoomBox_DragEnterOrOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Serializable)
                && e.Data.GetData(DataFormats.Serializable) is GuestPanel)
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        // チェックアウト処理
        private void RoomBox_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (e.Control is GuestPanel gp)
            {
                var gbx = (GroupBox)sender;
                var room = roomBoxes[gbx];
                var guest = gp.GetGuest();

                if (hotel.IsOccupied(room))
                {
                    hotel.CheckOut(guest);
                    UpdateRoomColor(gbx, room);
                }
            }
        }

        /// <summary>
        /// ドロップ時のチェックイン処理 → UI 移動 → 色更新／メッセージ表示
        /// </summary>
        private void RoomBox_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.Serializable) ||
                !(e.Data.GetData(DataFormats.Serializable) is GuestPanel gp))
                return;

            var newBox = (GroupBox)sender;
            var newRoom = roomBoxes[newBox];
            var guest = gp.GetGuest();

            // 重複チェック
            if (newBox.Controls.Contains(gp))
            {
                MessageBox.Show(
                    $"{guest.Name} さんは既にこの部屋にいます。",
                    "チェックイン重複",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // 旧部屋チェックアウト
            var oldBox = gp.Parent as GroupBox;
            Room oldRoom = null;
            if (oldBox != null)
                roomBoxes.TryGetValue(oldBox, out oldRoom);
            if (oldRoom != null)
                hotel.CheckOut(guest);

            // 新部屋チェックイン試行
            try
            {
                hotel.CheckIn(newRoom.Number, guest);
            }
            catch (Exception ex)
            {
                // 失敗したらロールバック：旧ルームに再チェックイン
                if (oldRoom != null)
                    hotel.CheckIn(oldRoom.Number, guest);

                MessageBox.Show(
                    $"チェックインに失敗しました: {ex.Message}",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            // UI 移動 → 色更新 → メッセージ
            if (oldBox != null)
            {
                oldBox.Controls.Remove(gp);
                UpdateRoomColor(oldBox, oldRoom);
            }
            else
            {
                gp.Parent?.Controls.Remove(gp);
            }

            newBox.Controls.Add(gp);
            gp.Location = new Point(10, 20);
            gp.BringToFront();
            UpdateRoomColor(newBox, newRoom);

            MessageBox.Show(
                $"{guest.Name} さんを {newRoom.Number}号室 にチェックインしました。",
                "チェックイン完了",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        // 部屋の状態に応じて GroupBox の背景色を変更
        private void UpdateRoomColor(GroupBox gbx, Room room)
        {
            if (hotel.IsOccupied(room))
                gbx.BackColor = Color.LightCoral;   // 使用中
            else if (hotel.IsReserved(room))
                gbx.BackColor = Color.LightBlue;    // 予約済
            else
                gbx.BackColor = Color.LightGreen;   // 空室
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) // ショートカットキー
        {
            // F12キーを押した時
            if (keyData == Keys.F12)
            {
                Application.Exit(); // プログラム終了
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}