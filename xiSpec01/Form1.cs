using Cyotek.Windows.Forms;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using xiApi.NET;

namespace xiSpec01
{
    
	public class Form1 : Form
	{
		private int demoMode;

		private bool visionDemoMode;

		private bool ignoreCameraCalib;

		private string fileNameTest = "";

		private bool startWithFullScreen;

		private bool fullScreenDisplayActive;

		private int boundsTop;

		private int boundsLeft;

		private int boundsHeight;

		private int boundsWidth;

		private int num_active_threads;

		private bool button_stop_pressed;

		public int maximizeMode;

		private bool startMeasurement;

		public int numMaterials = 3;

		public int numFramesToAvg = 3;

		private bool showMeasurementFields;

		private bool startDarkImage;

		private bool startWhiteImage;

		private bool startSaveHSIcube;

		public bool statusParallelProcessing;

		public int compareSpectraMode;

		public int pictureStretchMode = 1;

		public int derivationMode;

		public int distanceMode = 5;

		public int resolutionMode = 1;

		public int imageViewMode;

		private CamParameters param_1 = new CamParameters(0);

		private Camera camera_1;

		private int anz_active_threads;

		private Thread myThreads_1;

		public int exp_time = 10000;

		private bool infoBoxVisible = true;

		private Point mousePos = default(Point);

		private bool mouseActive;

		public int filterRangeStart;

		public int filterRangeEnd;

		private int bandNr_only = -1;

		private HSIParameters HSIParam;

		private HSIImage hsi_image;

		private bool useInterpolatedImage;

		private FormDistance fDistance;

		private DemoImageFile demoImage;

		private bool demoImageFound;

		private IContainer components;

		private Button buttonStart;

		private RichTextBox richTextBox_Cam1;

		private Button buttonStop;

		private TrackBar trackBar_ExpTime;

		private Label label1;

		private Label label2;

		private Label label3;

		private Button buttonToggleInfoBox;

		private Label labelCameraModel;

		private Button buttonMaximize;

		private Button buttonDerivation1;

		private Button buttonMeasure;

		private Button buttonCompare;

		private Button buttonWhiteImage;

		private Button buttonDarkImage;

		private Label label4;

		private TextBox textBox_filterStart;

		private Label label5;

		private TextBox textBox_filterEnd;

		private Label label6;

		private CheckBox checkBoxWhiteImage;

		private CheckBox checkBoxDarkImage;

		private CheckBox checkBoxMeasured;

		private Button buttonStretch;

		private Label label10;

		private Label label_bandNR;

		private TrackBar trackBar_bandNR;

		private Button buttonImageViewMode;

		private Panel panelSpectrum;

		private Button buttonFilterResponse;

		private Button buttonDistance;

		private Button buttonShowMeasureFileds;

		private Label label7;

		private NumericUpDown numericUpDown_numMaterials;

		private Button buttonSaveCube;

		private Button buttonResolution;

		private Button buttonParallel;

		private ImageBox imageBox1;

		private Button buttonSaveConfig;

		private Label label8;

		private Label label9;

		private NumericUpDown numericUpDown_NumFramesToAvg;

		private TextBox textBoxLiveDemo;

		private Panel panel1;

		private Panel panel2;

		private Panel panel3;

		private Button buttonFullScreen;
        
        public Form1(int pDemo, bool pIgnoreCameraCalib, string pFileNameTest)
		{
			InitializeComponent();
			demoMode = pDemo;
			if (demoMode == 1)
			{
				Text += " - demomode";
			}
			else if (demoMode == 3 || demoMode == 4)
			{
				Text += " - VISION demo";
				visionDemoMode = true;
				if (demoMode == 4)
				{
					demoMode = 3;
					startWithFullScreen = true;
				}
			}
			ignoreCameraCalib = pIgnoreCameraCalib;
			fileNameTest = pFileNameTest;
			setPictureStretchMode(1);
			setbuttonDistanceText();
			setbuttonResolutionText();
			setButtonParallelText();
			setButtonCompareText();
			setButtonDerivationText();
			setButtonMaximizeText();
			button_stop_pressed = false;
			trackBar_ExpTime.Value = exp_time;
			label3.Text = trackBar_ExpTime.Value.ToString();
			numericUpDown_numMaterials.Value = numMaterials;
			numericUpDown_NumFramesToAvg.Value = numFramesToAvg;
			trackBar_bandNR.Value = bandNr_only;
			label_bandNR.Text = bandNr_only.ToString();
			labelCameraModel.Text = "";
			richTextBox_Cam1.Visible = infoBoxVisible;
			num_active_threads = 0;
			buttonStop.Enabled = false;
			buttonFilterResponse.Enabled = false;
			buttonStart.Enabled = true;
			buttonCompare.Enabled = false;
			buttonMaximize.Enabled = false;
			buttonDerivation1.Enabled = false;
			buttonDistance.Enabled = false;
			buttonWhiteImage.Enabled = false;
			buttonDarkImage.Enabled = false;
			buttonMeasure.Enabled = false;
			buttonSaveCube.Enabled = false;
			buttonResolution.Enabled = false;
			buttonSaveConfig.Enabled = false;
			if (demoMode != 0)
			{
				if (demoMode == 1 || demoMode == 3)
				{
					buttonSaveConfig.Visible = false;
				}
				demoImage = new DemoImageFile();
				demoImageFound = demoImage.readDemoImageFile();
				if (demoImageFound)
				{
					startClicked_internal();
				}
			}
			else
			{
				textBoxLiveDemo.Visible = false;
			}
			if (visionDemoMode)
			{
				buttonFullScreen.Visible = true;
				buttonFullScreen.Enabled = true;
				buttonImageViewMode.Enabled = false;
				buttonImageViewMode.Visible = false;
				buttonMaximize.Visible = false;
				buttonDerivation1.Visible = false;
				buttonDistance.Visible = false;
				buttonDarkImage.Visible = false;
				checkBoxDarkImage.Visible = false;
				buttonWhiteImage.Visible = false;
				checkBoxWhiteImage.Visible = false;
				buttonMeasure.Visible = false;
				checkBoxMeasured.Visible = false;
				textBox_filterStart.Visible = false;
				textBox_filterEnd.Visible = false;
				label4.Visible = false;
				label5.Visible = false;
				label6.Visible = false;
				label7.Visible = false;
				numericUpDown_numMaterials.Visible = false;
				buttonShowMeasureFileds.Enabled = false;
				buttonShowMeasureFileds.Visible = false;
				buttonMeasure.Visible = false;
				buttonToggleInfoBox.Visible = false;
				buttonToggleInfoBox.Enabled = false;
			}
			else
			{
				buttonFullScreen.Visible = false;
				buttonFullScreen.Enabled = false;
			}
			initialize_richTextBos_Cam1();
			if (startWithFullScreen)
			{
				toggleFullScreen();
			}
		}

		public void initialize_richTextBos_Cam1()
		{
			DateTime dateTime = new DateTime(2000, 1, 1).AddDays((double)typeof(Form1).Assembly.GetName().Version.Build).AddSeconds((double)typeof(Form1).Assembly.GetName().Version.Revision * 2.0);
			richTextBox_Cam1.Text = "";
			richTextBox_Cam1.AppendText($"{typeof(Form1).Assembly.GetName().Name}, Version = {typeof(Form1).Assembly.GetName().Version.Major}.{typeof(Form1).Assembly.GetName().Version.Minor}\n");
			richTextBox_Cam1.AppendText($"Build {dateTime}\n");
			richTextBox_Cam1.ScrollToCaret();
		}

		public Point getMousePoint()
		{
			if (camera_1 == null || HSIParam == null || mousePos.X < 0 || mousePos.Y < 0)
			{
				int num3 = mousePos.X = (mousePos.Y = -1);
			}
			return mousePos;
		}

		public bool stop_pressed()
		{
			return button_stop_pressed;
		}

		public int get_bandNr_only()
		{
			return bandNr_only;
		}

		public int maximizeActive()
		{
			return maximizeMode;
		}

