namespace OOProjectBasedLeaning
{
    partial class GuestCreatorForm
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
            button1 = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(430, 12);
            button1.Name = "button1";
            button1.Size = new Size(89, 31);
            button1.TabIndex = 0;
            button1.Text = "ゲストの作成";
            button1.UseVisualStyleBackColor = true;
            button1.Click += CreateGuestEvent;
            // 
            // GuestCreatorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(531, 314);
            Controls.Add(button1);
            Name = "GuestCreatorForm";
            Text = "GuestCreatorForm";
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
    }
}