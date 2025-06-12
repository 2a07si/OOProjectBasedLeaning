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

        private Room room = NullRoom.Instance;

        public AbstractGuest()
        {

        }

        public AbstractGuest(string name)
        {

            Name = name;

        }

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

        public Room StayAt()
        {

            return room;

        }

        public abstract bool IsMember();

        public abstract bool IsVIP();

    }

    public class GuestModel : AbstractGuest
    {

        public GuestModel()
        {

        }

        public GuestModel(string name) : base(name)
        {

        }

        public override bool IsMember()
        {

            return false;

        }

        public override bool IsVIP()
        {

            return false;

        }

    }

    public class MemberModel : AbstractGuest, Member
    {

        private int id;

        public MemberModel() : this(Member.NEW)
        {

        }

        public MemberModel(int id) : this(id, string.Empty)
        {

        }

        public MemberModel(string name) : this(Member.NEW, name)
        {

        }

        public MemberModel(int id, string name) : base(name)
        {

            this.id = id;

        }

        public override int GetHashCode()
        {

            return Id;

        }

        public override bool Equals(object? obj)
        {

            if (obj is Member)
            {

                return Id == (obj as Member)?.Id;

            }

            return false;

        }

        public int Id { get { return id; } }

        public bool IsNew()
        {

            return id is Member.NEW;

        }

        public override bool IsMember()
        {

            return true;

        }

        public override bool IsVIP()
        {

            // TODO: Implement VIP logic for members

            // Assuming members are not VIPs by default
            return false;

        }

    }

    public class NullGuest : AbstractGuest, NullObject
    {

        private static Guest instance = new NullGuest();

        private NullGuest() : base(string.Empty)
        {

        }

        public static Guest Instance { get { return instance; } }

        public override bool IsMember()
        {

            return false;

        }

        public override bool IsVIP()
        {

            return false;

        }

    }

}
