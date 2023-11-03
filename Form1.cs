using MvCamCtrl.NET;
using MvCamCtrl.NET.CameraParams;

using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Flann;
using OpenCvSharp.Extensions;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
using System.Text.RegularExpressions;
using MetroFramework.Controls;

namespace A_Scout_Viewer
{
    public enum STATE_VAL
    {
        CAM_IDLE = 0,
        CAM_OPENED,
        CAM_PREVIEW,
        CAM_SAVE,
        CAM_SAVE_DONE,
        CAM_PLAY,
        CAM_PLAY_PAUSE,
        CAM_FILE_PLAY,
        CAM_FILE_PLAY_PAUSE,
        CAM_FILE_PLAY_PAUSE2,
    }

    public partial class A_Scout : MetroFramework.Forms.MetroForm //상속 클래스 변경 
    {
        int m_nValidCamNum = 0;
        int m_nHECamNumber = 0;        

        private CCamera m_MyCamera1 = null;
        private CCamera m_MyCamera2 = null;
        private CCamera m_MyCamera3 = null;
        
        List<CCameraInfo> m_ltDeviceList = new List<CCameraInfo>();

        bool m_bCam1Thread = false;
        Thread m_hCam1ReceiveThread = null;
        bool m_bCam2Thread = false;
        Thread m_hCam2ReceiveThread = null;
        bool m_bCam3Thread = false;
        Thread m_hCam3ReceiveThread = null;
        
        STATE_VAL m_Cam1State = STATE_VAL.CAM_IDLE;
        STATE_VAL m_Cam2State = STATE_VAL.CAM_IDLE;
        STATE_VAL m_Cam3State = STATE_VAL.CAM_IDLE;       
        
        
        MV_FRAME_OUT_INFO_EX m_stCam1ImageInfo;
        MV_FRAME_OUT_INFO_EX m_stCam2ImageInfo;
        MV_FRAME_OUT_INFO_EX m_stCam3ImageInfo;
        

        byte[] m_pCam1DisplayBuffer = null;
        byte[] m_pCam2DisplayBuffer = null;
        byte[] m_pCam3DisplayBuffer = null;
        
        byte[][] m_pSaveBuffer1 = new byte[Constants.CAM1_SAVE_COUNT][];
        byte[][] m_pSaveBuffer2 = new byte[Constants.CAM1_SAVE_COUNT][];
        byte[][] m_pSaveBuffer3 = new byte[Constants.CAM1_SAVE_COUNT][];
        
        bool m_bCam1NewFrame = false;
        bool m_bCam2NewFrame = false;
        bool m_bCam3NewFrame = false;
        
        int m_nSaveIndex1 = 0;
        int m_nSaveIndex2 = 0;
        int m_nSaveIndex3 = 0;
        
        int m_Cam1SaveCount = Constants.CAM1_SAVE_COUNT;
        int m_Cam2SaveCount = Constants.CAM1_SAVE_COUNT;
        int m_Cam3SaveCount = Constants.CAM1_SAVE_COUNT;
        
        bool m_bCaptureFlag1 = false;
        bool m_bCaptureFlag2 = false;
        bool m_bCaptureFlag3 = false;
        
        Mat m_pOriginalImage1 = null;
        Mat m_pDisplayImage1 = null;
        Mat m_pDisplayMode1 = null;

        Mat m_pOriginalImage2 = null;
        Mat m_pDisplayImage2 = null;
        Mat m_pDisplayMode2 = null;

        Mat m_pOriginalImage3 = null;
        Mat m_pDisplayImage3 = null;
        Mat m_pDisplayMode3 = null;

        VideoCapture video_file = null;

        int m_LastDisplayIndex1 = 0;
        int m_LastDisplayIndex2 = 0;
        int m_LastDisplayIndex3 = 0;
        
        int m_DefaultInterval = 1;
        int m_nCam1Interval = 30;
        int m_nCam2Interval = 30;
        int m_nCam3Interval = 30;
        
        double m_nFrameRate = 0.0;
        int m_nSaveFrameRate = 120;
        int m_PlaySpeedValue = 0;

        bool m_bFocusMode = false;
        int m_FocusValue = 0;

        Stopwatch FPSWatch = new Stopwatch();
        long m_nFrameCount = 0;

        Stopwatch FPSWatch2 = new Stopwatch();
        long m_nFrameCount2 = 0;

        Stopwatch FPSWatch3 = new Stopwatch();
        long m_nFrameCount3 = 0;

        static System.Windows.Forms.Timer GrabInfoTimer1;
        static System.Windows.Forms.Timer GrabInfoTimer2;
        static System.Windows.Forms.Timer GrabInfoTimer3;
        //static System.Windows.Forms.Timer PlayProgressTimer;

        private SaveFileDialog saveFileDialog1 = null;
        private OpenFileDialog openFileDialog = null;

        private MetroCheckBox[] CameraArray;
        private int m_EnalbeCamCount = 0;

        private bool isZoomed = false;
        private System.Drawing.Size originalSize1;
        private System.Drawing.Size originalSize2;
        private System.Drawing.Size originalSize3;
        private System.Drawing.Point originalLocation1;
        private System.Drawing.Point originalLocation2;
        private System.Drawing.Point originalLocation3;

        private static cbOutputExdelegate Cam1Callback;
        static void Cam1CallbackFunc(IntPtr pData, ref MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            if (pUser != null)
            {
                A_Scout thisClass = (A_Scout)GCHandle.FromIntPtr(pUser).Target;
                thisClass.m_nFrameCount++;
                switch (thisClass.m_Cam1State)
                {
                    case STATE_VAL.CAM_PREVIEW:
                        if (thisClass.m_bCam1NewFrame == false)
                        {
                            Marshal.Copy(pData, thisClass.m_pCam1DisplayBuffer, 0, (int)pFrameInfo.nFrameLen);
                            thisClass.m_stCam1ImageInfo = pFrameInfo;
                            thisClass.m_bCam1NewFrame = true;
                        }
                        break;

                    case STATE_VAL.CAM_SAVE:
                        Marshal.Copy(pData, thisClass.m_pSaveBuffer1[thisClass.m_nSaveIndex1], 0, (int)pFrameInfo.nFrameLen);
                        thisClass.m_stCam1ImageInfo = pFrameInfo;
                        thisClass.m_bCam1NewFrame = true;
                        thisClass.m_nSaveIndex1++;
                        if (thisClass.m_nSaveIndex1 == thisClass.m_Cam1SaveCount)
                        {
                            thisClass.m_nSaveIndex1 = 0;
                            thisClass.m_bCaptureFlag1 = true;
                            thisClass.m_Cam1State = STATE_VAL.CAM_SAVE_DONE;
                        }
                        break;

                    default:

                        break;
                }
            }
        }

        private static cbOutputExdelegate Cam2Callback;
        static void Cam2CallbackFunc(IntPtr pData, ref MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            if (pUser != null)
            {
                A_Scout thisClass = (A_Scout)GCHandle.FromIntPtr(pUser).Target;
                thisClass.m_nFrameCount2++;
                switch (thisClass.m_Cam2State)
                {
                    case STATE_VAL.CAM_PREVIEW:
                        Marshal.Copy(pData, thisClass.m_pCam2DisplayBuffer, 0, (int)pFrameInfo.nFrameLen);
                        thisClass.m_stCam2ImageInfo = pFrameInfo;
                        thisClass.m_bCam2NewFrame = true;
                        break;

                    case STATE_VAL.CAM_SAVE:
                        Marshal.Copy(pData, thisClass.m_pSaveBuffer2[thisClass.m_nSaveIndex2], 0, (int)pFrameInfo.nFrameLen);
                        thisClass.m_stCam2ImageInfo = pFrameInfo;
                        thisClass.m_bCam2NewFrame = true;
                        thisClass.m_nSaveIndex2++;
                        if (thisClass.m_nSaveIndex2 == thisClass.m_Cam2SaveCount)
                        {
                            thisClass.m_nSaveIndex2 = 0;
                            thisClass.m_bCaptureFlag2 = true;
                            thisClass.m_Cam2State = STATE_VAL.CAM_SAVE_DONE;
                        }
                        break;

                    default:

                        break;
                }
            }
        }

        private static cbOutputExdelegate Cam3Callback;
        static void Cam3CallbackFunc(IntPtr pData, ref MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            if (pUser != null)
            {
                A_Scout thisClass = (A_Scout)GCHandle.FromIntPtr(pUser).Target;
                thisClass.m_nFrameCount3++;
                switch (thisClass.m_Cam3State)
                {
                    case STATE_VAL.CAM_PREVIEW:
                        Marshal.Copy(pData, thisClass.m_pCam3DisplayBuffer, 0, (int)pFrameInfo.nFrameLen);
                        thisClass.m_stCam3ImageInfo = pFrameInfo;
                        thisClass.m_bCam3NewFrame = true;
                        break;

                    case STATE_VAL.CAM_SAVE:
                        Marshal.Copy(pData, thisClass.m_pSaveBuffer3[thisClass.m_nSaveIndex3], 0, (int)pFrameInfo.nFrameLen);
                        thisClass.m_stCam3ImageInfo = pFrameInfo;
                        thisClass.m_bCam3NewFrame = true;
                        thisClass.m_nSaveIndex3++;
                        if (thisClass.m_nSaveIndex3 == thisClass.m_Cam3SaveCount)
                        {
                            thisClass.m_nSaveIndex3 = 0;
                            thisClass.m_bCaptureFlag3 = true;
                            thisClass.m_Cam3State = STATE_VAL.CAM_SAVE_DONE;
                        }
                        break;

                    default:

                        break;
                }
            }
        }

        
        public A_Scout()
        {            
            InitializeComponent();
            InitializeCameraCheckBox_ComboBox();
            DeviceListAcq();
            if (m_nValidCamNum > 0)
            {                
                MemoryInitialize();
            }            
        }

