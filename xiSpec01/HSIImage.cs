using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace xiSpec01
{
	public class HSIImage
	{
		public int[,] image2SpectrumPos;

		public int[,] imageRAW;

		public double[,] imageRAWcorrected;

		public double[,,] imageHSI_spatial;

		public double[,,] imageHSI_interpol;

		public int[,] whiteImageRAW;

		public int[] whiteImageMaxValues;

		public double[,] factorWhiteImageRAW;

		public int[,] darkImageRAW;

		public double[,,] autoClusterArray;

		public double autoClusterMinR;

		public double autoClusterMaxR;

		public double autoClusterMinG;

		public double autoClusterMaxG;

		public double autoClusterMinB;

		public double autoClusterMaxB;

		public int sensor_width;

		public int sensor_height;

		public int bands;

		public int bands_active;

		public int width_spatial;

		public int height_spatial;

		public int width_interpol;

		public int height_interpol;

		public int pattern_width;

		public int pattern_height;

		public int offset_x;

		public int offset_y;

		public int exp_time_image;

		public bool measured;

		public bool measured_neu;

		public bool darkMeasured;

		public bool whiteMeasured;

		public bool spectrum_calculated;

		public int maximize_old;

		public int derivationMode_old;

		public int exp_time_white;

		public int MAX_MATERIALS;

		public int SIZE_MEASUREMENT_FIELDS;

		public int i_max_value_raw;

		public double d_max_value_raw;

		public int numMaterialsToUse;

		public double[,] spectrum;

		public double[,] spectrum_Use;

		public double[] Sd;

		public double[] SdSqr;

		public double[] VanDerMeer_Sqr;

		public double[,] SID_q;

		public double[,] JM_q;

		private FormDistance fDistance;

		private HSIParameters HSIParam;

		public HSIImage(HSIParameters param, FormDistance formDistance)
		{
			fDistance = formDistance;
			HSIParam = param;
			MAX_MATERIALS = 8;
			SIZE_MEASUREMENT_FIELDS = 12;
			i_max_value_raw = 255;
			d_max_value_raw = 255.0;
			bands = param.filter_nr_bands;
			bands_active = param.filter_nr_bands_active;
			offset_x = param.xi_offset_x;
			offset_y = param.xi_offset_y;
			pattern_width = param.filter_mosaic_pattern_width;
			pattern_height = param.filter_mosaic_pattern_height;
			width_spatial = param.image_width;
			height_spatial = param.image_height;
			width_interpol = (width_spatial - 1) * pattern_width + 1;
			height_interpol = (height_spatial - 1) * pattern_height + 1;
			sensor_width = param.sensor_width;
			sensor_height = param.sensor_height;
			image2SpectrumPos = new int[sensor_height, sensor_width];
			imageRAW = new int[sensor_height, sensor_width];
			imageRAWcorrected = new double[sensor_height, sensor_width];
			whiteImageRAW = new int[sensor_height, sensor_width];
			whiteImageMaxValues = new int[bands];
			factorWhiteImageRAW = new double[sensor_height, sensor_width];
			darkImageRAW = new int[sensor_height, sensor_width];
			imageHSI_spatial = new double[height_spatial, width_spatial, bands];
			imageHSI_interpol = new double[height_interpol, width_interpol, bands];
			int i;
			for (i = 0; i < sensor_height; i++)
			{
				for (int j = 0; j < sensor_width; j++)
				{
					image2SpectrumPos[i, j] = -1;
				}
			}
			int num = offset_y + height_spatial * pattern_height;
			i = offset_y;
			while (i < num)
			{
				int num2 = 0;
				for (int k = 0; k < pattern_height; k++)
				{
					int j = offset_x;
					for (int l = 0; l < width_spatial * pattern_width; l++)
					{
						int[,] array = image2SpectrumPos;
						int num3 = i;
						int num5 = j++;
						int num6 = param.xiImage_2_SpectrumPos[l % pattern_width + num2];
						array[num3, num5] = num6;
					}
					num2 += pattern_width;
					i++;
				}
			}
			for (i = 0; i < sensor_height; i++)
			{
				for (int j = 0; j < sensor_width; j++)
				{
					darkImageRAW[i, j] = 0;
					factorWhiteImageRAW[i, j] = 1.0;
				}
			}
			spectrum = new double[MAX_MATERIALS + 1, bands];
			spectrum_Use = new double[MAX_MATERIALS + 1, bands];
			Sd = new double[bands];
			SdSqr = new double[bands];
			VanDerMeer_Sqr = new double[MAX_MATERIALS + 1];
			SID_q = new double[MAX_MATERIALS + 1, bands];
			JM_q = new double[MAX_MATERIALS + 1, bands];
		}

		public unsafe bool darkHSIImage(Bitmap b)
		{
			BitmapData bitmapData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
			int num = Image.GetPixelFormatSize(b.PixelFormat) / 8;
			int height = bitmapData.Height;
			int width = bitmapData.Width;
			byte* ptr = (byte*)(void*)bitmapData.Scan0;
			if (num != 1)
			{
				return false;
			}
			for (int i = 0; i < sensor_height; i++)
			{
				byte* ptr2 = ptr + (long)i * (long)bitmapData.Stride;
				int num2 = 0;
				while (num2 < sensor_width)
				{
					int[,] array = darkImageRAW;
					int num3 = i;
					int num4 = num2;
					byte num5 = *ptr2;
					array[num3, num4] = num5;
					num2++;
					ptr2++;
				}
			}
			b.UnlockBits(bitmapData);
			darkMeasured = true;
			return true;
		}

		public unsafe bool whiteHSIImage(Bitmap b, int exp_time)
		{
			BitmapData bitmapData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
			int num = Image.GetPixelFormatSize(b.PixelFormat) / 8;
			int height = bitmapData.Height;
			int width = bitmapData.Width;
			byte* ptr = (byte*)(void*)bitmapData.Scan0;
			exp_time_white = exp_time;
			if (num != 1)
			{
				return false;
			}
			for (int i = 0; i < sensor_height; i++)
			{
				byte* ptr2 = ptr + (long)i * (long)bitmapData.Stride;
				int num2 = 0;
				while (num2 < sensor_width)
				{
					int[,] array = whiteImageRAW;
					int num3 = i;
					int num4 = num2;
					byte num5 = *ptr2;
					array[num3, num4] = num5;
					num2++;
					ptr2++;
				}
			}
			b.UnlockBits(bitmapData);
			whiteMeasured = true;
			calculateFPN();
			return true;
		}

		public void calculateFPN()
		{
			for (int i = 0; i < bands; i++)
			{
				whiteImageMaxValues[i] = 0;
			}
			int num = offset_x + (width_spatial / 2 - 10) * pattern_width;
			int num2 = offset_y + (height_spatial / 2 - 10) * pattern_height;
			int num3 = 20 * pattern_width;
			for (int j = num2; j < num2 + num3; j++)
			{
				for (int k = num; k < num + num3; k++)
				{
					int num4;
					if ((num4 = image2SpectrumPos[j, k]) >= 0)
					{
						whiteImageMaxValues[num4] += whiteImageRAW[j, k];
					}
				}
			}
			for (int l = 0; l < bands; l++)
			{
				whiteImageMaxValues[l] /= 400;
			}
			for (int m = 0; m < sensor_height; m++)
			{
				for (int n = 0; n < sensor_width; n++)
				{
					int num4;
					if ((num4 = image2SpectrumPos[m, n]) >= 0)
					{
						int num5 = whiteImageRAW[m, n];
						double[,] array = factorWhiteImageRAW;
						int num6 = m;
						int num7 = n;
						double num8 = (double)whiteImageMaxValues[num4] / (double)num5;
						array[num6, num7] = num8;
					}
					else
					{
						factorWhiteImageRAW[m, n] = 0.0;
					}
				}
			}
		}

		public bool measureHSIImage(Bitmap b, int numMaterials)
		{
			bool parallelProcessing = false;
			double[] array = new double[25];
			double[] array2 = new double[25];
			double[] array3 = new double[25];
			int num = SIZE_MEASUREMENT_FIELDS / 2;
			int sIZE_MEASUREMENT_FIELDS = SIZE_MEASUREMENT_FIELDS;
			numMaterialsToUse = numMaterials;
			if (numMaterialsToUse < 1 || numMaterialsToUse > MAX_MATERIALS)
			{
				return false;
			}
			createHSIImage(b, 1000, 0, 0, parallelProcessing, onlyRAWImage: true, interpolationMode: false);
			for (int i = 0; i < bands; i++)
			{
				array[i] = (array3[i] = 0.0);
				array2[i] = 10000.0;
			}
			for (int j = 0; j < 1; j++)
			{
				int num2;
				int num3;
				switch (j)
				{
				case 0:
					num2 = width_spatial / 2 - num;
					num3 = height_spatial / 2 - num;
					break;
				case 1:
					num2 = width_spatial / 2 - num;
					num3 = 0;
					break;
				case 2:
					num2 = width_spatial - sIZE_MEASUREMENT_FIELDS;
					num3 = height_spatial / 2 - num;
					break;
				case 3:
					num2 = 0;
					num3 = height_spatial / 2 - num;
					break;
				default:
					num2 = width_spatial / 2 - num;
					num3 = height_spatial - sIZE_MEASUREMENT_FIELDS;
					break;
				}
				for (int k = num3; k < num3 + sIZE_MEASUREMENT_FIELDS; k++)
				{
					for (int l = num2; l < num2 + sIZE_MEASUREMENT_FIELDS; l++)
					{
						for (int m = 0; m < bands; m++)
						{
							double num4;
							array[m] += (num4 = imageHSI_spatial[k, l, m]);
							if (num4 < array2[m])
							{
								array2[m] = num4;
							}
							if (num4 > array3[m])
							{
								array3[m] = num4;
							}
						}
					}
				}
			}
			for (int n = 0; n < bands; n++)
			{
				double[,] array4 = spectrum;
				int num5 = n;
				double num4;
				double num6 = num4 = array[n] / (double)(sIZE_MEASUREMENT_FIELDS * sIZE_MEASUREMENT_FIELDS);
				array4[0, num5] = num6;
			}
			for (int num7 = 1; num7 <= numMaterialsToUse; num7++)
			{
				int num2;
				int num3;
				switch (num7)
				{
				case 1:
					num2 = width_spatial / 4 - num;
					num3 = height_spatial / 4 - num;
					break;
				case 2:
					num2 = width_spatial * 3 / 4 - num;
					num3 = height_spatial / 4 - num;
					break;
				case 3:
					num2 = width_spatial * 3 / 4 - num;
					num3 = height_spatial * 3 / 4 - num;
					break;
				case 4:
					num2 = width_spatial / 4 - num;
					num3 = height_spatial * 3 / 4 - num;
					break;
				case 5:
					num2 = width_spatial / 2 - num;
					num3 = height_spatial / 4 - num;
					break;
				case 6:
					num2 = width_spatial * 3 / 4 - num;
					num3 = height_spatial / 2 - num;
					break;
				case 7:
					num2 = width_spatial / 2 - num;
					num3 = height_spatial * 3 / 4 - num;
					break;
				default:
					num2 = width_spatial / 4 - num;
					num3 = height_spatial / 2 - num;
					break;
				}
				for (int num8 = 0; num8 < bands; num8++)
				{
					array[num8] = (array3[num8] = 0.0);
					array2[num8] = 10000.0;
				}
				for (int num9 = num3; num9 < num3 + sIZE_MEASUREMENT_FIELDS; num9++)
				{
					for (int num10 = num2; num10 < num2 + sIZE_MEASUREMENT_FIELDS; num10++)
					{
						for (int num11 = 0; num11 < bands; num11++)
						{
							double num4;
							array[num11] += (num4 = imageHSI_spatial[num9, num10, num11]);
							if (num4 < array2[num11])
							{
								array2[num11] = num4;
							}
							if (num4 > array3[num11])
							{
								array3[num11] = num4;
							}
						}
					}
				}
				for (int num12 = 0; num12 < bands; num12++)
				{
					double[,] array5 = spectrum;
					int num13 = num7;
					int num14 = num12;
					double num4;
					double num15 = num4 = array[num12] / (double)(sIZE_MEASUREMENT_FIELDS * sIZE_MEASUREMENT_FIELDS);
					array5[num13, num14] = num15;
				}
			}
			measured = true;
			measured_neu = true;
			return true;
		}

		private unsafe void createHSIImage_InnerLoop_new_1(int y, byte* ptrFirstPixel, int stride, int calculateReflectance)
		{
			double num = d_max_value_raw * (double)exp_time_white / (double)exp_time_image;
			fixed (int* ptr2 = &imageRAW[y, 0])
			{
				fixed (int* ptr3 = &darkImageRAW[y, 0])
				{
					fixed (int* ptr4 = &whiteImageRAW[y, 0])
					{
						fixed (double* ptr5 = &imageRAWcorrected[y, 0])
						{
							fixed (double* ptr6 = &factorWhiteImageRAW[y, 0])
							{
								byte* ptr = ptrFirstPixel + (long)y * (long)stride;
								int num2 = 0;
								while (num2 < sensor_width)
								{
									int num3 = ptr2[num2] = *ptr;
									if (calculateReflectance != 0)
									{
										int num4;
										if ((num4 = num3 - ptr3[num2]) < 0)
										{
											num4 = 0;
										}
										int num5;
										if ((num5 = ptr4[num2] - ptr3[num2]) < 1)
										{
											num5 = 1;
										}
										ptr5[num2] = (double)num4 / (double)num5 * num;
										if (ptr5[num2] > d_max_value_raw)
										{
											ptr5[num2] = d_max_value_raw;
										}
									}
									else if (darkMeasured || whiteMeasured)
									{
										int num4;
										if ((num4 = num3 - ptr3[num2]) < 0)
										{
											ptr5[num2] = 0.0;
										}
										else
										{
											double num6 = (double)num4 * ptr6[num2];
											ptr5[num2] = ((num6 > d_max_value_raw) ? d_max_value_raw : num6);
										}
									}
									else
									{
										ptr5[num2] = (double)num3;
									}
									num2++;
									ptr++;
								}
							}
						}
					}
				}
			}
		}

		private void createHSIImage_InnerLoop_new_2(int y_dest, bool interpolationMode)
		{
			int num;
			int num2;
			int num3;
			if (interpolationMode)
			{
				num = y_dest + offset_y;
				num2 = 1;
				num3 = width_interpol;
			}
			else
			{
				num = y_dest * pattern_height + offset_y;
				num2 = pattern_width;
				num3 = width_spatial;
			}
			int num4 = 0;
			while (num4 < pattern_height)
			{
				int num5 = offset_x;
				int num6 = 0;
				while (num6 < num3)
				{
					int num7 = num5;
					if (interpolationMode)
					{
						int num8 = 0;
						while (num8 < pattern_width)
						{
							double[,,] array = imageHSI_interpol;
							int num9 = num6;
							int num10 = image2SpectrumPos[num, num7];
							double num11 = imageRAWcorrected[num, num7];
							array[y_dest, num9, num10] = num11;
							num8++;
							num7++;
						}
					}
					else
					{
						int num12 = 0;
						while (num12 < pattern_width)
						{
							double[,,] array2 = imageHSI_spatial;
							int num13 = num6;
							int num14 = image2SpectrumPos[num, num7];
							double num15 = imageRAWcorrected[num, num7];
							array2[y_dest, num13, num14] = num15;
							num12++;
							num7++;
						}
					}
					num6++;
					num5 += num2;
				}
				num4++;
				num++;
			}
		}

		public unsafe bool createHSIImage(Bitmap b, int exp_time, int maximize, int derivationMode, bool parallelProcessing, bool onlyRAWImage, bool interpolationMode)
		{
			BitmapData bitmapData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
			int num = Image.GetPixelFormatSize(b.PixelFormat) / 8;
			int height = bitmapData.Height;
			int width = bitmapData.Width;
			byte* ptrFirstPixel = (byte*)(void*)bitmapData.Scan0;
			int stride = bitmapData.Stride;
			if (num != 1)
			{
				return false;
			}
			exp_time_image = exp_time;
			if (!onlyRAWImage && measured && (measured_neu || !spectrum_calculated || maximize != maximize_old || derivationMode != derivationMode_old))
			{
				spectrum_calculated = true;
				maximize_old = maximize;
				derivationMode_old = derivationMode;
				measured_neu = false;
				for (int i = 0; i < bands_active; i++)
				{
					for (int j = 0; j <= numMaterialsToUse; j++)
					{
						double[,] array = spectrum_Use;
						int num2 = j;
						int num3 = i;
						double num4 = spectrum[j, i];
						array[num2, num3] = num4;
					}
				}
				double[] array2 = new double[MAX_MATERIALS + 1];
				double[] array3 = new double[MAX_MATERIALS + 1];
				for (int k = 0; k <= MAX_MATERIALS; k++)
				{
					array2[k] = 10000.0;
					array3[k] = -10000.0;
				}
				if (maximize == 1)
				{
					for (int l = 0; l < bands_active; l++)
					{
						for (int m = 0; m <= numMaterialsToUse; m++)
						{
							double num5 = spectrum_Use[m, l];
							if (num5 > array3[m])
							{
								array3[m] = num5;
							}
							if (num5 < array2[m])
							{
								array2[m] = num5;
							}
						}
					}
					for (int n = 0; n <= numMaterialsToUse; n++)
					{
						double num6 = (array2[n] < array3[n]) ? (255.0 / (array3[n] - array2[n])) : 1.0;
						for (int num7 = 0; num7 < bands_active; num7++)
						{
							double[,] array4 = spectrum_Use;
							int num8 = n;
							int num9 = num7;
							double num10 = (spectrum_Use[n, num7] - array2[n]) * num6;
							array4[num8, num9] = num10;
						}
					}
				}
				for (int num11 = 0; num11 <= numMaterialsToUse; num11++)
				{
					Sd[num11] = (SdSqr[num11] = 0.0);
				}
				for (int num12 = 0; num12 < bands_active; num12++)
				{
					for (int num13 = 0; num13 <= numMaterialsToUse; num13++)
					{
						double num14;
						Sd[num13] += (num14 = spectrum_Use[num13, num12]);
						SdSqr[num13] += num14 * num14;
					}
				}
				for (int num15 = 0; num15 <= numMaterialsToUse; num15++)
				{
					VanDerMeer_Sqr[num15] = Math.Sqrt(SdSqr[num15] - Sd[num15] * Sd[num15] / (double)bands_active);
				}
				for (int num16 = 0; num16 < bands_active; num16++)
				{
					for (int num17 = 0; num17 <= numMaterialsToUse; num17++)
					{
						double[,] sID_q = SID_q;
						int num18 = num17;
						int num19 = num16;
						double num20 = (spectrum_Use[num17, num16] + 0.3) / Sd[num17];
						sID_q[num18, num19] = num20;
						double[,] jM_q = JM_q;
						int num21 = num17;
						int num22 = num16;
						double num23 = Math.Sqrt(spectrum_Use[num17, num16] / Sd[num17]);
						jM_q[num21, num22] = num23;
					}
				}
			}
			int num24 = interpolationMode ? height_interpol : height_spatial;
			int calculateReflectance = (derivationMode > 0) ? 1 : 0;
			if (parallelProcessing)
			{
				Parallel.For(0, sensor_height, delegate(int y)
				{
					createHSIImage_InnerLoop_new_1(y, ptrFirstPixel, stride, calculateReflectance);
				});
				Parallel.For(0, num24, delegate(int y)
				{
					createHSIImage_InnerLoop_new_2(y, interpolationMode);
				});
				if (!onlyRAWImage && maximize > 0)
				{
					if (interpolationMode)
					{
						Parallel.For(0, height_interpol, delegate(int y)
						{
							for (int num61 = 0; num61 < width_interpol; num61++)
							{
								double num62 = 10000.0;
								double num63 = -10000.0;
								for (int num64 = 0; num64 < bands_active; num64++)
								{
									double num65 = imageHSI_interpol[y, num61, num64];
									if (num65 > num63)
									{
										num63 = num65;
									}
									if (num65 < num62)
									{
										num62 = num65;
									}
								}
								double num66 = (num62 < num63) ? (d_max_value_raw / (num63 - num62)) : 1.0;
								for (int num67 = 0; num67 < bands_active; num67++)
								{
									double[,,] array8 = imageHSI_interpol;
									int num68 = num61;
									int num69 = num67;
									double num70 = (imageHSI_interpol[y, num61, num67] - num62) * num66;
									array8[y, num68, num69] = num70;
								}
							}
						});
					}
					else
					{
						Parallel.For(0, height_spatial, delegate(int y)
						{
							for (int num51 = 0; num51 < width_spatial; num51++)
							{
								double num52 = 10000.0;
								double num53 = -10000.0;
								for (int num54 = 0; num54 < bands_active; num54++)
								{
									double num55 = imageHSI_spatial[y, num51, num54];
									if (num55 > num53)
									{
										num53 = num55;
									}
									if (num55 < num52)
									{
										num52 = num55;
									}
								}
								double num56 = (num52 < num53) ? (d_max_value_raw / (num53 - num52)) : 1.0;
								for (int num57 = 0; num57 < bands_active; num57++)
								{
									double[,,] array7 = imageHSI_spatial;
									int num58 = num51;
									int num59 = num57;
									double num60 = (imageHSI_spatial[y, num51, num57] - num52) * num56;
									array7[y, num58, num59] = num60;
								}
							}
						});
					}
				}
			}
			else
			{
				for (int num25 = 0; num25 < sensor_height; num25++)
				{
					createHSIImage_InnerLoop_new_1(num25, ptrFirstPixel, stride, calculateReflectance);
				}
				for (int num26 = 0; num26 < num24; num26++)
				{
					createHSIImage_InnerLoop_new_2(num26, interpolationMode);
				}
				if (!onlyRAWImage && maximize > 0)
				{
					if (interpolationMode)
					{
						for (int num27 = 0; num27 < height_interpol; num27++)
						{
							for (int num28 = 0; num28 < width_interpol; num28++)
							{
								double num29 = 10000.0;
								double num30 = -10000.0;
								for (int num31 = 0; num31 < bands_active; num31++)
								{
									double num32 = imageHSI_interpol[num27, num28, num31];
									if (num32 > num30)
									{
										num30 = num32;
									}
									if (num32 < num29)
									{
										num29 = num32;
									}
								}
								double num33 = (num29 < num30) ? (d_max_value_raw / (num30 - num29)) : 1.0;
								for (int num34 = 0; num34 < bands_active; num34++)
								{
									double[,,] array5 = imageHSI_interpol;
									int num35 = num27;
									int num36 = num28;
									int num37 = num34;
									double num38 = (imageHSI_interpol[num27, num28, num34] - num29) * num33;
									array5[num35, num36, num37] = num38;
								}
							}
						}
					}
					else
					{
						for (int num39 = 0; num39 < height_spatial; num39++)
						{
							for (int num40 = 0; num40 < width_spatial; num40++)
							{
								double num41 = 10000.0;
								double num42 = -10000.0;
								for (int num43 = 0; num43 < bands_active; num43++)
								{
									double num44 = imageHSI_spatial[num39, num40, num43];
									if (num44 > num42)
									{
										num42 = num44;
									}
									if (num44 < num41)
									{
										num41 = num44;
									}
								}
								double num45 = (num41 < num42) ? (d_max_value_raw / (num42 - num41)) : 1.0;
								for (int num46 = 0; num46 < bands_active; num46++)
								{
									double[,,] array6 = imageHSI_spatial;
									int num47 = num39;
									int num48 = num40;
									int num49 = num46;
									double num50 = (imageHSI_spatial[num39, num40, num46] - num41) * num45;
									array6[num47, num48, num49] = num50;
								}
							}
						}
					}
				}
			}
			b.UnlockBits(bitmapData);
			return true;
		}

		public void BSQwriteHSIImage(BinaryWriter w, bool useInterpolatedImage)
		{
			if (useInterpolatedImage)
			{
				for (int i = 0; i < bands_active; i++)
				{
					for (int j = 0; j < height_interpol; j++)
					{
						for (int k = 0; k < width_interpol; k++)
						{
							ushort value = (ushort)(imageHSI_interpol[j, k, i] * 128.0);
							w.Write(value);
						}
					}
				}
			}
			else
			{
				for (int l = 0; l < bands_active; l++)
				{
					for (int m = 0; m < height_spatial; m++)
					{
						for (int n = 0; n < width_spatial; n++)
						{
							ushort value = (ushort)(imageHSI_spatial[m, n, l] * 128.0);
							w.Write(value);
						}
					}
				}
			}
		}

		private unsafe void HSIImageToBitmap_InnerCompareLoop(int y, byte* ptrFirstPixel, int bytesPerPixel, int stride, int distanceMethod, int compare, Point mousePos, bool showDistanceValues)
		{
			byte* ptr = ptrFirstPixel + (long)y * (long)stride;
			double[] array = new double[MAX_MATERIALS + 1];
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			double num5 = 1.0;
			double num6 = 1.0;
			int num7 = 0;
			for (int i = 0; i <= MAX_MATERIALS; i++)
			{
				array[i] = 100000.0;
			}
			int num8 = 0;
			while (num8 < width_spatial)
			{
				for (int i = 0; i <= numMaterialsToUse; i++)
				{
					array[i] = 0.0;
				}
				switch (distanceMethod)
				{
				case 0:
					for (int num29 = 0; num29 < bands_active; num29++)
					{
						for (int i = 0; i <= numMaterialsToUse; i++)
						{
							double num30 = imageHSI_spatial[y, num8, num29] - spectrum_Use[i, num29];
							array[i] += num30 * num30;
						}
					}
					for (int i = 0; i <= numMaterialsToUse; i++)
					{
						array[i] = Math.Sqrt(array[i] / (double)bands_active);
					}
					break;
				case 1:
				{
					double num21 = 0.0;
					double num22 = 0.0;
					double[] array6 = new double[MAX_MATERIALS + 1];
					for (int i = 0; i <= numMaterialsToUse; i++)
					{
						array6[i] = 0.0;
					}
					for (int num23 = 0; num23 < bands_active; num23++)
					{
						for (int i = 0; i <= numMaterialsToUse; i++)
						{
							array6[i] += imageHSI_spatial[y, num8, num23] * spectrum_Use[i, num23];
						}
						num21 += imageHSI_spatial[y, num8, num23];
						num22 += imageHSI_spatial[y, num8, num23] * imageHSI_spatial[y, num8, num23];
					}
					double num24 = Math.Sqrt(num22 - num21 * num21 / (double)bands_active);
					for (int i = 0; i <= numMaterialsToUse; i++)
					{
						double num25 = num24 * VanDerMeer_Sqr[i];
						if (num25 == 0.0)
						{
							num25 = 1.0;
						}
						array[i] = 100.0 - 100.0 * (array6[i] - num21 * Sd[i] / (double)bands_active) / num25;
					}
					break;
				}
				case 2:
				{
					double num26 = 0.0;
					double[] array7 = new double[MAX_MATERIALS + 1];
					for (int i = 0; i <= numMaterialsToUse; i++)
					{
						array7[i] = 0.0;
					}
					for (int num27 = 0; num27 < bands_active; num27++)
					{
						num26 += imageHSI_spatial[y, num8, num27] * imageHSI_spatial[y, num8, num27];
					}
					for (int num28 = 0; num28 < bands_active; num28++)
					{
						for (int i = 0; i <= numMaterialsToUse; i++)
						{
							array7[i] += imageHSI_spatial[y, num8, num28] * spectrum_Use[i, num28];
						}
					}
					for (int i = 0; i <= numMaterialsToUse; i++)
					{
						array7[i] /= Math.Sqrt(num26 * SdSqr[i]);
						array[i] = 1000.0 - 1000.0 * (1.0 - 2.0 * Math.Acos(array7[i]) / 3.1415926535897931);
					}
					break;
				}
				case 3:
				{
					double num31 = 0.0;
					double[] array8 = new double[MAX_MATERIALS + 1];
					for (int i = 0; i <= numMaterialsToUse; i++)
					{
						array8[i] = 0.0;
					}
					for (int num32 = 0; num32 < bands_active; num32++)
					{
						num31 += imageHSI_spatial[y, num8, num32];
					}
					for (int num33 = 0; num33 < bands_active; num33++)
					{
						double num34 = (imageHSI_spatial[y, num8, num33] + 0.3) / num31;
						for (int i = 0; i <= numMaterialsToUse; i++)
						{
							double num35 = SID_q[i, num33];
							double num36 = Math.Log10(num34 / num35);
							array8[i] += num34 * num36 - num35 * num36;
						}
					}
					for (int i = 0; i <= numMaterialsToUse; i++)
					{
						array[i] = 10000.0 * array8[i];
					}
					break;
				}
				case 4:
				{
					double num13 = 0.0;
					double[] array4 = new double[MAX_MATERIALS + 1];
					double[] array5 = new double[MAX_MATERIALS + 1];
					for (int i = 0; i <= numMaterialsToUse; i++)
					{
						array5[i] = 0.0;
					}
					for (int n = 0; n < bands_active; n++)
					{
						num13 += imageHSI_spatial[y, num8, n] * imageHSI_spatial[y, num8, n];
					}
					for (int num14 = 0; num14 < bands_active; num14++)
					{
						for (int i = 0; i <= numMaterialsToUse; i++)
						{
							array5[i] += imageHSI_spatial[y, num8, num14] * spectrum_Use[i, num14];
						}
					}
					for (int i = 0; i <= numMaterialsToUse; i++)
					{
						array5[i] /= Math.Sqrt(num13 * SdSqr[i]);
						array4[i] = Math.Acos(array5[i]);
					}
					double num15 = 0.0;
					for (int i = 0; i <= numMaterialsToUse; i++)
					{
						array5[i] = 0.0;
					}
					for (int num16 = 0; num16 < bands_active; num16++)
					{
						num15 += imageHSI_spatial[y, num8, num16];
					}
					for (int num17 = 0; num17 < bands_active; num17++)
					{
						double num18 = (imageHSI_spatial[y, num8, num17] + 0.3) / num15;
						for (int i = 0; i <= numMaterialsToUse; i++)
						{
							double num19 = SID_q[i, num17];
							double num20 = Math.Log10(num18 / num19);
							array5[i] += num18 * num20 - num19 * num20;
						}
					}
					for (int i = 0; i <= numMaterialsToUse; i++)
					{
						array[i] = 100000.0 * array5[i] * Math.Sin(array4[i]);
					}
					break;
				}
				case 5:
				{
					double num9 = 0.0;
					double[] array2 = new double[MAX_MATERIALS + 1];
					double[] array3 = new double[MAX_MATERIALS + 1];
					for (int i = 0; i <= numMaterialsToUse; i++)
					{
						array3[i] = 0.0;
					}
					for (int j = 0; j < bands_active; j++)
					{
						num9 += imageHSI_spatial[y, num8, j] * imageHSI_spatial[y, num8, j];
					}
					for (int k = 0; k < bands_active; k++)
					{
						for (int i = 0; i <= numMaterialsToUse; i++)
						{
							array3[i] += imageHSI_spatial[y, num8, k] * spectrum_Use[i, k];
						}
					}
					for (int i = 0; i <= numMaterialsToUse; i++)
					{
						array3[i] /= Math.Sqrt(num9 * SdSqr[i]);
						array2[i] = Math.Acos(array3[i]);
					}
					double num10 = 0.0;
					for (int i = 0; i <= numMaterialsToUse; i++)
					{
						array3[i] = 0.0;
					}
					for (int l = 0; l < bands_active; l++)
					{
						num10 += imageHSI_spatial[y, num8, l];
					}
					for (int m = 0; m < bands_active; m++)
					{
						double num11 = Math.Sqrt(imageHSI_spatial[y, num8, m] / num10);
						for (int i = 0; i <= numMaterialsToUse; i++)
						{
							double num12 = num11 - JM_q[i, m];
							array3[i] += num12 * num12;
						}
					}
					for (int i = 0; i <= numMaterialsToUse; i++)
					{
						array[i] = 10000.0 * Math.Sqrt(array3[i]) * Math.Tan(array2[i]);
					}
					break;
				}
				}
				if (showDistanceValues && mousePos.X == num8 && mousePos.Y == y)
				{
					fDistance.showValues((int)array[0], (int)array[1], (int)array[2], (int)array[3]);
				}
				num = (num2 = array[0]);
				for (int i = 1; i <= numMaterialsToUse; i++)
				{
					if (array[i] < num)
					{
						num = array[i];
					}
					if (array[i] > num2)
					{
						num2 = array[i];
					}
				}
				num5 = 255.0 / (num2 - num);
				num3 = (num4 = array[1]);
				for (int i = 2; i <= numMaterialsToUse; i++)
				{
					if (array[i] < num3)
					{
						num3 = array[i];
					}
					if (array[i] > num4)
					{
						num4 = array[i];
					}
				}
				num6 = 255.0 / (num4 - num3);
				if (compare == 1)
				{
					double num37 = 1000000.0;
					int num38 = -1;
					for (int num39 = 0; num39 <= numMaterialsToUse; num39++)
					{
						if (array[num39] < num37)
						{
							num37 = array[num39];
							num38 = num39;
						}
					}
					switch (num38)
					{
					case 0:
						ptr[num7] = 0;
						ptr[num7 + 1] = 0;
						ptr[num7 + 2] = 0;
						break;
					case 1:
						ptr[num7] = 0;
						ptr[num7 + 1] = 0;
						ptr[num7 + 2] = byte.MaxValue;
						break;
					case 2:
						ptr[num7] = byte.MaxValue;
						ptr[num7 + 1] = 0;
						ptr[num7 + 2] = 0;
						break;
					case 3:
						ptr[num7] = 0;
						ptr[num7 + 1] = byte.MaxValue;
						ptr[num7 + 2] = 0;
						break;
					case 4:
						ptr[num7] = byte.MaxValue;
						ptr[num7 + 1] = byte.MaxValue;
						ptr[num7 + 2] = byte.MaxValue;
						break;
					case 5:
						ptr[num7] = byte.MaxValue;
						ptr[num7 + 1] = 0;
						ptr[num7 + 2] = byte.MaxValue;
						break;
					case 6:
						ptr[num7] = byte.MaxValue;
						ptr[num7 + 1] = byte.MaxValue;
						ptr[num7 + 2] = 0;
						break;
					case 7:
						ptr[num7] = 0;
						ptr[num7 + 1] = 128;
						ptr[num7 + 2] = 128;
						break;
					case 8:
						ptr[num7] = 0;
						ptr[num7 + 1] = byte.MaxValue;
						ptr[num7 + 2] = byte.MaxValue;
						break;
					default:
					{
						double num30 = (double)(int)((511.0 - (array[2] + array[4] - 2.0 * num3) * num6) * (array[0] - num) * num5 / 255.0);
						ptr[num7] = (byte)num30;
						num30 = (double)(int)((511.0 - (array[3] + array[4] - 2.0 * num3) * num6) * (array[0] - num) * num5 / 255.0);
						ptr[num7 + 1] = (byte)num30;
						num30 = (double)(int)((511.0 - (array[1] + array[4] - 2.0 * num3) * num6) * (array[0] - num) * num5 / 255.0);
						ptr[num7 + 2] = (byte)num30;
						break;
					}
					}
				}
				else
				{
					double num30 = (511.0 - (array[2] + array[4] - 2.0 * num3) * num6) * (array[0] - num) * num5 / 255.0;
					ptr[num7] = (byte)num30;
					num30 = (511.0 - (array[3] + array[4] - 2.0 * num3) * num6) * (array[0] - num) * num5 / 255.0;
					ptr[num7 + 1] = (byte)num30;
					num30 = (511.0 - (array[1] + array[4] - 2.0 * num3) * num6) * (array[0] - num) * num5 / 255.0;
					ptr[num7 + 2] = (byte)num30;
				}
				num8++;
				num7 += bytesPerPixel;
			}
		}

		public unsafe bool HSIImageToBitmap(Bitmap b, bool useInterpolatedImage, int compare, int bandNr_only, int imageViewMode, int distanceMethod, Point mousePos, int NumMeasurementFields, bool parallelProcessing)
		{
			int num = useInterpolatedImage ? width_interpol : width_spatial;
			int num2 = useInterpolatedImage ? height_interpol : height_spatial;
			if (b.Width != num || b.Height != num2)
			{
				return false;
			}
			BitmapData bitmapData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
			int bytesPerPixel = Image.GetPixelFormatSize(b.PixelFormat) / 8;
			int height = bitmapData.Height;
			int width = bitmapData.Width;
			int num14 = bytesPerPixel;
			byte* ptrFirstPixel = (byte*)(void*)bitmapData.Scan0;
			int stride = bitmapData.Stride;
			bool flag = compare != 0 && measured;
			bool showDistanceValues = false;
			if (bytesPerPixel < 3)
			{
				return false;
			}
			if (fDistance != null)
			{
				if (mousePos.X >= 0)
				{
					showDistanceValues = true;
				}
				else
				{
					fDistance.showValues(-1, -1, -1, -1);
				}
			}
			if (!flag)
			{
				if (showDistanceValues)
				{
					fDistance.showValues(-1, -1, -1, -1);
				}
				for (int i = 0; i < num2; i++)
				{
					byte* ptr = ptrFirstPixel + (long)i * (long)stride;
					int num3 = 0;
					for (int j = 0; j < num; j++)
					{
						if (bandNr_only == -1)
						{
							int num4 = 0;
							int num5 = 70000;
							int num6 = 0;
							for (int k = 0; k < bands_active; k++)
							{
								int num7 = (int)(useInterpolatedImage ? imageHSI_interpol[i, j, k] : imageHSI_spatial[i, j, k]);
								num4 += num7;
								if (num7 < num5)
								{
									num5 = num7;
								}
								if (num7 > num6)
								{
									num6 = num7;
								}
							}
							switch (imageViewMode)
							{
							case 0:
								ptr[num3] = (byte)(num4 / bands_active);
								ptr[num3 + 1] = (byte)(num4 / bands_active);
								ptr[num3 + 2] = (byte)(num4 / bands_active);
								break;
							case 1:
								ptr[num3] = (byte)num6;
								ptr[num3 + 1] = (byte)num6;
								ptr[num3 + 2] = (byte)num6;
								break;
							default:
								ptr[num3] = (byte)num5;
								ptr[num3 + 1] = (byte)num5;
								ptr[num3 + 2] = (byte)num5;
								break;
							}
						}
						else if (bandNr_only >= bands_active)
						{
							ptr[num3] = 0;
							ptr[num3 + 1] = 0;
							ptr[num3 + 2] = 0;
						}
						else
						{
							int num4 = (int)(useInterpolatedImage ? imageHSI_interpol[i, j, bandNr_only] : imageHSI_spatial[i, j, bandNr_only]);
							ptr[num3] = (byte)num4;
							ptr[num3 + 1] = (byte)num4;
							ptr[num3 + 2] = (byte)num4;
						}
						num3 += bytesPerPixel;
					}
				}
				if (NumMeasurementFields > 0)
				{
					int num8 = 0;
					int num9 = SIZE_MEASUREMENT_FIELDS / 2;
					int sIZE_MEASUREMENT_FIELDS = SIZE_MEASUREMENT_FIELDS;
					for (int l = 0; l < 1 + NumMeasurementFields; l++)
					{
						int num10;
						int num11;
						switch (l)
						{
						case 0:
							num10 = width_spatial / 2 - num9;
							num11 = height_spatial / 2 - num9;
							break;
						case 1:
							num10 = width_spatial / 4 - num9;
							num11 = height_spatial / 4 - num9;
							break;
						case 2:
							num10 = width_spatial * 3 / 4 - num9;
							num11 = height_spatial / 4 - num9;
							break;
						case 3:
							num10 = width_spatial * 3 / 4 - num9;
							num11 = height_spatial * 3 / 4 - num9;
							break;
						case 4:
							num10 = width_spatial / 4 - num9;
							num11 = height_spatial * 3 / 4 - num9;
							break;
						case 5:
							num10 = width_spatial / 2 - num9;
							num11 = height_spatial / 4 - num9;
							break;
						case 6:
							num10 = width_spatial * 3 / 4 - num9;
							num11 = height_spatial / 2 - num9;
							break;
						case 7:
							num10 = width_spatial / 2 - num9;
							num11 = height_spatial * 3 / 4 - num9;
							break;
						default:
							num10 = width_spatial / 4 - num9;
							num11 = height_spatial / 2 - num9;
							break;
						}
						byte* ptr2 = ptrFirstPixel + (long)num11 * (long)stride;
						byte* ptr3 = ptrFirstPixel + (long)(num11 + sIZE_MEASUREMENT_FIELDS - 1) * (long)stride;
						int num12;
						num8 = (num12 = num10) * bytesPerPixel;
						while (num12 < num10 + sIZE_MEASUREMENT_FIELDS)
						{
							ptr3[num8] = (ptr2[num8] = 0);
							ptr3[num8 + 1] = (ptr2[num8 + 1] = 0);
							byte* intPtr = ptr3 + (num8 + 2);
							byte b2;
							ptr2[num8 + 2] = (b2 = byte.MaxValue);
							*intPtr = b2;
							num12++;
							num8 += bytesPerPixel;
						}
						for (int m = num11 + 1; m < num11 + sIZE_MEASUREMENT_FIELDS - 1; m++)
						{
							ptr2 = ptrFirstPixel + (long)m * (long)stride;
							num12 = num10 * bytesPerPixel;
							int num13 = (num10 + sIZE_MEASUREMENT_FIELDS - 1) * bytesPerPixel;
							ptr2[num13] = (ptr2[num12] = 0);
							ptr2[num13 + 1] = (ptr2[num12 + 1] = 0);
							byte* intPtr2 = ptr2 + (num13 + 2);
							byte b3;
							ptr2[num12 + 2] = (b3 = byte.MaxValue);
							*intPtr2 = b3;
						}
					}
				}
			}
			else if (parallelProcessing)
			{
				Parallel.For(0, num2, delegate(int y)
				{
					HSIImageToBitmap_InnerCompareLoop(y, ptrFirstPixel, bytesPerPixel, stride, distanceMethod, compare, mousePos, showDistanceValues);
				});
			}
			else
			{
				for (int n = 0; n < num2; n++)
				{
					HSIImageToBitmap_InnerCompareLoop(n, ptrFirstPixel, bytesPerPixel, stride, distanceMethod, compare, mousePos, showDistanceValues);
				}
			}
			b.UnlockBits(bitmapData);
			return true;
		}
	}
}
