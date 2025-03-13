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
	public class View :
		TableObject
	{
		#region constants

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		public View(string name)
			: this(name, true)
		{
		}
		internal View(string name, bool checkName)
			: base(name, DxfObjectCode.View, checkName)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name), "The view name should be at least one character long.");
			}
		}

		#endregion

		#region public properties

		public Vector3 Target { get; set; } = Vector3.Zero;

		public Vector3 Camera { get; set; } = Vector3.UnitZ;

		public double Height { get; set; } = 1.0;

		public double Width { get; set; } = 1.0;

		public double Rotation { get; set; } = 0.0;

		public ViewModeFlags Viewmode { get; set; } = ViewModeFlags.Off;

		public double Fov { get; set; } = 40.0;

		public double FrontClippingPlane { get; set; } = 0.0;

		public double BackClippingPlane { get; set; } = 0.0;

		/// <summary>Gets the owner of the actual view.</summary>
		public new Views Owner
		{
			get => (Views)base.Owner;
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
			View copy = new View(newName)
			{
				Target = this.Target,
				Camera = this.Camera,
				Height = this.Height,
				Width = this.Width,
				Rotation = this.Rotation,
				Viewmode = this.Viewmode,
				Fov = this.Fov,
				FrontClippingPlane = this.FrontClippingPlane,
				BackClippingPlane = this.BackClippingPlane
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