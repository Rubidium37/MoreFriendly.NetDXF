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
using System.Threading;

namespace netDxf
{
	/// <summary>Represent a four component vector of double precision.</summary>
	public struct Vector4 :
		IEquatable<Vector4>
	{
		#region constructors

		/// <summary>Initializes a new instance of Vector4.</summary>
		/// <param name="x">X component.</param>
		/// <param name="y">Y component.</param>
		/// <param name="z">Z component.</param>
		/// <param name="w">W component.</param>
		public Vector4(double x, double y, double z, double w)
		{
			_X = x;
			_Y = y;
			_Z = z;
			_W = w;
			this.IsNormalized = false;
		}
		/// <summary>Initializes a new instance of Vector4.</summary>
		/// <param name="array">Array of four elements that represents the vector.</param>
		public Vector4(double[] array)
		{
			if (array == null)
			{
				throw new ArgumentNullException(nameof(array));
			}

			if (array.Length != 4)
			{
				throw new ArgumentOutOfRangeException(nameof(array), array.Length, "The dimension of the array must be four.");
			}

			_X = array[0];
			_Y = array[1];
			_Z = array[2];
			_W = array[3];
			this.IsNormalized = false;
		}

		#endregion

		#region constants

		/// <summary>Zero vector.</summary>
		public static Vector4 Zero => new Vector4(0.0, 0.0, 0.0, 0.0);

		/// <summary>Unit X vector.</summary>
		public static Vector4 UnitX => new Vector4(1.0, 0.0, 0.0, 0.0) { IsNormalized = true };

		/// <summary>Unit Y vector.</summary>
		public static Vector4 UnitY => new Vector4(0.0, 1.0, 0.0, 0.0) { IsNormalized = true };

		/// <summary>Unit Z vector.</summary>
		public static Vector4 UnitZ => new Vector4(0, 0, 1, 0) { IsNormalized = true };

		/// <summary>Unit W vector.</summary>
		public static Vector4 UnitW => new Vector4(0.0, 0.0, 0.0, 1.0) { IsNormalized = true };

		/// <summary>Represents a vector with not a number components.</summary>
		public static Vector4 NaN => new Vector4(double.NaN, double.NaN, double.NaN, double.NaN);

		#endregion

		#region public properties

		private double _X;
		/// <summary>Gets or sets the X component.</summary>
		public double X
		{
			get => _X;
			set
			{
				_X = value;
				this.IsNormalized = false;
			}
		}

		private double _Y;
		/// <summary>Gets or sets the Y component.</summary>
		public double Y
		{
			get => _Y;
			set
			{
				_Y = value;
				this.IsNormalized = false;
			}
		}

		private double _Z;
		/// <summary>Gets or sets the Z component.</summary>
		public double Z
		{
			get => _Z;
			set
			{
				_Z = value;
				this.IsNormalized = false;
			}
		}

		private double _W;
		/// <summary>Gets or sets the W component.</summary>
		public double W
		{
			get => _W;
			set
			{
				_W = value;
				this.IsNormalized = false;
			}
		}

		/// <summary>Gets or sets a vector element defined by its index.</summary>
		/// <param name="index">Index of the element.</param>
		public double this[int index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return _X;
					case 1:
						return _Y;
					case 2:
						return _Z;
					case 3:
						return _W;
					default:
						throw new ArgumentOutOfRangeException(nameof(index));
				}
			}
			set
			{
				switch (index)
				{
					case 0:
						_X = value;
						break;
					case 1:
						_Y = value;
						break;
					case 2:
						_Z = value;
						break;
					case 3:
						_W = value;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(index));
				}

