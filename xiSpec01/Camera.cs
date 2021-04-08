using BitMiracle.LibTiff.Classic;
using Cyotek.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using xiApi.NET;

namespace xiSpec01
{
	internal class Camera
	{
		private xiCam xiH;

		private Form1 form;

		private CamParameters param;

		public HSIParameters HSIParam;

		private RichTextBox ausgabe;

		private Label labelCameraModel;

		private ImageBox imageBox;

		public string xi_device_name;

		public string xi_device_sn;

		public string xi_device_user_id;

		public string xi_device_type;

		private int interface_data_rate;

		private int device_opened;

		private int xi_cameraModel = 144;

		private int filterrangeStart;

		private int filterrangeEnd;

		private string xi_sensor_sn;

		private int exp_time;

		private int exp_time_old = -1;

		private int num_frames;

		private int resolutionMode = 1;

		public int[,] imageRAWSum;

		private string fileNameCalibrationFile = "sens_calib.dat";

		private string externalFileDirectory = "calib_data\\";

		private string externalFileNameStart_LS = "CMV2K-LS100-600_1000-";

		private string externalFileNameStart_5X5 = "CMV2K-SSM5x5-";

		private string externalFileNameStart_5X5_backup = "CMV2K-SSM5x5-600_1000-";

		private string externalFileNameStart_4X4 = "CMV2K-SSM4x4-470_620-";

		private string externalFileNameStart = "";

		private string externalFileNameStart_backup;

		private string externalFileName;

		private string externalFileName_used;

		private string externalFileName_backup;

		private string internalFileNameStart_LS = "LS100-600_1000-";

		private string internalFileNameStart_5X5 = "SSM5x5-";

		private string internalFileNameStart_5X5_backup = "SSM5x5-600_1000-";

		private string internalFileNameStart_4X4 = "SSM4x4-470_620-";

		private string internalFileNameStart = "";

		private string internalFileNameStart_backup;

		private string internalFileName;

		private string internalFileName_used;

		private string internalFileName_backup;

		private string imageFileDirectory = "images\\";

		private string imageFileNameStart;

		private string imageFileName;

		private FormDistance fDistance;

		public string xi_devive_sn
		{
			get;
			set;
		}

		public Camera(CamParameters pParam)
		{
			param = new CamParameters(pParam.index_cam);
			exp_time = (exp_time_old = (param.exposure_time = pParam.exposure_time));
			param.output_bits = pParam.output_bits;
			param.use_autobandwidth = pParam.use_autobandwidth;
			param.use_packed_mode = pParam.use_packed_mode;
		}

		~Camera()
		{
		}

