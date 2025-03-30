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
using System.Runtime.CompilerServices;
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

		/// <summary>Generated when an <see cref="EntityObject"/> item has been added.</summary>
		public event AfterItemChangeEventHandler<EntityObject> AfterAddingEntityObject;
		/// <summary>Generates the <see cref="AfterAddingEntityObject"/> event.</summary>
		/// <param name="item">The item being added.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		protected virtual void OnAfterAddingEntityObject(EntityObject item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterAddingEntityObject is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Add, item));
		}

		/// <summary>Generated when an <see cref="EntityObject"/> item has been removed.</summary>
		public event AfterItemChangeEventHandler<EntityObject> AfterRemovingEntityObject;
		/// <summary>Generates the <see cref="AfterRemovingEntityObject"/> event.</summary>
		/// <param name="item">The item being removed.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		protected virtual void OnAfterRemovingEntityObject(EntityObject item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterRemovingEntityObject is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Remove, item));
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
			this.Entities.BeforeAddingItem += this.Entities_BeforeAddingItem;
			this.Entities.AfterAddingItem += this.Entities_AfterAddingItem;
			this.Entities.BeforeRemovingItem += this.Entities_BeforeRemovingItem;
			this.Entities.AfterRemovingItem += this.Entities_AfterRemovingItem;
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
			this.Entities.BeforeAddingItem += this.Entities_BeforeAddingItem;
			this.Entities.AfterAddingItem += this.Entities_AfterAddingItem;
			this.Entities.BeforeRemovingItem += this.Entities_BeforeRemovingItem;
			this.Entities.AfterRemovingItem += this.Entities_AfterRemovingItem;
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

		private void Entities_BeforeAddingItem(object sender, BeforeItemChangeEventArgs<EntityObject> e)
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

		private void Entities_AfterAddingItem(object sender, AfterItemChangeEventArgs<EntityObject> e)
		{
			e.Item.AddReactor(this);
			this.OnAfterAddingEntityObject(e.Item, $"{nameof(this.Entities)}.{e.PropertyName}");
		}

		private void Entities_BeforeRemovingItem(object sender, BeforeItemChangeEventArgs<EntityObject> e)
		{
		}

		private void Entities_AfterRemovingItem(object sender, AfterItemChangeEventArgs<EntityObject> e)
		{
			e.Item.RemoveReactor(this);
			this.OnAfterRemovingEntityObject(e.Item, $"{nameof(this.Entities)}.{e.PropertyName}");
		}

		#endregion
	}
}