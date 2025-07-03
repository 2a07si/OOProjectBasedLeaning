using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OOProjectBasedLeaning
{
    public partial class YoyakuForm : DragDropForm
    {
        // �V���O���g���o�R�Ńz�e�������ꌳ�Ǘ�
        private readonly Hotel hotel = Hotel.Instance;
        private readonly FlowLayoutPanel guestPanelArea;
        private readonly HomeForm homeForm;
        // �\�񊮗������L�^
        private DateTime? reservationCompletedTime;

        private readonly Dictionary<Guest, List<string>> reviewData = new();

        public YoyakuForm(HomeForm homeForm)
        {
            this.homeForm = homeForm;

            Text = "�\��Ǘ�";
            Size = new Size(735, 600);

            guestPanelArea = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                Width = 400,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            Controls.Add(guestPanelArea);
        }

        protected override void OnFormDragEnterSerializable(DragEventArgs e)
        {
            // �h���b�O�\�Ɣ��f
            e.Effect = e.Data.GetDataPresent(DataFormats.Serializable)
                && e.Data.GetData(DataFormats.Serializable) is GuestPanel
                ? DragDropEffects.Move
                : DragDropEffects.None;
        }

        protected override void OnFormDragDropSerializable(object? obj, DragEventArgs e)
        {
            if (!(obj is GuestPanel guestPanel))
            {
                return;
            }
           
            // �d���h���b�O�h�~
            if (guestPanelArea.Controls.Contains(guestPanel))
            {
                MessageBox.Show(
                    $"{guestPanel.GetGuest().Name} ����͊��ɗ\��ς݂ł��B\n" +
                    $"�\�񊮗������F{GetReservationTimeString()}",
                    "�\��d���G���[",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                e.Effect = DragDropEffects.None;
                return;
            }

            if (guestPanel.FindForm() is HotelForm)
            {
                var guest = guestPanel.GetGuest();

                // �`�F�b�N�A�E�g�����iHotel �N���X�Ƀ��\�b�h������O��j
                hotel.CheckOut(guest);

                // ���r���[����
                homeForm.CreateReview(guest);

            }

            // �\��\�^�\��σ��X�g�� Hotel ����擾
            var availableRooms = hotel.AllRooms.Where(r => hotel.IsVacant(r)).ToList();

            var reservedRooms = hotel.AllRooms.Where(r => hotel.IsReserved(r)).ToList();

            using var selectForm = new RoomSelectForm(availableRooms, reservedRooms, guestPanel.GetGuest());
            if (selectForm.ShowDialog() == DialogResult.OK)
            {
                var selectedRoom = selectForm.SelectedRoom;
                if (selectedRoom == null)
                {
                    MessageBox.Show("�������I������Ă��܂���B", "�\��G���[", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    // Hotel �N���X�ɗ\����Ϗ�
                    var guest = guestPanel.GetGuest();
                    var now = DateTime.Now;
                    hotel.Reserve(selectedRoom.Number, guest, now, now);

                    // UI ��ɃQ�X�g�p�l����z�u
                    guestPanelArea.Controls.Add(guestPanel);

                    reservationCompletedTime = DateTime.Now;
                    MessageBox.Show(
                        $"{guestPanel.GetGuest().Name} ����̗\�񂪊������܂����B\n" +
                        $"�\�񊮗������F{GetReservationTimeString()}\n" +
                        $"�\�񕔉��ԍ��F{selectedRoom.Number}",
                        "�\�񊮗�",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"{guestPanel.GetGuest().Name} ����̗\��Ɏ��s���܂����F\n{ex.Message}",
                        "�\��G���[",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            else
            {
                MessageBox.Show("�����̑I�����L�����Z������܂����B", "�\��L�����Z��", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// �\�񊮗������𕶎���Ŏ擾
        /// </summary>
        private string GetReservationTimeString()
        {
            if (reservationCompletedTime == null)
                reservationCompletedTime = DateTime.Now;
            return reservationCompletedTime.Value.ToString("yyyy�NMM��dd�� HH:mm:ss");
        }
        public void AddDragDropForm(Control container, Point location)
        {
            container.Controls.Add(this);
            this.Location = location;
        }

    }
}
