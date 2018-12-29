using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
/*
ALL SOURCE CODES OF THIS PROJECT IS UNDER LGPL 3.0
*/
namespace WindowsForms_showAPM
{
    public partial class FormMain : Form
    {
        private int totalAPM = 0;
        private int thisSecondsCounter = 0;
        private int oneCounter = 0;
        private int tenCounter = 0;
        private KeyboardHook k_hook;
        private MouseHook m_hook;
        private int[] apmSeconds;
        private int[] apmChartData;

        SynchronizationContext m_SyncContext = null;
        SynchronizationContext m_SyncChart = null;

        private static object actionLock = new object();

        private Series series1MinApmTotal = new Series("default");

        public FormMain()
        {
            InitializeComponent();
        }


        private void notifyIconTaskbar_DoubleClick(object sender,EventArgs e)
        {
            this.Visible = true;
            if ( this.WindowState == FormWindowState.Minimized )
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            } else
            {
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
            }


        }


        //private void dx_SizeChanged(object sender,EventArgs e)
        //{
        //    if ( this.WindowState == FormWindowState.Minimized )
        //    {
        //        this.Hide();
        //    }
        //}


        private void FormMain_Load(object sender,EventArgs e)
        {
            //////////////////
            ////initial
            //////////////////

            ////////////
            //form
            //CheckForIllegalCrossThreadCalls = false;//disable report
            m_SyncContext = SynchronizationContext.Current;
            m_SyncChart = SynchronizationContext.Current;

            this.WindowState = FormWindowState.Minimized;
            this.Hide();

            this.ShowInTaskbar = false;
            this.Text = "Real APM   v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();


            /////////////////
            //apm chart
            initialChart();
            apmChartData = new int[60];
            for ( int i = 0; i < 60; i++ )
            {
                apmChartData[i] = 0;
            }


            ///////////
            //key hook
            k_hook = new KeyboardHook();
            k_hook.KeyDownEvent += new System.Windows.Forms.KeyEventHandler(hook_KeyDown);//钩住键按下 
            //k_hook.KeyPressEvent += K_hook_KeyPressEvent;
            k_hook.Start();//安装键盘钩子


            //////////////
            //mouse hook
            m_hook = new MouseHook();
            m_hook.SetHook();
            //m_hook.MouseMoveEvent += mh_MouseMoveEvent;
            m_hook.MouseClickEvent += mh_MouseClickEvent;


            /////////////
            //seconds work array
            apmSeconds = new int[60];
            for ( int i = 0; i < 60; i++ )
            {
                apmSeconds[i] = 0;
            }


            /////////////
            //reset seconds counter
            thisSecondsCounter = 0;


            /////////////////
            //install timer
            //  apm get timer
            System.Timers.Timer tApm = new System.Timers.Timer(1000);//实例化Timer类，设置间隔时间为10000毫秒；
            tApm.Elapsed += new System.Timers.ElapsedEventHandler(checkTheSecondAction);//到达时间的时候执行事件；
            tApm.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            tApm.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；

            //  apm chart refresh timer
            System.Timers.Timer tApmChartUpdate = new System.Timers.Timer(1000);//实例化Timer类，设置间隔时间为10000毫秒；
            tApmChartUpdate.Elapsed += new System.Timers.ElapsedEventHandler(apmChartUpdate);//到达时间的时候执行事件；
            tApmChartUpdate.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            tApmChartUpdate.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；

            
        }


        private void apmChartUpdate(object source,System.Timers.ElapsedEventArgs e)
        {
            m_SyncChart.Post(apmChartUpdateData,totalAPM);
        }
      

        public void apmChartUpdateData(object totalAPM)
        {// refresh the APM chart
            //DRAW APM 1 MINITE CHART

            chart1MinApm.Series.Clear();            //clear the chart
            series1MinApmTotal.Points.Clear();    //clear the data
            
            for ( int i = 0; i < 60; i++ )
            {
                series1MinApmTotal.Points.Add(apmChartData[i]);
            }

            chart1MinApm.Series.Add(series1MinApmTotal);
            //chart1MinApm.Invalidate();
            //System.Console.WriteLine("update chart");

        }


