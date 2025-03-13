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
using System.Text;

namespace netDxf.IO
{
	internal class BinaryCodeValueReader :
		ICodeValueReader
	{
		private readonly BinaryReader reader;
		private readonly Encoding encoding;

		#region constructors

		public BinaryCodeValueReader(BinaryReader reader, Encoding encoding)
		{
			this.reader = reader;
			this.encoding = encoding;
			byte[] sentinel = this.reader.ReadBytes(22);
			StringBuilder sb = new StringBuilder(18);
			for (int i = 0; i < 18; i++)
			{
				sb.Append((char)sentinel[i]);
			}

			if (sb.ToString() != "AutoCAD Binary DXF")
			{
				throw new ArgumentException("Not a valid binary DXF.");
			}
		}

		#endregion

		#region public properties

		/// <inheritdoc/>
		public short Code { get; private set; } = 0;

		/// <inheritdoc/>
		public object Value { get; private set; }

		/// <inheritdoc/>
		public long CurrentPosition => this.reader.BaseStream.Position;

		#endregion

		#region public methods

		/// <inheritdoc/>
		public void Next()
		{
			this.Code = this.reader.ReadInt16();

			if (this.Code >= 0 && this.Code <= 9) // string
			{
				this.Value = this.NullTerminatedString();
			}
			else if (this.Code >= 10 && this.Code <= 39) // double precision 3D point value
			{
				this.Value = this.reader.ReadDouble();
			}
			else if (this.Code >= 40 && this.Code <= 59) // double precision floating point value
			{
				this.Value = this.reader.ReadDouble();
			}
			else if (this.Code >= 60 && this.Code <= 79) // 16-bit integer value
			{
				this.Value = this.reader.ReadInt16();
			}
			else if (this.Code >= 90 && this.Code <= 99) // 32-bit integer value
			{
				this.Value = this.reader.ReadInt32();
			}
			else if (this.Code == 100) // string (255-character maximum; less for Unicode strings)
			{
				this.Value = this.NullTerminatedString();
			}
			else if (this.Code == 101) // string (255-character maximum; less for Unicode strings). This code is undocumented and seems to affect only the AcdsData in dxf version 2013
			{
				this.Value = this.NullTerminatedString();
			}
			else if (this.Code == 102) // string (255-character maximum; less for Unicode strings)
			{
				this.Value = this.NullTerminatedString();
			}
			else if (this.Code == 105) // string representing hexadecimal (hex) handle value
			{
				this.Value = this.ReadHex(this.NullTerminatedString());
			}
			else if (this.Code >= 110 && this.Code <= 119) // double precision floating point value
			{
				this.Value = this.reader.ReadDouble();
			}
			else if (this.Code >= 120 && this.Code <= 129) // double precision floating point value
			{
				this.Value = this.reader.ReadDouble();
			}
			else if (this.Code >= 130 && this.Code <= 139) // double precision floating point value
			{
				this.Value = this.reader.ReadDouble();
			}
			else if (this.Code >= 140 && this.Code <= 149) // double precision scalar floating-point value
			{
				this.Value = this.reader.ReadDouble();
			}
			else if (this.Code >= 160 && this.Code <= 169) // 64-bit integer value
			{
				this.Value = this.reader.ReadInt64();
			}
			else if (this.Code >= 170 && this.Code <= 179) // 16-bit integer value
			{
				this.Value = this.reader.ReadInt16();
			}
			else if (this.Code >= 210 && this.Code <= 239) // double precision scalar floating-point value
			{
				this.Value = this.reader.ReadDouble();
			}
			else if (this.Code >= 270 && this.Code <= 279) // 16-bit integer value
			{
				this.Value = this.reader.ReadInt16();
			}
			else if (this.Code >= 280 && this.Code <= 289) // 16-bit integer value
			{
				this.Value = this.reader.ReadInt16();
			}
			else if (this.Code >= 290 && this.Code <= 299) // byte (boolean flag value)
			{
				this.Value = this.reader.ReadByte() > 0;
			}
			else if (this.Code >= 300 && this.Code <= 309) // arbitrary text string
			{
				this.Value = this.NullTerminatedString();
			}
			else if (this.Code >= 310 && this.Code <= 319) // string representing hex value of binary chunk
			{
				this.Value = this.ReadBinaryData();
			}
			else if (this.Code >= 320 && this.Code <= 329) // string representing hex handle value
			{
				this.Value = this.ReadHex(this.NullTerminatedString());
			}
			else if (this.Code >= 330 && this.Code <= 369) // string representing hex object IDs
			{
				this.Value = this.ReadHex(this.NullTerminatedString());
			}
			else if (this.Code >= 370 && this.Code <= 379) // 16-bit integer value
			{
				this.Value = this.reader.ReadInt16();
			}
			else if (this.Code >= 380 && this.Code <= 389) // 16-bit integer value
			{
				this.Value = this.reader.ReadInt16();
			}
			else if (this.Code >= 390 && this.Code <= 399) // string representing hex handle value
			{
				this.Value = this.ReadHex(this.NullTerminatedString());
			}
			else if (this.Code >= 400 && this.Code <= 409) // 16-bit integer value
			{
				this.Value = this.reader.ReadInt16();
			}
			else if (this.Code >= 410 && this.Code <= 419) // string
			{
				this.Value = this.NullTerminatedString();
			}
			else if (this.Code >= 420 && this.Code <= 429) // 32-bit integer value
			{
				this.Value = this.reader.ReadInt32();
			}
			else if (this.Code >= 430 && this.Code <= 439) // string
			{
				this.Value = this.NullTerminatedString();
			}
			else if (this.Code >= 440 && this.Code <= 449) // 32-bit integer value
			{
				this.Value = this.reader.ReadInt32();
			}
			else if (this.Code >= 450 && this.Code <= 459) // 32-bit integer value
			{
				this.Value = this.reader.ReadInt32();
			}
			else if (this.Code >= 460 && this.Code <= 469) // double-precision floating-point value
			{
				this.Value = this.reader.ReadDouble();
			}
			else if (this.Code >= 470 && this.Code <= 479) // string
			{
				this.Value = this.NullTerminatedString();
			}
			else if (this.Code >= 480 && this.Code <= 481) // string representing hex handle value
			{
				this.Value = this.ReadHex(this.NullTerminatedString());
			}
			else if (this.Code == 999) // comment (string)
			{
				throw new Exception(string.Format("The comment group, 999, is not used in binary DXF files at byte address {0}", this.reader.BaseStream.Position));
			}
			else if (this.Code >= 1010 && this.Code <= 1059) // double-precision floating-point value
			{
				this.Value = this.reader.ReadDouble();
			}
			else if (this.Code >= 1000 && this.Code <= 1003) // string (same limits as indicated with 0-9 code range)
			{
				this.Value = this.NullTerminatedString();
			}
			else if (this.Code == 1004) // string representing hex value of binary chunk
			{
				this.Value = this.ReadBinaryData();
			}
			else if (this.Code >= 1005 && this.Code <= 1009) // string (same limits as indicated with 0-9 code range)
			{
				this.Value = this.NullTerminatedString();
			}
			else if (this.Code >= 1060 && this.Code <= 1070) // 16-bit integer value
			{
				this.Value = this.reader.ReadInt16();
			}
			else if (this.Code == 1071) // 32-bit integer value
			{
				this.Value = this.reader.ReadInt32();
			}
			else
			{
				throw new Exception(string.Format("Code {0} not valid at byte address {1}", this.Code, this.reader.BaseStream.Position));
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

		private byte[] ReadBinaryData()
		{
			byte length = this.reader.ReadByte();
			return this.reader.ReadBytes(length);
		}

		private string NullTerminatedString()
		{
			byte c = this.reader.ReadByte();
			List<byte> bytes = new List<byte>();
			while (c != 0) // strings always end with a 0 byte (char NULL)
			{
				bytes.Add(c);
				c = this.reader.ReadByte();
			}
			return this.encoding.GetString(bytes.ToArray(), 0, bytes.Count);
		}

		private string ReadHex(string hex)
		{
			if (long.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long result))
			{
				return result.ToString("X");
			}

			Debug.Assert(false, string.Format("Value \"{0}\" not valid at line {1}", hex, this.CurrentPosition));

			return String.Empty;

		}

		#endregion
	}
}