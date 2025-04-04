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
	/// <summary>Represents a User Coordinate System.</summary>
	public class UCS :
		TableObject
	{
		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">User coordinate system name.</param>
		public UCS(string name)
			: this(name, true)
		{
		}
		internal UCS(string name, bool checkName)
			: base(name, DxfObjectCode.Ucs, checkName)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name), "The UCS name should be at least one character long.");
			}

			this.Origin = Vector3.Zero;
			this.XAxis = Vector3.UnitX;
			this.YAxis = Vector3.UnitY;
			this.ZAxis = Vector3.UnitZ;
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="name">User coordinate system name.</param>
		/// <param name="origin">Origin in WCS.</param>
		/// <param name="xDirection">X-axis direction in WCS.</param>
		/// <param name="yDirection">Y-axis direction in WCS.</param>
		/// <remarks>
		/// The x-axis direction and y-axis direction must be perpendicular.
		/// </remarks>
		public UCS(string name, Vector3 origin, Vector3 xDirection, Vector3 yDirection)
			: this(name, origin, xDirection, yDirection, true)
		{
		}
		internal UCS(string name, Vector3 origin, Vector3 xDirection, Vector3 yDirection, bool checkName)
			: base(name, DxfObjectCode.Ucs, checkName)
		{
			if (!Vector3.ArePerpendicular(xDirection, yDirection))
			{
				throw new ArgumentException("X-axis direction and Y-axis direction must be perpendicular.");
			}

			this.Origin = origin;
			this.XAxis = xDirection;
			this.XAxis.Normalize();
			this.YAxis = yDirection;
			this.YAxis.Normalize();
			this.ZAxis = Vector3.CrossProduct(this.XAxis, this.YAxis);
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets the user coordinate system origin in WCS.</summary>
		public Vector3 Origin { get; set; }

		/// <summary>Gets the user coordinate system x-axis direction in WCS.</summary>
		public Vector3 XAxis { get; private set; }

		/// <summary>Gets the user coordinate system y-axis direction in WCS.</summary>
		public Vector3 YAxis { get; private set; }

		/// <summary>Gets the user coordinate system z-axis direction in WCS.</summary>
		public Vector3 ZAxis { get; private set; }

		/// <summary>Gets the owner of the actual user coordinate system.</summary>
		public new UCSs Owner
		{
			get => (UCSs)base.Owner;
			internal set => base.Owner = value;
		}

		#endregion

		#region public methods

		/// <summary>Sets the user coordinate system x-axis and y-axis direction.</summary>
		/// <param name="xDirection">X-axis direction in WCS.</param>
		/// <param name="yDirection">Y-axis direction in WCS.</param>
		public void SetAxis(Vector3 xDirection, Vector3 yDirection)
		{
			if (!Vector3.ArePerpendicular(xDirection, yDirection))
			{
				throw new ArgumentException("X-axis direction and Y-axis direction must be perpendicular.");
			}
			this.XAxis = xDirection;
			this.XAxis.Normalize();
			this.YAxis = yDirection;
			this.YAxis.Normalize();
			this.ZAxis = Vector3.CrossProduct(this.XAxis, this.YAxis);
		}

		/// <summary>Creates a new user coordinate system from the x-axis and a point on <b>XY</b> plane.</summary>
		/// <param name="name">User coordinate system name.</param>
		/// <param name="origin">Origin in <b>WC</b>S.</param>
		/// <param name="xDirection">X-axis direction in <b>WCS</b>.</param>
		/// <param name="pointOnPlaneXY">Point on the <b>XY</b> plane.</param>
		/// <returns>A new user coordinate system.</returns>
		public static UCS FromXAxisAndPointOnXYplane(string name, Vector3 origin, Vector3 xDirection, Vector3 pointOnPlaneXY)
		{
			UCS ucs = new UCS(name);
			ucs.Origin = origin;
			ucs.XAxis = xDirection;
			ucs.XAxis.Normalize();
			ucs.ZAxis = Vector3.CrossProduct(xDirection, pointOnPlaneXY);
			ucs.ZAxis.Normalize();
			ucs.YAxis = Vector3.CrossProduct(ucs.ZAxis, ucs.XAxis);
			return ucs;
		}

		/// <summary>Creates a new user coordinate system from the <b>XY</b> plane normal (z-axis).</summary>
		/// <param name="name">User coordinate system name.</param>
		/// <param name="origin">Origin in <b>WCS</b>.</param>
		/// <param name="normal">XY plane normal (z-axis).</param>
		/// <returns>A new user coordinate system.</returns>
		/// <remarks>This method uses the <see cref="MathHelper.ArbitraryAxis"/> algorithm to obtain the user coordinate system x-axis and y-axis.</remarks>
		public static UCS FromNormal(string name, Vector3 origin, Vector3 normal)
		{
			Matrix3 mat = MathHelper.ArbitraryAxis(normal);
			UCS ucs = new UCS(name);
			ucs.Origin = origin;
			ucs.XAxis = new Vector3(mat.M11, mat.M21, mat.M31);
			ucs.YAxis = new Vector3(mat.M12, mat.M22, mat.M32);
			ucs.ZAxis = new Vector3(mat.M13, mat.M23, mat.M33);
			return ucs;
		}

		/// <summary>Creates a new user coordinate system from the <b>XY</b> plane normal (z-axis).</summary>
		/// <param name="name">User coordinate system name.</param>
		/// <param name="origin">Origin in <b>WCS</b>.</param>
		/// <param name="normal">XY plane normal (z-axis).</param>
		/// <param name="rotation">The counter-clockwise angle in radians along the normal (z-axis).</param>
		/// <returns>A new user coordinate system.</returns>
		/// <remarks>This method uses the <see cref="MathHelper.ArbitraryAxis"/> algorithm to obtain the user coordinate system x-axis and y-axis.</remarks>
		public static UCS FromNormal(string name, Vector3 origin, Vector3 normal, double rotation)
		{
			Matrix3 mat = MathHelper.ArbitraryAxis(normal);
			Matrix3 rot = Matrix3.RotationZ(rotation);
			mat *= rot;
			UCS ucs = new UCS(name);
			ucs.Origin = origin;
			ucs.XAxis = new Vector3(mat.M11, mat.M21, mat.M31);
			ucs.YAxis = new Vector3(mat.M12, mat.M22, mat.M32);
			ucs.ZAxis = new Vector3(mat.M13, mat.M23, mat.M33);
			return ucs;
		}

		/// <summary>Gets the user coordinate system rotation matrix.</summary>
		/// <returns>A Matrix3.</returns>
		public Matrix3 GetTransformation()
			=> new Matrix3
			(
				this.XAxis.X, this.YAxis.X, this.ZAxis.X,
				this.XAxis.Y, this.YAxis.Y, this.ZAxis.Y,
				this.XAxis.Z, this.YAxis.Z, this.ZAxis.Z
			);

		/// <summary>Transforms a point between coordinate systems.</summary>
		/// <param name="point">Point to transform.</param>
		/// <param name="from">Points coordinate system.</param>
		/// <param name="to">Coordinate system of the transformed points.</param>
		/// <returns>Transformed point list.</returns>
		public Vector3 Transform(Vector3 point, CoordinateSystem from, CoordinateSystem to)
		{
			Matrix3 transformation = this.GetTransformation();
			Vector3 translation = this.Origin;

			switch (from)
			{
				case CoordinateSystem.World when to == CoordinateSystem.Object:
					{
						transformation = transformation.Transpose();
						return transformation * (point - translation);
					}
				case CoordinateSystem.Object when to == CoordinateSystem.World:
					{
						return transformation * point + translation;
					}
				default:
					return point;
			}
		}

		/// <summary>Transforms a point list between coordinate systems.</summary>
		/// <param name="points">Points to transform.</param>
		/// <param name="from">Points coordinate system.</param>
		/// <param name="to">Coordinate system of the transformed points.</param>
		/// <returns>Transformed point list.</returns>
		public List<Vector3> Transform(IEnumerable<Vector3> points, CoordinateSystem from, CoordinateSystem to)
		{
			if (points == null)
			{
				throw new ArgumentNullException(nameof(points));
			}

			Matrix3 transformation = this.GetTransformation();
			Vector3 translation = this.Origin;
			List<Vector3> transPoints;

			switch (from)
			{
				case CoordinateSystem.World when to == CoordinateSystem.Object:
					{
						transPoints = new List<Vector3>();
						transformation = transformation.Transpose();
						foreach (Vector3 p in points)
						{
							transPoints.Add(transformation * (p - translation));
						}

						return transPoints;
					}
				case CoordinateSystem.Object when to == CoordinateSystem.World:
					{
						transPoints = new List<Vector3>();
						foreach (Vector3 p in points)
						{
							transPoints.Add(transformation * p + translation);
						}
						return transPoints;
					}
				default:
					return new List<Vector3>(points);
			}
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
			UCS copy = new UCS(newName)
			{
				Origin = this.Origin,
				XAxis = this.XAxis,
				YAxis = this.YAxis,
				ZAxis = this.ZAxis,
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