		private byte[] GetUncompressedPayload(byte[] data)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (MemoryStream baseInputStream = new MemoryStream(data))
				{
					using (ZipInputStream zipInputStream = new ZipInputStream(baseInputStream))
					{
						zipInputStream.GetNextEntry();
						zipInputStream.CopyTo(memoryStream);
					}
					return memoryStream.ToArray();
				}
			}
		}

		private bool openCamera()
		{
			bool flag = false;
			try
			{
				xiH.OpenDevice(param.index_cam);
				device_opened = 1;
			}
			catch
			{
				flag = true;
			}
			if (flag && ausgabe != null)
			{
				ausgabe.AppendText($"OpenDevice({param.index_cam}) FAILED!\n");
				ausgabe.ScrollToCaret();
			}
			if (!flag)
			{
				return true;
			}
			return false;
		}

		private bool readCameraParameters()
		{
			bool flag = false;
			try
			{
				xiH.SetParam("auto_bandwidth_calculation", param.use_autobandwidth ? 1 : 0);
				xiH.GetParam("auto_bandwidth_calculation", out int _);
				xiH.GetParam("device_name", out xi_device_name);
				xiH.GetParam("device_type", out xi_device_type);
				xiH.GetParam("device_sn", out xi_device_sn);
			}
			catch
			{
				if (ausgabe != null)
				{
					ausgabe.AppendText($"Set autobandwidth ... FAILED!\n");
					ausgabe.ScrollToCaret();
				}
				flag = true;
			}
			if (!flag)
			{
				try
				{
					xiH.GetParam("device_user_id", out xi_device_user_id);
				}
				catch
				{
				}
			}
			if (!flag)
			{
				try
				{
					if (ausgabe != null)
					{
						ausgabe.AppendText($"{xi_device_name}  #{xi_device_sn}  (User-ID={xi_device_user_id})\n");
						ausgabe.ScrollToCaret();
					}
					xiH.GetParam("available_bandwidth", out interface_data_rate);
					ausgabe.AppendText($"{xi_device_type} Bandwidth={interface_data_rate} MBit/s\n");
				}
				catch
				{
					if (ausgabe != null)
					{
						ausgabe.AppendText($"get current bandwidth FAILED!\n");
						ausgabe.ScrollToCaret();
					}
					flag = true;
				}
			}
			if (!flag)
			{
				labelCameraModel.Text = $"{xi_device_name}  SerNr: {xi_device_sn}";
			}
			if (!flag)
			{
				return true;
			}
			return false;
		}

		private bool checkValidXiSpec()
		{
			bool flag = false;
			externalFileNameStart_backup = null;
			try
			{
				xiH.GetParam("device_model_id", out xi_cameraModel);
				if (xi_cameraModel != 136 && xi_cameraModel != 144)
				{
					if (ausgabe != null)
					{
						ausgabe.AppendText($"ID={xi_cameraModel}: No currently supported xiSpec camera model found!\n");
						ausgabe.ScrollToCaret();
					}
					flag = true;
				}
				else
				{
					if (filterrangeStart == 0)
					{
						if (xi_cameraModel == 136)
						{
							filterrangeStart = 450;
						}
						else if (xi_cameraModel == 144)
						{
							filterrangeStart = 675;
						}
					}
					if (filterrangeEnd == 0)
					{
						if (xi_cameraModel == 136)
						{
							filterrangeEnd = 650;
						}
						else if (xi_cameraModel == 144)
						{
							filterrangeEnd = 975;
						}
					}
					switch (xi_cameraModel)
					{
					case 308:
						externalFileNameStart = externalFileNameStart_LS;
						internalFileNameStart = internalFileNameStart_LS;
						break;
					case 136:
						externalFileNameStart = externalFileNameStart_4X4;
						internalFileNameStart = internalFileNameStart_4X4;
						break;
					case 144:
						externalFileNameStart_backup = externalFileNameStart_5X5_backup;
						internalFileNameStart_backup = internalFileNameStart_5X5_backup;
						if (filterrangeStart == 600 && filterrangeEnd == 875)
						{
							externalFileNameStart = externalFileNameStart_5X5 + "600_875-";
							internalFileNameStart = internalFileNameStart_5X5 + "600_875-";
						}
						else if (filterrangeStart == 675 && filterrangeEnd == 975)
						{
							externalFileNameStart = externalFileNameStart_5X5 + "675_975-";
							internalFileNameStart = internalFileNameStart_5X5 + "675_975-";
						}
						else
						{
							externalFileNameStart = externalFileNameStart_5X5_backup;
							externalFileNameStart_backup = null;
							internalFileNameStart = internalFileNameStart_5X5_backup;
							internalFileNameStart_backup = null;
						}
						break;
					}
					if (ausgabe != null)
					{
						switch (xi_cameraModel)
						{
						case 308:
							ausgabe.AppendText($"xiSpec camera model LS100-NIR found!\n");
							break;
						case 136:
							ausgabe.AppendText($"xiSpec camera model SM4X4-VIS found!\n");
							break;
						case 144:
							ausgabe.AppendText($"xiSpec camera model SM5X5-NIR found!\n");
							break;
						default:
							ausgabe.AppendText($"xiSpec camera model ERROR!\n");
							break;
						}
						ausgabe.ScrollToCaret();
					}
				}
			}
			catch
			{
				if (ausgabe != null)
				{
					ausgabe.AppendText($"Get device model FAILED!\n");
					ausgabe.ScrollToCaret();
				}
				flag = true;
			}
			if (!flag)
			{
				try
				{
					xiH.GetParam("device_sens_sn", out xi_sensor_sn);
					if (ausgabe != null)
					{
						ausgabe.AppendText($"xiSpec IMEC sensor-ID = {xi_sensor_sn}\n");
						ausgabe.ScrollToCaret();
					}
				}
				catch
				{
					if (ausgabe != null)
					{
						ausgabe.AppendText($"Get sensor ser-Nr FAILED!\n");
						ausgabe.ScrollToCaret();
					}
					flag = true;
				}
			}
			if (!flag)
			{
				externalFileName = externalFileDirectory + externalFileNameStart + xi_sensor_sn + ".xml";
				if (externalFileNameStart_backup != null)
				{
					externalFileName_backup = externalFileDirectory + externalFileNameStart_backup + xi_sensor_sn + ".xml";
				}
				else
				{
					externalFileName_backup = null;
				}
				internalFileName = internalFileNameStart + xi_sensor_sn;
				if (internalFileNameStart_backup != null)
				{
					internalFileName_backup = internalFileNameStart_backup + xi_sensor_sn;
				}
				else
				{
					internalFileName_backup = null;
				}
			}
			if (!flag)
			{
				return true;
			}
			return false;
		}

		private bool readInternalFile(out string buffer)
		{
			bool flag = false;
			bool flag2 = false;
			buffer = "";
			for (int i = 0; i < 6; i++)
			{
				if (flag)
				{
					break;
				}
				switch (i)
				{
				case 0:
					internalFileName_used = internalFileName + ".zip";
					flag2 = true;
					break;
				case 1:
					internalFileName_used = internalFileName + ".xml";
					flag2 = false;
					break;
				case 2:
					internalFileName_used = internalFileName + ".xmll";
					flag2 = false;
					break;
				case 3:
					internalFileName_used = internalFileName_backup + ".zip";
					flag2 = true;
					break;
				case 4:
					internalFileName_used = internalFileName_backup + ".xml";
					flag2 = false;
					break;
				default:
					internalFileName_used = internalFileName_backup + ".xmll";
					flag2 = false;
					break;
				}
				xiH.SetParam("ffs_file_name", internalFileName_used);
				if (flag2)
				{
					try
					{
						byte[] paramByteArr = xiH.GetParamByteArr("read_file_ffs");
						int num = paramByteArr.Length;
						flag = true;
						ausgabe.AppendText($"Camera calibration file\n\t{internalFileName_used}\nhas been read, length = {num} bytes\n");
						ausgabe.ScrollToCaret();
						byte[] uncompressedPayload = GetUncompressedPayload(paramByteArr);
						buffer = Encoding.UTF8.GetString(uncompressedPayload);
						int length = buffer.Length;
						ausgabe.AppendText($"  Unzipped length = {length} bytes\n");
					}
					catch
					{
					}
				}
				else
				{
					try
					{
						xiH.GetParam("read_file_ffs", out buffer);
						int num = buffer.Length;
						flag = true;
						ausgabe.AppendText($"Camera calibration file\n\t{internalFileName_used}\nhas been read, length = {num} bytes\n");
						ausgabe.ScrollToCaret();
					}
					catch
					{
					}
				}
			}
			return flag;
		}

		private bool readExternalFile(out string buffer)
		{
			buffer = "";
			externalFileName_used = externalFileName;
			if (File.Exists(externalFileName_used))
			{
				try
				{
					buffer = File.ReadAllText(externalFileName_used);
					buffer += '\0';
				}
				catch
				{
					return false;
				}
			}
			else
			{
				if (externalFileName_backup == null)
				{
					return false;
				}
				externalFileName_used = externalFileName_backup;
				if (File.Exists(externalFileName_used))
				{
					try
					{
						buffer = File.ReadAllText(externalFileName_used);
						buffer += '\0';
					}
					catch
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool readXiSpecCalibrationData(bool ignoreCameraCalib, string fileNameTest)
		{
			bool flag = false;
			bool flag2 = false;
			string val = "";
			int num = 0;
			if (!ignoreCameraCalib)
			{
				flag = false;
				try
				{
					xiH.GetParam("free_ffs_size", out int val2);
					ausgabe.AppendText($"free file system size = {val2}\n");
					xiH.GetParam("used_ffs_size", out val2);
					ausgabe.AppendText($"used file system size = {val2}\n");
					xiH.SetParam("ffs_file_name", fileNameCalibrationFile);
					ausgabe.AppendText($"camera: filename = {fileNameCalibrationFile}");
					xiH.GetParam("read_file_ffs", out val);
					num = val.Length;
					ausgabe.AppendText($" has been read.\nlength = {num} bytes\n");
					ausgabe.ScrollToCaret();
				}
				catch
				{
					ausgabe.AppendText($", reading file FAILED\n");
					ausgabe.AppendText($"--> try to read external calibration data\n");
					ausgabe.ScrollToCaret();
					flag = true;
				}
				if (!flag)
				{
					XmlReader xmlReader = XmlReader.Create(new StringReader(val));
					while (xmlReader.Read())
					{
						if (xmlReader.NodeType == XmlNodeType.Element)
						{
							if (xmlReader.Name == "calibrations")
							{
								flag2 = true;
							}
							break;
						}
					}
					if (flag2)
					{
						ausgabe.AppendText($"new XIMEA HSI file system found\n");
						ausgabe.ScrollToCaret();
						if (!readInternalFile(out val))
						{
							ausgabe.AppendText($"reading INTERNAL xiSpec sensor calibration file FAILED\n");
							ausgabe.ScrollToCaret();
							flag = true;
						}
					}
				}
			}
			else
			{
				if (fileNameTest != "")
				{
					externalFileName = fileNameTest;
				}
				flag = true;
			}
			if (flag)
			{
				if (readExternalFile(out val))
				{
					flag = false;
				}
				else if (ausgabe != null)
				{
					num = val.Length;
					ausgabe.AppendText($"reading EXTERNAL xiSpec sensor calibration file FAILED\n");
					ausgabe.ScrollToCaret();
				}
				if (!flag && ausgabe != null)
				{
					num = val.Length;
					ausgabe.AppendText($"EXTERNAL xiSpec sensor calibration file\n\t{externalFileName_used}\nhas been read, length = {num} bytes\n");
					ausgabe.ScrollToCaret();
				}
			}
			if (!flag)
			{
				HSIParam = new HSIParameters(xi_cameraModel, val, filterrangeStart, filterrangeEnd, ausgabe);
				if (HSIParam.filter_nr_bands < 0)
				{
					if (ausgabe != null)
					{
						ausgabe.AppendText($"Calibration parameter NOT CORRECT\n");
						ausgabe.ScrollToCaret();
					}
					flag = true;
				}
				else if (ausgabe != null)
				{
					ausgabe.AppendText($"Calibration parameter\n");
					ausgabe.AppendText($"number of bands = {HSIParam.filter_nr_bands}, {HSIParam.filter_mosaic_pattern_width}x{HSIParam.filter_mosaic_pattern_height} Array\n");
					ausgabe.AppendText($"active area = {HSIParam.filter_active_area_offset_x},{HSIParam.filter_active_area_offset_y} - {HSIParam.filter_active_area_width},{HSIParam.filter_active_area_height}\n");
					ausgabe.AppendText($"effective HSI image size = {HSIParam.image_width} x {HSIParam.image_height} Pixel\n");
					ausgabe.AppendText($"Filter range: {filterrangeStart} - {filterrangeEnd}\n");
					ausgabe.AppendText($"band-number wavelength\n");
					for (int i = 0; i < HSIParam.filter_nr_bands; i++)
					{
						ausgabe.AppendText($"   peak {i} = {HSIParam.array_wavelengths[i]}\n");
					}
					ausgabe.AppendText($"LUT(band number) wavelength\n");
					for (int j = 0; j < HSIParam.filter_nr_bands; j++)
					{
						ausgabe.AppendText($"   {HSIParam.spectrumPos_2_BandNr[j]} : {HSIParam.array_wavelengths[HSIParam.spectrumPos_2_BandNr[j]]} nm\n");
					}
					ausgabe.ScrollToCaret();
				}
			}
			if (!flag)
			{
				return true;
			}
			return false;
		}

		private static void SaveTiff16(Bitmap image, string fileName)
		{
			using (Tiff tiff = Tiff.Open(fileName, "w"))
			{
				int width = image.Width;
				int height = image.Height;
				tiff.SetField(TiffTag.IMAGEWIDTH, width);
				tiff.SetField(TiffTag.IMAGELENGTH, height);
				tiff.SetField(TiffTag.SAMPLESPERPIXEL, 1);
				tiff.SetField(TiffTag.BITSPERSAMPLE, 8);
				tiff.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
				tiff.SetField(TiffTag.ROWSPERSTRIP, height);
				tiff.SetField(TiffTag.XRESOLUTION, 96.0);
				tiff.SetField(TiffTag.YRESOLUTION, 96.0);
				tiff.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
				tiff.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
				tiff.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
				tiff.SetField(TiffTag.COMPRESSION, Compression.NONE);
				tiff.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);
				byte[] array = new byte[width * height];
				Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
				BitmapData bitmapData = image.LockBits(rect, ImageLockMode.ReadWrite, image.PixelFormat);
				Marshal.Copy(bitmapData.Scan0, array, 0, width * height);
				tiff.WriteEncodedStrip(0, array, width * height);
				image.UnlockBits(bitmapData);
			}
		}

		private bool saveHSIcube(HSIImage hsi_image, Bitmap newBitmap, bool useInterpolatedImage)
		{
			string arg = "";
			string arg2 = "";
			string arg3 = "";
			int num = 0;
			int num2 = 0;
			NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
			numberFormatInfo.NumberDecimalSeparator = ".";
			numberFormatInfo.NumberGroupSeparator = ",";
			num = form.maximizeActive();
			num2 = form.derivationActive();
			if (num == 1)
			{
				arg2 = "values normalized";
			}
			switch (num2)
			{
			case 1:
				arg3 = "1. derivation";
				break;
			case 2:
				arg3 = "2. derivation";
				break;
			}
			switch (xi_cameraModel)
			{
			case 308:
				arg = "LS100-NIR";
				break;
			case 136:
				arg = "SSM4x4-VIS";
				break;
			case 144:
				arg = "SSM5x5-NIR";
				break;
			}
			string text = $"{DateTime.Now:yyyyMMddHHmmss}";
			imageFileNameStart = $"{arg}_{xi_device_sn}_{text}";
			imageFileName = $"{imageFileDirectory}{imageFileNameStart}.hdr";
			try
			{
				TextWriter textWriter = new StreamWriter(imageFileName);
				textWriter.WriteLine("ENVI");
				textWriter.WriteLine($"description = {{ XIMEA xiSpec {xi_device_name}, SerNr {xi_device_sn}, Sensor-SN {xi_sensor_sn}");
				if (num == 1 || num2 != 0)
				{
					textWriter.WriteLine(string.Format("{0}{1}{2}", arg2, (num == 1 && num2 != 0) ? ", " : "", arg3));
				}
				textWriter.WriteLine($"date/time {text} }}");
				textWriter.WriteLine("sensor type = IMEC {0}", arg);
				textWriter.WriteLine("file type = ENVI Standard");
				textWriter.WriteLine("header offset = 0");
				if (useInterpolatedImage)
				{
					textWriter.WriteLine("samples = {0}", hsi_image.width_interpol);
					textWriter.WriteLine("lines = {0}", hsi_image.height_interpol);
				}
				else
				{
					textWriter.WriteLine("samples = {0}", hsi_image.width_spatial);
					textWriter.WriteLine("lines = {0}", hsi_image.height_spatial);
				}
				textWriter.WriteLine("bands = {0}", HSIParam.filter_nr_bands);
				textWriter.WriteLine("data type = 12");
				textWriter.WriteLine("interleave = bsq");
				textWriter.WriteLine("byte order = 0");
				textWriter.WriteLine("wavelength = {");
				for (int i = 0; i < HSIParam.filter_nr_bands; i++)
				{
					textWriter.WriteLine(string.Format(" {0}{1}", HSIParam.array_wavelengths[HSIParam.spectrumPos_2_BandNr[i]].ToString(numberFormatInfo), (i >= HSIParam.filter_nr_bands - 1) ? "}" : ","));
				}
				textWriter.Close();
				imageFileName = $"{imageFileDirectory}{imageFileNameStart}.bsq";
				FileStream fileStream = new FileStream(imageFileName, FileMode.CreateNew);
				BinaryWriter binaryWriter = new BinaryWriter(fileStream);
				hsi_image.BSQwriteHSIImage(binaryWriter, useInterpolatedImage);
				binaryWriter.Close();
				fileStream.Close();
				imageFileName = $"{imageFileDirectory}{imageFileNameStart}.tif";
				SaveTiff16(newBitmap, imageFileName);
				if (ausgabe != null)
				{
					ausgabe.AppendText($"HDR-file {imageFileNameStart} written\n");
					ausgabe.ScrollToCaret();
				}
			}
			catch
			{
			}
			return true;
		}

		public unsafe void addFrameToAvg(Bitmap sum, Bitmap source, int mode)
		{
			BitmapData bitmapData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadWrite, source.PixelFormat);
			int num = Image.GetPixelFormatSize(source.PixelFormat) / 8;
			int height = bitmapData.Height;
			int width = bitmapData.Width;
			byte* ptr = (byte*)(void*)bitmapData.Scan0;
			if (num == 1)
			{
				for (int i = 0; i < height; i++)
				{
					byte* ptr2 = ptr + i * bitmapData.Stride / num;
					switch (mode)
					{
					case -1:
						for (int k = 0; k < width; k++)
						{
							int[,] array = imageRAWSum;
							int num2 = i;
							int num3 = k;
							byte* intPtr2 = ptr2;
							ptr2 = intPtr2 + 1;
							byte num4 = *intPtr2;
							array[num2, num3] = num4;
						}
						break;
					case 0:
						for (int l = 0; l < width; l++)
						{
							ref int reference = ref imageRAWSum[i, l];
							int num5 = reference;
							byte* intPtr3 = ptr2;
							ptr2 = intPtr3 + 1;
							reference = num5 + *intPtr3;
						}
						break;
					default:
						for (int j = 0; j < width; j++)
						{
							imageRAWSum[i, j] += *ptr2;
							byte* intPtr = ptr2;
							ptr2 = intPtr + 1;
							*intPtr = (byte)(imageRAWSum[i, j] / mode);
						}
						break;
					}
				}
				source.UnlockBits(bitmapData);
			}
		}

		public void run(Form1 p_form, RichTextBox pAusgabe, ImageBox p_imageBox, Label pLabelCameraModel, int filterStart, int filterEnd, FormDistance formDistance, bool ignoreCameraCalib, string fileNameTest)
		{

			int num = 2048;
			int num2 = 1088;
			int num3 = -1;
			bool flag = false;
			bool flag2 = false;
			form = p_form;
			ausgabe = pAusgabe;
			labelCameraModel = pLabelCameraModel;
			labelCameraModel.Text = "";
			fDistance = formDistance;
			imageBox = p_imageBox;
			imageBox.Image = null;
			imageBox.Refresh();
			imageRAWSum = new int[num2, num];
			filterrangeStart = filterStart;
			filterrangeEnd = filterEnd;
			xiH = new xiCam();
			form.inc_thread_counter();
			bool flag3 = !openCamera() || !readCameraParameters() || !checkValidXiSpec() || !readXiSpecCalibrationData(ignoreCameraCalib, fileNameTest);
			if (flag3)
			{
				if (device_opened != 0)
				{
					xiH.StopAcquisition();
					xiH.CloseDevice();
				}
				form.dec_thread_counter();
			}
			else
			{
				form.set_HSIParam(HSIParam);
				if (!flag3)
				{
					try
					{
						int val = 5;
						string arg = "RAW8";
						xiH.SetParam("imgdataformat", val);
						xiH.SetParam("exposure", param.exposure_time);
						xiH.StartAcquisition();
						xiH.GetImage(out Bitmap image, 1000);
						num = image.Width;
						num2 = image.Height;
						if (ausgabe != null)
						{
							ausgabe.AppendText($"Resolution({arg}) = {num} * {num2}\n");
							ausgabe.ScrollToCaret();
						}
						xiH.SetParam("buffer_policy", 0);
					}
					catch
					{
						flag3 = true;
					}
				}
				if (!flag3)
				{
					try
					{
						Bitmap image2 = new Bitmap(num, num2, PixelFormat.Format8bppIndexed);
						Bitmap sum = new Bitmap(num, num2, PixelFormat.Format32bppRgb);
						HSIImage hSIImage = new HSIImage(HSIParam, fDistance);
						int width_spatial = hSIImage.width_spatial;
						int height_spatial = hSIImage.height_spatial;
						Bitmap bitmap = new Bitmap(width_spatial, height_spatial, PixelFormat.Format32bppRgb);
						Bitmap bitmap2 = new Bitmap(width_spatial, height_spatial, PixelFormat.Format32bppRgb);
						Rectangle rect = new Rectangle(0, 0, width_spatial, height_spatial);
						width_spatial = hSIImage.width_interpol;
						height_spatial = hSIImage.height_interpol;
						Bitmap bitmap3 = new Bitmap(width_spatial, height_spatial, PixelFormat.Format32bppRgb);
						Bitmap bitmap4 = new Bitmap(width_spatial, height_spatial, PixelFormat.Format32bppRgb);
						Rectangle rect2 = new Rectangle(0, 0, width_spatial, height_spatial);
						bool flag4 = false;
						bool flag5 = false;
						int num4 = -1;
						int num5 = 0;
						int num6 = -1;
						form.register_HSIImage(hSIImage);
						while (!form.stop_pressed())
						{
							int num7 = 2;
							int num8 = 0;
							num_frames = form.get_NumFramesToAvg();
							try
							{
								exp_time = form.getExpTime();
								if (exp_time != exp_time_old)
								{
									exp_time_old = exp_time;
									xiH.SetParam("exposure:direct_update", exp_time);
								}
							}
							catch
							{
							}
							for (int i = 0; i < num_frames; i++)
							{
								num8 = 0;
								for (int j = 0; j < num7 && num8 >= j; j++)
								{
									try
									{
										xiH.GetImage(out image2, 1000);
									}
									catch
									{
										num8++;
									}
								}
								if (num8 >= num7)
								{
									xiH.GetImage(out image2, 1000);
								}
								if (num_frames == 1)
								{
									break;
								}
								if (i == 0)
								{
									addFrameToAvg(sum, image2, -1);
								}
								else if (i + 1 >= num_frames)
								{
									addFrameToAvg(sum, image2, num_frames);
								}
								else
								{
									addFrameToAvg(sum, image2, 0);
								}
							}
							resolutionMode = form.resolutionMethod();
							flag4 = (resolutionMode == 2);
							flag5 = ((num4 = form.show_NumMeasurementFields()) != -1);
							num5 = form.compareSpectraActive();
							num6 = form.get_bandNr_only();
							flag4 = ((!flag5 && num5 == 0 && resolutionMode == 2 && num6 <= -1) ? true : false);
							flag2 = ((resolutionMode != num3 || flag4 != flag) ? true : false);
							num3 = resolutionMode;
							flag = flag4;
							if (form.measurementStarted() && flag5)
							{
								hSIImage.measureHSIImage(image2, num4);
								form.measurementReady();
							}
							if (form.darkImageStarted())
							{
								hSIImage.darkHSIImage(image2);
								form.darkImageReady();
							}
							if (form.whiteImageStarted())
							{
								hSIImage.whiteHSIImage(image2, exp_time);
								form.whiteImageReady();
							}
							if (!hSIImage.createHSIImage(image2, exp_time, form.maximizeActive(), form.derivationActive(), form.get_statusParallelProcessing(), onlyRAWImage: false, flag4))
							{
								if (ausgabe != null)
								{
									ausgabe.AppendText($"createHSIImage FAILED\n");
									ausgabe.ScrollToCaret();
								}
							}
							else if (resolutionMode == 0)
							{
								imageBox.Image = image2;
								if (flag2)
								{
									imageBox.ZoomToFit();
								}
								imageBox.Refresh();
							}
							else
							{
								if (form.saveHSIcubeStarted())
								{
									saveHSIcube(hSIImage, image2, flag4);
									form.saveHSIcubeReady();
								}
								if (hSIImage.HSIImageToBitmap(flag4 ? bitmap4 : bitmap2, flag4, num5, num6, form.getImageViewMode(), form.distanceMethod(), form.getMousePoint(), num4, form.get_statusParallelProcessing()))
								{
									form.set_HSIImage(hSIImage, flag4);
									if (flag4)
									{
										bitmap3 = bitmap4.Clone(rect2, PixelFormat.Format32bppRgb);
									}
									else
									{
										bitmap = bitmap2.Clone(rect, PixelFormat.Format32bppRgb);
									}
									if (num5 == 0 && num6 >= 0 && num6 < HSIParam.filter_nr_bands_active)
									{
										RectangleF layoutRectangle = new RectangleF(0f, 0f, 100f, 30f);
										Graphics graphics = Graphics.FromImage(flag4 ? bitmap3 : bitmap);
										string s = $"{HSIParam.array_wavelengths[HSIParam.spectrumPos_2_BandNr[num6]]:f2} nm";
										graphics.DrawString(s, new Font("Tahoma", 8f), Brushes.Red, layoutRectangle);
										graphics.Flush();
									}
									imageBox.Image = (flag4 ? bitmap3 : bitmap);
									if (flag2)
									{
										imageBox.ZoomToFit();
									}
									imageBox.Refresh();
								}
							}
						}
						xiH.StopAcquisition();
					}
					catch
					{
						flag3 = true;
					}
					finally
					{
						if (device_opened != 0)
						{
							xiH.StopAcquisition();
							xiH.CloseDevice();
						}
						labelCameraModel.Text += "  -  stopped";
						ausgabe.AppendText("Done\n");
						ausgabe.ScrollToCaret();
						form.dec_thread_counter();
					}
				}
			}
		}
	}
}
