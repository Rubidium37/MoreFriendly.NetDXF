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
		#region private fields

		private double numeratorScale;
		private double denominatorScale;

		private short shadePlotDPI;

		#endregion

		#region constructors

		/// <summary>6Initializes a new instance of <see cref="PlotSettings"/>.</summary>
		public PlotSettings()
		{
			this.PageSetupName = string.Empty;
			this.PlotterName = "none_device";
			this.PaperSizeName = "ISO_A4_(210.00_x_297.00_MM)";
			this.ViewName = string.Empty;
			this.CurrentStyleSheet = string.Empty;

			this.PaperMargin = new PaperMargin(7.5, 20.0, 7.5, 20.0);

			this.PaperSize = new Vector2(210.0, 297.0);
			this.Origin = Vector2.Zero;
			this.WindowUpRight = Vector2.Zero;
			this.WindowBottomLeft = Vector2.Zero;

			this.ScaleToFit = true;
			this.numeratorScale = 1.0;
			this.denominatorScale = 1.0;
			this.Flags = PlotFlags.DrawViewportsFirst | PlotFlags.PrintLineweights | PlotFlags.PlotPlotStyles | PlotFlags.UseStandardScale;
			this.PlotType = PlotType.DrawingExtents;

			this.PaperUnits = PlotPaperUnits.Milimeters;
			this.PaperRotation = PlotRotation.Degrees90;

			this.ShadePlotMode = ShadePlotMode.AsDisplayed;
			this.ShadePlotResolutionMode = ShadePlotResolutionMode.Normal;
			this.shadePlotDPI = 300;
			this.PaperImageOrigin = Vector2.Zero;
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets the page setup name.</summary>
		public string PageSetupName { get; set; }

		/// <summary>Gets or sets the name of system printer or plot configuration file.</summary>
		public string PlotterName { get; set; }

		/// <summary>Gets or set the paper size name.</summary>
		public string PaperSizeName { get; set; }

		/// <summary>Gets or sets the plot view name.</summary>
		public string ViewName { get; set; }

		/// <summary>Gets or sets the current style sheet name.</summary>
		public string CurrentStyleSheet { get; set; }

		/// <summary>Gets or set the size, in millimeters, of unprintable margins of paper.</summary>
		public PaperMargin PaperMargin { get; set; }

		/// <summary>Gets or sets the plot paper size: physical paper width and height in millimeters.</summary>
		public Vector2 PaperSize { get; set; }

		/// <summary>Gets or sets the plot origin in millimeters.</summary>
		public Vector2 Origin { get; set; }

		/// <summary>Gets or sets the plot upper-right window corner.</summary>
		public Vector2 WindowUpRight { get; set; }

		/// <summary>Gets or sets the plot lower-left window corner.</summary>
		public Vector2 WindowBottomLeft { get; set; }

		/// <summary>Gets or sets if the plot scale will be automatically computed show the drawing fits the media.</summary>
		/// <remarks>
		/// If <see cref="ScaleToFit"/> is set to <see langword="false"/> the values specified by <see cref="PrintScaleNumerator"/> and <see cref="PrintScaleDenominator"/> will be used.
		/// </remarks>
		public bool ScaleToFit { get; set; }

		/// <summary>Gets or sets the numerator of custom print scale: real world paper units.</summary>
		/// <remarks>
		/// The paper units used are specified by the <see cref="PaperUnits"/> value.
		/// </remarks>
		public double PrintScaleNumerator
		{
			get => this.numeratorScale;
			set
			{
				if (value <= 0.0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The print scale numerator must be a number greater than zero.");
				}
				this.numeratorScale = value;
			}
		}

		/// <summary>Gets or sets the denominator of custom print scale: drawing units.</summary>
		public double PrintScaleDenominator
		{
			get => this.denominatorScale;
			set
			{
				if (value <= 0.0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The print scale denominator must be a number greater than zero.");
				}
				this.denominatorScale = value;
			}
		}

		/// <summary>Gets the scale factor.</summary>
		public double PrintScale => this.numeratorScale / this.denominatorScale;

		/// <summary>Gets or sets the plot layout flags.</summary>
		public PlotFlags Flags { get; set; }

		/// <summary>Gets or sets the portion of paper space to output to the media.</summary>
		public PlotType PlotType { get; set; }

		/// <summary>Gets or sets the paper units.</summary>
		/// <remarks>This value is only applicable to the scale parameter <see cref="PrintScaleNumerator"/>.</remarks>
		public PlotPaperUnits PaperUnits { get; set; }

		/// <summary>Gets or sets the paper rotation.</summary>
		public PlotRotation PaperRotation { get; set; }

		/// <summary>Gets or sets the shade plot mode.</summary>
		public ShadePlotMode ShadePlotMode { get; set; }

		/// <summary>Gets or sets the plot resolution mode.</summary>
		/// <remarks>
		/// if the <see cref="ShadePlotResolutionMode"/> is set to Custom the value specified by the <see cref="ShadePlotDPI"/> will be used.
		/// </remarks>
		public ShadePlotResolutionMode ShadePlotResolutionMode { get; set; }

		/// <summary>Gets or sets the shade plot custom DPI.</summary>
		public short ShadePlotDPI
		{
			get => this.shadePlotDPI;
			set
			{
				if (value < 100)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The valid shade plot DPI values range from 100 to the maximum value allowed by the plot device.");
				}
				this.shadePlotDPI = value;
			}
		}

		/// <summary>Gets or sets the paper image origin.</summary>
		public Vector2 PaperImageOrigin { get; set; }

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
				PrintScaleNumerator = this.numeratorScale,
				PrintScaleDenominator = this.denominatorScale,
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