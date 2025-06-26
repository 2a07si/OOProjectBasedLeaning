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
            this.Text = "�\��Ǘ�";
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
                  : "�s��";

            if (isAlreadyOnThisForm)
                {
                    panelCount = 0;

                    MessageBox.Show(guest.Name + "����͊��ɗ\��ς݂ł��B\n�\�񊮗������F" + UpdateTimeLabel() + "\n�\�񕔉��ԍ��F" + reservedRoomNumber,
                        "�\��d���G���[",
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
                                throw new InvalidOperationException("�������I������Ă��܂���B");

                            // �\�񏈗��i������VIP/���������s����j
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
                                $"{guest.Name}����̗\�񂪊������܂����B\n" +
                                $"�\�񊮗������F{UpdateTimeLabel()}\n" +
                                $"�\�񂳂ꂽ�����ԍ��F{selectRoom.RoomNumber}"
                            );
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(
                                $"{guest.Name}����̗\��Ɏ��s���܂����F\n{ex.Message}",
                                "�\��G���[",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            if (reservedRoomNumber == "�s��")
                            {
                                guestPanel.BackColor = Color.Yellow;
                            }

                        }
                    }
                    else
                    {
                        MessageBox.Show("�����̑I�����L�����Z������܂����B");
                        if (reservedRoomNumber == "�s��")
                        {
                            guestPanel.BackColor = Color.Yellow;
                        }
                    }
                }
               
            }
        }
        private string UpdateTimeLabel()
        {
            string date;�@
            if (ReservationCompletedTime == null || panelCount!=0)
            {
                ReservationCompletedTime = DateTime.Now;
                date = ReservationCompletedTime.Value.ToString("yyyy�NMM��dd�� HH:mm:ss");
            }
            else
            {
                date = ReservationCompletedTime.Value.ToString("yyyy�NMM��dd�� HH:mm:ss");
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
