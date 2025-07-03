using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Size = new Size(350, 250);
            StartPosition = FormStartPosition.CenterParent;

            for (int i = 0; i < 5; i++)
            {
                var btn = new Button
                {
                    Text = "☆",
                    Font = new Font(FontFamily.GenericSansSerif, 24),
                    Size = new Size(50, 50),
                    Location = new Point(30 + i * 55, 30),
                    Tag = i + 1
                };
                btn.Click += StarButton_Click;
                starButtons[i] = btn;
                Controls.Add(btn);
            }

            var commentLabel = new Label { Text = "コメント:", Location = new Point(30, 100) };
            var commentBox = new TextBox { Multiline = true, Size = new Size(250, 60), Location = new Point(30, 130) };
            Controls.Add(commentLabel);
            Controls.Add(commentBox);

            var submitButton = new Button { Text = "登録", Location = new Point(200, 200) };
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
