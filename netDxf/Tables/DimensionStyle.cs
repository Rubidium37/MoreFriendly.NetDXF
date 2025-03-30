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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using netDxf.Blocks;
using netDxf.Collections;
using netDxf.Units;

namespace netDxf.Tables
{
	/// <summary>Represents a dimension style.</summary>
	public class DimensionStyle :
		TableObject
	{
		#region delegates and events

		/// <summary>Generated when a property of <see cref="Linetype"/> type changes.</summary>
		public event BeforeValueChangeEventHandler<Linetype> BeforeChangingLinetypeValue;
		/// <summary>Generates the <see cref="BeforeChangingLinetypeValue"/> event.</summary>
		/// <param name="oldValue">The old value, being changed.</param>
		/// <param name="newValue">The new value, that will replace the old one.</param>
		/// <param name="propertyName">(automatic) Name of the affected property.</param>
		protected virtual Linetype OnBeforeChangingLinetypeValue(Linetype oldValue, Linetype newValue, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeChangingLinetypeValue is { } handler)
			{
				var e = new BeforeValueChangeEventArgs<Linetype>(propertyName, oldValue, newValue);
				handler(this, e);
				return e.NewValue;
			}
			return newValue;
		}

		/// <summary>Generated when a property of <see cref="TextStyle"/> type changes.</summary>
		public event BeforeValueChangeEventHandler<TextStyle> BeforeChangingTextStyleValue;
		/// <summary>Generates the <see cref="BeforeChangingTextStyleValue"/> event.</summary>
		/// <param name="oldValue">The old value, being changed.</param>
		/// <param name="newValue">The new value, that will replace the old one.</param>
		/// <param name="propertyName">(automatic) Name of the affected property.</param>
		protected virtual TextStyle OnBeforeChangingBeforeValueTextStyle(TextStyle oldValue, TextStyle newValue, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeChangingTextStyleValue is { } handler)
			{
				var e = new BeforeValueChangeEventArgs<TextStyle>(propertyName, oldValue, newValue);
				handler(this, e);
				return e.NewValue;
			}
			return newValue;
		}

		/// <summary>Generated when a property of <see cref="Block"/> type changes.</summary>
		public event BeforeValueChangeEventHandler<Block> BeforeChangingBlockValue;
		/// <summary>Generates the <see cref="BeforeChangingBlockValue"/> event.</summary>
		/// <param name="oldValue">The old value, being changed.</param>
		/// <param name="newValue">The new value, that will replace the old one.</param>
		/// <param name="propertyName">(automatic) Name of the affected property.</param>
		protected virtual Block OnBeforeChangingBlockValue(Block oldValue, Block newValue, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeChangingBlockValue is { } handler)
			{
				var e = new BeforeValueChangeEventArgs<Block>(propertyName, oldValue, newValue);
				handler(this, e);
				return e.NewValue;
			}
			return newValue;
		}

		#endregion

		#region constants

		/// <summary>Default dimension style name.</summary>
		public const string DefaultName = "Standard";

		/// <summary>Gets the default dimension style.</summary>
		public static DimensionStyle Default => new DimensionStyle(DefaultName);

		/// <summary>Gets the ISO-25 dimension style as defined in AutoCad.</summary>
		public static DimensionStyle Iso25
		{
			get
			{
				DimensionStyle style = new DimensionStyle("ISO-25")
				{
					DimBaselineSpacing = 3.75,
					ExtLineExtend = 1.25,
					ExtLineOffset = 0.625,
					ArrowSize = 2.5,
					CenterMarkSize = 2.5,
					TextHeight = 2.5,
					TextOffset = 0.625,
					TextOutsideAlign = true,
					TextInsideAlign = true,
					TextVerticalPlacement = DimensionStyleTextVerticalPlacement.Above,
					FitDimLineForce = true,
					DecimalSeparator = ',',
					LengthPrecision = 2,
					SuppressLinearTrailingZeros = true,
					AlternateUnits =
					{
						LengthPrecision = 3,
						Multiplier = 0.0394
					},
					Tolerances =
					{
						VerticalPlacement = DimensionStyleTolerancesVerticalPlacement.Bottom,
						Precision = 2,
						SuppressLinearTrailingZeros = true,
						AlternatePrecision = 3
					}
				};
				return style;
			}
		}

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">The dimension style name.</param>
		public DimensionStyle(string name)
			: this(name, true)
		{
		}
		internal DimensionStyle(string name, bool checkName)
			: base(name, DxfObjectCode.DimStyle, checkName)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name), "The dimension style name should be at least one character long.");
			}

			this.IsReserved = name.Equals(DefaultName, StringComparison.OrdinalIgnoreCase);
		}

		#endregion

		#region public properties

		#region dimension and extension lines

		private AciColor _DimLineColor = AciColor.ByBlock;
		/// <summary>Gets or set the color assigned to dimension lines, arrowheads, and dimension leader lines. (<b>DIMCLRD</b>)</summary>
		/// <remarks>
		/// Default: ByBlock<br />
		/// Only indexed <see cref="AciColor"/>s are supported.
		/// </remarks>
		public AciColor DimLineColor
		{
			get => _DimLineColor;
			set => _DimLineColor = value ?? throw new ArgumentNullException(nameof(value));
		}

		private Linetype _DimLineLinetype = Linetype.ByBlock;
		/// <summary>Gets or sets the line type of the dimension line. (<b>DIMLTYPE</b>)</summary>
		/// <remarks>
		/// Default: ByBlock
		/// </remarks>
		public Linetype DimLineLinetype
		{
			get => _DimLineLinetype;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_DimLineLinetype = this.OnBeforeChangingLinetypeValue(_DimLineLinetype, value);
			}
		}

		/// <summary>Gets or sets the line weight to dimension lines. (<b>DIMLWD</b>)</summary>
		/// <remarks>
		/// Default: ByBlock
		/// </remarks>
		public Lineweight DimLineLineweight { get; set; } = Lineweight.ByBlock;

		/// <summary>Suppresses display of the first dimension line. (DIMSD1)</summary>
		/// <remarks>
		/// Default: <see langword="false"/><br />
		/// To completely suppress the dimension line set both <see cref="DimLine1Off"/> and <see cref="DimLine2Off"/> to <see langword="false"/>.
		/// </remarks>
		public bool DimLine1Off { get; set; } = false;

		/// <summary>Suppresses display of the second dimension line. (DIMSD2)</summary>
		/// <remarks>
		/// Default: <see langword="false"/><br />
		/// To completely suppress the dimension line set both <see cref="DimLine1Off"/> and <see cref="DimLine2Off"/> to <see langword="false"/>.
		/// </remarks>
		public bool DimLine2Off { get; set; } = false;

		private double _DimLineExtend = 0.0;
		/// <summary>
		/// Gets or sets the distance the dimension line extends beyond the extension line when
		/// oblique, architectural tick, integral, or no marks are drawn for arrowheads. (<b>DIMDLE</b>)
		/// </summary>
		/// <remarks>
		/// Default: 0.0
		/// </remarks>
		public double DimLineExtend
		{
			get => _DimLineExtend;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The DimLineExtend must be equals or greater than zero.");
				}
				_DimLineExtend = value;
			}
		}

		private double _DimBaselineSpacing = 0.38;
		/// <summary>Gets or sets the spacing of the dimension lines in baseline dimensions. (<b>DIMDLI</b>)</summary>
		/// <remarks>
		/// Default: 0.38<br />
		/// This value is stored only for information purposes.
		/// Base dimensions are a compound entity made of several dimensions, there is no actual <b>DXF</b> entity that represents that.
		/// </remarks>
		public double DimBaselineSpacing
		{
			get => _DimBaselineSpacing;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The DimBaselineSpacing must be equals or greater than zero.");
				}
				_DimBaselineSpacing = value;
			}
		}

		private AciColor _ExtLineColor = AciColor.ByBlock;
		/// <summary>Gets or sets the color assigned to extension lines, center marks, and centerlines. (<b>DIMCLRE</b>)</summary>
		/// <remarks>
		/// Default: ByBlock<br />
		/// Only indexed <see cref="AciColor"/>s are supported.
		/// </remarks>
		public AciColor ExtLineColor
		{
			get => _ExtLineColor;
			set => _ExtLineColor = value ?? throw new ArgumentNullException(nameof(value));
		}

		private Linetype _ExtLine1Linetype = Linetype.ByBlock;
		/// <summary>Gets or sets the line type of the first extension line. (DIMLTEX1)</summary>
		/// <remarks>
		/// Default: ByBlock
		/// </remarks>
		public Linetype ExtLine1Linetype
		{
			get => _ExtLine1Linetype;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_ExtLine1Linetype = this.OnBeforeChangingLinetypeValue(_ExtLine1Linetype, value);
			}
		}

		private Linetype _ExtLine2Linetype = Linetype.ByBlock;
		/// <summary>Gets or sets the line type of the second extension line. (DIMLTEX2)</summary>
		/// <remarks>
		/// Default: ByBlock
		/// </remarks>
		public Linetype ExtLine2Linetype
		{
			get => _ExtLine2Linetype;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_ExtLine2Linetype = this.OnBeforeChangingLinetypeValue(_ExtLine2Linetype, value);
			}
		}

		/// <summary>Gets or sets line weight of extension lines. (<b>DIMLWE</b>)</summary>
		/// <remarks>
		/// Default: ByBlock
		/// </remarks>
		public Lineweight ExtLineLineweight { get; set; } = Lineweight.ByBlock;

		/// <summary>Suppresses display of the first extension line. (DIMSE1)</summary>
		/// <remarks>
		/// Default: <see langword="false"/>
		/// </remarks>
		public bool ExtLine1Off { get; set; } = false;

		/// <summary>Suppresses display of the second extension line. (DIMSE2)</summary>
		/// <remarks>
		/// Default: <see langword="false"/>
		/// </remarks>
		public bool ExtLine2Off { get; set; } = false;

		private double _ExtLineOffset = 0.0625;
		/// <summary>Gets or sets how far extension lines are offset from origin points. (<b>DIMEXO</b>)</summary>
		/// <remarks>
		/// Default: 0.0625
		/// </remarks>
		public double ExtLineOffset
		{
			get => _ExtLineOffset;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The ExtLineOffset must be equals or greater than zero.");
				}
				_ExtLineOffset = value;
			}
		}

		private double _ExtLineExtend = 0.18;
		/// <summary>Gets or sets how far to extend the extension line beyond the dimension line. (<b>DIMEXE</b>)</summary>
		/// <remarks>
		/// Default: 0.18
		/// </remarks>
		public double ExtLineExtend
		{
			get => _ExtLineExtend;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The ExtLineExtend must be equals or greater than zero.");
				}
				_ExtLineExtend = value;
			}
		}

		/// <summary>Enables fixed length extension lines. (<b>DIMFXLON</b>)</summary>
		/// <remarks>
		/// Default: <see langword="false"/>
		/// </remarks>
		public bool ExtLineFixed { get; set; } = false;

		private double _ExtLineFixedLength = 1.0;
		/// <summary>Gets or sets the total length of the extension lines starting from the dimension line toward the dimension origin. (<b>DIMFXL</b>)</summary>
		/// <remarks>
		/// Default: 1.0
		/// </remarks>
		public double ExtLineFixedLength
		{
			get => _ExtLineFixedLength;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The ExtLineFixedLength must be equals or greater than zero.");
				}
				_ExtLineFixedLength = value;
			}
		}

		#endregion

		#region symbols and arrows

		private Block _DimArrow1;
		/// <summary>Gets or sets the arrowhead block for the first end of the dimension line. (DIMBLK1)</summary>
		/// <remarks>
		/// Default: <see langword="null"/>. Closed filled.
		/// </remarks>
		public Block DimArrow1
		{
			get => _DimArrow1;
			set => _DimArrow1 = value == null ? null : this.OnBeforeChangingBlockValue(_DimArrow1, value);
		}

		private Block _DimArrow2;
		/// <summary>Gets or sets the arrowhead block for the second end of the dimension line. (DIMBLK2)</summary>
		/// <remarks>
		/// Default: <see langword="null"/>. Closed filled.
		/// </remarks>
		public Block DimArrow2
		{
			get => _DimArrow2;
			set => _DimArrow2 = value == null ? null : this.OnBeforeChangingBlockValue(_DimArrow2, value);
		}

		private Block _LeaderArrow;
		/// <summary>Gets or sets the arrowhead block for leaders. (<b>DIMLDRBLK</b>)</summary>
		/// <remarks>
		/// Default: <see langword="null"/>. Closed filled.
		/// </remarks>
		public Block LeaderArrow
		{
			get => _LeaderArrow;
			set => _LeaderArrow = value == null ? null : this.OnBeforeChangingBlockValue(_LeaderArrow, value);
		}

		private double _ArrowSize = 0.18;
		/// <summary>Controls the size of dimension line and leader line arrowheads. Also controls the size of hook lines. (<b>DIMASZ</b>)</summary>
		/// <remarks>
		/// Default: 0.18
		/// </remarks>
		public double ArrowSize
		{
			get => _ArrowSize;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The ArrowSize must be equals or greater than zero.");
				}
				_ArrowSize = value;
			}
		}

		/// <summary>Controls the drawing of circle or arc center marks and centerlines. (<b>DIMCEN</b>)</summary>
		/// <remarks>
		/// Default: 0.09<br/>
		/// 0 - No center marks or lines are drawn.<br />
		/// greater than 0 - Center marks are drawn.<br />
		/// lower than 0 - Center marks and centerlines are drawn.<br />
		/// The absolute value specifies the size of the center mark or center line.
		/// The size of the center line is the length of the center line segment that extends outside the circle or arc.
		/// It is also the size of the gap between the center mark and the start of the center line.
		/// The size of the center mark is the distance from the center of the circle or arc to the end of the center mark.
		/// </remarks>
		public double CenterMarkSize { get; set; } = 0.09;

		#endregion

		#region text appearance

		private TextStyle _TextStyle = TextStyle.Default;
		/// <summary>Gets or sets the text style of the dimension. (<b>DIMTXTSTY</b>)</summary>
		/// <remarks>
		/// Default: Standard
		/// </remarks>
		public TextStyle TextStyle
		{
			get => _TextStyle;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_TextStyle = this.OnBeforeChangingBeforeValueTextStyle(_TextStyle, value);
			}
		}

		private AciColor _TextColor = AciColor.ByBlock;
		/// <summary>Gets or set the color of dimension text. (<b>DIMCLRT</b>)</summary>
		/// <remarks>
		/// Default: ByBlock<br />
		/// Only indexed <see cref="AciColor"/>s are supported.
		/// </remarks>
		public AciColor TextColor
		{
			get => _TextColor;
			set => _TextColor = value ?? throw new ArgumentNullException(nameof(value));
		}

		/// <summary>Gets or set the background color of dimension text. Set to <see langword="null"/> to specify no color. (<b>DIMTFILLCLR</b>)</summary>
		/// <remarks>
		/// Default: <see langword="null"/><br />
		/// Only indexed <see cref="AciColor"/>s are supported.
		/// </remarks>
		public AciColor TextFillColor { get; set; }

		private double _TextHeight = 0.18;
		/// <summary>Gets or sets the height of dimension text, unless the current text style has a fixed height. (<b>DIMTXT</b>)</summary>
		/// <remarks>
		/// Default: 0.18
		/// </remarks>
		public double TextHeight
		{
			get => _TextHeight;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The TextHeight must be greater than zero.");
				}
				_TextHeight = value;
			}
		}

		/// <summary>Gets or sets the horizontal positioning of dimension text. (<b>DIMJUST</b>)</summary>
		/// <remarks>
		/// Default: Centered
		/// </remarks>
		public DimensionStyleTextHorizontalPlacement TextHorizontalPlacement { get; set; } = DimensionStyleTextHorizontalPlacement.Centered;

		/// <summary>Gets or sets the vertical position of text in relation to the dimension line. (<b>DIMTAD</b>)</summary>
		/// <remarks>
		/// Default: Centered
		/// </remarks>
		public DimensionStyleTextVerticalPlacement TextVerticalPlacement { get; set; } = DimensionStyleTextVerticalPlacement.Centered;

		/// <summary>Gets or sets the distance around the dimension text when the dimension line breaks to accommodate dimension text. (<b>DIMGAP</b>)</summary>
		/// <remarks>
		/// Default: 0.09<br />
		/// Displays a rectangular frame around the dimension text when negative values are used.
		/// </remarks>
		public double TextOffset { get; set; } = 0.09;

		/// <summary>Gets or sets if the dimension text is placed horizontally when inside extension lines. (<b>DIMTIH</b>)</summary>
		/// <remarks>
		/// Default: <see langword="false"/>
		/// </remarks>
		public bool TextInsideAlign { get; set; } = false;

		/// <summary>Gets or sets if the dimension text is placed horizontally when outside extension lines. (<b>DIMTOH</b>)</summary>
		/// <remarks>
		/// Default: <see langword="false"/>
		/// </remarks>
		public bool TextOutsideAlign { get; set; } = false;

		/// <summary>Gets or sets the direction of the dimension text. (<b>DIMTXTDIRECTION</b>)</summary>
		/// <remarks>
		/// Default: LeftToRight
		/// </remarks>
		public DimensionStyleTextDirection TextDirection { get; set; } = DimensionStyleTextDirection.LeftToRight;

		private double _TextFractionHeightScale = 1.0;
		/// <summary>Gets or sets the scale of fractions relative to dimension text height. (<b>DIMTFAC</b>)</summary>
		/// <remarks>
		/// Default: 1.0<br />
		/// This value is only applicable to <see cref="LinearUnitType.Architectural"/> and <see cref="LinearUnitType.Fractional"/> units, and also
		/// controls the height factor applied to the tolerance text in relation with the dimension text height.
		/// </remarks>
		public double TextFractionHeightScale
		{
			get => _TextFractionHeightScale;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The TextFractionHeightScale must be greater than zero.");
				}
				_TextFractionHeightScale = value;
			}
		}

		#endregion

		#region fit

		/// <summary>Gets or sets the drawing of a dimension line between the extension lines even when the text is placed outside the extension lines. (<b>DIMTOFL</b>)</summary>
		/// <remarks>
		/// Default: <see langword="false"/>
		/// </remarks>
		public bool FitDimLineForce { get; set; } = false;

		/// <summary>Gets or sets the drawing of the dimension line and arrowheads even if not enough space is available inside the extension lines. (<b>DIMSOXD</b>)</summary>
		/// <remarks>
		/// Default: <see langword="true"/><br />
		/// If not enough space is available inside the extension lines and <see cref="FitTextInside"/> is <see langword="true"/>,
		/// setting <see cref="FitDimLineInside"/> to false suppresses the arrowheads. If <see cref="FitDimLineInside"/> is <see langword="false"/>,
		/// <see cref="FitDimLineInside"/> has no effect.
		/// </remarks>
		public bool FitDimLineInside { get; set; } = true;

		private double _DimScaleOverall = 1.0;
		/// <summary>Get or set the overall scale factor applied to dimensioning variables that specify sizes, distances, or offsets. (<b>DIMSCALE</b>)</summary>
		/// <remarks>
		/// Default: 1.0<br/>
		/// <b>DIMSCALE</b> does not affect measured lengths, coordinates, or angles.<br/>
		/// <b>DIMSCALE</b> values of zero are not supported, any imported drawing with a zero value will set the scale to the default 1.0.
		/// </remarks>
		public double DimScaleOverall
		{
			get => _DimScaleOverall;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The DimScaleOverall must be greater than zero.");
				}
				_DimScaleOverall = value;
			}
		}

		/// <summary>Gets or sets the placement of text and arrowheads based on the space available between the extension lines. (<b>DIMATFIT</b>)</summary>
		/// <remarks>
		/// Default: BestFit<br/>
		/// Not implemented in the dimension drawing.
		/// </remarks>
		public DimensionStyleFitOptions FitOptions { get; set; } = DimensionStyleFitOptions.BestFit;

		/// <summary>Gets or sets the drawing of text between the extension lines. (<b>DIMTIX</b>)</summary>
		/// <remarks>
		/// Default: <see langword="false"/>
		/// </remarks>
		public bool FitTextInside { get; set; } = false;

		/// <summary>Gets or sets the position of the text when it's moved either manually or automatically. (<b>DIMTMOVE</b>)</summary>
		/// <remarks>
		/// Default: BesideDimLine
		/// </remarks>
		public DimensionStyleFitTextMove FitTextMove { get; set; } = DimensionStyleFitTextMove.BesideDimLine;

		#endregion

		#region primary units

		private short _AngularPrecision = 0;
		/// <summary>Gets or sets the number of precision places displayed in angular dimensions. (<b>DIMADEC</b>)</summary>
		/// <remarks>
		/// Default: 0<br/>
		/// If set to -1 angular dimensions display the number of decimal places specified by LengthPrecision.
		/// It is recommended to use values in the range 0 to 8.
		/// </remarks>
		public short AngularPrecision
		{
			get => _AngularPrecision;
			set
			{
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The AngularPrecision must be greater than -1.");
				}
				_AngularPrecision = value;
			}
		}

		private short _LengthPrecision = 4;
		/// <summary>Gets or sets the number of decimal places displayed for the primary units of a dimension. (<b>DIMDEC</b>)</summary>
		/// <remarks>
		/// Default: 2<br/>
		/// It is recommended to use values in the range 0 to 8.<br/>
		/// For architectural and fractional the precision used for the minimum fraction is 1/2^LinearDecimalPlaces.
		/// </remarks>
		public short LengthPrecision
		{
			get => _LengthPrecision;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The LengthPrecision must be equals or greater than zero.");
				}
				_LengthPrecision = value;
			}
		}

		private string _DimPrefix = string.Empty;
		/// <summary>Gets or sets the text prefix for the dimension. (<b>DIMPOST</b>)</summary>
		/// <remarks>
		/// Default: string.Empty
		/// </remarks>
		public string DimPrefix
		{
			get => _DimPrefix;
			set => _DimPrefix = value ?? string.Empty;
		}

		private string _DimSuffix = string.Empty;
		/// <summary>Gets or sets the text suffix for the dimension. (<b>DIMPOST</b>)</summary>
		/// <remarks>
		/// Default: string.Empty
		/// </remarks>
		public string DimSuffix
		{
			get => _DimSuffix;
			set => _DimSuffix = value ?? string.Empty;
		}

		/// <summary>Gets or sets a single-character decimal separator to use when creating dimensions whose unit format is decimal. (<b>DIMDSEP</b>)</summary>
		/// <remarks>
		/// Default: "."
		/// </remarks>
		public char DecimalSeparator { get; set; } = '.';

		private double _DimScaleLinear = 1.0;
		/// <summary>Gets or sets a scale factor for linear dimension measurements. (<b>DIMLFAC</b>)</summary>
		/// <remarks>
		/// All linear dimension distances, including radii, diameters, and coordinates, are multiplied by DimScaleLinear before being converted to dimension text.<br />
		/// Positive values of DimScaleLinear are applied to dimensions in both model space and paper space; negative values are applied to paper space only.<br />
		/// DimScaleLinear has no effect on angular dimensions.
		/// </remarks>
		public double DimScaleLinear
		{
			get => _DimScaleLinear;
			set
			{
				if (MathHelper.IsZero(value))
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The scale factor cannot be zero.");
				}
				_DimScaleLinear = value;
			}
		}

		/// <summary>Gets or sets the units for all dimension types except angular. (<b>DIMLUNIT</b>)</summary>
		/// <remarks>
		/// Scientific<br/>
		/// Decimal<br/>
		/// Engineering<br/>
		/// Architectural<br/>
		/// Fractional
		/// </remarks>
		public LinearUnitType DimLengthUnits { get; set; } = LinearUnitType.Decimal;

		private AngleUnitType _DimAngularUnits = AngleUnitType.DecimalDegrees;
		/// <summary>Gets or sets the units format for angular dimensions. (<b>DIMAUNIT</b>)</summary>
		/// <remarks>
		/// Decimal degrees<br/>
		/// Degrees/minutes/seconds<br/>
		/// Gradians<br/>
		/// Radians
		/// </remarks>
		public AngleUnitType DimAngularUnits
		{
			get => _DimAngularUnits;
			set
			{
				if (value == AngleUnitType.SurveyorUnits)
				{
					throw new ArgumentException("Surveyor's units are not applicable in angular dimensions.");
				}
				_DimAngularUnits = value;
			}
		}

		/// <summary>Gets or sets the fraction format when <b>DIMLUNIT</b> is set to <see cref="LinearUnitType.Architectural"/> or <see cref="LinearUnitType.Fractional"/>. (<b>DIMFRAC</b>)</summary>
		/// <remarks>
		/// Horizontal stacking<br/>
		/// Diagonal stacking<br/>
		/// Not stacked (for example, 1/2)
		/// </remarks>
		public FractionFormatType FractionType { get; set; } = FractionFormatType.Horizontal;

		/// <summary>Suppresses leading zeros in linear decimal dimensions; for example, 0.5000 becomes .5000. (<b>DIMZIN</b>)</summary>
		/// <remarks>
		/// This value is part of the <b>DIMZIN</b> variable.
		/// </remarks>
		public bool SuppressLinearLeadingZeros { get; set; } = false;

		/// <summary>Suppresses trailing zeros in linear decimal dimensions. (<b>DIMZIN</b>)</summary>
		/// <remarks>
		/// This value is part of the <b>DIMZIN</b> variable.
		/// </remarks>
		public bool SuppressLinearTrailingZeros { get; set; } = false;

		/// <summary>Suppresses zero feet in architectural dimensions. (<b>DIMZIN</b>)</summary>
		/// <remarks>
		/// This value is part of the <b>DIMZIN</b> variable.
		/// </remarks>
		public bool SuppressZeroFeet { get; set; } = true;

		/// <summary>Suppresses zero inches in architectural dimensions. (<b>DIMZIN</b>)</summary>
		/// <remarks>
		/// This value is part of the <b>DIMZIN</b> variable.
		/// </remarks>
		public bool SuppressZeroInches { get; set; } = true;

		/// <summary>Suppresses leading zeros in angular decimal dimensions. (<b>DIMZIN</b>)</summary>
		/// <remarks>
		/// This value is part of the <b>DIMAZIN</b> variable.
		/// </remarks>
		public bool SuppressAngularLeadingZeros { get; set; } = false;

		/// <summary>Suppresses trailing zeros in angular decimal dimensions. (<b>DIMZIN</b>)</summary>
		/// <remarks>
		/// This value is part of the <b>DIMAZIN</b> variable.
		/// </remarks>
		public bool SuppressAngularTrailingZeros { get; set; } = false;

		private double _DimRoundoff = 0.0;
		/// <summary>Gets or sets the value to round all dimensioning distances. (<b>DIMRND</b>)</summary>
		/// <remarks>
		/// Default: 0 (no rounding off).<br/>
		/// If <b>DIMRND</b> is set to 0.25, all distances round to the nearest 0.25 unit.
		/// If you set <b>DIMRND</b> to 1.0, all distances round to the nearest integer.
		/// Note that the number of digits edited after the decimal point depends on the precision set by DIMDEC.
		/// <b>DIMRND</b> does not apply to angular dimensions.
		/// </remarks>
		public double DimRoundoff
		{
			get => _DimRoundoff;
			set
			{
				if (value < 0.000001 && !MathHelper.IsZero(value, double.Epsilon))
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The nearest value to round all distances must be equal or greater than 0.000001 or zero (no rounding off).");
				}
				_DimRoundoff = value;
			}
		}

		#endregion

		#region alternate units

		private DimensionStyleAlternateUnits _AlternateUnits = new DimensionStyleAlternateUnits();
		/// <summary>Gets or sets the alternate units format for dimensions.</summary>
		/// <remarks>Alternative units are not applicable for angular dimensions.</remarks>
		public DimensionStyleAlternateUnits AlternateUnits
		{
			get => _AlternateUnits;
			set => _AlternateUnits = value ?? throw new ArgumentNullException(nameof(value));
		}

		#endregion

		#region tolerances

		private DimensionStyleTolerances _Tolerances = new DimensionStyleTolerances();
		/// <summary>Gets or sets the tolerances format for dimensions.</summary>
		public DimensionStyleTolerances Tolerances
		{
			get => _Tolerances;
			set => _Tolerances = value ?? throw new ArgumentNullException(nameof(value));
		}

		#endregion

		/// <summary>Gets the owner of the actual dimension style.</summary>
		public new DimensionStyles Owner
		{
			get => (DimensionStyles)base.Owner;
			internal set => base.Owner = value;
		}

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override bool HasReferences() => this.Owner != null && this.Owner.HasReferences(this.Name);

		/// <inheritdoc/>
		public override List<DxfObjectReference> GetReferences() => this.Owner?.GetReferences(this.Name);

		/// <inheritdoc/>
		public override TableObject Clone(string newName)
		{
			DimensionStyle copy = new DimensionStyle(newName)
			{
				// dimension lines
				DimLineColor = (AciColor)_DimLineColor.Clone(),
				DimLineLinetype = (Linetype)_DimLineLinetype.Clone(),
				DimLineLineweight = this.DimLineLineweight,
				DimLine1Off = this.DimLine1Off,
				DimLine2Off = this.DimLine2Off,
				DimBaselineSpacing = _DimBaselineSpacing,
				DimLineExtend = _DimLineExtend,

				// extension lines
				ExtLineColor = (AciColor)_ExtLineColor.Clone(),
				ExtLine1Linetype = (Linetype)_ExtLine1Linetype.Clone(),
				ExtLine2Linetype = (Linetype)_ExtLine2Linetype.Clone(),
				ExtLineLineweight = this.ExtLineLineweight,
				ExtLine1Off = this.ExtLine1Off,
				ExtLine2Off = this.ExtLine2Off,
				ExtLineOffset = _ExtLineOffset,
				ExtLineExtend = _ExtLineExtend,

				// symbols and arrows
				ArrowSize = _ArrowSize,
				CenterMarkSize = this.CenterMarkSize,
				LeaderArrow = (Block)_LeaderArrow?.Clone(),
				DimArrow1 = (Block)_DimArrow1?.Clone(),
				DimArrow2 = (Block)_DimArrow2?.Clone(),

				// text appearance
				TextStyle = (TextStyle)_TextStyle.Clone(),
				TextColor = (AciColor)_TextColor.Clone(),
				TextFillColor = (AciColor)this.TextFillColor?.Clone(),
				TextHeight = _TextHeight,
				TextHorizontalPlacement = this.TextHorizontalPlacement,
				TextVerticalPlacement = this.TextVerticalPlacement,
				TextOffset = this.TextOffset,
				TextFractionHeightScale = _TextFractionHeightScale,

				// fit
				FitDimLineForce = this.FitDimLineForce,
				FitDimLineInside = this.FitDimLineInside,
				DimScaleOverall = _DimScaleOverall,
				FitOptions = this.FitOptions,
				FitTextInside = this.FitTextInside,
				FitTextMove = this.FitTextMove,

				// primary units
				AngularPrecision = _AngularPrecision,
				LengthPrecision = _LengthPrecision,
				DimPrefix = _DimPrefix,
				DimSuffix = _DimSuffix,
				DecimalSeparator = this.DecimalSeparator,
				DimScaleLinear = _DimScaleLinear,
				DimLengthUnits = this.DimLengthUnits,
				DimAngularUnits = _DimAngularUnits,
				FractionType = this.FractionType,
				SuppressLinearLeadingZeros = this.SuppressLinearLeadingZeros,
				SuppressLinearTrailingZeros = this.SuppressLinearTrailingZeros,
				SuppressZeroFeet = this.SuppressZeroFeet,
				SuppressZeroInches = this.SuppressZeroInches,
				SuppressAngularLeadingZeros = this.SuppressAngularLeadingZeros,
				SuppressAngularTrailingZeros = this.SuppressAngularTrailingZeros,
				DimRoundoff = _DimRoundoff,

				// alternate units
				AlternateUnits = (DimensionStyleAlternateUnits)_AlternateUnits.Clone(),

				// tolerances
				Tolerances = (DimensionStyleTolerances)_Tolerances.Clone()
			};

			foreach (XData data in this.XData.Values)
			{
				copy.XData.Add((XData)data.Clone());
			}

			return copy;
		}
		/// <inheritdoc/>
		public override object Clone() => this.Clone(this.Name);

		#endregion
	}
}