        private void InitializeCameraCheckBox_ComboBox()
        {
            CameraArray = new MetroCheckBox[]
            {
                metroCBCam1, 
                metroCBCam2, 
                metroCBCam3                
            };

            for(int i = 0; i<3; i++)
            {
                CameraArray[i].Enabled = false;
            }

            cbCam1Pos.Items.Add("Not Selected");
            cbCam1Pos.Items.Add("Right Face On");
            cbCam1Pos.Items.Add("Left Face On");
            cbCam1Pos.Items.Add("Down The Line");
            cbCam1Pos.SelectedIndex = 0;
            cbCam1Pos.Enabled = false;

            cbCam2Pos.Items.Add("Not Selected");
            cbCam2Pos.Items.Add("Right Face On");
            cbCam2Pos.Items.Add("Left Face On");
            cbCam2Pos.Items.Add("Down The Line");
            cbCam2Pos.SelectedIndex = 0;
            cbCam2Pos.Enabled = false;

            cbCam3Pos.Items.Add("Not Selected");
            cbCam3Pos.Items.Add("Right Face On");
            cbCam3Pos.Items.Add("Left Face On");
            cbCam3Pos.Items.Add("Down The Line");
            cbCam3Pos.SelectedIndex = 0;
            cbCam3Pos.Enabled = false;

            cbCam1Pos.SelectedIndexChanged += CBCamPos_SelectedIndexChanged;
            cbCam2Pos.SelectedIndexChanged += CBCamPos_SelectedIndexChanged;
            cbCam3Pos.SelectedIndexChanged += CBCamPos_SelectedIndexChanged;


            metroRadioButton1.Checked = true;                  
            metroRadioButton1.Enabled = false;
            metroRadioButton2.Enabled = false;
            metroRadioButton3.Enabled = false;
            metroRadioButton1.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            metroRadioButton2.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            metroRadioButton3.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);

            tbFrameRate.Enabled = false;
            tbISO.Enabled = false;
            tbExposure.Enabled = false;
        }

