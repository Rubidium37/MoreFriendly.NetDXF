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

namespace netDxf.Header
{
	/// <summary>Defines a header variable.</summary>
	public class HeaderVariable
	{
		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">Header variable name.</param>
		/// <param name="groupCode">Header variable group code.</param>
		/// <param name="value">Header variable value.</param>
		/// <remarks>
		/// It is very important to match the group code with its corresponding value type,
		/// check the <b>DXF</b> documentation for details about what group code correspond to its associated type.
		/// For example, typical groups codes are 70, 40, and 2 that correspond to short, double, and string value types, respectively.<br />
		/// If the header value is a <see cref="Vector3"/> use the group code 30, if it is a <see cref="Vector2"/> use group code 20,
		/// when the variable is written to the <b>DXF</b> the codes 10, 20, and 30 will be added as necessary.
		/// </remarks>
		public HeaderVariable(string name, short groupCode, object value)
		{
			if (!name.StartsWith("$", StringComparison.InvariantCultureIgnoreCase))
				throw new ArgumentException("Header variable names always starts with '$'", nameof(name));
			this.Name = name;
			this.GroupCode = groupCode;
			this.Value = value;
		}

		#endregion

		#region public properties

		/// <summary>Gets the header variable name.</summary>
		/// <remarks>The header variable name is case insensitive.</remarks>
		public string Name { get; }

		/// <summary>Gets the header variable group code.</summary>
		public short GroupCode { get; }

		/// <summary>Gets the header variable stored value.</summary>
		/// <remarks>
		/// It is very important to match the group code with its corresponding value type,
		/// check the <b>DXF</b> documentation for details about what group code correspond to its associated type.
		/// For example, typical groups codes are 70, 40, and 2 that correspond to short, double, and string value types, respectively.<br />
		/// If the header value is a <see cref="Vector3"/> use the group code 30, if it is a <see cref="Vector2"/> use group code 20,
		/// when the variable is written to the <b>DXF</b> the codes 10, 20, and 30 will be added as necessary.
		/// </remarks>
		public object Value { get; set; }

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override string ToString()
			=> string.Format("{0}:{1}", this.Name, this.Value);

		#endregion
	}
}