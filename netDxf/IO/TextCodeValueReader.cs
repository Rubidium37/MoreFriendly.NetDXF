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
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace netDxf.IO
{
	internal class TextCodeValueReader :
		ICodeValueReader
	{
		#region private fields

		private readonly TextReader reader;
		private short code;

		#endregion

		#region constructors

		public TextCodeValueReader(TextReader reader)
		{
			this.reader = reader;
			this.code = 0;
			this.Value = null;
			this.CurrentPosition = 0;
		}

		#endregion

		#region public properties

		/// <inheritdoc/>
		public short Code => this.code;

		/// <inheritdoc/>
		public object Value { get; private set; }

		/// <inheritdoc/>
		public long CurrentPosition { get; private set; }

		#endregion

		#region public methods

		/// <inheritdoc/>
		public void Next()
		{
			string readCode = this.reader.ReadLine();
			if (readCode == null)
			{
				this.code = 0;
				this.Value = DxfObjectCode.EndOfFile;
			}
			else
			{
				this.CurrentPosition += 1;
				if (!short.TryParse(readCode, NumberStyles.Integer, CultureInfo.InvariantCulture, out this.code))
				{
					throw new Exception(string.Format("Code {0} not valid at line {1}", this.Code, this.CurrentPosition));
				}
				this.Value = this.ReadValue(this.reader.ReadLine());
				this.CurrentPosition += 1;
			}
		}

		/// <inheritdoc/>
		public byte ReadByte() => (byte)this.Value;

		/// <inheritdoc/>
		public byte[] ReadBytes() => (byte[])this.Value;

		/// <inheritdoc/>
		public short ReadShort() => (short)this.Value;

		/// <inheritdoc/>
		public int ReadInt() => (int)this.Value;

		/// <inheritdoc/>
		public long ReadLong() => (long)this.Value;

		/// <inheritdoc/>
		public bool ReadBool() => (bool)this.Value;

		/// <inheritdoc/>
		public double ReadDouble() => (double)this.Value;

		/// <inheritdoc/>
		public string ReadString() => (string)this.Value;

		/// <inheritdoc/>
		public string ReadHex() => (string)this.Value;

		/// <inheritdoc/>
		public override string ToString()
			=> string.Format("{0}:{1}", this.Code, this.Value);

		#endregion

		#region private methods

		private object ReadValue(string valueString)
		{
			if (this.Code >= 0 && this.Code <= 9) // string
			{
				return this.ReadString(valueString);
			}
			if (this.Code >= 10 && this.Code <= 39) // double precision 3D point value
			{
				return this.ReadDouble(valueString);
			}
			if (this.Code >= 40 && this.Code <= 59) // double precision floating point value
			{
				return this.ReadDouble(valueString);
			}
			if (this.Code >= 60 && this.Code <= 79) // 16-bit integer value
			{
				return this.ReadShort(valueString);
			}
			if (this.Code >= 90 && this.Code <= 99) // 32-bit integer value
			{
				return this.ReadInt(valueString);
			}
			if (this.Code == 100) // string (255-character maximum; less for Unicode strings)
			{
				return this.ReadString(valueString);
			}
			if (this.Code == 101) // string (255-character maximum; less for Unicode strings). This code is undocumented and seems to affect only the AcdsData in dxf version 2013
			{
				return this.ReadString(valueString);
			}
			if (this.Code == 102) // string (255-character maximum; less for Unicode strings)
			{
				return this.ReadString(valueString);
			}
			if (this.Code == 105) // string representing hexadecimal (hex) handle value
			{
				return this.ReadHex(valueString);
			}
			if (this.Code >= 110 && this.Code <= 119) // double precision floating point value
			{
				return this.ReadDouble(valueString);
			}
			if (this.Code >= 120 && this.Code <= 129) // double precision floating point value
			{
				return this.ReadDouble(valueString);
			}
			if (this.Code >= 130 && this.Code <= 139) // double precision floating point value
			{
				return this.ReadDouble(valueString);
			}
			if (this.Code >= 140 && this.Code <= 149) // double precision scalar floating-point value
			{
				return this.ReadDouble(valueString);
			}
			if (this.Code >= 160 && this.Code <= 169) // 64-bit integer value
			{
				return this.ReadLong(valueString);
			}
			if (this.Code >= 170 && this.Code <= 179) // 16-bit integer value
			{
				return this.ReadShort(valueString);
			}
			if (this.Code >= 210 && this.Code <= 239) // double precision scalar floating-point value
			{
				return this.ReadDouble(valueString);
			}
			if (this.Code >= 270 && this.Code <= 279) // 16-bit integer value
			{
				return this.ReadShort(valueString);
			}
			if (this.Code >= 280 && this.Code <= 289) // 16-bit integer value
			{
				return this.ReadShort(valueString);
			}
			if (this.Code >= 290 && this.Code <= 299) // byte (boolean flag value)
			{
				return this.ReadBool(valueString);
			}
			if (this.Code >= 300 && this.Code <= 309) // arbitrary text string
			{
				return this.ReadString(valueString);
			}
			if (this.Code >= 310 && this.Code <= 319) // string representing hex value of binary chunk
			{
				return this.ReadBytes(valueString);
			}
			if (this.Code >= 320 && this.Code <= 329) // string representing hex handle value
			{
				return this.ReadHex(valueString);
			}
			if (this.Code >= 330 && this.Code <= 369) // string representing hex object IDs
			{
				return this.ReadHex(valueString);
			}
			if (this.Code >= 370 && this.Code <= 379) // 16-bit integer value
			{
				return this.ReadShort(valueString);
			}
			if (this.Code >= 380 && this.Code <= 389) // 16-bit integer value
			{
				return this.ReadShort(valueString);
			}
			if (this.Code >= 390 && this.Code <= 399) // string representing hex handle value
			{
				return this.ReadHex(valueString);
			}
			if (this.Code >= 400 && this.Code <= 409) // 16-bit integer value
			{
				return this.ReadShort(valueString);
			}
			if (this.Code >= 410 && this.Code <= 419) // string
			{
				return this.ReadString(valueString);
			}
			if (this.Code >= 420 && this.Code <= 429) // 32-bit integer value
			{
				return this.ReadInt(valueString);
			}
			if (this.Code >= 430 && this.Code <= 439) // string
			{
				return this.ReadString(valueString);
			}
			if (this.Code >= 440 && this.Code <= 449) // 32-bit integer value
			{
				return this.ReadInt(valueString);
			}
			if (this.Code >= 450 && this.Code <= 459) // 32-bit integer value
			{
				return this.ReadInt(valueString);
			}
			if (this.Code >= 460 && this.Code <= 469) // double-precision floating-point value
			{
				return this.ReadDouble(valueString);
			}
			if (this.Code >= 470 && this.Code <= 479) // string
			{
				return this.ReadString(valueString);
			}
			if (this.Code >= 480 && this.Code <= 481) // string representing hex handle value
			{
				return this.ReadHex(valueString);
			}
			if (this.Code == 999) // comment (string)
			{
				return this.ReadString(valueString);
			}
			if (this.Code >= 1010 && this.Code <= 1059) // double-precision floating-point value
			{
				return this.ReadDouble(valueString);
			}
			if (this.Code >= 1000 && this.Code <= 1003) // string (same limits as indicated with 0-9 code range)
			{
				return this.ReadString(valueString);
			}
			if (this.Code == 1004) // string representing hex value of binary chunk
			{
				return this.ReadBytes(valueString);
			}
			if (this.Code >= 1005 && this.Code <= 1009) // string (same limits as indicated with 0-9 code range)
			{
				return this.ReadString(valueString);
			}
			if (this.Code >= 1060 && this.Code <= 1070) // 16-bit integer value
			{
				return this.ReadShort(valueString);
			}
			if (this.Code == 1071) // 32-bit integer value
			{
				return this.ReadInt(valueString);
			}

			throw new Exception(string.Format("Code \"{0}\" not valid at line {1}", this.Code, this.CurrentPosition));
		}

		//private byte ReadByte(string valueString)
		//{
		//	if (byte.TryParse(valueString, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out byte result))
		//	{
		//		return result;
		//	}

		//	Debug.Assert(false, string.Format("Value \"{0}\" not valid at line {1}", valueString, this.CurrentPosition));

		//	return 0;
		//}

		private byte[] ReadBytes(string valueString)
		{
			List<byte> bytes = new List<byte>();
			for (int i = 0; i < valueString.Length; i++)
			{
				string hex = string.Concat(valueString[i], valueString[++i]);
				if (byte.TryParse(hex, NumberStyles.AllowHexSpecifier | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out byte result))
				{
					bytes.Add(result);
				}
				else
				{
					Debug.Assert(false, string.Format("Value \"{0}\" not valid at line {1}", valueString, this.CurrentPosition));

					return new byte[0];
				}
			}

			return bytes.ToArray();
		}

		private short ReadShort(string valueString)
		{
			if (short.TryParse(valueString, NumberStyles.Integer, CultureInfo.InvariantCulture, out short result))
			{
				return result;
			}

			Debug.Assert(false, string.Format("Value \"{0}\" not valid at line {1}", valueString, this.CurrentPosition));

			return 0;
		}

		private int ReadInt(string valueString)
		{
			if (int.TryParse(valueString, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
			{
				return result;
			}

			Debug.Assert(false, string.Format("Value \"{0}\" not valid at line {1}", valueString, this.CurrentPosition));

			return 0;
		}

		private long ReadLong(string valueString)
		{
			if (long.TryParse(valueString, NumberStyles.Integer, CultureInfo.InvariantCulture, out long result))
			{
				return result;
			}

			Debug.Assert(false, string.Format("Value \"{0}\" not valid at line {1}", valueString, this.CurrentPosition));

			return 0;
		}

		private bool ReadBool(string valueString)
		{
			if (byte.TryParse(valueString, NumberStyles.Integer, CultureInfo.InvariantCulture, out byte result))
			{
				return result > 0;
			}

			Debug.Assert(false, string.Format("Value \"{0}\" not valid at line {1}", valueString, this.CurrentPosition));

			return false;
		}

		private double ReadDouble(string valueString)
		{
			if (double.TryParse(valueString, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
			{
				return result;
			}

			Debug.Assert(false, string.Format("Value \"{0}\" not valid at line {1}", valueString, this.CurrentPosition));

			return 0.0;
		}

		private string ReadString(string valueString) => valueString;

		private string ReadHex(string valueString)
		{
			if (long.TryParse(valueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long result))
			{
				return result.ToString("X");
			}

			Debug.Assert(false, string.Format("Value \"{0}\" not valid at line {1}", valueString, this.CurrentPosition));

			return string.Empty;
		}

		#endregion
	}
}