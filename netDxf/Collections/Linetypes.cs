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
using netDxf.Tables;

namespace netDxf.Collections
{
	/// <summary>Represents a collection of line types.</summary>
	public sealed class Linetypes :
		TableObjects<Linetype>
	{
		#region constructor

		internal Linetypes(DxfDocument document)
			: this(document, null)
		{
		}
		internal Linetypes(DxfDocument document, string handle)
			: base(document, DxfObjectCode.LinetypeTable, handle)
		{
		}

		#endregion

		#region public methods

		/// <summary>Gets the list of linetype names defined in a <b>LIN</b> file.</summary>
		/// <param name="file">Linetype definitions file.</param>
		/// <returns>List of linetype names contained in the specified <b>LIN</b> file.</returns>
		/// <remarks>
		/// If the file is not found in the specified folder, it will try to find them in the list of supported folders defined in the DxfDocument.<br />
		/// </remarks>
		public List<string> NamesFromFile(string file)
		{
			string f = this.Owner.SupportFolders.FindFile(file);
			if (string.IsNullOrEmpty(f))
			{
				throw new FileNotFoundException("The file has not been found.", file);
			}

			return Linetype.NamesFromFile(f);
		}

		/// <summary>Adds all linetypes to the list from the definition in a <b>LIN</b> file.</summary>
		/// <param name="file">File where the definition is located.</param>
		/// <param name="reload">Specifies if the linetype definitions of the file will overwrite the existing ones, in case another with the same name exists in the file.</param>
		/// <remarks>
		/// If the file is not found in the specified folder, it will try to find them in the list of supported folders defined in the DxfDocument.<br />
		/// Any text style or shape present in the linetype definition must be previously defined in the DxfDocument, if not an exception will be generated.
		/// </remarks>
		public void AddFromFile(string file, bool reload)
		{
			string f = this.Owner.SupportFolders.FindFile(file);
			if (string.IsNullOrEmpty(f))
			{
				throw new FileNotFoundException("The LIN file has not been found.", file);
			}

			List<string> names = Linetype.NamesFromFile(f);
			foreach (string name in names)
			{
				this.AddFromFile(f, name, reload);
			}
		}

		/// <summary>Adds a linetype to the list from the definition in a <b>LIN</b> file.</summary>
		/// <param name="file">File where the definition is located.</param>
		/// <param name="linetypeName">Name of the line type definition to read (ignore case).</param>
		/// <param name="reload">Specifies if the linetype definition of the file will overwrite the existing one, in case another with the same name exists in the file.</param>
		/// <returns>
		/// <see langword="true"/> if the linetype has been added from the linetype definitions <b>LIN</b> file; <see langword="false"/> otherwise.
		/// It will return false if the linetype is present in the file and the reload argument is <see langword="false"/>.
		/// </returns>
		/// <remarks>
		/// If the file is not found in the specified folder, it will try to find them in the list of supported folders defined in the DxfDocument.<br />
		/// Any text style or shape present in the linetype definition must be previously defined in the DxfDocument, if not an exception will be generated.
		/// </remarks>
		public bool AddFromFile(string file, string linetypeName, bool reload)
		{
			string f = this.Owner.SupportFolders.FindFile(file);
			if (string.IsNullOrEmpty(f))
			{
				throw new FileNotFoundException("The LIN file has not been found.", file);
			}

			Linetype linetype = Linetype.Load(f, linetypeName);

			if (linetype == null)
			{
				return false;
			}

			if (this.TryGetValue(linetype.Name, out Linetype existing))
			{
				if (!reload)
				{
					return false;
				}

				existing.Description = linetype.Description;
				existing.Segments.Clear();
				existing.Segments.AddRange(linetype.Segments);
				return true;
			}

			this.Add(linetype);
			return true;
		}

		/// <summary>Saves all linetype definitions to a <b>LIN</b> file.</summary>
		/// <param name="file">File where the linetype definitions will be saved.</param>
		/// <param name="overwrite">Defines if the file will be overwritten in case exits another one.</param>
		/// <remarks>Only non reserved linetypes will be saved, therefore Continuous, <b>ByLayer</b>, and <b>ByBlock</b> will be excluded.</remarks>
		public void Save(string file, bool overwrite)
		{
			if (overwrite) File.Delete(file);
			foreach (Linetype lt in this.List.Values)
			{
				if (!lt.IsReserved)
				{
					lt.Save(file);
				}
			}
		}

		#endregion

		#region override methods

		/// <inheritdoc/>
		internal override Linetype Add(Linetype linetype, bool assignHandle)
		{
			if (linetype == null)
			{
				throw new ArgumentNullException(nameof(linetype));
			}

			if (this.List.TryGetValue(linetype.Name, out Linetype add))
			{
				return add;
			}

			if (assignHandle || string.IsNullOrEmpty(linetype.Handle))
			{
				this.Owner.NumHandles = linetype.AssignHandle(this.Owner.NumHandles);
			}

			foreach (LinetypeSegment segment in linetype.Segments)
			{
				if (segment.Type == LinetypeSegmentType.Text)
				{
					LinetypeTextSegment textSegment = (LinetypeTextSegment)segment;
					textSegment.Style = this.Owner.TextStyles.Add(textSegment.Style);
					this.Owner.TextStyles.References[textSegment.Style.Name].Add(linetype);
				}
				if (segment.Type == LinetypeSegmentType.Shape)
				{
					LinetypeShapeSegment shapeSegment = (LinetypeShapeSegment)segment;
					shapeSegment.Style = this.Owner.ShapeStyles.Add(shapeSegment.Style);
					this.Owner.ShapeStyles.References[shapeSegment.Style.Name].Add(linetype);
					//TODO: shape names and indexes, require check to external SHX file
					//if (!shapeSegment.Style.ContainsShapeName(shapeSegment.Name))
					//{
					//	throw new ArgumentException("The linetype contains a shape segment which style does not contain a shape with the stored name.", nameof(linetype));
					//}
				}
			}

			this.List.Add(linetype.Name, linetype);
			this.References.Add(linetype.Name, new DxfObjectReferences());

			linetype.Owner = this;

			linetype.NameChanged += this.Item_NameChanged;
			linetype.AfterAddingLinetypeSegment += this.Linetype_AfterAddingLinetypeSegment;
			linetype.AfterRemovingLinetypeSegment += this.Linetype_AfterRemovingLinetypeSegment;
			linetype.BeforeChangingTextStyleValue += this.Linetype_BeforeChangingTextStyleValue;
			linetype.BeforeChangingShapeStyleValue += this.Linetype_BeforeChangingShapeStyleValue;

			this.Owner.AddedObjects.Add(linetype.Handle, linetype);

			return linetype;
		}

		/// <inheritdoc/>
		public override bool Remove(string name) => this.Remove(this[name]);
		/// <inheritdoc/>
		public override bool Remove(Linetype item)
		{
			if (item == null)
			{
				return false;
			}

			if (!this.Contains(item))
			{
				return false;
			}

			if (item.IsReserved)
			{
				return false;
			}

			if (this.HasReferences(item))
			{
				return false;
			}

			LinetypeSegment[] segments = new LinetypeSegment[item.Segments.Count];
			item.Segments.CopyTo(segments, 0);
			item.Segments.Remove(segments);

			this.Owner.AddedObjects.Remove(item.Handle);
			this.References.Remove(item.Name);
			this.List.Remove(item.Name);

			item.Handle = null;
			item.Owner = null;

			item.NameChanged -= this.Item_NameChanged;
			item.AfterAddingLinetypeSegment -= this.Linetype_AfterAddingLinetypeSegment;
			item.AfterRemovingLinetypeSegment -= this.Linetype_AfterRemovingLinetypeSegment;
			item.BeforeChangingTextStyleValue -= this.Linetype_BeforeChangingTextStyleValue;
			item.BeforeChangingShapeStyleValue -= this.Linetype_BeforeChangingShapeStyleValue;

			return true;
		}

		#endregion

		#region Linetype events

		private void Item_NameChanged(object sender, AfterValueChangeEventArgs<String> e)
		{
			if (this.Contains(e.NewValue))
			{
				throw new ArgumentException("There is already another line type with the same name.");
			}

			this.List.Remove(e.OldValue);
			this.List.Add(e.NewValue, (Linetype)sender);

			var refs = this.GetReferences(e.OldValue);
			this.References.Remove(e.OldValue);
			this.References.Add(e.NewValue, new DxfObjectReferences());
			this.References[e.NewValue].Add(refs);
		}

		private void Linetype_AfterAddingLinetypeSegment(object sender, AfterItemChangeEventArgs<LinetypeSegment> e)
		{
			if (sender is not Linetype senderT)
				return;

			if (e.Item.Type == LinetypeSegmentType.Text)
			{
				LinetypeTextSegment textSegment = (LinetypeTextSegment)e.Item;
				textSegment.Style = this.Owner.TextStyles.Add(textSegment.Style);
				this.Owner.TextStyles.References[textSegment.Style.Name].Add(senderT);
			}

			if (e.Item.Type == LinetypeSegmentType.Shape)
			{
				LinetypeShapeSegment shapeSegment = (LinetypeShapeSegment)e.Item;
				shapeSegment.Style = this.Owner.ShapeStyles.Add(shapeSegment.Style);
				this.Owner.ShapeStyles.References[shapeSegment.Style.Name].Add(senderT);
			}
		}

		private void Linetype_AfterRemovingLinetypeSegment(object sender, AfterItemChangeEventArgs<LinetypeSegment> e)
		{
			if (sender is not Linetype senderT)
				return;

			if (e.Item.Type == LinetypeSegmentType.Text)
			{
				this.Owner.TextStyles.References[((LinetypeTextSegment)e.Item).Style.Name].Remove(senderT);
			}
			if (e.Item.Type == LinetypeSegmentType.Shape)
			{
				this.Owner.ShapeStyles.References[((LinetypeShapeSegment)e.Item).Style.Name].Remove(senderT);
			}
		}

		private void Linetype_BeforeChangingTextStyleValue(object sender, BeforeValueChangeEventArgs<TextStyle> e)
		{
			var senderT = (DxfObject)sender;
			this.Owner.TextStyles.References[e.OldValue.Name].Remove(senderT);
			e.NewValue = this.Owner.TextStyles.Add(e.NewValue);
			this.Owner.TextStyles.References[e.NewValue.Name].Add(senderT);
		}

		private void Linetype_BeforeChangingShapeStyleValue(object sender, BeforeValueChangeEventArgs<ShapeStyle> e)
		{
			var senderT = (DxfObject)sender;
			this.Owner.ShapeStyles.References[e.OldValue.Name].Remove(senderT);
			e.NewValue = this.Owner.ShapeStyles.Add(e.NewValue);
			this.Owner.ShapeStyles.References[e.NewValue.Name].Add(senderT);
		}

		#endregion
	}
}