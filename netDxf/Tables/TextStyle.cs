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
using System.IO;
using netDxf.Collections;

namespace netDxf.Tables
{
	/// <summary>Represents a text style.</summary>
	public class TextStyle :
		TableObject
	{
		#region constants

		/// <summary>Default text style name.</summary>
		public const string DefaultName = "Standard";

		/// <summary>Default text style font.</summary>
		public const string DefaultFont = "simplex.shx";

		/// <summary>Gets the default text style.</summary>
		public static TextStyle Default => new TextStyle(DefaultName, DefaultFont);

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">Text style name.</param>
		/// <param name="font">Text style font file name with full or relative path.</param>
		public TextStyle(string name, string font)
			: this(name, font, true)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">Text style name.</param>
		/// <param name="font">Text style font file name with full or relative path.</param>
		/// <param name="checkName">Specifies if the style name has to be checked.</param>
		internal TextStyle(string name, string font, bool checkName)
			: base(name, DxfObjectCode.TextStyle, checkName)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name), "The text style name should be at least one character long.");
			}

			if (string.IsNullOrEmpty(font))
			{
				throw new ArgumentNullException(nameof(font));
			}

			if (!Path.GetExtension(font).Equals(".TTF", StringComparison.OrdinalIgnoreCase) &&
				!Path.GetExtension(font).Equals(".SHX", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("Only true type TTF fonts and ACAD compiled shape SHX fonts are allowed.");
			}

			this.IsReserved = name.Equals(DefaultName, StringComparison.OrdinalIgnoreCase);
			_File = font;
			_FontFamilyName = string.Empty;
			_FontStyle = FontStyle.Regular;
		}
		/// <summary>Initializes a new instance of the class exclusively to be used with <see langword="true"/> type fonts.</summary>
		/// <param name="name">Text style name.</param>
		/// <param name="fontFamily"><see langword="true"/> type font family name.</param>
		/// <param name="fontStyle"><see langword="true"/> type font style.</param>
		/// <remarks>This constructor is to be use only with <see langword="true"/> type fonts.</remarks>
		public TextStyle(string name, string fontFamily, FontStyle fontStyle)
			: this(name, fontFamily, fontStyle, true)
		{
		}
		/// <summary>Initializes a new instance of the class exclusively to be used with <see langword="true"/> type fonts.</summary>
		/// <param name="name">Text style name.</param>
		/// <param name="fontFamily"><see langword="true"/> type font family name.</param>
		/// <param name="fontStyle"><see langword="true"/> type font style</param>
		/// <param name="checkName">Specifies if the style name has to be checked.</param>
		/// <remarks>This constructor is to be use only with <see langword="true"/> type fonts.</remarks>
		internal TextStyle(string name, string fontFamily, FontStyle fontStyle, bool checkName)
			: base(name, DxfObjectCode.TextStyle, checkName)
		{
			_File = string.Empty;
			if (string.IsNullOrEmpty(fontFamily))
			{
				throw new ArgumentNullException(nameof(fontFamily));
			}
			_FontFamilyName = fontFamily;
			_FontStyle = fontStyle;
		}

		#endregion

		#region public properties

		private string _File;
		/// <summary>Gets or sets the text style font file name.</summary>
		/// <remarks>
		/// When this value is used for <see langword="true"/> type fonts should be present in the Font system folder.<br />
		/// When the style does not contain any information for the file the font information will be saved in the extended data when saved to a DXF,
		/// this is only applicable for <see langword="true"/> type fonts.
		/// </remarks>
		public string FontFile
		{
			get => _File;
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException(nameof(value));
				}

				if (!Path.GetExtension(value).Equals(".TTF", StringComparison.OrdinalIgnoreCase) &&
					!Path.GetExtension(value).Equals(".SHX", StringComparison.OrdinalIgnoreCase))
				{
					throw new ArgumentException("Only true type TTF fonts and ACAD compiled shape SHX fonts are allowed.");
				}

				_FontFamilyName = string.Empty;
				_BigFont = string.Empty;
				_FontStyle = FontStyle.Regular;
				_File = value;
			}
		}

		private string _BigFont = string.Empty;
		/// <summary>Gets or sets an <b>Asian</b>-language Big Font file.</summary>
		/// <remarks>Only <b>AutoCAD</b> compiled shape <b>SHX</b> fonts are valid for creating Big Fonts.</remarks>
		public string BigFont
		{
			get => _BigFont;
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					_BigFont = string.Empty;
				}
				else
				{
					if (string.IsNullOrEmpty(_File))
					{
						throw new NullReferenceException("The Big Font is only applicable for SHX Asian fonts.");
					}

					if (!Path.GetExtension(_File).Equals(".SHX", StringComparison.OrdinalIgnoreCase))
					{
						throw new NullReferenceException("The Big Font is only applicable for SHX Asian fonts.");
					}

					if (!Path.GetExtension(value).Equals(".SHX", StringComparison.OrdinalIgnoreCase))
					{
						throw new ArgumentException("The Big Font is only applicable for SHX Asian fonts.", nameof(value));
					}

					_BigFont = value;
				}
			}
		}

		private string _FontFamilyName;
		/// <summary>Gets or sets the <see langword="true"/> type font family name.</summary>
		/// <remarks>
		/// When the font family name is manually specified the file font will not be used and it will be set to empty,
		/// the font style will also we set to FontStyle.Regular.
		/// In this case the font information will be stored in the style extended data when saved to a DXF.<br />
		/// This value is only applicable for <see langword="true"/> type fonts.
		/// </remarks>
		public string FontFamilyName
		{
			get => _FontFamilyName;
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException(nameof(value));
				}
				_File = string.Empty;
				_BigFont = string.Empty;
				_FontStyle = FontStyle.Regular;
				_FontFamilyName = value;
			}
		}

		private FontStyle _FontStyle;
		/// <summary>Gets or sets the <see langword="true"/> type font style.</summary>
		/// <remarks>
		/// The font style value is ignored and will always return FontStyle.Regular when a font file has been specified.<br />
		/// All styles may not be available for the current font family.
		/// </remarks>
		public FontStyle FontStyle
		{
			get => _FontStyle;
			set
			{
				if (string.IsNullOrEmpty(_File))
				{
					_FontStyle = value;
				}
			}
		}

		private double _Height = 0.0;
		/// <summary>Gets or sets the text height.</summary>
		/// <remarks>Fixed text height; 0 if not fixed.</remarks>
		public double Height
		{
			get => _Height;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The TextStyle height must be equals or greater than zero.");
				}
				_Height = value;
			}
		}

		private double _WidthFactor = 1.0;
		/// <summary>Gets or sets the text width factor.</summary>
		/// <remarks>Valid values range from 0.01 to 100. Default: 1.0.</remarks>
		public double WidthFactor
		{
			get => _WidthFactor;
			set
			{
				if (value < 0.01 || value > 100.0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The TextStyle width factor valid values range from 0.01 to 100.");
				}
				_WidthFactor = value;
			}
		}

		private double _ObliqueAngle = 0.0;
		/// <summary>Gets or sets the font oblique angle in degrees.</summary>
		/// <remarks>Valid values range from -85 to 85. Default: 0.0.</remarks>
		public double ObliqueAngle
		{
			get => _ObliqueAngle;
			set
			{
				if (value < -85.0 || value > 85.0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The TextStyle oblique angle valid values range from -85 to 85.");
				}
				_ObliqueAngle = value;
			}
		}

		/// <summary>Gets or sets the text is vertical.</summary>
		public bool IsVertical { get; set; } = false;

		/// <summary>Gets or sets if the text is backward (mirrored in X).</summary>
		public bool IsBackward { get; set; } = false;

		/// <summary>Gets or sets if the text is upside down (mirrored in Y).</summary>
		public bool IsUpsideDown { get; set; } = false;

		/// <summary>Gets the owner of the actual text style.</summary>
		public new TextStyles Owner
		{
			get => (TextStyles)base.Owner;
			internal set => base.Owner = value;
		}

		#endregion

		#region public methods

