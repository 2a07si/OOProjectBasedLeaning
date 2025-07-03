using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class StarRatingForm : Form
    {
        public int SelectedRating { get; private set; } = 0;
        public string Comment { get; private set; } = "";

        private readonly Button[] starButtons = new Button[5];

        public StarRatingForm()
        {
            Text = "レビュー評価";
            Size = new Size(600, 450); // フォーム全体を少し大きく
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            int margin = 50; // 余白用変数
            int starSpacing = 90; // 星ボタンの間隔

            // 星ボタンの作成
            for (int i = 0; i < 5; i++)
            {
                var btn = new Button
                {
                    Text = "☆",
                    Font = new Font(FontFamily.GenericSansSerif, 32),
                    Size = new Size(70, 70),
                    Location = new Point(margin + i * starSpacing, margin), // 左右・上に余白
                    Tag = i + 1
                };
                btn.Click += StarButton_Click;
                starButtons[i] = btn;
                Controls.Add(btn);
            }

            // コメントラベル
            var commentLabel = new Label
            {
                Text = "コメント:",
                Font = new Font("MS UI Gothic", 14),
                Location = new Point(margin, margin + 100) // 星から適度に余白
            };

            // コメント入力欄
            var commentBox = new TextBox
            {
                Multiline = true,
                Size = new Size(480, 120),
                Location = new Point(margin, margin + 130),
                Font = new Font("MS UI Gothic", 12)
            };

            Controls.Add(commentLabel);
            Controls.Add(commentBox);

            // 登録ボタン
            var submitButton = new Button
            {
                Text = "登録",
                Font = new Font("MS UI Gothic", 16, FontStyle.Bold),
                Size = new Size(200, 60),
                Location = new Point((Width - 200) / 2 - 10, Height - 120) // 画面中央・下に配置
            };

            submitButton.Click += (s, e) =>
            {
                Comment = commentBox.Text;
                if (SelectedRating == 0)
                {
                    MessageBox.Show("星を選択してください。");
                    return;
                }
                DialogResult = DialogResult.OK;
                Close();
            };

            Controls.Add(submitButton);
        }

        private void StarButton_Click(object sender, EventArgs e)
        {
            if (sender is Button clickedButton && clickedButton.Tag is int rating)
            {
                SelectedRating = rating;
                for (int i = 0; i < 5; i++)
                {
                    starButtons[i].Text = i < rating ? "★" : "☆";
                }
            }
        }
    }
}
