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
	/// <summary>Represent a three component vector of double precision.</summary>
	public struct Vector3 :
		IEquatable<Vector3>
	{
		#region constructors

		/// <summary>Initializes a new instance of Vector3.</summary>
		/// <param name="value">X, Y, Z component.</param>
		public Vector3(double value)
		{
			_X = value;
			_Y = value;
			_Z = value;
			this.IsNormalized = false;
		}
		/// <summary>Initializes a new instance of Vector3.</summary>
		/// <param name="x">X component.</param>
		/// <param name="y">Y component.</param>
		/// <param name="z">Z component.</param>
		public Vector3(double x, double y, double z)
		{
			_X = x;
			_Y = y;
			_Z = z;
			this.IsNormalized = false;
		}
		/// <summary>Initializes a new instance of Vector3.</summary>
		/// <param name="array">Array of three elements that represents the vector.</param>
		public Vector3(double[] array)
		{
			if (array == null)
			{
				throw new ArgumentNullException(nameof(array));
			}

			if (array.Length != 3)
			{
				throw new ArgumentOutOfRangeException(nameof(array), array.Length, "The dimension of the array must be three.");
			}

			_X = array[0];
			_Y = array[1];
			_Z = array[2];
			this.IsNormalized = false;
		}

		#endregion

		#region constants

		/// <summary>Zero vector.</summary>
		public static Vector3 Zero => new Vector3(0.0, 0.0, 0.0);

		/// <summary>Unit X vector.</summary>
		public static Vector3 UnitX => new Vector3(1.0, 0.0, 0.0) { IsNormalized = true };

		/// <summary>Unit Y vector.</summary>
		public static Vector3 UnitY => new Vector3(0.0, 1.0, 0.0) { IsNormalized = true };

		/// <summary>Unit Z vector.</summary>
		public static Vector3 UnitZ => new Vector3(0.0, 0.0, 1.0) { IsNormalized = true };

		/// <summary>Represents a vector with not a number components.</summary>
		public static Vector3 NaN => new Vector3(double.NaN, double.NaN, double.NaN);

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
		/// <param name="u">Vector3.</param>
		/// <returns>Returns <see langword="true"/> if any component of the specified vector evaluates to <see cref="System.Double.NaN"/>; otherwise, <see langword="false"/>.</returns>
		public static bool IsNaN(Vector3 u) => double.IsNaN(u.X) || double.IsNaN(u.Y) || double.IsNaN(u.Z);

		/// <summary>Returns a value indicating if all components of the specified vector evaluates to zero.</summary>
		/// <param name="u">Vector3.</param>
		/// <returns>Returns <see langword="true"/> if all components of the specified vector evaluates to zero; otherwise, <see langword="false"/>.</returns>
		public static bool IsZero(Vector3 u) => MathHelper.IsZero(u.X) && MathHelper.IsZero(u.Y) && MathHelper.IsZero(u.Z);

		/// <summary>Obtains the dot product of two vectors.</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="v">Vector3.</param>
		/// <returns>The dot product.</returns>
		public static double DotProduct(Vector3 u, Vector3 v) => u.X * v.X + u.Y * v.Y + u.Z * v.Z;

		/// <summary>Obtains the cross product of two vectors.</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="v">Vector3.</param>
		/// <returns>The cross product.</returns>
		public static Vector3 CrossProduct(Vector3 u, Vector3 v)
		{
			double a = u.Y * v.Z - u.Z * v.Y;
			double b = u.Z * v.X - u.X * v.Z;
			double c = u.X * v.Y - u.Y * v.X;
			return new Vector3(a, b, c);
		}

		/// <summary>Obtains the distance between two points.</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="v">Vector3.</param>
		/// <returns>Distance.</returns>
		public static double Distance(Vector3 u, Vector3 v) => Math.Sqrt(SquareDistance(u, v));

		/// <summary>Obtains the square distance between two points.</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="v">Vector3.</param>
		/// <returns>Square distance.</returns>
		public static double SquareDistance(Vector3 u, Vector3 v)
			=> (u.X - v.X) * (u.X - v.X) + (u.Y - v.Y) * (u.Y - v.Y) + (u.Z - v.Z) * (u.Z - v.Z);

		/// <summary>Obtains the angle between two vectors.</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="v">Vector3.</param>
		/// <returns>Angle in radians.</returns>
		public static double AngleBetween(Vector3 u, Vector3 v)
		{
			double cos = DotProduct(u, v) / (u.Modulus() * v.Modulus());
			if (cos >= 1.0)
			{
				return 0.0;
			}

			if (cos <= -1.0)
			{
				return Math.PI;
			}

			return Math.Acos(cos);
		}

		/// <summary>Rotate given vector around the specified axis.</summary>
		/// <param name="v">Vector to rotate.</param>
		/// <param name="axis">Rotation axis.</param>
		/// <param name="angle">Rotation angle in radians.</param>
		/// <returns>The rotated vector.</returns>
		/// <remarks>Method provided by: Idelana. Original Author: Paul Bourke ( http://paulbourke.net/geometry/rotate/ )</remarks>
		public static Vector3 RotateAroundAxis(Vector3 v, Vector3 axis, double angle)
		{
			Vector3 q = new Vector3();
			axis.Normalize();
			double cos = Math.Cos(angle);
			double sin = Math.Sin(angle);

			q.X += (cos + (1 - cos) * axis.X * axis.X) * v.X;
			q.X += ((1 - cos) * axis.X * axis.Y - axis.Z * sin) * v.Y;
			q.X += ((1 - cos) * axis.X * axis.Z + axis.Y * sin) * v.Z;

			q.Y += ((1 - cos) * axis.X * axis.Y + axis.Z * sin) * v.X;
			q.Y += (cos + (1 - cos) * axis.Y * axis.Y) * v.Y;
			q.Y += ((1 - cos) * axis.Y * axis.Z - axis.X * sin) * v.Z;

			q.Z += ((1 - cos) * axis.X * axis.Z - axis.Y * sin) * v.X;
			q.Z += ((1 - cos) * axis.Y * axis.Z + axis.X * sin) * v.Y;
			q.Z += (cos + (1 - cos) * axis.Z * axis.Z) * v.Z;

			return q;
		}

		/// <summary>Obtains the midpoint.</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="v">Vector3.</param>
		/// <returns>Vector3.</returns>
		public static Vector3 MidPoint(Vector3 u, Vector3 v)
			=> new Vector3((v.X + u.X) * 0.5, (v.Y + u.Y) * 0.5, (v.Z + u.Z) * 0.5);

		/// <summary>Checks if two vectors are perpendicular.</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="v">Vector3.</param>
		/// <returns><see langword="true"/> if are perpendicular or <see langword="false"/> in any other case.</returns>
		public static bool ArePerpendicular(Vector3 u, Vector3 v) => ArePerpendicular(u, v, MathHelper.Epsilon);
		/// <summary>Checks if two vectors are perpendicular.</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="v">Vector3.</param>
		/// <param name="threshold">Tolerance used.</param>
		/// <returns><see langword="true"/> if are perpendicular or <see langword="false"/> in any other case.</returns>
		public static bool ArePerpendicular(Vector3 u, Vector3 v, double threshold)
			=> MathHelper.IsZero(DotProduct(u, v), threshold);

		/// <summary>Checks if two vectors are parallel.</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="v">Vector3.</param>
		/// <returns><see langword="true"/> if are parallel or <see langword="false"/> in any other case.</returns>
		public static bool AreParallel(Vector3 u, Vector3 v) => AreParallel(u, v, MathHelper.Epsilon);
		/// <summary>Checks if two vectors are parallel.</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="v">Vector3.</param>
		/// <param name="threshold">Tolerance used.</param>
		/// <returns><see langword="true"/> if are parallel or <see langword="false"/> in any other case.</returns>
		public static bool AreParallel(Vector3 u, Vector3 v, double threshold)
		{
			Vector3 cross = CrossProduct(u, v);

			if (!MathHelper.IsZero(cross.X, threshold))
			{
				return false;
			}

			if (!MathHelper.IsZero(cross.Y, threshold))
			{
				return false;
			}

			if (!MathHelper.IsZero(cross.Z, threshold))
			{
				return false;
			}

			return true;
		}

		/// <summary>Rounds the components of a vector.</summary>
		/// <param name="u">Vector to round.</param>
		/// <param name="numDigits">Number of decimal places in the return value.</param>
		/// <returns>The rounded vector.</returns>
		public static Vector3 Round(Vector3 u, int numDigits)
			=> new Vector3(Math.Round(u.X, numDigits), Math.Round(u.Y, numDigits), Math.Round(u.Z, numDigits));

		/// <summary>Normalizes the vector.</summary>
		/// <param name="u">Vector to normalize</param>
		/// <returns>A normalized vector.</returns>
		public static Vector3 Normalize(Vector3 u)
		{
			if (u.IsNormalized)
			{
				return u;
			}

			double mod = u.Modulus();
			if (MathHelper.IsZero(mod))
			{
				return NaN;
			}

			double modInv = 1 / mod;
			return new Vector3(u.X * modInv, u.Y * modInv, u.Z * modInv) { IsNormalized = true };
		}

		#endregion

		#region overloaded operators

		/// <summary>Check if the components of two vectors are equal.</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="v">Vector3.</param>
		/// <returns><see langword="true"/> if the three components are equal or <see langword="false"/> in any other case.</returns>
		public static bool operator ==(Vector3 u, Vector3 v) => Equals(u, v);
		/// <summary>Check if the components of two vectors are different.</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="v">Vector3.</param>
		/// <returns><see langword="true"/> if the three components are different or <see langword="false"/> in any other case.</returns>
		public static bool operator !=(Vector3 u, Vector3 v) => !Equals(u, v);

		/// <summary>Adds two vectors.</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="v">Vector3.</param>
		/// <returns>The addition of u plus v.</returns>
		public static Vector3 operator +(Vector3 u, Vector3 v) => new Vector3(u.X + v.X, u.Y + v.Y, u.Z + v.Z);

		/// <summary>Adds two vectors.</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="v">Vector3.</param>
		/// <returns>The addition of u plus v.</returns>
		public static Vector3 Add(Vector3 u, Vector3 v) => new Vector3(u.X + v.X, u.Y + v.Y, u.Z + v.Z);

		/// <summary>Subtracts two vectors.</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="v">Vector3.</param>
		/// <returns>The subtraction of u minus v.</returns>
		public static Vector3 operator -(Vector3 u, Vector3 v) => new Vector3(u.X - v.X, u.Y - v.Y, u.Z - v.Z);

		/// <summary>Subtracts two vectors.</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="v">Vector3.</param>
		/// <returns>The subtraction of u minus v.</returns>
		public static Vector3 Subtract(Vector3 u, Vector3 v) => new Vector3(u.X - v.X, u.Y - v.Y, u.Z - v.Z);

		/// <summary>Negates a vector.</summary>
		/// <param name="u">Vector3.</param>
		/// <returns>The negative vector of u.</returns>
		public static Vector3 operator -(Vector3 u) => new Vector3(-u.X, -u.Y, -u.Z) { IsNormalized = u.IsNormalized };

		/// <summary>Negates a vector.</summary>
		/// <param name="u">Vector3.</param>
		/// <returns>The negative vector of u.</returns>
		public static Vector3 Negate(Vector3 u) => new Vector3(-u.X, -u.Y, -u.Z) { IsNormalized = u.IsNormalized };

		/// <summary>Multiplies a vector with an scalar (same as a*u, commutative property).</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="a">Scalar.</param>
		/// <returns>The multiplication of u times a.</returns>
		public static Vector3 operator *(Vector3 u, double a) => new Vector3(u.X * a, u.Y * a, u.Z * a);

		/// <summary>Multiplies a vector with an scalar (same as a*u, commutative property).</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="a">Scalar.</param>
		/// <returns>The multiplication of u times a.</returns>
		public static Vector3 Multiply(Vector3 u, double a) => new Vector3(u.X * a, u.Y * a, u.Z * a);

		/// <summary>Multiplies a scalar with a vector (same as u*a, commutative property).</summary>
		/// <param name="a">Scalar.</param>
		/// <param name="u">Vector3.</param>
		/// <returns>The multiplication of u times a.</returns>
		public static Vector3 operator *(double a, Vector3 u) => new Vector3(u.X * a, u.Y * a, u.Z * a);

		/// <summary>Multiplies a scalar with a vector (same as u*a, commutative property).</summary>
		/// <param name="a">Scalar.</param>
		/// <param name="u">Vector3.</param>
		/// <returns>The multiplication of u times a.</returns>
		public static Vector3 Multiply(double a, Vector3 u) => new Vector3(u.X * a, u.Y * a, u.Z * a);

		/// <summary>Multiplies two vectors component by component.</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="v">Vector3.</param>
		/// <returns>The multiplication of u times v.</returns>
		public static Vector3 operator *(Vector3 u, Vector3 v) => new Vector3(u.X * v.X, u.Y * v.Y, u.Z * v.Z);

		/// <summary>Multiplies two vectors component by component.</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="v">Vector3.</param>
		/// <returns>The multiplication of u times v.</returns>
		public static Vector3 Multiply(Vector3 u, Vector3 v) => new Vector3(u.X * v.X, u.Y * v.Y, u.Z * v.Z);

		/// <summary>Divides an scalar with a vector.</summary>
		/// <param name="a">Scalar.</param>
		/// <param name="u">Vector3.</param>
		/// <returns>The division of u times a.</returns>
		public static Vector3 operator /(Vector3 u, double a)
		{
			double invScalar = 1 / a;
			return new Vector3(u.X * invScalar, u.Y * invScalar, u.Z * invScalar);
		}

		/// <summary>Divides a vector with an scalar.</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="a">Scalar.</param>
		/// <returns>The division of u times a.</returns>
		public static Vector3 Divide(Vector3 u, double a)
		{
			double invScalar = 1 / a;
			return new Vector3(u.X * invScalar, u.Y * invScalar, u.Z * invScalar);
		}

		/// <summary>Divides two vectors component by component.</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="v">Vector3.</param>
		/// <returns>The multiplication of u times v.</returns>
		public static Vector3 operator /(Vector3 u, Vector3 v) => new Vector3(u.X / v.X, u.Y / v.Y, u.Z / v.Z);

		/// <summary>Divides two vectors component by component.</summary>
		/// <param name="u">Vector3.</param>
		/// <param name="v">Vector3.</param>
		/// <returns>The multiplication of u times v.</returns>
		public static Vector3 Divide(Vector3 u, Vector3 v) => new Vector3(u.X / v.X, u.Y / v.Y, u.Z / v.Z);

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
		public override bool Equals(object obj) => obj is Vector3 other && this.Equals(other);
		/// <inheritdoc/>
		public bool Equals(Vector3 other) => this.Equals(other, MathHelper.Epsilon);
		/// <summary>Check if the components of two vectors are approximate equal.</summary>
		/// <param name="other">Vector3.</param>
		/// <param name="threshold">Maximum tolerance.</param>
		/// <returns><see langword="true"/> if the three components are almost equal or <see langword="false"/> in any other case.</returns>
		public bool Equals(Vector3 other, double threshold)
			=> MathHelper.IsEqual(other.X, _X, threshold) && MathHelper.IsEqual(other.Y, _Y, threshold) && MathHelper.IsEqual(other.Z, _Z, threshold);
		/// <summary>Check if the components of two vectors are approximate equal.</summary>
		/// <param name="a">Vector3.</param>
		/// <param name="b">Vector3.</param>
		/// <returns><see langword="true"/> if the three components are almost equal or <see langword="false"/> in any other case.</returns>
		public static bool Equals(Vector3 a, Vector3 b) => a.Equals(b, MathHelper.Epsilon);
		/// <summary>Check if the components of two vectors are approximate equal.</summary>
		/// <param name="a">Vector3.</param>
		/// <param name="b">Vector3.</param>
		/// <param name="threshold">Maximum tolerance.</param>
		/// <returns><see langword="true"/> if the three components are almost equal or <see langword="false"/> in any other case.</returns>
		public static bool Equals(Vector3 a, Vector3 b, double threshold) => a.Equals(b, threshold);

		/// <inheritdoc/>
		public override int GetHashCode() => this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode();

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override string ToString()
			=> string.Format("{0}{3} {1}{3} {2}", _X, _Y, _Z, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator);

		/// <summary>Obtains a string that represents the vector.</summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> object implementation that supplies culture-specific formatting information. </param>
		/// <returns>A string text.</returns>
		public string ToString(IFormatProvider provider)
			=> string.Format("{0}{3} {1}{3} {2}", _X.ToString(provider), _Y.ToString(provider), _Z.ToString(provider), Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator);

		#endregion
	}
}