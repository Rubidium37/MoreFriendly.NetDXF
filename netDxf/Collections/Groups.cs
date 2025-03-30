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
using netDxf.Entities;
using netDxf.Objects;
using netDxf.Tables;

namespace netDxf.Collections
{
	/// <summary>Represents a collection of groups.</summary>
	public sealed class Groups :
		TableObjects<Group>
	{
		#region constructor

		internal Groups(DxfDocument document)
			: this(document, null)
		{
		}
		internal Groups(DxfDocument document, string handle)
			: base(document, DxfObjectCode.GroupDictionary, handle)
		{
		}

		#endregion

		#region override methods

		/// <inheritdoc/>
		internal override Group Add(Group group, bool assignHandle)
		{
			if (group == null)
			{
				throw new ArgumentNullException(nameof(group));
			}

			// if no name has been given to the group a generic name will be created
			if (group.IsUnnamed && string.IsNullOrEmpty(group.Name))
			{
				group.SetName("*A" + this.Owner.GroupNamesIndex++, false);
			}

			if (this.List.TryGetValue(group.Name, out Group add))
			{
				return add;
			}

			if (assignHandle || string.IsNullOrEmpty(group.Handle))
			{
				this.Owner.NumHandles = group.AssignHandle(this.Owner.NumHandles);
			}

			this.List.Add(group.Name, group);
			this.References.Add(group.Name, new DxfObjectReferences());
			foreach (EntityObject entity in group.Entities)
			{
				if (entity.Owner != null)
				{
					// the group and its entities must belong to the same document
					if (!ReferenceEquals(entity.Owner.Owner.Owner.Owner, this.Owner))
					{
						throw new ArgumentException("The group and their entities must belong to the same document. Clone them instead.");
					}
				}
				else
				{
					// only entities not owned by anyone need to be added
					this.Owner.Entities.Add(entity);
				}
				this.References[group.Name].Add(entity);
			}

			group.Owner = this;

			group.NameChanged += this.Item_NameChanged;
			group.AfterAddingEntityObject += this.Group_AfterAddingEntityObject;
			group.AfterRemovingEntityObject += this.Group_AfterRemovingEntityObject;

			this.Owner.AddedObjects.Add(group.Handle, group);

			return group;
		}

		/// <inheritdoc/>
		public override bool Remove(string name) => this.Remove(this[name]);
		/// <inheritdoc/>
		public override bool Remove(Group item)
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

			foreach (EntityObject entity in item.Entities)
			{
				entity.RemoveReactor(item);
			}

			this.Owner.AddedObjects.Remove(item.Handle);
			this.References.Remove(item.Name);
			this.List.Remove(item.Name);

			item.Handle = null;
			item.Owner = null;

			item.NameChanged -= this.Item_NameChanged;
			item.AfterAddingEntityObject -= this.Group_AfterAddingEntityObject;
			item.AfterRemovingEntityObject -= this.Group_AfterRemovingEntityObject;

			return true;
		}

		#endregion

		#region Group events

		private void Item_NameChanged(object sender, AfterValueChangeEventArgs<String> e)
		{
			if (this.Contains(e.NewValue))
			{
				throw new ArgumentException("There is already another dimension style with the same name.");
			}

			this.List.Remove(e.OldValue);
			this.List.Add(e.NewValue, (Group)sender);

			var refs = this.GetReferences(e.OldValue);
			this.References.Remove(e.OldValue);
			this.References.Add(e.NewValue, new DxfObjectReferences());
			this.References[e.NewValue].Add(refs);
		}

		void Group_AfterAddingEntityObject(object sender, AfterItemChangeEventArgs<EntityObject> e)
		{
			if (sender is not Group senderT)
				return;

			if (e.Item.Owner != null)
			{
				// the group and its entities must belong to the same document
				if (!ReferenceEquals(e.Item.Owner.Owner.Owner.Owner, this.Owner))
				{
					throw new ArgumentException("The group and the entity must belong to the same document. Clone it instead.");
				}
			}
			else
			{
				// only entities not owned by anyone will be added
				this.Owner.Entities.Add(e.Item);
			}

			this.References[senderT.Name].Add(e.Item);
		}

		void Group_AfterRemovingEntityObject(object sender, AfterItemChangeEventArgs<EntityObject> e)
		{
			if (sender is not Group senderT)
				return;

			this.References[senderT.Name].Remove(e.Item);
		}

		#endregion
	}
}