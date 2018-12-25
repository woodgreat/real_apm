using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
/*
ALL SOURCE CODES OF THIS PROJECT IS UNDER LGPL 3.0
     */
namespace WindowsForms_showAPM
{
    public partial class FormMain : Form
    {
        private int totalAPM = 0;
        private int thisSecondsCounter = 0;
        private int oneCounter =0;
        private int tenCounter = 0;
        private KeyboardHook k_hook;
        private MouseHook m_hook;
        private int[] apmSeconds;

        SynchronizationContext m_SyncContext = null;

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

            this.WindowState = FormWindowState.Minimized;
            this.Hide();

            this.ShowInTaskbar = false;
            this.Text ="Real APM   v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();


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
            for(int i=0;i<60;i++ )
            {
                apmSeconds[i] = 0;
            }

            /////////////
            //reset seconds counter
            thisSecondsCounter = 0;

            /////////////////
            //install timer
            System.Timers.Timer t = new System.Timers.Timer(1000);//实例化Timer类，设置间隔时间为10000毫秒；
            t.Elapsed += new System.Timers.ElapsedEventHandler(checkTheSecondAction);//到达时间的时候执行事件；
            t.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；

        }


        public void checkTheSecondAction(object source,System.Timers.ElapsedEventArgs e)
        {// Core !!!
            //MessageBox.Show("OK!");
            calculateSecondAPM();

            //addCounter_one();

            int oneBit = totalAPM % 10;
            int handredTenBit = ( totalAPM - oneBit ) / 10;
            if(handredTenBit > 99)
            {
                handredTenBit = 99;     //adjust shown chars
            }
            string shownAPM = handredTenBit.ToString();
            SetTaskIconDynamic(shownAPM);
            //System.Console.WriteLine("totalAPM: "+ totalAPM);     //del

            //SetTaskIconDynamic(Convert.ToString(this.tenCounter));      //only test

        }


        public void SetTaskIconDynamic(string number)
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

            Font myFont = new Font("Tahoma", rect.Width /2  ,FontStyle.Regular);
            Brush bush = new SolidBrush(Color.Cyan);//填充的颜色
            //graphics.DrawString("99",myFont,bush,100,100);
            graphics.FillRectangle(new SolidBrush(Color.Indigo),0,0,size.Width,size.Height);
            graphics.DrawString(number,myFont,bush,new Rectangle(2,2,rect.Width ,rect.Height),new StringFormat()
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            });
            //System.Console.WriteLine("rect.Width :"+ rect.Width);     //del
            //System.Console.WriteLine("rect.Height :"+ rect.Height);     //del

            //生成Icon
            try
            {
                Icon cursor = Icon.FromHandle(cursorBitmap.GetHicon());
                graphics.Dispose();
                cursorBitmap.Dispose();

                //更新任务栏图标样式
                this.notifyIconTaskbar.Icon = cursor;

                this.notifyIconTaskbar.Text = "Real APM : " + totalAPM;

                m_SyncContext.Post(safePost_setAPMText,totalAPM);
                //this.labelTextApm.Text = totalAPM.ToString();
            } catch( Exception e )
            {
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

            if( (e.Button == MouseButtons.Left ) ||
                ( e.Button == MouseButtons.Right )
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
    }
}
