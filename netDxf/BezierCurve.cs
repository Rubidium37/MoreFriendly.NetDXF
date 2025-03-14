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
using System.Linq;

namespace netDxf
{
	/// <summary>Represent a bezier curve.</summary>
	public abstract class BezierCurve
	{
		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="controlPoints">A list of control points.</param>
		/// <param name="degree">Bezier curve degree.</param>
		/// <remarks>
		/// The curve degree must be equal to the number of control points minus one.
		/// </remarks>
		protected BezierCurve(IEnumerable<Vector3> controlPoints, int degree)
		{
			if (controlPoints == null)
			{
				throw new ArgumentNullException(nameof(controlPoints));
			}
			if (degree < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(degree), degree, "The bezier curve degree must be at least one.");
			}

			this.ControlPoints = controlPoints.ToArray();
			this.Degree = degree;

			if (this.Degree != this.ControlPoints.Length - 1)
			{
				throw new ArgumentException("The bezier curve degree must be equal to the number of control points minus one.");
			}
		}

		#endregion

		#region public properties

		/// <summary>Gets the control points.</summary>
		public Vector3[] ControlPoints { get; }

		/// <summary>Gets the bezier curve degree.</summary>
		public int Degree { get; }

		#endregion

		#region public methods

		/// <summary>Obtains a point along the curve at parameter t.</summary>
		/// <param name="t">Parameter t, between 0.0 and 1.0.</param>
		/// <returns>A point along the curve.</returns>
		public abstract Vector3 CalculatePoint(double t);

		/// <summary>Calculates the tangent vector at parameter t.</summary>
		/// <param name="t">Parameter t, between 0.0 and 1.0.</param>
		/// <returns>A normalized tangent vector.</returns>
		public abstract Vector3 CalculateTangent(double t);

		#endregion
	}
}
