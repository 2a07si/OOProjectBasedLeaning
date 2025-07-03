using System;
using System.Drawing;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class StarRatingForm : Form
    {
        public int SelectedRating { get; private set; } = 0;
        public string Comment { get; private set; } = "";

        private readonly Button[] starButtons = new Button[5];
        private RichTextBox commentBox;

        public StarRatingForm()
        {
            Text = "レビュー評価";
            Size = new Size(600, 450);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            int marginTop = 50;
            int starCount = 5;
            int starSize = 70;
            int starSpacing = 10;

            int totalWidth = starCount * starSize + (starCount - 1) * starSpacing;
            int startX = (this.ClientSize.Width - totalWidth) / 2;

            for (int i = 0; i < starCount; i++)
            {
                var btn = new Button
                {
                    Text = "☆",
                    Font = new Font(FontFamily.GenericSansSerif, 32),
                    Size = new Size(starSize, starSize),
                    Location = new Point(startX + i * (starSize + starSpacing), marginTop),
                    Tag = i + 1,
                    FlatStyle = FlatStyle.Flat,
                    TabStop = false
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseDownBackColor = Color.White;
                btn.FlatAppearance.MouseOverBackColor = Color.White;
                btn.Click += StarButton_Click;
                starButtons[i] = btn;
                Controls.Add(btn);
            }

            // コメントラベル
            var commentLabel = new Label
            {
                Text = "コメント:",
                Font = new Font("MS UI Gothic", 16, FontStyle.Bold),
                Size = new Size(120,30),
                Location = new Point(50, marginTop + 80)  // 10px上に移動
            };
            Controls.Add(commentLabel);

            // コメント入力欄をRichTextBoxに変更し、少し上に配置
            commentBox = new RichTextBox
            {
                Multiline = true,
                Size = new Size(480, 120),
                Location = new Point(50, marginTop + 120),  // 10px上に移動（元130 → 120）
                Font = new Font("MS UI Gothic", 12)
            };
            Controls.Add(commentBox);

            // 登録ボタン
            var submitButton = new Button
            {
                Text = "登録",
                Font = new Font("MS UI Gothic", 16, FontStyle.Bold),
                Size = new Size(200, 60),
                Location = new Point((Width - 200) / 2 - 10, Height - 120)
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
                for (int i = 0; i < starButtons.Length; i++)
                {
                    starButtons[i].Text = i < rating ? "★" : "☆";
                }
            }
        }
    }
}
