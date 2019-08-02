namespace ant.csscript.editor
{
    partial class AntCsScriptEditor
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.textEditorControl1 = new ICSharpCode.TextEditor.TextEditorControl();
            this.SuspendLayout();
            // 
            // textEditorControl1
            // 
            this.textEditorControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEditorControl1.IsReadOnly = false;
            this.textEditorControl1.Location = new System.Drawing.Point(0, 0);
            this.textEditorControl1.Name = "textEditorControl1";
            this.textEditorControl1.ShowHRuler = true;
            this.textEditorControl1.ShowInvalidLines = true;
            this.textEditorControl1.ShowSpaces = true;
            this.textEditorControl1.ShowTabs = true;
            this.textEditorControl1.Size = new System.Drawing.Size(515, 199);
            this.textEditorControl1.TabIndex = 3;
            this.textEditorControl1.Text = "textEditorControl1";
            // 
            // AntCsScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.textEditorControl1);
            this.Name = "AntCsScriptEditor";
            this.Size = new System.Drawing.Size(515, 199);
            this.ResumeLayout(false);

        }

        #endregion

        private ICSharpCode.TextEditor.TextEditorControl textEditorControl1;
    }
}
