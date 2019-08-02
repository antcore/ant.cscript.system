namespace ant.csscript.editor.avalonedit
{
    partial class Mainform
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.richTextBoxFileText = new System.Windows.Forms.RichTextBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.lblHandleFile = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BtnCompiler = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.richTextBoxMessage = new System.Windows.Forms.RichTextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(984, 281);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(976, 255);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "文件区域数据";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.richTextBoxFileText);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(976, 255);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "文件文本数据";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // richTextBoxFileText
            // 
            this.richTextBoxFileText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxFileText.Location = new System.Drawing.Point(3, 3);
            this.richTextBoxFileText.Name = "richTextBoxFileText";
            this.richTextBoxFileText.Size = new System.Drawing.Size(970, 249);
            this.richTextBoxFileText.TabIndex = 1;
            this.richTextBoxFileText.Text = "";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.richTextBoxMessage);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(976, 255);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "消息";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 45);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.elementHost1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.AutoScrollMargin = new System.Drawing.Size(0, 200);
            this.splitContainer2.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer2.Size = new System.Drawing.Size(984, 566);
            this.splitContainer2.SplitterDistance = 281;
            this.splitContainer2.TabIndex = 2;
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(4, 15);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(75, 23);
            this.btnSelectFile.TabIndex = 0;
            this.btnSelectFile.Text = "选择文件";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.BtnSelectFile_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(746, 9);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            36,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(45, 21);
            this.numericUpDown1.TabIndex = 2;
            this.numericUpDown1.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.NumericUpDown1_ValueChanged);
            // 
            // lblHandleFile
            // 
            this.lblHandleFile.AutoSize = true;
            this.lblHandleFile.Location = new System.Drawing.Point(85, 20);
            this.lblHandleFile.Name = "lblHandleFile";
            this.lblHandleFile.Size = new System.Drawing.Size(41, 12);
            this.lblHandleFile.TabIndex = 3;
            this.lblHandleFile.Text = "label1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblHandleFile);
            this.panel1.Controls.Add(this.numericUpDown1);
            this.panel1.Controls.Add(this.btnRun);
            this.panel1.Controls.Add(this.btnSelectFile);
            this.panel1.Controls.Add(this.BtnCompiler);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(984, 45);
            this.panel1.TabIndex = 1;
            // 
            // BtnCompiler
            // 
            this.BtnCompiler.Location = new System.Drawing.Point(816, 9);
            this.BtnCompiler.Name = "BtnCompiler";
            this.BtnCompiler.Size = new System.Drawing.Size(75, 23);
            this.BtnCompiler.TabIndex = 0;
            this.BtnCompiler.Text = "编 译";
            this.BtnCompiler.UseVisualStyleBackColor = true;
            this.BtnCompiler.Click += new System.EventHandler(this.BtnCompiler_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(897, 9);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 1;
            this.btnRun.Text = "执 行";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.BtnRun_Click);
            // 
            // elementHost1
            // 
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost1.Location = new System.Drawing.Point(0, 0);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(984, 281);
            this.elementHost1.TabIndex = 0;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = null;
            // 
            // richTextBoxMessage
            // 
            this.richTextBoxMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxMessage.Location = new System.Drawing.Point(3, 3);
            this.richTextBoxMessage.Name = "richTextBoxMessage";
            this.richTextBoxMessage.Size = new System.Drawing.Size(970, 249);
            this.richTextBoxMessage.TabIndex = 0;
            this.richTextBoxMessage.Text = "";
            // 
            // Mainform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 611);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.panel1);
            this.Name = "Mainform";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Mainform_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.RichTextBox richTextBoxMessage;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RichTextBox richTextBoxFileText;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label lblHandleFile;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button BtnCompiler;
    }
}

