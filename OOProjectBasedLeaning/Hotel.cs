using System;
using System.Collections.Generic;
using System.Linq;

namespace OOProjectBasedLeaning
{
    public sealed class Hotel
    {
        private static readonly Hotel _instance = new Hotel();
        public static Hotel Instance => _instance;

        private Hotel()
        {
            allRooms = new List<Room>
            {
                new RegularRoom(501,15000), new RegularRoom(502,15000), new RegularRoom(503,12000),
                new RegularRoom(601,16000), new RegularRoom(602,16000), new RegularRoom(603,15000),
                new RegularRoom(701,17000), new RegularRoom(702,17000), new RegularRoom(703,16000),
                new RegularRoom(801,18000), new RegularRoom(802,18000),
                new SuiteRoom(1001,360000), new SuiteRoom(1002,300000),
            };
            vacantRooms = new List<Room>(allRooms);
            guestBook = new List<Room>();
        }

        private readonly List<Room> allRooms;
        private readonly List<Room> vacantRooms;
        private readonly List<Room> guestBook;

        public IReadOnlyList<Room> AllRooms => allRooms;

        public bool IsOccupied(Room room) => guestBook.Contains(room);

        public bool IsVacant(Room room) => room.IsAvailable();

        public event Action<Reservation>? ReservationAdded;
        public event EventHandler<HotelEventArgs>? CheckedIn;
        public event EventHandler<HotelEventArgs>? CheckedOut;
        public event EventHandler<HotelErrorEventArgs>? OperationFailed;

        public Room GetRoomByNumber(int number)
            => allRooms.FirstOrDefault(r => r.Number == number) ?? NullRoom.Instance;
        public void Reserve(int roomNumber, Guest guest, DateTime checkIn, DateTime checkOut)
        {
            var room = GetRoomByNumber(roomNumber);

            if (!vacantRooms.Remove(room))
                throw new InvalidOperationException($"{room.Number}号室は空室リストに存在しません。");

            // 連れも渡すように修正
            room.Reserve(guest, guest.Companions, checkIn, checkOut);

            ReservationAdded?.Invoke(new Reservation(guest, room, checkIn, checkOut));
        }


        public void CancelReservation(int roomNumber)
        {
            var room = GetRoomByNumber(roomNumber);
            room.CancelReservation();

            if (!vacantRooms.Contains(room))
                vacantRooms.Add(room);
        }

        public void CheckIn(int roomNumber, Guest leader)
        {
            var room = GetRoomByNumber(roomNumber);
            try
            {
                if (guestBook.Contains(room))
                    throw new InvalidOperationException($"{room.Number}号室は使用中です。");

                if (room.ReservedBy != leader)
                    throw new InvalidOperationException($"{leader.Name} さんは {room.Number}号室を予約していません。");

                room.CancelReservation();

                var group = new List<Guest> { leader };
                group.AddRange(leader.Companions);
                room.AddGuests(group);

                guestBook.Add(room);
                CheckedIn?.Invoke(this, new HotelEventArgs(leader, room));
            }
            catch (Exception ex)
            {
                OperationFailed?.Invoke(this, new HotelErrorEventArgs(leader, room, ex.Message));
                throw;
            }
        }

        public void CheckOut(Guest leader)
        {
            var room = leader.StayAt();
            var group = new List<Guest> { leader };
            group.AddRange(leader.Companions);
            room.RemoveGuests(group);

            try
            {
                if (guestBook.Remove(room))
                    vacantRooms.Add(room);
                CheckedOut?.Invoke(this, new HotelEventArgs(leader, room));
            }
            catch (Exception ex)
            {
                OperationFailed?.Invoke(this, new HotelErrorEventArgs(leader, room, ex.Message));
                throw;
            }
        }
    }

    public class HotelEventArgs : EventArgs
    {
        public Guest Guest { get; }
        public Room Room { get; }
        public DateTime Timestamp { get; } = DateTime.Now;
        public HotelEventArgs(Guest g, Room r) { Guest = g; Room = r; }
    }

    public class HotelErrorEventArgs : EventArgs
    {
        public Guest Guest { get; }
        public Room Room { get; }
        public string ErrorMessage { get; }
        public HotelErrorEventArgs(Guest g, Room r, string msg)
        { Guest = g; Room = r; ErrorMessage = msg; }
    }

    public class Reservation
    {
        public Guest Guest { get; }
        public Room Room { get; }
        public DateTime CheckIn { get; }
        public DateTime CheckOut { get; }

        public Reservation(Guest guest, Room room, DateTime checkIn, DateTime checkOut)
        {
            Guest = guest;
            Room = room;
            CheckIn = checkIn;
            CheckOut = checkOut;
        }
    }
}