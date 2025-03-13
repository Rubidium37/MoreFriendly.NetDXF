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
	/// <summary>Represents a 2x2 double precision matrix.</summary>
	public struct Matrix2 :
		IEquatable<Matrix2>
	{
		private bool IsDirty;

		#region constructors

		/// <summary>Initializes a new instance of Matrix2.</summary>
		/// <param name="m11">Element [0,0].</param>
		/// <param name="m12">Element [0,1].</param>
		/// <param name="m21">Element [1,0].</param>
		/// <param name="m22">Element [1,1].</param>
		public Matrix2(double m11, double m12, double m21, double m22)
		{
			_M11 = m11;
			_M12 = m12;

			_M21 = m21;
			_M22 = m22;

			this.IsDirty = true;
			_IsIdentity = false;
		}

		#endregion

		#region constants

		/// <summary>Gets the zero matrix.</summary>
		public static Matrix2 Zero
			=> new Matrix2
			(
				0.0, 0.0,
				0.0, 0.0
			)
			{
				IsDirty = false,
				_IsIdentity = false
			};

		/// <summary>Gets the identity matrix.</summary>
		public static Matrix2 Identity
			=> new Matrix2
			(
				1.0, 0.0,
				0.0, 1.0
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

					_IsIdentity = true;
					return _IsIdentity;
				}
				return _IsIdentity;
			}
		}

		#endregion

		#region overloaded operators

		/// <summary>Matrix addition.</summary>
		/// <param name="a">Matrix2.</param>
		/// <param name="b">Matrix2.</param>
		/// <returns>Matrix2.</returns>
		public static Matrix2 operator +(Matrix2 a, Matrix2 b)
			=> new Matrix2
			(
				a.M11 + b.M11, a.M12 + b.M12,
				a.M21 + b.M21, a.M22 + b.M22
			);

		/// <summary>Matrix addition.</summary>
		/// <param name="a">Matrix2.</param>
		/// <param name="b">Matrix2.</param>
		/// <returns>Matrix2.</returns>
		public static Matrix2 Add(Matrix2 a, Matrix2 b)
			=> new Matrix2
			(
				a.M11 + b.M11, a.M12 + b.M12,
				a.M21 + b.M21, a.M22 + b.M22
			);

		/// <summary>Matrix subtraction.</summary>
		/// <param name="a">Matrix2.</param>
		/// <param name="b">Matrix2.</param>
		/// <returns>Matrix2.</returns>
		public static Matrix2 operator -(Matrix2 a, Matrix2 b)
			=> new Matrix2
			(
				a.M11 - b.M11, a.M12 - b.M12,
				a.M21 - b.M21, a.M22 - b.M22
			);

		/// <summary>Matrix subtraction.</summary>
		/// <param name="a">Matrix2.</param>
		/// <param name="b">Matrix2.</param>
		/// <returns>Matrix2.</returns>
		public static Matrix2 Subtract(Matrix2 a, Matrix2 b)
			=> new Matrix2
			(
				a.M11 - b.M11, a.M12 - b.M12,
				a.M21 - b.M21, a.M22 - b.M22
			);

		/// <summary>Product of two matrices.</summary>
		/// <param name="a">Matrix2.</param>
		/// <param name="b">Matrix2.</param>
		/// <returns>Matrix2.</returns>
		public static Matrix2 operator *(Matrix2 a, Matrix2 b)
		{
			if (a.IsIdentity)
			{
				return b;
			}

			if (b.IsIdentity)
			{
				return a;
			}

			return new Matrix2
			(
				a.M11 * b.M11 + a.M12 * b.M21, a.M11 * b.M12 + a.M12 * b.M22,
				a.M21 * b.M11 + a.M22 * b.M21, a.M21 * b.M12 + a.M22 * b.M22
			);
		}

		/// <summary>Product of two matrices.</summary>
		/// <param name="a">Matrix2.</param>
		/// <param name="b">Matrix2.</param>
		/// <returns>Matrix2.</returns>
		public static Matrix2 Multiply(Matrix2 a, Matrix2 b)
		{
			if (a.IsIdentity)
			{
				return b;
			}

			if (b.IsIdentity)
			{
				return a;
			}

			return new Matrix2
			(
				a.M11 * b.M11 + a.M12 * b.M21, a.M11 * b.M12 + a.M12 * b.M22,
				a.M21 * b.M11 + a.M22 * b.M21, a.M21 * b.M12 + a.M22 * b.M22
			);
		}

		/// <summary>Product of a matrix with a vector.</summary>
		/// <param name="a">Matrix2.</param>
		/// <param name="u">Vector2.</param>
		/// <returns>Matrix2.</returns>
		/// <remarks>Matrix2 adopts the convention of using column vectors.</remarks>
		public static Vector2 operator *(Matrix2 a, Vector2 u)
			=> a.IsIdentity
			? u
			: new Vector2
			(
				a.M11 * u.X + a.M12 * u.Y,
				a.M21 * u.X + a.M22 * u.Y
			);

		/// <summary>Product of a matrix with a vector.</summary>
		/// <param name="a">Matrix2.</param>
		/// <param name="u">Vector2.</param>
		/// <returns>Matrix2.</returns>
		/// <remarks>Matrix2 adopts the convention of using column vectors.</remarks>
		public static Vector2 Multiply(Matrix2 a, Vector2 u)
			=> a.IsIdentity
			? u
			: new Vector2
			(
				a.M11 * u.X + a.M12 * u.Y,
				a.M21 * u.X + a.M22 * u.Y
			);

		/// <summary>Product of a matrix with a scalar.</summary>
		/// <param name="m">Matrix2.</param>
		/// <param name="a">Scalar.</param>
		/// <returns>Matrix2.</returns>
		public static Matrix2 operator *(Matrix2 m, double a)
			=> new Matrix2
			(
				m.M11 * a, m.M12 * a,
				m.M21 * a, m.M22 * a
			);

		/// <summary>Product of a matrix with a scalar.</summary>
		/// <param name="m">Matrix2.</param>
		/// <param name="a">Scalar.</param>
		/// <returns>Matrix2.</returns>
		public static Matrix2 Multiply(Matrix2 m, double a)
			=> new Matrix2
			(
				m.M11 * a, m.M12 * a,
				m.M21 * a, m.M22 * a
			);

		/// <summary>Check if the components of two matrices are equal.</summary>
		/// <param name="u">Matrix2.</param>
		/// <param name="v">Matrix2.</param>
		/// <returns><see langword="true"/> if the matrix components are equal or <see langword="false"/> in any other case.</returns>
		public static bool operator ==(Matrix2 u, Matrix2 v) => Equals(u, v);
		/// <summary>Check if the components of two matrices are different.</summary>
		/// <param name="u">Matrix2.</param>
		/// <param name="v">Matrix2.</param>
		/// <returns><see langword="true"/> if the matrix components are different or <see langword="false"/> in any other case.</returns>
		public static bool operator !=(Matrix2 u, Matrix2 v) => !Equals(u, v);

		#endregion

		#region public methods

		/// <summary>Calculate the determinant of the actual matrix.</summary>
		/// <returns>Determinant.</returns>
		public double Determinant() => this.IsIdentity ? 1.0 : _M11 * _M22 - _M12 * _M21;

		/// <summary>Calculates the inverse matrix.</summary>
		/// <returns>Inverse Matrix2.</returns>
		public Matrix2 Inverse()
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

			return new Matrix2
			(
				det * _M22, -det * _M12,
				-det * _M21, det * _M11
			);
		}

		/// <summary>Obtains the transpose matrix.</summary>
		/// <returns>Transpose matrix.</returns>
		public Matrix2 Transpose()
			=> this.IsIdentity
			? Identity
			: new Matrix2
			(
				_M11, _M21,
				_M12, _M22
			);

		#endregion

		#region static methods

		/// <summary>Builds a rotation matrix for a rotation.</summary>
		/// <param name="angle">The counter-clockwise angle in radians.</param>
		/// <returns>The resulting <see cref="Matrix2"/> instance.</returns>
		/// <remarks><see cref="Matrix2"/> adopts the convention of using column vectors to represent a transformation matrix.</remarks>
		public static Matrix2 Rotation(double angle)
		{
			double cos = Math.Cos(angle);
			double sin = Math.Sin(angle);
			return new Matrix2
			(
				cos, -sin,
				sin, cos
			);
		}

		/// <summary>Build a scaling matrix.</summary>
		/// <param name="value">Single scale factor for x and y axis.</param>
		/// <returns>A scaling matrix.</returns>
		public static Matrix2 Scale(double value) => Scale(value, value);
		/// <summary>Build a scaling matrix.</summary>
		/// <param name="value">Scale factors for x and y axis.</param>
		/// <returns>A scaling matrix.</returns>
		public static Matrix2 Scale(Vector2 value) => Scale(value.X, value.Y);
		/// <summary>Build a scaling matrix.</summary>
		/// <param name="x">Scale factor for x-axis.</param>
		/// <param name="y">Scale factor for y-axis.</param>
		/// <returns>A scaling matrix.</returns>
		public static Matrix2 Scale(double x, double y)
			=> new Matrix2
			(
				x, 0.0,
				0.0, y
			);

		#endregion

		#region comparison methods

		/// <inheritdoc/>
		public override bool Equals(object obj) => obj is Matrix2 other && this.Equals(other);
		/// <inheritdoc/>
		public bool Equals(Matrix2 other) => this.Equals(other, MathHelper.Epsilon);
		/// <summary>Check if the components of two matrices are approximate equal.</summary>
		/// <param name="obj">Matrix2.</param>
		/// <param name="threshold">Maximum tolerance.</param>
		/// <returns><see langword="true"/> if the matrix components are almost equal or <see langword="false"/> in any other case.</returns>
		public bool Equals(Matrix2 obj, double threshold)
		{
			return
				MathHelper.IsEqual(obj.M11, this.M11, threshold) &&
				MathHelper.IsEqual(obj.M12, this.M12, threshold) &&
				MathHelper.IsEqual(obj.M21, this.M21, threshold) &&
				MathHelper.IsEqual(obj.M22, this.M22, threshold);
		}
		/// <summary>Check if the components of two matrices are approximate equal.</summary>
		/// <param name="a">Matrix2.</param>
		/// <param name="b">Matrix2.</param>
		/// <returns><see langword="true"/> if the matrix components are almost equal or <see langword="false"/> in any other case.</returns>
		public static bool Equals(Matrix2 a, Matrix2 b) => a.Equals(b, MathHelper.Epsilon);
		/// <summary>Check if the components of two matrices are approximate equal.</summary>
		/// <param name="a">Matrix2.</param>
		/// <param name="b">Matrix2.</param>
		/// <param name="threshold">Maximum tolerance.</param>
		/// <returns><see langword="true"/> if the matrix components are almost equal or <see langword="false"/> in any other case.</returns>
		public static bool Equals(Matrix2 a, Matrix2 b, double threshold) => a.Equals(b, threshold);

		/// <inheritdoc/>
		public override int GetHashCode()
			=> this.M11.GetHashCode() ^ this.M12.GetHashCode()
			^ this.M21.GetHashCode() ^ this.M22.GetHashCode();

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override string ToString()
		{
			string separator = Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator;
			StringBuilder s = new StringBuilder();
			s.Append(string.Format("|{0}{2} {1}|" + Environment.NewLine, _M11, _M12, separator));
			s.Append(string.Format("|{0}{2} {1}|", _M21, _M22, separator));
			return s.ToString();
		}

		/// <summary>Obtains a string that represents the matrix.</summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> object implementation that supplies culture-specific formatting information. </param>
		/// <returns>A string text.</returns>
		public string ToString(IFormatProvider provider)
		{
			string separator = Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator;
			StringBuilder s = new StringBuilder();
			s.Append(string.Format("|{0}{2} {1}|" + Environment.NewLine, _M11.ToString(provider), _M12.ToString(provider), separator));
			s.Append(string.Format("|{0}{2} {1}|", _M21.ToString(provider), _M22.ToString(provider), separator));
			return s.ToString();
		}

		#endregion
	}
}