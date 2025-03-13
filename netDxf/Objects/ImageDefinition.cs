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
using netDxf.Tables;
using netDxf.Units;

namespace netDxf.Objects
{
	/// <summary>Represents an image definition.</summary>
	public class ImageDefinition :
		TableObject
	{
		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="file">Image file name with full or relative path.</param>
		/// <param name="width">Image width in pixels.</param>
		/// <param name="horizontalResolution">Image horizontal resolution in pixels.</param>
		/// <param name="height">Image height in pixels.</param>
		/// <param name="verticalResolution">Image vertical resolution in pixels.</param>
		/// <param name="units">Image resolution units.</param>
		/// <remarks>
		/// <para>
		/// The name of the file without extension will be used as the name of the image definition.
		/// </para>
		/// <para>
		/// This is a generic constructor for all image formats supported by <b>AutoCAD</b>, note that not all <b>AutoCAD</b> versions support the same image formats.
		/// </para>
		/// <para>
		/// Note (this is from the <b>AutoCAD</b> docs): <b>AutoCAD 2000</b>, <b>AutoCAD LT 2000</b>, and later releases do not support LZW-compressed <b>TIFF</b> files,
		/// with the exception of English language versions sold in the <b>US</b> and Canada.<br />
		/// If you have <b>TIFF</b> files that were created using <b>LZW</b> compression and want to insert them into a drawing
		/// you must save the <b>TIFF</b> files with <b>LZW</b> compression disabled.
		/// </para>
		/// </remarks>
		public ImageDefinition(string file, int width, double horizontalResolution, int height, double verticalResolution, ImageResolutionUnits units)
			: this(Path.GetFileNameWithoutExtension(file), file, width, horizontalResolution, height, verticalResolution, units)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">Image definition name.</param>
		/// <param name="file">Image file name with full or relative path.</param>
		/// <param name="width">Image width in pixels.</param>
		/// <param name="horizontalResolution">Image horizontal resolution in pixels.</param>
		/// <param name="height">Image height in pixels.</param>
		/// <param name="verticalResolution">Image vertical resolution in pixels.</param>
		/// <param name="units">Image resolution units.</param>
		/// <remarks>
		/// <para>
		/// The name assigned to the image definition must be unique.
		/// </para>
		/// <para>
		/// This is a generic constructor for all image formats supported by <b>AutoCAD</b>, note that not all <b>AutoCAD</b> versions support the same image formats.
		/// </para>
		/// <para>
		/// Note (this is from the <b>AutoCAD</b> docs): <b>AutoCAD 2000</b>, <b>AutoCAD LT 2000</b>, and later releases do not support LZW-compressed <b>TIFF</b> files,
		/// with the exception of English language versions sold in the <b>US</b> and Canada.<br />
		/// If you have <b>TIFF</b> files that were created using <b>LZW</b> compression and want to insert them into a drawing
		/// you must save the <b>TIFF</b> files with <b>LZW</b> compression disabled.
		/// </para>
		/// </remarks>
		public ImageDefinition(string name, string file, int width, double horizontalResolution, int height, double verticalResolution, ImageResolutionUnits units)
			: base(name, DxfObjectCode.ImageDef, false)
		{
			if (string.IsNullOrEmpty(file))
			{
				throw new ArgumentNullException(nameof(file));
			}

			if (file.IndexOfAny(Path.GetInvalidPathChars()) == 0)
			{
				throw new ArgumentException("File path contains invalid characters.", nameof(file));
			}

			_File = file;

			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(width), width, "The ImageDefinition width must be greater than zero.");
			}
			_Width = width;

			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(height), height, "The ImageDefinition height must be greater than zero.");
			}
			_Height = height;

			if (horizontalResolution <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(horizontalResolution), horizontalResolution, "The ImageDefinition horizontal resolution must be greater than zero.");
			}
			_HorizontalResolution = horizontalResolution;

			if (verticalResolution <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(verticalResolution), verticalResolution, "The ImageDefinition vertical resolution must be greater than zero.");
			}
			_VerticalResolution = verticalResolution;

			_ResolutionUnits = units;
		}

