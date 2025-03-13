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
using System.Text;
using System.Threading;

namespace netDxf
{
	/// <summary>Represents a 3x3 double precision matrix.</summary>
	public struct Matrix3 :
		IEquatable<Matrix3>
	{
		private bool IsDirty;

		#region constructors

		/// <summary>Initializes a new instance of Matrix3.</summary>
		/// <param name="m11">Element [0,0].</param>
		/// <param name="m12">Element [0,1].</param>
		/// <param name="m13">Element [0,2].</param>
		/// <param name="m21">Element [1,0].</param>
		/// <param name="m22">Element [1,1].</param>
		/// <param name="m23">Element [1,2].</param>
		/// <param name="m31">Element [2,0].</param>
		/// <param name="m32">Element [2,1].</param>
		/// <param name="m33">Element [2,2].</param>
		public Matrix3(double m11, double m12, double m13,
						double m21, double m22, double m23,
						double m31, double m32, double m33)
		{
			_M11 = m11;
			_M12 = m12;
			_M13 = m13;

			_M21 = m21;
			_M22 = m22;
			_M23 = m23;

			_M31 = m31;
			_M32 = m32;
			_M33 = m33;

			this.IsDirty = true;
			_IsIdentity = false;
		}

		#endregion

		#region constants

		/// <summary>Gets the zero matrix.</summary>
		public static Matrix3 Zero
			=> new Matrix3
			(
				0.0, 0.0, 0.0,
				0.0, 0.0, 0.0,
				0.0, 0.0, 0.0
			)
			{
				IsDirty = false,
				_IsIdentity = false
			};

		/// <summary>Gets the identity matrix.</summary>
		public static Matrix3 Identity
			=> new Matrix3
			(
				1.0, 0.0, 0.0,
				0.0, 1.0, 0.0,
				0.0, 0.0, 1.0
			)
			{
				IsDirty = false,
				_IsIdentity = true
			};

		#endregion

		#region public properties

		private double _M11;
		/// <summary>Gets or sets the matrix element [0,0].</summary>
		public double M11
		{
			get => _M11;
			set
			{
				_M11 = value;
				this.IsDirty = true;
			}
		}

		private double _M12;
		/// <summary>Gets or sets the matrix element [0,1].</summary>
		public double M12
		{
			get => _M12;
			set
			{
				_M12 = value;
				this.IsDirty = true;
			}
		}

		private double _M13;
		/// <summary>Gets or sets the matrix element [0,2].</summary>
		public double M13
		{
			get => _M13;
			set
			{
				_M13 = value;
				this.IsDirty = true;
			}
		}

		private double _M21;
		/// <summary>Gets or sets the matrix element [1,0].</summary>
		public double M21
		{
			get => _M21;
			set
			{
				_M21 = value;
				this.IsDirty = true;
			}
		}

		private double _M22;
		/// <summary>Gets or sets the matrix element [1,1].</summary>
		public double M22
		{
			get => _M22;
			set
			{
				_M22 = value;
				this.IsDirty = true;
			}
		}

		private double _M23;
		/// <summary>Gets or sets the matrix element [1,2].</summary>
		public double M23
		{
			get => _M23;
			set
			{
				_M23 = value;
				this.IsDirty = true;
			}
		}

		private double _M31;
		/// <summary>Gets or sets the matrix element [2,0].</summary>
		public double M31
		{
			get => _M31;
			set
			{
				_M31 = value;
				this.IsDirty = true;
			}
		}

		private double _M32;
		/// <summary>Gets or sets the matrix element [2,1].</summary>
		public double M32
		{
			get => _M32;
			set
			{
				_M32 = value;
				this.IsDirty = true;
			}
		}

		private double _M33;
		/// <summary>Gets or sets the matrix element [2,2].</summary>
		public double M33
		{
			get => _M33;
			set
			{
				_M33 = value;
				this.IsDirty = true;
			}
		}

		/// <summary>Gets or sets the component at the given row and column index in the matrix.</summary>
		/// <param name="row">The row index of the matrix.</param>
		/// <param name="column">The column index of the matrix.</param>
		/// <returns>The component at the given row and column index in the matrix.</returns>
		public double this[int row, int column]
		{
			get
			{
				switch (row)
				{
					case 0:
						switch (column)
						{
							case 0:
								return _M11;
							case 1:
								return _M12;
							case 2:
								return _M13;
							default:
								throw new ArgumentOutOfRangeException(nameof(column));
						}
					case 1:
						switch (column)
						{
							case 0:
								return _M21;
							case 1:
								return _M22;
							case 2:
								return _M23;
							default:
								throw new ArgumentOutOfRangeException(nameof(column));
						}

					case 2:
						switch (column)
						{
							case 0:
								return _M31;
							case 1:
								return _M32;
							case 2:
								return _M33;
							default:
								throw new ArgumentOutOfRangeException(nameof(column));
						}

					default:
						throw new ArgumentOutOfRangeException(nameof(row));
				}
			}
			set
			{
				switch (row)
				{
					case 0:
						switch (column)
						{
							case 0:
								_M11 = value;
								break;
							case 1:
								_M12 = value;
								break;
							case 2:
								_M13 = value;
								break;
							default:
								throw new ArgumentOutOfRangeException(nameof(column));
						}
						break;

					case 1:
						switch (column)
						{
							case 0:
								_M21 = value;
								break;
							case 1:
								_M22 = value;
								break;
							case 2:
								_M23 = value;
								break;
							default:
								throw new ArgumentOutOfRangeException(nameof(column));
						}
						break;

					case 2:
						switch (column)
						{
							case 0:
								_M31 = value;
								break;
							case 1:
								_M32 = value;
								break;
							case 2:
								_M33 = value;
								break;
							default:
								throw new ArgumentOutOfRangeException(nameof(column));
						}
						break;

					default:
						throw new ArgumentOutOfRangeException(nameof(row));
				}
				this.IsDirty = true;
			}
		}

		private bool _IsIdentity;
		/// <summary>Gets if the actual matrix is the identity.</summary>
		/// <remarks>
		/// The checks to see if the matrix is the identity uses the MathHelper.Epsilon as a the threshold for testing values close to one and zero.
		/// </remarks>
		public bool IsIdentity
		{
			get
			{
				if (this.IsDirty)
				{
					this.IsDirty = false;

					// row 1
					if (!MathHelper.IsOne(this.M11))
					{
						_IsIdentity = false;
						return _IsIdentity;
					}
					if (!MathHelper.IsZero(this.M12))
					{
						_IsIdentity = false;
						return _IsIdentity;
					}
					if (!MathHelper.IsZero(this.M13))
					{
						_IsIdentity = false;
						return _IsIdentity;
					}

					// row 2
					if (!MathHelper.IsZero(this.M21))
					{
						_IsIdentity = false;
						return _IsIdentity;
					}
					if (!MathHelper.IsOne(this.M22))
					{
						_IsIdentity = false;
						return _IsIdentity;
					}
					if (!MathHelper.IsZero(this.M23))
					{
						_IsIdentity = false;
						return _IsIdentity;
					}

					// row 3
					if (!MathHelper.IsZero(this.M31))
					{
						_IsIdentity = false;
						return _IsIdentity;
					}
					if (!MathHelper.IsZero(this.M32))
					{
						_IsIdentity = false;
						return _IsIdentity;
					}
					if (!MathHelper.IsOne(this.M33))
					{
						_IsIdentity = false;
						return _IsIdentity;
					}

					_IsIdentity = true;
					return _IsIdentity;
				}

				return _IsIdentity;

				//if (this.IsDirty)
				//{
				//	bool check = true;
				//	for (int i = 0; i < 3; i++)
				//	{
				//		for (int j = 0; j < 3; j++)
				//		{
				//			check = i == j ? MathHelper.IsOne(this[i, j]) : MathHelper.IsZero(this[i, j]);
				//			if (!check) break;
				//		}
				//		if (!check) break;
				//	}
				//	this.IsDirty = false;
				//	_IsIdentity = check;
				//}
				//return _IsIdentity;
			}
		}

		#endregion

		#region overloaded operators

		/// <summary>Matrix addition.</summary>
		/// <param name="a">Matrix3.</param>
		/// <param name="b">Matrix3.</param>
		/// <returns>Matrix3.</returns>
		public static Matrix3 operator +(Matrix3 a, Matrix3 b)
			=> new Matrix3
			(
				a.M11 + b.M11, a.M12 + b.M12, a.M13 + b.M13,
				a.M21 + b.M21, a.M22 + b.M22, a.M23 + b.M23,
				a.M31 + b.M31, a.M32 + b.M32, a.M33 + b.M33
			);

		/// <summary>Matrix addition.</summary>
		/// <param name="a">Matrix3.</param>
		/// <param name="b">Matrix3.</param>
		/// <returns>Matrix3.</returns>
		public static Matrix3 Add(Matrix3 a, Matrix3 b)
			=> new Matrix3
			(
				a.M11 + b.M11, a.M12 + b.M12, a.M13 + b.M13,
				a.M21 + b.M21, a.M22 + b.M22, a.M23 + b.M23,
				a.M31 + b.M31, a.M32 + b.M32, a.M33 + b.M33
			);

		/// <summary>Matrix subtraction.</summary>
		/// <param name="a">Matrix3.</param>
		/// <param name="b">Matrix3.</param>
		/// <returns>Matrix3.</returns>
		public static Matrix3 operator -(Matrix3 a, Matrix3 b)
			=> new Matrix3
			(
				a.M11 - b.M11, a.M12 - b.M12, a.M13 - b.M13,
				a.M21 - b.M21, a.M22 - b.M22, a.M23 - b.M23,
				a.M31 - b.M31, a.M32 - b.M32, a.M33 - b.M33
			);

		/// <summary>Matrix subtraction.</summary>
		/// <param name="a">Matrix3.</param>
		/// <param name="b">Matrix3.</param>
		/// <returns>Matrix3.</returns>
		public static Matrix3 Subtract(Matrix3 a, Matrix3 b)
			=> new Matrix3
			(
				a.M11 - b.M11, a.M12 - b.M12, a.M13 - b.M13,
				a.M21 - b.M21, a.M22 - b.M22, a.M23 - b.M23,
				a.M31 - b.M31, a.M32 - b.M32, a.M33 - b.M33
			);

		/// <summary>Product of two matrices.</summary>
		/// <param name="a">Matrix3.</param>
		/// <param name="b">Matrix3.</param>
		/// <returns>Matrix3.</returns>
		public static Matrix3 operator *(Matrix3 a, Matrix3 b)
		{
			if (a.IsIdentity)
			{
				return b;
			}

			if (b.IsIdentity)
			{
				return a;
			}

			return new Matrix3
			(
				a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31, a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32, a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33,
				a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31, a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32, a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33,
				a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31, a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32, a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33
			);
		}

		/// <summary>Product of two matrices.</summary>
		/// <param name="a">Matrix3.</param>
		/// <param name="b">Matrix3.</param>
		/// <returns>Matrix3.</returns>
		public static Matrix3 Multiply(Matrix3 a, Matrix3 b)
		{
			if (a.IsIdentity)
			{
				return b;
			}

			if (b.IsIdentity)
			{
				return a;
			}

			return new Matrix3
			(
				a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31, a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32, a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33,
				a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31, a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32, a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33,
				a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31, a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32, a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33
			);
		}

		/// <summary>Product of a matrix with a vector.</summary>
		/// <param name="a">Matrix3.</param>
		/// <param name="u">Vector3.</param>
		/// <returns>Matrix3.</returns>
		/// <remarks>Matrix3 adopts the convention of using column vectors.</remarks>
		public static Vector3 operator *(Matrix3 a, Vector3 u)
			=> a.IsIdentity
			? u
			: new Vector3
			(
				a.M11 * u.X + a.M12 * u.Y + a.M13 * u.Z,
				a.M21 * u.X + a.M22 * u.Y + a.M23 * u.Z,
				a.M31 * u.X + a.M32 * u.Y + a.M33 * u.Z
			);

		/// <summary>Product of a matrix with a vector.</summary>
		/// <param name="a">Matrix3.</param>
		/// <param name="u">Vector3.</param>
		/// <returns>Matrix3.</returns>
		/// <remarks>Matrix3 adopts the convention of using column vectors.</remarks>
		public static Vector3 Multiply(Matrix3 a, Vector3 u)
			=> a.IsIdentity
			? u
			: new Vector3
			(
				a.M11 * u.X + a.M12 * u.Y + a.M13 * u.Z,
				a.M21 * u.X + a.M22 * u.Y + a.M23 * u.Z,
				a.M31 * u.X + a.M32 * u.Y + a.M33 * u.Z
			);

		/// <summary>Product of a matrix with a scalar.</summary>
		/// <param name="m">Matrix3.</param>
		/// <param name="a">Scalar.</param>
		/// <returns>Matrix3.</returns>
		public static Matrix3 operator *(Matrix3 m, double a)
			=> new Matrix3
			(
				m.M11 * a, m.M12 * a, m.M13 * a,
				m.M21 * a, m.M22 * a, m.M23 * a,
				m.M31 * a, m.M32 * a, m.M33 * a
			);

		/// <summary>Product of a matrix with a scalar.</summary>
		/// <param name="m">Matrix3.</param>
		/// <param name="a">Scalar.</param>
		/// <returns>Matrix3.</returns>
		public static Matrix3 Multiply(Matrix3 m, double a)
			=> new Matrix3
			(
				m.M11 * a, m.M12 * a, m.M13 * a,
				m.M21 * a, m.M22 * a, m.M23 * a,
				m.M31 * a, m.M32 * a, m.M33 * a
			);

		/// <summary>Check if the components of two matrices are equal.</summary>
		/// <param name="u">Matrix3.</param>
		/// <param name="v">Matrix3.</param>
		/// <returns><see langword="true"/> if the matrix components are equal or <see langword="false"/> in any other case.</returns>
		public static bool operator ==(Matrix3 u, Matrix3 v) => Equals(u, v);
		/// <summary>Check if the components of two matrices are different.</summary>
		/// <param name="u">Matrix3.</param>
		/// <param name="v">Matrix3.</param>
		/// <returns><see langword="true"/> if the matrix components are different or <see langword="false"/> in any other case.</returns>
		public static bool operator !=(Matrix3 u, Matrix3 v) => !Equals(u, v);

		#endregion

		#region public methods

		/// <summary>Calculate the determinant of the actual matrix.</summary>
		/// <returns>Determinant.</returns>
		public double Determinant()
		{
			if (this.IsIdentity)
			{
				return 1.0;
			}

			return _M11 * _M22 * _M33 +
					_M12 * _M23 * _M31 +
					_M13 * _M21 * _M32 -
					_M13 * _M22 * _M31 -
					_M11 * _M23 * _M32 -
					_M12 * _M21 * _M33;
		}

		/// <summary>Calculates the inverse matrix.</summary>
		/// <returns>Inverse Matrix3.</returns>
		public Matrix3 Inverse()
		{
			if (this.IsIdentity)
			{
				return Identity;
			}

			double det = this.Determinant();
			if (MathHelper.IsZero(det))
			{
				throw new ArithmeticException("The matrix is not invertible.");
			}

			det = 1 / det;

			return new Matrix3
			(
				det * (_M22 * _M33 - _M23 * _M32),
				det * (_M13 * _M32 - _M12 * _M33),
				det * (_M12 * _M23 - _M13 * _M22),
				det * (_M23 * _M31 - _M21 * _M33),
				det * (_M11 * _M33 - _M13 * _M31),
				det * (_M13 * _M21 - _M11 * _M23),
				det * (_M21 * _M32 - _M22 * _M31),
				det * (_M12 * _M31 - _M11 * _M32),
				det * (_M11 * _M22 - _M12 * _M21)
			);
		}

		/// <summary>Obtains the transpose matrix.</summary>
		/// <returns>Transpose matrix.</returns>
		public Matrix3 Transpose()
			=> this.IsIdentity
			? Identity
			: new Matrix3
			(
				_M11, _M21, _M31,
				_M12, _M22, _M32,
				_M13, _M23, _M33
			);

		#endregion

		#region static methods

		/// <summary>Builds a rotation matrix for a rotation around the x-axis.</summary>
		/// <param name="angle">The counter-clockwise angle in radians.</param>
		/// <returns>The resulting <see cref="Matrix3"/> instance.</returns>
		/// <remarks><see cref="Matrix3"/> adopts the convention of using column vectors to represent a transformation matrix.</remarks>
		public static Matrix3 RotationX(double angle)
		{
			double cos = Math.Cos(angle);
			double sin = Math.Sin(angle);
			return new Matrix3
			(
				1.0, 0.0, 0.0,
				0.0, cos, -sin,
				0.0, sin, cos
			);
		}

		/// <summary>Builds a rotation matrix for a rotation around the y-axis.</summary>
		/// <param name="angle">The counter-clockwise angle in radians.</param>
		/// <returns>The resulting <see cref="Matrix3"/> instance.</returns>
		/// <remarks><see cref="Matrix3"/> adopts the convention of using column vectors to represent a transformation matrix.</remarks>
		public static Matrix3 RotationY(double angle)
		{
			double cos = Math.Cos(angle);
			double sin = Math.Sin(angle);
			return new Matrix3
			(
				cos, 0.0, sin,
				0.0, 1.0, 0.0,
				-sin, 0.0, cos
			);
		}

		/// <summary>Builds a rotation matrix for a rotation around the z-axis.</summary>
		/// <param name="angle">The counter-clockwise angle in radians.</param>
		/// <returns>The resulting <see cref="Matrix3"/> instance.</returns>
		/// <remarks><see cref="Matrix3"/> adopts the convention of using column vectors to represent a transformation matrix.</remarks>
		public static Matrix3 RotationZ(double angle)
		{
			double cos = Math.Cos(angle);
			double sin = Math.Sin(angle);
			return new Matrix3
			(
				cos, -sin, 0.0,
				sin, cos, 0.0,
				0.0, 0.0, 1.0
			);
		}

		/// <summary>Build a scaling matrix.</summary>
		/// <param name="value">Single scale factor for x, y, and z axis.</param>
		/// <returns>A scaling matrix.</returns>
		public static Matrix3 Scale(double value) => Scale(value, value, value);
		/// <summary>Build a scaling matrix.</summary>
		/// <param name="value">Scale factors for x, y, and z axis.</param>
		/// <returns>A scaling matrix.</returns>
		public static Matrix3 Scale(Vector3 value) => Scale(value.X, value.Y, value.Z);
		/// <summary>Build a scaling matrix.</summary>
		/// <param name="x">Scale factor for x-axis.</param>
		/// <param name="y">Scale factor for y-axis.</param>
		/// <param name="z">Scale factor for z-axis.</param>
		/// <returns>A scaling matrix.</returns>
		public static Matrix3 Scale(double x, double y, double z)
			=> new Matrix3
			(
				x, 0.0, 0.0,
				0.0, y, 0.0,
				0.0, 0.0, z
			);

		/// <summary>Build the reflection matrix of a mirror plane that passes through the origin.</summary>
		/// <param name="normal">Mirror plane normal vector.</param>
		/// <returns>A mirror plane reflection matrix that passes through the origin.</returns>
		public static Matrix3 Reflection(Vector3 normal)
		{
			// plane equation that passes through the origin ax + by + cz = 0
			Vector3 n = Vector3.Normalize(normal);
			double a = n.X;
			double b = n.Y;
			double c = n.Z;
			return new Matrix3
			(
				1.0 - 2.0 * a * a, -2.0 * a * b, -2.0 * a * c,
				-2.0 * a * b, 1.0 - 2.0 * b * b, -2.0 * b * c,
				-2.0 * a * c, -2.0 * b * c, 1.0 - 2.0 * c * c
			);
		}

		#endregion

		#region comparison methods

		/// <inheritdoc/>
		public override bool Equals(object obj) => obj is Matrix3 other && this.Equals(other);
		/// <inheritdoc/>
		public bool Equals(Matrix3 other) => this.Equals(other, MathHelper.Epsilon);
		/// <summary>Check if the components of two matrices are approximate equal.</summary>
		/// <param name="obj">Matrix3.</param>
		/// <param name="threshold">Maximum tolerance.</param>
		/// <returns><see langword="true"/> if the matrix components are almost equal or <see langword="false"/> in any other case.</returns>
		public bool Equals(Matrix3 obj, double threshold)
		{
			return
				MathHelper.IsEqual(obj.M11, this.M11, threshold) &&
				MathHelper.IsEqual(obj.M12, this.M12, threshold) &&
				MathHelper.IsEqual(obj.M13, this.M13, threshold) &&
				MathHelper.IsEqual(obj.M21, this.M21, threshold) &&
				MathHelper.IsEqual(obj.M22, this.M22, threshold) &&
				MathHelper.IsEqual(obj.M23, this.M23, threshold) &&
				MathHelper.IsEqual(obj.M31, this.M31, threshold) &&
				MathHelper.IsEqual(obj.M32, this.M32, threshold) &&
				MathHelper.IsEqual(obj.M33, this.M33, threshold);
		}
		/// <summary>Check if the components of two matrices are approximate equal.</summary>
		/// <param name="a">Matrix3.</param>
		/// <param name="b">Matrix3.</param>
		/// <returns><see langword="true"/> if the matrix components are almost equal or <see langword="false"/> in any other case.</returns>
		public static bool Equals(Matrix3 a, Matrix3 b) => a.Equals(b, MathHelper.Epsilon);
		/// <summary>Check if the components of two matrices are approximate equal.</summary>
		/// <param name="a">Matrix3.</param>
		/// <param name="b">Matrix3.</param>
		/// <param name="threshold">Maximum tolerance.</param>
		/// <returns><see langword="true"/> if the matrix components are almost equal or <see langword="false"/> in any other case.</returns>
		public static bool Equals(Matrix3 a, Matrix3 b, double threshold) => a.Equals(b, threshold);

		/// <inheritdoc/>
		public override int GetHashCode()
			=> this.M11.GetHashCode() ^ this.M12.GetHashCode() ^ this.M13.GetHashCode()
			^ this.M21.GetHashCode() ^ this.M22.GetHashCode() ^ this.M23.GetHashCode()
			^ this.M31.GetHashCode() ^ this.M32.GetHashCode() ^ this.M33.GetHashCode();

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override string ToString()
		{
			string separator = Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator;
			StringBuilder s = new StringBuilder();
			s.Append(string.Format("|{0}{3} {1}{3} {2}|" + Environment.NewLine, _M11, _M12, _M13, separator));
			s.Append(string.Format("|{0}{3} {1}{3} {2}|" + Environment.NewLine, _M21, _M22, _M23, separator));
			s.Append(string.Format("|{0}{3} {1}{3} {2}|", _M31, _M32, _M33, separator));
			return s.ToString();
		}

		/// <summary>Obtains a string that represents the matrix.</summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> object implementation that supplies culture-specific formatting information. </param>
		/// <returns>A string text.</returns>
		public string ToString(IFormatProvider provider)
		{
			string separator = Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator;
			StringBuilder s = new StringBuilder();
			s.Append(string.Format("|{0}{3} {1}{3} {2}|" + Environment.NewLine, _M11.ToString(provider), _M12.ToString(provider), _M13.ToString(provider), separator));
			s.Append(string.Format("|{0}{3} {1}{3} {2}|" + Environment.NewLine, _M21.ToString(provider), _M22.ToString(provider), _M23.ToString(provider), separator));
			s.Append(string.Format("|{0}{3} {1}{3} {2}|", _M31.ToString(provider), _M32.ToString(provider), _M33.ToString(provider), separator));
			return s.ToString();
		}

		#endregion
	}
}