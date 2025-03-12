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
using netDxf.Entities;

namespace netDxf.Collections
{
	/// <summary>Represents the arguments thrown by the <see cref="EntityCollection"/> events.</summary>
	public class EntityCollectionEventArgs :
		EventArgs
	{
		#region constructor

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="item">Item that is being added or removed from the collection.</param>
		public EntityCollectionEventArgs(EntityObject item)
		{
			this.Item = item;
			this.Cancel = false;
		}

		#endregion

		#region public properties

		/// <summary>Get the item that is being added or removed from the collection.</summary>
		public EntityObject Item { get; }

		/// <summary>Gets or sets if the operation must be canceled.</summary>
		/// <remarks>This property is used by the <see cref="EntityCollection.BeforeAddItem"/> and <see cref="EntityCollection.BeforeRemoveItem"/> events to cancel the add or remove operation.</remarks>
		public bool Cancel { get; set; }

		#endregion
	}
}