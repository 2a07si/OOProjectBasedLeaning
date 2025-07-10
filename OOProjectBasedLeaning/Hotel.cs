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

        private readonly List<Room> allRooms;           // 全ての部屋
        private readonly List<Room> vacantRooms;        // 空室リスト
        private readonly List<Room> guestBook;          // チェックイン済み部屋リスト

        public IReadOnlyList<Room> AllRooms => allRooms;

        // チェックイン済み判定
        public bool IsOccupied(Room room) => guestBook.Contains(room);

        // 空室判定
        public bool IsVacant(Room room) => room.IsAvailable();

        // 各種イベント定義
        public event Action<Reservation>? ReservationAdded;
        public event EventHandler<HotelEventArgs>? CheckedIn;
        public event EventHandler<HotelEventArgs>? CheckedOut;
        public event EventHandler<HotelErrorEventArgs>? OperationFailed;

        // 部屋番号からRoomを取得
        public Room GetRoomByNumber(int number)
            => allRooms.FirstOrDefault(r => r.Number == number) ?? NullRoom.Instance;

        // 予約処理
        public void Reserve(int roomNumber, Guest guest, DateTime checkIn, DateTime checkOut)
        {
            var room = GetRoomByNumber(roomNumber);

            if (!vacantRooms.Remove(room))
                throw new InvalidOperationException($"{room.Number}号室は空室リストに存在しません。");

            room.Reserve(guest, guest.Companions, checkIn, checkOut);

            ReservationAdded?.Invoke(new Reservation(guest, room, checkIn, checkOut));
        }

        // 予約キャンセル処理
        public void CancelReservation(int roomNumber)
        {
            var room = GetRoomByNumber(roomNumber);
            room.CancelReservation();

            if (!vacantRooms.Contains(room))
                vacantRooms.Add(room);
        }

        // チェックイン処理
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

        // チェックアウト処理
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

    // チェックイン・アウト通知
    public class HotelEventArgs : EventArgs
    {
        public Guest Guest { get; }
        public Room Room { get; }
        public DateTime Timestamp { get; } = DateTime.Now;
        public HotelEventArgs(Guest g, Room r) { Guest = g; Room = r; }
    }

    // 操作失敗通知
    public class HotelErrorEventArgs : EventArgs
    {
        public Guest Guest { get; }
        public Room Room { get; }
        public string ErrorMessage { get; }
        public HotelErrorEventArgs(Guest g, Room r, string msg)
        { Guest = g; Room = r; ErrorMessage = msg; }
    }

    // 予約情報モデル
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
