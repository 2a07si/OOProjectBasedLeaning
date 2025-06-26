using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static OOProjectBasedLeaning.CompanionRank;

namespace OOProjectBasedLeaning
{
    public partial class GuestCreatorForm : Form
    {
        private TextBox txtLeaderName;         // 代表名入力用テキストボックス
        private ComboBox cmbLeaderRank;        // 代表のランクを選択するコンボボックス
        private NumericUpDown nudCompanionCount; // お連れ様の人数を指定する数値入力
        private Button btnCreate;              // ゲスト生成トリガーボタン
        private FlowLayoutPanel flow;         // 生成された GuestPanel を縦並びで保持する FlowLayoutPanel


        public GuestCreatorForm()
        {
            InitializeComponent();

            // フォーム全体サイズ設定
            ClientSize = new Size(550, 800);

            // ───── 代表情報入力エリア ─────

            // 代表名ラベル
            Controls.Add(new Label
            {
                Text = "代表名：",
                Location = new Point(10, 10),
                AutoSize = true
            });

            // 代表名入力用テキストボックス
            txtLeaderName = new TextBox
            {
                Location = new Point(94, 8),
                Size = new Size(250, 24)
            };
            Controls.Add(txtLeaderName);

            // 代表のランクラベル
            Controls.Add(new Label
            {
                Text = "ランク：",
                Location = new Point(25, 45),
                AutoSize = true
            });

            // ランクを選択するコンボボックス
            cmbLeaderRank = new ComboBox
            {
                Location = new Point(94, 43),
                Size = new Size(120, 24),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // 「一般」「会員」「VIP」を選択肢として追加
            cmbLeaderRank.Items.AddRange(new[] { "一般", "会員", "VIP" });
            cmbLeaderRank.SelectedIndex = 0;  // デフォルトは「一般」
            Controls.Add(cmbLeaderRank);

            // ───── お連れ様数指定 ─────

            Controls.Add(new Label
            {
                Text = "お連れ様の人数：",
                Location = new Point(220, 45),
                AutoSize = true
            });
            nudCompanionCount = new NumericUpDown
            {
                Location = new Point(369, 43),
                Minimum = 0,   // 0〜3 人まで
                Maximum = 3,
                Width = 43
            };
            Controls.Add(nudCompanionCount);

            // ───── 生成ボタン ─────

            btnCreate = new Button
            {
                Text = "生成",
                Location = new Point(422, 22),
                Size = new Size(100, 40)
            };

            btnCreate.Click += CreateGuestEvent;
            Controls.Add(btnCreate);

            // ------- GuestPanel 用コンテナ -------
            flow = new FlowLayoutPanel
            {
                Location = new Point(70, 120),
                Size = new Size(350, 700),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            Controls.Add(flow);
        }

        // 代表＋お連れ様をまとめて GuestPanel にし、flow に追加
        private void CreateGuestEvent(object sender, EventArgs e)
        {
            // 代表名チェック
            string leaderName = txtLeaderName.Text.Trim();
            if (string.IsNullOrEmpty(leaderName))
            {
                MessageBox.Show("代表名を入力してください。");
                return;
            }

            Guest leader;
            switch (cmbLeaderRank.SelectedItem.ToString())
            {
                case "会員":
                    leader = new MemberModel(Member.NEW, leaderName, /*isVip:*/ false);
                    break;
                case "VIP":
                    leader = new MemberModel(Member.NEW, leaderName, /*isVip:*/ true);
                    break;
                default:
                    leader = new GuestModel(leaderName);
                    break;
            }

            // お連れ様入力フォーム
            int count = (int)nudCompanionCount.Value;
            if (count > 0)
            {
                using var compForm = new GuestCompanionForm(count);
                if (compForm.ShowDialog() != DialogResult.OK) return;

                foreach (var info in compForm.CompanionInfos)
                {
                    Guest g = info.Rank switch
                    {
                        CompanionRank.会員 => new MemberModel(Member.NEW, info.Name, /*isVip:*/ false),
                        CompanionRank.VIP => new MemberModel(Member.NEW, info.Name, /*isVip:*/ true),
                        _ => new GuestModel(info.Name)
                    };
                    leader.AddCompanion(g);
                }
            }

            // Panel 生成・追加
            var panel = new GuestPanel(leader)
            {
                Margin = new Padding(0, 0, 0, 10)
            };
            flow.Controls.Add(panel);
        }

        // 使わない予定
        private Guest CreateGuest(string guestName) => new GuestModel(guestName);
        private Guest CreateMember() => new MemberModel("Member");

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) // ショートカットキー
        {
            // F12キーを押した時
            if (keyData == Keys.F12)
            {
                Application.Exit(); // プログラム終了
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

    }
}