#if NET4X

		/// <summary>Initializes a new instance of the class. Only available for Net Framework 4.5 builds.</summary>
		/// <param name="file">Image file name with full or relative path.</param>
		/// <remarks>
		/// <para>
		/// The name of the file without extension will be used as the name of the image definition.
		/// </para>
		/// <para>
		/// Supported image formats: <b>BMP</b>, <b>JPG</b>, <b>PNG</b>, <b>TIFF</b>.<br />
		/// Even thought <b>AutoCAD</b> supports more image formats, this constructor is restricted to the ones the net framework supports in common with AutoCAD.
		/// Use the generic constructor instead.
		/// </para>
		/// <para>
		/// Note (this is from the <b>AutoCAD</b> docs): <b>AutoCAD 2000</b>, <b>AutoCAD LT 2000</b>, and later releases do not support LZW-compressed <b>TIFF</b> files,
		/// with the exception of English language versions sold in the <b>US</b> and Canada.<br />
		/// If you have <b>TIFF</b> files that were created using <b>LZW</b> compression and want to insert them into a drawing
		/// you must save the <b>TIFF</b> files with <b>LZW</b> compression disabled.
		/// </para>
		/// </remarks>
		public ImageDefinition(string file)
			: this(Path.GetFileNameWithoutExtension(file), file)
		{
		}
		/// <summary>Initializes a new instance of the class. Only available for Net Framework 4.5 builds.</summary>
		/// <param name="name">Image definition name.</param>
		/// <param name="file">Image file name with full or relative path.</param>
		/// <remarks>
		/// <para>
		/// The name assigned to the image definition must be unique.
		/// </para>
		/// <para>
		/// Supported image formats: <b>BMP</b>, <b>JPG</b>, <b>PNG</b>, <b>TIFF</b>.<br />
		/// Even thought <b>AutoCAD</b> supports more image formats, this constructor is restricted to the ones the .net library supports in common with AutoCAD.
		/// Use the generic constructor instead.
		/// </para>
		/// <para>
		/// Note (this is from the <b>AutoCAD</b> docs): <b>AutoCAD 2000</b>, <b>AutoCAD LT 2000</b>, and later releases do not support LZW-compressed <b>TIFF</b> files,
		/// with the exception of English language versions sold in the <b>US</b> and Canada.<br />
		/// If you have <b>TIFF</b> files that were created using <b>LZW</b> compression and want to insert them into a drawing
		/// you must save the <b>TIFF</b> files with <b>LZW</b> compression disabled.
		/// </para>
		/// </remarks>
		public ImageDefinition(string name, string file)
			: base(name, DxfObjectCode.ImageDef, false)
		{
			if (string.IsNullOrEmpty(file))
			{
				throw new ArgumentNullException(nameof(file));
			}

			FileInfo info = new FileInfo(file);
			if (!info.Exists)
			{
				throw new FileNotFoundException("Image file not found.", file);
			}

			_File = file;

			try
			{
				using (System.Drawing.Image bitmap = System.Drawing.Image.FromFile(file))
				{
					_Width = bitmap.Width;
					_Height = bitmap.Height;
					_HorizontalResolution = bitmap.HorizontalResolution;
					_VerticalResolution = bitmap.VerticalResolution;
					// the System.Drawing.Image stores the image resolution in inches
					_ResolutionUnits = ImageResolutionUnits.Inches;
				}
			}
			catch (Exception)
			{
				throw new ArgumentException("Image file not supported.", file);
			}

		}

#endif

		#endregion

		#region public properties

		private string _File;
		/// <summary>Gets or sets the image file.</summary>
		/// <remarks>
		/// When changing the image file the other properties should also be modified accordingly to avoid distortions in the final image.
		/// </remarks>
		public string File
		{
			get => _File;
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException(nameof(value));
				}

				if (value.IndexOfAny(Path.GetInvalidPathChars()) == 0)
				{
					throw new ArgumentException("File path contains invalid characters.", nameof(value));
				}

				_File = value;
			}
		}

		private int _Width;
		/// <summary>Gets or sets the image width in pixels.</summary>
		public int Width
		{
			get => _Width;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The ImageDefinition width must be greater than zero.");
				}

				_Width = value;
			}
		}

		private int _Height;
		/// <summary>Gets or sets the image height in pixels.</summary>
		public int Height
		{
			get => _Height;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The ImageDefinition height must be greater than zero.");
				}

				_Height = value;
			}
		}

		private double _HorizontalResolution;
		/// <summary>Gets or sets the image horizontal resolution in pixels per unit.</summary>
		public double HorizontalResolution
		{
			get => _HorizontalResolution;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The ImageDefinition horizontal resolution must be greater than zero.");
				}

				_HorizontalResolution = value;
			}
		}

		private double _VerticalResolution;
		/// <summary>Gets or sets the image vertical resolution in pixels per unit.</summary>
		public double VerticalResolution
		{
			get => _VerticalResolution;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The ImageDefinition vertical resolution must be greater than zero.");
				}

				_VerticalResolution = value;
			}
		}

		private ImageResolutionUnits _ResolutionUnits;
		/// <summary>Gets or sets the image resolution units.</summary>
		public ImageResolutionUnits ResolutionUnits
		{
			get => _ResolutionUnits;
			set
			{
				if (_ResolutionUnits != value)
				{
					switch (value)
					{
						case ImageResolutionUnits.Centimeters:
							_HorizontalResolution /= 2.54;
							_VerticalResolution /= 2.54;
							break;
						case ImageResolutionUnits.Inches:
							_HorizontalResolution *= 2.54;
							_VerticalResolution *= 2.54;
							break;
						case ImageResolutionUnits.Unitless:
							break;
					}
				}
				_ResolutionUnits = value;
			}
		}

		/// <summary>Gets the owner of the actual image definition.</summary>
		public new ImageDefinitions Owner
		{
			get => (ImageDefinitions)base.Owner;
			internal set => base.Owner = value;
		}

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override bool HasReferences() => this.Owner != null && this.Owner.HasReferences(this.Name);

		/// <inheritdoc/>
		public override List<DxfObjectReference> GetReferences()
		{
			if (this.Owner == null)
			{
				return null;
			}

			return this.Owner.GetReferences(this.Name);
		}

		/// <inheritdoc/>
		public override TableObject Clone(string newName)
		{
			ImageDefinition copy = new ImageDefinition(newName, _File, _Width, _HorizontalResolution, _Height, _VerticalResolution, _ResolutionUnits);

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