#region netDxf library licensed under the MIT License
//
//                       netDxf library
// Copyright (c) Daniel Carvajal (haplokuon@gmail.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
#endregion

using System;

namespace netDxf.Objects
{
	/// <summary>Represents the plot settings of a layout.</summary>
	public class PlotSettings :
		ICloneable
	{
		#region constructors

		/// <summary>6Initializes a new instance of <see cref="PlotSettings"/>.</summary>
		public PlotSettings()
		{
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets the page setup name.</summary>
		public string PageSetupName { get; set; } = string.Empty;

		/// <summary>Gets or sets the name of system printer or plot configuration file.</summary>
		public string PlotterName { get; set; } = "none_device";

		/// <summary>Gets or set the paper size name.</summary>
		public string PaperSizeName { get; set; } = "ISO_A4_(210.00_x_297.00_MM)";

		/// <summary>Gets or sets the plot view name.</summary>
		public string ViewName { get; set; } = string.Empty;

		/// <summary>Gets or sets the current style sheet name.</summary>
		public string CurrentStyleSheet { get; set; } = string.Empty;

		/// <summary>Gets or set the size, in millimeters, of unprintable margins of paper.</summary>
		public PaperMargin PaperMargin { get; set; } = new PaperMargin(7.5, 20.0, 7.5, 20.0);

		/// <summary>Gets or sets the plot paper size: physical paper width and height in millimeters.</summary>
		public Vector2 PaperSize { get; set; } = new Vector2(210.0, 297.0);

		/// <summary>Gets or sets the plot origin in millimeters.</summary>
		public Vector2 Origin { get; set; } = Vector2.Zero;

		/// <summary>Gets or sets the plot upper-right window corner.</summary>
		public Vector2 WindowUpRight { get; set; } = Vector2.Zero;

		/// <summary>Gets or sets the plot lower-left window corner.</summary>
		public Vector2 WindowBottomLeft { get; set; } = Vector2.Zero;

		/// <summary>Gets or sets if the plot scale will be automatically computed show the drawing fits the media.</summary>
		/// <remarks>
		/// If <see cref="ScaleToFit"/> is set to <see langword="false"/> the values specified by <see cref="PrintScaleNumerator"/> and <see cref="PrintScaleDenominator"/> will be used.
		/// </remarks>
		public bool ScaleToFit { get; set; } = true;

		private double _PrintScaleNumerator = 1.0;
		/// <summary>Gets or sets the numerator of custom print scale: real world paper units.</summary>
		/// <remarks>
		/// The paper units used are specified by the <see cref="PaperUnits"/> value.
		/// </remarks>
		public double PrintScaleNumerator
		{
			get => _PrintScaleNumerator;
			set
			{
				if (value <= 0.0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The print scale numerator must be a number greater than zero.");
				}
				_PrintScaleNumerator = value;
			}
		}

		private double _PrintScaleDenominator = 1.0;
		/// <summary>Gets or sets the denominator of custom print scale: drawing units.</summary>
		public double PrintScaleDenominator
		{
			get => _PrintScaleDenominator;
			set
			{
				if (value <= 0.0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The print scale denominator must be a number greater than zero.");
				}
				_PrintScaleDenominator = value;
			}
		}

		/// <summary>Gets the scale factor.</summary>
		public double PrintScale => PrintScaleNumerator / _PrintScaleDenominator;

		/// <summary>Gets or sets the plot layout flags.</summary>
		public PlotFlags Flags { get; set; }
			= PlotFlags.DrawViewportsFirst
			| PlotFlags.PrintLineweights
			| PlotFlags.PlotPlotStyles
			| PlotFlags.UseStandardScale;

		/// <summary>Gets or sets the portion of paper space to output to the media.</summary>
		public PlotType PlotType { get; set; } = PlotType.DrawingExtents;

		/// <summary>Gets or sets the paper units.</summary>
		/// <remarks>This value is only applicable to the scale parameter <see cref="PrintScaleNumerator"/>.</remarks>
		public PlotPaperUnits PaperUnits { get; set; } = PlotPaperUnits.Milimeters;

		/// <summary>Gets or sets the paper rotation.</summary>
		public PlotRotation PaperRotation { get; set; } = PlotRotation.Degrees90;

		/// <summary>Gets or sets the shade plot mode.</summary>
		public ShadePlotMode ShadePlotMode { get; set; } = ShadePlotMode.AsDisplayed;

		/// <summary>Gets or sets the plot resolution mode.</summary>
		/// <remarks>
		/// if the <see cref="ShadePlotResolutionMode"/> is set to Custom the value specified by the <see cref="ShadePlotDPI"/> will be used.
		/// </remarks>
		public ShadePlotResolutionMode ShadePlotResolutionMode { get; set; } = ShadePlotResolutionMode.Normal;

		private short _ShadePlotDPI = 300;
		/// <summary>Gets or sets the shade plot custom DPI.</summary>
		public short ShadePlotDPI
		{
			get => _ShadePlotDPI;
			set
			{
				if (value < 100)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The valid shade plot DPI values range from 100 to the maximum value allowed by the plot device.");
				}
				_ShadePlotDPI = value;
			}
		}

		/// <summary>Gets or sets the paper image origin.</summary>
		public Vector2 PaperImageOrigin { get; set; } = Vector2.Zero;

		#endregion

		#region implements ICloneable

		/// <inheritdoc/>
		public object Clone()
			=> new PlotSettings
			{
				PageSetupName = this.PageSetupName,
				PlotterName = this.PlotterName,
				PaperSizeName = this.PaperSizeName,
				ViewName = this.ViewName,
				CurrentStyleSheet = this.CurrentStyleSheet,
				PaperMargin = this.PaperMargin,
				PaperSize = this.PaperSize,
				Origin = this.Origin,
				WindowUpRight = this.WindowUpRight,
				WindowBottomLeft = this.WindowBottomLeft,
				ScaleToFit = this.ScaleToFit,
				PrintScaleNumerator = _PrintScaleNumerator,
				PrintScaleDenominator = _PrintScaleDenominator,
				Flags = this.Flags,
				PlotType = this.PlotType,
				PaperUnits = this.PaperUnits,
				PaperRotation = this.PaperRotation,
				ShadePlotMode = this.ShadePlotMode,
				ShadePlotResolutionMode = this.ShadePlotResolutionMode,
				ShadePlotDPI = this.ShadePlotDPI,
				PaperImageOrigin = this.PaperImageOrigin
			};

		#endregion
	}
}