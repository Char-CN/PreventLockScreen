using System;

namespace PreventLockScreen
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btn_on = new System.Windows.Forms.Button();
            this.msg = new System.Windows.Forms.Label();
            this.info = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_on
            // 
            this.btn_on.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btn_on.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_on.Location = new System.Drawing.Point(28, 31);
            this.btn_on.Name = "btn_on";
            this.btn_on.Size = new System.Drawing.Size(128, 34);
            this.btn_on.TabIndex = 0;
            this.btn_on.Text = "开启";
            this.btn_on.UseVisualStyleBackColor = false;
            this.btn_on.Click += new System.EventHandler(this.BtnOnToggle_Click);
            // 
            // msg
            // 
            this.msg.Font = new System.Drawing.Font("宋体", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.msg.Location = new System.Drawing.Point(38, 9);
            this.msg.Name = "msg";
            this.msg.Size = new System.Drawing.Size(106, 15);
            this.msg.TabIndex = 2;
            this.msg.Text = "状态：已关闭";
            this.msg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // info
            // 
            this.info.Font = new System.Drawing.Font("Constantia", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.info.ForeColor = System.Drawing.SystemColors.Highlight;
            this.info.Location = new System.Drawing.Point(-1, 72);
            this.info.Name = "info";
            this.info.Size = new System.Drawing.Size(188, 15);
            this.info.TabIndex = 3;
            this.info.Text = "BlazerHe@gmail.com";
            this.info.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(185, 97);
            this.Controls.Add(this.info);
            this.Controls.Add(this.msg);
            this.Controls.Add(this.btn_on);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "防锁屏";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        #endregion

        private System.Windows.Forms.Button btn_on;
        private System.Windows.Forms.Label msg;
        private System.Windows.Forms.Label info;
    }
}