        private void CBCamPos_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckForDuplicates(cbCam1Pos, cbCam2Pos, cbCam3Pos);
            CheckForDuplicates(cbCam2Pos, cbCam1Pos, cbCam3Pos);
            CheckForDuplicates(cbCam3Pos, cbCam1Pos, cbCam2Pos);
        }

        private void CheckForDuplicates(ComboBox primary, ComboBox secondary, ComboBox tertiary)
        {
            if (primary.SelectedIndex != 0) // "NOT SELECT"가 아닌 경우만 확인
            {
                if (primary.SelectedItem.Equals(secondary.SelectedItem))
                {
                    secondary.SelectedIndex = 0; // "NOT SELECT"로 설정
                }

                if (primary.SelectedItem.Equals(tertiary.SelectedItem))
                {
                    tertiary.SelectedIndex = 0; // "NOT SELECT"로 설정
                }
            }
        }

        private void DeviceListAcq()
        {
            System.GC.Collect();
            m_ltDeviceList.Clear();
            
            int nRet = CSystem.EnumDevices(CSystem.MV_USB_DEVICE, ref m_ltDeviceList);
            if (0 != nRet)
            {
                //MessageBox.Show("Enumerate devices fail!");
                return;
            }
            TileState.Text = "Connection Check \r" +  "Connect Camera && \r" + "Press this tile";
            m_nHECamNumber = 0;
            m_nValidCamNum = 0;
            string usbUDName;
            for (int i = 0; i < m_ltDeviceList.Count; i++)
            {
                if (m_ltDeviceList[i].nTLayerType == CSystem.MV_USB_DEVICE)
                {
                    CUSBCameraInfo usbInfo = (CUSBCameraInfo)m_ltDeviceList[i];
                    if ((usbInfo.chModelName == "MV-CS017-10UC")&&(usbInfo.chManufacturerName == "AIVIWORKS"))
                    {
                        m_nValidCamNum++;
                        usbUDName = Regex.Replace(usbInfo.UserDefinedName, @"[^A-Z0-9 -]", string.Empty);
                        CameraOpen(i, m_nHECamNumber, usbUDName);
                        m_nHECamNumber++;
                    }
                    else if ((usbInfo.chModelName == "MV-CS016-10UC")&& (usbInfo.chManufacturerName == "AIVIWORKS"))
                    {
                        m_nValidCamNum++;
                        usbUDName = Regex.Replace(usbInfo.UserDefinedName, @"[^A-Z0-9 -]", string.Empty);
                        CameraOpen(i, 1, usbUDName);

                        int currentWidth = pictureBox1.Width;
                        int currentHeight = pictureBox1.Height;
                        int newWidth = currentWidth; // Put your desired width here
                        int newHeight = (int)((1080.0 / 1440.0) * newWidth);
                        pictureBox1.Top = pictureBox1.Top - 80;
                        pictureBox1.Size = new System.Drawing.Size(newWidth, newHeight);
                    }
                }
            }
        }

        private void CameraOpen(int Index, int CamNumber, string UserDefineID)
        {
            if (CamNumber == 0)
            {
                if (null == m_MyCamera1)
                {
                    m_MyCamera1 = new CCamera();
                    if (null == m_MyCamera1)
                    {
                        return;
                    }
                }

                CCameraInfo device = m_ltDeviceList[Index];
                int nRet = m_MyCamera1.CreateHandle(ref device);
                if (CErrorDefine.MV_OK != nRet)
                {
                    return;
                }

                nRet = m_MyCamera1.OpenDevice();
                if (CErrorDefine.MV_OK != nRet)
                {
                    m_MyCamera1.DestroyHandle();
                    //MessageBox.Show("Device open fail! ");
                    return;
                }
                CameraArray[0].Enabled = true;
                //metroRadioButton1.Enabled = true;

                if (UserDefineID.Length >= 4)
                {
                    string firstFour = UserDefineID.Substring(0, 4);

                    if (firstFour.Equals("RFON"))
                    {
                        cbCam1Pos.SelectedIndex = 1;
                    }
                    else if (firstFour.Equals("LFON"))
                    {
                        cbCam1Pos.SelectedIndex = 2;
                    }
                    else if (firstFour.Equals("DTLN"))
                    {
                        cbCam1Pos.SelectedIndex = 3;
                    }
                    else
                    {
                        cbCam1Pos.SelectedIndex = 0;
                    }
                }
                else
                {
                    cbCam1Pos.SelectedIndex = 0;
                }
                cbCam1Pos.Enabled = true;

                float DigitalShift = 5.0f;
                m_MyCamera1.SetEnumValue("AcquisitionMode", (uint)MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
                m_MyCamera1.SetEnumValue("TriggerMode", (uint)MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
                m_MyCamera1.SetIntValue("Width", Constants.CAM1_WIDTH);
                m_MyCamera1.SetBoolValue("DigitalShiftEnable", true);
                m_MyCamera1.SetFloatValue("DigitalShift", DigitalShift);
                m_Cam1State = STATE_VAL.CAM_OPENED;
                TileState.Text = "Camera State : \r" + "Camera Ready";
            }
            else if (CamNumber == 1)
            {
                if (null == m_MyCamera2)
                {
                    m_MyCamera2 = new CCamera();
                    if (null == m_MyCamera2)
                    {
                        return;
                    }
                }

                CCameraInfo device = m_ltDeviceList[Index];
                int nRet = m_MyCamera2.CreateHandle(ref device);
                if (CErrorDefine.MV_OK != nRet)
                {
                    return;
                }

                nRet = m_MyCamera2.OpenDevice();
                if (CErrorDefine.MV_OK != nRet)
                {
                    m_MyCamera2.DestroyHandle();
                    //MessageBox.Show("Device open fail! ");
                    return;
                }
                CameraArray[1].Enabled = true;
                //metroRadioButton2.Enabled = true;
                if (UserDefineID.Length >= 4)
                {
                    string firstFour = UserDefineID.Substring(0, 4);

                    if (firstFour.Equals("RFON"))
                    {
                        cbCam2Pos.SelectedIndex = 1;
                    }
                    else if (firstFour.Equals("LFON"))
                    {
                        cbCam2Pos.SelectedIndex = 2;
                    }
                    else if (firstFour.Equals("DTLN"))
                    {
                        cbCam2Pos.SelectedIndex = 3;
                    }
                    else
                    {
                        cbCam2Pos.SelectedIndex = 0;
                    }
                }
                else
                {
                    cbCam2Pos.SelectedIndex = 0;
                }
                cbCam2Pos.Enabled = true;

                float DigitalShift = 5.0f;
                m_MyCamera2.SetEnumValue("AcquisitionMode", (uint)MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
                m_MyCamera2.SetEnumValue("TriggerMode", (uint)MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
                m_MyCamera2.SetIntValue("Width", Constants.CAM1_WIDTH);
                m_MyCamera2.SetBoolValue("DigitalShiftEnable", true);
                m_MyCamera2.SetFloatValue("DigitalShift", DigitalShift);
                m_Cam2State = STATE_VAL.CAM_OPENED;
                TileState.Text = "Camera State : \r" + "Camera Ready";
            }
            else if (CamNumber == 2)
            {
                if (null == m_MyCamera3)
                {
                    m_MyCamera3 = new CCamera();
                    if (null == m_MyCamera3)
                    {
                        return;
                    }
                }

                CCameraInfo device = m_ltDeviceList[Index];
                int nRet = m_MyCamera3.CreateHandle(ref device);
                if (CErrorDefine.MV_OK != nRet)
                {
                    return;
                }

                nRet = m_MyCamera3.OpenDevice();
                if (CErrorDefine.MV_OK != nRet)
                {
                    m_MyCamera3.DestroyHandle();
                    //MessageBox.Show("Device open fail! ");
                    return;
                }

                CameraArray[2].Enabled = true;
                //metroRadioButton3.Enabled = true;
                if (UserDefineID.Length >= 4)
                {
                    string firstFour = UserDefineID.Substring(0, 4);

                    if (firstFour.Equals("RFON"))
                    {
                        cbCam3Pos.SelectedIndex = 1;
                    }
                    else if (firstFour.Equals("LFON"))
                    {
                        cbCam3Pos.SelectedIndex = 2;
                    }
                    else if (firstFour.Equals("DTLN"))
                    {
                        cbCam3Pos.SelectedIndex = 3;
                    }
                    else
                    {
                        cbCam3Pos.SelectedIndex = 0;
                    }
                }
                else
                {
                    cbCam3Pos.SelectedIndex = 0;
                }
                cbCam3Pos.Enabled = true;

                float DigitalShift = 5.0f;
                m_MyCamera3.SetEnumValue("AcquisitionMode", (uint)MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
                m_MyCamera3.SetEnumValue("TriggerMode", (uint)MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
                m_MyCamera3.SetIntValue("Width", Constants.CAM1_WIDTH);
                m_MyCamera3.SetBoolValue("DigitalShiftEnable", true);
                m_MyCamera3.SetFloatValue("DigitalShift", DigitalShift);
                m_Cam3State = STATE_VAL.CAM_OPENED;
                TileState.Text = "Camera State : \r" + "Camera Ready";
            }            
        }

        private void ThreadCallbackStart1()
        {
            int nRet;
            // this 객체를 GCHandle로 래핑합니다.
            GCHandle handle = GCHandle.Alloc(this);
            // GCHandle을 IntPtr로 변환합니다.
            IntPtr thisClassPtr = GCHandle.ToIntPtr(handle);

            if (m_MyCamera1 != null)
            {
                m_bCam1Thread = true;
                m_hCam1ReceiveThread = new Thread(Cam1ThreadProcess);
                m_hCam1ReceiveThread.Start();

                Cam1Callback = new cbOutputExdelegate(Cam1CallbackFunc);
                nRet = m_MyCamera1.RegisterImageCallBackEx(Cam1Callback, thisClassPtr);
                if (CErrorDefine.MV_OK != nRet)
                {
                    //MessageBox.Show("Register Cam1 image callback failed!");
                }
            }            
        }

        private void ThreadStop1()
        {
            if (m_bCam1Thread == true)
            {
                m_bCam1Thread = false;
                Thread.Sleep(300);
                m_hCam1ReceiveThread.Join(1000);
            }
            ReleaseImageBuffer1();
        }

        private void ThreadCallbackStart2()
        {
            int nRet;
            // this 객체를 GCHandle로 래핑합니다.
            GCHandle handle = GCHandle.Alloc(this);
            // GCHandle을 IntPtr로 변환합니다.
            IntPtr thisClassPtr = GCHandle.ToIntPtr(handle);
                        
            if (m_MyCamera2 != null)
            {
                m_bCam2Thread = true;
                m_hCam2ReceiveThread = new Thread(Cam2ThreadProcess);
                m_hCam2ReceiveThread.Start();

                Cam2Callback = new cbOutputExdelegate(Cam2CallbackFunc);
                nRet = m_MyCamera2.RegisterImageCallBackEx(Cam2Callback, thisClassPtr);
                if (CErrorDefine.MV_OK != nRet)
                {
                    //MessageBox.Show("Register Cam2 image callback failed!");
                }
            }
        }

        private void ThreadStop2()
        {
            if (m_bCam2Thread == true)
            {
                m_bCam2Thread = false;
                Thread.Sleep(300);
                m_hCam2ReceiveThread.Join(1000);
            }
            ReleaseImageBuffer2();
        }

        private void ThreadCallbackStart3()
        {
            int nRet;
            // this 객체를 GCHandle로 래핑합니다.
            GCHandle handle = GCHandle.Alloc(this);
            // GCHandle을 IntPtr로 변환합니다.
            IntPtr thisClassPtr = GCHandle.ToIntPtr(handle);

            if (m_MyCamera3 != null)
            {
                m_bCam3Thread = true;
                m_hCam3ReceiveThread = new Thread(Cam3ThreadProcess);
                m_hCam3ReceiveThread.Start();

                Cam3Callback = new cbOutputExdelegate(Cam3CallbackFunc);
                nRet = m_MyCamera3.RegisterImageCallBackEx(Cam3Callback, thisClassPtr);
                if (CErrorDefine.MV_OK != nRet)
                {
                    //MessageBox.Show("Register Cam1 image callback failed!");
                }
            }
        }

        private void ThreadStop3()
        {
            if (m_bCam3Thread == true)
            {
                m_bCam3Thread = false;
                Thread.Sleep(300);
                m_hCam3ReceiveThread.Join(1000);
            }
            ReleaseImageBuffer3();
        }
               

        public void Cam1ThreadProcess()
        {
            while (m_bCam1Thread)
            {
                switch (m_Cam1State)
                {
                    case STATE_VAL.CAM_PREVIEW:
                        if ((m_bCam1NewFrame) && (m_pOriginalImage1 == null) && (m_pDisplayImage1 == null))
                        {
                            CreateImageBuffer1();
                        }
                        else
                        {
                            if ((m_bCam1NewFrame) && (m_pOriginalImage1 != null) && (m_pDisplayImage1 != null))
                            {
                                //ImageDisplayCam1();
                                if (!pictureBox1.IsDisposed && pictureBox1.InvokeRequired)
                                {
                                    try
                                    {
                                        pictureBox1.Invoke(new MethodInvoker(delegate
                                        {
                                            ImageDisplayCam1();
                                            m_bCam1NewFrame = false;
                                        }));
                                        //return;
                                    }
                                    catch (System.ComponentModel.InvalidAsynchronousStateException)
                                    {

                                    }
                                    //return;
                                }
                                //m_bCam1NewFrame = false;
                            }
                        }
                        Thread.Sleep(30);
                        break;
                    
                    default:
                        Thread.Sleep(10);
                        break;
                }
                Thread.Sleep(1);
            }
        }

        public void Cam2ThreadProcess()
        {
            while (m_bCam2Thread)
            {
                switch (m_Cam2State)
                {
                    case STATE_VAL.CAM_PREVIEW:
                        if ((m_bCam2NewFrame) && (m_pOriginalImage2 == null) && (m_pDisplayImage2 == null))
                        {
                            CreateImageBuffer2();
                        }
                        else
                        {
                            if ((m_bCam2NewFrame) && (m_pOriginalImage2 != null) && (m_pDisplayImage2 != null))
                            {
                                if (!pictureBox2.IsDisposed && pictureBox2.InvokeRequired)
                                {
                                    try
                                    {
                                        pictureBox2.Invoke(new MethodInvoker(delegate
                                        {
                                            ImageDisplayCam2();
                                            m_bCam2NewFrame = false;
                                        }));
                                        //return;
                                    }
                                    catch (System.ComponentModel.InvalidAsynchronousStateException)
                                    {
                                        
                                    }

                                }
                            }
                        }
                        Thread.Sleep(30);
                        break;
                    
                    default:
                        Thread.Sleep(10);
                        break;
                }
                Thread.Sleep(1);
            }
        }

        public void Cam3ThreadProcess()
        {
            while (m_bCam3Thread)
            {
                switch (m_Cam3State)
                {
                    case STATE_VAL.CAM_PREVIEW:
                        if ((m_bCam3NewFrame) && (m_pOriginalImage3 == null) && (m_pDisplayImage3 == null))
                        {
                            CreateImageBuffer3();
                        }
                        else
                        {
                            if ((m_bCam3NewFrame) && (m_pOriginalImage3 != null) && (m_pDisplayImage3 != null))
                            {
                                //ImageDisplayCam1();
                                if (!pictureBox3.IsDisposed && pictureBox3.InvokeRequired)
                                {
                                    try
                                    {
                                        pictureBox3.Invoke(new MethodInvoker(delegate
                                        {
                                            ImageDisplayCam3();
                                            m_bCam3NewFrame = false;
                                        }));
                                        //return;
                                    }
                                    catch (System.ComponentModel.InvalidAsynchronousStateException)
                                    {

                                    }
                                }
                                //m_bCam1NewFrame = false;
                            }
                        }
                        Thread.Sleep(30);
                        break;
                    
                    default:
                        Thread.Sleep(10);
                        break;
                }
                Thread.Sleep(1);
            }
        }
                

        private void MemoryInitialize()
        {
            int Cam1DisplayBufferSize = Constants.CAM1_WIDTH * Constants.CAM1_HEIGHT + Constants.MEM_BUFFER_MARGIN;
            int Cam2DisplayBufferSize = Constants.CAM1_WIDTH * Constants.CAM1_HEIGHT + Constants.MEM_BUFFER_MARGIN;
            int Cam3DisplayBufferSize = Constants.CAM1_WIDTH * Constants.CAM1_HEIGHT + Constants.MEM_BUFFER_MARGIN;
                        
            // display Buffer
            m_pCam1DisplayBuffer = new byte[Cam1DisplayBufferSize];
            {
                if (m_pCam1DisplayBuffer == null)
                {
                    return;
                }
            }

            m_pCam2DisplayBuffer = new byte[Cam2DisplayBufferSize];
            {
                if (m_pCam2DisplayBuffer == null)
                {
                    return;
                }
            }

            m_pCam3DisplayBuffer = new byte[Cam3DisplayBufferSize];
            {
                if (m_pCam3DisplayBuffer == null)
                {
                    return;
                }
            }            
        }

        private void CreateImageBuffer1()
        {
            int width;
            int height;

            width = m_stCam1ImageInfo.nWidth;
            height = m_stCam1ImageInfo.nHeight;

            m_pOriginalImage1 = new Mat(height, width, MatType.CV_8UC1);
            m_pDisplayImage1 = new Mat(height, width, MatType.CV_8UC3);
            m_pDisplayMode1 = new Mat(height, width, MatType.CV_8UC3);                                  
        }

        private void CreateImageBuffer2()
        {
            int width;
            int height;
            
            width = m_stCam2ImageInfo.nWidth;
            height = m_stCam2ImageInfo.nHeight;

            m_pOriginalImage2 = new Mat(height, width, MatType.CV_8UC1);
            m_pDisplayImage2 = new Mat(height, width, MatType.CV_8UC3);
            m_pDisplayMode2 = new Mat(height, width, MatType.CV_8UC3);
                        
        }

        private void CreateImageBuffer3()
        {
            int width;
            int height;

            width = m_stCam3ImageInfo.nWidth;
            height = m_stCam3ImageInfo.nHeight;

            m_pOriginalImage3 = new Mat(height, width, MatType.CV_8UC1);
            m_pDisplayImage3 = new Mat(height, width, MatType.CV_8UC3);
            m_pDisplayMode3 = new Mat(height, width, MatType.CV_8UC3);                        
        }

        private void ImageDisplayCam1()
        {           
            m_pOriginalImage1.SetArray(m_pCam1DisplayBuffer);
            Cv2.CvtColor(m_pOriginalImage1, m_pDisplayImage1, ColorConversionCodes.BayerGR2BGR);

            Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(m_pDisplayImage1);
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();                
            }
            pictureBox1.Image = bitmap;            
        }


        private void ImageDisplayCam2()
        {
            m_pOriginalImage2.SetArray(m_pCam2DisplayBuffer);
            Cv2.CvtColor(m_pOriginalImage2, m_pDisplayImage2, ColorConversionCodes.BayerGR2BGR);

            Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(m_pDisplayImage2);
            if (pictureBox2.Image != null)
            {
                pictureBox2.Image.Dispose();
            }
            pictureBox2.Image = bitmap;
        }

        private void ImageDisplayCam3()
        {
            m_pOriginalImage3.SetArray(m_pCam3DisplayBuffer);
            Cv2.CvtColor(m_pOriginalImage3, m_pDisplayImage3, ColorConversionCodes.BayerGR2BGR);

            Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(m_pDisplayImage3);
            if (pictureBox3.Image != null)
            {
                pictureBox3.Image.Dispose();
            }
            pictureBox3.Image = bitmap;
        }

        private void ReleaseImageBuffer1()
        {
            if (m_pOriginalImage1 != null)
            {
                m_pOriginalImage1.Dispose();
                m_pOriginalImage1 = null;
            }

            if (m_pDisplayImage1 != null)
            {
                m_pDisplayImage1.Dispose();
                m_pDisplayImage1 = null;
            }

            if (m_pDisplayMode1 != null)
            {
                m_pDisplayMode1.Dispose();
                m_pDisplayMode1 = null;
            }
        }

        private void ReleaseImageBuffer2()
        {
            if (m_pOriginalImage2 != null)
            {
                m_pOriginalImage2.Dispose();
                m_pOriginalImage2 = null;
            }

            if (m_pDisplayImage2 != null)
            {
                m_pDisplayImage2.Dispose();
                m_pDisplayImage2 = null;
            }

            if (m_pDisplayMode2 != null)
            {
                m_pDisplayMode2.Dispose();
                m_pDisplayMode2 = null;
            }
        }

        private void ReleaseImageBuffer3()
        {
            if (m_pOriginalImage3 != null)
            {
                m_pOriginalImage3.Dispose();
                m_pOriginalImage3 = null;
            }

            if (m_pDisplayImage3 != null)
            {
                m_pDisplayImage3.Dispose();
                m_pDisplayImage3 = null;
            }

            if (m_pDisplayMode3 != null)
            {
                m_pDisplayMode3.Dispose();
                m_pDisplayMode3 = null;
            }
        }

        private void InitializeContents(int Index)
        {            
            tbFrameRate.Minimum = 0;
            tbFrameRate.Maximum = 3;
            tbFrameRate.Value = 3;

            tbISO.Minimum = 0;
            tbISO.Maximum = 8;
            tbISO.Value = 4;

            tbExposure.Minimum = 0;
            tbExposure.Maximum = 6;
            tbExposure.Value = 1;

            GetFrameRate(Index);
            GetGain(Index);
            GetExposureTime(Index);
        }

        private void btPreview_Click(object sender, EventArgs e)
        {
            int nRet;

            if ((CameraArray[0].Checked == true) && ((m_Cam1State == STATE_VAL.CAM_OPENED)|| (m_Cam1State == STATE_VAL.CAM_FILE_PLAY_PAUSE) || (m_Cam1State == STATE_VAL.CAM_FILE_PLAY_PAUSE2)))
            {
                ReleaseImageBuffer1();
                //MakeLUT();

                // 타이머 생성 및 설정
                GrabInfoTimer1 = new System.Windows.Forms.Timer();
                GrabInfoTimer1.Interval = 3000;
                GrabInfoTimer1.Tick += Timer_Tick;
                m_nFrameCount = 0;               

                //// 타이머 시작
                GrabInfoTimer1.Start();
                FPSWatch.Reset();
                FPSWatch.Start();
                if (m_MyCamera1 != null)
                {
                    nRet = m_MyCamera1.StartGrabbing();
                    if (CErrorDefine.MV_OK != nRet)
                    {
                        return;
                    }
                    m_Cam1State = STATE_VAL.CAM_PREVIEW;
                    TileState.Text = "Camera State : \r" + "Live View";
                }                                
            }

            if ((CameraArray[1].Checked == true) && ((m_Cam2State == STATE_VAL.CAM_OPENED) || (m_Cam2State == STATE_VAL.CAM_FILE_PLAY_PAUSE) || (m_Cam2State == STATE_VAL.CAM_FILE_PLAY_PAUSE2)))
            {
                ReleaseImageBuffer2();
                //MakeLUT();

                // 타이머 생성 및 설정
                GrabInfoTimer2 = new System.Windows.Forms.Timer();
                GrabInfoTimer2.Interval = 3000;
                GrabInfoTimer2.Tick += Timer_Tick2;
                m_nFrameCount2 = 0;
                
                //// 타이머 시작
                GrabInfoTimer2.Start();
                FPSWatch2.Reset();
                FPSWatch2.Start();
                if (m_MyCamera2 != null)
                {
                    nRet = m_MyCamera2.StartGrabbing();
                    if (CErrorDefine.MV_OK != nRet)
                    {
                        return;
                    }
                    m_Cam2State = STATE_VAL.CAM_PREVIEW;
                    TileState.Text = "Camera State : \r" + "Live View";
                }
            }

            if ((CameraArray[2].Checked == true) && ((m_Cam3State == STATE_VAL.CAM_OPENED) || (m_Cam3State == STATE_VAL.CAM_FILE_PLAY_PAUSE) || (m_Cam3State == STATE_VAL.CAM_FILE_PLAY_PAUSE2)))
            {                
                ReleaseImageBuffer3();
                //MakeLUT();

                // 타이머 생성 및 설정
                GrabInfoTimer3 = new System.Windows.Forms.Timer();
                GrabInfoTimer3.Interval = 3000;
                GrabInfoTimer3.Tick += Timer_Tick3;
                m_nFrameCount3 = 0;

                //// 타이머 시작
                GrabInfoTimer3.Start();
                FPSWatch3.Reset();
                FPSWatch3.Start();
                if (m_MyCamera3 != null)
                {
                    nRet = m_MyCamera3.StartGrabbing();
                    if (CErrorDefine.MV_OK != nRet)
                    {
                        return;
                    }
                    m_Cam3State = STATE_VAL.CAM_PREVIEW;
                    TileState.Text = "Camera State : \r" + "Live View";
                }
            }
            if(m_EnalbeCamCount != 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    CameraArray[i].Enabled = false;
                }
            }            
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            int nRet;

            if (((STATE_VAL.CAM_PREVIEW != m_Cam1State) && (STATE_VAL.CAM_PREVIEW != m_Cam2State) && (STATE_VAL.CAM_PREVIEW != m_Cam3State)&&(STATE_VAL.CAM_SAVE_DONE != m_Cam1State) && (STATE_VAL.CAM_SAVE_DONE != m_Cam2State) && (STATE_VAL.CAM_SAVE_DONE != m_Cam3State)))
            {
                return;
            }

            if(CameraArray[0].Checked == true)
            {
                CameraArray[0].Enabled = true;
                GrabInfoTimer1.Stop();
                lbFPS.Text = "Cam1 FPS : ";
                if (m_MyCamera1 != null)
                {
                    nRet = m_MyCamera1.StopGrabbing();
                    if (CErrorDefine.MV_OK != nRet)
                    {
                        return;
                    }
                    m_Cam1State = STATE_VAL.CAM_OPENED;
                    TileState.Text = "Camera State : \r" + "Camera Ready";
                }
            }
            else
            {
                if (m_MyCamera1 != null)
                {
                    CameraArray[0].Enabled = true;
                }
            }

            if (CameraArray[1].Checked == true)
            {
                CameraArray[1].Enabled = true;
                GrabInfoTimer2.Stop();
                lbFPS2.Text = "Cam2 FPS : ";
                if (m_MyCamera2 != null)
                {
                    nRet = m_MyCamera2.StopGrabbing();
                    if (CErrorDefine.MV_OK != nRet)
                    {
                        return;
                    }
                    m_Cam2State = STATE_VAL.CAM_OPENED;
                    TileState.Text = "Camera State : \r" + "Camera Ready";
                }
            }
            else
            {
                if(m_MyCamera2 != null)
                {
                    CameraArray[1].Enabled = true;
                }
            }

            if (CameraArray[2].Checked == true)
            {
                CameraArray[2].Enabled = true;
                GrabInfoTimer3.Stop();
                lbFPS3.Text = "Cam3 FPS : ";
                if (m_MyCamera3 != null)
                {
                    nRet = m_MyCamera3.StopGrabbing();
                    if (CErrorDefine.MV_OK != nRet)
                    {
                        return;
                    }
                    m_Cam3State = STATE_VAL.CAM_OPENED;
                    TileState.Text = "Camera State : \r" + "Camera Ready";
                }
            }
            else
            {
                if (m_MyCamera3 != null)
                {
                    CameraArray[2].Enabled = true;
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (m_MyCamera1 != null)
            {   
                long elapsedSeconds = (long)FPSWatch.Elapsed.TotalMilliseconds;
                m_nFrameRate = (double) (m_nFrameCount) * 1000 / elapsedSeconds;
                string result = string.Format("Cam1 FPS : {0:F1}", m_nFrameRate);
                lbFPS.Text = result;
                FPSWatch.Reset();
                m_nFrameCount = 0;
                FPSWatch.Start();
            }            
        }

        private void Timer_Tick2(object sender, EventArgs e)
        {
            if (m_MyCamera2 != null)
            {
                long elapsedSeconds = (long)FPSWatch2.Elapsed.TotalMilliseconds;
                m_nFrameRate = (double)(m_nFrameCount2) * 1000 / elapsedSeconds;
                string result = string.Format("Cam2 FPS : {0:F1}", m_nFrameRate);
                lbFPS2.Text = result;
                FPSWatch2.Reset();
                m_nFrameCount2 = 0;
                FPSWatch2.Start();
            }
        }

        private void Timer_Tick3(object sender, EventArgs e)
        {
            if (m_MyCamera3 != null)
            {
                long elapsedSeconds = (long)FPSWatch3.Elapsed.TotalMilliseconds;
                m_nFrameRate = (double)(m_nFrameCount3) * 1000 / elapsedSeconds;
                string result = string.Format("Cam3 FPS : {0:F1}", m_nFrameRate);
                lbFPS3.Text = result;
                FPSWatch3.Reset();
                m_nFrameCount3 = 0;
                FPSWatch3.Start();
            }
        }

        private void A_Scout_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (((STATE_VAL.CAM_PREVIEW == m_Cam1State) || (STATE_VAL.CAM_PREVIEW == m_Cam2State) || (STATE_VAL.CAM_SAVE_DONE == m_Cam1State) || (STATE_VAL.CAM_SAVE_DONE == m_Cam2State)))
            {
                btStop_Click(null, EventArgs.Empty);
                Thread.Sleep(1000);               
            }            

            if (m_bCam1Thread == true)
            {
                m_bCam1Thread = false;
                Thread.Sleep(300);
                m_hCam1ReceiveThread.Join(1000);
            }

            if (m_bCam2Thread == true)
            {
                m_bCam2Thread = false;
                Thread.Sleep(300);
                m_hCam2ReceiveThread.Join(1000);
            }

            if (m_bCam3Thread == true)
            {
                m_bCam3Thread = false;
                Thread.Sleep(300);
                m_hCam3ReceiveThread.Join(1000);
            }

           
            if (m_MyCamera1 != null)
            {            
                m_MyCamera1.CloseDevice();
                m_MyCamera1.DestroyHandle();
            }

            if (m_MyCamera2 != null)
            {
                m_MyCamera2.CloseDevice();
                m_MyCamera2.DestroyHandle();
            }

            if (m_MyCamera3 != null)
            {
                m_MyCamera3.CloseDevice();
                m_MyCamera3.DestroyHandle();
            }
            
            ReleaseImageBuffer1();
            ReleaseImageBuffer2();
            ReleaseImageBuffer3();            
        }


        
        private void GetFrameRate(int Index)
        {
            int nRet;
            float fps = 120.0f;

            if(Index == 0) 
            {
                if (m_MyCamera1 != null)
                {
                    CFloatValue pcFloatValue = new CFloatValue();
                    nRet = m_MyCamera1.GetFloatValue("ResultingFrameRate", ref pcFloatValue);
                    if (CErrorDefine.MV_OK == nRet)
                    {
                        tbFrameRate.Text = pcFloatValue.CurValue.ToString("F1");
                        fps = pcFloatValue.CurValue;
                    }

                    if (fps < 31)
                    {
                        tbFrameRate.Value = 0;
                        lbFrameRate.Text = "Frame Rate 30";
                        m_Cam1SaveCount = 30 * 3;
                        m_DefaultInterval = 1;
                        m_nSaveFrameRate = 30;
                    }
                    else if (fps < 61)
                    {
                        tbFrameRate.Value = 1;
                        lbFrameRate.Text = "Frame Rate 60";
                        m_Cam1SaveCount = 60 * 3;
                        m_DefaultInterval = 2;
                        m_nSaveFrameRate = 60;
                    }
                    else if (fps < 121)
                    {
                        tbFrameRate.Value = 2;
                        lbFrameRate.Text = "Frame Rate 120";
                        m_Cam1SaveCount = 120 * 3;
                        m_DefaultInterval = 4;
                        m_nSaveFrameRate = 120;
                    }
                    else
                    {
                        tbFrameRate.Value = 3;
                        lbFrameRate.Text = "Frame Rate 180";
                        m_Cam1SaveCount = 180 * 3;
                        m_DefaultInterval = 6;
                        m_nSaveFrameRate = 180;
                        bool bValue = true;
                        fps = 180.0f;
                        nRet = m_MyCamera1.SetBoolValue("AcquisitionFrameRateEnable", bValue);
                        nRet = m_MyCamera1.SetFloatValue("AcquisitionFrameRate", fps);
                        m_nSaveFrameRate = 180;
                    }
                }
            }
            else if (Index == 1)
            {
                if (m_MyCamera2 != null)
                {
                    CFloatValue pcFloatValue = new CFloatValue();
                    nRet = m_MyCamera2.GetFloatValue("ResultingFrameRate", ref pcFloatValue);
                    if (CErrorDefine.MV_OK == nRet)
                    {
                        tbFrameRate.Text = pcFloatValue.CurValue.ToString("F1");
                        fps = pcFloatValue.CurValue;
                    }

                    if (fps < 31)
                    {
                        tbFrameRate.Value = 0;
                        lbFrameRate.Text = "Frame Rate 30";
                        m_Cam2SaveCount = 30 * 3;
                        m_DefaultInterval = 1;
                        m_nSaveFrameRate = 30;
                    }
                    else if (fps < 61)
                    {
                        tbFrameRate.Value = 1;
                        lbFrameRate.Text = "Frame Rate 60";
                        m_Cam2SaveCount = 60 * 3;
                        m_DefaultInterval = 2;
                        m_nSaveFrameRate = 60;
                    }
                    else if (fps < 121)
                    {
                        tbFrameRate.Value = 2;
                        lbFrameRate.Text = "Frame Rate 120";
                        m_Cam2SaveCount = 120 * 3;
                        m_DefaultInterval = 4;
                        m_nSaveFrameRate = 120;
                    }
                    else
                    {
                        tbFrameRate.Value = 3;
                        lbFrameRate.Text = "Frame Rate 180";
                        m_Cam2SaveCount = 180 * 3;
                        m_DefaultInterval = 6;
                        m_nSaveFrameRate = 180;
                        bool bValue = true;
                        fps = 180.0f;
                        nRet = m_MyCamera2.SetBoolValue("AcquisitionFrameRateEnable", bValue);
                        nRet = m_MyCamera2.SetFloatValue("AcquisitionFrameRate", fps);
                        m_nSaveFrameRate = 180;

                    }
                }
            }
            else if (Index == 2)
            {
                if (m_MyCamera3 != null)
                {
                    CFloatValue pcFloatValue = new CFloatValue();
                    nRet = m_MyCamera3.GetFloatValue("ResultingFrameRate", ref pcFloatValue);
                    if (CErrorDefine.MV_OK == nRet)
                    {
                        tbFrameRate.Text = pcFloatValue.CurValue.ToString("F1");
                        fps = pcFloatValue.CurValue;
                    }

                    if (fps < 31)
                    {
                        tbFrameRate.Value = 0;
                        lbFrameRate.Text = "Frame Rate 30";
                        m_Cam3SaveCount = 30 * 3;
                        m_DefaultInterval = 1;
                        m_nSaveFrameRate = 30;
                    }
                    else if (fps < 61)
                    {
                        tbFrameRate.Value = 1;
                        lbFrameRate.Text = "Frame Rate 60";
                        m_Cam3SaveCount = 60 * 3;
                        m_DefaultInterval = 2;
                        m_nSaveFrameRate = 60;
                    }
                    else if (fps < 121)
                    {
                        tbFrameRate.Value = 2;
                        lbFrameRate.Text = "Frame Rate 120";
                        m_Cam3SaveCount = 120 * 3;
                        m_DefaultInterval = 4;
                        m_nSaveFrameRate = 120;
                    }
                    else
                    {
                        tbFrameRate.Value = 3;
                        lbFrameRate.Text = "Frame Rate 180";
                        m_Cam3SaveCount = 180 * 3;
                        m_DefaultInterval = 6;
                        m_nSaveFrameRate = 180;
                        bool bValue = true;
                        fps = 180.0f;
                        nRet = m_MyCamera3.SetBoolValue("AcquisitionFrameRateEnable", bValue);
                        nRet = m_MyCamera3.SetFloatValue("AcquisitionFrameRate", fps);
                        m_nSaveFrameRate = 180;

                    }
                }
            }          
        }

        private void SetFrameRate()
        {
            int nRet;
            float fps = 120.0f;
            if (m_MyCamera1 != null)
            {
                if (tbFrameRate.Value == 0)
                {
                    fps = 30.0f;
                    m_Cam1SaveCount = 30 * 3;
                    lbFrameRate.Text = "Frame Rate 30";
                    m_DefaultInterval = 1;
                    m_nSaveFrameRate = 30;
                }
                else if (tbFrameRate.Value == 1)
                {
                    fps = 60.0f;
                    m_Cam1SaveCount = 60 * 3;
                    lbFrameRate.Text = "Frame Rate 60";
                    m_DefaultInterval = 2;
                    m_nSaveFrameRate = 60;
                }
                else if (tbFrameRate.Value == 2)
                {
                    fps = 120.0f;
                    m_Cam1SaveCount = 120 * 3;
                    lbFrameRate.Text = "Frame Rate 120";
                    m_DefaultInterval = 4;
                    m_nSaveFrameRate = 120;
                }
                else
                {
                    fps = 180.0f;
                    m_Cam1SaveCount = 180 * 3;
                    lbFrameRate.Text = "Frame Rate 180";
                    m_DefaultInterval = 6;
                    m_nSaveFrameRate = 180;
                }

                bool bValue = true;
                nRet = m_MyCamera1.SetBoolValue("AcquisitionFrameRateEnable", bValue);
                nRet = m_MyCamera1.SetFloatValue("AcquisitionFrameRate", fps);
            }
            if (m_MyCamera2 != null)
            {
                if (tbFrameRate.Value == 0)
                {
                    fps = 30.0f;
                    m_Cam2SaveCount = 30 * 3;
                    lbFrameRate.Text = "Frame Rate 30";
                    m_DefaultInterval = 1;
                    m_nSaveFrameRate = 30;
                }
                else if (tbFrameRate.Value == 1)
                {
                    fps = 60.0f;
                    m_Cam2SaveCount = 60 * 3;
                    lbFrameRate.Text = "Frame Rate 60";
                    m_DefaultInterval = 2;
                    m_nSaveFrameRate = 60;
                }
                else if (tbFrameRate.Value == 2)
                {
                    fps = 120.0f;
                    m_Cam2SaveCount = 120 * 3;
                    lbFrameRate.Text = "Frame Rate 120";
                    m_DefaultInterval = 4;
                    m_nSaveFrameRate = 120;
                }
                else
                {
                    fps = 180.0f;
                    m_Cam2SaveCount = 180 * 3;
                    lbFrameRate.Text = "Frame Rate 180";
                    m_DefaultInterval = 6;
                    m_nSaveFrameRate = 180;
                }

                bool bValue = true;
                nRet = m_MyCamera2.SetBoolValue("AcquisitionFrameRateEnable", bValue);
                nRet = m_MyCamera2.SetFloatValue("AcquisitionFrameRate", fps);
            }
            if (m_MyCamera3 != null)
            {
                if (tbFrameRate.Value == 0)
                {
                    fps = 30.0f;
                    m_Cam3SaveCount = 30 * 3;
                    lbFrameRate.Text = "Frame Rate 30";
                    m_DefaultInterval = 1;
                    m_nSaveFrameRate = 30;
                }
                else if (tbFrameRate.Value == 1)
                {
                    fps = 60.0f;
                    m_Cam3SaveCount = 60 * 3;
                    lbFrameRate.Text = "Frame Rate 60";
                    m_DefaultInterval = 2;
                    m_nSaveFrameRate = 60;
                }
                else if (tbFrameRate.Value == 2)
                {
                    fps = 120.0f;
                    m_Cam3SaveCount = 120 * 3;
                    lbFrameRate.Text = "Frame Rate 120";
                    m_DefaultInterval = 4;
                    m_nSaveFrameRate = 120;
                }
                else
                {
                    fps = 180.0f;
                    m_Cam3SaveCount = 180 * 3;
                    lbFrameRate.Text = "Frame Rate 180";
                    m_DefaultInterval = 6;
                    m_nSaveFrameRate = 180;
                }

                bool bValue = true;
                nRet = m_MyCamera3.SetBoolValue("AcquisitionFrameRateEnable", bValue);
                nRet = m_MyCamera3.SetFloatValue("AcquisitionFrameRate", fps);
            }            
        }


        private void GetFrameRateLE()
        {
            int nRet;
            float fps = 250.0f;
            if (m_MyCamera2 != null)
            {
                CFloatValue pcFloatValue = new CFloatValue();
                nRet = m_MyCamera2.GetFloatValue("ResultingFrameRate", ref pcFloatValue);
                if (CErrorDefine.MV_OK == nRet)
                {
                    tbFrameRate.Text = pcFloatValue.CurValue.ToString("F1");
                    fps = pcFloatValue.CurValue;
                }

                if (fps < 61)
                {
                    tbFrameRate.Value = 0;
                    lbFrameRate.Text = "Frame Rate 60";
                    m_Cam2SaveCount = 60 * 3;
                    m_DefaultInterval = 1;
                    m_nSaveFrameRate = 60;
                }
                else if (fps < 121)
                {
                    tbFrameRate.Value = 1;
                    lbFrameRate.Text = "Frame Rate 120";
                    m_Cam2SaveCount = 120 * 3;
                    m_DefaultInterval = 4;
                    m_nSaveFrameRate = 120;
                }
                else if (fps < 181)
                {
                    tbFrameRate.Value = 2;
                    lbFrameRate.Text = "Frame Rate 180";
                    m_Cam2SaveCount = 180 * 3;
                    m_DefaultInterval = 5;
                    m_nSaveFrameRate = 180;
                }
                else
                {
                    tbFrameRate.Value = 3;
                    lbFrameRate.Text = "Frame Rate 250";
                    m_Cam2SaveCount = 250 * 3;
                    m_DefaultInterval = 8;
                    m_nSaveFrameRate = 250;
                    bool bValue = true;
                    fps = 250.0f;
                    nRet = m_MyCamera2.SetBoolValue("AcquisitionFrameRateEnable", bValue);
                    nRet = m_MyCamera2.SetFloatValue("AcquisitionFrameRate", fps);
                    m_nSaveFrameRate = 250;

                }
            }
        }

        
        private void tbFrameRate_Scroll(object sender, ScrollEventArgs e)
        {
             SetFrameRate();
        }

        private void GetGain(int Index)
        {
            int nRet;
            float Gain = 1.0f;
            CCamera tempCam = null;

            if(Index == 0)
            {
                tempCam = m_MyCamera1;
            }
            else if (Index == 1)
            {
                tempCam = m_MyCamera2;
            }
            else if (Index == 2)
            {
                tempCam = m_MyCamera3;
            }
            
            if (tempCam != null)
            {
                CFloatValue pcFloatValue = new CFloatValue();

                nRet = tempCam.GetFloatValue("Gain", ref pcFloatValue);
                if (CErrorDefine.MV_OK == nRet)
                {
                    Gain = pcFloatValue.CurValue;
                }

                if (Gain < 0.1)
                {
                    tbISO.Value = 0;
                    lbISO.Text = "ISO 50";
                }
                else if (Gain < 10.1)
                {
                    tbISO.Value = 1;
                    lbISO.Text = "ISO 100";
                }
                else if (Gain < 15.1)
                {
                    tbISO.Value = 2;
                    lbISO.Text = "ISO 200";
                }
                else if (Gain < 20.1)
                {
                    tbISO.Value = 3;
                    lbISO.Text = "ISO 400";
                }
                else if (Gain < 25.1)
                {
                    tbISO.Value = 4;
                    lbISO.Text = "ISO 800";
                }
                else if (Gain < 30.1)
                {
                    tbISO.Value = 5;
                    lbISO.Text = "ISO 1600";
                }
                else if (Gain < 35.1)
                {
                    tbISO.Value = 6;
                    lbISO.Text = "ISO 3200";
                }
                else if (Gain < 40.1)
                {
                    tbISO.Value = 7;
                    lbISO.Text = "ISO 6400";
                }
                else
                {
                    tbISO.Value = 8;
                    lbISO.Text = "ISO 12800";
                }
            }
        }

        private void SetGain(int Index)
        {
            int nRet;
            float Gain = 20.0f;
            if (tbISO.Value == 0)
            {
                Gain = 0.0f;
                lbISO.Text = "ISO 50";
            }
            else if (tbISO.Value == 1)
            {
                Gain = 10.0f;
                lbISO.Text = "ISO 100";
            }
            else if (tbISO.Value == 2)
            {
                Gain = 15.0f;
                lbISO.Text = "ISO 200";
            }
            else if (tbISO.Value == 3)
            {
                Gain = 20.0f;
                lbISO.Text = "ISO 400";
            }
            else if (tbISO.Value == 4)
            {
                Gain = 25.0f;
                lbISO.Text = "ISO 800";
            }
            else if (tbISO.Value == 5)
            {
                Gain = 30.0f;
                lbISO.Text = "ISO 1600";
            }
            else if (tbISO.Value == 6)
            {
                Gain = 35.0f;
                lbISO.Text = "ISO 3200";
            }
            else if (tbISO.Value == 7)
            {
                Gain = 40.0f;
                lbISO.Text = "ISO 6400";
            }
            else
            {
                Gain = 45.0f;
                lbISO.Text = "ISO 12800";
            }

            if ((m_MyCamera1 != null)&&(Index ==0))
            {                
                m_MyCamera1.SetEnumValue("GainAuto", 0);
                nRet = m_MyCamera1.SetFloatValue("Gain", Gain);
            }
            else if ((m_MyCamera2 != null)&&(Index == 1))
            {                
                m_MyCamera2.SetEnumValue("GainAuto", 0);
                nRet = m_MyCamera2.SetFloatValue("Gain", Gain);
            }
            else if ((m_MyCamera3 != null) && (Index == 2))
            {                
                m_MyCamera3.SetEnumValue("GainAuto", 0);
                nRet = m_MyCamera3.SetFloatValue("Gain", Gain);
            }            
        }


        private void tbISO_Scroll(object sender, ScrollEventArgs e)
        {
            int Index;
            Index = GetSelectedRadioButton();
            SetGain(Index);
        }

        private void GetExposureTime(int Index)
        {
            int nRet;
            float ExposureTime = 100.0f;
            CCamera tempCam = null;

            if (Index == 0)
            {
                tempCam = m_MyCamera1;
            }
            else if (Index == 1)
            {
                tempCam = m_MyCamera2;
            }
            else if (Index == 2)
            {
                tempCam = m_MyCamera3;
            }
            
            if (tempCam != null)
            {
                CFloatValue pcFloatValue = new CFloatValue();

                nRet = tempCam.GetFloatValue("ExposureTime", ref pcFloatValue);
                if (CErrorDefine.MV_OK == nRet)
                {
                    ExposureTime = pcFloatValue.CurValue;
                }

                if (ExposureTime < 101.0f)
                {
                    tbExposure.Value = 0;
                    lbExposure.Text = "Exposure Time 100us";
                }
                else if (ExposureTime < 501.0f)
                {
                    tbExposure.Value = 1;
                    lbExposure.Text = "Exposure Time 500us";
                }
                else if (ExposureTime < 1001.0f)
                {
                    tbExposure.Value = 2;
                    lbExposure.Text = "Exposure Time 1000us";
                }
                else if (ExposureTime < 2001.0f)
                {
                    tbExposure.Value = 3;
                    lbExposure.Text = "Exposure Time 2000us";
                }
                else if (ExposureTime < 3001.0f)
                {
                    tbExposure.Value = 4;
                    lbExposure.Text = "Exposure Time 3000us";
                }
                else if (ExposureTime < 4001.0f)
                {
                    tbExposure.Value = 5;
                    lbExposure.Text = "Exposure Time 4000us";
                }
                else
                {
                    tbExposure.Value = 6;
                    lbExposure.Text = "Exposure Time 5000us";
                }
            }
        }

        private void SetExposureTime(int Index)
        {
            int nRet;
            float ExposureTime = 500.0f;
            if (tbExposure.Value == 0)
            {
                ExposureTime = 100.0f;
                lbExposure.Text = "Exposure Time 100us";
            }
            else if (tbExposure.Value == 1)
            {
                ExposureTime = 500.0f;
                lbExposure.Text = "Exposure Time 500us";
            }
            else if (tbExposure.Value == 2)
            {
                ExposureTime = 1000.0f;
                lbExposure.Text = "Exposure Time 1000us";
            }
            else if (tbExposure.Value == 3)
            {
                ExposureTime = 2000.0f;
                lbExposure.Text = "Exposure Time 2000us";
            }
            else if (tbExposure.Value == 4)
            {
                ExposureTime = 3000.0f;
                lbExposure.Text = "Exposure Time 3000us";
            }
            else if (tbExposure.Value == 5)
            {
                ExposureTime = 4000.0f;
                lbExposure.Text = "Exposure Time 4000us";
            }
            else
            {
                ExposureTime = 5000.0f;
                lbExposure.Text = "Exposure Time 5000us";
            }

            if ((m_MyCamera1 != null)&&(Index == 0))
            {
                nRet = m_MyCamera1.SetFloatValue("ExposureTime", ExposureTime);
            }
            else if ((m_MyCamera2 != null)&&(Index == 1))
            {
                nRet = m_MyCamera2.SetFloatValue("ExposureTime", ExposureTime);
            }
            else if ((m_MyCamera3 != null)&&(Index == 2))
            {
                nRet = m_MyCamera3.SetFloatValue("ExposureTime", ExposureTime);
            }            
        }

        private void GetExposureTimeLE()
        {
            int nRet;
            float ExposureTime = 100.0f;
            if (m_MyCamera2 != null)
            {
                CFloatValue pcFloatValue = new CFloatValue();

                nRet = m_MyCamera2.GetFloatValue("ExposureTime", ref pcFloatValue);
                if (CErrorDefine.MV_OK == nRet)
                {
                    ExposureTime = pcFloatValue.CurValue;
                }

                if (ExposureTime < 101.0f)
                {
                    tbExposure.Value = 0;
                    lbExposure.Text = "Exposure Time 100us";
                }
                else if (ExposureTime < 501.0f)
                {
                    tbExposure.Value = 1;
                    lbExposure.Text = "Exposure Time 500us";
                }
                else if (ExposureTime < 1001.0f)
                {
                    tbExposure.Value = 2;
                    lbExposure.Text = "Exposure Time 1000us";
                }
                else if (ExposureTime < 1501.0f)
                {
                    tbExposure.Value = 3;
                    lbExposure.Text = "Exposure Time 1500us";
                }
                else if (ExposureTime < 2001.0f)
                {
                    tbExposure.Value = 4;
                    lbExposure.Text = "Exposure Time 2000us";
                }
                else if (ExposureTime < 2501.0f)
                {
                    tbExposure.Value = 5;
                    lbExposure.Text = "Exposure Time 2500us";
                }
                else
                {
                    tbExposure.Value = 6;
                    lbExposure.Text = "Exposure Time 3000us";
                }
            }
        }

        
        private void tbExposure_Scroll(object sender, ScrollEventArgs e)
        {
            int Index;
            Index = GetSelectedRadioButton();
            SetExposureTime(Index);            
        }
                
        private void TileState_Click(object sender, EventArgs e)
        {
            if ((m_nValidCamNum == 0) && (m_Cam1State == STATE_VAL.CAM_IDLE))
            {
                DeviceListAcq();
                if (m_nValidCamNum > 0)
                {                    
                    MemoryInitialize();
                }                
            }
        }

        private void cbPlaySpeed_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
                
        private void metroCBCam1_CheckedChanged(object sender, EventArgs e)
        {
            if(metroCBCam1.Checked == true)
            {
                if(m_EnalbeCamCount < 3)
                {
                    m_EnalbeCamCount++;
                }                
                InitializeContents(0);
                metroRadioButton1.Enabled = true;
                metroRadioButton1.Checked = true;
                ThreadCallbackStart1();                
            }
            else
            {
                metroRadioButton1.Enabled = false;
                metroRadioButton1.Checked = false;
                ThreadStop1();
                if (m_EnalbeCamCount > 0)
                {
                    m_EnalbeCamCount--;
                }                
            }
            if(m_EnalbeCamCount != 0)
            {
                tbFrameRate.Enabled = true;
                tbISO.Enabled = true;
                tbExposure.Enabled = true;
            }
            else
            {
                tbFrameRate.Enabled = false;
                tbISO.Enabled = false;
                tbExposure.Enabled = false;
            }
        }

        private void metroCBCam2_CheckedChanged(object sender, EventArgs e)
        {
            if (metroCBCam2.Checked == true)
            {
                if (m_EnalbeCamCount < 3)
                {
                    m_EnalbeCamCount++;
                }
                metroRadioButton2.Enabled = true;
                metroRadioButton2.Checked = true;
                InitializeContents(1);
                ThreadCallbackStart2();                
            }
            else
            {
                metroRadioButton2.Enabled = false;
                metroRadioButton2.Checked = false;
                ThreadStop2();
                if (m_EnalbeCamCount > 0)
                {
                    m_EnalbeCamCount--;
                }                
            }
            if (m_EnalbeCamCount != 0)
            {
                tbFrameRate.Enabled = true;
                tbISO.Enabled = true;
                tbExposure.Enabled = true;
            }
            else
            {
                tbFrameRate.Enabled = false;
                tbISO.Enabled = false;
                tbExposure.Enabled = false;
            }
        }

        private void metroCBCam3_CheckedChanged(object sender, EventArgs e)
        {
            if (metroCBCam3.Checked == true)
            {
                if (m_EnalbeCamCount < 3)
                {
                    m_EnalbeCamCount++;
                }
                metroRadioButton3.Enabled = true;
                metroRadioButton3.Checked = true;
                InitializeContents(2);
                ThreadCallbackStart3();
            }
            else
            {
                metroRadioButton3.Enabled = false;
                metroRadioButton3.Checked = false;
                ThreadStop3();
                if (m_EnalbeCamCount > 0)
                {
                    m_EnalbeCamCount--;
                }
            }
            if (m_EnalbeCamCount != 0)
            {
                tbFrameRate.Enabled = true;
                tbISO.Enabled = true;
                tbExposure.Enabled = true;
            }
            else
            {
                tbFrameRate.Enabled = false;
                tbISO.Enabled = false;
                tbExposure.Enabled = false;
            }
        }
            
        private void pictureBox_DoubleClick(object sender, EventArgs e)
        {
            //PictureBox clickedPictureBox = sender as PictureBox;
            //if (clickedPictureBox != null)
            //{
            //    if (!isZoomed)
            //    {
            //        // 모든 PictureBox를 숨깁니다.
            //        pictureBox1.Visible = false;
            //        pictureBox2.Visible = false;
            //        pictureBox3.Visible = false;

            //        // 클릭된 PictureBox만 크게 표시합니다.
            //        clickedPictureBox.Visible = true;
            //        clickedPictureBox.Location = new System.Drawing.Point(originalLocation1.X, originalLocation1.Y);
            //        clickedPictureBox.Size = new System.Drawing.Size(originalSize1.Width*2, originalSize1.Height*2);                    
            //        isZoomed = true;
            //    }
            //    else
            //    {
            //        // 모든 PictureBox의 원래 크기와 위치를 복원합니다.
            //        pictureBox1.Visible = true;
            //        pictureBox1.Size = originalSize1;
            //        pictureBox1.Location = originalLocation1;

            //        pictureBox2.Visible = true;
            //        pictureBox2.Size = originalSize2;
            //        pictureBox2.Location = originalLocation2;

            //        pictureBox3.Visible = true;
            //        pictureBox3.Size = originalSize3;
            //        pictureBox3.Location = originalLocation3;

            //        isZoomed = false;
            //    }
            //}
        }

        private void lbFPS2_Click(object sender, EventArgs e)
        {

        }

        private void lbFPS_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }


        private void UserSetSave()
        {
            bool SaveReturn = false;

            if (m_MyCamera1 != null)
            {
                if (cbCam1Pos.SelectedIndex == 0)
                {
                    SaveReturn = true;
                    MessageBox.Show("Please specify the location of camera 1.");
                }
                else if (cbCam1Pos.SelectedIndex == 1)
                {
                    m_MyCamera1.SetStringValue("DeviceUserID", "RFON\0");
                    Thread.Sleep(100);
                }
                else if (cbCam1Pos.SelectedIndex == 2)
                {
                    m_MyCamera1.SetStringValue("DeviceUserID", "LFON\0");
                    Thread.Sleep(100);
                }
                else if (cbCam1Pos.SelectedIndex == 3)
                {
                    m_MyCamera1.SetStringValue("DeviceUserID", "DTLN\0");
                    Thread.Sleep(100);
                }
            }

            if (m_MyCamera2 != null)
            {
                if (cbCam2Pos.SelectedIndex == 0)
                {
                    SaveReturn = true;
                    MessageBox.Show("Please specify the location of camera 2.");
                }
                else if (cbCam2Pos.SelectedIndex == 1)
                {
                    m_MyCamera2.SetStringValue("DeviceUserID", "RFON\0");
                    Thread.Sleep(100);
                }
                else if (cbCam2Pos.SelectedIndex == 2)
                {
                    m_MyCamera2.SetStringValue("DeviceUserID", "LFON\0");
                    Thread.Sleep(100);
                }
                else if (cbCam2Pos.SelectedIndex == 3)
                {
                    m_MyCamera2.SetStringValue("DeviceUserID", "DTLN\0");
                    Thread.Sleep(100);
                }
            }

            if (m_MyCamera3 != null)
            {
                if (cbCam3Pos.SelectedIndex == 0)
                {
                    SaveReturn = true;
                    MessageBox.Show("Please specify the location of camera 3.");
                }
                else if (cbCam3Pos.SelectedIndex == 1)
                {
                    m_MyCamera3.SetStringValue("DeviceUserID", "RFON\0");
                    Thread.Sleep(100);
                }
                else if (cbCam3Pos.SelectedIndex == 2)
                {
                    m_MyCamera3.SetStringValue("DeviceUserID", "LFON\0");
                    Thread.Sleep(100);
                }
                else if (cbCam3Pos.SelectedIndex == 3)
                {
                    m_MyCamera3.SetStringValue("DeviceUserID", "DTLN\0");
                    Thread.Sleep(100);
                }
            }

            if (SaveReturn == true)
            {
                return;
            }

            if (m_MyCamera1 != null)
            {
                m_MyCamera1.SetEnumValue("UserSetSelector", 1); // userset 1
                Thread.Sleep(300);
                m_MyCamera1.SetEnumValue("UserSetDefault", 1); // userset 1
                Thread.Sleep(300);
                m_MyCamera1.SetCommandValue("UserSetSave"); // userset 1
                Thread.Sleep(1000);
            }

            if (m_MyCamera2 != null)
            {
                m_MyCamera2.SetEnumValue("UserSetSelector", 1); // userset 1
                Thread.Sleep(300);
                m_MyCamera2.SetEnumValue("UserSetDefault", 1); // userset 1
                Thread.Sleep(300);
                m_MyCamera2.SetCommandValue("UserSetSave"); // userset 1
                Thread.Sleep(1000);
            }

            if (m_MyCamera3 != null)
            {
                m_MyCamera3.SetEnumValue("UserSetSelector", 1); // userset 1
                Thread.Sleep(300);
                m_MyCamera3.SetEnumValue("UserSetDefault", 1); // userset 1
                Thread.Sleep(300);
                m_MyCamera3.SetCommandValue("UserSetSave"); // userset 1
                Thread.Sleep(1000);
            }
        }

        private void btSettingSave_Click(object sender, EventArgs e)
        {          
            TileState.Text = "Camera Location \r" + "settings save... ";
            this.Cursor = Cursors.WaitCursor;

            if ((m_Cam1State == STATE_VAL.CAM_PREVIEW) || (m_Cam2State == STATE_VAL.CAM_PREVIEW) || (m_Cam3State == STATE_VAL.CAM_PREVIEW))
            {
                btStop_Click(null, EventArgs.Empty);
                Thread.Sleep(100);
            }
            UserSetSave();
            
            this.Cursor = Cursors.Default;
            TileState.Text = "Camera Location \r" + "settings complete ";
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            
            if (radio != null && radio.Checked)
            {
                switch (radio.Name)
                {
                    case "metroRadioButton1":
                        MessageBox.Show("Option 1 selected");
                        break;
                    case "metroRadioButton2":
                        MessageBox.Show("Option 2 selected");
                        break;
                    case "metroRadioButton3":
                        MessageBox.Show("Option 3 selected");
                        break;
                }
            }
        }

        private int GetSelectedRadioButton()
        {
            // 활성화되고 체크된 라디오 버튼이 있는지 확인
            if (metroRadioButton1.Enabled && metroRadioButton1.Checked)
            {
                return 0;
            }
            else if (metroRadioButton2.Enabled && metroRadioButton2.Checked)
            {
                return 1;
            }
            else if (metroRadioButton3.Enabled && metroRadioButton3.Checked)
            {
                return 2;
            }
                        
            return 0;
        }
    }

    public static class Constants
    {
        public const int CAM1_WIDTH = 1600;
        public const int CAM1_HEIGHT = 1100;
        public const int CAM2_WIDTH = 1440;
        public const int CAM2_HEIGHT = 1080;
        public const int MEM_BUFFER_MARGIN = 1024;

        public const int CAM1_SAVE_COUNT = 185 * 3 ;
        public const int CAM2_SAVE_COUNT = 250 * 3;
        public const double FRAME_RATIO = 1;
        public const int MAX_SAVE_FOLDER = 100;
        public const int CAM1_FPS = 120;
        public const int CAM2_FPS = 250;

        public const int CAM1_DEFAULT_INTERVAL = 1;
        public const int CAM2_DEFAULT_INTERVAL = 1;
      
    }    
}
