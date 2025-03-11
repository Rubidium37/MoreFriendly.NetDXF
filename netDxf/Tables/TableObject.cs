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
using System.Linq;

namespace netDxf.Tables
{
	/// <summary>Defines classes that can be accessed by name. They are usually part of the <b>DXF TABLE</b> section but can also be part of the <b>OBJECTS</b> section.</summary>
	public abstract class TableObject :
		DxfObject,
		ICloneable,
		IComparable,
		IComparable<TableObject>,
		IEquatable<TableObject>
	{
		#region delegates and events

		public delegate void NameChangedEventHandler(TableObject sender, TableObjectChangedEventArgs<string> e);
		public event NameChangedEventHandler NameChanged;
		protected virtual void OnNameChangedEvent(string oldName, string newName)
		{
			NameChangedEventHandler ae = this.NameChanged;
			if (ae != null)
			{
				TableObjectChangedEventArgs<string> eventArgs = new TableObjectChangedEventArgs<string>(oldName, newName);
				ae(this, eventArgs);
			}
		}

		#endregion

		#region private fields

		private static readonly char[] invalidCharacters = { '\\', '/', ':', '*', '?', '"', '<', '>', '|', ';', ',', '=', '`' };
		private bool reserved;
		private string name;

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">Table name. The following characters \&lt;&gt;/?":;*|,=` are not supported for table object names.</param>
		/// <param name="codeName">Table <see cref="DxfObjectCode">code name</see>.</param>
		/// <param name="checkName">Defines if the table object name needs to be checked for invalid characters.</param>
		protected TableObject(string name, string codeName, bool checkName)
			: base(codeName)
		{
			name = name.Trim();
			if (checkName)
			{
				if (!IsValidName(name))
				{
					throw new ArgumentException("The name should be at least one character long and the following characters \\<>/?\":;*|,=` are not supported.", nameof(name));
				}
			}

			this.name = name;
			this.reserved = false;
		}

		#endregion

		#region public properties

		/// <summary>Gets the name of the table object.</summary>
		/// <remarks>Table object names are case insensitive.</remarks>
		public string Name
		{
			get { return this.name; }
			set { this.SetName(value, true); }
		}

		/// <summary>Gets if the table object is reserved and cannot be deleted.</summary>
		public bool IsReserved
		{
			get { return this.reserved; }
			internal set { this.reserved = value; }
		}

		/// <summary>Gets the array of characters not supported as table object names.</summary>
		public static char[] InvalidCharacters
		{
			get { return invalidCharacters.ToArray(); }
		}

		#endregion

		#region public methods

		/// <summary>Checks if a string is valid as a table object name.</summary>
		/// <param name="name">String to check.</param>
		/// <returns><see langword="true"/> if the string is valid as a table object name; otherwise, <see langword="false"/>.</returns>
		public static bool IsValidName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return false;
			}

			return name.IndexOfAny(invalidCharacters) == -1;
		}

		/// <summary>Checks if this instance has been referenced by other <see cref="DxfObject"/>s.</summary>
		/// <returns>
		/// Returns <see langword="true"/> if this instance has been referenced by other <see cref="DxfObject"/>s; otherwise, <see langword="false"/>.
		/// It will always return <see langword="false"/> if this instance does not belong to a document.
		/// </returns>
		/// <remarks>
		/// This method returns the same value as the <see cref="HasReferences"/> method that can be found in the <see cref="TableObject"/>s class.
		/// </remarks>
		public abstract bool HasReferences();

		/// <summary>Gets the list of <see cref="DxfObject"/>s referenced by this instance.</summary>
		/// <returns>
		/// A list of <see cref="DxfObjectReference"/> that contains the <see cref="DxfObject"/> referenced by this instance and the number of times it does.
		/// It will return <see langword="null"/> if this instance does not belong to a document.
		/// </returns>
		/// <remarks>
		/// This method returns the same list as the <see cref="GetReferences"/> method that can be found in the <see cref="TableObject"/>s class.
		/// </remarks>
		public abstract List<DxfObjectReference> GetReferences();

		#endregion

		#region internal methods

		internal void SetName(string newName, bool checkName)
		{
			// Hack to change the table name without having to check its name.
			// Some invalid characters are used for internal purposes only.
			if (string.IsNullOrEmpty(newName))
			{
				throw new ArgumentNullException(nameof(newName));
			}

			if (this.IsReserved)
			{
				throw new ArgumentException("Reserved table objects cannot be renamed.", nameof(newName));
			}

			if (string.Equals(this.name, newName, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}

			if (checkName)
			{
				if (!IsValidName(newName))
				{
					throw new ArgumentException("The following characters \\<>/?\":;*|,=` are not supported for table object names.", nameof(newName));
				}
			}
			this.OnNameChangedEvent(this.name, newName);
			this.name = newName;
		}

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override string ToString()
		{
			return this.Name;
		}

		#endregion

		#region implements IComparable

		/// <inheritdoc/>
		/// <remarks>If both table objects are no of the same type it will return zero. The comparison is made by their names.</remarks>
		public int CompareTo(object other)
		{
			return this.CompareTo((TableObject)other);
		}

		/// <inheritdoc/>
		/// <remarks>If both table objects are not of the same type it will return zero. The comparison is made by their names.</remarks>
		public int CompareTo(TableObject other)
		{
			if (other == null)
			{
				throw new ArgumentNullException(nameof(other));
			}

			return this.GetType() == other.GetType() ? string.Compare(this.Name, other.Name, StringComparison.OrdinalIgnoreCase) : 0;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		///// <summary>
		///// Check if the tables are equal.
		///// </summary>
		///// <param name="u">TableObject.</param>
		///// <param name="v">TableObject.</param>
		///// <returns><see langword="true"/> if the two table names are equal, <see langword="false"/> in any other case.</returns>
		//public static bool operator ==(TableObject u, TableObject v)
		//{
		//	if (ReferenceEquals(u, null) && ReferenceEquals(v, null))
		//		return true;

		//	if (ReferenceEquals(u, null) || ReferenceEquals(v, null))
		//		return false;

		//	return string.Equals(u.Name, v.Name, StringComparison.OrdinalIgnoreCase);
		//}

		///// <summary>
		///// Check if the tables are different.
		///// </summary>
		///// <param name="u">TableObject.</param>
		///// <param name="v">TableObject.</param>
		///// <returns><see langword="true"/> if the two table names are different, <see langword="false"/> in any other case.</returns>
		//public static bool operator !=(TableObject u, TableObject v)
		//{
		//	if (ReferenceEquals(u, null) && ReferenceEquals(v, null))
		//		return false;

		//	if (ReferenceEquals(u, null) || ReferenceEquals(v, null))
		//		return true;

		//	return !string.Equals(u.Name, v.Name, StringComparison.OrdinalIgnoreCase);
		//}

		///// <summary>
		///// Check if the first table is lesser than the second.
		///// </summary>
		///// <param name="u">TableObject.</param>
		///// <param name="v">TableObject.</param>
		///// <returns><see langword="true"/> if the first table name is lesser than the second, <see langword="false"/> in any other case.</returns>
		//public static bool operator <(TableObject u, TableObject v)
		//{
		//	if (ReferenceEquals(u, null) || ReferenceEquals(v, null))
		//		return false;

		//	return string.Compare(u.Name, v.Name, StringComparison.OrdinalIgnoreCase) < 0;
		//}

		///// <summary>
		///// Check if first table is greater than the second.
		///// </summary>
		///// <param name="u">TableObject.</param>
		///// <param name="v">TableObject.</param>
		///// <returns><see langword="true"/> if the first table name is greater than the second, <see langword="false"/> in any other case.</returns>
		//public static bool operator >(TableObject u, TableObject v)
		//{
		//	if (ReferenceEquals(u, null) || ReferenceEquals(v, null))
		//		return false;

		//	return string.Compare(u.Name, v.Name, StringComparison.OrdinalIgnoreCase) > 0;
		//}

		#endregion

		#region implements IEquatable

		/// <inheritdoc/>
		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}

			if (this.GetType() != other.GetType())
			{
				return false;
			}

			return this.Equals((TableObject)other);
		}
		/// <inheritdoc/>
		public bool Equals(TableObject other)
		{
			if (other == null)
			{
				return false;
			}

			return string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);
		}

		#endregion

		#region ICloneable

		/// <inheritdoc/>
		public abstract object Clone();
		/// <summary>Creates a new table object that is a copy of the current instance.</summary>
		/// <param name="newName">TableObject name of the copy.</param>
		/// <returns>A new table object that is a copy of this instance.</returns>
		public abstract TableObject Clone(string newName);

		#endregion

	}
}