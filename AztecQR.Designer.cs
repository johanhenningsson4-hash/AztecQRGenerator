namespace EMVCard
{
    partial class MainEMVReaderBin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;



        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainEMVReaderBin));
            this.buttonRun = new System.Windows.Forms.Button();
            this.labelInput = new System.Windows.Forms.Label();
            this.textInputString = new System.Windows.Forms.TextBox();
            this.pictureAztecQR = new System.Windows.Forms.PictureBox();
            this.checkOptionAztecQR = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureAztecQR)).BeginInit();

            // 
            // buttonRun
            // 
            this.buttonRun.Location = new System.Drawing.Point(16, 80);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(136, 36);
            this.buttonRun.TabIndex = 0;
            this.buttonRun.Text = "Generate";
            this.buttonRun.UseVisualStyleBackColor = true;
            // 
            // labelInput
            // 
            this.labelInput.AutoSize = true;
            this.labelInput.Location = new System.Drawing.Point(12, 9);
            this.labelInput.Name = "labelInput";
            this.labelInput.Size = new System.Drawing.Size(121, 20);
            this.labelInput.TabIndex = 2;
            this.labelInput.Text = "Input string:";
            // 
            // textInputString
            // 
            this.textInputString.Location = new System.Drawing.Point(149, 2);
            this.textInputString.Name = "textInputString";
            this.textInputString.Size = new System.Drawing.Size(100, 27);
            this.textInputString.TabIndex = 3;
            this.textInputString.Text = "Input";
            // 
            // pictureAztecQR
            // 
            this.pictureAztecQR.Location = new System.Drawing.Point(255, 9);
            this.pictureAztecQR.Name = "pictureAztecQR";
            this.pictureAztecQR.Size = new System.Drawing.Size(499, 458);
            this.pictureAztecQR.TabIndex = 4;
            this.pictureAztecQR.TabStop = false;
            // 
            // checkOptionAztecQR
            // 
            this.checkOptionAztecQR.AutoSize = true;
            this.checkOptionAztecQR.Location = new System.Drawing.Point(16, 50);
            this.checkOptionAztecQR.Name = "checkOptionAztecQR";
            this.checkOptionAztecQR.Size = new System.Drawing.Size(78, 24);
            this.checkOptionAztecQR.TabIndex = 5;
            this.checkOptionAztecQR.Text = "Aztec";

        }

        #endregion

        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.Label labelInput;
        private System.Windows.Forms.TextBox textInputString;
        private System.Windows.Forms.PictureBox pictureAztecQR;
        private System.Windows.Forms.CheckBox checkOptionAztecQR;
    }
}

