using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class HotelForm : DragDropForm
    {
        // シングルトンインスタンス
        private readonly Hotel hotel = Hotel.Instance;
        private readonly HomeForm homeForm; 
        // 各 GroupBox と対応する Room オブジェクトを保持するマップ
        private readonly Dictionary<GroupBox, Room> roomBoxes = new();

        // デジタル時計用のタイマー
        private System.Windows.Forms.Timer clockTimer;
        // 現在時刻を表示するラベル
        private Label clockLabel;

        public HotelForm(HomeForm home)
        {
            InitializeComponent();
            homeForm = home;
            // 部屋構築
            InitializeRoomBoxes();

            InitializeClock(); // 時計配置

            hotel.ReservationAdded += OnReservationAdded;    // 予約追加時
            hotel.CheckedIn += OnHotelCheckedIn;             // チェックイン成功時
            hotel.CheckedOut += OnHotelCheckedOut;           // チェックアウト成功時
            hotel.OperationFailed += OnHotelOperationFailed; // 操作失敗時

            foreach (var res in hotel.AllRooms
                            .Where(r => r.IsReserved)
                            .Select(r => new Reservation(r.ReservedBy!, r, r.ReservedCheckIn!.Value, r.ReservedCheckOut!.Value)))
            {
                OnReservationAdded(res);
            }
        }

        private void InitializeRoomBoxes() // 部屋構築　&　ドラッグドロップのイベントパンドラ設定
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
                int roomNumber = int.Parse(gbx.Text.Replace("号室", ""));
                var room = hotel.GetRoomByNumber(roomNumber);

                roomBoxes[gbx] = room;
                UpdateRoomColor(gbx, room);

                gbx.AllowDrop = true;
                gbx.DragEnter += RoomBox_DragEnterOrOver;
                gbx.DragOver += RoomBox_DragEnterOrOver;
                gbx.DragDrop += RoomBox_DragDrop;
                gbx.ControlRemoved += RoomBox_ControlRemoved;
            }
        }

        private void InitializeClock() // 時計
        {
            clockLabel = new Label
            {
                Name = "lblClock",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(this.ClientSize.Width - 100, 70),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            Controls.Add(clockLabel);

            // System.Windows.Forms.Timer を使い 1 秒ごとに Tick
            clockTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            clockTimer.Tick += (s, e) =>
            {
                // 毎 Tick 時に現在時刻を更新
                clockLabel.Text = DateTime.Now.ToString("HH:mm:ss");
            };
            clockTimer.Start();

            // 初回表示
            clockLabel.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        protected override void OnFormDragEnterSerializable(DragEventArgs e) => e.Effect = DragDropEffects.None;
        protected override void OnFormDragDropSerializable(object? obj, DragEventArgs e) { }



        private void RoomBox_DragEnterOrOver(object sender, DragEventArgs e)
        {
            e.Effect = (e.Data.GetDataPresent(DataFormats.Serializable) &&
                        e.Data.GetData(DataFormats.Serializable) is GuestPanel)
                       ? DragDropEffects.Move
                       : DragDropEffects.None;
        }

        // チェックアウト
        private void RoomBox_ControlRemoved(object sender, ControlEventArgs e) 
        {
            if (e.Control is GuestPanel gp && sender is GroupBox gbx)
            {
                var guest = gp.GetGuest();
                var room = roomBoxes[gbx];
                if (hotel.IsOccupied(room))
                    hotel.CheckOut(guest);
            }
        }

        // チェックイン
        private void RoomBox_DragDrop(object sender, DragEventArgs e)
        {
            if (!(e.Data.GetDataPresent(DataFormats.Serializable) &&
                  e.Data.GetData(DataFormats.Serializable) is GuestPanel gp))
                return;

            var newBox = (GroupBox)sender;
            var guest = gp.GetGuest();
            var roomNumber = roomBoxes[newBox].Number;

            try
            {
                hotel.CheckIn(roomNumber, guest);
                MoveGuestPanelTo(gp, newBox);
            }
            catch
            {
                // エラーは OperationFailed イベントで表示
            }
        }

        // 予約
        private void OnReservationAdded(Reservation res)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnReservationAdded(res)));
                return;
            }

            var gbx = roomBoxes.First(kv => kv.Value == res.Room).Key;
            var lbl = new Label
            {
                Text = $"予約: {res.Guest.Name}",
                AutoSize = true,
                Location = new Point(10,46)
            };
            gbx.Controls.Add(lbl);
            UpdateRoomColor(gbx, res.Room);
        }

        /// <summary>
        /// CheckedIn イベントハンドラ：チェックイン成功時に色更新とダイアログ表示
        /// </summary>
        private void OnHotelCheckedIn(object? sender, HotelEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnHotelCheckedIn(sender, e)));
                return;
            }

            var gbx = roomBoxes.First(kv => kv.Value == e.Room).Key;
            UpdateRoomColor(gbx, e.Room);
            MessageBox.Show(
                $"{e.Guest.Name} さんを {e.Room.Number}号室 にチェックインしました。\n日時：{e.Timestamp:yyyy/MM/dd HH:mm:ss}",
                "チェックイン完了",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        /// <summary>
        /// CheckedOut イベントハンドラ：チェックアウト成功時に予約ラベル削除・色更新・ダイアログ表示
        /// </summary>
        private void OnHotelCheckedOut(object? sender, HotelEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnHotelCheckedOut(sender, e)));
                return;
            }

            var gbx = roomBoxes.First(kv => kv.Value == e.Room).Key;

            // 予約ラベルを消去
            var toRemove = gbx.Controls
                             .OfType<Label>()
                             .Where(l => l.Text.StartsWith("予約:"))
                             .ToList();
            toRemove.ForEach(lbl => gbx.Controls.Remove(lbl));

            UpdateRoomColor(gbx, e.Room);
            MessageBox.Show(
                $"{e.Guest.Name} さんを {e.Room.Number}号室 からチェックアウトしました。\n日時：{e.Timestamp:yyyy/MM/dd HH:mm:ss}",
                "チェックアウト完了",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }


        /// <summary>
        /// OperationFailed イベントハンドラ：操作失敗時にエラーメッセージを表示
        /// </summary>
        private void OnHotelOperationFailed(object? sender, HotelErrorEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnHotelOperationFailed(sender, e)));
                return;
            }

            MessageBox.Show(
                $"操作に失敗しました：\n{e.ErrorMessage}",
                "エラー",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        // グループボックス内に入ったパネル位置を調整
        private void MoveGuestPanelTo(GuestPanel gp, GroupBox targetBox)
        {
            gp.Parent?.Controls.Remove(gp);
            targetBox.Controls.Add(gp);
            gp.Location = new Point(15, 30);
            gp.BringToFront();
        }

        // 部屋の色替え
        private void UpdateRoomColor(GroupBox gbx, Room room)
        {
            if (hotel.IsOccupied(room)) gbx.BackColor = Color.LightCoral;
            else if (room.IsReserved) gbx.BackColor = Color.LightBlue;
            else gbx.BackColor = Color.LightGreen;
        }
    }
}
