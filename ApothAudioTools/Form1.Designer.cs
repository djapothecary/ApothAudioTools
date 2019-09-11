namespace ApothAudioTools
{
    partial class Form1
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
            this.lblUrl = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxUrl = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnDownload = new System.Windows.Forms.Button();
            this.lblPercentage = new System.Windows.Forms.Label();
            this.tbxLinkList = new System.Windows.Forms.TextBox();
            this.cbxAddList = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblUrl
            // 
            this.lblUrl.AutoSize = true;
            this.lblUrl.Location = new System.Drawing.Point(77, 46);
            this.lblUrl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUrl.Name = "lblUrl";
            this.lblUrl.Size = new System.Drawing.Size(30, 17);
            this.lblUrl.TabIndex = 0;
            this.lblUrl.Text = "Url:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 85);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Progress:";
            // 
            // tbxUrl
            // 
            this.tbxUrl.Location = new System.Drawing.Point(116, 37);
            this.tbxUrl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbxUrl.Name = "tbxUrl";
            this.tbxUrl.Size = new System.Drawing.Size(879, 22);
            this.tbxUrl.TabIndex = 3;
            // 
            // progressBar
            // 
            this.progressBar.ForeColor = System.Drawing.Color.Lime;
            this.progressBar.Location = new System.Drawing.Point(116, 80);
            this.progressBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(824, 21);
            this.progressBar.TabIndex = 5;
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(896, 164);
            this.btnDownload.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(100, 28);
            this.btnDownload.TabIndex = 6;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click_1);
            // 
            // lblPercentage
            // 
            this.lblPercentage.AutoSize = true;
            this.lblPercentage.Location = new System.Drawing.Point(949, 85);
            this.lblPercentage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPercentage.Name = "lblPercentage";
            this.lblPercentage.Size = new System.Drawing.Size(28, 17);
            this.lblPercentage.TabIndex = 7;
            this.lblPercentage.Text = "0%";
            // 
            // tbxLinkList
            // 
            this.tbxLinkList.Location = new System.Drawing.Point(47, 123);
            this.tbxLinkList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbxLinkList.MaxLength = 92767;
            this.tbxLinkList.Multiline = true;
            this.tbxLinkList.Name = "tbxLinkList";
            this.tbxLinkList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbxLinkList.Size = new System.Drawing.Size(819, 67);
            this.tbxLinkList.TabIndex = 10;
            this.tbxLinkList.Visible = false;
            // 
            // cbxAddList
            // 
            this.cbxAddList.AutoSize = true;
            this.cbxAddList.Location = new System.Drawing.Point(896, 123);
            this.cbxAddList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbxAddList.Name = "cbxAddList";
            this.cbxAddList.Size = new System.Drawing.Size(85, 21);
            this.cbxAddList.TabIndex = 11;
            this.cbxAddList.Text = "Add List:";
            this.cbxAddList.UseVisualStyleBackColor = true;
            this.cbxAddList.CheckedChanged += new System.EventHandler(this.cbxAddList_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 215);
            this.Controls.Add(this.cbxAddList);
            this.Controls.Add(this.tbxLinkList);
            this.Controls.Add(this.lblPercentage);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.tbxUrl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblUrl);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "Download Youtube Video";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblUrl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxUrl;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Label lblPercentage;
        private System.Windows.Forms.TextBox tbxLinkList;
        private System.Windows.Forms.CheckBox cbxAddList;
    }
}

