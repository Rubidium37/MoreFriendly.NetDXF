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
using System.Runtime.CompilerServices;

namespace netDxf.Tables
{
	/// <summary>Represents a shape linetype segment.</summary>
	public class LinetypeShapeSegment :
		LinetypeSegment
	{
		#region delegates and events

		/// <summary>Generated when a property of <see cref="ShapeStyle"/> type changes.</summary>
		public event BeforeValueChangeEventHandler<ShapeStyle> BeforeChangingShapeStyleValue;
		/// <summary>Generates the <see cref="BeforeChangingShapeStyleValue"/> event.</summary>
		/// <param name="oldValue">The old value, being changed.</param>
		/// <param name="newValue">The new value, that will replace the old one.</param>
		/// <param name="propertyName">(automatic) Name of the affected property.</param>
		protected virtual ShapeStyle OnBeforeChangingBeforeValueShapeStyle(ShapeStyle oldValue, ShapeStyle newValue, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeChangingShapeStyleValue is { } handler)
			{
				var e = new BeforeValueChangeEventArgs<ShapeStyle>(propertyName, oldValue, newValue);
				handler(this, e);
				return e.NewValue;
			}
			return newValue;
		}

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">Shape name of the linetype segment.</param>
		/// <param name="style">File where the shape of the linetype segment is defined.</param>
		/// <remarks>
		/// The shape must be defined in the .shx shape definitions file.<br />
		/// The <b>DXF</b> instead of saving the shape name, as the Shape entity or the shape linetype segments definition in a .lin file,
		/// it stores the shape number. Therefore when saving a <b>DXF</b> file the shape number will be obtained reading the .shp file.<br />
		/// It is required that the equivalent .shp file to be also present in the same folder or one of the support folders defined in the DxfDocument.
		/// </remarks>
		public LinetypeShapeSegment(string name, ShapeStyle style)
			: this(name, style, 1.0, Vector2.Zero, LinetypeSegmentRotationType.Relative, 0.0, 1.0)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">Shape name of the linetype segment.</param>
		/// <param name="style">File where the shape of the linetype segment is defined.</param>
		/// <param name="length">Dash, dot, or space length of the linetype segment.</param>
		/// <remarks>
		/// The shape must be defined in the .shx shape definitions file.<br />
		/// The <b>DXF</b> instead of saving the shape name, as the Shape entity or the shape linetype segments definition in a .lin file,
		/// it stores the shape number. Therefore when saving a <b>DXF</b> file the shape number will be obtained reading the .shp file.<br />
		/// It is required that the equivalent .shp file to be also present in the same folder or one of the support folders defined in the DxfDocument.
		/// </remarks>
		public LinetypeShapeSegment(string name, ShapeStyle style, double length)
			: this(name, style, length, Vector2.Zero, LinetypeSegmentRotationType.Relative, 0.0, 1.0)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">Shape name of the linetype segment.</param>
		/// <param name="style">File where the shape of the linetype segment is defined.</param>
		/// <param name="length">Dash, dot, or space length of the linetype segment.</param>
		/// <param name="offset">Shift of the shape along the line.</param>
		/// <param name="rotationType">Type of rotation defined by the rotation value.</param>
		/// <param name="rotation">Rotation of the shape.</param>
		/// <param name="scale">Scale of the shape.</param>
		/// <remarks>
		/// The shape must be defined in the .shx shape definitions file.<br />
		/// The <b>DXF</b> instead of saving the shape name, as the Shape entity or the shape linetype segments definition in a .lin file,
		/// it stores the shape number. Therefore when saving a <b>DXF</b> file the shape number will be obtained reading the .shp file.<br />
		/// It is required that the equivalent .shp file to be also present in the same folder or one of the support folders defined in the DxfDocument.
		/// </remarks>
		public LinetypeShapeSegment(string name, ShapeStyle style, double length, Vector2 offset, LinetypeSegmentRotationType rotationType, double rotation, double scale)
			: base(LinetypeSegmentType.Shape, length)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name), "The linetype shape name should be at least one character long.");
			}
			_Name = name;
			_Style = style ?? throw new ArgumentNullException(nameof(style));
			this.Offset = offset;
			this.RotationType = rotationType;
			_Rotation = MathHelper.NormalizeAngle(rotation);
			this.Scale = scale;
		}

		#endregion

		#region public properties

		private string _Name;
		/// <summary>Gets or sets the name of the shape.</summary>
		/// <remarks>
		/// The shape must be defined in the .shx shape definitions file.<br />
		/// The <b>DXF</b> instead of saving the shape name, as the Shape entity or the shape linetype segments definition in a .lin file,
		/// it stores the shape number. Therefore when saving a <b>DXF</b> file the shape number will be obtained reading the .shx file.
		/// </remarks>
		public string Name
		{
			get => _Name;
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException(nameof(value), "The linetype shape name should be at least one character long.");
				}
				_Name = value;
			}
		}

		private ShapeStyle _Style;
		/// <summary>Gets the shape style.</summary>
		/// <remarks>
		/// It is required that the equivalent .shp file to be also present in the same folder or one of the support folders defined in the DxfDocument.
		/// </remarks>
		public ShapeStyle Style
		{
			get => _Style;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_Style = this.OnBeforeChangingBeforeValueShapeStyle(_Style, value);
			}
		}

		/// <summary>Gets or sets the shift of the shape along the line.</summary>
		public Vector2 Offset { get; set; }

		/// <summary>Gets or sets the type of rotation defined by the rotation value upright, relative, or absolute.</summary>
		public LinetypeSegmentRotationType RotationType { get; set; }

		private double _Rotation;
		/// <summary>Gets or sets the angle in degrees of the shape.</summary>
		public double Rotation
		{
			get => _Rotation;
			set => _Rotation = MathHelper.NormalizeAngle(value);
		}

		/// <summary>Gets or sets the scale of the shape relative to the scale of the line type.</summary>
		public double Scale { get; set; }

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override object Clone()
			=> new LinetypeShapeSegment(_Name, (ShapeStyle)_Style.Clone(), this.Length)
			{
				Offset = this.Offset,
				RotationType = this.RotationType,
				Rotation = _Rotation,
				Scale = this.Scale
			};

		#endregion
	}
}