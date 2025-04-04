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
using System.Collections;
using System.Collections.Generic;
using Attribute = netDxf.Entities.Attribute;

namespace netDxf.Collections
{
	/// <summary>Represents a collection of <see cref="Attribute">Attributes</see>.</summary>
	public sealed class AttributeCollection :
		IReadOnlyList<Attribute>
	{
		private readonly List<Attribute> innerArray;

		#region constructor

		/// <summary>Initializes a new instance of the class with the specified collection of attributes.</summary>
		public AttributeCollection()
		{
			this.innerArray = new List<Attribute>();
		}
		/// <summary>Initializes a new instance of the class with the specified collection of attributes.</summary>
		/// <param name="attributes">The collection of attributes from which build the dictionary.</param>
		public AttributeCollection(IEnumerable<Attribute> attributes)
		{
			if (attributes == null)
			{
				throw new ArgumentNullException(nameof(attributes));
			}
			this.innerArray = new List<Attribute>(attributes);
		}

		#endregion

		#region public properties

		/// <inheritdoc/>
		public int Count => this.innerArray.Count;

		/// <summary>Gets a value indicating whether the collection is read-only.</summary>
		public static bool IsReadOnly => true;

		/// <inheritdoc/>
		public Attribute this[int index] => this.innerArray[index];

		#endregion

		#region public methods

		/// <summary>Determines whether an attribute is in the collection.</summary>
		/// <param name="item">The attribute to locate in the collection.</param>
		/// <returns><see langword="true"/> if attribute is found in the collection; otherwise, <see langword="false"/>.</returns>
		public bool Contains(Attribute item) => this.innerArray.Contains(item);

		/// <summary>Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.</summary>
		/// <param name="array">The one-dimensional array that is the destination of the elements copied from the collection. The System.Array must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		public void CopyTo(Attribute[] array, int arrayIndex) => this.innerArray.CopyTo(array, arrayIndex);

		/// <summary>Searches for the specified attribute and returns the zero-based index of the first occurrence within the entire collection.</summary>
		/// <param name="item">The attribute to locate in the collection.</param>
		/// <returns>The zero-based index of the first occurrence of item within the entire collection, if found; otherwise, –1.</returns>
		public int IndexOf(Attribute item) => this.innerArray.IndexOf(item);

		/// <summary>Searches for the first occurrence attribute with the specified attribute definition tag within the entire collection</summary>
		/// <param name="tag"></param>
		/// <returns>The first occurrence of the attribute with the specified attribute definition tag within the entire collection.</returns>
		public Attribute AttributeWithTag(string tag)
		{
			if (string.IsNullOrEmpty(tag))
			{
				return null;
			}
			foreach (Attribute att in this.innerArray)
			{
				if (att.Definition != null)
				{
					if (string.Equals(tag, att.Tag, StringComparison.OrdinalIgnoreCase))
					{
						return att;
					}
				}
			}

			return null;
		}

		/// <inheritdoc/>
		public IEnumerator<Attribute> GetEnumerator() => this.innerArray.GetEnumerator();

		#endregion

		#region private methods

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		#endregion
	}
}