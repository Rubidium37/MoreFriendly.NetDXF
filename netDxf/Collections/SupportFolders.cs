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
using System.IO;

namespace netDxf.Collections
{
	/// <summary>Represents a list of support folders for the document.</summary>
	public class SupportFolders :
		IList<string>
	{
		private readonly List<string> folders;

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		public SupportFolders()
		{
			this.folders = new List<string>();
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="folders">The collection whose elements should be added to the list. The items in the collection cannot be <see langword="null"/>.</param>
		public SupportFolders(IEnumerable<string> folders)
		{
			if (folders == null)
			{
				throw new ArgumentNullException(nameof(folders));
			}
			this.folders = new List<string>(folders);
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets the base folder to resolver relative paths of external references.</summary>
		/// <remarks>By default it points to the current <see cref="System.Environment.CurrentDirectory"/> when the <see cref="DxfDocument"/> was created.</remarks>
		public string WorkingFolder { get; set; } = Environment.CurrentDirectory;

		#endregion

		#region public methods

		/// <summary>Looks for a file in one of the support folders.</summary>
		/// <param name="file">File name to find in one of the support folders.</param>
		/// <returns>The path to the file found in one of the support folders. It includes both the path and the specified file name.</returns>
		/// <remarks>If the specified file already exists it return the same value, if neither it cannot be found in any of the support folders it will return an empty string.</remarks>
		public string FindFile(string file)
		{
			string foundFile = string.Empty;

			string currentDirectory = Environment.CurrentDirectory;
			Environment.CurrentDirectory = this.WorkingFolder;

			if (File.Exists(file))
			{
				foundFile = Path.GetFullPath(file);
			}

			string name = Path.GetFileName(file);

			foreach (string folder in this.folders)
			{
				string newFile = string.Format("{0}{1}{2}", folder, Path.DirectorySeparatorChar, name);
				if (File.Exists(newFile))
				{
					foundFile = Path.GetFullPath(newFile);
				}
			}

			Environment.CurrentDirectory = currentDirectory;
			return foundFile;
		}

		#endregion

		#region implements IList<string>

		/// <inheritdoc/>
		public string this[int index]
		{
			get => this.folders[index];
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException(nameof(value));
				}
				this.folders[index] = value;
			}
		}

		/// <inheritdoc/>
		public int Count => this.folders.Count;

		/// <inheritdoc/>
		public bool IsReadOnly => false;

		/// <inheritdoc/>
		public IEnumerator<string> GetEnumerator() => this.folders.GetEnumerator();

		/// <summary>Returns an enumerator that iterates through the list.</summary>
		/// <returns>The enumerator for the list.</returns>
		IEnumerator IEnumerable.GetEnumerator() => this.folders.GetEnumerator();

		/// <inheritdoc/>
		public void Add(string item)
		{
			if (string.IsNullOrEmpty(item))
			{
				throw new ArgumentNullException(nameof(item));
			}
			this.folders.Add(item);
		}

		/// <summary>Adds the elements of the collection to the list.</summary>
		/// <param name="collection">The collection whose elements should be added to the end of the list. The items in the collection cannot be <see langword="null"/>.</param>
		public void AddRange(IEnumerable<string> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException(nameof(collection));
			}
			foreach (string s in collection)
			{
				this.folders.Add(s);
			}
		}

		/// <inheritdoc/>
		public void Clear() => this.folders.Clear();

		/// <inheritdoc/>
		public bool Contains(string item)
		{
			if (string.IsNullOrEmpty(item))
			{
				throw new ArgumentNullException(nameof(item));
			}
			return this.folders.Contains(item);
		}

		/// <inheritdoc/>
		public void CopyTo(string[] array, int arrayIndex) => this.folders.CopyTo(array, arrayIndex);

		/// <inheritdoc/>
		public bool Remove(string item)
		{
			if (string.IsNullOrEmpty(item))
			{
				throw new ArgumentNullException(nameof(item));
			}
			return this.folders.Remove(item);
		}

		/// <inheritdoc/>
		public int IndexOf(string item) => this.folders.IndexOf(item);

		/// <inheritdoc/>
		public void Insert(int index, string item)
		{
			if (string.IsNullOrEmpty(item))
			{
				throw new ArgumentNullException(nameof(item));
			}
			this.folders.Insert(index, item);
		}

		/// <inheritdoc/>
		public void RemoveAt(int index) => this.folders.RemoveAt(index);

		#endregion
	}
}