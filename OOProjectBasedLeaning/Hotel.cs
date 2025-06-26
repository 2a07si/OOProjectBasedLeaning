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
        // 唯一のインスタンスを保持
        private static readonly Hotel _instance = new Hotel();

        /// <summary>
        /// 外部からアクセスするためのプロパティ
        /// </summary>
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
                new SuiteRoom  (1001, 360000), new SuiteRoom  (1002, 300000),
            };
            // 空室リストは全室初期化時のコピー
            vacantRooms = new List<Room>(allRooms);
            guestBook = new List<Room>();
            reservedRooms = new List<Room>();
        }

        // フィールド定義
        private readonly List<Room> allRooms;      // 全室（不変）
        private readonly List<Room> vacantRooms;   // 現在空きのある部屋
        private readonly List<Room> guestBook;     // チェックイン済みの部屋
        private readonly List<Room> reservedRooms; // 予約済みの部屋

        // 全室一覧を外部参照用に公開
        public IReadOnlyList<Room> AllRooms => allRooms;

        /// <summary>
        /// 指定した号室を返す。存在しない場合は NullRoom.Instance。
        /// </summary>
        public Room GetRoomByNumber(int number)
            => allRooms.FirstOrDefault(r => r.Number == number) ?? NullRoom.Instance;

        /// <summary>その部屋が今「使用中」か</summary>
        public bool IsOccupied(Room room) => guestBook.Contains(room);
        /// <summary>その部屋が今「予約済み」か</summary>
        public bool IsReserved(Room room) => reservedRooms.Contains(room);
        /// <summary>その部屋が「空室」（使用中でも予約済みでもない）か</summary>
        public bool IsVacant(Room room) => !IsOccupied(room) && !IsReserved(room);

        //  予約関連メソッド
        /// <summary>
        /// 指定号室を予約リストに登録します。
        /// 使用中・二重予約は例外。
        /// </summary>
        public void Reserve(int roomNumber)
        {
            var room = GetRoomByNumber(roomNumber);
            if (IsOccupied(room))
                throw new InvalidOperationException($"{room.Number}号室は使用中です。");
            if (IsReserved(room))
                throw new InvalidOperationException($"{room.Number}号室はすでに予約済みです。");
            if (!vacantRooms.Remove(room))
                throw new InvalidOperationException($"{room.Number}号室は空室リストにありません。");
            reservedRooms.Add(room);
        }

        /// <summary>
        /// 指定号室の予約を取り消し、空室リストに戻します。
        /// </summary>
        public void CancelReservation(int roomNumber)
        {
            var room = GetRoomByNumber(roomNumber);
            if (!reservedRooms.Remove(room))
                throw new InvalidOperationException($"{room.Number}号室は予約リストにありません。");
            vacantRooms.Add(room);
        }

        //  チェックイン／チェックアウト
        /// <summary>
        /// 代表＋お連れ様全員をまとめてチェックインします。
        /// 予約済／使用中は例外。AddGuests で例外が出たら必ず空室リストに戻します。
        /// </summary>
        public void CheckIn(int roomNumber, Guest leader)
        {
            var room = GetRoomByNumber(roomNumber);

            if (guestBook.Contains(room))
                throw new InvalidOperationException($"{room.Number}号室は使用中です。");
            if (reservedRooms.Contains(room))
                throw new InvalidOperationException($"{room.Number}号室は予約済みです。");
            if (!vacantRooms.Remove(room))
                throw new InvalidOperationException($"{room.Number}号室は空室リストにありません。");

            try
            {
                var group = new List<Guest> { leader };
                group.AddRange(leader.Companions);
                room.AddGuests(group);    
                guestBook.Add(room);
            }
            catch
            {
                // 例外なら空きリストに戻す
                vacantRooms.Add(room);
                throw;
            }
        }

        /// <summary>
        /// 代表＋お連れ様全員をまとめてチェックアウトします。
        /// 部屋が空になれば guestBook から除去し空室へ戻す。
        /// </summary>
        public void CheckOut(Guest leader)
        {
            var room = leader.StayAt();
            var group = new List<Guest> { leader };
            group.AddRange(leader.Companions);
            room.RemoveGuests(group);

            if (guestBook.Remove(room))
                vacantRooms.Add(room);
        }

    }
}
