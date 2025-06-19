using System;
using System.Collections.Generic;

namespace OOProjectBasedLeaning
{
    public class Room
    {
        private int number;
        private int price;
        private List<Guest> guests = new List<Guest>();

        public Room(int number, int price)
        {
            this.number = number;
            this.price = price;
        }

        public override int GetHashCode()
        {
            return Number;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Room)
                return Number == (obj as Room)?.Number;

            return false;
        }

        public int Number => number;

        public virtual int Price => price;

        public virtual Room AddGuest(Guest guest)
        {
            guests.Add(guest.AddRoom(this));
            return this;
        }

        public virtual Room AddGuests(List<Guest> guests)
        {
            guests.ForEach(g => AddGuest(g));
            return this;
        }

        public Room RemoveGuest(Guest guest)
        {
            guests.Remove(guest.RemoveRoom());
            return this;
        }

        public Room RemoveGuests(List<Guest> guests)
        {
            guests.ForEach(g => RemoveGuest(g));
            return this;
        }

        public bool HasMember()
        {
            foreach (var guest in guests)
            {
                if (guest.IsMember())
                    return true;
            }
            return false;
        }

        public bool HasVIP()
        {
            foreach (var guest in guests)
            {
                if (guest.IsVIP())
                    return true;
            }
            return false;
        }

        public bool IsEmpty()
        {
            return guests.Count == 0;
        }
    }

    public class RegularRoom : Room
    {
        public RegularRoom(int number, int price) : base(number, price) { }

        public override int Price
        {
            get
            {
                if (HasMember())
                    return base.Price;
                return base.Price + base.Price / 10;
            }
        }
    }

    public class SuiteRoom : Room
    {
        public SuiteRoom(int number, int price) : base(number, price) { }

        public override int Price
        {
            get
            {
                if (HasVIP())
                    return base.Price;
                return base.Price + base.Price / 10;
            }
        }

        public override Room AddGuest(Guest guest)
        {
            if (!HasMember())
                throw new NotImplementedException("スイートルームには会員が必要です。");

            return base.AddGuest(guest);
        }

        public override Room AddGuests(List<Guest> guests)
        {
            if (!HasMember())
                throw new NotImplementedException("スイートルームには会員が必要です。");

            return base.AddGuests(guests);
        }
    }

    public class NullRoom : Room, NullObject
    {
        private static readonly Room instance = new NullRoom();

        private NullRoom() : base(0, 0) { }

        public static Room Instance => instance;

        public override Room AddGuest(Guest guest) => this;
        public override Room AddGuests(List<Guest> guests) => this;
    }
}
