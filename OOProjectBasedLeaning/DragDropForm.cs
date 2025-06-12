using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOProjectBasedLeaning
{

    public class DragDropForm : Form
    {

        public DragDropForm()
        {

            AllowDrop = true;

            DragEnter += DragDropForm_DragEnter;

            DragDrop += DragDropForm_DragDrop;

        }

        private void DragDropForm_DragEnter(object? sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.Serializable))
            {

                OnFormDragEnterSerializable(e);

            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {

                OnFormDragEnterText(e);

            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                OnFormDragEnterFileDrop(e);

            }

        }

        protected virtual void OnFormDragEnterSerializable(DragEventArgs dragEventArgs)
        {

            dragEventArgs.Effect = DragDropEffects.None;

        }

        protected virtual void OnFormDragEnterText(DragEventArgs dragEventArgs)
        {

            dragEventArgs.Effect = DragDropEffects.None;

        }

        protected virtual void OnFormDragEnterFileDrop(DragEventArgs dragEventArgs)
        {

            dragEventArgs.Effect = DragDropEffects.None;

        }

        private void DragDropForm_DragDrop(object? sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.Serializable))
            {

                OnFormDragDropSerializable(e.Data.GetData(DataFormats.Serializable), e);

            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {

                OnFormDragDropText(e.Data.GetData(DataFormats.Text).ToString(), e);

            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                OnFormDragDropFileDrop(e.Data.GetData(DataFormats.FileDrop) as string[], e);

            }

        }

        protected virtual void OnFormDragDropSerializable(object? serializableObject, DragEventArgs dragEventArgs)
        {

        }

        protected virtual void OnFormDragDropText(string text, DragEventArgs dragEventArgs)
        {

        }

        protected virtual void OnFormDragDropFileDrop(string[]? fileDropObject, DragEventArgs dragEventArgs)
        {

        }

    }

    public class NullDragDropForm : DragDropForm, NullObject
    {

        private static readonly DragDropForm instance = new NullDragDropForm();

        private NullDragDropForm()
        {
        
        }

        public static DragDropForm Instance
        {

            get { return instance; }

        }

    }

}