		public int derivationActive()
		{
			return derivationMode;
		}

		public int distanceMethod()
		{
			return distanceMode;
		}

		public int resolutionMethod()
		{
			return resolutionMode;
		}

		public int compareSpectraActive()
		{
			return compareSpectraMode;
		}

		public bool darkImageStarted()
		{
			return startDarkImage;
		}

		public void darkImageReady()
		{
			startDarkImage = false;
			checkBoxDarkImage.Checked = true;
		}

		public bool whiteImageStarted()
		{
			return startWhiteImage;
		}

		public void whiteImageReady()
		{
			startWhiteImage = false;
			checkBoxWhiteImage.Checked = true;
			if (!visionDemoMode)
			{
				buttonDerivation1.Enabled = true;
			}
		}

		public bool measurementStarted()
		{
			return startMeasurement;
		}

		public void measurementReady()
		{
			startMeasurement = false;
			checkBoxMeasured.Checked = true;
			buttonCompare.Enabled = true;
			if (demoMode != 1 && demoMode != 3)
			{
				buttonSaveConfig.Enabled = true;
			}
		}

		public bool saveHSIcubeStarted()
		{
			return startSaveHSIcube;
		}

		public void saveHSIcubeReady()
		{
			startSaveHSIcube = false;
		}

		public int show_NumMeasurementFields()
		{
			if (!showMeasurementFields)
			{
				return -1;
			}
			return numMaterials;
		}

		public int get_NumMeasurementFields()
		{
			return numMaterials;
		}

		public int get_NumFramesToAvg()
		{
			return numFramesToAvg;
		}

		public bool get_statusParallelProcessing()
		{
			return statusParallelProcessing;
		}

		public int getExpTime()
		{
			return exp_time;
		}

		public int getImageViewMode()
		{
			return imageViewMode;
		}

		public void inc_thread_counter()
		{
			num_active_threads++;
			buttonStart.Enabled = false;
			buttonStop.Enabled = true;
		}

		public void dec_thread_counter()
		{
			num_active_threads--;
			if (num_active_threads == 0)
			{
				buttonStart.Enabled = true;
				buttonFilterResponse.Enabled = false;
				buttonStop.Enabled = false;
				buttonCompare.Enabled = false;
				buttonMaximize.Enabled = false;
				buttonDerivation1.Enabled = false;
				buttonDistance.Enabled = false;
				buttonWhiteImage.Enabled = false;
				buttonDarkImage.Enabled = false;
				buttonMeasure.Enabled = false;
				buttonSaveCube.Enabled = false;
				buttonResolution.Enabled = false;
				buttonSaveConfig.Enabled = false;
				HSIParam = null;
				camera_1 = null;
				hsi_image = null;
				if (fDistance != null)
				{
					fDistance.Close();
				}
				fDistance = null;
			}
		}

		public void set_HSIParam(HSIParameters param)
		{
			HSIParam = param;
			if (param != null)
			{
				bandNr_only = -1;
				trackBar_bandNR.Maximum = HSIParam.filter_nr_bands - 1;
				trackBar_bandNR.LargeChange = 1;
				if (!visionDemoMode)
				{
					buttonFilterResponse.Enabled = true;
					buttonMaximize.Enabled = true;
					buttonDerivation1.Enabled = false;
					derivationMode = 0;
					buttonDerivation1.Text = "int->reflectance";
					buttonDarkImage.Enabled = true;
					buttonDistance.Enabled = true;
				}
				buttonWhiteImage.Enabled = true;
				buttonMeasure.Enabled = true;
				buttonSaveCube.Enabled = true;
				buttonResolution.Enabled = true;
			}
		}

		public void register_HSIImage(HSIImage image)
		{
			hsi_image = image;
			if (demoMode != 0)
			{
				if (demoImageFound)
				{
					richTextBox_Cam1.AppendText($"demoImageFile successfully read\n");
					if (camera_1.xi_device_sn != demoImage.xi_devive_sn)
					{
						richTextBox_Cam1.AppendText($"demoImageFile differnt camera used!\n");
					}
					else if (demoImage.sensor_height != hsi_image.sensor_height || demoImage.sensor_width != hsi_image.sensor_width || demoImage.bands != hsi_image.bands || demoImage.MAX_MATERIALS != hsi_image.MAX_MATERIALS)
					{
						richTextBox_Cam1.AppendText($"demoImageFile wrong parameter set!\n");
					}
					else
					{
						textBox_filterStart.Text = demoImage.filterRangeStart.ToString();
						textBox_filterEnd.Text = demoImage.filterRangeEnd.ToString();
						if (!demoImage.setDemoSettings(hsi_image, this))
						{
							richTextBox_Cam1.AppendText($"Error setDemoSettings()!\n");
						}
						button_stop_pressed = false;
						trackBar_ExpTime.Value = exp_time;
						label3.Text = trackBar_ExpTime.Value.ToString();
						numericUpDown_numMaterials.Value = numMaterials;
						numericUpDown_NumFramesToAvg.Value = numFramesToAvg;
						trackBar_bandNR.Value = bandNr_only;
						label_bandNR.Text = bandNr_only.ToString();
						if (hsi_image.measured)
						{
							checkBoxMeasured.Checked = true;
							buttonCompare.Enabled = true;
							hsi_image.measured_neu = true;
							if (demoMode == 2)
							{
								buttonSaveConfig.Enabled = true;
							}
						}
						if (hsi_image.whiteMeasured)
						{
							checkBoxWhiteImage.Checked = true;
						}
						if (hsi_image.darkMeasured)
						{
							checkBoxDarkImage.Checked = true;
						}
						if (hsi_image.whiteMeasured || hsi_image.darkMeasured)
						{
							hsi_image.calculateFPN();
						}
						setPictureStretchMode(pictureStretchMode);
						setImageViewMode(imageViewMode);
						setbuttonDistanceText();
						setShowMeasureFiledsText();
						setbuttonResolutionText();
						setButtonParallelText();
						setButtonCompareText();
						setButtonDerivationText();
						setButtonMaximizeText();
					}
				}
				else
				{
					richTextBox_Cam1.AppendText($"Error reading demoImageFile\n");
				}
				richTextBox_Cam1.ScrollToCaret();
			}
		}

		public void set_HSIImage(HSIImage image, bool interpolatedImage)
		{
			hsi_image = image;
			useInterpolatedImage = interpolatedImage;
			updatePanelSpectrum();
		}

		private void oeffneUndLiesKameras()
		{
			int devCount = 0;
			xiCam xiCam = new xiCam();
			button_stop_pressed = false;
			initialize_richTextBos_Cam1();
			xiCam.GetNumberDevices(out devCount);
			richTextBox_Cam1.AppendText($"numDevices (found) = {devCount}\n");
			richTextBox_Cam1.ScrollToCaret();
			if (devCount > 1)
			{
				devCount = 1;
			}
			if (devCount == 0 && ignoreCameraCalib && fileNameTest != "")
			{
				devCount = 1;
			}
			richTextBox_Cam1.AppendText($"numDevices (used)  = {devCount}\n");
			richTextBox_Cam1.ScrollToCaret();
			checkBoxDarkImage.Checked = false;
			checkBoxWhiteImage.Checked = false;
			checkBoxMeasured.Checked = false;
			if (devCount > 0)
			{
				Thread thread = new Thread(test_all_cameras);
				thread.Name = "Test-All-Thread";
				thread.IsBackground = true;
				thread.Start(devCount);
			}
		}

        private void test_all_cameras(object number)
		{
			anz_active_threads = (int)number;
			if (anz_active_threads > 0)
			{
				richTextBox_Cam1.AppendText($"Starte Test-Thread 1\n");
				richTextBox_Cam1.ScrollToCaret();
				myThreads_1 = new Thread(test_camera);
				myThreads_1.Name = "Test-Thread 1";
				myThreads_1.IsBackground = true;
				myThreads_1.Start(0);
				myThreads_1.Join();
				richTextBox_Cam1.AppendText($"Test-Thread 1 beendet\n");
				richTextBox_Cam1.ScrollToCaret();
			}
		}

