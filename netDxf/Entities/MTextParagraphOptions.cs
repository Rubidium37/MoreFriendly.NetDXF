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

namespace netDxf.Entities
{
	/// <summary>Options for the <see cref="MText">multiline text</see> entity paragraph formatting.</summary>
	/// <remarks>Old <b>DXF</b> versions might not support all available formatting codes.</remarks>
	public class MTextParagraphOptions
	{
		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		public MTextParagraphOptions()
		{
		}

		#endregion

		#region public properties

		private double _HeightFactor = 1.0;
		/// <summary>Gets or sets the paragraph height factor.</summary>
		/// <remarks>Set as 1.0 to apply the default height factor.</remarks>
		public double HeightFactor
		{
			get => _HeightFactor;
			set
			{
				if (value <= 0)
					throw new ArgumentOutOfRangeException(nameof(value), value, "The character percentage height must be greater than zero.");
				_HeightFactor = value;
			}
		}

		/// <summary>Gets or sets the paragraph justification (text horizontal alignment).</summary>
		public MTextParagraphAlignment Alignment { get; set; } = MTextParagraphAlignment.Left;

		/// <summary>Gets or sets the paragraph line vertical alignment.</summary>
		/// <remarks>
		/// The vertical alignment affects how fractions, superscripts, subscripts, and characters of different heights are placed in a paragraph line.
		/// By default the paragraph vertical alignment is Center.
		/// </remarks>
		public MTextParagraphVerticalAlignment VerticalAlignment { get; set; } = MTextParagraphVerticalAlignment.Center;

		private double _SpacingBefore = 0.0;
		/// <summary>Specifies the spacing before the paragraphs.</summary>
		/// <remarks>
		/// If set to zero no value will be applied and the default will be inherited. When it is non zero, valid values range from 0.25 to 4.0.<br />
		/// The distance between two paragraphs is determined by the total of the after paragraph spacing value of the upper paragraph
		/// and the before paragraph spacing value of the lower paragraph.
		/// </remarks>
		public double SpacingBefore
		{
			get => _SpacingBefore;
			set
			{
				if (MathHelper.IsZero(value))
				{
					_SpacingBefore = 0.0;
				}
				else
				{
					if (value < 0.25 || value > 4.0)
						throw new ArgumentOutOfRangeException(nameof(value), value, "The paragraph spacing valid values range from 0.25 to 4.0");
					_SpacingBefore = value;
				}
			}
		}

		private double _SpacingAfter = 0.0;
		/// <summary>Specifies the spacing before or after the paragraph.</summary>
		/// <remarks>
		/// If set to zero no value will be applied and the default will be inherited. When it is non zero, valid values range from 0.25 to 4.0.<br />
		/// The distance between two paragraphs is determined by the total of the after paragraph spacing value of the upper paragraph
		/// and the before paragraph spacing value of the lower paragraph.
		/// </remarks>
		public double SpacingAfter
		{
			get => _SpacingAfter;
			set
			{
				if (MathHelper.IsZero(value))
				{
					_SpacingAfter = 0.0;
				}
				else
				{
					if (value < 0.25 || value > 4.0)
						throw new ArgumentOutOfRangeException(nameof(value), value, "The paragraph spacing valid values range from 0.25 to 4.0");
					_SpacingAfter = value;
				}
			}
		}

		private double _FirstLineIndent = 0.0;
		/// <summary>Gets or sets the indent value for the first line of the paragraph.</summary>
		/// <remarks>
		/// Valid values range from -10000.0 to 10000.0, the default value 0.0.<br />
		/// Negative first line indent values are limited by the left indent,
		/// in the case its absolute value is larger than the left indent, when applied to the paragraph it will be automatically adjusted .
		/// </remarks>
		public double FirstLineIndent
		{
			get => _FirstLineIndent;
			set
			{
				if (value < -10000.0 || value > 10000.0)
					throw new ArgumentOutOfRangeException(nameof(value), value, "The paragraph indent valid values range from -10000.0 to 10000.0");

				_FirstLineIndent = value;
			}
		}

		private double _LeftIndent = 0.0;
		/// <summary>Gets or sets the left indent of the current paragraph.</summary>
		/// <remarks>
		/// Valid values range from 0.0 to 10000.0, the default value 0.0.
		/// </remarks>
		public double LeftIndent
		{
			get => _LeftIndent;
			set
			{
				if (value < 0.0 || value > 10000.0)
					throw new ArgumentOutOfRangeException(nameof(value), value, "The paragraph indent valid values range from 0.0 to 10000.0");
				_LeftIndent = value;
			}
		}

		private double _RightIndent = 0.0;
		/// <summary>Gets or sets the right indent value of the paragraphs.</summary>
		/// <remarks>
		/// Valid values range from 0.0 to 10000.0, the default value 0.0.
		/// </remarks>
		public double RightIndent
		{
			get => _RightIndent;
			set
			{
				if (value < 0.0 || value > 10000.0)
					throw new ArgumentOutOfRangeException(nameof(value), value, "The paragraph indent valid values range from 0.0 to 10000.0");
				_RightIndent = value;
			}
		}

		private double _LineSpacingFactor = 1.0;
		/// <summary>Gets or sets the paragraph line spacing factor.</summary>
		/// <remarks>
		/// Percentage of default line spacing to be applied. Valid values range from 0.25 to 4.0, the default value 1.0.
		/// </remarks>
		public double LineSpacingFactor
		{
			get => _LineSpacingFactor;
			set
			{
				if (value < 0.25 || value > 4.0)
					throw new ArgumentOutOfRangeException(nameof(value), value, "The MText LineSpacingFactor valid values range from 0.25 to 4.0");
				_LineSpacingFactor = value;
			}
		}

		/// <summary>Get or sets the paragraph <see cref="MTextLineSpacingStyle">line spacing style</see>.</summary>
		public MTextLineSpacingStyle LineSpacingStyle { get; set; } = MTextLineSpacingStyle.Default;

		#endregion
	}
}