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
        // シングルトンインスタンス
        private static readonly Hotel _instance = new Hotel();
        public static Hotel Instance => _instance;

        private Hotel()
        {
            // 全室情報の初期化（RegularRoom と SuiteRoom）
            allRooms = new List<Room>
            {
                new RegularRoom(501,15000), new RegularRoom(502,15000), new RegularRoom(503,12000),
                new RegularRoom(601,16000), new RegularRoom(602,16000), new RegularRoom(603,15000),
                new RegularRoom(701,17000), new RegularRoom(702,17000), new RegularRoom(703,16000),
                new RegularRoom(801,18000), new RegularRoom(802,18000),
                new SuiteRoom(1001,360000), new SuiteRoom(1002,300000),
            };
            // 空き室リストには全室をコピー
            vacantRooms = new List<Room>(allRooms);
            // チェックイン済みリスト
            guestBook = new List<Room>();
            // 予約済みリスト
            reservedRooms = new List<Room>();
            // 予約情報リスト
            reservations = new List<Reservation>();
        }

        // 内部データ
        private readonly List<Room> allRooms;         // 全室
        private readonly List<Room> vacantRooms;      // 空室リスト
        private readonly List<Room> guestBook;        // チェックイン済みの部屋
        private readonly List<Room> reservedRooms;    // 予約済みの部屋
        private readonly List<Reservation> reservations; // 予約情報リスト

        // 外部参照用
        public IReadOnlyList<Room> AllRooms => allRooms;
        public IEnumerable<Reservation> GetAllReservations() => reservations.AsReadOnly();

        // 状態判定メソッド
        public bool IsOccupied(Room room) => guestBook.Contains(room);      // 使用中か
        public bool IsReserved(Room room) => reservedRooms.Contains(room);  // 予約済みか
        public bool IsVacant(Room room) => !IsOccupied(room) && !IsReserved(room); // 空室か

        // イベント通知
        public event Action<Reservation>? ReservationAdded;  // 予約追加時
        public event EventHandler<HotelEventArgs>? CheckedIn;         // チェックイン成功時
        public event EventHandler<HotelEventArgs>? CheckedOut;        // チェックアウト成功時
        public event EventHandler<HotelErrorEventArgs>? OperationFailed; // 操作失敗時

        /// <summary>
        /// 部屋番号から Room を取得。存在しなければ NullRoom.Instance。
        /// </summary>
        public Room GetRoomByNumber(int number)
            => allRooms.FirstOrDefault(r => r.Number == number) ?? NullRoom.Instance;

        /// <summary>
        /// 指定号室を予約リストに登録
        /// </summary>
        /// <param name="roomNumber">部屋番号</param>
        /// <param name="guest">予約者</param>
        /// <param name="checkIn">チェックイン日時</param>
        /// <param name="checkOut">チェックアウト日時</param>
        public void Reserve(int roomNumber, Guest guest, DateTime checkIn, DateTime checkOut)
        {
            var room = GetRoomByNumber(roomNumber);
            if (IsOccupied(room))
                throw new InvalidOperationException($"{room.Number}号室は使用中です。");
            if (IsReserved(room))
                throw new InvalidOperationException($"{room.Number}号室は既に予約済みです。");
            if (!vacantRooms.Remove(room))
                throw new InvalidOperationException($"{room.Number}号室は空室リストに存在しません。");

            // 予約リスト登録
            reservedRooms.Add(room);
            var res = new Reservation(guest, room, checkIn, checkOut);
            reservations.Add(res);
            ReservationAdded?.Invoke(res);
        }

        /// <summary>
        /// 予約を取り消し、空室リストに戻す
        /// </summary>
        public void CancelReservation(int roomNumber)
        {
            var room = GetRoomByNumber(roomNumber);
            if (!reservedRooms.Remove(room))
                throw new InvalidOperationException($"{room.Number}号室は予約されていません。");
            vacantRooms.Add(room);
            reservations.RemoveAll(r => r.Room.Number == roomNumber);
        }

        /// <summary>
        /// チェックイン処理
        /// </summary>
        public void CheckIn(int roomNumber, Guest leader)
        {
            var room = GetRoomByNumber(roomNumber);
            try
            {
                // すでに使用中か
                if (guestBook.Contains(room))
                    throw new InvalidOperationException($"{room.Number}号室は使用中です。");

                // 自分の予約を探す
                var myReservation = reservations
                    .FirstOrDefault(r => r.Guest == leader && r.Room == room);
                if (myReservation == null)
                    throw new InvalidOperationException(
                        $"{leader.Name} さんは {room.Number}号室は予約されていません。");

                // 予約情報をクリア
                reservedRooms.Remove(room);
                reservations.Remove(myReservation);

                // 部屋にゲストを追加
                var group = new List<Guest> { leader };
                group.AddRange(leader.Companions);
                room.AddGuests(group);

                // チェックイン済みに登録
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
        /// チェックアウト処理
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

    /// <summary>チェックイン/アウト成功時の情報を保持</summary>
    public class HotelEventArgs : EventArgs
    {
        public Guest Guest { get; }
        public Room Room { get; }
        public DateTime Timestamp { get; } = DateTime.Now;
        public HotelEventArgs(Guest g, Room r) { Guest = g; Room = r; }
    }

    /// <summary>操作失敗時の情報を保持</summary>
    public class HotelErrorEventArgs : EventArgs
    {
        public Guest Guest { get; }
        public Room Room { get; }
        public string ErrorMessage { get; }
        public HotelErrorEventArgs(Guest g, Room r, string msg)
        { Guest = g; Room = r; ErrorMessage = msg; }
    }

    /// <summary>予約情報を保持</summary>
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
