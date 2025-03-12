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

using System.Collections.Generic;
using netDxf.Collections;
using netDxf.Entities;
using netDxf.Tables;

namespace netDxf.Objects
{
	/// <summary>Represents a group of entities.</summary>
	public class Group :
		TableObject
	{
		#region delegates and events

		public delegate void EntityAddedEventHandler(Group sender, GroupEntityChangeEventArgs e);
		public event EntityAddedEventHandler EntityAdded;
		protected virtual void OnEntityAddedEvent(EntityObject item)
		{
			EntityAddedEventHandler ae = this.EntityAdded;
			if (ae != null)
				ae(this, new GroupEntityChangeEventArgs(item));
		}

		public delegate void EntityRemovedEventHandler(Group sender, GroupEntityChangeEventArgs e);
		public event EntityRemovedEventHandler EntityRemoved;
		protected virtual void OnEntityRemovedEvent(EntityObject item)
		{
			EntityRemovedEventHandler ae = this.EntityRemoved;
			if (ae != null)
				ae(this, new GroupEntityChangeEventArgs(item));
		}

		#endregion

		#region constructor

		/// <summary>Initialized a new unnamed empty group.</summary>
		/// <remarks>
		/// A unique name will be generated when the group is added to the document.
		/// </remarks>
		public Group()
			: this(string.Empty)
		{
		}
		/// <summary>Initialized a new empty group.</summary>
		/// <param name="name">Group name.</param>
		/// <remarks>
		/// If the name is set to <see langword="null"/> or empty, a unique name will be generated when the group is added to the document.
		/// </remarks>
		public Group(string name)
			: this(name, null)
		{
		}
		/// <summary>Initialized a new group with the specified entities.</summary>
		/// <param name="entities">The list of entities contained in the group.</param>
		/// <remarks>
		/// A unique name will be generated when the group is added to the document.
		/// </remarks>
		public Group(IEnumerable<EntityObject> entities)
			: this(string.Empty, entities)
		{
		}
		/// <summary>Initialized a new group with the specified entities.</summary>
		/// <param name="name">Group name (optional).</param>
		/// <param name="entities">The list of entities contained in the group.</param>
		/// <remarks>
		/// If the name is set to <see langword="null"/> or empty, a unique name will be generated when the group is added to the document.
		/// </remarks>
		public Group(string name, IEnumerable<EntityObject> entities)
			: base(name, DxfObjectCode.Group, !string.IsNullOrEmpty(name))
		{
			this.IsUnnamed = string.IsNullOrEmpty(name);
			this.Description = string.Empty;
			this.IsSelectable = true;
			this.Entities = new EntityCollection();
			this.Entities.BeforeAddItem += this.Entities_BeforeAddItem;
			this.Entities.AddItem += this.Entities_AddItem;
			this.Entities.BeforeRemoveItem += this.Entities_BeforeRemoveItem;
			this.Entities.RemoveItem += this.Entities_RemoveItem;
			if (entities != null)
			{
				this.Entities.AddRange(entities);
			}
		}
		internal Group(string name, bool checkName)
			: base(name, DxfObjectCode.Group, checkName)
		{
			this.IsUnnamed = string.IsNullOrEmpty(name) || name.StartsWith("*");
			this.Description = string.Empty;
			this.IsSelectable = true;
			this.Entities = new EntityCollection();
			this.Entities.BeforeAddItem += this.Entities_BeforeAddItem;
			this.Entities.AddItem += this.Entities_AddItem;
			this.Entities.BeforeRemoveItem += this.Entities_BeforeRemoveItem;
			this.Entities.RemoveItem += this.Entities_RemoveItem;
		}

		#endregion

		#region public properties

		/// <summary>Gets the name of the table object.</summary>
		/// <remarks>Table object names are case insensitive.</remarks>
		public new string Name
		{
			get => base.Name;
			set
			{
				base.Name = value;
				this.IsUnnamed = false;
			}
		}

		/// <summary>Gets or sets the description of the group.</summary>
		public string Description { get; set; }

		/// <summary>Gets if the group has an automatic generated name.</summary>
		public bool IsUnnamed { get; internal set; }

		/// <summary>Gets or sets if the group is selectable.</summary>
		public bool IsSelectable { get; set; }

		/// <summary>Gets the list of entities contained in the group.</summary>
		/// <remarks>
		/// When the group is added to the document the entities in it will be automatically added too.<br/>
		/// An entity may be contained in different groups.
		/// </remarks>
		public EntityCollection Entities { get; }

		/// <summary>Gets the owner of the actual <b>DXF</b> object.</summary>
		public new Groups Owner
		{
			get => (Groups)base.Owner;
			internal set => base.Owner = value;
		}

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override bool HasReferences() => this.Owner != null && this.Owner.HasReferences(this.Name);

		/// <inheritdoc/>
		public override List<DxfObjectReference> GetReferences()
		{
			if (this.Owner == null)
			{
				return null;
			}

			return this.Owner.GetReferences(this.Name);
		}

		/// <inheritdoc/>
		public override TableObject Clone(string newName)
		{
			EntityObject[] refs = new EntityObject[this.Entities.Count];
			for (int i = 0; i < this.Entities.Count; i++)
			{
				refs[i] = (EntityObject)this.Entities[i].Clone();
			}

			Group copy = new Group(newName, refs)
			{
				Description = this.Description,
				IsSelectable = this.IsSelectable
			};

			foreach (XData data in this.XData.Values)
			{
				copy.XData.Add((XData)data.Clone());
			}

			return copy;
		}
		/// <inheritdoc/>
		public override object Clone() => this.Clone(this.IsUnnamed ? string.Empty : this.Name);

		#endregion

		#region Entities collection events

		private void Entities_BeforeAddItem(EntityCollection sender, EntityCollectionEventArgs e)
		{
			// null or duplicate items are not allowed in the entities list.
			if (e.Item == null)
			{
				e.Cancel = true;
			}
			else if (this.Entities.Contains(e.Item))
			{
				e.Cancel = true;
			}
			else
			{
				e.Cancel = false;
			}
		}

		private void Entities_AddItem(EntityCollection sender, EntityCollectionEventArgs e)
		{
			e.Item.AddReactor(this);
			this.OnEntityAddedEvent(e.Item);
		}

		private void Entities_BeforeRemoveItem(EntityCollection sender, EntityCollectionEventArgs e)
		{
		}

		private void Entities_RemoveItem(EntityCollection sender, EntityCollectionEventArgs e)
		{
			e.Item.RemoveReactor(this);
			this.OnEntityRemovedEvent(e.Item);
		}

		#endregion
	}
}