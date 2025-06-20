using System;
using System.Collections.Generic;

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
        private readonly List<Guest> companions = new();
        private Room room = NullRoom.Instance;

        public IReadOnlyList<Guest> Companions => companions;

        public Guest AddCompanion(Guest g)
        {
            if (companions.Count < 3) companions.Add(g);
            return this;
        }

        public Guest RemoveCompanion(Guest g)
        {
            companions.Remove(g);
            return this;
        }

        public AbstractGuest() { }
        public AbstractGuest(string name) { Name = name; }

        public Guest AddRoom(Room r) { room = r; return this; }
        public Guest RemoveRoom() { room = NullRoom.Instance; return this; }
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
        private readonly int id;
        private readonly bool isVip;

        public MemberModel(int id, string name, bool isVip = false)
          : base(name)
        {
            this.id = id;
            this.isVip = isVip;
        }

        public MemberModel() : this(Member.NEW, string.Empty, false) { }
        public MemberModel(int id) : this(id, string.Empty, false) { }
        public MemberModel(string name) : this(Member.NEW, name, false) { }
        public MemberModel(string name, bool isVip) : this(Member.NEW, name, isVip) { }

        public int Id => id;
        public bool IsNew() => id == Member.NEW;
        public override bool IsMember() => true;
        public override bool IsVIP() => isVip;

        public override int GetHashCode() => Id;
        public override bool Equals(object obj)
          => obj is Member m && m.Id == Id;
    }

    public class NullGuest : AbstractGuest, NullObject
    {
        private static readonly Guest instance = new　NullGuest();
        private NullGuest() : base(string.Empty) { }
        public static Guest Instance => instance;
        public override bool IsMember() => false;
        public override bool IsVIP() => false;
    }
}
