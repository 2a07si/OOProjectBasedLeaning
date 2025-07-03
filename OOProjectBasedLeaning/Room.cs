using OOProjectBasedLeaning;

public class Room
{
    private readonly int number;
    public int RoomNumber => number;
    private readonly int price;

    private readonly List<Guest> guests = new();

    public bool IsReserved { get; private set; } = false;
    public Guest? ReservedBy { get; private set; }
    public DateTime? ReservedCheckIn { get; private set; }
    public DateTime? ReservedCheckOut { get; private set; }

    public Room(int number, int price)
    {
        this.number = number;
        this.price = price;
    }

    public int Number => number;
    public virtual int Price => price;

    public override int GetHashCode() => Number;
    public override bool Equals(object obj) => obj is Room r && r.Number == Number;

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

    public virtual Room RemoveGuest(Guest guest)
    {
        if (guests.Remove(guest))
        {
            guest.RemoveRoom();
        }
        return this;
    }

    public virtual Room RemoveGuests(IEnumerable<Guest> companions)
    {
        foreach (var g in companions)
        {
            RemoveGuest(g);
        }
        return this;
    }

    public bool HasMember() => guests.Any(g => g.IsMember());
    public bool HasVIP() => guests.Any(g => g.IsVIP());
    public bool IsEmpty() => !guests.Any();
    public bool IsAvailable() => IsEmpty() && !IsReserved;

    public virtual void Reserve(Guest leader, IEnumerable<Guest> companions, DateTime checkIn, DateTime checkOut)
    {
        if (IsReserved)
            throw new InvalidOperationException($"{Number}号室は既に予約済みです。");

        if (!IsEmpty())
            throw new InvalidOperationException($"{Number}号室は使用中です。");

        IsReserved = true;
        ReservedBy = leader;
        ReservedCheckIn = checkIn;
        ReservedCheckOut = checkOut;
    }

    public virtual void CancelReservation()
    {
        if (!IsReserved)
            throw new InvalidOperationException($"{Number}号室は予約されていません。");

        IsReserved = false;
        ReservedBy = null;
        ReservedCheckIn = null;
        ReservedCheckOut = null;
    }

    public override string ToString()
    {
        return $"部屋 {RoomNumber}";
    }
}

public class RegularRoom : Room
{
    public RegularRoom(int n, int p) : base(n, p) { }
    public override int Price => HasMember() ? base.Price : base.Price + base.Price / 10;
}

public class SuiteRoom : Room
{
    public SuiteRoom(int number, int price) : base(number, price) { }

    public override int Price => HasVIP() ? base.Price : base.Price + base.Price / 10;

    public override void Reserve(Guest leader, IEnumerable<Guest> companions, DateTime checkIn, DateTime checkOut)
    {
        if (IsReserved)
            throw new InvalidOperationException($"{Number}号室は既に予約済みです。");

        if (!IsEmpty())
            throw new InvalidOperationException($"{Number}号室は使用中です。");

        bool hasPrivilege = leader.IsMember() || leader.IsVIP() || companions.Any(g => g.IsMember() || g.IsVIP());

        if (!hasPrivilege)
            throw new InvalidOperationException($"スイートルームは、連れの中に 1 人以上の会員またはVIPが必要です。");

        base.Reserve(leader, companions, checkIn, checkOut);
    }


    public override Room AddGuests(List<Guest> guests)
    {
        bool privileged = HasVIP() || guests.Any(g => g.IsMember() || g.IsVIP());

        if (!privileged)
            throw new InvalidOperationException("スイートルームには会員またはVIP権限者が必要です。");

        return base.AddGuests(guests);
    }
}

public class NullRoom : Room, NullObject
{
    private static readonly NullRoom instance = new NullRoom();
    private NullRoom() : base(0, 0) { }
    public static NullRoom Instance => instance;
    public override Room AddGuest(Guest guest) => this;
    public override Room AddGuests(List<Guest> guests) => this;
}

