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
using System.Text;
using netDxf.Collections;

namespace netDxf.Tables
{
	/// <summary>Represent a shape style.</summary>
	public class ShapeStyle :
		TableObject
	{
		#region private fields

		private string shapeFile;

		#endregion

		#region constants

		/// <summary>Default text style font.</summary>
		public const string DefaultShapeFile = "ltypeshp.shx";

		/// <summary>Gets the default shape style.</summary>
		/// <remarks>AutoCad stores the shapes for the predefined complex linetypes in the ltypeshp.shx file.</remarks>
		internal static ShapeStyle Default => new ShapeStyle("ltypeshp", ShapeStyle.DefaultShapeFile);

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">Shape style name.</param>
		/// <param name="file">Shape definitions <b>SHX</b> file.</param>
		public ShapeStyle(string name, string file)
			: this(name, file, 0.0, 1.0, 0.0)
		{
		}

		internal ShapeStyle(string name, string file, double size, double widthFactor, double obliqueAngle)
			: base(name, DxfObjectCode.TextStyle, true)
		{
			if (string.IsNullOrEmpty(file))
			{
				throw new ArgumentNullException(nameof(file));
			}

			if (file.IndexOfAny(Path.GetInvalidPathChars()) == 0)
			{
				throw new ArgumentException("File path contains invalid characters.", nameof(file));
			}

			this.shapeFile = file;
			this.Size = size;
			this.WidthFactor = widthFactor;
			this.ObliqueAngle = obliqueAngle;
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets the shape <b>SHX</b> file name.</summary>
		public string File
		{
			get => this.shapeFile;
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

				this.shapeFile = value;
			}
		}

		/// <summary>Gets the shape size.</summary>
		/// <remarks>This value seems to have no effect on shapes or complex line types with shapes. Default: 0.0.</remarks>
		public double Size { get; }

		/// <summary>Gets the shape width factor.</summary>
		/// <remarks>This value seems to have no effect on shapes or complex line types with shapes. Default: 1.0.</remarks>
		public double WidthFactor { get; }

		/// <summary>Gets the shape oblique angle in degrees.</summary>
		/// <remarks>This value seems to have no effect on shapes or complex line types with shapes. Default: 0.0.</remarks>
		public double ObliqueAngle { get; }

		/// <summary>Gets the owner of the actual shape style.</summary>
		public new ShapeStyles Owner
		{
			get => (ShapeStyles)base.Owner;
			internal set => base.Owner = value;
		}

		#endregion

		#region public methods

		/// <summary>Gets the list of shapes names defined in a <b>SHX</b> file.</summary>
		/// <param name="file">Shape <b>SHX</b> file.</param>
		/// <returns>List of shape names contained in the specified <b>SHX</b> file.</returns>
		public static List<string> NamesFromFile(string file)
		{
			List<string> names = new List<string>();

			if (string.IsNullOrEmpty(file))
			{
				throw new ArgumentNullException(nameof(file));
			}

			if (!string.Equals(Path.GetExtension(file), ".SHX", StringComparison.InvariantCultureIgnoreCase))
			{
				throw new ArgumentException("The shape file must have the extension SHX.", nameof(file));
			}

			using (BinaryReader reader = new BinaryReader(System.IO.File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
			{
				Encoding encoding = new ASCIIEncoding();

				byte[] sentinel = reader.ReadBytes(24);
				StringBuilder sb = new StringBuilder(21);
				for (int i = 0; i < 21; i++)
				{
					sb.Append((char)sentinel[i]);
				}

				if (sb.ToString() != "AutoCAD-86 shapes 1.0")
				{
					throw new ArgumentException("Not a valid Shape binary file .SHX.", nameof(file));
				}

				reader.ReadInt16(); // first shape number
				reader.ReadInt16(); // last shape number
				short num = reader.ReadInt16(); // number of entries in file

				short[] numbers = new short[num];
				short[] numBytes = new short[num]; // includes the number of bytes of the shape name as a null terminated string

				for (int i = 0; i < num; i++)
				{
					numbers[i] = reader.ReadInt16();
					numBytes[i] = reader.ReadInt16();
				}

				for (int i = 0; i < num; i++)
				{
					names.Add(NullTerminatedString(reader, encoding));
					reader.ReadBytes(numBytes[i] - (names[i].Length + 1)); // these bytes holds the shape geometry
				}
			}

			return names;
		}

		/// <summary>Checks if the shape <b>SHP</b> file contains a shape with the specified name.</summary>
		/// <param name="file">Shape <b>SHX</b> file.</param>
		/// <param name="shapeName">Shape name.</param>
		/// <returns><see langword="true"/> if the shape <b>SHX</b> file that contains a shape with the specified name; otherwise, <see langword="false"/>.</returns>
		public static bool ContainsShapeName(string file, string shapeName)
		{
			List<string> names = NamesFromFile(file);
			foreach (string s in names)
			{
				if (s.Equals(shapeName, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>Gets the list of shapes names defined in the actual shape style (the shape <b>SHX</b> file must be accessible).</summary>
		/// <returns>List of shape names contained in the actual shape style.</returns>
		/// <remarks>
		/// If the actual shape style belongs to a document, it will look for the <b>SHX</b> file also in the document support folders.
		/// </remarks>
		public List<string> NamesFromShapeStyle()
		{
			string f = this.shapeFile;
			if (this.Owner != null)
			{
				f = this.Owner.Owner.SupportFolders.FindFile(f);
			}

			if (string.IsNullOrEmpty(f) || !System.IO.File.Exists(f))
			{
				return new List<string>();
			}

			return NamesFromFile(f);
		}

		/// <summary>Checks if the actual shape style contains a shape with the specified name (the shape <b>SHX</b> file must be accessible).</summary>
		/// <param name="name">Shape name.</param>
		/// <returns><see langword="true"/> if the shape style that contains a shape with the specified name; otherwise, <see langword="false"/>.</returns>
		/// <remarks>If the actual shape style belongs to a document, it will look for the <b>SHX</b> file also in the document support folders.</remarks>
		public bool ContainsShapeName(string name)
		{
			List<string> names = this.NamesFromShapeStyle();
			foreach (string s in names)
			{
				if (s.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}

			return false;
		}

		#endregion

		#region internal methods

		/// <summary>Gets the number of the shape with the specified name.</summary>
		/// <param name="name">Name of the shape.</param>
		/// <returns>The number of the shape, 0 in case the shape has not been found.</returns>
		/// <remarks>If the actual shape style belongs to a document, it will look for the <b>SHX</b> file also in the document support folders.</remarks>
		public short ShapeNumber(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return 0;
			}

			string f = this.shapeFile;
			if (this.Owner != null)
			{
				f = this.Owner.Owner.SupportFolders.FindFile(f);
			}
			else
			{
				if (!System.IO.File.Exists(f))
				{
					f = string.Empty;
				}
			}

			if (string.IsNullOrEmpty(f))
			{
				return 0;
			}

			using (BinaryReader reader = new BinaryReader(System.IO.File.Open(f, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
			{
				Encoding encoding = new ASCIIEncoding();

				byte[] sentinel = reader.ReadBytes(24); // the use of the last three bytes is unknown, the first 21 hold the file signature
				StringBuilder sb = new StringBuilder(21);
				for (int i = 0; i < 21; i++)
				{
					sb.Append((char)sentinel[i]);
				}

				if (sb.ToString() != "AutoCAD-86 shapes 1.0")
				{
					throw new ArgumentException("Not a valid Shape binary file .SHX.", nameof(f));
				}

				reader.ReadInt16(); // first shape number
				reader.ReadInt16(); // last shape number
				short num = reader.ReadInt16(); // number of entries in file

				short[] numbers = new short[num];
				short[] numBytes = new short[num]; // includes the number of bytes of the shape name as a null terminated string

				for (int i = 0; i < num; i++)
				{
					numbers[i] = reader.ReadInt16();
					numBytes[i] = reader.ReadInt16();
				}

				for (int i = 0; i < num; i++)
				{
					string n = NullTerminatedString(reader, encoding);
					if (name.Equals(n, StringComparison.InvariantCultureIgnoreCase))
					{
						return numbers[i];
					}
					reader.ReadBytes(numBytes[i] - (n.Length + 1)); // these bytes holds the shape geometry
				}
			}

			return 0;
		}

		/// <summary>Gets the name of the shape with the specified number.</summary>
		/// <param name="number">Number of the shape.</param>
		/// <returns>The name of the shape, empty in case the shape has not been found.</returns>
		/// <remarks>If the actual shape style belongs to a document, it will look for the <b>SHX</b> file also in the document support folders.</remarks>
		public string ShapeName(short number)
		{
			string f = this.shapeFile;
			if (this.Owner != null)
			{
				f = this.Owner.Owner.SupportFolders.FindFile(f);
			}
			else
			{
				if (!System.IO.File.Exists(f))
				{
					f = string.Empty;
				}
			}

			if (string.IsNullOrEmpty(f))
			{
				return string.Empty;
			}

			using (BinaryReader reader = new BinaryReader(System.IO.File.Open(f, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
			{
				Encoding encoding = new ASCIIEncoding();

				byte[] sentinel = reader.ReadBytes(24); // the use of the last three bytes is unknown, the first 21 hold the file signature
				StringBuilder sb = new StringBuilder(21);
				for (int i = 0; i < 21; i++)
				{
					sb.Append((char)sentinel[i]);
				}

				if (sb.ToString() != "AutoCAD-86 shapes 1.0")
				{
					throw new ArgumentException("Not a valid Shape binary file .SHX.", nameof(f));
				}

				reader.ReadInt16(); // first shape number
				reader.ReadInt16(); // last shape number
				short num = reader.ReadInt16(); // number of entries in file

				short[] numbers = new short[num];
				short[] numBytes = new short[num]; // includes the number of bytes of the shape name as a null terminated string

				int index = -1;
				for (int i = 0; i < num; i++)
				{
					short n = reader.ReadInt16();
					if (n == number)
					{
						index = i;
					}
					numbers[i] = n;

					numBytes[i] = reader.ReadInt16();
				}

				for (int i = 0; i < num; i++)
				{
					string name = NullTerminatedString(reader, encoding);
					if (index == i)
					{
						return name;
					}
					reader.ReadBytes(numBytes[i] - (name.Length + 1)); // these bytes holds the shape geometry
				}
			}

			return string.Empty;
		}

		#endregion

		#region private methods

		private static string NullTerminatedString(BinaryReader reader, Encoding encoding)
		{
			byte c = reader.ReadByte();
			List<byte> bytes = new List<byte>();
			while (c != 0) // strings always end with a 0 byte (char NULL)
			{
				bytes.Add(c);
				c = reader.ReadByte();
			}
			return encoding.GetString(bytes.ToArray(), 0, bytes.Count);
		}

		#endregion

		#region Methods to read the shape info from .SHP files

		#region public methods

		///// <summary>
		///// Gets the list of shapes names defined in a <b>SHP</b> file.
		///// </summary>
		///// <param name="file">Shape <b>SHP</b> file.</param>
		///// <returns>List of shape names contained in the specified <b>SHP</b> file.</returns>
		//public static List<string> NamesFromFile(string file)
		//{
		//	List<string> names = new List<string>();

		//	if (string.IsNullOrEmpty(file))
		//	{
		//		throw new ArgumentNullException(nameof(file));
		//	}

		//	if (!string.Equals(Path.GetExtension(file), ".SHP", StringComparison.InvariantCultureIgnoreCase))
		//	{
		//		throw new ArgumentException("The shape file must have the extension SHP.", nameof(file));
		//	}

		//	using (StreamReader reader = new StreamReader(System.IO.File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), true))
		//	{
		//		while (!reader.EndOfStream)
		//		{
		//			string line = reader.ReadLine();
		//			if (line == null)
		//			{
		//				throw new FileLoadException("Unknown error reading SHP file.", file);
		//			}

		//			// lines starting with semicolons are comments
		//			if (line.StartsWith(";"))
		//			{
		//				continue;
		//			}

		//			// every shape definition starts with '*'
		//			if (!line.StartsWith("*"))
		//			{
		//				continue;
		//			}

		//			string[] tokens = line.TrimStart('*').Split(',');
		//			names.Add(tokens[2]);
		//		}
		//	}

		//	return names;
		//}

		///// <summary>
		///// Checks if the shape <b>SHP</b> file contains a shape with the specified name.
		///// </summary>
		///// <param name="file">Shape <b>SHP</b> file.</param>
		///// <param name="shapeName">Shape name.</param>
		///// <returns><see langword="true"/> if the shape <b>SHP</b> file that contains a shape with the specified name; otherwise, <see langword="false"/>.</returns>
		//public static bool ContainsShapeName(string file, string shapeName)
		//{
		//	if (string.IsNullOrEmpty(file))
		//	{
		//		throw new ArgumentNullException(nameof(file));
		//	}

		//	if (!string.Equals(Path.GetExtension(file), ".SHP", StringComparison.InvariantCultureIgnoreCase))
		//	{
		//		throw new ArgumentException("The shape file must have the extension SHP.", nameof(file));
		//	}

		//	if (string.IsNullOrEmpty(shapeName))
		//	{
		//		return false;
		//	}

		//	using (StreamReader reader = new StreamReader(System.IO.File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), true))
		//	{
		//		while (!reader.EndOfStream)
		//		{
		//			string line = reader.ReadLine();
		//			if (line == null)
		//			{
		//				throw new FileLoadException("Unknown error reading SHP file.", file);
		//			}
		//			// lines starting with semicolons are comments
		//			if (line.StartsWith(";"))
		//			{
		//				continue;
		//			}
		//			// every shape definition starts with '*'
		//			if (!line.StartsWith("*"))
		//			{
		//				continue;
		//			}

		//			string[] tokens = line.TrimStart('*').Split(',');
		//			if (string.Equals(shapeName, tokens[2], StringComparison.InvariantCultureIgnoreCase))
		//			{
		//				//the shape style that contains a shape with the specified name has been found
		//				return true;
		//			}
		//		}
		//	}
		//	// there are no shape styles that contain a shape with the specified name
		//	return false;
		//}

		///// <summary>
		///// Gets the list of shapes names defined in the actual shape style (the shape <b>SHP</b> file must be accessible).
		///// </summary>
		///// <returns>List of shape names contained in the actual shape style.</returns>
		///// <remarks>
		///// If the actual shape style belongs to a document, it will look for the <b>SHP</b> file also in the document support folders.
		///// </remarks>
		//public List<string> NamesFromShapeStyle(string file)
		//{
		//	string f = Path.ChangeExtension(this.shapeFile, "SHP");
		//	if (this.Owner != null)
		//	{
		//		f = this.Owner.Owner.SupportFolders.FindFile(f);
		//	}
		//	else
		//	{
		//		if (!System.IO.File.Exists(f)) f = string.Empty;
		//	}

		//	// we will look for the shape name in the SHP file
		//	if (string.IsNullOrEmpty(f))
		//	{
		//		return new List<string>();
		//	}

		//	return NamesFromFile(f);
		//}

		///// <summary>
		///// Checks if the actual shape style contains a shape with the specified name (the shape <b>SHP</b> file must be accessible).
		///// </summary>
		///// <param name="name">Shape name.</param>
		///// <returns><see langword="true"/> if the shape style that contains a shape with the specified name; otherwise, <see langword="false"/>.</returns>
		///// <remarks>If the actual shape style belongs to a document, it will look for the <b>SHP</b> file also in the document support folders.</remarks>
		//public bool ContainsShapeName(string name)
		//{
		//	if (string.IsNullOrEmpty(name))
		//	{
		//		return false;
		//	}

		//	string f = Path.ChangeExtension(this.shapeFile, "SHP");
		//	if (this.Owner != null)
		//	{
		//		f = this.Owner.Owner.SupportFolders.FindFile(f);
		//	}
		//	else
		//	{
		//		if (!System.IO.File.Exists(f)) f = string.Empty;
		//	}

		//	// we will look for the shape name in the SHP file
		//	if (string.IsNullOrEmpty(f))
		//	{
		//		return false;
		//	}

		//	return ContainsShapeName(f, name);

		//}

		#endregion

		#region internal methods

		///// <summary>
		///// Gets the number of the shape with the specified name.
		///// </summary>
		///// <param name="name">Name of the shape.</param>
		///// <returns>The number of the shape, 0 in case the shape has not been found.</returns>
		///// <remarks>If the actual shape style belongs to a document, it will look for the <b>SHP</b> file also in the document support folders.</remarks>
		//internal short ShapeNumber(string name)
		//{
		//	if (string.IsNullOrEmpty(name))
		//	{
		//		return 0;
		//	}

		//	// we will look for the shape name in the SHP file
		//	string f = Path.ChangeExtension(this.shapeFile, "SHP");
		//	if (this.Owner != null)
		//	{
		//		f = this.Owner.Owner.SupportFolders.FindFile(f);
		//	}
		//	else
		//	{
		//		if (!System.IO.File.Exists(f)) f = string.Empty;
		//	}

		//	if (string.IsNullOrEmpty(f))
		//	{
		//		return 0;
		//	}

		//	using (StreamReader reader = new StreamReader(System.IO.File.Open(f, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), true))
		//	{
		//		while (!reader.EndOfStream)
		//		{
		//			string line = reader.ReadLine();
		//			if (line == null)
		//			{
		//				throw new FileLoadException("Unknown error reading SHP file.", f);
		//			}

		//			// lines starting with semicolons are comments
		//			if (line.StartsWith(";"))
		//			{
		//				continue;
		//			}

		//			// every shape definition starts with '*'
		//			if (!line.StartsWith("*"))
		//			{
		//				continue;
		//			}

		//			string[] tokens = line.TrimStart('*').Split(',');
		//			// the third item is the name of the shape
		//			if (string.Equals(tokens[2], name, StringComparison.InvariantCultureIgnoreCase))
		//			{
		//				return short.Parse(tokens[0]);
		//			}
		//		}
		//	}

		//	return 0;
		//}

		///// <summary>
		///// Gets the name of the shape with the specified number.
		///// </summary>
		///// <param name="number">Number of the shape.</param>
		///// <returns>The name of the shape, empty in case the shape has not been found.</returns>
		///// <remarks>If the actual shape style belongs to a document, it will look for the <b>SHP</b> file also in the document support folders.</remarks>
		//internal string ShapeName(short number)
		//{
		//	// we will look for the shape name in the SHP file
		//	string f = Path.ChangeExtension(this.shapeFile, "SHP");
		//	if (this.Owner != null)
		//	{
		//		f = this.Owner.Owner.SupportFolders.FindFile(f);
		//	}
		//	else
		//	{
		//		if (!System.IO.File.Exists(f)) f = string.Empty;
		//	}

		//	if (string.IsNullOrEmpty(f)) return string.Empty;

		//	using (StreamReader reader = new StreamReader(System.IO.File.Open(f, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), true))
		//	{
		//		while (!reader.EndOfStream)
		//		{
		//			string line = reader.ReadLine();
		//			if (line == null)
		//			{
		//				throw new FileLoadException("Unknown error reading SHP file.", f);
		//			}

		//			// lines starting with semicolons are comments
		//			if (line.StartsWith(";"))
		//			{
		//				continue;
		//			}

		//			// every shape definition starts with '*'
		//			if (!line.StartsWith("*"))
		//			{
		//				continue;
		//			}

		//			string[] tokens = line.TrimStart('*').Split(',');
		//			// the first item is the number of the shape
		//			if (short.Parse(tokens[0]) == number)
		//			{
		//				return tokens[2];
		//			}
		//		}
		//	}

		//	return string.Empty;
		//}

		#endregion

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override bool HasReferences() => this.Owner != null && this.Owner.HasReferences(this.Name);

		/// <inheritdoc/>
		public override List<DxfObjectReference> GetReferences() => this.Owner?.GetReferences(this.Name);

		/// <inheritdoc/>
		public override TableObject Clone(string newName)
		{
			ShapeStyle copy = new ShapeStyle(newName, this.shapeFile, this.Size, this.WidthFactor, this.ObliqueAngle);

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