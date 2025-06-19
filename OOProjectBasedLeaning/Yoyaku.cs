using System;
using System.Drawing;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class yoyaku : DragDropForm
    {
        public yoyaku()
        {
            this.Text = "予約管理";
            this.Size = new Size(800, 600);

            new GuestCreatorForm().Show();
            new HomeForm().Show();
            new HotelForm().Show();
        }

        protected override void OnFormDragEnterSerializable(DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        protected override void OnFormDragDropSerializable(object? obj, DragEventArgs e)
        {
            if (obj is DragDropPanel panel)
                panel.AddDragDropForm(this, PointToClient(new Point(e.X, e.Y)));
            MessageBox.Show("予約が完了しました。\n予約完了日時：" + UpdateTimeLabel() );
        }
        private string UpdateTimeLabel()
        {
            // DateTime.Now で現在の日付と時刻を取得
            // ToString() メソッドで表示形式を指定
            // "yyyy/MM/dd HH:mm:ss" は「年/月/日 時:分:秒」の形式
            string date = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
            return date;
        }
    }
}
