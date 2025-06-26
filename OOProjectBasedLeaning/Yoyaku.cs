using System;
using System.Drawing;
using System.Windows.Forms;
using OOProjectBasedLeaning;


namespace OOProjectBasedLeaning
{
    public partial class yoyaku : DragDropForm
    {
        private List<Room> allRooms;
        private List<Room> reservedRooms;
        private List<Room> availableRooms;

        private Hotel hotel = Hotel.Instance;

        int panelCount = 1;

        public DateTime? ReservationCompletedTime { get; private set; }

        private Dictionary<Guest, Room> guestRoomMap = new();
        private Guest guestLeader;

        public yoyaku()
        {
            this.Text = "予約管理";
            this.Size = new Size(800, 600);

           

            reservedRooms = new List<Room>();
            allRooms = new List<Room>
            {
                new RegularRoom(501, 15000), new RegularRoom(502, 15000), new RegularRoom(503, 12000),
                new RegularRoom(601, 16000), new RegularRoom(602, 16000), new RegularRoom(603, 15000),
                new RegularRoom(701, 17000), new RegularRoom(702, 17000), new RegularRoom(703, 16000),
                new RegularRoom(801, 18000), new RegularRoom(802, 18000),
                new SuiteRoom  (1001, 360000), new SuiteRoom  (1002, 300000),
            };
            UpdateAvailableRooms();

        }

        protected override void OnFormDragEnterSerializable(DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        protected override void OnFormDragDropSerializable(object? obj, DragEventArgs e)
        {
            var roomForm = new RoomSelectForm(allRooms, reservedRooms, guestLeader);

            if (obj is GuestPanel guestPanel)
            {
                Guest guest = guestPanel.GetGuest();

                bool isAlreadyOnThisForm = this.Controls.Contains(guestPanel);

               guestPanel.AddDragDropForm(this, PointToClient(new Point(e.X, e.Y)));

                string reservedRoomNumber = guestRoomMap.TryGetValue(guest, out var room)
                  ? room.RoomNumber.ToString()
                  : "不明";

            if (isAlreadyOnThisForm)
                {
                    panelCount = 0;

                    MessageBox.Show(guest.Name + "さんは既に予約済みです。\n予約完了日時：" + UpdateTimeLabel() + "\n予約部屋番号：" + reservedRoomNumber,
                        "予約重複エラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    e.Effect = DragDropEffects.None;
                }
                else
                {
                    panelCount++;

                    var selectForm = new RoomSelectForm(availableRooms, reservedRooms, guestLeader);

                    if (selectForm.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            Room selectRoom = selectForm.SelectedRoom;

                            if (selectRoom == null)
                                throw new InvalidOperationException("部屋が選択されていません。");

                            // 予約処理（内部でVIP/会員判定も行われる）
                            var guests = new List<Guest> { guest };

                            if (guest.Companions is List<Guest> companions)
                            {
                                guests.AddRange(companions);
                            }

                            selectRoom.AddGuests(guests);

                            guestRoomMap[guest] = selectRoom;
                            reservedRooms.Add(selectRoom);
                            UpdateAvailableRooms();

                            MessageBox.Show(
                                $"{guest.Name}さんの予約が完了しました。\n" +
                                $"予約完了日時：{UpdateTimeLabel()}\n" +
                                $"予約された部屋番号：{selectRoom.RoomNumber}"
                            );
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(
                                $"{guest.Name}さんの予約に失敗しました：\n{ex.Message}",
                                "予約エラー",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            if (reservedRoomNumber == "不明")
                            {
                                guestPanel.BackColor = Color.Yellow;
                            }

                        }
                    }
                    else
                    {
                        MessageBox.Show("部屋の選択がキャンセルされました。");
                        if (reservedRoomNumber == "不明")
                        {
                            guestPanel.BackColor = Color.Yellow;
                        }
                    }
                }
               
            }
        }
        private string UpdateTimeLabel()
        {
            string date;　
            if (ReservationCompletedTime == null || panelCount!=0)
            {
                ReservationCompletedTime = DateTime.Now;
                date = ReservationCompletedTime.Value.ToString("yyyy年MM月dd日 HH:mm:ss");
            }
            else
            {
                date = ReservationCompletedTime.Value.ToString("yyyy年MM月dd日 HH:mm:ss");
            }
            return date;
        }
        private void UpdateAvailableRooms()
        {
            availableRooms = allRooms
                .Where(room => !reservedRooms.Any(r => r.RoomNumber == room.RoomNumber))
                .ToList();
        }
    }
}
