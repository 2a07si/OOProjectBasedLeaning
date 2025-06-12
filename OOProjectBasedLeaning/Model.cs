using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        public ModelEntity()
        {

        }

        public virtual string Name
        {
            
            get { return name; }
            
            set { name = value; }
        
        }

    }

    public class NotifierModelEntity : ModelEntity, NotifierModel
    {

        private List<Observer> observers = new List<Observer>();

        public NotifierModelEntity()
        {

        }

        public override string Name
        {

            set
            {

                base.Name = value;

                Notify();

            }

        }

        public void AddObserver(Observer observer)
        {

            if (!observers.Contains(observer))
            {
                
                observers.Add(observer);
                
            }

        }

        public void RemoveObserver(Observer observer)
        {

            if (observers.Contains(observer))
            {
                
                observers.Remove(observer);
                
            }

        }

        protected void Notify()
        {

            observers.ForEach(observer => observer.Update(this));

        }

    }

    public class NullModel : ModelEntity, NullObject
    {

        private static readonly Model instance = new NullModel();

        private NullModel()
        {

        }

        public static Model Instance
        {

            get { return instance; }

        }

        public override string Name
        {

            get { return string.Empty; }

            set { }

        }

    }

    public class NullNotifierModel : NotifierModelEntity, NullObject
    {

        private static readonly NotifierModel instance = new NullNotifierModel();

        private NullNotifierModel()
        {

        }

        public static NotifierModel Instance
        {

            get { return instance; }

        }

        public override string Name
        {
            
            get { return string.Empty; }
            
            set { }
        
        }

    }

}
