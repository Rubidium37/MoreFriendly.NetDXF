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
	/// <summary>Options for the <see cref="MText">multiline text</see> entity text formatting.</summary>
	/// <remarks>Old <b>DXF</b> versions might not support all available formatting codes.</remarks>
	public class MTextFormattingOptions
	{
		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		public MTextFormattingOptions()
		{
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets if the text is bold.</summary>
		/// <remarks>The font style must support bold characters.</remarks>
		public bool Bold { get; set; } = false;

		/// <summary>Gets or sets if the text is italic.</summary>
		/// <remarks>The font style must support italic characters.</remarks>
		public bool Italic { get; set; } = false;

		/// <summary>Gets or sets the over line.</summary>
		public bool Overline { get; set; } = false;

		/// <summary>Gets or sets underline.</summary>
		public bool Underline { get; set; } = false;

		/// <summary>Gets or sets strike-through.</summary>
		public bool StrikeThrough { get; set; } = false;

		private bool _Superscript;
		/// <summary>Get or set if the text is superscript.</summary>
		/// <remarks>
		/// The Superscript and subscript properties are mutually exclusive, if it is set to <see langword="true"/> the <see cref="Subscript"/> property will be set to <see langword="false"/> automatically.<br />
		/// Internally, superscripts and subscripts are written as stacking text (like fractions);
		/// therefore the characters '/' and '#' are reserved if you need to write them you must write '\/' and '\#' respectively.
		/// </remarks>
		public bool Superscript
		{
			get => _Superscript;
			set
			{
				if (value) _Subscript = false;
				_Superscript = value;
			}
		}

		private bool _Subscript;
		/// <summary>Get or set if the text is subscript.</summary>
		/// <remarks>
		/// The Superscript and Subscript properties are mutually exclusive, if it is set to <see langword="true"/> the <see cref="Superscript"/> property will be set to <see langword="false"/> automatically.<br />
		/// Internally, superscripts and subscripts are written as stacking text (like fractions);
		/// therefore the characters '/' and '#' are reserved if you need to write them you must write '\/' and '\#' respectively.
		/// </remarks>
		public bool Subscript
		{
			get => _Subscript;
			set
			{
				if (value) _Superscript = false;
				_Subscript = value;
			}
		}

		private double _SuperSubScriptHeightFactor = 0.7;
		/// <summary>Gets or sets the superscript and subscript text height as a multiple of the current text height.</summary>
		/// <remarks>By default it is set as 0.7 the current text height.</remarks>
		public double SuperSubScriptHeightFactor
		{
			get => _SuperSubScriptHeightFactor;
			set
			{
				if (value <= 0)
					throw new ArgumentOutOfRangeException(nameof(value), value, "The character percentage height must be greater than zero.");
				_SuperSubScriptHeightFactor = value;
			}
		}

		/// <summary>Gets or sets the text color.</summary>
		/// <remarks>
		/// Set as <see langword="null"/> to apply the default color defined by the MText entity.
		/// </remarks>
		public AciColor Color { get; set; }

		/// <summary>Gets or sets the font that will override the default defined in the TextStyle.</summary>
		/// <remarks>
		/// Set as <see langword="null"/> or empty to apply the default font.<br />
		/// When using <b>SHX</b> fonts use the font file with the <b>SHX</b> extension,
		/// when using <b>TTF</b> fonts use the font family name.
		/// </remarks>
		public string FontName { get; set; }

		private double _HeightFactor = 1.0;
		/// <summary>Gets or sets the text height as a multiple of the current text height.</summary>
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

		private double _ObliqueAngle = 0.0;
		/// <summary>Gets or sets the obliquing angle in degrees.</summary>
		/// <remarks>Set as 0.0 to apply the default obliquing angle.</remarks>
		public double ObliqueAngle
		{
			get => _ObliqueAngle;
			set
			{
				if (value < -85.0 || value > 85.0)
					throw new ArgumentOutOfRangeException(nameof(value), value, "The oblique angle valid values range from -85 to 85.");
				_ObliqueAngle = value;
			}
		}

		private double _CharacterSpaceFactor = 1.0;
		/// <summary>Gets or sets the space between characters as a multiple of the original spacing between characters.</summary>
		/// <remarks>
		/// Valid values range from a minimum of .75 to 4 times the original spacing between characters.
		/// Set as 1.0 to apply the default character space factor.
		/// </remarks>
		public double CharacterSpaceFactor
		{
			get => _CharacterSpaceFactor;
			set
			{
				if (value < 0.75 || value > 4)
					throw new ArgumentOutOfRangeException(nameof(value), value, "The character space valid values range from a minimum of .75 to 4");
				_CharacterSpaceFactor = value;
			}
		}

		private double _WidthFactor = 1.0;
		/// <summary>Gets or sets the width factor to produce wide text.</summary>
		/// <remarks>Set as 1.0 to apply the default width factor.</remarks>
		public double WidthFactor
		{
			get => _WidthFactor;
			set
			{
				if (value <= 0)
					throw new ArgumentOutOfRangeException(nameof(value), value, "The width factor should be greater than zero.");
				_WidthFactor = value;
			}
		}

		#endregion
	}
}