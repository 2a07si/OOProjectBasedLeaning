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
            AllowDrop = true; // このフォーム全体でドラッグドロップ許可
            DragEnter += DragDropForm_DragEnter; // ドラッグがフォーム領域に入ってきたとき
            DragDrop += DragDropForm_DragDrop; // ドロップされたとき
        }

        private void DragDropForm_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Serializable)) OnFormDragEnterSerializable(e);
            else if (e.Data.GetDataPresent(DataFormats.Text)) OnFormDragEnterText(e);
            else if (e.Data.GetDataPresent(DataFormats.FileDrop)) OnFormDragEnterFileDrop(e);
        }
        private void DragDropForm_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Serializable)) OnFormDragDropSerializable(e.Data.GetData(DataFormats.Serializable), e);
            else if (e.Data.GetDataPresent(DataFormats.Text)) OnFormDragDropText(e.Data.GetData(DataFormats.Text)?.ToString(), e);
            else if (e.Data.GetDataPresent(DataFormats.FileDrop)) OnFormDragDropFileDrop(e.Data.GetData(DataFormats.FileDrop) as string[], e);
        }
        protected virtual void OnFormDragEnterSerializable(DragEventArgs e) => e.Effect = DragDropEffects.None;
        protected virtual void OnFormDragEnterText(DragEventArgs e) => e.Effect = DragDropEffects.None;
        protected virtual void OnFormDragEnterFileDrop(DragEventArgs e) => e.Effect = DragDropEffects.None;
        protected virtual void OnFormDragDropSerializable(object? obj, DragEventArgs e) { }
        protected virtual void OnFormDragDropText(string? text, DragEventArgs e) { }
        protected virtual void OnFormDragDropFileDrop(string[]? files, DragEventArgs e) { }
    }
    public class NullDragDropForm : DragDropForm, NullObject
    {
        private static readonly DragDropForm instance = new NullDragDropForm();
        private NullDragDropForm() { }
        public static DragDropForm Instance => instance;
    }

}