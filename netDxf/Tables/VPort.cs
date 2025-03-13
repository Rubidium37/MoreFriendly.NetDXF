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
	/// <summary>Represents a document viewport.</summary>
	public class VPort :
		TableObject
	{
		#region constants

		/// <summary>Default <b>VPort</b> name.</summary>
		public const string DefaultName = "*Active";

		/// <summary>Gets the active viewport.</summary>
		public static VPort Active => new VPort(DefaultName, false);

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		public VPort(string name)
			: this(name, true)
		{
		}
		internal VPort(string name, bool checkName)
			: base(name, DxfObjectCode.VPort, checkName)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name), "The viewport name should be at least one character long.");
			}

			this.IsReserved = name.Equals("*Active", StringComparison.OrdinalIgnoreCase);
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets the view center point in <b>DCS</b> (Display Coordinate System)</summary>
		public Vector2 ViewCenter { get; set; } = Vector2.Zero;

		/// <summary>Gets or sets the snap base point in <b>DCS</b> (Display Coordinate System)</summary>
		public Vector2 SnapBasePoint { get; set; } = Vector2.Zero;

		/// <summary>Gets or sets the snap spacing X and Y.</summary>
		public Vector2 SnapSpacing { get; set; } = new Vector2(0.5);

		/// <summary>Gets or sets the grid spacing X and Y.</summary>
		public Vector2 GridSpacing { get; set; } = new Vector2(10.0);

		private Vector3 _Direction = Vector3.UnitZ;
		/// <summary>Gets or sets the view direction from target point in <b>WCS</b> (World Coordinate System).</summary>
		public Vector3 ViewDirection
		{
			get => _Direction;
			set
			{
				_Direction = Vector3.Normalize(value);
				if (Vector3.IsZero(_Direction))
				{
					throw new ArgumentException("The direction can not be the zero vector.", nameof(value));
				}
			}
		}

		/// <summary>Gets or sets the view target point in <b>WCS</b> (World Coordinate System).</summary>
		public Vector3 ViewTarget { get; set; } = Vector3.Zero;

		/// <summary>Gets or sets the view height.</summary>
		public double ViewHeight { get; set; } = 10;

		private double _ViewAspectRatio = 1.0;
		/// <summary>Gets or sets the view aspect ratio (view width/view height).</summary>
		public double ViewAspectRatio
		{
			get => _ViewAspectRatio;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The VPort aspect ratio must be greater than zero.");
				}
				_ViewAspectRatio = value;
			}
		}

		/// <summary>Gets or sets the grid on/off.</summary>
		public bool ShowGrid { get; set; } = true;

		/// <summary>Gets or sets the snap mode on/off.</summary>
		public bool SnapMode { get; set; } = false;

		/// <summary>Gets the owner of the actual viewport.</summary>
		public new VPorts Owner
		{
			get => (VPorts)base.Owner;
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
			VPort copy = new VPort(newName)
			{
				ViewCenter = this.ViewCenter,
				SnapBasePoint = this.SnapBasePoint,
				SnapSpacing = this.SnapSpacing,
				GridSpacing = this.GridSpacing,
				ViewTarget = this.ViewTarget,
				ViewDirection = _Direction,
				ViewHeight = this.ViewHeight,
				ViewAspectRatio = _ViewAspectRatio,
				ShowGrid = this.ShowGrid
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