#if NET4X

		/// <summary>Find the font family name of an specified <b>TTF</b> font file. Only available for Net Framework 4.5 builds.</summary>
		/// <param name="ttfFont">TTF font file.</param>
		/// <returns>The font family name of the specified <b>TTF</b> font file.</returns>
		/// <remarks>This method will return an empty string if the specified font is not found in its path or the system font folder or if it is not a valid <b>TTF</b> font.</remarks>
		public static string TrueTypeFontFamilyName(string ttfFont)
		{
			if (string.IsNullOrEmpty(ttfFont))
			{
				throw new ArgumentNullException(nameof(ttfFont));
			}

			// the following information is only applied to TTF not SHX fonts
			if (!Path.GetExtension(ttfFont).Equals(".TTF", StringComparison.InvariantCultureIgnoreCase))
			{
				return string.Empty;
			}

			// try to find the file in the specified directory, if not try it in the fonts system folder
			string fontFile = File.Exists(ttfFont) ?
				Path.GetFullPath(ttfFont) :
				string.Format("{0}{1}{2}", Environment.GetFolderPath(Environment.SpecialFolder.Fonts), Path.DirectorySeparatorChar, Path.GetFileName(ttfFont));

			System.Drawing.Text.PrivateFontCollection fontCollection = new System.Drawing.Text.PrivateFontCollection();
			try
			{
				fontCollection.AddFontFile(fontFile);
				return fontCollection.Families[0].Name;
			}
			catch (FileNotFoundException)
			{
				return string.Empty;
			}
			finally
			{
				fontCollection.Dispose();
			}
		}

#endif

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override bool HasReferences() => this.Owner != null && this.Owner.HasReferences(this.Name);

		/// <inheritdoc/>
		public override List<DxfObjectReference> GetReferences() => this.Owner?.GetReferences(this.Name);

		/// <inheritdoc/>
		public override TableObject Clone(string newName)
		{
			TextStyle copy;

			if (string.IsNullOrEmpty(this.FontFamilyName))
			{
				copy = new TextStyle(newName, _File)
				{
					Height = _Height,
					IsBackward = this.IsBackward,
					IsUpsideDown = this.IsUpsideDown,
					IsVertical = this.IsVertical,
					ObliqueAngle = _ObliqueAngle,
					WidthFactor = _WidthFactor
				};
			}
			else
			{
				copy = new TextStyle(newName, _FontFamilyName, _FontStyle)
				{
					Height = _Height,
					IsBackward = this.IsBackward,
					IsUpsideDown = this.IsUpsideDown,
					IsVertical = this.IsVertical,
					ObliqueAngle = _ObliqueAngle,
					WidthFactor = _WidthFactor
				};
			}

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
