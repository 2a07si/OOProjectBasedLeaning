using OOProjectBasedLeaning;

public class Room
{
    private readonly int number; // 部屋番号
    public int RoomNumber => number;
    private readonly int price;  // 基本料金

    private readonly List<Guest> guests = new(); // 滞在中のゲスト

    private Guest? reservedBy; // 予約者
    private DateTimeRange? reservationPeriod; // 予約期間

    public Room(int number, int price)
    {
        this.number = number;
        this.price = price;
    }

    public int Number => number;
    public virtual int Price => price;

    public override int GetHashCode() => Number;
    public override bool Equals(object obj) => obj is Room r && r.Number == Number;

    // 単体ゲスト追加
    public virtual Room AddGuest(Guest guest)
    {
        guests.Add(guest.AddRoom(this));
        return this;
    }

    // 複数ゲスト追加
    public virtual Room AddGuests(List<Guest> guests)
    {
        guests.ForEach(g => AddGuest(g));
        return this;
    }

    // 単体ゲスト削除
    public virtual Room RemoveGuest(Guest guest)
    {
        if (guests.Remove(guest))
        {
            guest.RemoveRoom();
        }
        return this;
    }

    // 複数ゲスト削除
    public virtual Room RemoveGuests(IEnumerable<Guest> companions)
    {
        foreach (var g in companions)
        {
            RemoveGuest(g);
        }
        return this;
    }

    // 滞在中に会員がいるか
    public bool HasMember() => guests.Any(g => g.IsMember());
    // 滞在中にVIPがいるか
    public bool HasVIP() => guests.Any(g => g.IsVIP());
    // 滞在者なし
    public bool IsEmpty() => !guests.Any();
    // 空室かつ予約なし
    public bool IsAvailable() => IsEmpty() && !IsReserved();

    // 予約中かどうか
    public bool IsReserved() => reservedBy != null && reservationPeriod != null;
    // 予約開始日時
    public DateTime ReservedCheckIn() => this.reservationPeriod?.Start
        ?? throw new InvalidOperationException("予約日が未設定です");
    // 予約終了日時
    public DateTime ReservedCheckOut() => this.reservationPeriod?.End
        ?? throw new InvalidOperationException("退室日が未設定です");

    // 予約者取得
    public Guest ReservedBy => reservedBy ?? throw new InvalidOperationException("予約者が存在しません。");
    // 予約期間取得
    public DateTimeRange ReservedPeriod => reservationPeriod ?? throw new InvalidOperationException("予約期間が存在しません。");

    // 予約処理
    public virtual void Reserve(Guest leader, IEnumerable<Guest> companions, DateTime checkIn, DateTime checkOut)
    {
        if (IsReserved())
            throw new InvalidOperationException($"{Number}号室は既に予約済みです。");

        if (!IsEmpty())
            throw new InvalidOperationException($"{Number}号室は使用中です。");

        reservedBy = leader;
        reservationPeriod = new DateTimeRange(checkIn, checkOut);
    }

    // 予約解除
    public virtual void CancelReservation()
    {
        if (!IsReserved())
            throw new InvalidOperationException($"{Number}号室は予約されていません。");

        reservedBy = null;
        reservationPeriod = null;
    }

    public override string ToString()
    {
        return $"部屋 {RoomNumber}";
    }
}

// 通常部屋（会員でない場合は+10%）
public class RegularRoom : Room
{
    public RegularRoom(int n, int p) : base(n, p) { }
    public override int Price => HasMember() ? base.Price : base.Price + base.Price / 10;
}

// スイートルーム（会員/VIPの同行が必要）
public class SuiteRoom : Room
{
    public SuiteRoom(int number, int price) : base(number, price) { }

    public override int Price => HasVIP() ? base.Price : base.Price + base.Price / 10;

    public override void Reserve(Guest leader, IEnumerable<Guest> companions, DateTime checkIn, DateTime checkOut)
    {
        if (IsReserved())
            throw new InvalidOperationException($"{Number}号室は既に予約済みです。");

        if (!IsEmpty())
            throw new InvalidOperationException($"{Number}号室は使用中です。");

        bool hasPrivilege = leader.IsMember() || leader.IsVIP() || companions.Any(g => g.IsMember() || g.IsVIP());

        if (!hasPrivilege)
            throw new InvalidOperationException("スイートルームは、連れの中に 1 人以上の会員またはVIPが必要です。");

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

// 未定義部屋
public class NullRoom : Room, NullObject
{
    private static readonly NullRoom instance = new NullRoom();
    private NullRoom() : base(0, 0) { }
    public static NullRoom Instance => instance;
    public override Room AddGuest(Guest guest) => this;
    public override Room AddGuests(List<Guest> guests) => this;
}

// 日付範囲クラス
public class DateTimeRange
{
    public DateTime Start { get; }
    public DateTime End { get; }

    public DateTimeRange(DateTime start, DateTime end)
    {
        if (start > end)
            throw new ArgumentException("開始日は終了日より後にできません。");

        Start = start;
        End = end;
    }
}
