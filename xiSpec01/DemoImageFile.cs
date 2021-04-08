using System.IO;

namespace xiSpec01
{
	public class DemoImageFile
	{
		private int modus = -1;

		private bool dataSet;

		public string xi_devive_sn;

		public int sensor_width;

		public int sensor_height;

		public int bands;

		public int filterRangeStart;

		public int filterRangeEnd;

		public int exp_time_image;

		public int exp_time_white;

		public bool measured;

		public bool darkMeasured;

		public bool whiteMeasured;

		public int MAX_MATERIALS;

		public int numMaterialsToUse;

		public int numFramesToAvg = 3;

		public int maximizeMode;

		public bool statusParallelProcessing;

		public int compareSpectraMode;

		public int pictureStretchMode;

		public int derivationMode;

		public int distanceMode;

		public int resolutionMode;

		public int imageViewMode;

		public double[,] spectrum;

		public int[,] whiteImageRAW;

		public int[,] darkImageRAW;

		public DemoImageFile(HSIImage image, Form1 form, string p_xi_devive_sn)
		{
			exp_time_image = form.exp_time;
			filterRangeStart = form.filterRangeStart;
			filterRangeEnd = form.filterRangeEnd;
			maximizeMode = form.maximizeMode;
			statusParallelProcessing = form.statusParallelProcessing;
			compareSpectraMode = form.compareSpectraMode;
			pictureStretchMode = form.pictureStretchMode;
			derivationMode = form.derivationMode;
			distanceMode = form.distanceMode;
			resolutionMode = form.resolutionMode;
			imageViewMode = form.imageViewMode;
			numFramesToAvg = form.numFramesToAvg;
			xi_devive_sn = p_xi_devive_sn;
			sensor_height = image.sensor_height;
			sensor_width = image.sensor_width;
			bands = image.bands;
			MAX_MATERIALS = image.MAX_MATERIALS;
			exp_time_white = image.exp_time_white;
			measured = image.measured;
			darkMeasured = image.darkMeasured;
			whiteMeasured = image.whiteMeasured;
			numMaterialsToUse = image.numMaterialsToUse;
			whiteImageRAW = new int[sensor_height, sensor_width];
			darkImageRAW = new int[sensor_height, sensor_width];
			spectrum = new double[MAX_MATERIALS + 1, bands];
			for (int i = 0; i < sensor_height; i++)
			{
				for (int j = 0; j < sensor_width; j++)
				{
					int[,] array = darkImageRAW;
					int num = i;
					int num2 = j;
					int num3 = image.darkImageRAW[i, j];
					array[num, num2] = num3;
					int[,] array2 = whiteImageRAW;
					int num4 = i;
					int num5 = j;
					int num6 = image.whiteImageRAW[i, j];
					array2[num4, num5] = num6;
				}
			}
			for (int k = 0; k < MAX_MATERIALS + 1; k++)
			{
				for (int l = 0; l < bands; l++)
				{
					double[,] array3 = spectrum;
					int num7 = k;
					int num8 = l;
					double num9 = image.spectrum[k, l];
					array3[num7, num8] = num9;
				}
			}
			modus = 1;
			dataSet = true;
		}

		public DemoImageFile()
		{
			modus = 0;
			dataSet = false;
		}

		public bool saveDemoImageFile()
		{
			string path = "xiSpecDemoImage.bin";
			if (modus != 1 || !dataSet)
			{
				return false;
			}
			try
			{
				FileStream fileStream = new FileStream(path, FileMode.Create);
				BinaryWriter binaryWriter = new BinaryWriter(fileStream);
				binaryWriter.Write(exp_time_image);
				binaryWriter.Write(filterRangeStart);
				binaryWriter.Write(filterRangeEnd);
				binaryWriter.Write(maximizeMode);
				binaryWriter.Write(statusParallelProcessing);
				binaryWriter.Write(compareSpectraMode);
				binaryWriter.Write(pictureStretchMode);
				binaryWriter.Write(derivationMode);
				binaryWriter.Write(distanceMode);
				binaryWriter.Write(resolutionMode);
				binaryWriter.Write(imageViewMode);
				binaryWriter.Write(xi_devive_sn);
				binaryWriter.Write(sensor_height);
				binaryWriter.Write(sensor_width);
				binaryWriter.Write(bands);
				binaryWriter.Write(MAX_MATERIALS);
				binaryWriter.Write(exp_time_white);
				binaryWriter.Write(measured);
				binaryWriter.Write(darkMeasured);
				binaryWriter.Write(whiteMeasured);
				binaryWriter.Write(numMaterialsToUse);
				for (int i = 0; i < sensor_height; i++)
				{
					for (int j = 0; j < sensor_width; j++)
					{
						binaryWriter.Write(darkImageRAW[i, j]);
						binaryWriter.Write(whiteImageRAW[i, j]);
					}
				}
				for (int k = 0; k < MAX_MATERIALS + 1; k++)
				{
					for (int l = 0; l < bands; l++)
					{
						binaryWriter.Write(spectrum[k, l]);
					}
				}
				binaryWriter.Write(numFramesToAvg);
				binaryWriter.Close();
				fileStream.Close();
			}
			catch
			{
				return false;
			}
			return true;
		}

