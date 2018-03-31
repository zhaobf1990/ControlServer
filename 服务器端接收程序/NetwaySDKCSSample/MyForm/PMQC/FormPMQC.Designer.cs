namespace 服务器端接收程序.MyForm.PMQC
{
    partial class FormPMQC
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lb_msg = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lb_msg
            // 
            this.lb_msg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_msg.FormattingEnabled = true;
            this.lb_msg.ItemHeight = 12;
            this.lb_msg.Location = new System.Drawing.Point(0, 0);
            this.lb_msg.Name = "lb_msg";
            this.lb_msg.Size = new System.Drawing.Size(645, 488);
            this.lb_msg.TabIndex = 1;
            // 
            // FormPMQC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lb_msg);
            this.Name = "FormPMQC";
            this.Size = new System.Drawing.Size(645, 488);
            this.Load += new System.EventHandler(this.FormPMQC_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lb_msg;

    }
}
