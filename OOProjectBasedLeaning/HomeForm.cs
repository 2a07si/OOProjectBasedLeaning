using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class HomeForm : DragDropForm
    {
        private Hotel hotel = new Hotel();

        // パネルごとにチェックアウト時間を管理
        private readonly Dictionary<GuestPanel, DateTime> checkoutTime = new();

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

                // パネル単位でチェックアウト済みか確認
                if (checkoutTime.ContainsKey(guestPanel))
                {
                    MessageBox.Show($"{guest.Name} さんは既にチェックアウト済みです。\nチェックアウト完了日時：{checkoutTime[guestPanel]:yyyy年MM月dd日 HH:mm:ss}",
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
                        DateTime completedTime = DateTime.Now;

                        // パネルのチェックアウト完了時間を登録
                        checkoutTime[guestPanel] = completedTime;

                        MessageBox.Show($"{guest.Name} さん（このパネル）がホテルからチェックアウトしました。\nチェックアウト完了日時：{completedTime:yyyy年MM月dd日 HH:mm:ss}");

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
