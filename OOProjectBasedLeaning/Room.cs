// Room.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace OOProjectBasedLeaning
{
    public class Room
    {
        // 部屋番号
        private readonly int number;
        public int RoomNumber => number;
        // 基本料金
        private readonly int price;

        public override string ToString()
        {
            return $"部屋 {RoomNumber}";
        }

        // この部屋に滞在中のゲストリスト
        private readonly List<Guest> guests = new();

        public Room(int number, int price)
        {
            this.number = number;
            this.price = price;
        }

        public int Number => number;
        public virtual int Price => price;

        public override int GetHashCode() => Number;
        public override bool Equals(object obj) => obj is Room r && r.Number == Number;

        // 単一ゲストのチェックイン
        public virtual Room AddGuest(Guest guest)
        {
            guests.Add(guest.AddRoom(this));
            return this;
        }

        // 複数ゲスト（リーダー＋連れ）のまとめてチェックイン
        public virtual Room AddGuests(List<Guest> guests)
        {
            guests.ForEach(g => AddGuest(g));
            return this;
        }

        // 単一ゲストをチェックアウト
        public Room RemoveGuest(Guest guest)
        {
            guests.Remove(guest.RemoveRoom());
            return this;
        }

        // 複数ゲストまとめてチェックアウト
        public Room RemoveGuests(List<Guest> guests)
        {
            guests.ForEach(g => RemoveGuest(g));
            return this;
        }

        public bool HasMember() => guests.Any(g => g.IsMember());  // 会員が滞在中か
        public bool HasVIP() => guests.Any(g => g.IsVIP());        // VIP が滞在中か
        public bool IsEmpty() => !guests.Any();                    // 空室か
    }

    public class RegularRoom : Room
    {
        public RegularRoom(int n, int p) : base(n, p) { }
        public override int Price => HasMember() ? base.Price : base.Price + base.Price / 10;
    }

    public class SuiteRoom : Room
    {
        public SuiteRoom(int number, int price) : base(number, price) { }

        // 会員なら基本料金、非会員なら基本料金 +10%
        public override int Price
            => HasVIP() ? base.Price : base.Price + base.Price / 10;

        // グループチェックイン時、少なくとも一人以上が会員/VIPでないと例外
        public override Room AddGuests(List<Guest> guests)
        {
            bool privileged = HasVIP()
                              || guests.Any(g => g.IsMember() || g.IsVIP());

            if (!privileged)
                throw new InvalidOperationException(
                    "スイートルームには会員またはVIP権限者が必要です。");

            // 問題なければ base に委譲
            return base.AddGuests(guests);
        }
    }

    // 存在しない部屋用の NullObject
    public class NullRoom : Room, NullObject
    {
        private static readonly NullRoom instance = new NullRoom();
        private NullRoom() : base(0, 0) { }
        public static NullRoom Instance => instance;
        public override Room AddGuest(Guest guest) => this;
        public override Room AddGuests(List<Guest> guests) => this;
    }
}
