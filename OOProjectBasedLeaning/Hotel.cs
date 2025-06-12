using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                // 5F
                new RegularRoom(501, 15000), new RegularRoom(502, 15000), new RegularRoom(503, 12000),
                // 6F
                new RegularRoom(601, 16000), new RegularRoom(602, 16000), new RegularRoom(603, 15000),
                // 7F
                new RegularRoom(701, 17000), new RegularRoom(702, 17000), new RegularRoom(703, 16000),
                // 8F
                new RegularRoom(801, 18000), new RegularRoom(802, 18000),
                // 10F
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
            {

                // TODO
                throw new NotImplementedException();

            }

            guestBook.Add(AcquireRoom().AddGuest(guest));

        }

        public void CheckIn(List<Guest> guests)
        {

            if (!IsVacancies())
            {

                // TODO
                throw new NotImplementedException();

            }

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

        public void CheckOut(List<Guest> guests)
        {

            guests.ForEach(guest => CheckOut(guest));

        }

        private bool IsVacancies()
        {

            return vacantRooms.Count > 0;

        }

    }

}
