﻿using System;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.LayerMixingFilter.LumaKey
{
	public partial class LumaKeySetup : BaseForm
	{
		private int _lowerLimit { get; set; }
		private int _upperLimit { get; set; }

		public LumaKeySetup(double lowerLimit, double upperLimit)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			_lowerLimit = (int)(lowerLimit * 100);
			_upperLimit = (int)(upperLimit * 100);
			UpdateLimitControls();
		}

		public double LowerLimit { get { return _lowerLimit / 100d; } }
		public double UpperLimit { get { return _upperLimit / 100d; } }

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;
		}

		private void trkLowerLimit_Scroll(object sender, EventArgs e)
		{
			ValidateLower(trkLowerLimit.Value);
		}

		private void trkUpperLimit_Scroll(object sender, EventArgs e)
		{
			ValidateUpper(trkUpperLimit.Value);
		}

		private void numLowerLimit_LostFocus(object sender, EventArgs e)
		{
			//ValidateLower(numLowerLimit.IntValue);
		}

		private void numUpperLimit_LostFocus(object sender, EventArgs e)
		{
			//ValidateUpper(numUpperLimit.IntValue);
		}

		private void ValidateLower(int v)
		{
			_lowerLimit = v <= _upperLimit ? v : _upperLimit;
			UpdateLimitControls();
		}

		private void ValidateUpper(int v)
		{
			_upperLimit = v >= _lowerLimit ? v : _lowerLimit;
			//_upperLimit = v > 100 ? 100 : v;
			UpdateLimitControls();
		}

		private void UpdateLimitControls()
		{
			trkLowerLimit.Value = _lowerLimit;
			trkUpperLimit.Value = _upperLimit;
			numLowerLimit.Text = _lowerLimit.ToString();
			numUpperLimit.Text = _upperLimit.ToString();
		}

		private void numUpperLimit_TextChanged(object sender, EventArgs e)
		{
			ValidateUpper(numUpperLimit.IntValue);
			Console.Out.WriteLine("numUpperLimit.Value =" + numUpperLimit.IntValue);
		}

		private void numLowerLimit_TextChanged(object sender, EventArgs e)
		{
			ValidateLower(numLowerLimit.IntValue);
			Console.Out.WriteLine("numLowerLimit.Value =" + numLowerLimit.IntValue);
		}
	}
}