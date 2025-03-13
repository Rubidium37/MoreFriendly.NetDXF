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
using netDxf.Collections;

namespace netDxf.Tables
{
	/// <summary>Represents a layer.</summary>
	public class Layer :
		TableObject
	{
		#region delegates and events

		public delegate void LinetypeChangedEventHandler(TableObject sender, TableObjectChangedEventArgs<Linetype> e);
		public event LinetypeChangedEventHandler LinetypeChanged;
		protected virtual Linetype OnLinetypeChangedEvent(Linetype oldLinetype, Linetype newLinetype)
		{
			LinetypeChangedEventHandler ae = this.LinetypeChanged;
			if (ae != null)
			{
				TableObjectChangedEventArgs<Linetype> eventArgs = new TableObjectChangedEventArgs<Linetype>(oldLinetype, newLinetype);
				ae(this, eventArgs);
				return eventArgs.NewValue;
			}
			return newLinetype;
		}

		#endregion

		#region constants

		/// <summary>Default layer name.</summary>
		public const string DefaultName = "0";

		/// <summary>Gets the default Layer 0.</summary>
		public static Layer Default => new Layer(DefaultName);

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">Layer name.</param>
		public Layer(string name)
			: this(name, true)
		{
		}
		internal Layer(string name, bool checkName)
			: base(name, DxfObjectCode.Layer, checkName)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name), "The layer name should be at least one character long.");
			}

			this.IsReserved = name.Equals(DefaultName, StringComparison.OrdinalIgnoreCase);
		}

		#endregion

		#region public properties

		private string _Description = string.Empty;
		/// <summary>Gets or sets the layer description.</summary>
		/// <remarks>
		/// The layer description is saved in the extended data of the layer, it will be handle automatically when the file is saved or loaded.<br />
		/// New line characters are not allowed.
		/// </remarks>
		public string Description
		{
			get => _Description;
			set => _Description = string.IsNullOrEmpty(value) ? string.Empty : value;
		}

		private Linetype _Linetype = Linetype.Continuous;
		/// <summary>Gets or sets the layer <see cref="Linetype">line type</see>.</summary>
		public Linetype Linetype
		{
			get => _Linetype;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_Linetype = this.OnLinetypeChangedEvent(_Linetype, value);
			}
		}

		private AciColor _Color = AciColor.Default;
		/// <summary>Gets or sets the layer <see cref="AciColor">color</see>.</summary>
		public AciColor Color
		{
			get => _Color;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}
				if (value.IsByLayer || value.IsByBlock)
				{
					throw new ArgumentException("The layer color cannot be <b>ByLayer</b> or <b>ByBlock</b>", nameof(value));
				}

				_Color = value;
			}
		}

		/// <summary>Gets or sets the layer visibility.</summary>
		public bool IsVisible { get; set; } = true;

		/// <summary>Gets or sets if the layer is frozen; otherwise layer is thawed.</summary>
		public bool IsFrozen { get; set; }

		/// <summary>Gets or sets if the layer is locked.</summary>
		public bool IsLocked { get; set; }

		/// <summary>Gets or sets if the plotting flag.</summary>
		/// <remarks>If set to <see langword="false"/>, do not plot this layer.</remarks>
		public bool Plot { get; set; } = true;

		private Lineweight _Lineweight = Lineweight.Default;
		/// <summary>Gets or sets the layer line weight, one unit is always 1/100 mm (default = Default).</summary>
		public Lineweight Lineweight
		{
			get => _Lineweight;
			set
			{
				if (value == Lineweight.ByLayer || value == Lineweight.ByBlock)
				{
					throw new ArgumentException("The lineweight of a layer cannot be set to <b>ByLayer</b> or <b>ByBlock</b>.", nameof(value));
				}
				_Lineweight = value;
			}
		}

		private Transparency _Transparency = new Transparency(0);
		/// <summary>Gets or sets layer transparency (default: 0, opaque).</summary>
		public Transparency Transparency
		{
			get => _Transparency;
			set => _Transparency = value ?? throw new ArgumentNullException(nameof(value));
		}

		/// <summary>Gets the owner of the actual layer.</summary>
		public new Layers Owner
		{
			get => (Layers)base.Owner;
			internal set => base.Owner = value;
		}

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override bool HasReferences() => this.Owner != null && this.Owner.HasReferences(this.Name);

		/// <inheritdoc/>
		public override List<DxfObjectReference> GetReferences() => this.Owner?.GetReferences(this.Name);

		/// <inheritdoc/>
		public override TableObject Clone(string newName)
		{
			Layer copy = new Layer(newName)
			{
				Color = (AciColor)this.Color.Clone(),
				IsVisible = this.IsVisible,
				IsFrozen = this.IsFrozen,
				IsLocked = this.IsLocked,
				Plot = this.Plot,
				Linetype = (Linetype)this.Linetype.Clone(),
				Lineweight = this.Lineweight,
				Transparency = (Transparency)this.Transparency.Clone()
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
	}
}