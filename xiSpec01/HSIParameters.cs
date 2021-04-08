using System.Windows.Forms;
using System.Xml;

namespace xiSpec01
{
	public class HSIParameters
	{
		public bool linescan;

		public bool filter_responses_read;

		public int xi_cameraModel;

		public int xi_offset_x;

		public int xi_offset_y;

		public int xi_calibration_version = -1;

		public int calibration_type = -1;

		public int sensor_width;

		public int sensor_height;

		public int filter_nr_bands = -1;

		public int filter_nr_bands_active = -1;

		public int filter_active_area_offset_x;

		public int filter_active_area_offset_y;

		public int filter_active_area_width;

		public int filter_active_area_height;

		public int filter_mosaic_pattern_width;

		public int filter_mosaic_pattern_height;

		public int filter_start_spectral_range;

		public int filter_end_spectral_range;

		public int start_calibration_range;

		public int end_calibration_range;

		public int calibration_resolution;

		public int image_width;

		public int image_height;

		public int filterRangeStart;

		public int filterRangeEnd;

		public double[] array_wavelengths;

		public int[] spectrumPos_2_BandNr;

		public int[] bandNr_2_SpectrumPos;

		public int[] xiImage_2_SpectrumPos;

		public double[,] filter_responses;

		public int num_filter_responses = 601;

		public double filter_max_value;

		public RichTextBox ausgabe;

		private bool HSISectionOnOff(string zeile, string tag, ref bool value)
		{
			if (zeile.StartsWith(tag))
			{
				value = ((!zeile.StartsWith("</")) ? true : false);
				return true;
			}
			return false;
		}

		private bool HSIParameterIn(string zeile, string tag, ref int value)
		{
			if (zeile.StartsWith(tag))
			{
				string[] array = zeile.Substring(tag.Length).Split('"', '<');
				try
				{
					value = XmlConvert.ToInt32(array[0]);
				}
				catch
				{
				}
				return true;
			}
			return false;
		}

		private bool HSIParameterDoubleIn(string zeile, string tag, ref double value)
		{
			if (zeile.StartsWith(tag))
			{
				string[] array = zeile.Substring(tag.Length).Split('"', '<');
				string s = array[0].Trim();
				try
				{
					value = XmlConvert.ToDouble(s);
				}
				catch
				{
				}
				return true;
			}
			return false;
		}

