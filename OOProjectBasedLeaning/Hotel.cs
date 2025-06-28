using System;
using System.Collections.Generic;
using System.Linq;

namespace OOProjectBasedLeaning
{
    /// <summary>
    /// ホテル業務を一元管理するシングルトンクラス
    /// </summary>
    public sealed class Hotel
    {
        // シングルトン
        private static readonly Hotel _instance = new Hotel();
        public static Hotel Instance => _instance;

        private Hotel()
        {
            // 全室の初期化
            allRooms = new List<Room>
            {
                new RegularRoom(501, 15000), new RegularRoom(502, 15000), new RegularRoom(503, 12000),
                new RegularRoom(601, 16000), new RegularRoom(602, 16000), new RegularRoom(603, 15000),
                new RegularRoom(701, 17000), new RegularRoom(702, 17000), new RegularRoom(703, 16000),
                new RegularRoom(801, 18000), new RegularRoom(802, 18000),
                new SuiteRoom(1001, 360000), new SuiteRoom(1002, 300000),
            };
            vacantRooms = new List<Room>(allRooms); // 空き室
            guestBook = new List<Room>(); // チェックインしている部屋
            reservedRooms = new List<Room>(); // 予約された部屋
            reservations = new List<Reservation>(); // 予約された部屋の情報
        }

        // 内部データ
        private readonly List<Room> allRooms;
        private readonly List<Room> vacantRooms; // 空き室
        private readonly List<Room> guestBook; // チェックインしている部屋
        private readonly List<Room> reservedRooms; // 予約された部屋
        private readonly List<Reservation> reservations; // 予約された部屋の情報

        // 外部参照用
        public IReadOnlyList<Room> AllRooms => allRooms;
        public IEnumerable<Reservation> GetAllReservations() => reservations.AsReadOnly();


        /// <summary>
        /// その部屋に「チェックイン済み」のゲストが滞在中かどうか
        /// </summary>
        public bool IsOccupied(Room room)
            => guestBook.Contains(room);

        /// <summary>
        /// その部屋が「予約済み」かどうか
        /// </summary>
        public bool IsReserved(Room room)
            => reservedRooms.Contains(room);

        /// <summary>
        /// 「空室」状態かどうか
        /// </summary>
        public bool IsVacant(Room room)
            => !IsOccupied(room) && !IsReserved(room);


        // イベント通知
        public event Action<Reservation>? ReservationAdded;
        public event EventHandler<HotelEventArgs>? CheckedIn;
        public event EventHandler<HotelEventArgs>? CheckedOut;
        public event EventHandler<HotelErrorEventArgs>? OperationFailed;

        /// <summary>
        /// 部屋番号から Room を取得。存在しなければ NullRoom.Instance。
        /// </summary>
        public Room GetRoomByNumber(int number)
            => allRooms.FirstOrDefault(r => r.Number == number) ?? NullRoom.Instance;

        /// <summary>
        /// 部屋を予約リストに登録
        /// </summary>
        public void Reserve(int roomNumber, Guest guest, DateTime checkIn, DateTime checkOut)
        {
            var room = GetRoomByNumber(roomNumber);
            if (IsOccupied(room))
                throw new InvalidOperationException($"{room.Number}号室は使用中です。");
            if (IsReserved(room))
                throw new InvalidOperationException($"{room.Number}号室はすでに予約済みです。");
            if (!vacantRooms.Remove(room))
                throw new InvalidOperationException($"{room.Number}号室は空室リストにありません。");

            reservedRooms.Add(room);
            var reservation = new Reservation(guest, room, checkIn, checkOut);
            reservations.Add(reservation);
            ReservationAdded?.Invoke(reservation);
        }

        /// <summary>
        /// 予約を取り消し、空室リストに戻す
        /// </summary>
        public void CancelReservation(int roomNumber)
        {
            var room = GetRoomByNumber(roomNumber);
            if (!reservedRooms.Remove(room))
                throw new InvalidOperationException($"{room.Number}号室は予約リストにありません。");

            vacantRooms.Add(room);
            reservations.RemoveAll(r => r.Room.Number == roomNumber);
        }

        /// <summary>
        /// 自身の予約がある場合のみチェックイン
        /// </summary>
        public void CheckIn(int roomNumber, Guest leader)
        {
            var room = GetRoomByNumber(roomNumber);
            if (guestBook.Contains(room))
                throw new InvalidOperationException($"{room.Number}号室は使用中です。");

            // 自分の予約を探す
            var myReservation = reservations.FirstOrDefault(r => r.Guest == leader && r.Room == room);
            if (myReservation == null)
                throw new InvalidOperationException($"{room.Number}号室は予約されていません。");

            try
            {
                // 予約情報のクリア
                reservedRooms.Remove(room);
                reservations.Remove(myReservation);

                // 部屋にゲストを追加
                var group = new List<Guest> { leader };
                group.AddRange(leader.Companions);
                room.AddGuests(group);

                // チェックイン済リストに登録
                guestBook.Add(room);

                // 成功通知
                CheckedIn?.Invoke(this, new HotelEventArgs(leader, room));
            }
            catch (Exception ex)
            {
                // 失敗通知
                OperationFailed?.Invoke(this, new HotelErrorEventArgs(leader, room, ex.Message));
                throw;
            }
        }

        /// <summary>
        /// チェックアウト
        /// </summary>
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

    /// <summary>
    /// チェックイン／チェックアウト成功通知用 EventArgs
    /// </summary>
    public class HotelEventArgs : EventArgs
    {
        public Guest Guest { get; }
        public Room Room { get; }
        public DateTime Timestamp { get; } = DateTime.Now;

        public HotelEventArgs(Guest guest, Room room)
        {
            Guest = guest;
            Room = room;
        }
    }

    /// <summary>
    /// 操作失敗通知用 EventArgs
    /// </summary>
    public class HotelErrorEventArgs : EventArgs
    {
        public Guest Guest { get; }
        public Room Room { get; }
        public string ErrorMessage { get; }

        public HotelErrorEventArgs(Guest guest, Room room, string message)
        {
            Guest = guest;
            Room = room;
            ErrorMessage = message;
        }
    }

    /// <summary>
    /// 予約情報を保持するクラス
    /// </summary>
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
