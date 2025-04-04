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
using System.Globalization;

namespace netDxf
{
	/// <summary>Represents the transparency of a layer or an entity.</summary>
	/// <remarks>
	/// When the transparency of an entity is <b>ByLayer</b> the code 440 will not appear in the dxf,
	/// but for comparison purposes the <b>ByLayer</b> transparency is assigned a value of -1.
	/// </remarks>
	public class Transparency :
		ICloneable,
		IEquatable<Transparency>
	{
		#region constants

		/// <summary>Gets the <b>ByLayer</b> transparency.</summary>
		public static Transparency ByLayer => new Transparency { _Transparency = -1 };

		/// <summary>Gets the <b>ByBlock</b> transparency.</summary>
		public static Transparency ByBlock => new Transparency { _Transparency = 100 };

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		public Transparency()
		{
			_Transparency = -1;
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="value">Alpha value range from 0 to 90.</param>
		/// <remarks>
		/// Accepted transparency values range from 0 (opaque) to 90 (almost transparent), the reserved values -1 and 100 represents <b>ByLayer</b> and <b>ByBlock</b> transparency.
		/// </remarks>
		public Transparency(short value)
		{
			if (value < 0 || value > 90)
			{
				throw new ArgumentOutOfRangeException(nameof(value), value, "Accepted transparency values range from 0 to 90.");
			}
			_Transparency = value;
		}

		#endregion

		#region public properties

		/// <summary>Defines if the transparency is defined by layer.</summary>
		public bool IsByLayer => _Transparency == -1;

		/// <summary>Defines if the transparency is defined by block.</summary>
		public bool IsByBlock => _Transparency == 100;

		private short _Transparency;
		/// <summary>Gets or sets the transparency value range from 0 to 90.</summary>
		/// <remarks>
		/// Accepted transparency values range from 0 to 90, the reserved values -1 and 100 represents <b>ByLayer</b> and <b>ByBlock</b>.
		/// </remarks>
		public short Value
		{
			get => _Transparency;
			set
			{
				if (value < 0 || value > 90)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "Accepted transparency values range from 0 to 90.");
				}
				_Transparency = value;
			}
		}

		#endregion

		#region public methods

		/// <summary>Gets the transparency value from a <see cref="Transparency">transparency</see> object.</summary>
		/// <param name="transparency">A <see cref="Transparency">transparency</see>.</param>
		/// <returns>A transparency value.</returns>
		public static int ToAlphaValue(Transparency transparency)
		{
			if (transparency == null)
			{
				throw new ArgumentNullException(nameof(transparency));
			}

			byte alpha = (byte)(255 * (100 - transparency.Value) / 100.0);
			byte[] bytes = transparency.IsByBlock ? new byte[] { 0, 0, 0, 1 } : new byte[] { alpha, 0, 0, 2 };
			return BitConverter.ToInt32(bytes, 0);
		}

		/// <summary>Gets the <see cref="Transparency">transparency</see> object from a transparency value.</summary>
		/// <param name="value">A transparency value.</param>
		/// <returns>A <see cref="Transparency">transparency</see></returns>
		public static Transparency FromAlphaValue(int value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			short alpha = (short)(100 - (bytes[0] / 255.0) * 100);
			return FromCadIndex(alpha);
		}

		public static Transparency FromCadIndex(short alpha)
		{
			if (alpha == -1)
			{
				return ByLayer;
			}
			if (alpha == 100)
			{
				return ByBlock;
			}
			if (alpha < 0)
			{
				return new Transparency(0);
			}
			if (alpha > 90)
			{
				return new Transparency(90);
			}

			return new Transparency(alpha);
		}

		#endregion

		#region implements ICloneable

		/// <inheritdoc/>
		public object Clone() => FromCadIndex(_Transparency);

		#endregion

		#region implements IEquatable

		/// <inheritdoc/>
		public bool Equals(Transparency other)
		{
			if (other == null)
			{
				return false;
			}

			return other._Transparency == _Transparency;
		}

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override string ToString()
		{
			if (_Transparency == -1)
			{
				return "ByLayer";
			}

			if (_Transparency == 100)
			{
				return "ByBlock";
			}

			return _Transparency.ToString(CultureInfo.CurrentCulture);
		}

		#endregion
	}
}