		public bool readDemoImageFile()
		{
			string path = "xiSpecDemoImage.bin";
			if (modus != 0)
			{
				return false;
			}
			try
			{
				FileStream fileStream = new FileStream(path, FileMode.Open);
				BinaryReader binaryReader = new BinaryReader(fileStream);
				exp_time_image = binaryReader.ReadInt32();
				filterRangeStart = binaryReader.ReadInt32();
				filterRangeEnd = binaryReader.ReadInt32();
				maximizeMode = binaryReader.ReadInt32();
				statusParallelProcessing = binaryReader.ReadBoolean();
				compareSpectraMode = binaryReader.ReadInt32();
				pictureStretchMode = binaryReader.ReadInt32();
				derivationMode = binaryReader.ReadInt32();
				distanceMode = binaryReader.ReadInt32();
				resolutionMode = binaryReader.ReadInt32();
				imageViewMode = binaryReader.ReadInt32();
				xi_devive_sn = binaryReader.ReadString();
				sensor_height = binaryReader.ReadInt32();
				sensor_width = binaryReader.ReadInt32();
				bands = binaryReader.ReadInt32();
				MAX_MATERIALS = binaryReader.ReadInt32();
				exp_time_white = binaryReader.ReadInt32();
				measured = binaryReader.ReadBoolean();
				darkMeasured = binaryReader.ReadBoolean();
				whiteMeasured = binaryReader.ReadBoolean();
				numMaterialsToUse = binaryReader.ReadInt32();
				whiteImageRAW = new int[sensor_height, sensor_width];
				darkImageRAW = new int[sensor_height, sensor_width];
				spectrum = new double[MAX_MATERIALS + 1, bands];
				for (int i = 0; i < sensor_height; i++)
				{
					for (int j = 0; j < sensor_width; j++)
					{
						int[,] array = darkImageRAW;
						int num = i;
						int num2 = j;
						int num3 = binaryReader.ReadInt32();
						array[num, num2] = num3;
						int[,] array2 = whiteImageRAW;
						int num4 = i;
						int num5 = j;
						int num6 = binaryReader.ReadInt32();
						array2[num4, num5] = num6;
					}
				}
				for (int k = 0; k < MAX_MATERIALS + 1; k++)
				{
					for (int l = 0; l < bands; l++)
					{
						double[,] array3 = spectrum;
						int num7 = k;
						int num8 = l;
						double num9 = binaryReader.ReadDouble();
						array3[num7, num8] = num9;
					}
				}
				numFramesToAvg = binaryReader.ReadInt32();
				binaryReader.Close();
				fileStream.Close();
			}
			catch
			{
				return false;
			}
			dataSet = true;
			return true;
		}

		public bool setDemoSettings(HSIImage image, Form1 form)
		{
			if (modus != 0 || !dataSet)
			{
				return false;
			}
			form.exp_time = exp_time_image;
			form.filterRangeStart = filterRangeStart;
			form.filterRangeEnd = filterRangeEnd;
			form.maximizeMode = maximizeMode;
			form.statusParallelProcessing = statusParallelProcessing;
			form.compareSpectraMode = compareSpectraMode;
			form.pictureStretchMode = pictureStretchMode;
			form.derivationMode = derivationMode;
			form.distanceMode = distanceMode;
			form.resolutionMode = resolutionMode;
			form.imageViewMode = imageViewMode;
			form.numMaterials = numMaterialsToUse;
			form.numFramesToAvg = numFramesToAvg;
			image.exp_time_white = exp_time_white;
			image.measured = measured;
			image.darkMeasured = darkMeasured;
			image.whiteMeasured = whiteMeasured;
			image.numMaterialsToUse = numMaterialsToUse;
			for (int i = 0; i < sensor_height; i++)
			{
				for (int j = 0; j < sensor_width; j++)
				{
					int[,] array = image.darkImageRAW;
					int num = i;
					int num2 = j;
					int num3 = darkImageRAW[i, j];
					array[num, num2] = num3;
					int[,] array2 = image.whiteImageRAW;
					int num4 = i;
					int num5 = j;
					int num6 = whiteImageRAW[i, j];
					array2[num4, num5] = num6;
				}
			}
			for (int k = 0; k < MAX_MATERIALS + 1; k++)
			{
				for (int l = 0; l < bands; l++)
				{
					double[,] array3 = image.spectrum;
					int num7 = k;
					int num8 = l;
					double num9 = spectrum[k, l];
					array3[num7, num8] = num9;
				}
			}
			return true;
		}
	}
}
