namespace OOProjectBasedLeaning
{
    // レビュー内容といいね数を管理するクラス
    public class Review
    {
        public string Content { get; set; } = "";
        public int Likes { get; set; } = 0;
        public int Bads { get; set; }
    }

    public partial class HomeForm : DragDropForm
    {
        private readonly Hotel hotel = Hotel.Instance;
        private readonly FlowLayoutPanel guestPanelArea;

        // Guestごとに複数レビュー保持（Review型リスト）
        private readonly Dictionary<Guest, List<Review>> reviewData = new();

        // 一度いいねしたレビューを保存するセット（重複防止）
        private readonly HashSet<Review> likedReviews = new();

        //低評価機能
        private readonly HashSet<Review> badReviews = new();


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

        // 星評価フォームを呼び出す
        public void CreateReview(Guest guest)
        {
            using (var form = new StarRatingForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    int rating = form.SelectedRating;
                    string comment = form.Comment;

                    string stars = new string('★', rating) + new string('☆', 5 - rating);
                    string reviewText = $"評価：{stars}\nコメント：{comment}";

                    if (!reviewData.ContainsKey(guest))
                        reviewData[guest] = new List<Review>();

                    reviewData[guest].Add(new Review { Content = reviewText, Likes = 0 });

                    MessageBox.Show(
                        $"{guest.Name} さんのレビューを登録しました。\n{reviewText}",
                        "レビュー内容",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
        }

        // レビュー一覧表示 + いいね！機能付き
        private void ReviewButton_Click(object sender, EventArgs e)
        {
            if (reviewData.Count == 0)
            {
                MessageBox.Show("レビューはまだ登録されていません。");
                return;
            }

            var form = new Form
            {
                Text = "レビュー一覧",
                Size = new Size(500, 600),
                StartPosition = FormStartPosition.CenterParent
            };

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            form.Controls.Add(panel);

            foreach (var kv in reviewData)
            {
                var guest = kv.Key;
                var reviews = kv.Value;

                var guestLabel = new Label
                {
                    Text = $"● {guest.Name} さんのレビュー ({reviews.Count}件):",
                    Font = new Font("MS UI Gothic", 12, FontStyle.Bold),
                    AutoSize = true
                };
                panel.Controls.Add(guestLabel);

                reviews.Sort((a, b) => b.Likes.CompareTo(a.Likes));

                foreach (var review in reviews)
                {
                    var reviewPanel = new Panel
                    {
                        Width = panel.ClientSize.Width - 25,
                        Height = 80,
                        BorderStyle = BorderStyle.FixedSingle,
                        Margin = new Padding(3)
                    };

                    var reviewLabel = new Label
                    {
                        Text = review.Content,
                        Location = new Point(5, 5),
                        Size = new Size(reviewPanel.Width - 90, 50),
                        Font = review.Likes >= 5 ? new Font("MS UI Gothic", 11, FontStyle.Bold) : new Font("MS UI Gothic", 10),
                        ForeColor = review.Likes >= 5 ? Color.DarkOrange : Color.Black
                    };
                    reviewPanel.Controls.Add(reviewLabel);

                    var likeButton = new Button
                    {
                        Text = review.Likes >= 99 ? "👍 99+" : $"👍 {review.Likes}",
                        Location = new Point(reviewPanel.Width - 75,10 ),
                        Size = new Size(60, 30),
                        Tag = review
                    };
                    likeButton.Click += (s, ev) =>
                    {
                        var btn = s as Button;
                        if (btn?.Tag is Review r)
                        {
                            if (likedReviews.Contains(r))
                            {
                                MessageBox.Show("このレビューには既にいいねしています。", "いいね済み", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }

                            r.Likes++;
                            btn.Text = r.Likes >= 99 ? "👍 99+" : $"👍 {r.Likes}";
                            likedReviews.Add(r);

                            if (r.Likes == 5)
                            {
                                reviewLabel.Font = new Font("MS UI Gothic", 11, FontStyle.Bold);
                                reviewLabel.ForeColor = Color.DarkOrange;
                            }
                        }
                    };

                    var BadButton = new Button
                    {
                        Text = review.Bads >= 99 ? "👎 99+" : $"👎 {review.Bads}",
                        Location = new Point(reviewPanel.Width - 75, 40),
                        Size = new Size(60, 30),
                        Tag = review
                    };
                    BadButton.Click += (s, ev) =>
                    {
                        var btn = s as Button;
                        if (btn?.Tag is Review r)
                        {
                            if (badReviews.Contains(r))
                            {
                                MessageBox.Show("このレビューには既に低評価をしています。", "低評価済み", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }

                            r.Bads++;
                            btn.Text = r.Bads >= 99 ? "👎 99+" : $"👎 {r.Bads}";
                            badReviews.Add(r);

                            if (r.Bads == 5)
                            {
                                reviewLabel.Font = new Font("MS UI Gothic", 11, FontStyle.Bold);
                                reviewLabel.ForeColor = Color.DarkOrange;
                            }
                        }
                    };

                    reviewPanel.Controls.Add(likeButton);
                    reviewPanel.Controls.Add(BadButton);

                    panel.Controls.Add(reviewPanel);
                }
            }

            form.ShowDialog();
        }
    }
}
