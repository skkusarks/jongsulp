﻿using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

//Additional namespace
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using WolframAlphaNET;
using WolframAlphaNET.Objects;
using WolframAlphaNET.Misc;

namespace Jonsulp
{
    public partial class MainForm : Form
    {
        // ppt temp path
        string ppt_temp_path = System.Windows.Forms.Application.StartupPath + "\\ppt_temp";
        
        // objects
        Presentation ppt;

        //Image variables
        private Boolean drag = false;
        private System.Drawing.Point point;

        // variables
        int slide = 0;
        int slide_max = 0;
        string input = "y=(x+1)(x-1)(x-3)";

        #region MainForm
        /// <summary>
        /// MainForm 생성자
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            text_input.Text = input;
            pictureBox_image.MouseWheel += new MouseEventHandler(mouse_wheel);
        }
        #endregion

        #region Image Mouse Wheel
        /// <summary>
        /// 이미지 picturebox에서 마우스 휠 이벤트
        /// 크기를 조절
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mouse_wheel(object sender, MouseEventArgs e)
        {
            if ((e.Delta / 120) > 0)
            {
                pictureBox_image.Width+=5;
                pictureBox_image.Height+=5;  
                //wheel up
            }
            else
            {
                pictureBox_image.Width-=5;
                pictureBox_image.Height-=5;
                //wheel down
            }
        }
        #endregion

        #region LoadFile
        /// <summary>
        /// 파일 읽기 함수
        /// </summary>
        /// <param name="ofd"></param>
        /// <returns></returns>
        private int LoadFile(OpenFileDialog ofd)
        {
            ofd.Filter = "All Files (*.*)|*.*";
            ofd.Title = "Select a File.";
            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return 1;

            return 0;
        }
        #endregion

        #region LoadImage
        /// <summary>
        /// 이미지의 주소를 이미지 picturebox에 올리는 함수
        /// </summary>
        /// <param name="path">이미지의 주소</param>
        private void LoadImage(string path)
        {
            image.ImageLocation = path;
        }
        #endregion

        #region LoadPPT
        /// <summary>
        /// PPT파일을 ppt객체에 저장하는 함수?
        /// </summary>
        /// <param name="path">PPT파일의 주소</param>
        /// <returns>ppt파일의 정보가 담긴 ppt객체</returns>
        private Presentation LoadPPT(string path)
        {
            ApplicationClass app = new ApplicationClass();
            Presentation ppt = app.Presentations.Open
                (
                path,
                MsoTriState.msoTrue,
                MsoTriState.msoFalse,
                MsoTriState.msoFalse
                );

            app.Quit();
            return ppt;
        }
        #endregion

        #region MakePPTImage
        /// <summary>
        /// ppt객체를 가지고 PPT를 이미지로 변환하는 함수
        /// </summary>
        /// <param name="ppt">ppt내용이 담긴 ppt객체</param>
        private void MakePPTimage(Presentation ppt)
        {
            string picturesPath = ppt_temp_path;
            MakeDir(picturesPath);
            slide_max = ppt.Slides.Count;
            for (int i = 0; i < ppt.Slides.Count; ++i)
            {
                ppt.Slides[i + 1].Export
                    (
                    string.Format("{0}\\temp{1}.jpg", picturesPath, i),
                    "JPG",
                    (int)ppt.Slides[i + 1].Master.Width,
                    (int)ppt.Slides[i + 1].Master.Height
                    );
            }
        }
        #endregion

        #region DisplayPPTImage
        /// <summary>
        /// ppt슬라이드를 LoadImage함수로 전달하는 함수
        /// </summary>
        /// <param name="slide">슬라이드 번호</param>
        private void DisplayPPTimage(int slide)
        {
            LoadImage(ppt_temp_path + "\\temp" + slide + ".jpg");
        }
        #endregion

