namespace OOProjectBasedLeaning
{
    partial class HotelForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            hierarchy_change = new Panel();
            rdb_10 = new RadioButton();
            rdb_5 = new RadioButton();
            rdb_8 = new RadioButton();
            rdb_7 = new RadioButton();
            rdb_6 = new RadioButton();
            vacant_room_list = new Panel();
            vacant_room_lbl = new Label();
            lbl_Hi_10 = new Label();
            lbl_Hi_8 = new Label();
            lbl_Hi_7 = new Label();
            lbl_Hi_6 = new Label();
            lbl_Hi_5 = new Label();
            hierarchy_change.SuspendLayout();
            vacant_room_list.SuspendLayout();
            SuspendLayout();
            // 
            // hierarchy_change
            // 
            hierarchy_change.BackColor = Color.White;
            hierarchy_change.BorderStyle = BorderStyle.FixedSingle;
            hierarchy_change.Controls.Add(rdb_10);
            hierarchy_change.Controls.Add(rdb_5);
            hierarchy_change.Controls.Add(rdb_8);
            hierarchy_change.Controls.Add(rdb_7);
            hierarchy_change.Controls.Add(rdb_6);
            hierarchy_change.Location = new Point(2, 0);
            hierarchy_change.Name = "hierarchy_change";
            hierarchy_change.Size = new Size(689, 41);
            hierarchy_change.TabIndex = 0;
            // 
            // rdb_10
            // 
            rdb_10.Location = new Point(296, 3);
            rdb_10.Name = "rdb_10";
            rdb_10.Size = new Size(77, 35);
            rdb_10.TabIndex = 4;
            rdb_10.TabStop = true;
            rdb_10.Text = "10階";
            rdb_10.UseVisualStyleBackColor = true;
            // 
            // rdb_5
            // 
            rdb_5.Location = new Point(3, 3);
            rdb_5.Name = "rdb_5";
            rdb_5.Size = new Size(71, 35);
            rdb_5.TabIndex = 0;
            rdb_5.TabStop = true;
            rdb_5.Text = "5階";
            rdb_5.UseVisualStyleBackColor = true;
            // 
            // rdb_8
            // 
            rdb_8.Location = new Point(234, 3);
            rdb_8.Name = "rdb_8";
            rdb_8.Size = new Size(71, 35);
            rdb_8.TabIndex = 2;
            rdb_8.TabStop = true;
            rdb_8.Text = "8階";
            rdb_8.UseVisualStyleBackColor = true;
            // 
            // rdb_7
            // 
            rdb_7.Location = new Point(157, 3);
            rdb_7.Name = "rdb_7";
            rdb_7.Size = new Size(71, 35);
            rdb_7.TabIndex = 3;
            rdb_7.TabStop = true;
            rdb_7.Text = "7階";
            rdb_7.UseVisualStyleBackColor = true;
            // 
            // rdb_6
            // 
            rdb_6.Location = new Point(80, 3);
            rdb_6.Name = "rdb_6";
            rdb_6.Size = new Size(71, 35);
            rdb_6.TabIndex = 1;
            rdb_6.TabStop = true;
            rdb_6.Text = "6階";
            rdb_6.UseVisualStyleBackColor = true;
            // 
            // vacant_room_list
            // 
            vacant_room_list.BackColor = SystemColors.ActiveBorder;
            vacant_room_list.BorderStyle = BorderStyle.FixedSingle;
            vacant_room_list.Controls.Add(vacant_room_lbl);
            vacant_room_list.Controls.Add(lbl_Hi_10);
            vacant_room_list.Controls.Add(lbl_Hi_8);
            vacant_room_list.Controls.Add(lbl_Hi_7);
            vacant_room_list.Controls.Add(lbl_Hi_6);
            vacant_room_list.Controls.Add(lbl_Hi_5);
            vacant_room_list.ForeColor = SystemColors.ControlText;
            vacant_room_list.Location = new Point(690, 0);
            vacant_room_list.Name = "vacant_room_list";
            vacant_room_list.Size = new Size(178, 315);
            vacant_room_list.TabIndex = 1;
            // 
            // vacant_room_lbl
            // 
            vacant_room_lbl.BackColor = SystemColors.ActiveCaption;
            vacant_room_lbl.BorderStyle = BorderStyle.FixedSingle;
            vacant_room_lbl.Font = new Font("Yu Gothic UI", 10F);
            vacant_room_lbl.Location = new Point(0, 0);
            vacant_room_lbl.Name = "vacant_room_lbl";
            vacant_room_lbl.Size = new Size(178, 47);
            vacant_room_lbl.TabIndex = 2;
            vacant_room_lbl.Text = "部屋状況";
            vacant_room_lbl.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbl_Hi_10
            // 
            lbl_Hi_10.BackColor = Color.White;
            lbl_Hi_10.Location = new Point(23, 270);
            lbl_Hi_10.Name = "lbl_Hi_10";
            lbl_Hi_10.Size = new Size(132, 28);
            lbl_Hi_10.TabIndex = 6;
            lbl_Hi_10.Text = "10階：";
            // 
            // lbl_Hi_8
            // 
            lbl_Hi_8.BackColor = Color.White;
            lbl_Hi_8.Location = new Point(23, 219);
            lbl_Hi_8.Name = "lbl_Hi_8";
            lbl_Hi_8.Size = new Size(132, 28);
            lbl_Hi_8.TabIndex = 5;
            lbl_Hi_8.Text = "  8階：";
            // 
            // lbl_Hi_7
            // 
            lbl_Hi_7.BackColor = Color.White;
            lbl_Hi_7.Location = new Point(23, 167);
            lbl_Hi_7.Name = "lbl_Hi_7";
            lbl_Hi_7.Size = new Size(132, 28);
            lbl_Hi_7.TabIndex = 4;
            lbl_Hi_7.Text = "  7階：";
            // 
            // lbl_Hi_6
            // 
            lbl_Hi_6.BackColor = Color.White;
            lbl_Hi_6.Location = new Point(23, 117);
            lbl_Hi_6.Name = "lbl_Hi_6";
            lbl_Hi_6.Size = new Size(132, 28);
            lbl_Hi_6.TabIndex = 3;
            lbl_Hi_6.Text = "  6階：";
            // 
            // lbl_Hi_5
            // 
            lbl_Hi_5.BackColor = Color.White;
            lbl_Hi_5.BorderStyle = BorderStyle.Fixed3D;
            lbl_Hi_5.FlatStyle = FlatStyle.System;
            lbl_Hi_5.Location = new Point(23, 65);
            lbl_Hi_5.Name = "lbl_Hi_5";
            lbl_Hi_5.Size = new Size(132, 28);
            lbl_Hi_5.TabIndex = 2;
            lbl_Hi_5.Text = "  5階：";
            // 
            // HotelForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(869, 544);
            Controls.Add(vacant_room_list);
            Controls.Add(hierarchy_change);
            Name = "HotelForm";
            Text = "HotelForm";
            hierarchy_change.ResumeLayout(false);
            vacant_room_list.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel hierarchy_change;
        private RadioButton rdb_5;
        private RadioButton rdb_8;
        private RadioButton rdb_7;
        private RadioButton rdb_6;
        private RadioButton rdb_10;
        private Panel vacant_room_list;
        private Label lbl_Hi_10;
        private Label lbl_Hi_8;
        private Label lbl_Hi_7;
        private Label lbl_Hi_6;
        private Label lbl_Hi_5;
        private Label vacant_room_lbl;
    }
}