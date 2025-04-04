﻿#region netDxf library licensed under the MIT License
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

// This is a translation to C# from the original C++ code of the Geometric Tool Library
// Original license
// David Eberly, Geometric Tools, Redmond WA 98052
// Copyright (c) 1998-2022
// Distributed under the Boost Software License, Version 1.0.
// https://www.boost.org/LICENSE_1_0.txt
// https://www.geometrictools.com/License/Boost/LICENSE_1_0.txt
// Version: 6.0.2022.01.06

using System;
using System.Diagnostics;

namespace netDxf.GTE
{
	public class GMatrix :
		IEquatable<GMatrix>
	{
		//// The table is length zero and mNumRows and mNumCols are set to zero.
		//public GMatrix()
		//{
		//}

		// The table is length numRows*numCols and the elements are
		// initialized to zero.
		public GMatrix(int numRows, int numCols)
		{
			this.NumRows = numRows;
			this.NumCols = numCols;
			this.Elements = new GVector(numRows * numCols);
		}

		// For 0 <= r < numRows and 0 <= c < numCols, element (r,c) is 1 and
		// all others are 0.  If either of r or c is invalid, the zero matrix
		// is created.  This is a convenience for creating the standard
		// Euclidean basis matrices; see also MakeUnit(int,int) and
		// Unit(int,int).
		public GMatrix(int numRows, int numCols, int r, int c)
		{
			this.NumRows = numRows;
			this.NumCols = numCols;
			this.Elements = new GVector(numRows * numCols);

			this.MakeUnit(r, c);
		}
		public GMatrix(int numRows, int numCols, double[] elements)
		{
			this.NumRows = numRows;
			this.NumCols = numCols;
			this.Elements = new GVector(elements);
		}
		// The copy constructor, destructor, and assignment operator are
		// generated by the compiler.

		// Member access for which the storage representation is transparent.
		// The matrix entry in row r and column c is A(r,c).  The first
		// operator() returns a const reference rather than a double value.
		// This supports writing via standard file operations that require a
		// const pointer to data.
		//void SetSize(int numRows, int numCols)
		//{
		//	if (numRows > 0 && numCols > 0)
		//	{
		//		mNumRows = numRows;
		//		mNumCols = numCols;
		//		mElements.resize(static_cast<size_t>(mNumRows) * static_cast<size_t>(mNumCols));
		//	}
		//	else
		//	{
		//		mNumRows = 0;
		//		mNumCols = 0;
		//		mElements.clear();
		//	}
		//}

		public void GetSize(out int rows, out int cols)
		{
			rows = this.NumRows;
			cols = this.NumCols;
		}

		public int NumRows { get; }

		public int NumCols { get; }

		public int NumElements => this.Elements.Size;

		public GVector Elements { get; }

		public double this[int r, int c]
		{
			get
			{
				if (0 <= r && r < this.NumRows && 0 <= c && c < this.NumCols)
				{
					return GTE.UseRowMajor ? this.Elements[c + this.NumCols * r] : this.Elements[r + this.NumRows * c];
				}

				Debug.Assert(false, "Invalid index.");
				return double.NaN;
			}
			set
			{
				if (0 <= r && r < this.NumRows && 0 <= c && c < this.NumCols)
				{
					if (GTE.UseRowMajor)
					{
						this.Elements[c + this.NumCols * r] = value;
					}
					else
					{
						this.Elements[r + this.NumRows * c] = value;
					}
				}
				else
				{
					Debug.Assert(false, "Invalid index.");
				}
			}
		}

		// Member access by rows or by columns.  The input vectors must have
		// the correct number of elements for the matrix size.
		public void SetRow(int r, GVector vec)
		{
			if (0 <= r && r < this.NumRows)
			{
				if (vec.Size == this.NumCols)
				{
					for (int c = 0; c < this.NumCols; ++c)
					{
						this[r, c] = vec[c];
					}

					return;
				}

				Debug.Assert(false, "Mismatched sizes.");
			}

			Debug.Assert(false, "Invalid index.");
		}

		public void SetCol(int c, GVector vec)
		{
			if (0 <= c && c < this.NumCols)
			{
				if (vec.Size == this.NumRows)
				{
					for (int r = 0; r < this.NumRows; ++r)
					{
						this[r, c] = vec[r];
					}

					return;
				}

				Debug.Assert(false, "Mismatched sizes.");
			}

			Debug.Assert(false, "Invalid index.");
		}

		public GVector GetRow(int r)
		{
			if (0 <= r && r < this.NumRows)
			{
				GVector vec = new GVector(this.NumCols);
				for (int c = 0; c < this.NumCols; ++c)
				{
					vec[c] = this[r, c];
				}

				return vec;
			}

			Debug.Assert(false, "Invalid index.");
			return new GVector(0);
		}

		public GVector GetCol(int c)
		{
			if (0 <= c && c < this.NumCols)
			{
				GVector vec = new GVector(this.NumRows);
				for (int r = 0; r < this.NumRows; ++r)
				{
					vec[r] = this[r, c];
				}

				return vec;
			}

			Debug.Assert(false, "Invalid index.");
			return new GVector(0);
		}

		// Member access by 1-dimensional index.  NOTE: These accessors are
		// useful for the manipulation of matrix entries when it does not
		// matter whether storage is row-major or column-major.  Do not use
		// constructs such as M[c+NumCols*r] or M[r+NumRows*c] that expose the
		// storage convention.
		public double this[int i]
		{
			get => this.Elements[i];
			set => this.Elements[i] = value;
		}

		// Comparisons for sorted containers and geometric ordering.
		public static bool operator ==(GMatrix mat1, GMatrix mat2)
		{
			if (mat1 == null || mat2 == null)
			{
				return false;
			}
			return mat1.NumRows == mat2.NumRows && mat1.NumCols == mat2.NumCols && mat1.Elements == mat2.Elements;
		}
		public static bool operator !=(GMatrix mat1, GMatrix mat2)
		{
			if (mat1 == null || mat2 == null)
			{
				return false;
			}
			return mat1.NumRows == mat2.NumRows && mat1.NumCols == mat2.NumCols && mat1.Elements != mat2.Elements;
		}
		public static bool operator <(GMatrix mat1, GMatrix mat2)
			=> mat1.NumRows == mat2.NumRows && mat1.NumCols == mat2.NumCols && mat1.Elements < mat2.Elements;
		public static bool operator <=(GMatrix mat1, GMatrix mat2)
			=> mat1.NumRows == mat2.NumRows && mat1.NumCols == mat2.NumCols && mat1.Elements <= mat2.Elements;
		public static bool operator >(GMatrix mat1, GMatrix mat2)
			=> mat1.NumRows == mat2.NumRows && mat1.NumCols == mat2.NumCols && mat1.Elements > mat2.Elements;
		public static bool operator >=(GMatrix mat1, GMatrix mat2)
			=> mat1.NumRows == mat2.NumRows && mat1.NumCols == mat2.NumCols && mat1.Elements >= mat2.Elements;

		// Special matrices.

		// All components are 0.
		public void MakeZero() => this.Elements.MakeZero();

		// Component (r,c) is 1, all others zero.
		public void MakeUnit(int r, int c)
		{
			if (0 <= r && r < this.NumRows && 0 <= c && c < this.NumCols)
			{
				this.MakeZero();
				this[r, c] = 1.0;
				return;
			}

			Debug.Assert(false, "Invalid index.");
		}

		// Diagonal entries 1, others 0, even when non square.
		public void MakeIdentity()
		{
			this.MakeZero();
			int numDiagonal = this.NumRows <= this.NumCols ? this.NumRows : this.NumCols;
			for (int i = 0; i < numDiagonal; ++i)
			{
				this[i, i] = 1.0;
			}
		}

		public static GMatrix Zero(int numRows, int numCols)
		{
			GMatrix M = new GMatrix(numRows, numCols);
			M.MakeZero();
			return M;
		}

		public static GMatrix Unit(int numRows, int numCols, int r, int c)
		{
			GMatrix M = new GMatrix(numRows, numCols);
			M.MakeUnit(r, c);
			return M;
		}

		public static GMatrix Identity(int numRows, int numCols)
		{
			GMatrix M = new GMatrix(numRows, numCols);
			M.MakeIdentity();
			return M;
		}

		// Unary operations.

		// Linear-algebraic operations.
		public static GMatrix operator +(GMatrix m1, GMatrix m2)
		{
			if (m1.NumRows == m2.NumRows && m1.NumCols == m2.NumCols)
			{
				GMatrix result = new GMatrix(m1.NumRows, m1.NumCols);
				for (int i = 0; i < result.NumElements; i++)
				{
					result[i] = m1.Elements[i] + m2.Elements[i];
				}

				return result;
			}

			Debug.Assert(false, "Mismatched sizes");
			return new GMatrix(0, 0);
		}

		public static GMatrix operator -(GMatrix m1, GMatrix m2)
		{
			if (m1.NumRows == m2.NumRows && m1.NumCols == m2.NumCols)
			{
				GMatrix result = new GMatrix(m1.NumRows, m1.NumCols);
				for (int i = 0; i < result.NumElements; i++)
				{
					result[i] = m1.Elements[i] - m2.Elements[i];
				}

				return result;
			}

			Debug.Assert(false, "Mismatched sizes");
			return new GMatrix(0, 0);
		}

		public static GMatrix operator *(double scalar, GMatrix m)
		{
			GMatrix result = new GMatrix(m.NumRows, m.NumCols);
			for (int i = 0; i < result.NumElements; i++)
			{
				result[i] = scalar * m.Elements[i];
			}

			return result;
		}

		public static GMatrix operator *(GMatrix m, double scalar) => scalar * m;

		public static GMatrix operator /(GMatrix m, double scalar) => m * (1 / scalar);

		// Geometric operations.
		public static double L1Norm(GMatrix m)
		{
			double sum = 0.0;
			for (int i = 0; i < m.NumElements; i++)
			{
				sum += Math.Abs(m[i]);
			}

			return sum;
		}

		public static double L2Norm(GMatrix m)
		{
			double sum = 0.0;
			for (int i = 0; i < m.NumElements; i++)
			{
				sum += m[i] * m[i];
			}

			return Math.Sqrt(sum);
		}

		public static double LInfinityNorm(GMatrix m)
		{
			double maxAbsElement = 0.0;
			for (int i = 0; i < m.NumElements; i++)
			{
				double absElement = Math.Abs(m[i]);
				if (absElement > maxAbsElement)
				{
					maxAbsElement = absElement;
				}
			}

			return maxAbsElement;
		}

		public static GMatrix Inverse(GMatrix m, out bool invertible)
		{
			invertible = false;
			if (m.NumRows == m.NumCols)
			{
				double[] invM = new double[m.NumRows * m.NumCols];
				invertible = GaussianElimination.Solve(m.NumRows, m.Elements.Vector, invM, out _, null, null, null, 0, null);
				return new GMatrix(m.NumRows, m.NumCols, invM);
			}

			Debug.Assert(false, "Matrix must be square.");
			return new GMatrix(0, 0);
		}

		public static double Determinant(GMatrix m)
		{
			if (m.NumRows == m.NumCols)
			{
				GaussianElimination.Solve(m.NumRows, m.Elements.Vector, null, out double determinant, null, null, null, 0, null);
				return determinant;
			}

			Debug.Assert(false, "Matrix must be square.");
			return double.NaN;
		}

		// M^T
		public static GMatrix Transpose(GMatrix m)
		{
			GMatrix result = new GMatrix(m.NumCols, m.NumRows);
			for (int r = 0; r < m.NumRows; ++r)
			{
				for (int c = 0; c < m.NumCols; ++c)
				{
					result[c, r] = m[r, c];
				}
			}

			return result;
		}

		// M*V
		public static GVector operator *(GMatrix m, GVector v)
		{
			if (v.Size == m.NumCols)
			{
				GVector result = new GVector(m.NumRows);
				for (int r = 0; r < m.NumRows; ++r)
				{
					result[r] = 0;
					for (int c = 0; c < m.NumCols; ++c)
					{
						result[r] += m[r, c] * v[c];
					}
				}

				return result;
			}

			Debug.Assert(false, "Mismatched sizes.");
			return new GVector(0);
		}

		// V^T*M
		public static GVector operator *(GVector v, GMatrix m)
		{
			if (v.Size == m.NumRows)
			{
				GVector result = new GVector(m.NumCols);
				for (int c = 0; c < m.NumCols; ++c)
				{
					result[c] = 0.0;
					for (int r = 0; r < m.NumRows; ++r)
					{
						result[c] += v[r] * m[r, c];
					}
				}

				return result;
			}

			Debug.Assert(false, "Mismatched sizes.");
			return new GVector(0);
		}

		// A*B
		public static GMatrix operator *(GMatrix a, GMatrix b) => MultiplyAB(a, b);

		public static GMatrix MultiplyAB(GMatrix a, GMatrix b)
		{
			if (a.NumCols == b.NumRows)
			{
				GMatrix result = new GMatrix(a.NumRows, b.NumCols);
				int numCommon = a.NumCols;
				for (int r = 0; r < result.NumRows; ++r)
				{
					for (int c = 0; c < result.NumCols; ++c)
					{
						result[r, c] = 0.0;
						for (int i = 0; i < numCommon; ++i)
						{
							result[r, c] += a[r, i] * b[i, c];
						}
					}
				}

				return result;
			}

			Debug.Assert(false, "Mismatched sizes.");
			return new GMatrix(0, 0);
		}

		// A*B^T
		public static GMatrix MultiplyABT(GMatrix a, GMatrix b)
		{
			if (a.NumCols == b.NumCols)
			{
				GMatrix result = new GMatrix(a.NumRows, b.NumRows);
				int numCommon = a.NumCols;
				for (int r = 0; r < result.NumRows; ++r)
				{
					for (int c = 0; c < result.NumCols; ++c)
					{
						result[r, c] = 0.0;
						for (int i = 0; i < numCommon; ++i)
						{
							result[r, c] += a[r, i] * b[c, i];
						}
					}
				}

				return result;
			}

			Debug.Assert(false, "Mismatched sizes.");
			return new GMatrix(0, 0);
		}

		// A^T*B
		public static GMatrix MultiplyATB(GMatrix a, GMatrix b)
		{
			if (a.NumRows == b.NumRows)
			{
				GMatrix result = new GMatrix(a.NumCols, b.NumCols);
				int numCommon = a.NumRows;
				for (int r = 0; r < result.NumRows; ++r)
				{
					for (int c = 0; c < result.NumCols; ++c)
					{
						result[r, c] = 0;
						for (int i = 0; i < numCommon; ++i)
						{
							result[r, c] += a[i, r] * b[i, c];
						}
					}
				}

				return result;
			}

			Debug.Assert(false, "Mismatched sizes.");
			return new GMatrix(0, 0);
		}

		// A^T*B^T
		public static GMatrix MultiplyATBT(GMatrix a, GMatrix b)
		{
			if (a.NumRows == b.NumCols)
			{
				GMatrix result = new GMatrix(a.NumCols, b.NumRows);
				int numCommon = a.NumRows;
				for (int r = 0; r < result.NumRows; ++r)
				{
					for (int c = 0; c < result.NumCols; ++c)
					{
						result[r, c] = 0;
						for (int i = 0; i < numCommon; ++i)
						{
							result[r, c] += a[i, r] * b[c, i];
						}
					}
				}

				return result;
			}

			Debug.Assert(false, "Mismatched sizes.");
			return new GMatrix(0, 0);
		}

		// M*D, D is square diagonal (stored as vector)
		public static GMatrix MultiplyMD(GMatrix m, GVector d)
		{
			if (d.Size == m.NumCols)
			{
				GMatrix result = new GMatrix(m.NumRows, m.NumCols);
				for (int r = 0; r < result.NumRows; ++r)
				{
					for (int c = 0; c < result.NumCols; ++c)
					{
						result[r, c] = m[r, c] * d[c];
					}
				}

				return result;
			}

			Debug.Assert(false, "Mismatched sizes.");
			return new GMatrix(0, 0);
		}

		// D*M, D is square diagonal (stored as vector)
		public static GMatrix MultiplyDM(GVector d, GMatrix m)
		{
			if (d.Size == m.NumRows)
			{
				GMatrix result = new GMatrix(m.NumRows, m.NumCols);
				for (int r = 0; r < result.NumRows; ++r)
				{
					for (int c = 0; c < result.NumCols; ++c)
					{
						result[r, c] = d[r] * m[r, c];
					}
				}

				return result;
			}

			Debug.Assert(false, "Mismatched sizes.");
			return new GMatrix(0, 0);
		}

		// U*V^T, U is N-by-1, V is M-by-1, result is N-by-M.
		public static GMatrix OuterProduct(GVector u, GVector v)
		{
			GMatrix result = new GMatrix(u.Size, v.Size);
			for (int r = 0; r < result.NumRows; ++r)
			{
				for (int c = 0; c < result.NumCols; ++c)
				{
					result[r, c] = u[r] * v[c];
				}
			}

			return result;
		}

		// Initialization to a diagonal matrix whose diagonal entries are the
		// components of D, even when nonsquare.
		public static void MakeDiagonal(GVector d, GMatrix m)
		{
			int numRows = m.NumRows;
			int numCols = m.NumCols;
			int numDiagonal = numRows <= numCols ? numRows : numCols;
			m.MakeZero();
			for (int i = 0; i < numDiagonal; ++i)
			{
				m[i, i] = d[i];
			}
		}

		/// <inheritdoc/>
		public override bool Equals(object obj) => obj is GMatrix other && this.Equals(other);
		/// <inheritdoc/>
		public bool Equals(GMatrix other)
		{
			if (other == null)
			{
				return false;
			}

			return this == other;
		}

		/// <inheritdoc/>
		public override int GetHashCode() => this.Elements.GetHashCode();
	}
}