        public void checkTheSecondAction(object source,System.Timers.ElapsedEventArgs e)
        {// Core !!!
            //MessageBox.Show("OK!");
            calculateSecondAPM();

            //addCounter_one();

            int oneBit = totalAPM % 10;
            int handredTenBit = ( totalAPM - oneBit ) / 10;
            if ( handredTenBit > 99 )
            {
                handredTenBit = 99;     //adjust shown chars
            }
            string shownAPM = handredTenBit.ToString();
            SetTaskIconDynamic(shownAPM);
            //System.Console.WriteLine("totalAPM: "+ totalAPM);     //del

            //SetTaskIconDynamic(Convert.ToString(this.tenCounter));      //only test

        }

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr handle);   //Icon对象后调用此方法释放Handler

        public void SetTaskIconDynamic(string number)
        {
            try
            {
                lock ( actionLock )
                {
                    //System.Console.WriteLine("SetTaskIconDynamic");     //del
                    //动态绘制图标样式
                    Size size = this.Icon.Size;
                    Bitmap cursorBitmap = new Bitmap(size.Width,size.Height);
                    Graphics graphics = Graphics.FromImage(cursorBitmap);
                    graphics.Clear(Color.FromArgb(0,0,0,0));
                    graphics.ResetClip();
                    Rectangle rect = new Rectangle(0,0,size.Width,size.Height);

                    //Gdi+自定义绘制图标
                    //graphics.DrawImage(this.Icon.ToBitmap(),rect);
                    //graphics.FillEllipse(new SolidBrush(Color.FromArgb(244,107,10)),new Rectangle(rect.Width / 2 - 2,rect.Height / 2 - 2,rect.Width / 2,rect.Height / 2));
                    //graphics.DrawString(number,this.Font,Brushes.White,new Rectangle(rect.Width / 2 - 2,rect.Height / 2 - 2,rect.Width / 2,rect.Height / 2),new StringFormat()
                    //{
                    //    LineAlignment = StringAlignment.Center,
                    //    Alignment = StringAlignment.Center
                    //});

                    Font myFont = new Font("Tahoma",rect.Width / 2,FontStyle.Regular);
                    Brush bush = new SolidBrush(Color.Cyan);//填充的颜色
                    //graphics.DrawString("99",myFont,bush,100,100);
                    graphics.FillRectangle(new SolidBrush(Color.Indigo),0,0,size.Width,size.Height);
                    graphics.DrawString(number,myFont,bush,new Rectangle(2,2,rect.Width,rect.Height),new StringFormat()
                    {
                        LineAlignment = StringAlignment.Center,
                        Alignment = StringAlignment.Center
                    });
                    //System.Console.WriteLine("rect.Width :"+ rect.Width);     //del
                    //System.Console.WriteLine("rect.Height :"+ rect.Height);     //del

                    //生成Icon
                    IntPtr iconHandle = cursorBitmap.GetHicon();
                    Icon cursor = Icon.FromHandle(iconHandle);
                
                    //更新任务栏图标样式
                    this.notifyIconTaskbar.Icon = cursor;

                    this.notifyIconTaskbar.Text = "Real APM : " + totalAPM;
          
                    m_SyncContext.Post(safePost_setAPMText,totalAPM);
                    //this.labelTextApm.Text = totalAPM.ToString();

                    
                    DestroyIcon(iconHandle);    //free icon resource , 
                      // anti System.Runtime.InteropServices.ExternalException (0x80004005): GDI+ 中发生一般性错误。
                    graphics.Dispose();
                    cursorBitmap.Dispose();
                    cursor.Dispose();
                    myFont.Dispose();
                    bush.Dispose();

                }           
            } catch( Exception e )
            {
                //System.Console.WriteLine("e :" + e.ToString());     //del
                //no-nothing
            }
            
        }


        private void safePost_setAPMText(object totalAPM)
        {
            this.labelTextApm.Text = totalAPM.ToString();
        }


        private void hook_KeyDown(object sender,KeyEventArgs e)
        {
            //addCounter_one();     //only for test
            addOneAPMToSecondsCounter();
        }


        private void mh_MouseClickEvent(object sender,MouseEventArgs e)
        {
            //there is a problem,in this method it can listen both click and move
            // so ,how can we let those action in different method ?? need to finish


            //MessageBox.Show(e.X + "-" + e.Y);
            //System.Console.WriteLine(e.X + "-" + e.Y);     //del

            if( ( e.Button == MouseButtons.Left )   ||
                ( e.Button == MouseButtons.Right )  ||
                ( e.Button == MouseButtons.Middle )
              )
            {
                //addCounter_one();     //only for test
                addOneAPMToSecondsCounter();
            }
        }


        //private void mh_MouseMoveEvent(object sender,MouseEventArgs e)
        //{
        //    int x = e.Location.X;
        //    int y = e.Location.Y;
        //    textBox1.Text = x + "";
        //    textBox2.Text = y + "";
        //}


        //private void addCounter_one()
        //{//this function for test only
        //    oneCounter++;
        //    if ( oneCounter >= 10 )
        //    {
        //        if(tenCounter < 99)
        //        {//full counter is below 999
        //         //full counter: tenCounter * 10 + oneCounter
        //            tenCounter++;
        //        }else
        //            {
        //                //do-nothing
        //            }
        //        oneCounter = 0;
        //    }
        //}


        private void addOneAPMToSecondsCounter()
        {
            thisSecondsCounter++;
        }


        private void calculateSecondAPM()
        {
            int secondNow =DateTime.Now.Second;
            //System.Console.WriteLine("secondNow: "+ secondNow);     //del
            //DateTime.Now.Second.ToString();       //获取秒数 

            apmSeconds[secondNow] = thisSecondsCounter;

            totalAPM = 0;
            for ( int i = 0; i < 60; i++ )
            {
                totalAPM += apmSeconds[i];
            }

            thisSecondsCounter = 0;     //reset seconds counter


            //chart data
            for ( int i = 1; i < 60; i++ )
            {//move data one slot
                apmChartData[i-1] = apmChartData[i];
            }
            apmChartData[59]  =totalAPM;    //keep last apm valuse

        }


        private void FormMain_FormClosed(object sender,FormClosedEventArgs e)
        {
            k_hook.Stop();
            m_hook.UnHook();
        }


        private void buttonExit_Click(object sender,EventArgs e)
        {
            k_hook.Stop();
            m_hook.UnHook();

            this.Dispose(false);
            System.Environment.Exit(0);
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            if ( this.Visible )
            {
                e.Cancel = true;
            }

            this.Hide();
            this.WindowState = FormWindowState.Minimized;
        }


        private void FormMain_DoubleClick(object sender,EventArgs e)
        {
            MessageBox.Show("责任作者：mr_wood@126.com");
        }


        private void initialChart()
        {
          chart1MinApm.Series.Clear();//清除默认的图例

          series1MinApmTotal.ChartType = SeriesChartType.Area;

          series1MinApmTotal["PointWidth"] = "0.6";
          series1MinApmTotal.IsValueShownAsLabel = false;//是否显示值

          series1MinApmTotal.BorderColor = Color.FromArgb(200,255,0,0);

          chart1MinApm.Series.Add(series1MinApmTotal);
          //series["DrawingStyle"] = "cylinder";
          chart1MinApm.Legends[0].Enabled = false;//是否显示图例
                                            //chart1.BackColor = Color.FromArgb(243, 223, 193);
          chart1MinApm.BackColor = ColorTranslator.FromHtml("#D3DFF0");//用网页颜色
          chart1MinApm.BackGradientStyle = GradientStyle.TopBottom;//渐变背景，从上到下
          chart1MinApm.BorderlineDashStyle = ChartDashStyle.Solid;//外框线为实线
          chart1MinApm.BorderlineWidth = 2;

          chart1MinApm.ChartAreas[0].BackColor = Color.Transparent;//数据区域的背景，默认为白色
          chart1MinApm.ChartAreas[0].BackGradientStyle = GradientStyle.TopBottom;
          chart1MinApm.ChartAreas[0].BorderDashStyle = ChartDashStyle.Solid;
          chart1MinApm.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(24,64,64,64);//数据区域，纵向的线条颜色
          chart1MinApm.ChartAreas[0].AxisX.MajorGrid.Interval = 3;//主网格间距
          chart1MinApm.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(24,64,64,64);//数据区域，横向线条的颜色
          chart1MinApm.ChartAreas[0].AxisX.MinorGrid.Interval = 2;//副网格间距
          
        }


        /*
        private void initialChartTest()
        {
          chart1.Series.Clear();//清除默认的图例
          

          Series series2 = new Series("default2");
          series2.Points.Add(160);
          series2.Points.AddY(150);
          series2.Points.AddY(140);
          series2.Points.AddY(170);
          series2.Points.AddY(120);

          //series.ChartType = SeriesChartType.Column;
          series2.ChartType = SeriesChartType.Area;

          series2["PointWidth"] = "0.6";
          series2.IsValueShownAsLabel = true;//是否显示值

          series2.BorderColor = Color.FromArgb(80,26,255,255);
          chart1.Series.Add(series2);


          Series series = new Series("default");
          series.Points.Add(60);
          series.Points.AddY(50);
          series.Points.AddY(40);
          series.Points.AddY(70);
          series.Points.AddY(20);

          //series.ChartType = SeriesChartType.Column;
          series.ChartType = SeriesChartType.Area;

          series["PointWidth"] = "0.6";
          series.IsValueShownAsLabel = true;//是否显示值

          series.BorderColor = Color.FromArgb(200,255,0,0);

          chart1.Series.Add(series);
          //series["DrawingStyle"] = "cylinder";
          chart1.Legends[0].Enabled = false;//是否显示图例
                                            //chart1.BackColor = Color.FromArgb(243, 223, 193);
          chart1.BackColor = ColorTranslator.FromHtml("#D3DFF0");//用网页颜色
          chart1.BackGradientStyle = GradientStyle.TopBottom;//渐变背景，从上到下
          chart1.BorderlineDashStyle = ChartDashStyle.Solid;//外框线为实线
          chart1.BorderlineWidth = 2;

          chart1.ChartAreas[0].BackColor = Color.Transparent;//数据区域的背景，默认为白色
          chart1.ChartAreas[0].BackGradientStyle = GradientStyle.TopBottom;
          chart1.ChartAreas[0].BorderDashStyle = ChartDashStyle.Solid;
          chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(24,64,64,64);//数据区域，纵向的线条颜色
          chart1.ChartAreas[0].AxisX.MajorGrid.Interval = 3;//主网格间距
          chart1.ChartAreas[0].AxisX.MinorGrid.Interval = 2;//副网格间距
          chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(24,64,64,64);//数据区域，横向线条的颜色
        }
        */

    }
}
