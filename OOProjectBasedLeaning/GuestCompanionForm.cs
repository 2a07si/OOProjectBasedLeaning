using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public enum CompanionRank { 一般, 会員, VIP }

    public partial class GuestCompanionForm : Form
    {
        private readonly int count;                  // 入力行数（お連れ様の人数）
        private readonly List<TextBox> nameBoxes = new();   // 名前入力用 TextBox のリスト
        private readonly List<ComboBox> rankBoxes = new();  // ランク選択用 ComboBox のリスト
        private readonly Button btnOk, btnCancel;    // OK/Cancel ボタン

        // 外部から取得するプロパティ：入力された (名前, ランク) のリスト
        public List<(string Name, CompanionRank Rank)> CompanionInfos { get; } = new();

        public GuestCompanionForm(int count)
        {
            InitializeComponent();
            this.count = count;

            // フォームサイズ
            ClientSize = new Size(360, 40 * count + 100);
            Text = "お連れ様情報";

            for (int i = 0; i < count; i++)
            {
                var y = 10 + i * 40;
                // 名前欄
                var tb = new TextBox
                {
                    Location = new Point(10, y),
                    Size = new Size(180, 24),
                    PlaceholderText = $"お連れ様{i + 1}の名前"
                };
                nameBoxes.Add(tb);
                Controls.Add(tb);

                // ランク付け
                var cb = new ComboBox
                {
                    Location = new Point(200, y),
                    Size = new Size(120, 24),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                cb.Items.AddRange(Enum.GetNames(typeof(CompanionRank)));
                cb.SelectedIndex = 0;
                rankBoxes.Add(cb);
                Controls.Add(cb);
            }

            for (int i = 0; i < count; i++)
            {
                nameBoxes[i].TabStop = true;
                nameBoxes[i].TabIndex = i;
                // ランクのコンボボックスのTabIndexはfalse
                rankBoxes[i].TabStop = false;
            }

            // OK / Cancel ボタン
            btnOk = new Button
            {
                Text = "OK",
                Location = new Point(80, 40 * count + 20),
                Size = new Size(80, 30),
                DialogResult = DialogResult.OK
            };
            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(180, 40 * count + 20),
                Size = new Size(80, 30),
                DialogResult = DialogResult.Cancel,
                TabStop = false   // タブ移動不要
            };
            Controls.AddRange(new Control[] { btnOk, btnCancel });

            AcceptButton = btnOk;
            CancelButton = btnCancel;

            btnCancel.TabStop = false;

            btnOk.Click += (s, e) =>
            {
                // 全て入力されているかチェック
                if (nameBoxes.Any(tb => string.IsNullOrWhiteSpace(tb.Text)))
                {
                    MessageBox.Show("すべてのお連れ様の名前を入力してください。");
                    this.DialogResult = DialogResult.None;
                    return;
                }

                // CompanionInfos に格納
                CompanionInfos.Clear();
                for (int i = 0; i < count; i++)
                {
                    CompanionInfos.Add((nameBoxes[i].Text.Trim(),
                        (CompanionRank)rankBoxes[i].SelectedIndex));
                }
            };
        }
    }
}
