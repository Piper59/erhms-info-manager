
namespace ERHMS.Desktop.Utilities
{
    partial class UtilityDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UtilityDialog));
            this.LeadLabel = new System.Windows.Forms.Label();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.BodyLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LeadLabel
            // 
            this.LeadLabel.AutoSize = true;
            this.LeadLabel.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LeadLabel.Location = new System.Drawing.Point(20, 20);
            this.LeadLabel.Margin = new System.Windows.Forms.Padding(5);
            this.LeadLabel.Name = "LeadLabel";
            this.LeadLabel.Size = new System.Drawing.Size(99, 28);
            this.LeadLabel.TabIndex = 0;
            this.LeadLabel.Text = "Working...";
            this.LeadLabel.UseWaitCursor = true;
            // 
            // ProgressBar
            // 
            this.ProgressBar.Location = new System.Drawing.Point(20, 58);
            this.ProgressBar.Margin = new System.Windows.Forms.Padding(5);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(424, 25);
            this.ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.ProgressBar.TabIndex = 1;
            this.ProgressBar.UseWaitCursor = true;
            // 
            // BodyLabel
            // 
            this.BodyLabel.AutoSize = true;
            this.BodyLabel.Location = new System.Drawing.Point(20, 93);
            this.BodyLabel.Margin = new System.Windows.Forms.Padding(5);
            this.BodyLabel.MaximumSize = new System.Drawing.Size(424, 0);
            this.BodyLabel.Name = "BodyLabel";
            this.BodyLabel.Size = new System.Drawing.Size(0, 21);
            this.BodyLabel.TabIndex = 2;
            this.BodyLabel.UseWaitCursor = true;
            // 
            // UtilityDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(464, 134);
            this.Controls.Add(this.BodyLabel);
            this.Controls.Add(this.ProgressBar);
            this.Controls.Add(this.LeadLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UtilityDialog";
            this.Padding = new System.Windows.Forms.Padding(15);
            this.Text = "ERHMS Info Manager";
            this.UseWaitCursor = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LeadLabel;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.Label BodyLabel;
    }
}