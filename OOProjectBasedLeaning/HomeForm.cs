using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class HomeForm : DragDropForm
    {
        private readonly Hotel hotel = Hotel.Instance;

        // ゲストパネル表示用エリア
        private readonly FlowLayoutPanel guestPanelArea;

        // ゲストごとのレビュー保存辞書（複数レビューを保持）
        private readonly Dictionary<Guest, List<string>> reviewData = new();

        public HomeForm()
        {
            Text = "Home Form";
            Size = new Size(650, 500);
            BackColor = Color.White;

            // ゲストパネル表示エリアのセットアップ
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
            var reviewButton = new Button
            {
                Text = "レビュー確認",
                Size = new Size(120, 40),
                Location = new Point(400, 20)
            };
            reviewButton.Click += ReviewButton_Click;
            Controls.Add(reviewButton);
        }

        // HomeForm へのドラッグ許可
        protected override void OnFormDragEnterSerializable(DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        // HomeForm へドロップされたときの処理
        protected override void OnFormDragDropSerializable(object? obj, DragEventArgs e)
        {
            if (!(obj is GuestPanel guestPanel))
                return;

            var guest = guestPanel.GetGuest();

            // 既に HomeForm 内にいなければパネルを追加
            if (!guestPanelArea.Controls.Contains(guestPanel))
            {
                var oldParent = guestPanel.Parent;
                if (oldParent != null)
                    oldParent.Controls.Remove(guestPanel);

                guestPanelArea.Controls.Add(guestPanel);
            }

            // レビュー作成
            CreateReview(guest);
        }

        // レビュー入力ダイアログ
        private void CreateReview(Guest guest)
        {
            int rating;
            while (true)
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox(
                    "1〜5の評価を入力してください。（☆の数）",
                    "レビュー評価",
                    "5"
                );
                if (int.TryParse(input, out rating) && rating >= 1 && rating <= 5)
                    break;
                MessageBox.Show("1〜5の数字で入力してください。");
            }

            string comment = Microsoft.VisualBasic.Interaction.InputBox(
                "コメントを入力してください。",
                "レビューコメント",
                ""
            );
            string stars = new string('★', rating) + new string('☆', 5 - rating);
            string review = $"評価：{stars}\nコメント：{comment}";

            // 辞書に追加
            if (!reviewData.ContainsKey(guest))
                reviewData[guest] = new List<string>();
            reviewData[guest].Add(review);

            // 完了メッセージ
            MessageBox.Show(
                $"{guest.Name} さんのレビューを登録しました。\n{review}",
                "レビュー内容",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        // 「レビュー確認」ボタン押下時
        private void ReviewButton_Click(object sender, EventArgs e)
        {
            if (reviewData.Count == 0)
            {
                MessageBox.Show("レビューはまだ登録されていません。");
                return;
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("登録済みレビュー一覧\n");
            foreach (var kv in reviewData)
            {
                var guest = kv.Key;
                var reviews = kv.Value;
                sb.AppendLine($"● {guest.Name} さんのレビュー ({reviews.Count}件):");
                foreach (var r in reviews)
                    sb.AppendLine(r + "\n");
            }

            MessageBox.Show(sb.ToString(), "レビュー一覧");
        }
    }
}
