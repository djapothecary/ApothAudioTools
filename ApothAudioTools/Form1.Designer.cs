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
            this.lblUrl.Location = new System.Drawing.Point(58, 37);
            this.lblUrl.Name = "lblUrl";
            this.lblUrl.Size = new System.Drawing.Size(23, 13);
            this.lblUrl.TabIndex = 0;
            this.lblUrl.Text = "Url:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Progress:";
            // 
            // tbxUrl
            // 
            this.tbxUrl.Location = new System.Drawing.Point(87, 30);
            this.tbxUrl.Name = "tbxUrl";
            this.tbxUrl.Size = new System.Drawing.Size(660, 20);
            this.tbxUrl.TabIndex = 3;
            // 
            // progressBar
            // 
            this.progressBar.ForeColor = System.Drawing.Color.Lime;
            this.progressBar.Location = new System.Drawing.Point(87, 65);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(618, 17);
            this.progressBar.TabIndex = 5;
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(672, 133);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(75, 23);
            this.btnDownload.TabIndex = 6;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click_1);
            // 
            // lblPercentage
            // 
            this.lblPercentage.AutoSize = true;
            this.lblPercentage.Location = new System.Drawing.Point(712, 69);
            this.lblPercentage.Name = "lblPercentage";
            this.lblPercentage.Size = new System.Drawing.Size(21, 13);
            this.lblPercentage.TabIndex = 7;
            this.lblPercentage.Text = "0%";
            // 
            // tbxLinkList
            // 
            this.tbxLinkList.Location = new System.Drawing.Point(35, 100);
            this.tbxLinkList.Multiline = true;
            this.tbxLinkList.Name = "tbxLinkList";
            this.tbxLinkList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbxLinkList.Size = new System.Drawing.Size(615, 55);
            this.tbxLinkList.TabIndex = 10;
            this.tbxLinkList.Visible = false;
            // 
            // cbxAddList
            // 
            this.cbxAddList.AutoSize = true;
            this.cbxAddList.Location = new System.Drawing.Point(672, 100);
            this.cbxAddList.Name = "cbxAddList";
            this.cbxAddList.Size = new System.Drawing.Size(67, 17);
            this.cbxAddList.TabIndex = 11;
            this.cbxAddList.Text = "Add List:";
            this.cbxAddList.UseVisualStyleBackColor = true;
            this.cbxAddList.CheckedChanged += new System.EventHandler(this.cbxAddList_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 175);
            this.Controls.Add(this.cbxAddList);
            this.Controls.Add(this.tbxLinkList);
            this.Controls.Add(this.lblPercentage);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.tbxUrl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblUrl);
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

