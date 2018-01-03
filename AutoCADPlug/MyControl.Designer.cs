namespace AutoCADPlug
{
    partial class MyControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.nudBeamHeight = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.nudBeamTopHeight = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.lblPointValue = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBeamHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBeamTopHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.lblPointValue);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.nudBeamHeight);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.nudBeamTopHeight);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(209, 221);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "底图梁预处理";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(119, 192);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "转化已有图";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 192);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "绘制";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // nudBeamHeight
            // 
            this.nudBeamHeight.DecimalPlaces = 2;
            this.nudBeamHeight.Location = new System.Drawing.Point(74, 68);
            this.nudBeamHeight.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudBeamHeight.Name = "nudBeamHeight";
            this.nudBeamHeight.Size = new System.Drawing.Size(120, 21);
            this.nudBeamHeight.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "梁的高度";
            // 
            // nudBeamTopHeight
            // 
            this.nudBeamTopHeight.DecimalPlaces = 2;
            this.nudBeamTopHeight.Location = new System.Drawing.Point(74, 27);
            this.nudBeamTopHeight.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudBeamTopHeight.Name = "nudBeamTopHeight";
            this.nudBeamTopHeight.Size = new System.Drawing.Size(120, 21);
            this.nudBeamTopHeight.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "梁顶高程";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(10, 149);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(184, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "选取绘制基点";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // lblPointValue
            // 
            this.lblPointValue.Location = new System.Drawing.Point(72, 114);
            this.lblPointValue.Name = "lblPointValue";
            this.lblPointValue.Size = new System.Drawing.Size(122, 23);
            this.lblPointValue.TabIndex = 7;
            this.lblPointValue.Text = "label3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "基点坐标";
            // 
            // MyControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "MyControl";
            this.Size = new System.Drawing.Size(215, 606);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBeamHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBeamTopHeight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NumericUpDown nudBeamHeight;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudBeamTopHeight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblPointValue;
        private System.Windows.Forms.Button button3;
    }
}
