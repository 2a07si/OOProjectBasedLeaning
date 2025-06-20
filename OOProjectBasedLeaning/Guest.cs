using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OOProjectBasedLeaning
{
    public interface Guest : Model
    {
        IReadOnlyList<Guest> Companions { get; }
        Guest AddCompanion(Guest guest);
        Guest RemoveCompanion(Guest guest);

        Guest AddRoom(Room room);
        Guest RemoveRoom();
        Room StayAt();

        bool IsMember();
        bool IsVIP();
    }

    public interface Member : Guest
    {
        const int NEW = 0;
        int Id { get; }
        bool IsNew();
    }

    public abstract class AbstractGuest : ModelEntity, Guest
    {
        private List<Guest> companions = new();
        public IReadOnlyList<Guest> Companions => companions;

        public Guest AddCompanion(Guest guest)
        {
            if (companions.Count < 3)
                companions.Add(guest);
            return this;
        }

        public Guest RemoveCompanion(Guest guest)
        {
            companions.Remove(guest);
            return this;
        }

        private Room room = NullRoom.Instance;
        public AbstractGuest() { }
        public AbstractGuest(string name) { Name = name; }

        public Guest AddRoom(Room room)
        {
            this.room = room;
            return this;
        }
        public Guest RemoveRoom()
        {
            room = NullRoom.Instance;
            return this;
        }
        public Room StayAt() => room;

        public abstract bool IsMember();
        public abstract bool IsVIP();
    }

    public class GuestModel : AbstractGuest
    {
        public GuestModel() { }
        public GuestModel(string name) : base(name) { }

        public override bool IsMember() => false;
        public override bool IsVIP() => false;
    }

    public class MemberModel : AbstractGuest, Member
    {
        private int id;
        private bool isVip;

        // 汎用コンストラクタ
        public MemberModel(int id, string name, bool isVip = false)
            : base(name)
        {
            this.id = id;
            this.isVip = isVip;
        }

        // 引数なし → NEW 会員（非VIP）
        public MemberModel()
            : this(Member.NEW, string.Empty, false)
        { }

        // ID 指定のみ → 指定 ID の会員（非VIP）
        public MemberModel(int id)
            : this(id, string.Empty, false)
        { }

        // 名前指定のみ → 名前付き会員（非VIP）
        public MemberModel(string name)
            : this(Member.NEW, name, false)
        { }

        // 名前＋VIP フラグ指定
        public MemberModel(string name, bool isVip)
            : this(Member.NEW, name, isVip)
        { }

        // Member インターフェイス
        public int Id => id;

        public bool IsNew() => id == Member.NEW;

        public override bool IsMember() => true;
        public override bool IsVIP() => isVip;

        public override int GetHashCode() => Id;
        public override bool Equals(object? obj)
            => obj is Member m && m.Id == Id;
    }


    public class NullGuest : AbstractGuest, NullObject
    {
        private static readonly Guest instance = new NullGuest();
        private NullGuest() : base(string.Empty) { }
        public static Guest Instance => instance;

        public override bool IsMember() => false;
        public override bool IsVIP() => false;
    }
}