				this.IsNormalized = false;
			}
		}

		/// <summary>Gets if the vector has been normalized.</summary>
		public bool IsNormalized { get; private set; }

		#endregion

		#region static methods

		/// <summary>Returns a value indicating if any component of the specified vector evaluates to a value that is not a number <see cref="System.Double.NaN"/>.</summary>
		/// <param name="u">Vector4.</param>
		/// <returns>Returns <see langword="true"/> if any component of the specified vector evaluates to <see cref="System.Double.NaN"/>; otherwise, <see langword="false"/>.</returns>
		public static bool IsNaN(Vector4 u) => double.IsNaN(u.X) || double.IsNaN(u.Y) || double.IsNaN(u.Z) || double.IsNaN(u.W);

		/// <summary>Returns a value indicating if all components of the specified vector evaluates to zero.</summary>
		/// <param name="u">Vector4.</param>
		/// <returns>Returns <see langword="true"/> if all components of the specified vector evaluates to zero; otherwise, <see langword="false"/>.</returns>
		public static bool IsZero(Vector4 u)
			=> MathHelper.IsZero(u.X) && MathHelper.IsZero(u.Y) && MathHelper.IsZero(u.Z) && MathHelper.IsZero(u.W);

		/// <summary>Obtains the dot product of two vectors.</summary>
		/// <param name="u">Vector4.</param>
		/// <param name="v">Vector4.</param>
		/// <returns>The dot product.</returns>
		public static double DotProduct(Vector4 u, Vector4 v) => u.X * v.X + u.Y * v.Y + u.Z * v.Z + u.W * v.W;

		/// <summary>Obtains the distance between two points.</summary>
		/// <param name="u">Vector4.</param>
		/// <param name="v">Vector4.</param>
		/// <returns>Distance.</returns>
		public static double Distance(Vector4 u, Vector4 v) => Math.Sqrt(SquareDistance(u, v));

		/// <summary>Obtains the square distance between two points.</summary>
		/// <param name="u">Vector4.</param>
		/// <param name="v">Vector4.</param>
		/// <returns>Square distance.</returns>
		public static double SquareDistance(Vector4 u, Vector4 v)
			=> (u.X - v.X) * (u.X - v.X) + (u.Y - v.Y) * (u.Y - v.Y) + (u.Z - v.Z) * (u.Z - v.Z) + (u.W - v.Z) * (u.W - v.W);

		/// <summary>Rounds the components of a vector.</summary>
		/// <param name="u">Vector to round.</param>
		/// <param name="numDigits">Number of decimal places in the return value.</param>
		/// <returns>The rounded vector.</returns>
		public static Vector4 Round(Vector4 u, int numDigits)
			=> new Vector4(Math.Round(u.X, numDigits), Math.Round(u.Y, numDigits), Math.Round(u.Z, numDigits), Math.Round(u.W, numDigits));

		/// <summary>Normalizes the vector.</summary>
		/// <param name="u">Vector to normalize</param>
		/// <returns>The normalized vector.</returns>
		public static Vector4 Normalize(Vector4 u)
		{
			if (u.IsNormalized)
			{
				return u;
			}

			double mod = u.Modulus();
			if (MathHelper.IsZero(mod))
			{
				return Zero;
			}

			double modInv = 1 / mod;
			return new Vector4(u.X * modInv, u.Y * modInv, u.Z * modInv, u.W * modInv) { IsNormalized = true };
		}

		#endregion

		#region overloaded operators

		/// <summary>Check if the components of two vectors are equal.</summary>
		/// <param name="u">Vector4.</param>
		/// <param name="v">Vector4.</param>
		/// <returns><see langword="true"/> if the three components are equal or <see langword="false"/> in any other case.</returns>
		public static bool operator ==(Vector4 u, Vector4 v) => Equals(u, v);
		/// <summary>Check if the components of two vectors are different.</summary>
		/// <param name="u">Vector4.</param>
		/// <param name="v">Vector4.</param>
		/// <returns><see langword="true"/> if the three components are different or <see langword="false"/> in any other case.</returns>
		public static bool operator !=(Vector4 u, Vector4 v) => !Equals(u, v);

		/// <summary>Adds two vectors.</summary>
		/// <param name="u">Vector4.</param>
		/// <param name="v">Vector4.</param>
		/// <returns>The addition of u plus v.</returns>
		public static Vector4 operator +(Vector4 u, Vector4 v) => new Vector4(u.X + v.X, u.Y + v.Y, u.Z + v.Z, u.W + v.W);

		/// <summary>Adds two vectors.</summary>
		/// <param name="u">Vector4.</param>
		/// <param name="v">Vector4.</param>
		/// <returns>The addition of u plus v.</returns>
		public static Vector4 Add(Vector4 u, Vector4 v) => new Vector4(u.X + v.X, u.Y + v.Y, u.Z + v.Z, u.W + v.W);

		/// <summary>Subtracts two vectors.</summary>
		/// <param name="u">Vector4.</param>
		/// <param name="v">Vector4.</param>
		/// <returns>The subtraction of u minus v.</returns>
		public static Vector4 operator -(Vector4 u, Vector4 v) => new Vector4(u.X - v.X, u.Y - v.Y, u.Z - v.Z, u.W - v.W);

		/// <summary>Subtracts two vectors.</summary>
		/// <param name="u">Vector4.</param>
		/// <param name="v">Vector4.</param>
		/// <returns>The subtraction of u minus v.</returns>
		public static Vector4 Subtract(Vector4 u, Vector4 v) => new Vector4(u.X - v.X, u.Y - v.Y, u.Z - v.Z, u.W - v.W);

		/// <summary>Negates a vector.</summary>
		/// <param name="u">Vector4.</param>
		/// <returns>The negative vector of u.</returns>
		public static Vector4 operator -(Vector4 u) => new Vector4(-u.X, -u.Y, -u.Z, -u.W) { IsNormalized = u.IsNormalized };

		/// <summary>Negates a vector.</summary>
		/// <param name="u">Vector4.</param>
		/// <returns>The negative vector of u.</returns>
		public static Vector4 Negate(Vector4 u) => new Vector4(-u.X, -u.Y, -u.Z, -u.W) { IsNormalized = u.IsNormalized };

		/// <summary>Multiplies a vector with an scalar (same as a*u, commutative property).</summary>
		/// <param name="u">Vector4.</param>
		/// <param name="a">Scalar.</param>
		/// <returns>The multiplication of u times a.</returns>
		public static Vector4 operator *(Vector4 u, double a) => new Vector4(u.X * a, u.Y * a, u.Z * a, u.W * a);

		/// <summary>Multiplies a vector with an scalar (same as a*u, commutative property).</summary>
		/// <param name="u">Vector4.</param>
		/// <param name="a">Scalar.</param>
		/// <returns>The multiplication of u times a.</returns>
		public static Vector4 Multiply(Vector4 u, double a) => new Vector4(u.X * a, u.Y * a, u.Z * a, u.W * a);

		/// <summary>Multiplies a scalar with a vector (same as u*a, commutative property).</summary>
		/// <param name="a">Scalar.</param>
		/// <param name="u">Vector4.</param>
		/// <returns>The multiplication of u times a.</returns>
		public static Vector4 operator *(double a, Vector4 u) => new Vector4(u.X * a, u.Y * a, u.Z * a, u.W * a);

		/// <summary>Multiplies a scalar with a vector (same as u*a, commutative property).</summary>
		/// <param name="a">Scalar.</param>
		/// <param name="u">Vector4.</param>
		/// <returns>The multiplication of u times a.</returns>
		public static Vector4 Multiply(double a, Vector4 u) => new Vector4(u.X * a, u.Y * a, u.Z * a, u.W * a);

		/// <summary>Multiplies two vectors component by component.</summary>
		/// <param name="u">Vector4.</param>
		/// <param name="v">Vector4.</param>
		/// <returns>The multiplication of u times v.</returns>
		public static Vector4 operator *(Vector4 u, Vector4 v) => new Vector4(u.X * v.X, u.Y * v.Y, u.Z * v.Z, u.W * v.W);

		/// <summary>Multiplies two vectors component by component.</summary>
		/// <param name="u">Vector4.</param>
		/// <param name="v">Vector4.</param>
		/// <returns>The multiplication of u times v.</returns>
		public static Vector4 Multiply(Vector4 u, Vector4 v) => new Vector4(u.X * v.X, u.Y * v.Y, u.Z * v.Z, u.W * v.W);

		/// <summary>Divides an scalar with a vector.</summary>
		/// <param name="a">Scalar.</param>
		/// <param name="u">Vector4.</param>
		/// <returns>The division of u times a.</returns>
		public static Vector4 operator /(Vector4 u, double a)
		{
			double invScalar = 1 / a;
			return new Vector4(u.X * invScalar, u.Y * invScalar, u.Z * invScalar, u.W * invScalar);
		}

		/// <summary>Divides a vector with an scalar.</summary>
		/// <param name="u">Vector4.</param>
		/// <param name="a">Scalar.</param>
		/// <returns>The division of u times a.</returns>
		public static Vector4 Divide(Vector4 u, double a)
		{
			double invScalar = 1 / a;
			return new Vector4(u.X * invScalar, u.Y * invScalar, u.Z * invScalar, u.W * invScalar);
		}

		/// <summary>Divides two vectors component by component.</summary>
		/// <param name="u">Vector4.</param>
		/// <param name="v">Vector4.</param>
		/// <returns>The multiplication of u times v.</returns>
		public static Vector4 operator /(Vector4 u, Vector4 v) => new Vector4(u.X / v.X, u.Y / v.Y, u.Z / v.Z, u.W / v.W);

		/// <summary>Divides two vectors component by component.</summary>
		/// <param name="u">Vector4.</param>
		/// <param name="v">Vector4.</param>
		/// <returns>The multiplication of u times v.</returns>
		public static Vector4 Divide(Vector4 u, Vector4 v) => new Vector4(u.X / v.X, u.Y / v.Y, u.Z / v.Z, u.W / v.W);

		#endregion

		#region public methods

		/// <summary>Normalizes the current vector.</summary>
		public void Normalize()
		{
			if (this.IsNormalized)
			{
				return;
			}

			double mod = this.Modulus();
			if (MathHelper.IsZero(mod))
			{
				this = Zero;
				return;
			}

			double modInv = 1 / mod;
			_X *= modInv;
			_Y *= modInv;
			_Z *= modInv;
			_W *= modInv;

			this.IsNormalized = true;
		}

		/// <summary>Obtains the modulus of the vector.</summary>
		/// <returns>Vector modulus.</returns>
		public double Modulus() => this.IsNormalized ? 1.0 : Math.Sqrt(DotProduct(this, this));

		/// <summary>Returns an array that represents the vector.</summary>
		/// <returns>Array.</returns>
		public double[] ToArray() => new[] { _X, _Y, _Z };

		#endregion

		#region comparison methods

		/// <inheritdoc/>
		public override bool Equals(object obj) => obj is Vector4 other && this.Equals(other);
		/// <inheritdoc/>
		public bool Equals(Vector4 other) => this.Equals(other, MathHelper.Epsilon);
		/// <summary>Check if the components of two vectors are approximate equal.</summary>
		/// <param name="other">Vector4.</param>
		/// <param name="threshold">Maximum tolerance.</param>
		/// <returns><see langword="true"/> if the four components are almost equal or <see langword="false"/> in any other case.</returns>
		public bool Equals(Vector4 other, double threshold)
		{
			return MathHelper.IsEqual(other.X, this.X, threshold) &&
					MathHelper.IsEqual(other.Y, this.Y, threshold) &&
					MathHelper.IsEqual(other.Z, this.Z, threshold) &&
					MathHelper.IsEqual(other.W, this.W, threshold);
		}
		/// <summary>Check if the components of two vectors are approximate equal.</summary>
		/// <param name="a">Vector4.</param>
		/// <param name="b">Vector4.</param>
		/// <returns><see langword="true"/> if the four components are almost equal or <see langword="false"/> in any other case.</returns>
		public static bool Equals(Vector4 a, Vector4 b) => a.Equals(b, MathHelper.Epsilon);
		/// <summary>Check if the components of two vectors are approximate equal.</summary>
		/// <param name="a">Vector4.</param>
		/// <param name="b">Vector4.</param>
		/// <param name="threshold">Maximum tolerance.</param>
		/// <returns><see langword="true"/> if the four components are almost equal or <see langword="false"/> in any other case.</returns>
		public static bool Equals(Vector4 a, Vector4 b, double threshold) => a.Equals(b, threshold);

		/// <inheritdoc/>
		public override int GetHashCode() => this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode() ^ this.W.GetHashCode();

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override string ToString()
			=> string.Format("{0}{4} {1}{4} {2}{4} {3}", _X, _Y, _Z, _W, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator);

		/// <summary>Obtains a string that represents the vector.</summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param>
		/// <returns>A string text.</returns>
		public string ToString(IFormatProvider provider)
			=> string.Format("{0}{4} {1}{4} {2}{4} {3}", _X.ToString(provider), _Y.ToString(provider), _Z.ToString(provider), _W.ToString(provider), Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator);

		#endregion
	}
}