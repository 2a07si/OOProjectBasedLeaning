using System;
using System.Collections.Generic;
using System.Linq;

namespace OOProjectBasedLeaning
{
    public class Hotel
    {
        private List<Room> vacantRooms;
        private List<Room> guestBook = new List<Room>();

        public Hotel()
        {
            vacantRooms = new List<Room>
            {
                new RegularRoom(501, 15000), new RegularRoom(502, 15000), new RegularRoom(503, 12000),
                new RegularRoom(601, 16000), new RegularRoom(602, 16000), new RegularRoom(603, 15000),
                new RegularRoom(701, 17000), new RegularRoom(702, 17000), new RegularRoom(703, 16000),
                new RegularRoom(801, 18000), new RegularRoom(802, 18000),
                new SuiteRoom(1001, 360000), new SuiteRoom(1002, 300000)
            };
        }

        private Room AcquireRoom()
        {
            Room room = vacantRooms.First();
            vacantRooms.Remove(room);
            return room;
        }

        private void ReleaseRoom(Room room)
        {
            vacantRooms.Add(room);
        }

        public void CheckIn(Guest guest)
        {
            if (!IsVacancies())
                throw new InvalidOperationException("空室がありません。");

            guestBook.Add(AcquireRoom().AddGuest(guest));
        }

        public void CheckIn(List<Guest> guests)
        {
            if (!IsVacancies())
                throw new InvalidOperationException("空室がありません。");

            guestBook.Add(AcquireRoom().AddGuests(guests));
        }

        public void CheckOut(Guest guest)
        {
            Room room = guest.StayAt();
            if (room.RemoveGuest(guest).IsEmpty())
            {
                guestBook.Remove(room);
                ReleaseRoom(room);
            }
        }

        public void CheckOut(List<Guest> guests) => guests.ForEach(guest => CheckOut(guest));

        private bool IsVacancies() => vacantRooms.Count > 0;
    }
}