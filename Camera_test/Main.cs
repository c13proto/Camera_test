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
        CvCapture CAPTURE;
        private System.Timers.Timer timer_back;
        private System.Windows.Forms.Timer timer_form;
        public static IplImage camera;


        public Main()
        {
            InitializeComponent();
            CAPTURE = Cv.CreateCameraCapture(0);
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
            Cv.SetCaptureProperty(CAPTURE, CaptureProperty.FrameWidth, w);
            Cv.SetCaptureProperty(CAPTURE, CaptureProperty.FrameHeight, h);
            CAPTURE.Fps = fps;

            if (camera != null) camera.Dispose();
            camera = Cv.CreateImage(new CvSize(w, h), BitDepth.U8, 3);//カラーらしい
            camera.Zero();

        }
        private void form_ctrl(object sender, EventArgs e)//タイマ割り込みで行う処理
        {
            pictureBoxIpl1.RefreshIplImage(camera);
        }
        void back_ctrl(object sender, ElapsedEventArgs e)
        {
            var frame = Cv.QueryFrame(CAPTURE);
            if (frame != null) Cv.Copy(frame,camera);
            else System.Diagnostics.Debug.WriteLine("frame=null");
            Cv.ReleaseImage(frame);
        }

        void タイマー開始()
        {
            if (timer_form != null) timer_form.Dispose();
            timer_form = new System.Windows.Forms.Timer();
            timer_form.Tick += new EventHandler(form_ctrl);
            timer_form.Interval = 1000/int.Parse(textBox_FPS.Text);
            timer_form.Start();

            if (timer_back != null) timer_back.Dispose();
            timer_back = new System.Timers.Timer();
            timer_back.Elapsed += new ElapsedEventHandler(back_ctrl);
            timer_back.Interval = 1000/int.Parse(textBox_FPS.Text);
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
