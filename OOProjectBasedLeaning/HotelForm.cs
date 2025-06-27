using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class HotelForm : DragDropForm
    {
        private readonly Hotel hotel = Hotel.Instance;
        private readonly Dictionary<GroupBox, Room> roomBoxes = new();


        private System.Windows.Forms.Timer clockTimer;
        private Label clockLabel;

        public HotelForm()
        {
            InitializeComponent();
            InitializeRoomBoxes();

            InitializeClock();

            hotel.ReservationAdded += OnReservationAdded;
            hotel.CheckedIn += OnHotelCheckedIn;
            hotel.CheckedOut += OnHotelCheckedOut;
            hotel.OperationFailed += OnHotelOperationFailed;

            foreach (var res in hotel.GetAllReservations())
                OnReservationAdded(res);
        }

        // 時計
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

        protected override void OnFormDragEnterSerializable(DragEventArgs e) => e.Effect = DragDropEffects.None;
        protected override void OnFormDragDropSerializable(object? obj, DragEventArgs e) { }

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

                gbx.AllowDrop = true;
                gbx.DragEnter += RoomBox_DragEnterOrOver;
                gbx.DragOver += RoomBox_DragEnterOrOver;
                gbx.DragDrop += RoomBox_DragDrop;
                gbx.ControlRemoved += RoomBox_ControlRemoved;
            }
        }

        private void RoomBox_DragEnterOrOver(object sender, DragEventArgs e)
        {
            e.Effect = (e.Data.GetDataPresent(DataFormats.Serializable) &&
                        e.Data.GetData(DataFormats.Serializable) is GuestPanel)
                       ? DragDropEffects.Move
                       : DragDropEffects.None;
        }

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

        private void RoomBox_DragDrop(object sender, DragEventArgs e)
        {
            if (!(e.Data.GetDataPresent(DataFormats.Serializable) &&
                  e.Data.GetData(DataFormats.Serializable) is GuestPanel gp))
                return;

            // Room→Room の直接移動は禁止
            var oldBox = gp.Parent as GroupBox;
            if (oldBox != null)
            {
                MessageBox.Show(
                    "部屋から部屋への直接移動はできません。\n一度チェックアウトしてから再度チェックインしてください。",
                    "移動不可",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

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
                Location = new Point(10, 20)
            };
            gbx.Controls.Add(lbl);
            UpdateRoomColor(gbx, res.Room);
        }

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

        private void MoveGuestPanelTo(GuestPanel gp, GroupBox targetBox)
        {
            gp.Parent?.Controls.Remove(gp);
            targetBox.Controls.Add(gp);
            gp.Location = new Point(10, 20);
            gp.BringToFront();
        }

        private void UpdateRoomColor(GroupBox gbx, Room room)
        {
            if (hotel.IsOccupied(room)) gbx.BackColor = Color.LightCoral;
            else if (hotel.IsReserved(room)) gbx.BackColor = Color.LightBlue;
            else gbx.BackColor = Color.LightGreen;
        }
    }
}