        #region MakeDir
        /// <summary>
        /// 디렉토리 생성
        /// </summary>
        /// <param name="path"></param>
        private void MakeDir(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists)
                dir.Create();
        }
        #endregion

        #region Display Graph
        private void display_Graph(string src)
        {
            image_graph.Visible = true;
            image_graph.ImageLocation = src;
        }
        #endregion

        //For debug
        private void printText(string text)
        {
            text_debug.Text = text;
        }

        #region Button 이벤트

        #region Button Prev Click
        private void button_prev_Click(object sender, EventArgs e)
        {
            if (slide <= 0)
                return;

            slide--;
            DisplayPPTimage(slide);
        }
        #endregion

        #region Button Next Click
        private void button_next_Click(object sender, EventArgs e)
        {
            if (slide > slide_max)
                return;

            slide++;
            DisplayPPTimage(slide);
        }
        #endregion
        #endregion

        #region ToolStrip 이벤트
        #region Button Image Click
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (LoadFile(ofd) != 0)
                return;

            pictureBox_image.Visible = true;
            pictureBox_image.ImageLocation = ofd.FileName;
        }
        #endregion

        #region Button PPT Click
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (LoadFile(ofd) != 0)
                return;

            ppt = LoadPPT(ofd.FileName);
            MakePPTimage(ppt);

            DisplayPPTimage(slide);
        }
        #endregion

        #region Button Graph Click
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            WolframAlpha wolf = new WolframAlpha("K8WRVX-A3Y7YUUQAV");

            display_Graph(wolf.get_Graph_address(text_input.Text));
            text_input.Text = "";
        }
        #endregion

        #region Button Ink Click
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            InkRecognition.InkRecognition ink = new InkRecognition.InkRecognition();
            ink.Owner = this;
            ink.Show();
        }
        #endregion

        #region Button Search Click
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            search web = new search();
            web.recv(text_input.Text);
            web.Show();
        }
        #endregion

        #region Button Clear Click
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            image_graph.Visible = false;
            pictureBox_image.Visible = false;
        }
        #endregion

        #endregion

        #region 메뉴 툴팁 이벤트
        #region 툴팁 종료
        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        #endregion

        #region 툴팁 이미지열기
        private void 이미지열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (LoadFile(ofd) != 0)
                return;

            LoadImage(ofd.FileName);
        }
        #endregion

        #region 툴팁 PPT열기
        private void pPT열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (LoadFile(ofd) != 0)
                return;

            ppt = LoadPPT(ofd.FileName);
            MakePPTimage(ppt);

            DisplayPPTimage(slide);
        }
        #endregion

        #region 툴팁 그래프
        private void 그래프ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WolframAlpha wolf = new WolframAlpha("K8WRVX-A3Y7YUUQAV");
            display_Graph(wolf.get_Graph_address(text_input.Text));
            text_input.Text = "";
        }
        #endregion

        #region 툴팁 서치
        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            search web = new search();
            web.recv(text_input.Text);
            web.Show();
        }
        #endregion

        #region 툴팁 클리어
        private void 초기화ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            image_graph.Visible = false;
        }
        #endregion

        #region 툴팁 필기인식
        private void 필기인식ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InkRecognition.InkRecognition ink = new InkRecognition.InkRecognition();
            ink.Owner = this;
            ink.Show();
        }
        #endregion

        #region 툴팁 웹검색
        private void 웹검색ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            search web = new search();
            web.recv(text_input.Text);
            web.Show();
        }
        #endregion

        #region 툴팁 최대화
        private void 최대화ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // 현재 Full-Screen 모드일 경우 처리  
            if ((FormBorderStyle == System.Windows.Forms.FormBorderStyle.None)
                && (WindowState == FormWindowState.Maximized))
            {
                menuStrip1.Visible = true;
                image.Dock = DockStyle.None;
                // Form 상태 변경  
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                WindowState = FormWindowState.Normal;
            }

            // 현재 Full-Screen 모드가 아닐 경우 처리  
            else
            {
                //menuStrip1.Visible = false;
                image.Dock = DockStyle.Fill;
                // Form 상태 변경  
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            }
        }
        #endregion
        #endregion

        #region picture Box 마우스 이벤트
        private void pictureBox_image_MouseUp(object sender, MouseEventArgs e)
        {
            drag = false;
        }

        private void pictureBox_image_MouseMove(object sender, MouseEventArgs e)
        {
            if (drag)
            {
                PictureBox temp = (PictureBox)sender;
                temp.Left = e.X + temp.Left - point.X;
                temp.Top = e.Y + temp.Top - point.Y;
            }
        }

        private void pictureBox_image_MouseDown(object sender, MouseEventArgs e)
        {
            drag = true;
            point = new System.Drawing.Point(e.X, e.Y);
        }



        #endregion

    }
}
