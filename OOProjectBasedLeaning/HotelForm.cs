using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class HotelForm : DragDropForm
    {
        private readonly Hotel hotel = Hotel.Instance; // ホテルのシングルトンインスタンス
        private readonly HomeForm homeForm; // ホームフォームの参照

        // 各部屋GroupBoxとRoomの対応辞書
        private readonly Dictionary<GroupBox, Room> roomBoxes = new();

        private System.Windows.Forms.Timer clockTimer; // 時計用タイマー
        private Label clockLabel; // 時計表示用ラベル

        public HotelForm(HomeForm home)
        {
            InitializeComponent();
            homeForm = home;

            InitializeRoomBoxes(); // 部屋GroupBox初期化
            InitializeClock();     // 時計初期化

            // イベント購読
            hotel.ReservationAdded += OnReservationAdded;
            hotel.CheckedIn += OnHotelCheckedIn;
            hotel.CheckedOut += OnHotelCheckedOut;
            hotel.OperationFailed += OnHotelOperationFailed;

            // 予約済み部屋の再描画
            foreach (var res in hotel.AllRooms
                             .Where(r => r.IsReserved())
                             .Select(r => new Reservation(r.ReservedBy, r, r.ReservedCheckIn(), r.ReservedCheckOut())))
            {
                OnReservationAdded(res);
            }
        }

        // 部屋GroupBoxの初期化とイベントハンドラの登録
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
                int roomNumber = int.Parse(gbx.Text.Replace("号室", ""));
                var room = hotel.GetRoomByNumber(roomNumber);

                roomBoxes[gbx] = room;
                UpdateRoomColor(gbx, room);

                // ドラッグ＆ドロップ設定
                gbx.AllowDrop = true;
                gbx.DragEnter += RoomBox_DragEnterOrOver;
                gbx.DragOver += RoomBox_DragEnterOrOver;
                gbx.DragDrop += RoomBox_DragDrop;
                gbx.ControlRemoved += RoomBox_ControlRemoved;
            }
        }

        // 時計ラベルとタイマーの初期化
        private void InitializeClock()
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

            clockTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            clockTimer.Tick += (s, e) =>
            {
                clockLabel.Text = DateTime.Now.ToString("HH:mm:ss");
            };
            clockTimer.Start();
            clockLabel.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        // フォームへのドロップ無効
        protected override void OnFormDragEnterSerializable(DragEventArgs e) => e.Effect = DragDropEffects.None;
        protected override void OnFormDragDropSerializable(object? obj, DragEventArgs e) { }

        // ドラッグ可能判定
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
                // OperationFailedイベントで処理済
            }
        }

        // 予約表示
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
                Location = new Point(10, 46)
            };
            gbx.Controls.Add(lbl);
            UpdateRoomColor(gbx, res.Room);
        }

        // チェックイン時の表示更新と通知
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

        // チェックアウト時の表示更新と通知
        private void OnHotelCheckedOut(object? sender, HotelEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnHotelCheckedOut(sender, e)));
                return;
            }

            var gbx = roomBoxes.First(kv => kv.Value == e.Room).Key;

            // 予約ラベル削除
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

        // エラー通知処理
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

        // ゲストパネルを部屋内に配置
        private void MoveGuestPanelTo(GuestPanel gp, GroupBox targetBox)
        {
            gp.Parent?.Controls.Remove(gp);
            targetBox.Controls.Add(gp);
            gp.Location = new Point(15, 30);
            gp.BringToFront();
        }

        // 部屋の色を予約/使用状況に応じて変更
        private void UpdateRoomColor(GroupBox gbx, Room room)
        {
            if (hotel.IsOccupied(room)) gbx.BackColor = Color.LightCoral; // 使用中
            else if (room.IsReserved()) gbx.BackColor = Color.LightBlue;  // 予約済み
            else gbx.BackColor = Color.LightGreen;                        // 空室
        }
    }
}
