namespace AztecQR
{
    partial class MainEMVReaderBin
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
            this.groupInput = new System.Windows.Forms.GroupBox();
            this.textInputString = new System.Windows.Forms.TextBox();
            this.labelInput = new System.Windows.Forms.Label();
            this.groupOptions = new System.Windows.Forms.GroupBox();
            this.numericErrorCorrection = new System.Windows.Forms.NumericUpDown();
            this.labelErrorCorrection = new System.Windows.Forms.Label();
            this.numericSize = new System.Windows.Forms.NumericUpDown();
            this.labelSize = new System.Windows.Forms.Label();
            this.radioAztec = new System.Windows.Forms.RadioButton();
            this.radioQR = new System.Windows.Forms.RadioButton();
            this.buttonGenerate = new System.Windows.Forms.Button();
            this.buttonSaveAs = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.groupPreview = new System.Windows.Forms.GroupBox();
            this.picturePreview = new System.Windows.Forms.PictureBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupInput.SuspendLayout();
            this.groupOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericErrorCorrection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSize)).BeginInit();
            this.groupPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picturePreview)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupInput
            // 
            this.groupInput.Controls.Add(this.textInputString);
            this.groupInput.Controls.Add(this.labelInput);
            this.groupInput.Location = new System.Drawing.Point(12, 12);
            this.groupInput.Name = "groupInput";
            this.groupInput.Size = new System.Drawing.Size(760, 100);
            this.groupInput.TabIndex = 0;
            this.groupInput.TabStop = false;
            this.groupInput.Text = "Input Data";
            // 
            // textInputString
            // 
            this.textInputString.Location = new System.Drawing.Point(120, 32);
            this.textInputString.Multiline = true;
            this.textInputString.Name = "textInputString";
            this.textInputString.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textInputString.Size = new System.Drawing.Size(620, 55);
            this.textInputString.TabIndex = 1;
            // 
            // labelInput
            // 
            this.labelInput.AutoSize = true;
            this.labelInput.Location = new System.Drawing.Point(15, 35);
            this.labelInput.Name = "labelInput";
            this.labelInput.Size = new System.Drawing.Size(99, 20);
            this.labelInput.TabIndex = 0;
            this.labelInput.Text = "Base64 String:";
            // 
            // groupOptions
            // 
            this.groupOptions.Controls.Add(this.numericErrorCorrection);
            this.groupOptions.Controls.Add(this.labelErrorCorrection);
            this.groupOptions.Controls.Add(this.numericSize);
            this.groupOptions.Controls.Add(this.labelSize);
            this.groupOptions.Controls.Add(this.radioAztec);
            this.groupOptions.Controls.Add(this.radioQR);
            this.groupOptions.Location = new System.Drawing.Point(12, 118);
            this.groupOptions.Name = "groupOptions";
            this.groupOptions.Size = new System.Drawing.Size(300, 180);
            this.groupOptions.TabIndex = 1;
            this.groupOptions.TabStop = false;
            this.groupOptions.Text = "Options";
            // 
            // numericErrorCorrection
            // 
            this.numericErrorCorrection.Location = new System.Drawing.Point(150, 135);
            this.numericErrorCorrection.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            this.numericErrorCorrection.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numericErrorCorrection.Name = "numericErrorCorrection";
            this.numericErrorCorrection.Size = new System.Drawing.Size(120, 27);
            this.numericErrorCorrection.TabIndex = 5;
            this.numericErrorCorrection.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // labelErrorCorrection
            // 
            this.labelErrorCorrection.AutoSize = true;
            this.labelErrorCorrection.Location = new System.Drawing.Point(15, 137);
            this.labelErrorCorrection.Name = "labelErrorCorrection";
            this.labelErrorCorrection.Size = new System.Drawing.Size(129, 20);
            this.labelErrorCorrection.TabIndex = 4;
            this.labelErrorCorrection.Text = "Error Correction:";
            // 
            // numericSize
            // 
            this.numericSize.Increment = new decimal(new int[] { 50, 0, 0, 0 });
            this.numericSize.Location = new System.Drawing.Point(150, 95);
            this.numericSize.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numericSize.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            this.numericSize.Name = "numericSize";
            this.numericSize.Size = new System.Drawing.Size(120, 27);
            this.numericSize.TabIndex = 3;
            this.numericSize.Value = new decimal(new int[] { 300, 0, 0, 0 });
            // 
            // labelSize
            // 
            this.labelSize.AutoSize = true;
            this.labelSize.Location = new System.Drawing.Point(15, 97);
            this.labelSize.Name = "labelSize";
            this.labelSize.Size = new System.Drawing.Size(102, 20);
            this.labelSize.TabIndex = 2;
            this.labelSize.Text = "Size (pixels):";
            // 
            // radioAztec
            // 
            this.radioAztec.AutoSize = true;
            this.radioAztec.Location = new System.Drawing.Point(150, 55);
            this.radioAztec.Name = "radioAztec";
            this.radioAztec.Size = new System.Drawing.Size(110, 24);
            this.radioAztec.TabIndex = 1;
            this.radioAztec.Text = "Aztec Code";
            this.radioAztec.UseVisualStyleBackColor = true;
            // 
            // radioQR
            // 
            this.radioQR.AutoSize = true;
            this.radioQR.Checked = true;
            this.radioQR.Location = new System.Drawing.Point(15, 55);
            this.radioQR.Name = "radioQR";
            this.radioQR.Size = new System.Drawing.Size(90, 24);
            this.radioQR.TabIndex = 0;
            this.radioQR.TabStop = true;
            this.radioQR.Text = "QR Code";
            this.radioQR.UseVisualStyleBackColor = true;
            // 
            // buttonGenerate
            // 
            this.buttonGenerate.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.buttonGenerate.Location = new System.Drawing.Point(12, 315);
            this.buttonGenerate.Name = "buttonGenerate";
            this.buttonGenerate.Size = new System.Drawing.Size(140, 45);
            this.buttonGenerate.TabIndex = 2;
            this.buttonGenerate.Text = "Generate";
            this.buttonGenerate.UseVisualStyleBackColor = true;
            this.buttonGenerate.Click += new System.EventHandler(this.buttonGenerate_Click);
            // 
            // buttonSaveAs
            // 
            this.buttonSaveAs.Enabled = false;
            this.buttonSaveAs.Location = new System.Drawing.Point(158, 315);
            this.buttonSaveAs.Name = "buttonSaveAs";
            this.buttonSaveAs.Size = new System.Drawing.Size(140, 45);
            this.buttonSaveAs.TabIndex = 3;
            this.buttonSaveAs.Text = "Save As...";
            this.buttonSaveAs.UseVisualStyleBackColor = true;
            this.buttonSaveAs.Click += new System.EventHandler(this.buttonSaveAs_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(12, 370);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(140, 35);
            this.buttonClear.TabIndex = 4;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // groupPreview
            // 
            this.groupPreview.Controls.Add(this.picturePreview);
            this.groupPreview.Location = new System.Drawing.Point(318, 118);
            this.groupPreview.Name = "groupPreview";
            this.groupPreview.Size = new System.Drawing.Size(454, 480);
            this.groupPreview.TabIndex = 5;
            this.groupPreview.TabStop = false;
            this.groupPreview.Text = "Preview";
            // 
            // picturePreview
            // 
            this.picturePreview.BackColor = System.Drawing.Color.White;
            this.picturePreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picturePreview.Location = new System.Drawing.Point(15, 32);
            this.picturePreview.Name = "picturePreview";
            this.picturePreview.Size = new System.Drawing.Size(420, 430);
            this.picturePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picturePreview.TabIndex = 0;
            this.picturePreview.TabStop = false;
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.statusLabel });
            this.statusStrip.Location = new System.Drawing.Point(0, 608);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(784, 26);
            this.statusStrip.TabIndex = 6;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(50, 20);
            this.statusLabel.Text = "Ready";
            // 
            // MainEMVReaderBin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 634);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.groupPreview);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.buttonSaveAs);
            this.Controls.Add(this.buttonGenerate);
            this.Controls.Add(this.groupOptions);
            this.Controls.Add(this.groupInput);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainEMVReaderBin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Aztec & QR Code Generator";
            this.groupInput.ResumeLayout(false);
            this.groupInput.PerformLayout();
            this.groupOptions.ResumeLayout(false);
            this.groupOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericErrorCorrection)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSize)).EndInit();
            this.groupPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picturePreview)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox groupInput;
        private System.Windows.Forms.TextBox textInputString;
        private System.Windows.Forms.Label labelInput;
        private System.Windows.Forms.GroupBox groupOptions;
        private System.Windows.Forms.NumericUpDown numericErrorCorrection;
        private System.Windows.Forms.Label labelErrorCorrection;
        private System.Windows.Forms.NumericUpDown numericSize;
        private System.Windows.Forms.Label labelSize;
        private System.Windows.Forms.RadioButton radioAztec;
        private System.Windows.Forms.RadioButton radioQR;
        private System.Windows.Forms.Button buttonGenerate;
        private System.Windows.Forms.Button buttonSaveAs;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.GroupBox groupPreview;
        private System.Windows.Forms.PictureBox picturePreview;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
    }
}

