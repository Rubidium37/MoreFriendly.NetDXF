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
using netDxf.Tables;

namespace netDxf.Objects
{
	/// <summary>Represents the state of the properties of a layer.</summary>
	public class LayerStateProperties :
		ICloneable
	{
		#region constructor

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">Name of the layer state properties.</param>
		public LayerStateProperties(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name));
			}
			this.Name = name;
			this.Flags = LayerPropertiesFlags.Plot;
			_Linetype = Linetype.DefaultName;
			this.Color = AciColor.Default;
			this.Lineweight = Lineweight.Default;
			this.Transparency = new Transparency(0);
			//this.plotStyle = "Color_7";
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="layer">Layer from which copy the properties.</param>
		public LayerStateProperties(Layer layer)
		{
			this.Name = layer.Name;
			if (!layer.IsVisible) this.Flags |= LayerPropertiesFlags.Hidden;
			if (layer.IsFrozen) this.Flags |= LayerPropertiesFlags.Frozen;
			if (layer.IsLocked) this.Flags |= LayerPropertiesFlags.Locked;
			if (layer.Plot) this.Flags |= LayerPropertiesFlags.Plot;
			_Linetype = layer.Linetype.Name;
			this.Color = (AciColor)layer.Color.Clone();
			this.Lineweight = layer.Lineweight;
			this.Transparency = (Transparency)layer.Transparency.Clone();
			//this.plotStyle = "Color_" + layer.Color.Index;
		}

		#endregion

		#region public properties

		/// <summary>Gets the layer properties name.</summary>
		public string Name { get; }

		/// <summary>Layer properties flags.</summary>
		public LayerPropertiesFlags Flags { get; set; }

		private string _Linetype;
		/// <summary>Layer properties linetype name.</summary>
		public string LinetypeName
		{
			get => _Linetype;
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException(nameof(value));
				}
				_Linetype = value;
			}
		}

		/// <summary>Layer properties color.</summary>
		public AciColor Color { get; set; }

		/// <summary>Layer properties lineweight.</summary>
		public Lineweight Lineweight { get; set; }

		/// <summary>Layer properties transparency.</summary>
		public Transparency Transparency { get; set; }

		///// <summary>
		///// Layer properties plot style name.
		///// </summary>
		//public string PlotStyleName { get; set; }

		#endregion

		#region public methods

		/// <summary>Copy the layer to the current layer state properties.</summary>
		/// <param name="layer">Layer from which copy the properties.</param>
		/// <param name="options">Layer properties to copy.</param>
		public void CopyFrom(Layer layer, LayerPropertiesRestoreFlags options)
		{
			if (!string.Equals(this.Name, layer.Name, StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("Only a layer with the same name can be copied.", nameof(layer));
			}

			this.Flags = LayerPropertiesFlags.None;

			if (options.HasFlag(LayerPropertiesRestoreFlags.Hidden))
			{
				if (!layer.IsVisible) this.Flags |= LayerPropertiesFlags.Hidden;
			}
			if (options.HasFlag(LayerPropertiesRestoreFlags.Frozen))
			{
				if (layer.IsFrozen) this.Flags |= LayerPropertiesFlags.Frozen;
			}
			if (options.HasFlag(LayerPropertiesRestoreFlags.Locked))
			{
				if (layer.IsLocked) this.Flags |= LayerPropertiesFlags.Locked;
			}
			if (options.HasFlag(LayerPropertiesRestoreFlags.Plot))
			{
				if (layer.Plot) this.Flags |= LayerPropertiesFlags.Plot;
			}
			if (options.HasFlag(LayerPropertiesRestoreFlags.Linetype))
			{
				_Linetype = layer.Linetype.Name;
			}
			if (options.HasFlag(LayerPropertiesRestoreFlags.Color))
			{
				this.Color = (AciColor)layer.Color.Clone();
			}
			if (options.HasFlag(LayerPropertiesRestoreFlags.Lineweight))
			{
				this.Lineweight = layer.Lineweight;
			}
			if (options.HasFlag(LayerPropertiesRestoreFlags.Transparency))
			{
				this.Transparency = (Transparency)layer.Transparency.Clone();
			}
		}

		/// <summary>Copy the current layer state properties to a layer.</summary>
		/// <param name="layer">Layer to which copy the properties.</param>
		/// <param name="options">Layer properties to copy.</param>
		public void CopyTo(Layer layer, LayerPropertiesRestoreFlags options)
		{
			if (!string.Equals(this.Name, layer.Name, StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("Only a layer with the same name can be copied.", nameof(layer));
			}

			if (options.HasFlag(LayerPropertiesRestoreFlags.Hidden))
			{
				layer.IsVisible = !this.Flags.HasFlag(LayerPropertiesFlags.Hidden);
			}
			if (options.HasFlag(LayerPropertiesRestoreFlags.Frozen))
			{
				layer.IsFrozen = this.Flags.HasFlag(LayerPropertiesFlags.Frozen);
			}
			if (options.HasFlag(LayerPropertiesRestoreFlags.Locked))
			{
				layer.IsLocked = this.Flags.HasFlag(LayerPropertiesFlags.Locked);
			}
			if (options.HasFlag(LayerPropertiesRestoreFlags.Plot))
			{
				layer.Plot = this.Flags.HasFlag(LayerPropertiesFlags.Plot);
			}
			if (options.HasFlag(LayerPropertiesRestoreFlags.Linetype))
			{
				Linetype line = null;
				if (layer.Owner != null)
				{
					DxfDocument doc = layer.Owner.Owner;
					line = doc.Linetypes[this.LinetypeName];
				}
				layer.Linetype = line ?? new Linetype(this.LinetypeName);

			}
			if (options.HasFlag(LayerPropertiesRestoreFlags.Color))
			{
				layer.Color = (AciColor)this.Color.Clone();
			}
			if (options.HasFlag(LayerPropertiesRestoreFlags.Lineweight))
			{
				layer.Lineweight = this.Lineweight;
			}
			if (options.HasFlag(LayerPropertiesRestoreFlags.Transparency))
			{
				layer.Transparency = (Transparency)this.Transparency.Clone();
			}
		}

		/// <summary>Compares the stored properties with the specified layer.</summary>
		/// <param name="layer">Layer to compare with.</param>
		/// <returns>If the stored properties are the same as the specified layer it returns <see langword="true"/>; otherwise, <see langword="false"/>.</returns>
		public bool CompareWith(Layer layer)
		{
			if (!string.Equals(layer.Name, this.Name, StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}

			if (layer.IsVisible != !this.Flags.HasFlag(LayerPropertiesFlags.Hidden))
			{
				return false;
			}

			if (layer.IsFrozen != this.Flags.HasFlag(LayerPropertiesFlags.Frozen))
			{
				return false;
			}

			if (layer.IsLocked != this.Flags.HasFlag(LayerPropertiesFlags.Locked))
			{
				return false;
			}

			if (layer.Plot != this.Flags.HasFlag(LayerPropertiesFlags.Plot))
			{
				return false;
			}

			if (!string.Equals(layer.Linetype.Name, this.LinetypeName, StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}

			if (!layer.Color.Equals(this.Color))
			{
				return false;
			}

			if (layer.Lineweight != this.Lineweight)
			{
				return false;
			}

			if (!layer.Transparency.Equals(this.Transparency))
			{
				return false;
			}

			return true;
		}

		#endregion

		#region ICloneable

		/// <inheritdoc/>
		public object Clone()
			=> new LayerStateProperties(this.Name)
			{
				Flags = this.Flags,
				LinetypeName = _Linetype,
				Color = (AciColor)this.Color.Clone(),
				Lineweight = this.Lineweight,
				Transparency = (Transparency)this.Transparency.Clone(),
				//PlotStyleName = this.plotStyle
			};

		#endregion

	}
}