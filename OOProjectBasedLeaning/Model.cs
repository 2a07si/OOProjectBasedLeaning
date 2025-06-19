using System.Collections.Generic;

namespace OOProjectBasedLeaning
{
    public interface Model
    {
        string Name { get; set; }
    }

    public interface NotifierModel : Model
    {
        void AddObserver(Observer observer);
        void RemoveObserver(Observer observer);
    }

    public class ModelEntity : Model
    {
        private string name = string.Empty;
        public virtual string Name { get => name; set => name = value; }
    }

    public class NotifierModelEntity : ModelEntity, NotifierModel
    {
        private List<Observer> observers = new();

        public override string Name
        {
            set { base.Name = value; Notify(); }
        }

        public void AddObserver(Observer observer)
        {
            if (!observers.Contains(observer)) observers.Add(observer);
        }

        public void RemoveObserver(Observer observer)
        {
            if (observers.Contains(observer)) observers.Remove(observer);
        }

        protected void Notify()
        {
            observers.ForEach(o => o.Update(this));
        }
    }

    public class NullModel : ModelEntity, NullObject
    {
        private static readonly Model instance = new NullModel();
        public static Model Instance => instance;
        public override string Name { get => string.Empty; set { } }
    }

    public class NullNotifierModel : NotifierModelEntity, NullObject
    {
        private static readonly NotifierModel instance = new NullNotifierModel();
        public static NotifierModel Instance => instance;
        public override string Name { get => string.Empty; set { } }
    }
}
