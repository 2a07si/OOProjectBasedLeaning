using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class HomeForm : DragDropForm
    {
        private Hotel hotel = Hotel.Instance;

        // パネルごとにチェックアウト時間を管理
        private readonly Dictionary<GuestPanel, DateTime> checkoutTime = new();

        // ゲストパネル表示用パネル
        private FlowLayoutPanel guestPanelArea;

        // レビュー保存用辞書
        private Dictionary<Guest, string> reviewData = new();

        public HomeForm()
        {
            Text = "Home Form";
            Size = new Size(800, 500);
            BackColor = Color.White;

            // ゲストパネル表示エリア
            guestPanelArea = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                Width = 400,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            Controls.Add(guestPanelArea);

            // レビュー確認ボタン
            Button reviewButton = new Button
            {
                Text = "レビュー確認",
                Size = new Size(120, 40),
                Location = new Point(420, 20)
            };
            reviewButton.Click += ReviewButton_Click;
            Controls.Add(reviewButton);
        }

        protected override void OnFormDragEnterSerializable(DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        protected override void OnFormDragDropSerializable(object? obj, DragEventArgs e)
        {
            if (obj is GuestPanel guestPanel)
            {
                Guest guest = guestPanel.GetGuest();

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

                        checkoutTime[guestPanel] = completedTime;

                        MessageBox.Show($"{guest.Name} さんがホテルからチェックアウトしました。\nチェックアウト完了日時：{completedTime:yyyy年MM月dd日 HH:mm:ss}");

                        // ゲストパネルを表示エリアに追加
                        if (!guestPanelArea.Controls.Contains(guestPanel))
                        {
                            guestPanelArea.Controls.Add(guestPanel);
                        }

                        // レビュー入力処理
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

            string review = $"評価：{stars}\nコメント：{comment}";

            // レビューを保存
            reviewData[guest] = review;

            // 入力直後にも表示
            MessageBox.Show($"{guest.Name} さんのレビュー\n{review}", "レビュー内容");
        }

        // レビュー確認ボタンの処理
        private void ReviewButton_Click(object sender, EventArgs e)
        {
            if (reviewData.Count == 0)
            {
                MessageBox.Show("レビューはまだ登録されていません。");
                return;
            }

            string allReviews = "登録済みレビュー一覧\n\n";

            foreach (var item in reviewData)
            {
                Guest guest = item.Key;
                string review = item.Value;

                allReviews += $"● {guest.Name} さんのレビュー\n{review}\n\n";
            }

            MessageBox.Show(allReviews, "レビュー一覧");
        }
    }
}
