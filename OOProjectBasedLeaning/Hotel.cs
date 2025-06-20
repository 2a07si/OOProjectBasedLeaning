// Hotel.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace OOProjectBasedLeaning
{
    public class Hotel
    {
        private readonly List<Room> allRooms;      // 全室
        private readonly List<Room> vacantRooms;   // 空室リスト
        private readonly List<Room> guestBook;     // チェックイン済リスト
        // TODO
        private readonly List<Room> reservedRooms; // 予約済リスト(未実装) 

        public Hotel()
        {
            // 全室を生成
            allRooms = new List<Room>
            {
                new RegularRoom(501, 15000), new RegularRoom(502, 15000), new RegularRoom(503, 12000),
                new RegularRoom(601, 16000), new RegularRoom(602, 16000), new RegularRoom(603, 15000),
                new RegularRoom(701, 17000), new RegularRoom(702, 17000), new RegularRoom(703, 16000),
                new RegularRoom(801, 18000), new RegularRoom(802, 18000),
                new SuiteRoom  (1001, 360000), new SuiteRoom  (1002, 300000),
            };

            // 各リスト初期化
            vacantRooms = new List<Room>(allRooms);
            guestBook = new List<Room>();
            reservedRooms = new List<Room>();
        }

        public IReadOnlyList<Room> AllRooms => allRooms;

        /// <summary>
        /// 番号から Room を取得。なければ NullRoom.Instance。
        /// </summary>
        public Room GetRoomByNumber(int number)
            => allRooms.FirstOrDefault(r => r.Number == number) ?? NullRoom.Instance;

        public bool IsOccupied(Room room) => guestBook.Contains(room);
        public bool IsReserved(Room room) => reservedRooms.Contains(room);
        public bool IsVacant(Room room) => !IsOccupied(room) && !IsReserved(room);

        // 代表＋お連れ様全員をまとめてチェックイン
        public void CheckIn(int roomNumber, Guest leader)
        {
            // 対象部屋取得
            var room = GetRoomByNumber(roomNumber);

            // 使用中 or 予約済 チェック
            if (guestBook.Contains(room))
                throw new InvalidOperationException($"{room.Number}号室は使用中です。");
            if (reservedRooms.Contains(room))
                throw new InvalidOperationException($"{room.Number}号室は予約済です。");

            // 空室リストから一旦外す
            if (!vacantRooms.Remove(room))
                throw new InvalidOperationException($"{room.Number}号室は空室リストにありません。");

            try
            {
                // 代表＋コンパニオン全員まとめて AddGuests
                var group = new List<Guest> { leader };
                group.AddRange(leader.Companions);

                room.AddGuests(group);  // SuiteRoom の権限チェック
                guestBook.Add(room);   // 成功時に guestBook へ
            }
            catch
            {
                // 例外時に空室リストに戻す
                vacantRooms.Add(room);
                throw;
            }
        }

        // 代表を渡せば、その代表＋お連れ様全員をまとめてチェックアウト
        public void CheckOut(Guest leader)
        {
            // 代表が泊まっている部屋
            var room = leader.StayAt();

            // 代表＋コンパニオン全員をまとめて RemoveGuests
            var group = new List<Guest> { leader };
            group.AddRange(leader.Companions);

            room.RemoveGuests(group);

            // guestBook から除去＆空室リストに戻す
            if (guestBook.Remove(room))
                vacantRooms.Add(room);
        }

        // 現状使用予定なし
        public void CheckIn(List<Guest> guests) { /* … */ }
        public void CheckOut(List<Guest> guests) { /* … */ }
    }
}
