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

        int panelCount = 1;

        public DateTime? ReservationCompletedTime { get; private set; }

        private Dictionary<Guest, Room> guestRoomMap = new();
        private Guest guestLeader;

        public yoyaku()
        {
            this.Text = "ó\ñÒä«óù";
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
                bool isAlreadyOnThisForm = this.Controls.Contains(guestPanel);

               guestPanel.AddDragDropForm(this, PointToClient(new Point(e.X, e.Y)));



                Guest guest = guestPanel.GetGuest();

                if (isAlreadyOnThisForm)
                {
                    panelCount = 0;

                    string reservedRoomNumber = guestRoomMap.TryGetValue(guest, out var room)
                    ? room.RoomNumber.ToString()
                    : "ïsñæ";

                    MessageBox.Show(guest.Name + "Ç≥ÇÒÇÕä˘Ç…ó\ñÒçœÇ›Ç≈Ç∑ÅB\nó\ñÒäÆóπì˙éûÅF" + UpdateTimeLabel() + "\nó\ñÒïîâÆî‘çÜÅF" + reservedRoomNumber,
                        "ó\ñÒèdï°ÉGÉâÅ[",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    e.Effect = DragDropEffects.None;
                }
                else
                {
                    panelCount++;

                    var selectForm = new RoomSelectForm(availableRooms,reservedRooms, guestLeader);
                    if (selectForm.ShowDialog() == DialogResult.OK)
                    {
                        Room selectRoom = selectForm.SelectedRoom;

                        if (selectRoom != null)
                        {
                            selectRoom.AddGuests(new List<Guest> { guest });
                            guestRoomMap[guest] = selectRoom;
                            reservedRooms.Add(selectRoom);
                            UpdateAvailableRooms();

                            try
                            {
                                guestRoomMap[guest] = selectRoom;
                                reservedRooms.Add(selectRoom);
                                MessageBox.Show(guest.Name + "Ç≥ÇÒÇÃó\ñÒÇ™äÆóπÇµÇ‹ÇµÇΩÅB\nó\ñÒäÆóπì˙éûÅF" + UpdateTimeLabel() + "\nó\ñÒÇ≥ÇÍÇΩïîâÆî‘çÜÅF" + selectRoom.RoomNumber);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(guest.Name + "Ç≥ÇÒÇÃó\ñÒÇ…é∏îsÇµÇ‹ÇµÇΩÅB" + ex.Message);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("ïîâÆÇÃëIëÇ™ÉLÉÉÉìÉZÉãÇ≥ÇÍÇ‹ÇµÇΩÅB");
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
                date = ReservationCompletedTime.Value.ToString("yyyyîNMMåéddì˙ HH:mm:ss");
            }
            else
            {
                date = ReservationCompletedTime.Value.ToString("yyyyîNMMåéddì˙ HH:mm:ss");
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
