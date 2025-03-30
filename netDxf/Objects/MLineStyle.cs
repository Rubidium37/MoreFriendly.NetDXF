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
using System.Runtime.CompilerServices;
using netDxf.Collections;
using netDxf.Tables;

namespace netDxf.Objects
{
	/// <summary>Represents as <b>MLine</b> style.</summary>
	public class MLineStyle :
		TableObject
	{
		#region delegates and events

		/// <summary>Generated when an <see cref="MLineStyleElement"/> item has been added.</summary>
		public event AfterItemChangeEventHandler<MLineStyleElement> AfterAddingMLineStyleElement;
		/// <summary>Generates the <see cref="AfterAddingMLineStyleElement"/> event.</summary>
		/// <param name="item">The item being added.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		protected virtual void OnAfterAddingMLineStyleElement(MLineStyleElement item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterAddingMLineStyleElement is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Add, item));
		}

		/// <summary>Generated when an <see cref="MLineStyleElement"/> item has been removed.</summary>
		public event AfterItemChangeEventHandler<MLineStyleElement> AfterRemovingMLineStyleElement;
		/// <summary>Generates the <see cref="AfterRemovingMLineStyleElement"/> event.</summary>
		/// <param name="item">The item being removed.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		protected virtual void OnAfterRemovingMLineStyleElement(MLineStyleElement item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterRemovingMLineStyleElement is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Remove, item));
		}

		/// <summary>Generated when a property of <see cref="Linetype"/> type changes.</summary>
		public event BeforeValueChangeEventHandler<Linetype> BeforeChangingLinetypeValue;
		/// <summary>Generates the <see cref="BeforeChangingLinetypeValue"/> event.</summary>
		/// <param name="oldValue">The old value, being changed.</param>
		/// <param name="newValue">The new value, that will replace the old one.</param>
		/// <param name="propertyName">(automatic) Name of the affected property.</param>
		protected virtual Linetype OnBeforeChangingLinetypeValue(Linetype oldValue, Linetype newValue, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeChangingLinetypeValue is { } handler)
			{
				var e = new BeforeValueChangeEventArgs<Linetype>(propertyName, oldValue, newValue);
				handler(this, e);
				return e.NewValue;
			}
			return newValue;
		}

		#endregion

		#region constants

		/// <summary>Default multiline style name.</summary>
		public const string DefaultName = "Standard";

		/// <summary>Gets the default <b>MLine</b> style.</summary>
		public static MLineStyle Default => new MLineStyle(DefaultName);

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">MLine style name.</param>
		/// <remarks>By default the multiline style has to elements with offsets 0.5 y -0.5.</remarks>
		public MLineStyle(string name)
			: this(name, null, string.Empty)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">MLine style name.</param>
		/// <param name="description">MLine style description.</param>
		/// <remarks>By default the multiline style has to elements with offsets 0.5 y -0.5.</remarks>
		public MLineStyle(string name, string description)
			: this(name, null, description)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">MLine style name.</param>
		/// <param name="elements">Elements of the multiline, if <see langword="null"/> two default elements will be added.</param>
		public MLineStyle(string name, IEnumerable<MLineStyleElement> elements)
			: this(name, elements, string.Empty)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">MLine style name.</param>
		/// <param name="elements">Elements of the multiline, if <see langword="null"/> two default elements will be added.</param>
		/// <param name="description">MLine style description (optional).</param>
		public MLineStyle(string name, IEnumerable<MLineStyleElement> elements, string description)
			: base(name, DxfObjectCode.MLineStyle, true)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name), "The multiline style name should be at least one character long.");
			}

			_Description = string.IsNullOrEmpty(description) ? string.Empty : description;

			this.Elements.BeforeAddingItem += this.Elements_BeforeAddingItem;
			this.Elements.AfterAddingItem += this.Elements_AfterAddingItem;
			this.Elements.BeforeRemovingItem += this.Elements_BeforeRemovingItem;
			this.Elements.AfterRemovingItem += this.Elements_AfterRemovingItem;
			this.Elements.AddRange(elements ?? new[] { new MLineStyleElement(0.5), new MLineStyleElement(-0.5) });
			this.Elements.Sort(); // the elements list must be ordered

			if (this.Elements.Count < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(elements), this.Elements.Count, "The elements list must have at least one element.");
			}
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets the <b>MLine</b> style flags.</summary>
		public MLineStyleFlags Flags { get; set; } = MLineStyleFlags.None;

		private string _Description;
		/// <summary>Gets or sets the line type description (optional).</summary>
		public string Description
		{
			get => _Description;
			set => _Description = string.IsNullOrEmpty(value) ? string.Empty : value;
		}

		private AciColor _FillColor = AciColor.ByLayer;
		/// <summary>Gets or sets the <b>MLine</b> fill color.</summary>
		/// <remarks>
		/// AutoCad2000 <b>DXF</b> version does not support <see langword="true"/> colors for MLineStyle fill color.
		/// </remarks>
		public AciColor FillColor
		{
			get => _FillColor;
			set => _FillColor = value ?? throw new ArgumentNullException(nameof(value));
		}

		private double _StartAngle = 90.0;
		/// <summary>Gets or sets the <b>MLine</b> start angle in degrees.</summary>
		/// <remarks>Valid values range from 10.0 to 170.0 degrees. Default: 90.0.</remarks>
		public double StartAngle
		{
			get => _StartAngle;
			set
			{
				if (value < 10.0 || value > 170.0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The MLine style start angle valid values range from 10 to 170 degrees.");
				}
				_StartAngle = value;
			}
		}

		private double _EndAngle = 90.0;
		/// <summary>Gets or sets the <b>MLine</b> end angle in degrees.</summary>
		/// <remarks>Valid values range from 10.0 to 170.0 degrees. Default: 90.0.</remarks>
		public double EndAngle
		{
			get => _EndAngle;
			set
			{
				if (value < 10.0 || value > 170.0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The MLine style end angle valid values range from 10 to 170 degrees.");
				}
				_EndAngle = value;
			}
		}

		/// <summary>Gets the list of elements that make up the multiline.</summary>
		/// <remarks>
		/// The elements list must be ordered from larger to smaller <see cref="MLineStyleElement.Offset">offset</see> values.
		/// During the initialization process the list will be sorted automatically,
		/// but if new elements are added individually to the list or the offset values of individual elements are modified,
		/// it will have to be sorted manually calling the Sort() method.
		/// </remarks>
		public ObservableCollection<MLineStyleElement> Elements { get; } = new ObservableCollection<MLineStyleElement>();

		/// <summary>Gets the owner of the actual multi line style.</summary>
		public new MLineStyles Owner
		{
			get => (MLineStyles)base.Owner;
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
			List<MLineStyleElement> copyElements = new List<MLineStyleElement>();
			foreach (MLineStyleElement e in this.Elements)
			{
				copyElements.Add((MLineStyleElement)e.Clone());
			}

			MLineStyle copy = new MLineStyle(newName, copyElements)
			{
				Flags = this.Flags,
				Description = _Description,
				FillColor = (AciColor)_FillColor.Clone(),
				StartAngle = _StartAngle,
				EndAngle = _EndAngle,
			};

			foreach (XData data in this.XData.Values)
			{
				copy.XData.Add((XData)data.Clone());
			}

			return copy;
		}
		/// <inheritdoc/>
		public override object Clone() => this.Clone(this.Name);

		#endregion

		#region Elements collection events

		private void Elements_BeforeAddingItem(object sender, BeforeItemChangeEventArgs<MLineStyleElement> e)
		{
			// null items are not allowed
			if (e.Item == null)
			{
				e.Cancel = true;
			}
			else
			{
				e.Cancel = false;
			}
		}

		private void Elements_AfterAddingItem(object sender, AfterItemChangeEventArgs<MLineStyleElement> e)
		{
			this.OnAfterAddingMLineStyleElement(e.Item, $"{nameof(this.Elements)}.{e.PropertyName}");
			e.Item.BeforeChangingLinetypeValue += this.Elements_Item_BeforeChangingLinetypeValue;
		}

		private void Elements_BeforeRemovingItem(object sender, BeforeItemChangeEventArgs<MLineStyleElement> e)
		{
		}

		private void Elements_AfterRemovingItem(object sender, AfterItemChangeEventArgs<MLineStyleElement> e)
		{
			this.OnAfterRemovingMLineStyleElement(e.Item, $"{nameof(this.Elements)}.{e.PropertyName}");
			e.Item.BeforeChangingLinetypeValue -= this.Elements_Item_BeforeChangingLinetypeValue;
		}

		#endregion

		#region MLineStyleElement events

		private void Elements_Item_BeforeChangingLinetypeValue(object sender, BeforeValueChangeEventArgs<Linetype> e)
			=> e.NewValue = this.OnBeforeChangingLinetypeValue(e.OldValue, e.NewValue, $"{nameof(this.Elements)}.{e.PropertyName}");

		#endregion
	}
}