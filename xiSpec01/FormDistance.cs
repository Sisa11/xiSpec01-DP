using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace xiSpec01
{
	public class FormDistance : Form
	{
		private IContainer components;

		private Label label1;

		private TextBox textBox1;

		private TextBox textBox2;

		private Label label2;

		private TextBox textBox3;

		private Label label3;

		private TextBox textBox4;

		private Label label4;

		public FormDistance()
		{
			InitializeComponent();
		}

		public void showValues(int d0, int d1, int d2, int d3)
		{
			if (d0 < 0)
			{
				textBox1.Text = "";
			}
			else
			{
				textBox1.Text = d0.ToString();
			}
			if (d1 < 0)
			{
				textBox2.Text = "";
			}
			else
			{
				textBox2.Text = d1.ToString();
			}
			if (d2 < 0)
			{
				textBox3.Text = "";
			}
			else
			{
				textBox3.Text = d2.ToString();
			}
			if (d3 < 0)
			{
				textBox4.Text = "";
			}
			else
			{
				textBox4.Text = d3.ToString();
			}
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
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(xiSpec01.FormDistance));
			label1 = new System.Windows.Forms.Label();
			textBox1 = new System.Windows.Forms.TextBox();
			textBox2 = new System.Windows.Forms.TextBox();
			label2 = new System.Windows.Forms.Label();
			textBox3 = new System.Windows.Forms.TextBox();
			label3 = new System.Windows.Forms.Label();
			textBox4 = new System.Windows.Forms.TextBox();
			label4 = new System.Windows.Forms.Label();
			SuspendLayout();
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(12, 29);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(25, 13);
			label1.TabIndex = 0;
			label1.Text = "d0=";
			textBox1.Location = new System.Drawing.Point(37, 26);
			textBox1.Name = "textBox1";
			textBox1.Size = new System.Drawing.Size(44, 20);
			textBox1.TabIndex = 1;
			textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			textBox2.Location = new System.Drawing.Point(112, 26);
			textBox2.Name = "textBox2";
			textBox2.Size = new System.Drawing.Size(50, 20);
			textBox2.TabIndex = 3;
			textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(87, 29);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(25, 13);
			label2.TabIndex = 2;
			label2.Text = "d1=";
			textBox3.Location = new System.Drawing.Point(193, 26);
			textBox3.Name = "textBox3";
			textBox3.Size = new System.Drawing.Size(51, 20);
			textBox3.TabIndex = 5;
			textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(168, 29);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(25, 13);
			label3.TabIndex = 4;
			label3.Text = "d2=";
			textBox4.Location = new System.Drawing.Point(275, 26);
			textBox4.Name = "textBox4";
			textBox4.Size = new System.Drawing.Size(49, 20);
			textBox4.TabIndex = 7;
			textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(250, 29);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(25, 13);
			label4.TabIndex = 6;
			label4.Text = "d3=";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(345, 68);
			base.Controls.Add(textBox4);
			base.Controls.Add(label4);
			base.Controls.Add(textBox3);
			base.Controls.Add(label3);
			base.Controls.Add(textBox2);
			base.Controls.Add(label2);
			base.Controls.Add(textBox1);
			base.Controls.Add(label1);
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.Name = "FormDistance";
			Text = "FormDistance";
			ResumeLayout(performLayout: false);
			PerformLayout();
		}
	}
}