		private void test_camera(object index)
		{
			if ((int)index == 0)
			{
				param_1.index_cam = 0;
				param_1.exposure_time = exp_time;
				camera_1 = new Camera(param_1);
				richTextBox_Cam1.AppendText($"run Camera 1\n");
				richTextBox_Cam1.ScrollToCaret();
				camera_1.run(this, richTextBox_Cam1, imageBox1, labelCameraModel, filterRangeStart, filterRangeEnd, fDistance, ignoreCameraCalib, fileNameTest);
			}
		}

		private void startClicked_internal()
		{
			if (!int.TryParse(textBox_filterStart.Text, out filterRangeStart))
			{
				filterRangeStart = 0;
			}
			if (!int.TryParse(textBox_filterEnd.Text, out filterRangeEnd))
			{
				filterRangeEnd = 0;
			}
			if (filterRangeStart >= filterRangeEnd || filterRangeStart <= 0 || filterRangeEnd <= 0)
			{
				filterRangeStart = 0;
				filterRangeEnd = 0;
			}
			textBox_filterStart.Text = filterRangeStart.ToString();
			textBox_filterEnd.Text = filterRangeEnd.ToString();
			trackBar_bandNR.Value = (bandNr_only = -1);
			label_bandNR.Text = bandNr_only.ToString();
			if (num_active_threads == 0)
			{
				oeffneUndLiesKameras();
			}
		}

		private void buttonStart_Click(object sender, EventArgs e)
		{
			startClicked_internal();
		}

		private void buttonStop_Click(object sender, EventArgs e)
		{
			button_stop_pressed = true;
		}

		private void trackBar_ExpTime_ValueChanged(object sender, EventArgs e)
		{
			label3.Text = trackBar_ExpTime.Value.ToString();
			exp_time = trackBar_ExpTime.Value;
		}

		private void buttonToggleInfoBox_Click(object sender, EventArgs e)
		{
			infoBoxVisible = ((!infoBoxVisible) ? true : false);
			richTextBox_Cam1.Visible = infoBoxVisible;
		}

		private void Form1_SizeChanged(object sender, EventArgs e)
		{
		}

		private void setButtonMaximizeText()
		{
			switch (maximizeMode)
			{
			case 0:
				buttonMaximize.Text = "int -> max";
				break;
			default:
				buttonMaximize.Text = "max -> int";
				break;
			}
		}

		private void buttonMaximize_Click(object sender, EventArgs e)
		{
			maximizeMode = ++maximizeMode % 2;
			setButtonMaximizeText();
		}

		private void setButtonDerivationText()
		{
			if (derivationMode == 0)
			{
				buttonDerivation1.Text = "int->reflectance";
			}
			else
			{
				buttonDerivation1.Text = "reflectance->int";
			}
		}

		private void buttonDerivation1_Click(object sender, EventArgs e)
		{
			derivationMode = ++derivationMode % 2;
			setButtonDerivationText();
		}

		private void buttonMeasure_Click(object sender, EventArgs e)
		{
			startMeasurement = true;
		}

		private void setButtonCompareText()
		{
			if (visionDemoMode)
			{
				int num = compareSpectraMode;
				if (num == 1)
				{
					buttonCompare.Text = "detect->image";
				}
				else
				{
					compareSpectraMode = 0;
					buttonCompare.Text = "image->detect";
				}
			}
			else
			{
				switch (compareSpectraMode)
				{
				case 0:
					buttonCompare.Text = "image->hard";
					break;
				case 1:
					buttonCompare.Text = "hard->soft";
					break;
				default:
					buttonCompare.Text = "soft->image";
					break;
				}
			}
		}

		private void buttonCompare_Click(object sender, EventArgs e)
		{
			compareSpectraMode = ++compareSpectraMode % 3;
			setButtonCompareText();
		}

		private void buttonWhiteImage_Click(object sender, EventArgs e)
		{
			startWhiteImage = true;
		}

		private void buttonDarkImage_Click(object sender, EventArgs e)
		{
			startDarkImage = true;
		}

		private void setPictureStretchMode(int mode)
		{
			pictureStretchMode = mode % 2;
			switch (pictureStretchMode)
			{
			case 0:
				buttonStretch.Text = "zoom -> fit";
				imageBox1.SizeMode = ImageBoxSizeMode.Normal;
				break;
			default:
				buttonStretch.Text = "fit -> zoom";
				imageBox1.SizeMode = ImageBoxSizeMode.Fit;
				break;
			}
		}

		private void buttonStretch_Click(object sender, EventArgs e)
		{
			setPictureStretchMode(++pictureStretchMode);
		}

		private void trackBar_bandNR_ValueChanged(object sender, EventArgs e)
		{
			label_bandNR.Text = trackBar_bandNR.Value.ToString();
			bandNr_only = trackBar_bandNR.Value;
		}

		private void setImageViewMode(int mode)
		{
			imageViewMode = mode % 3;
			switch (imageViewMode)
			{
			case 0:
				buttonImageViewMode.Text = "avg -> max";
				break;
			case 1:
				buttonImageViewMode.Text = "max -> min";
				break;
			default:
				buttonImageViewMode.Text = "min -> avg";
				break;
			}
		}

		private void buttonImageViewMode_Click(object sender, EventArgs e)
		{
			setImageViewMode(++imageViewMode);
		}

		private void imageBox1_MouseEnter(object sender, EventArgs e)
		{
			mouseActive = true;
		}

		private void imageBox1_MouseLeave(object sender, EventArgs e)
		{
			mouseActive = false;
			int num3 = mousePos.X = (mousePos.Y = -1);
		}

		private void imageBox1_MouseMove(object sender, MouseEventArgs e)
		{
			Point location = e.Location;
			if (!imageBox1.IsPointInImage(location))
			{
				int num3 = mousePos.X = (mousePos.Y = -1);
			}
			else
			{
				Point point = imageBox1.PointToImage(location);
				mousePos.X = point.X;
				mousePos.Y = point.Y;
			}
			updatePanelSpectrum();
		}

		private void updatePanelSpectrum()
		{
			panelSpectrum.Invalidate();
		}

