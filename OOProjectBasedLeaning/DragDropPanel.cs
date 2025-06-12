using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OOProjectBasedLeaning
{

    public abstract class DragDropPanel : Panel, ISerializable
    {

        private Form form = NullDragDropForm.Instance;

        public DragDropPanel()
        {

            MouseDown += DragDropPanel_MouseDown;

        }

        private void DragDropPanel_MouseDown(object? sender, MouseEventArgs e)
        {

            OnPanelMouseDown();

        }

        /// <summary>
        /// Usage of this method is to handle mouse down events on the panel.
        /// e.g.) [to Copy]
        /// protected override void OnPanelMouseDown()
        /// {
        ///     DoDragDropCopy();
        /// }
        /// </summary>
        protected abstract void OnPanelMouseDown();

        protected void DoDragDropCopy()
        {

            DoDragDrop(this, DragDropEffects.Copy);

        }

        protected void DoDragDropMove()
        {

            DoDragDrop(this, DragDropEffects.Move);

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }

        public virtual DragDropPanel AddDragDropForm(Form form, Point dropPoint)
        {

            RemoveForm();

            this.form = form;
            this.form.Controls.Add(MoveTo(dropPoint));

            return this;

        }

        public virtual DragDropPanel RemoveForm()
        {

            if (form.Contains(this))
            {

                form.Controls.Remove(this);

                form = NullDragDropForm.Instance;

            }

            return this;

        }

        protected DragDropPanel MoveTo(Point point)
        {

            Location = point;

            return this;

        }

    }

    public class NullDragDropPanel : DragDropPanel, NullObject
    {

        private static readonly DragDropPanel instance = new NullDragDropPanel();

        private NullDragDropPanel()
        {

        }

        public static DragDropPanel Instance
        {

            get { return instance; }

        }

        protected override void OnPanelMouseDown()
        {

        }

        public override DragDropPanel AddDragDropForm(Form form, Point dropPoint)
        {

            return this;

        }

        public override DragDropPanel RemoveForm()
        {

            return this;

        }

    }

}
