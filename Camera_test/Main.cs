using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Camera_test
{
    public partial class Main : Form
    {
        bool STREAM = false;
        VideoCapture CAPTURE;
        //VideoWriter Writer;
        Mat FRAME;
        private System.Timers.Timer timer_back;
        private System.Windows.Forms.Timer timer_form;
        


        public Main()
        {
            InitializeComponent();
            CAPTURE = new VideoCapture(0);
            //Writer = new VideoWriter();
        }

        private void Click_start(object sender, EventArgs e)
        {
            STREAM = !STREAM;
            if (STREAM)
            {
                カメラ設定(int.Parse(textBox_width.Text), int.Parse(textBox_height.Text), int.Parse(textBox_FPS.Text));
                タイマー開始();
                button_start.Text = "stop";
            }
            else
            {
                タイマー停止();
                button_start.Text = "start";
            }
        }

        void カメラ設定(int w, int h,int fps)
        {
            CAPTURE.Set(CaptureProperty.FrameHeight,h);
            CAPTURE.Set(CaptureProperty.FrameWidth, w);
            CAPTURE.Set(CaptureProperty.Fps,fps);

            if (FRAME != null) FRAME.Dispose();
            FRAME = new Mat(CAPTURE.FrameHeight, CAPTURE.FrameWidth, MatType.CV_8UC3);//カラーらしい
            textBox_width.Text = CAPTURE.FrameWidth.ToString();//設定できたフレームの大きさを表示
            textBox_height.Text = CAPTURE.FrameHeight.ToString();
            textBox_FPS.Text = ((int)(CAPTURE.Fps)).ToString();
        }
        private void form_ctrl(object sender, EventArgs e)//タイマ割り込みで行う処理
        {
            //var mat3 = new MatOfByte3(FRAME);
            //var indexer = mat3.GetIndexer();
            var color = new MatOfByte3(FRAME).GetIndexer()[0, 0];//(y,x)の画素を取得
            pictureBoxIpl1.RefreshIplImage(FRAME);
            Console.WriteLine("({0},{1},{2})", color[0], color[1], color[2]);
        }
        void back_ctrl(object sender, ElapsedEventArgs e)
        {
            var frame = new Mat(FRAME.Height,FRAME.Width,FRAME.Type());
            var isFrame=CAPTURE.Read(frame);
            if(!isFrame)System.Diagnostics.Debug.WriteLine("frame=null");
            frame.CopyTo(FRAME);
            frame.Release();
        }

        void タイマー開始()
        {
            if (timer_form != null) timer_form.Dispose();
            timer_form = new System.Windows.Forms.Timer();
            timer_form.Tick += new EventHandler(form_ctrl);
            timer_form.Interval = (int)(1000/double.Parse(textBox_FPS.Text));
            timer_form.Start();

            if (timer_back != null) timer_back.Dispose();
            timer_back = new System.Timers.Timer();
            timer_back.Elapsed += new ElapsedEventHandler(back_ctrl);
            timer_back.Interval = (int)(1000 / double.Parse(textBox_FPS.Text));
            timer_back.Start();
        }
        void タイマー停止()
        {
            timer_form.Stop();
            timer_form.Dispose();

            timer_back.Stop();
            timer_back.Dispose();
        }
    }
}