		private void panelSpectrum_Paint(object sender, PaintEventArgs e)
		{
			if (hsi_image != null && HSIParam != null && camera_1 != null && resolutionMode != 0)
			{
				Graphics graphics = panelSpectrum.CreateGraphics();
				PointF[] array = new PointF[25];
				PointF[] array2 = new PointF[25];
				PointF[] array3 = new PointF[25];
				PointF[] array4 = new PointF[25];
				PointF[] array5 = new PointF[25];
				PointF[] array6 = new PointF[25];
				PointF[] array7 = new PointF[25];
				PointF[] array8 = new PointF[25];
				PointF[] array9 = new PointF[25];
				PointF[] array10 = new PointF[25];
				Pen pen = new Pen(Color.Black, 1f);
				Pen pen2 = new Pen(Color.Orange, 1f);
				Pen pen3 = new Pen(Color.Red, 1f);
				Pen pen4 = new Pen(Color.Blue, 1f);
				Pen pen5 = new Pen(Color.Green, 1f);
				Pen pen6 = new Pen(Color.LightBlue, 1f);
				Pen pen7 = new Pen(Color.Yellow, 1f);
				Pen pen8 = new Pen(Color.Cyan, 1f);
				Pen pen9 = new Pen(Color.Violet, 1f);
				Pen pen10 = new Pen(Color.Magenta, 1f);
				Font font = new Font("Microsoft Sans Serif", 8f);
				SolidBrush brush = new SolidBrush(Color.Black);
				float num = (float)panelSpectrum.Width / (float)(HSIParam.filter_nr_bands - 1);
				float num2 = (float)panelSpectrum.Height - 2f;
				float num3 = (float)((double)num2 / 255.0);
				float num4 = 0f;
				int num5 = 0;
				while (num5 < HSIParam.filter_nr_bands)
				{
					ref PointF reference = ref array[num5];
					ref PointF reference2 = ref array2[num5];
					ref PointF reference3 = ref array3[num5];
					ref PointF reference4 = ref array4[num5];
					ref PointF reference5 = ref array5[num5];
					ref PointF reference6 = ref array6[num5];
					ref PointF reference7 = ref array7[num5];
					ref PointF reference8 = ref array8[num5];
					ref PointF reference9 = ref array9[num5];
					float num7 = array10[num5].X = num4;
					float num9 = reference9.X = num7;
					float num11 = reference8.X = num9;
					float num13 = reference7.X = num11;
					float num15 = reference6.X = num13;
					float num17 = reference5.X = num15;
					float num19 = reference4.X = num17;
					float num21 = reference3.X = num19;
					float num24 = reference.X = (reference2.X = num21);
					if (mouseActive && mousePos.X >= 0)
					{
						if (useInterpolatedImage)
						{
							array[num5].Y = num2 - num3 * (float)hsi_image.imageHSI_interpol[mousePos.Y, mousePos.X, num5];
						}
						else
						{
							array[num5].Y = num2 - num3 * (float)hsi_image.imageHSI_spatial[mousePos.Y, mousePos.X, num5];
						}
					}
					if (compareSpectraMode != 0)
					{
						array2[num5].Y = num2 - num3 * (float)hsi_image.spectrum_Use[0, num5];
						array3[num5].Y = num2 - num3 * (float)hsi_image.spectrum_Use[1, num5];
						if (hsi_image.numMaterialsToUse >= 2)
						{
							array4[num5].Y = num2 - num3 * (float)hsi_image.spectrum_Use[2, num5];
						}
						if (hsi_image.numMaterialsToUse >= 3)
						{
							array5[num5].Y = num2 - num3 * (float)hsi_image.spectrum_Use[3, num5];
						}
						if (hsi_image.numMaterialsToUse >= 4)
						{
							array6[num5].Y = num2 - num3 * (float)hsi_image.spectrum_Use[4, num5];
						}
						if (hsi_image.numMaterialsToUse >= 5)
						{
							array7[num5].Y = num2 - num3 * (float)hsi_image.spectrum_Use[5, num5];
						}
						if (hsi_image.numMaterialsToUse >= 6)
						{
							array8[num5].Y = num2 - num3 * (float)hsi_image.spectrum_Use[6, num5];
						}
						if (hsi_image.numMaterialsToUse >= 7)
						{
							array9[num5].Y = num2 - num3 * (float)hsi_image.spectrum_Use[7, num5];
						}
						if (hsi_image.numMaterialsToUse >= 8)
						{
							array10[num5].Y = num2 - num3 * (float)hsi_image.spectrum_Use[8, num5];
						}
					}
					num5++;
					num4 += num;
				}
				if (mouseActive && mousePos.X >= 0)
				{
					graphics.DrawCurve(pen, array, 0, HSIParam.filter_nr_bands - 1, 0.5f);
				}
				if (compareSpectraMode != 0)
				{
					graphics.DrawCurve(pen2, array2, 0, HSIParam.filter_nr_bands - 1, 0.5f);
					graphics.DrawCurve(pen3, array3, 0, HSIParam.filter_nr_bands - 1, 0.5f);
					if (hsi_image.numMaterialsToUse >= 2)
					{
						graphics.DrawCurve(pen4, array4, 0, HSIParam.filter_nr_bands - 1, 0.5f);
					}
					if (hsi_image.numMaterialsToUse >= 3)
					{
						graphics.DrawCurve(pen5, array5, 0, HSIParam.filter_nr_bands - 1, 0.5f);
					}
					if (hsi_image.numMaterialsToUse >= 4)
					{
						graphics.DrawCurve(pen6, array6, 0, HSIParam.filter_nr_bands - 1, 0.5f);
					}
					if (hsi_image.numMaterialsToUse >= 5)
					{
						graphics.DrawCurve(pen7, array7, 0, HSIParam.filter_nr_bands - 1, 0.5f);
					}
					if (hsi_image.numMaterialsToUse >= 6)
					{
						graphics.DrawCurve(pen8, array8, 0, HSIParam.filter_nr_bands - 1, 0.5f);
					}
					if (hsi_image.numMaterialsToUse >= 7)
					{
						graphics.DrawCurve(pen9, array9, 0, HSIParam.filter_nr_bands - 1, 0.5f);
					}
					if (hsi_image.numMaterialsToUse >= 8)
					{
						graphics.DrawCurve(pen10, array10, 0, HSIParam.filter_nr_bands - 1, 0.5f);
					}
				}
				if (mouseActive && mousePos.X >= 0)
				{
					string s = mousePos.X + "; " + mousePos.Y;
					graphics.DrawString(s, font, brush, 0f, 0f);
				}
			}
		}

		private void buttonFilterResponse_Click(object sender, EventArgs e)
		{
			if (HSIParam != null)
			{
				FormFilterResponse formFilterResponse = new FormFilterResponse(HSIParam, labelCameraModel.Text);
				formFilterResponse.Show();
				formFilterResponse.Activate();
				formFilterResponse.Focus();
			}
		}

		private void setbuttonDistanceText()
		{
			switch (distanceMode)
			{
			case 0:
				buttonDistance.Text = "RMS->VdMeer";
				break;
			case 1:
				buttonDistance.Text = "VdMeer->MSAM";
				break;
			case 2:
				buttonDistance.Text = "MSAM->SID";
				break;
			case 3:
				buttonDistance.Text = "SID->SID-SAM";
				break;
			case 4:
				buttonDistance.Text = "SID-SAM->JM-SAM";
				break;
			default:
				buttonDistance.Text = "JM-SAM->RMS";
				break;
			}
		}

		private void buttonDistance_Click(object sender, EventArgs e)
		{
			distanceMode = ++distanceMode % 6;
			setbuttonDistanceText();
		}

		private void setShowMeasureFiledsText()
		{
			buttonShowMeasureFileds.Text = (showMeasurementFields ? "-ShowMeasure" : "+ShowMeasure");
		}

		private void buttonShowMeasureFileds_Click(object sender, EventArgs e)
		{
			showMeasurementFields = ((!showMeasurementFields) ? true : false);
			setShowMeasureFiledsText();
		}

		private void numericUpDown_numMaterials_ValueChanged(object sender, EventArgs e)
		{
			numMaterials = (int)numericUpDown_numMaterials.Value;
		}

		private void numericUpDown_NumFramesToAvg_ValueChanged(object sender, EventArgs e)
		{
			numFramesToAvg = (int)numericUpDown_NumFramesToAvg.Value;
		}

		private void buttonSaveCube_Click(object sender, EventArgs e)
		{
			startSaveHSIcube = true;
		}

		private void setbuttonResolutionText()
		{
			switch (resolutionMode)
			{
			case 0:
				buttonResolution.Text = "RAW->spatial";
				break;
			case 1:
				buttonResolution.Text = "spatial->interpol";
				break;
			default:
				buttonResolution.Text = "interpol->RAW";
				break;
			}
		}

		private void buttonResolution_Click(object sender, EventArgs e)
		{
			resolutionMode = ++resolutionMode % 3;
			setbuttonResolutionText();
		}

		private void setButtonParallelText()
		{
			buttonParallel.Text = (statusParallelProcessing ? "- parallel proc." : "+ parallel proc.");
		}

		private void buttonParallel_Click(object sender, EventArgs e)
		{
			statusParallelProcessing = ((!statusParallelProcessing) ? true : false);
			setButtonParallelText();
		}

		private void buttonSaveConfig_Click(object sender, EventArgs e)
		{
			bool flag = false;
			DemoImageFile demoImageFile = new DemoImageFile(hsi_image, this, camera_1.xi_device_sn);
			if (demoImageFile.saveDemoImageFile())
			{
				richTextBox_Cam1.AppendText($"demoImageFile successfully saved\n");
			}
			else
			{
				richTextBox_Cam1.AppendText($"Error saving demoImageFile\n");
			}
			richTextBox_Cam1.ScrollToCaret();
		}

