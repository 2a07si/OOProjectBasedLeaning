using System;
using System.Drawing;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class HomeForm : DragDropForm
    {
        public DateTime? ReservationCompletedTime { get; private set; }

        private Hotel hotel = new Hotel();
        private bool isCheckedOut = false;

        public HomeForm()
        {
            Text = "Home Form";
            Size = new Size(400, 400);
            BackColor = Color.White;
        }

        protected override void OnFormDragEnterSerializable(DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        protected override void OnFormDragDropSerializable(object? obj, DragEventArgs e)
        {
            if (obj is GuestPanel guestPanel)
            {
                bool isAlreadyOnThisForm = this.Controls.Contains(guestPanel);
                guestPanel.AddDragDropForm(this, PointToClient(new Point(e.X, e.Y)));
                Guest guest = guestPanel.GetGuest();

                if (isAlreadyOnThisForm || isCheckedOut)
                {
                    MessageBox.Show($"{guest.Name} さんは既にチェックアウト済みです。\nチェックアウト完了日時：{ReservationCompletedTime?.ToString("yyyy年MM月dd日 HH:mm:ss")}",
                        "チェックアウト重複エラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    e.Effect = DragDropEffects.None;
                }
                else
                {
                    try
                    {
                        hotel.CheckOut(guest);
                        isCheckedOut = true;
                        ReservationCompletedTime = DateTime.Now;

                        MessageBox.Show($"{guest.Name} さんがホテルからチェックアウトしました。\nチェックアウト完了日時：{ReservationCompletedTime?.ToString("yyyy年MM月dd日 HH:mm:ss")}");

                        CreateReview(guest);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"チェックアウトに失敗しました: {ex.Message}");
                    }
                }
            }
        }

        private void CreateReview(Guest guest)
        {
            int rating = 0;
            while (true)
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox("1〜5の評価を入力してください。（☆の数）", "レビュー評価", "5");
                if (int.TryParse(input, out rating) && rating >= 1 && rating <= 5)
                {
                    break;
                }
                MessageBox.Show("1〜5の数字で入力してください。");
            }

            string comment = Microsoft.VisualBasic.Interaction.InputBox("コメントを入力してください。", "レビューコメント", "");

            string stars = new string('★', rating) + new string('☆', 5 - rating);

            MessageBox.Show($"{guest.Name} さんのレビュー\n評価：{stars}\nコメント：{comment}", "レビュー内容");
        }
    }
}
