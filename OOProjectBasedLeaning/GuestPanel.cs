using System.Drawing;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public class GuestPanel : DragDropPanel
    {
        private readonly Guest guest;
        private readonly Label leaderLabel;
        private readonly FlowLayoutPanel iconPanel;
        private readonly Button btnDetail;

        public Control? OriginalContainer { get; set; }


        public GuestPanel(Guest guest)
        {
            this.guest = guest;

            // 枠線と内側余白
            BorderStyle = BorderStyle.FixedSingle;
            Padding = new Padding(5);
            Size = new Size(170, 65);
            BackColor = Color.White;

            leaderLabel = new Label
            {
                Text = guest.Name,
                Location = new Point(5, 5),
                AutoSize = true
            };
            Controls.Add(leaderLabel);

            iconPanel = new FlowLayoutPanel
            {
                Location = new Point(5, 25),
                Size = new Size(180, 30),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = true,
                //Padding = new Padding(0),
                //Margin = new Padding(0),
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };
            this.Controls.Add(iconPanel);

            //for (int i = 0; i < 10; i++)
            //{
            //    var panel = new Panel
            //    {
            //        Size = new Size(160, 40),
            //        BackColor = Color.LightBlue,
            //        Margin = new Padding(0, 0, 0, 10) // 下に隙間
            //    };

            //    iconPanel.Controls.Add(panel);
            //}


            RefreshIcons();

            btnDetail = new Button
            {
                Text = "▽",
                Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold),
                Size = new Size(24, 30),
                Location = new Point(this.ClientSize.Width - 30, 5),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.TopCenter,
            };
            btnDetail.FlatAppearance.BorderSize = 0;
            btnDetail.Click += (s, e) => ShowCompanionNames();
            Controls.Add(btnDetail);
            btnDetail.BringToFront();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            ShowCompanionNames();
        }

        private void RefreshIcons()
        {
            iconPanel.Controls.Clear();
            iconPanel.Controls.Add(CreateStatusIcon(guest));

            // リーダーと連れアイコンの間に余白
            var spacer = new Panel { Width = 10, Height = 1 };
            iconPanel.Controls.Add(spacer);

            // 連れアイコン
            foreach (var c in guest.Companions)
            {
                iconPanel.Controls.Add(CreateStatusIcon(c));
            }
        }

        protected override void OnPanelMouseDown() => DoDragDropMove();

        private Control CreateStatusIcon(Guest g) // ランクアイコン定義
        {
            var pb = new PictureBox
            {
                Size = new Size(16, 16),
                Margin = new Padding(left: 2, top: 6, right: 2, bottom: 0)
            };
            if (g.IsVIP()) pb.BackColor = Color.Gold;
            else if (g.IsMember()) pb.BackColor = Color.Silver;
            else pb.BackColor = Color.Black;
            return pb;
        }

        private void ShowCompanionNames()
        {
            if (guest.Companions.Count == 0)
            {
                MessageBox.Show("お連れ様はいません。");
                return;
            }

            int rows = guest.Companions.Count;
            int formWidth = 300;
            int formHeight = 30 + rows * 25 + 20;  // 見出し＋各行25px＋余白

            using var detail = new Form
            {
                StartPosition = FormStartPosition.CenterParent,
                ClientSize = new Size(formWidth, formHeight),
                Text = "お連れ様一覧"
            };
            var lb = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("MS UI Gothic", 10)
            };
            foreach (var c in guest.Companions)
            {
                string rank = c.IsVIP() ? "VIP" : c.IsMember() ? "会員" : "一般";
                lb.Items.Add($"{c.Name} ({rank})");
            }
            detail.Controls.Add(lb);
            detail.ShowDialog();
        }

        public Guest GetGuest() => guest;

    }
}