		private void label11_Click(object sender, EventArgs e)
		{
		}

		private void toggleFullScreen()
		{
			if (!fullScreenDisplayActive)
			{
				boundsTop = base.Bounds.Top;
				boundsLeft = base.Bounds.Left;
				boundsHeight = base.Bounds.Height;
				boundsWidth = base.Bounds.Width;
				base.FormBorderStyle = FormBorderStyle.None;
				base.TopMost = true;
				base.Bounds = Screen.PrimaryScreen.Bounds;
				fullScreenDisplayActive = true;
			}
			else
			{
				SetBounds(boundsLeft, boundsTop, boundsWidth, boundsHeight, BoundsSpecified.All);
				base.FormBorderStyle = FormBorderStyle.Sizable;
				base.TopMost = false;
				fullScreenDisplayActive = false;
			}
		}

		private void buttonFullScreen_Click(object sender, EventArgs e)
		{
			toggleFullScreen();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(xiSpec01.Form1));
			buttonStart = new System.Windows.Forms.Button();
			richTextBox_Cam1 = new System.Windows.Forms.RichTextBox();
			buttonStop = new System.Windows.Forms.Button();
			trackBar_ExpTime = new System.Windows.Forms.TrackBar();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			buttonToggleInfoBox = new System.Windows.Forms.Button();
			labelCameraModel = new System.Windows.Forms.Label();
			buttonMaximize = new System.Windows.Forms.Button();
			buttonDerivation1 = new System.Windows.Forms.Button();
			buttonMeasure = new System.Windows.Forms.Button();
			buttonCompare = new System.Windows.Forms.Button();
			buttonWhiteImage = new System.Windows.Forms.Button();
			buttonDarkImage = new System.Windows.Forms.Button();
			label4 = new System.Windows.Forms.Label();
			textBox_filterStart = new System.Windows.Forms.TextBox();
			label5 = new System.Windows.Forms.Label();
			textBox_filterEnd = new System.Windows.Forms.TextBox();
			label6 = new System.Windows.Forms.Label();
			checkBoxWhiteImage = new System.Windows.Forms.CheckBox();
			checkBoxDarkImage = new System.Windows.Forms.CheckBox();
			checkBoxMeasured = new System.Windows.Forms.CheckBox();
			buttonStretch = new System.Windows.Forms.Button();
			label10 = new System.Windows.Forms.Label();
			label_bandNR = new System.Windows.Forms.Label();
			trackBar_bandNR = new System.Windows.Forms.TrackBar();
			buttonImageViewMode = new System.Windows.Forms.Button();
			panelSpectrum = new System.Windows.Forms.Panel();
			buttonFilterResponse = new System.Windows.Forms.Button();
			buttonDistance = new System.Windows.Forms.Button();
			buttonShowMeasureFileds = new System.Windows.Forms.Button();
			label7 = new System.Windows.Forms.Label();
			numericUpDown_numMaterials = new System.Windows.Forms.NumericUpDown();
			buttonSaveCube = new System.Windows.Forms.Button();
			buttonResolution = new System.Windows.Forms.Button();
			buttonParallel = new System.Windows.Forms.Button();
			imageBox1 = new Cyotek.Windows.Forms.ImageBox();
			buttonSaveConfig = new System.Windows.Forms.Button();
			label8 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			numericUpDown_NumFramesToAvg = new System.Windows.Forms.NumericUpDown();
			textBoxLiveDemo = new System.Windows.Forms.TextBox();
			panel1 = new System.Windows.Forms.Panel();
			panel2 = new System.Windows.Forms.Panel();
			panel3 = new System.Windows.Forms.Panel();
			buttonFullScreen = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)trackBar_ExpTime).BeginInit();
			((System.ComponentModel.ISupportInitialize)trackBar_bandNR).BeginInit();
			((System.ComponentModel.ISupportInitialize)numericUpDown_numMaterials).BeginInit();
			((System.ComponentModel.ISupportInitialize)numericUpDown_NumFramesToAvg).BeginInit();
			panel2.SuspendLayout();
			SuspendLayout();
			buttonStart.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			buttonStart.Location = new System.Drawing.Point(1123, 60);
			buttonStart.Margin = new System.Windows.Forms.Padding(4);
			buttonStart.Name = "buttonStart";
			buttonStart.Size = new System.Drawing.Size(119, 28);
			buttonStart.TabIndex = 0;
			buttonStart.Text = "start";
			buttonStart.UseVisualStyleBackColor = true;
			buttonStart.Click += new System.EventHandler(buttonStart_Click);
			richTextBox_Cam1.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			richTextBox_Cam1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			richTextBox_Cam1.ForeColor = System.Drawing.SystemColors.WindowText;
			richTextBox_Cam1.Location = new System.Drawing.Point(617, 571);
			richTextBox_Cam1.Margin = new System.Windows.Forms.Padding(4);
			richTextBox_Cam1.Name = "richTextBox_Cam1";
			richTextBox_Cam1.Size = new System.Drawing.Size(492, 133);
			richTextBox_Cam1.TabIndex = 1;
			richTextBox_Cam1.Text = "";
			buttonStop.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			buttonStop.Location = new System.Drawing.Point(1123, 124);
			buttonStop.Margin = new System.Windows.Forms.Padding(4);
			buttonStop.Name = "buttonStop";
			buttonStop.Size = new System.Drawing.Size(119, 28);
			buttonStop.TabIndex = 8;
			buttonStop.Text = "stop";
			buttonStop.UseVisualStyleBackColor = true;
			buttonStop.Click += new System.EventHandler(buttonStop_Click);
			trackBar_ExpTime.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			trackBar_ExpTime.LargeChange = 1000;
			trackBar_ExpTime.Location = new System.Drawing.Point(1119, 175);
			trackBar_ExpTime.Margin = new System.Windows.Forms.Padding(4);
			trackBar_ExpTime.Maximum = 500000;
			trackBar_ExpTime.Minimum = 50;
			trackBar_ExpTime.Name = "trackBar_ExpTime";
			trackBar_ExpTime.Orientation = System.Windows.Forms.Orientation.Vertical;
			trackBar_ExpTime.Size = new System.Drawing.Size(45, 97);
			trackBar_ExpTime.SmallChange = 500;
			trackBar_ExpTime.TabIndex = 10;
			trackBar_ExpTime.TickFrequency = 25000;
			trackBar_ExpTime.Value = 1000;
			trackBar_ExpTime.ValueChanged += new System.EventHandler(trackBar_ExpTime_ValueChanged);
			label1.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			label1.AutoSize = true;
			label1.ForeColor = System.Drawing.SystemColors.ControlText;
			label1.Location = new System.Drawing.Point(1119, 158);
			label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(95, 16);
			label1.TabIndex = 11;
			label1.Text = "exposure time:";
			label2.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			label2.AutoSize = true;
			label2.ForeColor = System.Drawing.SystemColors.ControlText;
			label2.Location = new System.Drawing.Point(1165, 181);
			label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(72, 16);
			label2.TabIndex = 12;
			label2.Text = "value [µs]: ";
			label3.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			label3.AutoSize = true;
			label3.ForeColor = System.Drawing.SystemColors.ControlText;
			label3.Location = new System.Drawing.Point(1165, 202);
			label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(72, 16);
			label3.TabIndex = 13;
			label3.Text = "value [µs]: ";
			buttonToggleInfoBox.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			buttonToggleInfoBox.Location = new System.Drawing.Point(1123, 673);
			buttonToggleInfoBox.Margin = new System.Windows.Forms.Padding(4);
			buttonToggleInfoBox.Name = "buttonToggleInfoBox";
			buttonToggleInfoBox.Size = new System.Drawing.Size(119, 32);
			buttonToggleInfoBox.TabIndex = 14;
			buttonToggleInfoBox.Text = "toggleInfoBox";
			buttonToggleInfoBox.UseVisualStyleBackColor = true;
			buttonToggleInfoBox.Click += new System.EventHandler(buttonToggleInfoBox_Click);
			labelCameraModel.AutoSize = true;
			labelCameraModel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			labelCameraModel.ForeColor = System.Drawing.SystemColors.ControlText;
			labelCameraModel.Location = new System.Drawing.Point(38, 11);
			labelCameraModel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			labelCameraModel.Name = "labelCameraModel";
			labelCameraModel.Size = new System.Drawing.Size(293, 15);
			labelCameraModel.TabIndex = 15;
			labelCameraModel.Text = "MQ022HG-IM-SM5X5-NIR, SerNr: 88888888             ";
			buttonMaximize.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			buttonMaximize.Location = new System.Drawing.Point(1123, 490);
			buttonMaximize.Margin = new System.Windows.Forms.Padding(4);
			buttonMaximize.Name = "buttonMaximize";
			buttonMaximize.Size = new System.Drawing.Size(119, 28);
			buttonMaximize.TabIndex = 16;
			buttonMaximize.TabStop = false;
			buttonMaximize.Text = "int -> max";
			buttonMaximize.UseVisualStyleBackColor = true;
			buttonMaximize.Click += new System.EventHandler(buttonMaximize_Click);
			buttonDerivation1.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			buttonDerivation1.Location = new System.Drawing.Point(1123, 522);
			buttonDerivation1.Margin = new System.Windows.Forms.Padding(4);
			buttonDerivation1.Name = "buttonDerivation1";
			buttonDerivation1.Size = new System.Drawing.Size(119, 28);
			buttonDerivation1.TabIndex = 17;
			buttonDerivation1.TabStop = false;
			buttonDerivation1.Text = "int->reflectance";
			buttonDerivation1.UseVisualStyleBackColor = true;
			buttonDerivation1.Click += new System.EventHandler(buttonDerivation1_Click);
			buttonMeasure.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			buttonMeasure.Location = new System.Drawing.Point(1213, 623);
			buttonMeasure.Margin = new System.Windows.Forms.Padding(4);
			buttonMeasure.Name = "buttonMeasure";
			buttonMeasure.Size = new System.Drawing.Size(28, 28);
			buttonMeasure.TabIndex = 18;
			buttonMeasure.Text = "M";
			buttonMeasure.UseVisualStyleBackColor = true;
			buttonMeasure.Click += new System.EventHandler(buttonMeasure_Click);
			buttonCompare.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			buttonCompare.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			buttonCompare.Location = new System.Drawing.Point(1123, 554);
			buttonCompare.Margin = new System.Windows.Forms.Padding(4);
			buttonCompare.Name = "buttonCompare";
			buttonCompare.Size = new System.Drawing.Size(119, 28);
			buttonCompare.TabIndex = 19;
			buttonCompare.TabStop = false;
			buttonCompare.Text = "image->hard";
			buttonCompare.UseVisualStyleBackColor = true;
			buttonCompare.Click += new System.EventHandler(buttonCompare_Click);
			buttonWhiteImage.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			buttonWhiteImage.Location = new System.Drawing.Point(1123, 623);
			buttonWhiteImage.Margin = new System.Windows.Forms.Padding(4);
			buttonWhiteImage.Name = "buttonWhiteImage";
			buttonWhiteImage.Size = new System.Drawing.Size(28, 28);
			buttonWhiteImage.TabIndex = 20;
			buttonWhiteImage.Text = "W";
			buttonWhiteImage.UseVisualStyleBackColor = true;
			buttonWhiteImage.Click += new System.EventHandler(buttonWhiteImage_Click);
			buttonDarkImage.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			buttonDarkImage.Location = new System.Drawing.Point(1169, 623);
			buttonDarkImage.Margin = new System.Windows.Forms.Padding(4);
			buttonDarkImage.Name = "buttonDarkImage";
			buttonDarkImage.Size = new System.Drawing.Size(28, 28);
			buttonDarkImage.TabIndex = 21;
			buttonDarkImage.Text = "D";
			buttonDarkImage.UseVisualStyleBackColor = true;
			buttonDarkImage.Click += new System.EventHandler(buttonDarkImage_Click);
			label4.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			label4.AutoSize = true;
			label4.ForeColor = System.Drawing.SystemColors.ControlText;
			label4.Location = new System.Drawing.Point(1119, 9);
			label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(73, 16);
			label4.TabIndex = 22;
			label4.Text = "filter range:";
			textBox_filterStart.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			textBox_filterStart.Location = new System.Drawing.Point(1123, 28);
			textBox_filterStart.Margin = new System.Windows.Forms.Padding(4);
			textBox_filterStart.Name = "textBox_filterStart";
			textBox_filterStart.Size = new System.Drawing.Size(41, 22);
			textBox_filterStart.TabIndex = 23;
			textBox_filterStart.Text = "0";
			textBox_filterStart.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			label5.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			label5.AutoSize = true;
			label5.ForeColor = System.Drawing.SystemColors.ControlText;
			label5.Location = new System.Drawing.Point(1165, 32);
			label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(12, 16);
			label5.TabIndex = 24;
			label5.Text = "-";
			textBox_filterEnd.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			textBox_filterEnd.Location = new System.Drawing.Point(1176, 28);
			textBox_filterEnd.Margin = new System.Windows.Forms.Padding(4);
			textBox_filterEnd.Name = "textBox_filterEnd";
			textBox_filterEnd.Size = new System.Drawing.Size(41, 22);
			textBox_filterEnd.TabIndex = 25;
			textBox_filterEnd.Tag = "";
			textBox_filterEnd.Text = "0";
			textBox_filterEnd.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			label6.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			label6.AutoSize = true;
			label6.ForeColor = System.Drawing.SystemColors.ControlText;
			label6.Location = new System.Drawing.Point(1220, 32);
			label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(26, 16);
			label6.TabIndex = 26;
			label6.Text = "nm";
			checkBoxWhiteImage.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBoxWhiteImage.AutoSize = true;
			checkBoxWhiteImage.Enabled = false;
			checkBoxWhiteImage.Location = new System.Drawing.Point(1136, 651);
			checkBoxWhiteImage.Margin = new System.Windows.Forms.Padding(4);
			checkBoxWhiteImage.Name = "checkBoxWhiteImage";
			checkBoxWhiteImage.Size = new System.Drawing.Size(15, 14);
			checkBoxWhiteImage.TabIndex = 30;
			checkBoxWhiteImage.UseVisualStyleBackColor = true;
			checkBoxDarkImage.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBoxDarkImage.AutoSize = true;
			checkBoxDarkImage.Enabled = false;
			checkBoxDarkImage.Location = new System.Drawing.Point(1182, 651);
			checkBoxDarkImage.Margin = new System.Windows.Forms.Padding(4);
			checkBoxDarkImage.Name = "checkBoxDarkImage";
			checkBoxDarkImage.Size = new System.Drawing.Size(15, 14);
			checkBoxDarkImage.TabIndex = 31;
			checkBoxDarkImage.UseVisualStyleBackColor = true;
			checkBoxMeasured.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBoxMeasured.AutoSize = true;
			checkBoxMeasured.Enabled = false;
			checkBoxMeasured.Location = new System.Drawing.Point(1226, 651);
			checkBoxMeasured.Margin = new System.Windows.Forms.Padding(4);
			checkBoxMeasured.Name = "checkBoxMeasured";
			checkBoxMeasured.Size = new System.Drawing.Size(15, 14);
			checkBoxMeasured.TabIndex = 32;
			checkBoxMeasured.UseVisualStyleBackColor = true;
			buttonStretch.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			buttonStretch.Location = new System.Drawing.Point(1123, 346);
			buttonStretch.Margin = new System.Windows.Forms.Padding(4);
			buttonStretch.Name = "buttonStretch";
			buttonStretch.Size = new System.Drawing.Size(119, 28);
			buttonStretch.TabIndex = 33;
			buttonStretch.TabStop = false;
			buttonStretch.Text = "zoom -> fit";
			buttonStretch.UseVisualStyleBackColor = true;
			buttonStretch.Click += new System.EventHandler(buttonStretch_Click);
			label10.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			label10.AutoSize = true;
			label10.ForeColor = System.Drawing.SystemColors.ControlText;
			label10.Location = new System.Drawing.Point(1167, 236);
			label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(70, 16);
			label10.TabIndex = 34;
			label10.Text = "only band:";
			label_bandNR.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			label_bandNR.AutoSize = true;
			label_bandNR.ForeColor = System.Drawing.SystemColors.ControlText;
			label_bandNR.Location = new System.Drawing.Point(1201, 256);
			label_bandNR.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label_bandNR.Name = "label_bandNR";
			label_bandNR.Size = new System.Drawing.Size(29, 16);
			label_bandNR.TabIndex = 35;
			label_bandNR.Text = "188";
			trackBar_bandNR.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			trackBar_bandNR.Location = new System.Drawing.Point(1181, 272);
			trackBar_bandNR.Margin = new System.Windows.Forms.Padding(4);
			trackBar_bandNR.Maximum = 24;
			trackBar_bandNR.Minimum = -1;
			trackBar_bandNR.Name = "trackBar_bandNR";
			trackBar_bandNR.Orientation = System.Windows.Forms.Orientation.Vertical;
			trackBar_bandNR.Size = new System.Drawing.Size(45, 74);
			trackBar_bandNR.TabIndex = 36;
			trackBar_bandNR.TickFrequency = 5;
			trackBar_bandNR.ValueChanged += new System.EventHandler(trackBar_bandNR_ValueChanged);
			buttonImageViewMode.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			buttonImageViewMode.Location = new System.Drawing.Point(1123, 378);
			buttonImageViewMode.Margin = new System.Windows.Forms.Padding(4);
			buttonImageViewMode.Name = "buttonImageViewMode";
			buttonImageViewMode.Size = new System.Drawing.Size(119, 28);
			buttonImageViewMode.TabIndex = 37;
			buttonImageViewMode.TabStop = false;
			buttonImageViewMode.Text = "avg -> max";
			buttonImageViewMode.UseVisualStyleBackColor = true;
			buttonImageViewMode.Click += new System.EventHandler(buttonImageViewMode_Click);
			panelSpectrum.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			panelSpectrum.BackColor = System.Drawing.SystemColors.ControlLightLight;
			panelSpectrum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			panelSpectrum.Location = new System.Drawing.Point(16, 571);
			panelSpectrum.Margin = new System.Windows.Forms.Padding(4);
			panelSpectrum.Name = "panelSpectrum";
			panelSpectrum.Size = new System.Drawing.Size(593, 134);
			panelSpectrum.TabIndex = 38;
			panelSpectrum.Paint += new System.Windows.Forms.PaintEventHandler(panelSpectrum_Paint);
			buttonFilterResponse.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			buttonFilterResponse.Location = new System.Drawing.Point(1123, 92);
			buttonFilterResponse.Margin = new System.Windows.Forms.Padding(4);
			buttonFilterResponse.Name = "buttonFilterResponse";
			buttonFilterResponse.Size = new System.Drawing.Size(119, 28);
			buttonFilterResponse.TabIndex = 39;
			buttonFilterResponse.Text = "filter response";
			buttonFilterResponse.UseVisualStyleBackColor = true;
			buttonFilterResponse.Click += new System.EventHandler(buttonFilterResponse_Click);
			buttonDistance.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			buttonDistance.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			buttonDistance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			buttonDistance.Location = new System.Drawing.Point(1123, 585);
			buttonDistance.Margin = new System.Windows.Forms.Padding(4);
			buttonDistance.Name = "buttonDistance";
			buttonDistance.Size = new System.Drawing.Size(119, 28);
			buttonDistance.TabIndex = 40;
			buttonDistance.TabStop = false;
			buttonDistance.Text = "RMS->VdMeer";
			buttonDistance.UseVisualStyleBackColor = true;
			buttonDistance.Click += new System.EventHandler(buttonDistance_Click);
			buttonShowMeasureFileds.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			buttonShowMeasureFileds.Location = new System.Drawing.Point(1123, 446);
			buttonShowMeasureFileds.Margin = new System.Windows.Forms.Padding(4);
			buttonShowMeasureFileds.Name = "buttonShowMeasureFileds";
			buttonShowMeasureFileds.Size = new System.Drawing.Size(119, 28);
			buttonShowMeasureFileds.TabIndex = 41;
			buttonShowMeasureFileds.TabStop = false;
			buttonShowMeasureFileds.Text = "+ShowMeasure";
			buttonShowMeasureFileds.UseVisualStyleBackColor = true;
			buttonShowMeasureFileds.Click += new System.EventHandler(buttonShowMeasureFileds_Click);
			label7.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			label7.AutoSize = true;
			label7.ForeColor = System.Drawing.SystemColors.ControlText;
			label7.Location = new System.Drawing.Point(1120, 421);
			label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(66, 16);
			label7.TabIndex = 42;
			label7.Text = "materials:";
			numericUpDown_numMaterials.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			numericUpDown_numMaterials.Location = new System.Drawing.Point(1196, 418);
			numericUpDown_numMaterials.Margin = new System.Windows.Forms.Padding(4);
			numericUpDown_numMaterials.Maximum = new decimal(new int[4]
			{
				8,
				0,
				0,
				0
			});
			numericUpDown_numMaterials.Minimum = new decimal(new int[4]
			{
				1,
				0,
				0,
				0
			});
			numericUpDown_numMaterials.Name = "numericUpDown_numMaterials";
			numericUpDown_numMaterials.Size = new System.Drawing.Size(45, 22);
			numericUpDown_numMaterials.TabIndex = 44;
			numericUpDown_numMaterials.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			numericUpDown_numMaterials.Value = new decimal(new int[4]
			{
				3,
				0,
				0,
				0
			});
			numericUpDown_numMaterials.ValueChanged += new System.EventHandler(numericUpDown_numMaterials_ValueChanged);
			buttonSaveCube.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			buttonSaveCube.Location = new System.Drawing.Point(968, 7);
			buttonSaveCube.Margin = new System.Windows.Forms.Padding(4);
			buttonSaveCube.Name = "buttonSaveCube";
			buttonSaveCube.Size = new System.Drawing.Size(141, 28);
			buttonSaveCube.TabIndex = 45;
			buttonSaveCube.Text = "save HSI-cube";
			buttonSaveCube.UseVisualStyleBackColor = true;
			buttonSaveCube.Click += new System.EventHandler(buttonSaveCube_Click);
			buttonResolution.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			buttonResolution.Location = new System.Drawing.Point(799, 7);
			buttonResolution.Margin = new System.Windows.Forms.Padding(4);
			buttonResolution.Name = "buttonResolution";
			buttonResolution.Size = new System.Drawing.Size(161, 28);
			buttonResolution.TabIndex = 46;
			buttonResolution.TabStop = false;
			buttonResolution.Text = "spatial -> RAW";
			buttonResolution.UseVisualStyleBackColor = true;
			buttonResolution.Click += new System.EventHandler(buttonResolution_Click);
			buttonParallel.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			buttonParallel.Location = new System.Drawing.Point(649, 7);
			buttonParallel.Margin = new System.Windows.Forms.Padding(4);
			buttonParallel.Name = "buttonParallel";
			buttonParallel.Size = new System.Drawing.Size(141, 28);
			buttonParallel.TabIndex = 47;
			buttonParallel.Text = "+ parallel proc.";
			buttonParallel.UseVisualStyleBackColor = true;
			buttonParallel.Click += new System.EventHandler(buttonParallel_Click);
			imageBox1.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
			imageBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			imageBox1.GridColor = System.Drawing.SystemColors.Control;
			imageBox1.GridColorAlternate = System.Drawing.SystemColors.Control;
			imageBox1.Location = new System.Drawing.Point(16, 43);
			imageBox1.Margin = new System.Windows.Forms.Padding(4);
			imageBox1.Name = "imageBox1";
			imageBox1.Size = new System.Drawing.Size(1093, 519);
			imageBox1.TabIndex = 48;
			imageBox1.MouseEnter += new System.EventHandler(imageBox1_MouseEnter);
			imageBox1.MouseLeave += new System.EventHandler(imageBox1_MouseLeave);
			imageBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(imageBox1_MouseMove);
			buttonSaveConfig.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			buttonSaveConfig.Location = new System.Drawing.Point(505, 7);
			buttonSaveConfig.Margin = new System.Windows.Forms.Padding(4);
			buttonSaveConfig.Name = "buttonSaveConfig";
			buttonSaveConfig.Size = new System.Drawing.Size(104, 28);
			buttonSaveConfig.TabIndex = 49;
			buttonSaveConfig.Text = "save config";
			buttonSaveConfig.UseVisualStyleBackColor = true;
			buttonSaveConfig.Click += new System.EventHandler(buttonSaveConfig_Click);
			label8.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			label8.AutoSize = true;
			label8.ForeColor = System.Drawing.SystemColors.ControlText;
			label8.Location = new System.Drawing.Point(1120, 276);
			label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(49, 16);
			label8.TabIndex = 50;
			label8.Text = "frames";
			label9.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			label9.AutoSize = true;
			label9.ForeColor = System.Drawing.SystemColors.ControlText;
			label9.Location = new System.Drawing.Point(1120, 292);
			label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(48, 16);
			label9.TabIndex = 51;
			label9.Text = "to avg:";
			numericUpDown_NumFramesToAvg.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			numericUpDown_NumFramesToAvg.Location = new System.Drawing.Point(1123, 311);
			numericUpDown_NumFramesToAvg.Margin = new System.Windows.Forms.Padding(4);
			numericUpDown_NumFramesToAvg.Maximum = new decimal(new int[4]
			{
				20,
				0,
				0,
				0
			});
			numericUpDown_NumFramesToAvg.Minimum = new decimal(new int[4]
			{
				1,
				0,
				0,
				0
			});
			numericUpDown_NumFramesToAvg.Name = "numericUpDown_NumFramesToAvg";
			numericUpDown_NumFramesToAvg.Size = new System.Drawing.Size(45, 22);
			numericUpDown_NumFramesToAvg.TabIndex = 52;
			numericUpDown_NumFramesToAvg.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			numericUpDown_NumFramesToAvg.Value = new decimal(new int[4]
			{
				1,
				0,
				0,
				0
			});
			numericUpDown_NumFramesToAvg.ValueChanged += new System.EventHandler(numericUpDown_NumFramesToAvg_ValueChanged);
			textBoxLiveDemo.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			textBoxLiveDemo.BackColor = System.Drawing.SystemColors.Control;
			textBoxLiveDemo.BorderStyle = System.Windows.Forms.BorderStyle.None;
			textBoxLiveDemo.Font = new System.Drawing.Font("Microsoft Sans Serif", 30f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			textBoxLiveDemo.ForeColor = System.Drawing.Color.Red;
			textBoxLiveDemo.Location = new System.Drawing.Point(32, 5);
			textBoxLiveDemo.Margin = new System.Windows.Forms.Padding(4);
			textBoxLiveDemo.Multiline = true;
			textBoxLiveDemo.Name = "textBoxLiveDemo";
			textBoxLiveDemo.Size = new System.Drawing.Size(581, 121);
			textBoxLiveDemo.TabIndex = 53;
			textBoxLiveDemo.Text = "hyperspectral imaging live demonstration";
			panel1.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			panel1.BackgroundImage = (System.Drawing.Image)componentResourceManager.GetObject("panel1.BackgroundImage");
			panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			panel1.Location = new System.Drawing.Point(621, 5);
			panel1.Margin = new System.Windows.Forms.Padding(4);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(340, 121);
			panel1.TabIndex = 0;
			panel2.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			panel2.Controls.Add(textBoxLiveDemo);
			panel2.Controls.Add(panel1);
			panel2.Location = new System.Drawing.Point(151, 576);
			panel2.Margin = new System.Windows.Forms.Padding(4);
			panel2.Name = "panel2";
			panel2.Size = new System.Drawing.Size(965, 128);
			panel2.TabIndex = 54;
			panel3.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			panel3.Location = new System.Drawing.Point(609, 574);
			panel3.Margin = new System.Windows.Forms.Padding(4);
			panel3.Name = "panel3";
			panel3.Size = new System.Drawing.Size(9, 130);
			panel3.TabIndex = 55;
			buttonFullScreen.Image = (System.Drawing.Image)componentResourceManager.GetObject("buttonFullScreen.Image");
			buttonFullScreen.Location = new System.Drawing.Point(-3, 1);
			buttonFullScreen.Name = "buttonFullScreen";
			buttonFullScreen.Size = new System.Drawing.Size(32, 25);
			buttonFullScreen.TabIndex = 56;
			buttonFullScreen.Text = ".";
			buttonFullScreen.UseVisualStyleBackColor = true;
			buttonFullScreen.Click += new System.EventHandler(buttonFullScreen_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 16f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			BackColor = System.Drawing.SystemColors.Control;
			base.ClientSize = new System.Drawing.Size(1252, 716);
			base.Controls.Add(buttonFullScreen);
			base.Controls.Add(panel3);
			base.Controls.Add(numericUpDown_NumFramesToAvg);
			base.Controls.Add(label9);
			base.Controls.Add(label8);
			base.Controls.Add(buttonSaveConfig);
			base.Controls.Add(imageBox1);
			base.Controls.Add(buttonParallel);
			base.Controls.Add(buttonResolution);
			base.Controls.Add(buttonSaveCube);
			base.Controls.Add(numericUpDown_numMaterials);
			base.Controls.Add(label7);
			base.Controls.Add(buttonShowMeasureFileds);
			base.Controls.Add(buttonDistance);
			base.Controls.Add(buttonFilterResponse);
			base.Controls.Add(buttonImageViewMode);
			base.Controls.Add(trackBar_bandNR);
			base.Controls.Add(label_bandNR);
			base.Controls.Add(label10);
			base.Controls.Add(buttonStretch);
			base.Controls.Add(checkBoxMeasured);
			base.Controls.Add(checkBoxDarkImage);
			base.Controls.Add(checkBoxWhiteImage);
			base.Controls.Add(label6);
			base.Controls.Add(textBox_filterEnd);
			base.Controls.Add(label5);
			base.Controls.Add(textBox_filterStart);
			base.Controls.Add(label4);
			base.Controls.Add(buttonDarkImage);
			base.Controls.Add(buttonWhiteImage);
			base.Controls.Add(buttonCompare);
			base.Controls.Add(buttonMeasure);
			base.Controls.Add(buttonDerivation1);
			base.Controls.Add(buttonMaximize);
			base.Controls.Add(labelCameraModel);
			base.Controls.Add(buttonToggleInfoBox);
			base.Controls.Add(label3);
			base.Controls.Add(label2);
			base.Controls.Add(label1);
			base.Controls.Add(trackBar_ExpTime);
			base.Controls.Add(buttonStop);
			base.Controls.Add(buttonStart);
			base.Controls.Add(richTextBox_Cam1);
			base.Controls.Add(panelSpectrum);
			base.Controls.Add(panel2);
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.Margin = new System.Windows.Forms.Padding(4);
			MinimumSize = new System.Drawing.Size(1268, 755);
			base.Name = "Form1";
			Text = "xiSpec01";
			base.SizeChanged += new System.EventHandler(Form1_SizeChanged);
			((System.ComponentModel.ISupportInitialize)trackBar_ExpTime).EndInit();
			((System.ComponentModel.ISupportInitialize)trackBar_bandNR).EndInit();
			((System.ComponentModel.ISupportInitialize)numericUpDown_numMaterials).EndInit();
			((System.ComponentModel.ISupportInitialize)numericUpDown_NumFramesToAvg).EndInit();
			panel2.ResumeLayout(performLayout: false);
			panel2.PerformLayout();
			ResumeLayout(performLayout: false);
			PerformLayout();
		}
	}
}
