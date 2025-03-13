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
using netDxf.Tables;

namespace netDxf.Entities
{
	/// <summary>Defines a single line thats is part of a <see cref="HatchPattern">hatch pattern</see>.</summary>
	public class HatchPatternLineDefinition :
		ICloneable
	{
		#region constructor

		/// <summary>Initializes a new instance of the class.</summary>
		public HatchPatternLineDefinition()
		{
		}

		#endregion

		#region public properties

		private double _Angle = 0.0;
		/// <summary>Gets or sets the angle of the line.</summary>
		public double Angle
		{
			get => _Angle;
			set => _Angle = MathHelper.NormalizeAngle(value);
		}

		/// <summary>Gets or sets the origin of the line.</summary>
		public Vector2 Origin { get; set; } = Vector2.Zero;

		/// <summary>Gets or sets the local displacements between lines of the same family.</summary>
		/// <remarks>
		/// The Delta.X value indicates the displacement between members of the family in the direction of the line. It is used only for dashed lines.
		/// The Delta.Y value indicates the spacing between members of the family; that is, it is measured perpendicular to the lines.
		/// </remarks>
		public Vector2 Delta { get; set; } = Vector2.Zero;

		/// <summary>Gets he dash pattern of the line it is equivalent as the segments of a <see cref="Linetype"/>.</summary>
		/// <remarks>
		/// Positive values means solid segments and negative values means spaces (one entry per element).
		/// </remarks>
		public List<double> DashPattern { get; } = new List<double>();

		#endregion

		#region overrides

		/// <inheritdoc/>
		public object Clone()
		{
			HatchPatternLineDefinition copy = new HatchPatternLineDefinition
			{
				Angle = _Angle,
				Origin = this.Origin,
				Delta = this.Delta,
			};

			foreach (double dash in this.DashPattern)
				copy.DashPattern.Add(dash);

			return copy;
		}

		#endregion
	}
}