using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    /// <summary>
    /// 予約管理フォーム (YoyakuForm)
    /// Hotel クラスに情報を集約し、ローカル状態を排除
    /// </summary>
    public partial class YoyakuForm : DragDropForm
    {
        // シングルトン経由でホテル情報を一元管理
        private readonly Hotel hotel = Hotel.Instance;

        // 予約完了日時記録
        private DateTime? reservationCompletedTime;

        public YoyakuForm()
        {
            Text = "予約管理";
            Size = new Size(735, 600);
        }

        protected override void OnFormDragEnterSerializable(DragEventArgs e)
        {
            // ドラッグ可能と判断
            e.Effect = e.Data.GetDataPresent(DataFormats.Serializable)
                && e.Data.GetData(DataFormats.Serializable) is GuestPanel
                ? DragDropEffects.Move
                : DragDropEffects.None;
        }

        protected override void OnFormDragDropSerializable(object? obj, DragEventArgs e)
        {
            if (!(obj is GuestPanel guestPanel))
                return;

            // 重複ドラッグ防止
            if (Controls.Contains(guestPanel))
            {
                MessageBox.Show(
                    $"{guestPanel.GetGuest().Name} さんは既に予約済みです。\n" +
                    $"予約完了日時：{GetReservationTimeString()}",
                    "予約重複エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                e.Effect = DragDropEffects.None;
                return;
            }

            // 予約可能／予約済リストを Hotel から取得
            var availableRooms = hotel.AllRooms.Where(r => hotel.IsVacant(r)).ToList();
            var reservedRooms = hotel.AllRooms.Where(r => hotel.IsReserved(r)).ToList();

            using var selectForm = new RoomSelectForm(availableRooms, reservedRooms, guestPanel.GetGuest());
            if (selectForm.ShowDialog() == DialogResult.OK)
            {
                var selectedRoom = selectForm.SelectedRoom;
                if (selectedRoom == null)
                {
                    MessageBox.Show("部屋が選択されていません。", "予約エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    // Hotel クラスに予約を委譲
                    var guest = guestPanel.GetGuest();
                    var now = DateTime.Now;
                    hotel.Reserve(selectedRoom.Number, guest, now, now);

                    // UI 上にゲストパネルを配置
                    guestPanel.AddDragDropForm(this, PointToClient(new Point(e.X, e.Y)));

                    reservationCompletedTime = DateTime.Now;
                    MessageBox.Show(
                        $"{guestPanel.GetGuest().Name} さんの予約が完了しました。\n" +
                        $"予約完了日時：{GetReservationTimeString()}\n" +
                        $"予約部屋番号：{selectedRoom.Number}",
                        "予約完了",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"{guestPanel.GetGuest().Name} さんの予約に失敗しました：\n{ex.Message}",
                        "予約エラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            else
            {
                MessageBox.Show("部屋の選択がキャンセルされました。", "予約キャンセル", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 予約完了時刻を文字列で取得
        /// </summary>
        private string GetReservationTimeString()
        {
            if (reservationCompletedTime == null)
                reservationCompletedTime = DateTime.Now;
            return reservationCompletedTime.Value.ToString("yyyy年MM月dd日 HH:mm:ss");
        }
    }
}
