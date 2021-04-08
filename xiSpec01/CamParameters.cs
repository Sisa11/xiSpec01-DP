namespace xiSpec01
{
	internal class CamParameters
	{
		public int index_cam;

		public int exp_change_mode = 1;

		public int output_bits = 8;

		public int exposure_time = 100;

		public bool use_autobandwidth = true;

		public bool use_packed_mode = true;

		public CamParameters(int index)
		{
			index_cam = index;
		}
	}
}
