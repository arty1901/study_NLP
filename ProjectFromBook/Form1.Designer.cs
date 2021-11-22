namespace ProjectFromBook
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.InputTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BigramTextBox = new System.Windows.Forms.TextBox();
            this.freqButton = new System.Windows.Forms.Button();
            this.bigramValueButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.openFileButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.морфАнализToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileAnalysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // InputTextBox
            // 
            this.InputTextBox.Location = new System.Drawing.Point(12, 60);
            this.InputTextBox.Multiline = true;
            this.InputTextBox.Name = "InputTextBox";
            this.InputTextBox.Size = new System.Drawing.Size(589, 94);
            this.InputTextBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(186, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(252, 30);
            this.label1.TabIndex = 1;
            this.label1.Text = "Обрабатываемый текст";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(186, 157);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(240, 30);
            this.label2.TabIndex = 3;
            this.label2.Text = "Биграмма для анализа";
            // 
            // BigramTextBox
            // 
            this.BigramTextBox.Location = new System.Drawing.Point(12, 190);
            this.BigramTextBox.Name = "BigramTextBox";
            this.BigramTextBox.Size = new System.Drawing.Size(589, 23);
            this.BigramTextBox.TabIndex = 2;
            // 
            // freqButton
            // 
            this.freqButton.Location = new System.Drawing.Point(11, 219);
            this.freqButton.Name = "freqButton";
            this.freqButton.Size = new System.Drawing.Size(140, 23);
            this.freqButton.TabIndex = 4;
            this.freqButton.Text = "Частотный словарь";
            this.freqButton.UseVisualStyleBackColor = true;
            this.freqButton.Click += new System.EventHandler(this.freqButton_Click);
            // 
            // bigramValueButton
            // 
            this.bigramValueButton.Location = new System.Drawing.Point(171, 219);
            this.bigramValueButton.Name = "bigramValueButton";
            this.bigramValueButton.Size = new System.Drawing.Size(202, 23);
            this.bigramValueButton.TabIndex = 5;
            this.bigramValueButton.Text = "Вывод о значимости биграммы";
            this.bigramValueButton.UseVisualStyleBackColor = true;
            this.bigramValueButton.Click += new System.EventHandler(this.bigramValueButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(186, 245);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(248, 30);
            this.label3.TabIndex = 7;
            this.label3.Text = "Выходная информация";
            // 
            // outputTextBox
            // 
            this.outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.outputTextBox.Location = new System.Drawing.Point(12, 278);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.outputTextBox.Size = new System.Drawing.Size(589, 241);
            this.outputTextBox.TabIndex = 6;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(607, 27);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(480, 492);
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // openFileButton
            // 
            this.openFileButton.Location = new System.Drawing.Point(398, 219);
            this.openFileButton.Name = "openFileButton";
            this.openFileButton.Size = new System.Drawing.Size(202, 23);
            this.openFileButton.TabIndex = 9;
            this.openFileButton.Text = "Открыть файл";
            this.openFileButton.UseVisualStyleBackColor = true;
            this.openFileButton.Click += new System.EventHandler(this.openFileButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.морфАнализToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1099, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // морфАнализToolStripMenuItem
            // 
            this.морфАнализToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileAnalysToolStripMenuItem});
            this.морфАнализToolStripMenuItem.Name = "морфАнализToolStripMenuItem";
            this.морфАнализToolStripMenuItem.Size = new System.Drawing.Size(94, 20);
            this.морфАнализToolStripMenuItem.Text = "Морф анализ";
            // 
            // fileAnalysToolStripMenuItem
            // 
            this.fileAnalysToolStripMenuItem.Name = "fileAnalysToolStripMenuItem";
            this.fileAnalysToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.fileAnalysToolStripMenuItem.Text = "Анализ файла";
            this.fileAnalysToolStripMenuItem.Click += new System.EventHandler(this.fileAnalysToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1099, 531);
            this.Controls.Add(this.openFileButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.outputTextBox);
            this.Controls.Add(this.bigramValueButton);
            this.Controls.Add(this.freqButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.BigramTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.InputTextBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox InputTextBox;
        private Label label1;
        private Label label2;
        private TextBox BigramTextBox;
        private Button freqButton;
        private Button bigramValueButton;
        private Label label3;
        private TextBox outputTextBox;
        private PictureBox pictureBox1;
        private Button openFileButton;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem морфАнализToolStripMenuItem;
        private ToolStripMenuItem fileAnalysToolStripMenuItem;
    }
}