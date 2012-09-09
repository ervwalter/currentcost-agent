namespace CurrentCostAgent
{
    partial class StatusForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatusForm));
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelOverall = new System.Windows.Forms.Label();
            this.labelPendingUpload = new System.Windows.Forms.Label();
            this.labelLastUpload = new System.Windows.Forms.Label();
            this.listViewAppliances = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.Location = new System.Drawing.Point(14, 13);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(86, 25);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Overall:";
            // 
            // labelOverall
            // 
            this.labelOverall.AutoSize = true;
            this.labelOverall.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOverall.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelOverall.Location = new System.Drawing.Point(12, 40);
            this.labelOverall.Name = "labelOverall";
            this.labelOverall.Size = new System.Drawing.Size(72, 55);
            this.labelOverall.TabIndex = 0;
            this.labelOverall.Text = "---";
            // 
            // labelPendingUpload
            // 
            this.labelPendingUpload.AutoSize = true;
            this.labelPendingUpload.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPendingUpload.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.labelPendingUpload.Location = new System.Drawing.Point(13, 258);
            this.labelPendingUpload.Name = "labelPendingUpload";
            this.labelPendingUpload.Size = new System.Drawing.Size(105, 13);
            this.labelPendingUpload.TabIndex = 1;
            this.labelPendingUpload.Text = "No uploads pending.";
            // 
            // labelLastUpload
            // 
            this.labelLastUpload.AutoSize = true;
            this.labelLastUpload.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLastUpload.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.labelLastUpload.Location = new System.Drawing.Point(14, 276);
            this.labelLastUpload.Name = "labelLastUpload";
            this.labelLastUpload.Size = new System.Drawing.Size(98, 13);
            this.labelLastUpload.TabIndex = 1;
            this.labelLastUpload.Text = "Last upload: never.";
            // 
            // listViewAppliances
            // 
            this.listViewAppliances.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewAppliances.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewAppliances.Location = new System.Drawing.Point(19, 106);
            this.listViewAppliances.MultiSelect = false;
            this.listViewAppliances.Name = "listViewAppliances";
            this.listViewAppliances.Size = new System.Drawing.Size(216, 139);
            this.listViewAppliances.TabIndex = 2;
            this.listViewAppliances.UseCompatibleStateImageBehavior = false;
            this.listViewAppliances.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Appliance";
            this.columnHeader1.Width = 81;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Usage";
            this.columnHeader2.Width = 108;
            // 
            // StatusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(247, 306);
            this.Controls.Add(this.listViewAppliances);
            this.Controls.Add(this.labelLastUpload);
            this.Controls.Add(this.labelPendingUpload);
            this.Controls.Add(this.labelOverall);
            this.Controls.Add(this.labelTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StatusForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Current Status";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StatusForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelOverall;
        private System.Windows.Forms.Label labelPendingUpload;
        private System.Windows.Forms.Label labelLastUpload;
        private System.Windows.Forms.ListView listViewAppliances;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}