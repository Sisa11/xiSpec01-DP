using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace xiSpec01
{
	public class FormFilterResponse : Form
	{
		public HSIParameters HSIParam;

		private CheckBox[] box;

		private Pen[] pen;

		private Color[] color;

		private SolidBrush[] brush;

		private int filterStartGraphics;

		private int filterEndGraphics;

		private IContainer components;

		private Label labelCameraModelFilter;

		private Button buttonClose;

		private Panel panelCurves;

		private CheckBox checkBoxAll;

		private CheckBox checkBox0;

		private CheckBox checkBox1;

		private CheckBox checkBox2;

		private CheckBox checkBox3;

		private CheckBox checkBox4;

		private CheckBox checkBox5;

		private CheckBox checkBox6;

		private CheckBox checkBox7;

		private CheckBox checkBox8;

		private CheckBox checkBox9;

		private CheckBox checkBox10;

		private CheckBox checkBox11;

		private CheckBox checkBox12;

		private CheckBox checkBox13;

		private CheckBox checkBox14;

		private CheckBox checkBox15;

		private CheckBox checkBox16;

		private CheckBox checkBox17;

		private CheckBox checkBox18;

		private CheckBox checkBox19;

		private CheckBox checkBox20;

		private CheckBox checkBox21;

		private CheckBox checkBox22;

		private CheckBox checkBox23;

		private CheckBox checkBox24;

		private Label label2;

		private TextBox textBox_filterStartInput;

		private Label label3;

		private TextBox textBox_filterEndInput;

		private Label label4;

		private Button button_repaint;

		private Panel panel1;

		public FormFilterResponse(HSIParameters param, string CameraModel)
		{
			InitializeComponent();
			HSIParam = param;
			labelCameraModelFilter.Text = $"{CameraModel} , Filter-Range {HSIParam.filterRangeStart}-{HSIParam.filterRangeEnd} nm";
			filterStartGraphics = HSIParam.filterRangeStart;
			filterEndGraphics = HSIParam.filterRangeEnd;
			textBox_filterStartInput.Text = filterStartGraphics.ToString();
			textBox_filterEndInput.Text = filterEndGraphics.ToString();
			box = new CheckBox[25];
			pen = new Pen[25];
			color = new Color[25];
			brush = new SolidBrush[25];
			box[0] = checkBox0;
			box[1] = checkBox1;
			box[2] = checkBox2;
			box[3] = checkBox3;
			box[4] = checkBox4;
			box[5] = checkBox5;
			box[6] = checkBox6;
			box[7] = checkBox7;
			box[8] = checkBox8;
			box[9] = checkBox9;
			box[10] = checkBox10;
			box[11] = checkBox11;
			box[12] = checkBox12;
			box[13] = checkBox13;
			box[14] = checkBox14;
			box[15] = checkBox15;
			box[16] = checkBox16;
			box[17] = checkBox17;
			box[18] = checkBox18;
			box[19] = checkBox19;
			box[20] = checkBox20;
			box[21] = checkBox21;
			box[22] = checkBox22;
			box[23] = checkBox23;
			box[24] = checkBox24;
			for (int i = 0; i < 25; i++)
			{
				color[i] = box[i].BackColor;
				pen[i] = new Pen(color[i]);
				pen[i].Width = 2f;
				brush[i] = new SolidBrush(color[i]);
			}
			if (HSIParam.filter_nr_bands < 25)
			{
				for (int j = HSIParam.filter_nr_bands; j < 25; j++)
				{
					box[j].Enabled = false;
					box[j].Visible = false;
				}
			}
			panelCurves.Invalidate();
		}

		private void paint_responseCurves()
		{
			Graphics graphics = panelCurves.CreateGraphics();
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			double num = (double)panelCurves.Width;
			double num2 = (double)panelCurves.Height;
			double num3 = 15.0;
			double num4 = num2 - 30.0;
			double num5 = num - 30.0;
			double num6 = num2 - 45.0;
			double num7 = num5 / (double)(filterEndGraphics - filterStartGraphics);
			double num8 = num6 / HSIParam.filter_max_value;
			int num9 = (filterEndGraphics - filterStartGraphics) / HSIParam.calibration_resolution + 1;
			if (num9 >= 2)
			{
				Font font = new Font("Microsoft Sans Serif", 8f);
				Pen pen = new Pen(Color.LightGray, 2f);
				Pen pen2 = new Pen(Color.LightGray, 0.5f);
				Pen pen3 = new Pen(Color.Green, 2f);
				Pen pen4 = new Pen(Color.Red, 2f);
				SolidBrush solidBrush = new SolidBrush(Color.Black);
				graphics.DrawLine(pen, 0f, (float)num4, (float)num, (float)num4);
				int num10 = filterStartGraphics / 25 * 25;
				for (num10 = filterStartGraphics / 10 * 10; num10 <= filterEndGraphics; num10 += 10)
				{
					double num11 = (double)(num10 - filterStartGraphics) * num7 + num3;
					if (num10 % 50 == 0)
					{
						string text = num10.ToString();
						SizeF sizeF = graphics.MeasureString(text, font);
						graphics.DrawLine(pen, (float)num11, (float)num4 + 5f, (float)num11, (float)(num4 - num6));
						graphics.DrawString(text, font, solidBrush, (float)(num11 - (double)(sizeF.Width / 2f)), (float)(num4 + 8.0));
					}
					else
					{
						graphics.DrawLine(pen2, (float)num11, (float)num4 + 5f, (float)num11, (float)(num4 - num6));
					}
				}
				if (HSIParam.filter_start_spectral_range >= filterStartGraphics && HSIParam.filter_start_spectral_range <= filterEndGraphics)
				{
					double num11 = (double)(HSIParam.filter_start_spectral_range - filterStartGraphics) * num7 + num3;
					graphics.DrawLine(pen3, (float)num11, (float)num4 + 5f, (float)num11, (float)(num4 - num6));
				}
				if (HSIParam.filter_end_spectral_range >= filterStartGraphics && HSIParam.filter_end_spectral_range <= filterEndGraphics)
				{
					double num11 = (double)(HSIParam.filter_end_spectral_range - filterStartGraphics) * num7 + num3;
					graphics.DrawLine(pen3, (float)num11, (float)num4 + 5f, (float)num11, (float)(num4 - num6));
				}
				if (HSIParam.filterRangeStart >= filterStartGraphics && HSIParam.filterRangeStart <= filterEndGraphics)
				{
					double num11 = (double)(HSIParam.filterRangeStart - filterStartGraphics) * num7 + num3;
					graphics.DrawLine(pen4, (float)num11, (float)num4 + 5f, (float)num11, (float)(num4 - num6));
				}
				if (HSIParam.filterRangeEnd >= filterStartGraphics && HSIParam.filterRangeEnd <= filterEndGraphics)
				{
					double num11 = (double)(HSIParam.filterRangeEnd - filterStartGraphics) * num7 + num3;
					graphics.DrawLine(pen4, (float)num11, (float)num4 + 5f, (float)num11, (float)(num4 - num6));
				}
				PointF[] array = new PointF[num9];
				for (int i = 0; i < HSIParam.filter_nr_bands; i++)
				{
					if (box[i].Checked)
					{
						int num12 = (filterStartGraphics - HSIParam.start_calibration_range) / HSIParam.calibration_resolution;
						double num11 = num3;
						num10 = 0;
						while (num10 < num9)
						{
							array[num10].X = (float)num11;
							array[num10].Y = (float)(num4 - (double)(int)(HSIParam.filter_responses[i, num12] * num8));
							num10++;
							num12++;
							num11 += num7;
						}
						if (num9 <= 2)
						{
							graphics.DrawLines(this.pen[i], array);
						}
						else
						{
							graphics.DrawCurve(this.pen[i], array, 0, num9 - 1, 0.5f);
						}
					}
				}
			}
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void checkBoxAll_CheckedChanged(object sender, EventArgs e)
		{
			for (int i = 0; i < 25; i++)
			{
				box[i].Checked = checkBoxAll.Checked;
			}
			panelCurves.Invalidate();
		}

		private void panelCurves_Paint(object sender, PaintEventArgs e)
		{
			paint_responseCurves();
		}

		private void button_repaint_Click(object sender, EventArgs e)
		{
			if (!int.TryParse(textBox_filterStartInput.Text, out filterStartGraphics))
			{
				filterStartGraphics = HSIParam.filterRangeStart;
			}
			if (!int.TryParse(textBox_filterEndInput.Text, out filterEndGraphics))
			{
				filterEndGraphics = HSIParam.filterRangeEnd;
			}
			if (filterStartGraphics >= filterEndGraphics || filterStartGraphics <= 0 || filterEndGraphics <= 0)
			{
				filterStartGraphics = HSIParam.filterRangeStart;
				filterEndGraphics = HSIParam.filterRangeEnd;
			}
			if (filterStartGraphics < HSIParam.start_calibration_range)
			{
				filterStartGraphics = HSIParam.start_calibration_range;
			}
			if (filterEndGraphics > HSIParam.end_calibration_range)
			{
				filterEndGraphics = HSIParam.end_calibration_range;
			}
			textBox_filterStartInput.Text = filterStartGraphics.ToString();
			textBox_filterEndInput.Text = filterEndGraphics.ToString();
			panelCurves.Invalidate();
		}

		private void checkBox_CheckedChanged(object sender, EventArgs e)
		{
			panelCurves.Invalidate();
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
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(xiSpec01.FormFilterResponse));
			labelCameraModelFilter = new System.Windows.Forms.Label();
			buttonClose = new System.Windows.Forms.Button();
			panelCurves = new System.Windows.Forms.Panel();
			checkBoxAll = new System.Windows.Forms.CheckBox();
			checkBox0 = new System.Windows.Forms.CheckBox();
			checkBox1 = new System.Windows.Forms.CheckBox();
			checkBox2 = new System.Windows.Forms.CheckBox();
			checkBox3 = new System.Windows.Forms.CheckBox();
			checkBox4 = new System.Windows.Forms.CheckBox();
			checkBox5 = new System.Windows.Forms.CheckBox();
			checkBox6 = new System.Windows.Forms.CheckBox();
			checkBox7 = new System.Windows.Forms.CheckBox();
			checkBox8 = new System.Windows.Forms.CheckBox();
			checkBox9 = new System.Windows.Forms.CheckBox();
			checkBox10 = new System.Windows.Forms.CheckBox();
			checkBox11 = new System.Windows.Forms.CheckBox();
			checkBox12 = new System.Windows.Forms.CheckBox();
			checkBox13 = new System.Windows.Forms.CheckBox();
			checkBox14 = new System.Windows.Forms.CheckBox();
			checkBox15 = new System.Windows.Forms.CheckBox();
			checkBox16 = new System.Windows.Forms.CheckBox();
			checkBox17 = new System.Windows.Forms.CheckBox();
			checkBox18 = new System.Windows.Forms.CheckBox();
			checkBox19 = new System.Windows.Forms.CheckBox();
			checkBox20 = new System.Windows.Forms.CheckBox();
			checkBox21 = new System.Windows.Forms.CheckBox();
			checkBox22 = new System.Windows.Forms.CheckBox();
			checkBox23 = new System.Windows.Forms.CheckBox();
			checkBox24 = new System.Windows.Forms.CheckBox();
			label2 = new System.Windows.Forms.Label();
			textBox_filterStartInput = new System.Windows.Forms.TextBox();
			label3 = new System.Windows.Forms.Label();
			textBox_filterEndInput = new System.Windows.Forms.TextBox();
			label4 = new System.Windows.Forms.Label();
			button_repaint = new System.Windows.Forms.Button();
			panel1 = new System.Windows.Forms.Panel();
			panel1.SuspendLayout();
			SuspendLayout();
			labelCameraModelFilter.AutoSize = true;
			labelCameraModelFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			labelCameraModelFilter.Location = new System.Drawing.Point(12, 9);
			labelCameraModelFilter.Name = "labelCameraModelFilter";
			labelCameraModelFilter.Size = new System.Drawing.Size(463, 15);
			labelCameraModelFilter.TabIndex = 0;
			labelCameraModelFilter.Text = "MQ022HG-IM-SM5X5-600-1000, SerNr: 88888888 , Filter-Range 888-888 nm          ";
			buttonClose.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			buttonClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			buttonClose.Location = new System.Drawing.Point(843, 555);
			buttonClose.Name = "buttonClose";
			buttonClose.Size = new System.Drawing.Size(67, 24);
			buttonClose.TabIndex = 4;
			buttonClose.Text = "close";
			buttonClose.UseVisualStyleBackColor = true;
			buttonClose.Click += new System.EventHandler(buttonClose_Click);
			panelCurves.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
			panelCurves.BackColor = System.Drawing.SystemColors.ControlLightLight;
			panelCurves.Location = new System.Drawing.Point(15, 34);
			panelCurves.Name = "panelCurves";
			panelCurves.Size = new System.Drawing.Size(822, 542);
			panelCurves.TabIndex = 5;
			panelCurves.Paint += new System.Windows.Forms.PaintEventHandler(panelCurves_Paint);
			checkBoxAll.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBoxAll.AutoSize = true;
			checkBoxAll.Checked = true;
			checkBoxAll.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBoxAll.Location = new System.Drawing.Point(843, 7);
			checkBoxAll.Name = "checkBoxAll";
			checkBoxAll.Size = new System.Drawing.Size(68, 17);
			checkBoxAll.TabIndex = 6;
			checkBoxAll.Text = "all bands";
			checkBoxAll.UseVisualStyleBackColor = true;
			checkBoxAll.CheckedChanged += new System.EventHandler(checkBoxAll_CheckedChanged);
			checkBox0.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox0.AutoSize = true;
			checkBox0.BackColor = System.Drawing.Color.Maroon;
			checkBox0.Checked = true;
			checkBox0.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox0.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			checkBox0.Location = new System.Drawing.Point(843, 28);
			checkBox0.Name = "checkBox0";
			checkBox0.Size = new System.Drawing.Size(59, 17);
			checkBox0.TabIndex = 7;
			checkBox0.Text = "band 0";
			checkBox0.UseVisualStyleBackColor = false;
			checkBox0.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox1.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox1.AutoSize = true;
			checkBox1.BackColor = System.Drawing.Color.DarkGoldenrod;
			checkBox1.Checked = true;
			checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox1.Location = new System.Drawing.Point(843, 49);
			checkBox1.Name = "checkBox1";
			checkBox1.Size = new System.Drawing.Size(59, 17);
			checkBox1.TabIndex = 8;
			checkBox1.Text = "band 1";
			checkBox1.UseVisualStyleBackColor = false;
			checkBox1.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox2.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox2.AutoSize = true;
			checkBox2.BackColor = System.Drawing.Color.Gold;
			checkBox2.Checked = true;
			checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox2.Location = new System.Drawing.Point(843, 70);
			checkBox2.Name = "checkBox2";
			checkBox2.Size = new System.Drawing.Size(59, 17);
			checkBox2.TabIndex = 9;
			checkBox2.Text = "band 2";
			checkBox2.UseVisualStyleBackColor = false;
			checkBox2.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox3.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox3.AutoSize = true;
			checkBox3.BackColor = System.Drawing.Color.Green;
			checkBox3.Checked = true;
			checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			checkBox3.Location = new System.Drawing.Point(843, 91);
			checkBox3.Name = "checkBox3";
			checkBox3.Size = new System.Drawing.Size(59, 17);
			checkBox3.TabIndex = 10;
			checkBox3.Text = "band 3";
			checkBox3.UseVisualStyleBackColor = false;
			checkBox3.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox4.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox4.AutoSize = true;
			checkBox4.BackColor = System.Drawing.Color.FromArgb(0, 0, 64);
			checkBox4.Checked = true;
			checkBox4.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			checkBox4.Location = new System.Drawing.Point(843, 112);
			checkBox4.Name = "checkBox4";
			checkBox4.Size = new System.Drawing.Size(59, 17);
			checkBox4.TabIndex = 11;
			checkBox4.Text = "band 4";
			checkBox4.UseVisualStyleBackColor = false;
			checkBox4.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox5.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox5.AutoSize = true;
			checkBox5.BackColor = System.Drawing.Color.FromArgb(0, 0, 192);
			checkBox5.Checked = true;
			checkBox5.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			checkBox5.Location = new System.Drawing.Point(843, 133);
			checkBox5.Name = "checkBox5";
			checkBox5.Size = new System.Drawing.Size(59, 17);
			checkBox5.TabIndex = 12;
			checkBox5.Text = "band 5";
			checkBox5.UseVisualStyleBackColor = false;
			checkBox5.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox6.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox6.AutoSize = true;
			checkBox6.BackColor = System.Drawing.Color.Purple;
			checkBox6.Checked = true;
			checkBox6.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox6.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			checkBox6.Location = new System.Drawing.Point(843, 154);
			checkBox6.Name = "checkBox6";
			checkBox6.Size = new System.Drawing.Size(59, 17);
			checkBox6.TabIndex = 13;
			checkBox6.Text = "band 6";
			checkBox6.UseVisualStyleBackColor = false;
			checkBox6.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox7.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox7.AutoSize = true;
			checkBox7.BackColor = System.Drawing.Color.FromArgb(255, 128, 128);
			checkBox7.Checked = true;
			checkBox7.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox7.Location = new System.Drawing.Point(843, 175);
			checkBox7.Name = "checkBox7";
			checkBox7.Size = new System.Drawing.Size(59, 17);
			checkBox7.TabIndex = 14;
			checkBox7.Text = "band 7";
			checkBox7.UseVisualStyleBackColor = false;
			checkBox7.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox8.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox8.AutoSize = true;
			checkBox8.BackColor = System.Drawing.Color.FromArgb(255, 192, 128);
			checkBox8.Checked = true;
			checkBox8.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox8.Location = new System.Drawing.Point(843, 196);
			checkBox8.Name = "checkBox8";
			checkBox8.Size = new System.Drawing.Size(59, 17);
			checkBox8.TabIndex = 15;
			checkBox8.Text = "band 8";
			checkBox8.UseVisualStyleBackColor = false;
			checkBox8.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox9.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox9.AutoSize = true;
			checkBox9.BackColor = System.Drawing.Color.Olive;
			checkBox9.Checked = true;
			checkBox9.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox9.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			checkBox9.Location = new System.Drawing.Point(843, 217);
			checkBox9.Name = "checkBox9";
			checkBox9.Size = new System.Drawing.Size(59, 17);
			checkBox9.TabIndex = 16;
			checkBox9.Text = "band 9";
			checkBox9.UseVisualStyleBackColor = false;
			checkBox9.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox10.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox10.AutoSize = true;
			checkBox10.BackColor = System.Drawing.Color.GreenYellow;
			checkBox10.Checked = true;
			checkBox10.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox10.Location = new System.Drawing.Point(843, 238);
			checkBox10.Name = "checkBox10";
			checkBox10.Size = new System.Drawing.Size(65, 17);
			checkBox10.TabIndex = 17;
			checkBox10.Text = "band 10";
			checkBox10.UseVisualStyleBackColor = false;
			checkBox10.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox11.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox11.AutoSize = true;
			checkBox11.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
			checkBox11.Checked = true;
			checkBox11.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox11.Location = new System.Drawing.Point(843, 259);
			checkBox11.Name = "checkBox11";
			checkBox11.Size = new System.Drawing.Size(65, 17);
			checkBox11.TabIndex = 18;
			checkBox11.Text = "band 11";
			checkBox11.UseVisualStyleBackColor = false;
			checkBox11.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox12.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox12.AutoSize = true;
			checkBox12.BackColor = System.Drawing.Color.FromArgb(128, 128, 255);
			checkBox12.Checked = true;
			checkBox12.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox12.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			checkBox12.Location = new System.Drawing.Point(843, 280);
			checkBox12.Name = "checkBox12";
			checkBox12.Size = new System.Drawing.Size(65, 17);
			checkBox12.TabIndex = 19;
			checkBox12.Text = "band 12";
			checkBox12.UseVisualStyleBackColor = false;
			checkBox12.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox13.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox13.AutoSize = true;
			checkBox13.BackColor = System.Drawing.Color.FromArgb(255, 128, 255);
			checkBox13.Checked = true;
			checkBox13.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox13.Location = new System.Drawing.Point(843, 301);
			checkBox13.Name = "checkBox13";
			checkBox13.Size = new System.Drawing.Size(65, 17);
			checkBox13.TabIndex = 20;
			checkBox13.Text = "band 13";
			checkBox13.UseVisualStyleBackColor = false;
			checkBox13.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox14.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox14.AutoSize = true;
			checkBox14.BackColor = System.Drawing.Color.Red;
			checkBox14.Checked = true;
			checkBox14.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox14.Location = new System.Drawing.Point(843, 322);
			checkBox14.Name = "checkBox14";
			checkBox14.Size = new System.Drawing.Size(65, 17);
			checkBox14.TabIndex = 21;
			checkBox14.Text = "band 14";
			checkBox14.UseVisualStyleBackColor = false;
			checkBox14.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox15.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox15.AutoSize = true;
			checkBox15.BackColor = System.Drawing.Color.FromArgb(255, 128, 0);
			checkBox15.Checked = true;
			checkBox15.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox15.Location = new System.Drawing.Point(843, 343);
			checkBox15.Name = "checkBox15";
			checkBox15.Size = new System.Drawing.Size(65, 17);
			checkBox15.TabIndex = 22;
			checkBox15.Text = "band 15";
			checkBox15.UseVisualStyleBackColor = false;
			checkBox15.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox16.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox16.AutoSize = true;
			checkBox16.BackColor = System.Drawing.Color.DarkOliveGreen;
			checkBox16.Checked = true;
			checkBox16.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox16.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			checkBox16.Location = new System.Drawing.Point(843, 364);
			checkBox16.Name = "checkBox16";
			checkBox16.Size = new System.Drawing.Size(65, 17);
			checkBox16.TabIndex = 23;
			checkBox16.Text = "band 16";
			checkBox16.UseVisualStyleBackColor = false;
			checkBox16.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox17.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox17.AutoSize = true;
			checkBox17.BackColor = System.Drawing.Color.Lime;
			checkBox17.Checked = true;
			checkBox17.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox17.Location = new System.Drawing.Point(843, 385);
			checkBox17.Name = "checkBox17";
			checkBox17.Size = new System.Drawing.Size(65, 17);
			checkBox17.TabIndex = 24;
			checkBox17.Text = "band 17";
			checkBox17.UseVisualStyleBackColor = false;
			checkBox17.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox18.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox18.AutoSize = true;
			checkBox18.BackColor = System.Drawing.Color.Aqua;
			checkBox18.Checked = true;
			checkBox18.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox18.Location = new System.Drawing.Point(843, 406);
			checkBox18.Name = "checkBox18";
			checkBox18.Size = new System.Drawing.Size(65, 17);
			checkBox18.TabIndex = 25;
			checkBox18.Text = "band 18";
			checkBox18.UseVisualStyleBackColor = false;
			checkBox18.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox19.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox19.AutoSize = true;
			checkBox19.BackColor = System.Drawing.Color.Blue;
			checkBox19.Checked = true;
			checkBox19.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox19.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			checkBox19.Location = new System.Drawing.Point(843, 427);
			checkBox19.Name = "checkBox19";
			checkBox19.Size = new System.Drawing.Size(65, 17);
			checkBox19.TabIndex = 26;
			checkBox19.Text = "band 19";
			checkBox19.UseVisualStyleBackColor = false;
			checkBox19.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox20.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox20.AutoSize = true;
			checkBox20.BackColor = System.Drawing.Color.Fuchsia;
			checkBox20.Checked = true;
			checkBox20.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox20.Location = new System.Drawing.Point(843, 448);
			checkBox20.Name = "checkBox20";
			checkBox20.Size = new System.Drawing.Size(65, 17);
			checkBox20.TabIndex = 27;
			checkBox20.Text = "band 20";
			checkBox20.UseVisualStyleBackColor = false;
			checkBox20.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox21.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox21.AutoSize = true;
			checkBox21.BackColor = System.Drawing.Color.FromArgb(192, 0, 0);
			checkBox21.Checked = true;
			checkBox21.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox21.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			checkBox21.Location = new System.Drawing.Point(843, 469);
			checkBox21.Name = "checkBox21";
			checkBox21.Size = new System.Drawing.Size(65, 17);
			checkBox21.TabIndex = 28;
			checkBox21.Text = "band 21";
			checkBox21.UseVisualStyleBackColor = false;
			checkBox21.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox22.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox22.AutoSize = true;
			checkBox22.BackColor = System.Drawing.Color.FromArgb(192, 192, 0);
			checkBox22.Checked = true;
			checkBox22.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox22.Location = new System.Drawing.Point(843, 490);
			checkBox22.Name = "checkBox22";
			checkBox22.Size = new System.Drawing.Size(65, 17);
			checkBox22.TabIndex = 29;
			checkBox22.Text = "band 22";
			checkBox22.UseVisualStyleBackColor = false;
			checkBox22.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox23.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox23.AutoSize = true;
			checkBox23.BackColor = System.Drawing.Color.FromArgb(0, 192, 0);
			checkBox23.Checked = true;
			checkBox23.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox23.Location = new System.Drawing.Point(843, 511);
			checkBox23.Name = "checkBox23";
			checkBox23.Size = new System.Drawing.Size(65, 17);
			checkBox23.TabIndex = 30;
			checkBox23.Text = "band 23";
			checkBox23.UseVisualStyleBackColor = false;
			checkBox23.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			checkBox24.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			checkBox24.AutoSize = true;
			checkBox24.BackColor = System.Drawing.Color.FromArgb(0, 192, 192);
			checkBox24.Checked = true;
			checkBox24.CheckState = System.Windows.Forms.CheckState.Checked;
			checkBox24.Location = new System.Drawing.Point(843, 532);
			checkBox24.Name = "checkBox24";
			checkBox24.Size = new System.Drawing.Size(65, 17);
			checkBox24.TabIndex = 31;
			checkBox24.Text = "band 24";
			checkBox24.UseVisualStyleBackColor = false;
			checkBox24.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
			label2.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(8, 8);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(34, 13);
			label2.TabIndex = 32;
			label2.Text = "Show";
			textBox_filterStartInput.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			textBox_filterStartInput.Location = new System.Drawing.Point(44, 4);
			textBox_filterStartInput.Name = "textBox_filterStartInput";
			textBox_filterStartInput.Size = new System.Drawing.Size(44, 20);
			textBox_filterStartInput.TabIndex = 33;
			textBox_filterStartInput.Text = "400";
			textBox_filterStartInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			label3.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(90, 7);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(10, 13);
			label3.TabIndex = 34;
			label3.Text = "-";
			textBox_filterEndInput.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			textBox_filterEndInput.Location = new System.Drawing.Point(104, 4);
			textBox_filterEndInput.Name = "textBox_filterEndInput";
			textBox_filterEndInput.Size = new System.Drawing.Size(44, 20);
			textBox_filterEndInput.TabIndex = 35;
			textBox_filterEndInput.Text = "1000";
			textBox_filterEndInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			label4.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(151, 8);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(21, 13);
			label4.TabIndex = 36;
			label4.Text = "nm";
			button_repaint.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			button_repaint.Location = new System.Drawing.Point(187, 3);
			button_repaint.Name = "button_repaint";
			button_repaint.Size = new System.Drawing.Size(58, 23);
			button_repaint.TabIndex = 37;
			button_repaint.Text = "repaint";
			button_repaint.UseVisualStyleBackColor = true;
			button_repaint.Click += new System.EventHandler(button_repaint_Click);
			panel1.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			panel1.Controls.Add(button_repaint);
			panel1.Controls.Add(label4);
			panel1.Controls.Add(textBox_filterEndInput);
			panel1.Controls.Add(label3);
			panel1.Controls.Add(textBox_filterStartInput);
			panel1.Controls.Add(label2);
			panel1.Location = new System.Drawing.Point(560, 3);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(259, 30);
			panel1.TabIndex = 38;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(922, 588);
			base.Controls.Add(panel1);
			base.Controls.Add(checkBox24);
			base.Controls.Add(checkBox23);
			base.Controls.Add(checkBox22);
			base.Controls.Add(checkBox21);
			base.Controls.Add(checkBox20);
			base.Controls.Add(checkBox19);
			base.Controls.Add(checkBox18);
			base.Controls.Add(checkBox17);
			base.Controls.Add(checkBox16);
			base.Controls.Add(checkBox15);
			base.Controls.Add(checkBox14);
			base.Controls.Add(checkBox13);
			base.Controls.Add(checkBox12);
			base.Controls.Add(checkBox11);
			base.Controls.Add(checkBox10);
			base.Controls.Add(checkBox9);
			base.Controls.Add(checkBox8);
			base.Controls.Add(checkBox7);
			base.Controls.Add(checkBox6);
			base.Controls.Add(checkBox5);
			base.Controls.Add(checkBox4);
			base.Controls.Add(checkBox3);
			base.Controls.Add(checkBox2);
			base.Controls.Add(checkBox1);
			base.Controls.Add(checkBox0);
			base.Controls.Add(checkBoxAll);
			base.Controls.Add(panelCurves);
			base.Controls.Add(buttonClose);
			base.Controls.Add(labelCameraModelFilter);
			DoubleBuffered = true;
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			MinimumSize = new System.Drawing.Size(938, 627);
			base.Name = "FormFilterResponse";
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			Text = "FormFilterResponse";
			base.SizeChanged += new System.EventHandler(checkBox_CheckedChanged);
			panel1.ResumeLayout(performLayout: false);
			panel1.PerformLayout();
			ResumeLayout(performLayout: false);
			PerformLayout();
		}
	}
}