		public HSIParameters(int cameraModel, string buffer, int filterStart, int filterEnd, RichTextBox pAusgabe)
		{
			string[] array = buffer.Split('\n', '\r');
			bool value = false;
			bool value2 = false;
			bool value3 = false;
			bool value4 = false;
			bool flag = false;
			bool value5 = true;
			int index_peak = 0;
			int value6 = 0;
			int num = 0;
			double peak = 0.0;
			bool flag2 = false;
			int num2 = 0;
			int value7 = -1;
			int[] array2 = new int[200];
			ausgabe = pAusgabe;
			array_wavelengths = new double[200];
			spectrumPos_2_BandNr = new int[200];
			bandNr_2_SpectrumPos = new int[200];
			xiImage_2_SpectrumPos = new int[25];
			xi_cameraModel = cameraModel;
			if (filterStart == 0)
			{
				if (xi_cameraModel == 136)
				{
					filterRangeStart = 450;
				}
				else if (xi_cameraModel == 144)
				{
					filterRangeStart = 675;
				}
			}
			else
			{
				filterRangeStart = filterStart;
			}
			if (filterEnd == 0)
			{
				if (xi_cameraModel == 136)
				{
					filterRangeEnd = 650;
				}
				else if (xi_cameraModel == 144)
				{
					filterRangeEnd = 975;
				}
			}
			else
			{
				filterRangeEnd = filterEnd;
			}
			for (int i = 0; i < 200; i++)
			{
				array_wavelengths[i] = 0.0;
			}
			string[] array3 = array;
			foreach (string text in array3)
			{
				string text2 = text.Trim();
				if (!(text2 == ""))
				{
					if (xi_calibration_version == 0 && flag2)
					{
						string value8 = "</calibration_data>";
						if (text2.StartsWith(value8) || !get_filter_responses(text2, num2++))
						{
							flag2 = false;
							filter_responses_read = false;
						}
						else if (num2 >= num_filter_responses)
						{
							flag2 = false;
						}
					}
					else if (HSIParameterIn(text2, "<calibration type=\"", ref calibration_type))
					{
						if (calibration_type != 3)
						{
							calibration_type = -1;
							filter_nr_bands = -1;
							return;
						}
						xi_calibration_version = 0;
					}
					else if (HSIParameterIn(text2, "<sensor_calibration version=\"", ref calibration_type))
					{
						if (calibration_type == 1)
						{
							xi_calibration_version = 3;
						}
						else
						{
							if (calibration_type != 0)
							{
								calibration_type = -1;
								filter_nr_bands = -1;
								return;
							}
							xi_calibration_version = 1;
						}
					}
					else
					{
						if (xi_calibration_version == 0)
						{
							if (HSISectionOnOff(text2, "<sensor>", ref value) || HSISectionOnOff(text2, "</sensor>", ref value) || HSISectionOnOff(text2, "<filter>", ref value2) || HSISectionOnOff(text2, "</filter>", ref value2) || HSISectionOnOff(text2, "<wavelength>", ref value3) || HSISectionOnOff(text2, "</wavelength>", ref value3) || HSIParameterIn(text2, "<nr_bands>", ref filter_nr_bands))
							{
								continue;
							}
						}
						else if (xi_calibration_version >= 1)
						{
							string value8 = "<filter_info version=\"2\" layout=\"MOSAIC\">";
							if (text2.StartsWith(value8))
							{
								xi_calibration_version = 2;
							}
							if (HSISectionOnOff(text2, "<sensor_info", ref value) || HSISectionOnOff(text2, "</sensor_info>", ref value) || HSISectionOnOff(text2, "<filter_area", ref value2) || HSISectionOnOff(text2, "</filter_area>", ref value2) || ((xi_calibration_version == 1 || xi_calibration_version == 2) && (HSISectionOnOff(text2, "<response_composition", ref value4) || HSISectionOnOff(text2, "</response_composition>", ref value4))) || (xi_calibration_version == 2 && (HSISectionOnOff(text2, "<peak version=\"0\" order=\"1\">", ref value5) || HSISectionOnOff(text2, "</peak>", ref value5))) || (xi_calibration_version == 3 && (HSISectionOnOff(text2, "<peak version=\"1\" order=\"1\"", ref value5) || HSISectionOnOff(text2, "</peak>", ref value5))))
							{
								continue;
							}
						}
						if (!HSIParameterIn(text2, "<offset_x>", ref filter_active_area_offset_x) && !HSIParameterIn(text2, "<offset_y>", ref filter_active_area_offset_y))
						{
							if (HSIParameterIn(text2, "<width>", ref value6))
							{
								if (value)
								{
									sensor_width = value6;
								}
								else if (value2)
								{
									filter_active_area_width = value6;
								}
							}
							else if (HSIParameterIn(text2, "<height>", ref value6))
							{
								if (value)
								{
									sensor_height = value6;
								}
								else if (value2)
								{
									filter_active_area_height = value6;
								}
							}
							else
							{
								if (xi_calibration_version == 0)
								{
									if (HSIParameterIn(text2, "<mosaic_pattern_width>", ref filter_mosaic_pattern_width) || HSIParameterIn(text2, "<mosaic_pattern_height>", ref filter_mosaic_pattern_height) || HSIParameterIn(text2, "<start_spectral_range>", ref filter_start_spectral_range) || HSIParameterIn(text2, "<end_spectral_range>", ref filter_end_spectral_range) || HSIParameterIn(text2, "<start_calibration_range>", ref start_calibration_range) || HSIParameterIn(text2, "<end_calibration_range>", ref end_calibration_range) || HSIParameterIn(text2, "<calibration_resolution>", ref calibration_resolution))
									{
										continue;
									}
								}
								else if (xi_calibration_version >= 1)
								{
									if (HSIParameterIn(text2, "<pattern_width>", ref filter_mosaic_pattern_width) || HSIParameterIn(text2, "<pattern_height>", ref filter_mosaic_pattern_height))
									{
										if (filter_mosaic_pattern_width != 0 && filter_mosaic_pattern_height != 0)
										{
											filter_nr_bands = filter_mosaic_pattern_width * filter_mosaic_pattern_height;
										}
										continue;
									}
									if (HSIParameterIn(text2, "<spectral_range_start_nm>", ref filter_start_spectral_range) || HSIParameterIn(text2, "<spectral_range_end_nm>", ref filter_end_spectral_range) || HSIParameterIn(text2, "<measurement_range_start_nm>", ref start_calibration_range) || HSIParameterIn(text2, "<measurement_range_end_nm>", ref end_calibration_range) || HSIParameterIn(text2, "<measurement_resolution_nm>", ref calibration_resolution) || HSIParameterIn(text2, "<nr_measurements>", ref num_filter_responses) || HSIParameterIn(text2, "<wavelength_range_start_nm>", ref start_calibration_range) || HSIParameterIn(text2, "<wavelength_range_end_nm>", ref end_calibration_range) || HSIParameterIn(text2, "<wavelength_resolution_nm>", ref calibration_resolution))
									{
										continue;
									}
								}
								if (xi_calibration_version == 0)
								{
									string value8 = "<band number=\"";
									if (text2.StartsWith(value8))
									{
										if (value3 && ermittel_band_peaks(text2.Substring(value8.Length), ref index_peak, ref peak))
										{
											if (peak >= 0.0)
											{
												array_wavelengths[index_peak] = peak;
											}
											else
											{
												array_wavelengths[index_peak] = (double)(10000 + ++num);
											}
										}
										continue;
									}
									value8 = "<filter_responses>";
									if (text2.StartsWith(value8))
									{
										if (filter_nr_bands >= 1 && start_calibration_range >= 1 && end_calibration_range >= 1 && calibration_resolution >= 1)
										{
											num2 = 0;
											num_filter_responses = (end_calibration_range - start_calibration_range) / calibration_resolution + 1;
											flag2 = true;
											filter_responses_read = true;
											filter_responses = new double[filter_nr_bands, num_filter_responses];
											if (!get_filter_responses(text2.Substring(value8.Length), num2++))
											{
												flag2 = false;
												filter_responses_read = false;
											}
										}
										continue;
									}
								}
								if (xi_calibration_version == 1 || xi_calibration_version == 2)
								{
									if (HSIParameterIn(text2, "<band version=\"0\" pattern_position_index=\"", ref value7) || HSIParameterIn(text2, "<band version=\"2\" pattern_position_index=\"", ref value7))
									{
										continue;
									}
									string value8 = "<wavelength_nm>";
									if (text2.StartsWith(value8))
									{
										if (value7 >= 0 && value7 < filter_nr_bands && value5)
										{
											if (ermittel_band_peaks_new(text2.Substring(value8.Length), ref peak))
											{
												if (peak >= 0.0)
												{
													array_wavelengths[value7] = peak;
												}
												else
												{
													array_wavelengths[value7] = (double)(10000 + ++num);
												}
											}
											else
											{
												array_wavelengths[value7] = (double)value7;
											}
										}
										continue;
									}
									value8 = "<bands>";
									if (text2.StartsWith(value8))
									{
										if (filter_nr_bands >= 1 && start_calibration_range >= 1 && end_calibration_range >= 1 && calibration_resolution >= 1 && num_filter_responses >= 1)
										{
											filter_responses = new double[filter_nr_bands, num_filter_responses];
											filter_responses_read = true;
											flag = true;
										}
										continue;
									}
									value8 = "</bands>";
									if (text2.StartsWith(value8))
									{
										flag = false;
										continue;
									}
									if (value4 && flag && value7 >= 0 && value7 < filter_nr_bands)
									{
										value8 = "<data>";
										if (text2.StartsWith(value8) && !get_filter_responses_new(text2.Substring(value8.Length), value7))
										{
											filter_responses_read = false;
										}
									}
								}
								if (xi_calibration_version == 3 && !HSIParameterIn(text2, "<band version=\"3\" index=\"", ref value7))
								{
									string value8 = "<wavelength_nm>";
									if (text2.StartsWith(value8))
									{
										if (value7 >= 0 && value7 < filter_nr_bands && value5)
										{
											if (ermittel_band_peaks_new(text2.Substring(value8.Length), ref peak))
											{
												if (peak >= 0.0)
												{
													array_wavelengths[value7] = peak;
												}
												else
												{
													array_wavelengths[value7] = (double)(10000 + ++num);
												}
											}
											else
											{
												array_wavelengths[value7] = (double)value7;
											}
										}
									}
									else
									{
										value8 = "<bands>";
										if (text2.StartsWith(value8))
										{
											if(calibration_resolution != 0) { 
												num_filter_responses = (end_calibration_range - start_calibration_range + 1) / calibration_resolution;
											}
											if (filter_nr_bands >= 1 && start_calibration_range >= 1 && end_calibration_range >= 1 && calibration_resolution >= 1 && num_filter_responses >= 1)
											{
												filter_responses = new double[filter_nr_bands, num_filter_responses];
												filter_responses_read = true;
												flag = true;
											}
										}
										else
										{
											value8 = "</bands>";
											if (text2.StartsWith(value8))
											{
												flag = false;
											}
											else if (flag && value7 >= 0 && value7 < filter_nr_bands)
											{
												value8 = "<response nr_elements=\"601\">";
												if (text2.StartsWith(value8) && !get_filter_responses_new(text2.Substring(value8.Length), value7))
												{
													filter_responses_read = false;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			if ((xi_calibration_version != 0 && xi_calibration_version != 1 && xi_calibration_version != 2 && xi_calibration_version != 3) || (filter_nr_bands != 25 && filter_nr_bands != 16) || (filter_mosaic_pattern_width != 4 && filter_mosaic_pattern_width != 5) || (filter_mosaic_pattern_height != 4 && filter_mosaic_pattern_height != 5) || filter_mosaic_pattern_width != filter_mosaic_pattern_height || sensor_width != 2048 || sensor_height != 1088)
			{
				xi_calibration_version = -1;
				calibration_type = -1;
				filter_nr_bands = -1;
			}
			else
			{
				if (filter_mosaic_pattern_width > 0)
				{
					image_width = (filter_active_area_width - filter_active_area_offset_x) / filter_mosaic_pattern_width;
				}
				if (filter_mosaic_pattern_height > 0)
				{
					image_height = (filter_active_area_height - filter_active_area_offset_y) / filter_mosaic_pattern_height;
				}
				if (xi_calibration_version == 0)
				{
					xi_offset_x = filter_active_area_offset_x;
					xi_offset_y = sensor_height - filter_active_area_height - filter_active_area_offset_y;
					if (filter_mosaic_pattern_width == 4)
					{
						int[] array4 = new int[16]
						{
							12,
							13,
							14,
							15,
							8,
							9,
							10,
							11,
							4,
							5,
							6,
							7,
							0,
							1,
							2,
							3
						};
						for (int k = 0; k < filter_nr_bands; k++)
						{
							array2[k] = array4[k];
						}
					}
					else
					{
						int[] array5 = new int[25]
						{
							20,
							21,
							22,
							23,
							24,
							15,
							16,
							17,
							18,
							19,
							10,
							11,
							12,
							13,
							14,
							5,
							6,
							7,
							8,
							9,
							0,
							1,
							2,
							3,
							4
						};
						for (int l = 0; l < filter_nr_bands; l++)
						{
							array2[l] = array5[l];
						}
					}
				}
				else
				{
					xi_offset_x = filter_active_area_offset_x;
					xi_offset_y = filter_active_area_offset_y;
					for (int m = 0; m < filter_nr_bands; m++)
					{
						array2[m] = m;
					}
				}
				filter_nr_bands_active = filter_nr_bands - num;
				double num5 = 0.0;
				double num6 = 20000.0;
				for (int n = 0; n < filter_nr_bands; n++)
				{
					int num7 = 0;
					num6 = 20000.0;
					for (int num8 = 0; num8 < filter_nr_bands; num8++)
					{
						double num9 = array_wavelengths[num8];
						if (num9 > num5 && num9 < num6)
						{
							num7 = num8;
							num6 = num9;
						}
					}
					spectrumPos_2_BandNr[n] = num7;
					num5 = num6 + 0.001;
				}
				for (int num10 = 0; num10 < filter_nr_bands; num10++)
				{
					bandNr_2_SpectrumPos[spectrumPos_2_BandNr[num10]] = num10;
				}
				for (int num11 = 0; num11 < filter_nr_bands; num11++)
				{
					xiImage_2_SpectrumPos[num11] = bandNr_2_SpectrumPos[array2[num11]];
				}
			}
		}

		~HSIParameters()
		{
		}

		private bool ermittel_band_peaks(string buffer, ref int index_peak, ref double peak)
		{
			string[] array = buffer.Split('"', '>', '<', ',', ' ', '/', 'b', 'a', 'n', 'd');
			int num = -1;
			int num2 = 0;
			double num3 = 0.0;
			double num4 = 0.0;
			index_peak = 0;
			peak = 0.0;
			int num5 = 0;
			string[] array2 = array;
			foreach (string text in array2)
			{
				string text2 = text.Trim();
				if (!(text2 == ""))
				{
					switch (num5)
					{
					case 0:
						try
						{
							num = XmlConvert.ToInt32(text2);
						}
						catch
						{
						}
						break;
					case 1:
						try
						{
							num3 = XmlConvert.ToDouble(text2);
							num2++;
						}
						catch
						{
						}
						break;
					case 2:
						try
						{
							num4 = XmlConvert.ToDouble(text2);
							num2++;
						}
						catch
						{
						}
						break;
					}
					num5++;
				}
			}
			if (num > filter_nr_bands - 1 || num < 0 || num2 < 1)
			{
				return false;
			}
			index_peak = num;
			if (num2 == 1)
			{
				peak = num3;
				return true;
			}
			if (num3 <= (double)filterRangeEnd && num3 >= (double)filterRangeStart && (num4 > (double)filterRangeEnd || num4 < (double)filterRangeStart))
			{
				peak = num3;
			}
			else if (num4 <= (double)filterRangeEnd && num4 >= (double)filterRangeStart && (num3 > (double)filterRangeEnd || num3 < (double)filterRangeStart))
			{
				peak = num4;
			}
			else
			{
				peak = num3;
			}
			return true;
		}

		private bool ermittel_band_peaks_new(string buffer, ref double peak)
		{
			string[] array = buffer.Split('"', '>', '<', ',', ' ', '/');
			int num = 0;
			double num2 = 0.0;
			double num3 = 0.0;
			peak = 0.0;
			int num4 = 0;
			string[] array2 = array;
			foreach (string text in array2)
			{
				string text2 = text.Trim();
				if (!(text2 == ""))
				{
					switch (num4)
					{
					case 0:
						try
						{
							num2 = XmlConvert.ToDouble(text2);
							num++;
						}
						catch
						{
						}
						break;
					case 1:
						try
						{
							num3 = XmlConvert.ToDouble(text2);
							num++;
						}
						catch
						{
						}
						break;
					}
					num4++;
				}
			}
			if (num < 1)
			{
				return false;
			}
			if (num == 1)
			{
				peak = num2;
				return true;
			}
			if (num2 <= (double)filterRangeEnd && num2 >= (double)filterRangeStart && (num3 > (double)filterRangeEnd || num3 < (double)filterRangeStart))
			{
				peak = num2;
			}
			else if (num3 <= (double)filterRangeEnd && num3 >= (double)filterRangeStart && (num2 > (double)filterRangeEnd || num2 < (double)filterRangeStart))
			{
				peak = num3;
			}
			else
			{
				peak = num2;
			}
			return true;
		}

		private bool get_filter_responses(string buffer, int row)
		{
			string[] array = buffer.Split(',', ' ');
			int num = 0;
			double num2 = 0.0;
			string[] array2 = array;
			foreach (string text in array2)
			{
				string text2 = text.Trim();
				if (!(text2 == ""))
				{
					try
					{
						num2 = XmlConvert.ToDouble(text2);
					}
					catch
					{
					}
					filter_responses[num++, row] = num2;
					if (num2 > filter_max_value)
					{
						filter_max_value = num2;
					}
					if (num >= filter_nr_bands)
					{
						break;
					}
				}
			}
			if (num < filter_nr_bands)
			{
				return false;
			}
			return true;
		}

		private bool get_filter_responses_new(string buffer, int band)
		{
			string[] array = buffer.Split(',', ' ', '<');
			int num = 0;
			double num2 = 0.0;
			string[] array2 = array;
			foreach (string text in array2)
			{
				string text2 = text.Trim();
				if (!(text2 == ""))
				{
					try
					{
						num2 = XmlConvert.ToDouble(text2);
					}
					catch
					{
					}
					filter_responses[band, num++] = num2;
					if (num2 > filter_max_value)
					{
						filter_max_value = num2;
					}
					if (num >= num_filter_responses)
					{
						break;
					}
				}
			}
			if (num < num_filter_responses)
			{
				return false;
			}
			return true;
		}
	}
}
