using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class HomeForm : DragDropForm
    {
        private readonly Hotel hotel = Hotel.Instance;

        private readonly FlowLayoutPanel guestPanelArea;

        private readonly Dictionary<Guest, List<string>> reviewData = new();

        public HomeForm()
        {
            Text = "Home Form";
            Size = new Size(652, 600);
            BackColor = Color.White;

            guestPanelArea = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                Width = 400,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            Controls.Add(guestPanelArea);

            var reviewButton = new Button
            {
                Text = "レビュー確認",
                Size = new Size(120, 40),
                Location = new Point(400, 40)
            };
            reviewButton.Click += ReviewButton_Click;
            Controls.Add(reviewButton);
        }

        protected override void OnFormDragEnterSerializable(DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;

            if (e.Data.GetDataPresent(DataFormats.Serializable) &&
                e.Data.GetData(DataFormats.Serializable) is GuestPanel guestPanel)
            {
                if (guestPanel.FindForm() is YoyakuForm || guestPanel.FindForm() is GuestCreatorForm)
                {
                    e.Effect = DragDropEffects.None;
                    return;
                }

                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        protected override void OnFormDragDropSerializable(object? obj, DragEventArgs e)
        {
            if (obj is GuestPanel guestPanel)
            {
                if (guestPanel.FindForm() is YoyakuForm || guestPanel.FindForm() is GuestCreatorForm)
                {
                    MessageBox.Show("予約管理画面からホーム画面への移動はできません。", "操作無効", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                guestPanel.AddDragDropForm(this, PointToClient(new Point(e.X, e.Y)));
            }
            else
            {
                return;
            }

            var guest = guestPanel.GetGuest();

            if (!guestPanelArea.Controls.Contains(guestPanel))
            {
                var oldParent = guestPanel.Parent;
                if (oldParent != null)
                    oldParent.Controls.Remove(guestPanel);

                guestPanelArea.Controls.Add(guestPanel);
            }

            CreateReview(guest);
        }

        // ⭐ 星評価フォームを呼び出す ⭐
        private void CreateReview(Guest guest)
        {
            using (var form = new StarRatingForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    int rating = form.SelectedRating;
                    string comment = form.Comment;

                    string stars = new string('★', rating) + new string('☆', 5 - rating);
                    string review = $"評価：{stars}\nコメント：{comment}";

                    if (!reviewData.ContainsKey(guest))
                        reviewData[guest] = new List<string>();

                    reviewData[guest].Add(review);

                    MessageBox.Show(
                        $"{guest.Name} さんのレビューを登録しました。\n{review}",
                        "レビュー内容",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
        }

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
