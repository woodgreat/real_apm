namespace WindowsForms_showAPM
{
    partial class FormMain
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
            if ( disposing && ( components != null ) )
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
      System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
      this.notifyIconTaskbar = new System.Windows.Forms.NotifyIcon(this.components);
      this.labelTipApm = new System.Windows.Forms.Label();
      this.labelTextApm = new System.Windows.Forms.Label();
      this.buttonExit = new System.Windows.Forms.Button();
      this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
      ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
      this.SuspendLayout();
      // 
      // notifyIconTaskbar
      // 
      this.notifyIconTaskbar.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconTaskbar.Icon")));
      this.notifyIconTaskbar.Text = "notifyIconText";
      this.notifyIconTaskbar.Visible = true;
      this.notifyIconTaskbar.DoubleClick += new System.EventHandler(this.notifyIconTaskbar_DoubleClick);
      // 
      // labelTipApm
      // 
      this.labelTipApm.AutoSize = true;
      this.labelTipApm.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
      this.labelTipApm.ForeColor = System.Drawing.Color.DarkMagenta;
      this.labelTipApm.Location = new System.Drawing.Point(38, 22);
      this.labelTipApm.Name = "labelTipApm";
      this.labelTipApm.Size = new System.Drawing.Size(174, 20);
      this.labelTipApm.TabIndex = 0;
      this.labelTipApm.Text = "Realtime APM : ";
      // 
      // labelTextApm
      // 
      this.labelTextApm.AutoSize = true;
      this.labelTextApm.Font = new System.Drawing.Font("宋体", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
      this.labelTextApm.ForeColor = System.Drawing.Color.RoyalBlue;
      this.labelTextApm.Location = new System.Drawing.Point(212, 14);
      this.labelTextApm.Name = "labelTextApm";
      this.labelTextApm.Size = new System.Drawing.Size(32, 33);
      this.labelTextApm.TabIndex = 1;
      this.labelTextApm.Text = "0";
      // 
      // buttonExit
      // 
      this.buttonExit.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
      this.buttonExit.Location = new System.Drawing.Point(169, 258);
      this.buttonExit.Name = "buttonExit";
      this.buttonExit.Size = new System.Drawing.Size(75, 23);
      this.buttonExit.TabIndex = 2;
      this.buttonExit.Text = "Exit";
      this.buttonExit.UseVisualStyleBackColor = true;
      this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
      // 
      // chart1
      // 
      chartArea1.Name = "ChartArea1";
      this.chart1.ChartAreas.Add(chartArea1);
      legend1.Name = "Legend1";
      this.chart1.Legends.Add(legend1);
      this.chart1.Location = new System.Drawing.Point(42, 76);
      this.chart1.Name = "chart1";
      series1.ChartArea = "ChartArea1";
      series1.Legend = "Legend1";
      series1.Name = "Series1";
      this.chart1.Series.Add(series1);
      this.chart1.Size = new System.Drawing.Size(281, 125);
      this.chart1.TabIndex = 3;
      this.chart1.Text = "chart1";
      // 
      // FormMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(395, 293);
      this.Controls.Add(this.chart1);
      this.Controls.Add(this.buttonExit);
      this.Controls.Add(this.labelTextApm);
      this.Controls.Add(this.labelTipApm);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.Name = "FormMain";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Real APM";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
      this.Load += new System.EventHandler(this.FormMain_Load);
      this.DoubleClick += new System.EventHandler(this.FormMain_DoubleClick);
      ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIconTaskbar;
        private System.Windows.Forms.Label labelTipApm;
        private System.Windows.Forms.Label labelTextApm;
        private System.Windows.Forms.Button buttonExit;
    private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
  }
}

