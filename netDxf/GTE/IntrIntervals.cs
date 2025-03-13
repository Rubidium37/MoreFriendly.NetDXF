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

// This is a translation to C# from the original C++ code of the Geometric Tool Library
// Original license
// David Eberly, Geometric Tools, Redmond WA 98052
// Copyright (c) 1998-2022
// Distributed under the Boost Software License, Version 1.0.
// https://www.boost.org/LICENSE_1_0.txt
// https://www.geometrictools.com/License/Boost/LICENSE_1_0.txt
// Version: 6.0.2022.01.06

using System;

namespace netDxf.GTE
{
	// The intervals are of the form [t0,t1], [t0,+infinity) or (-infinity,t1].
	// Degenerate intervals are allowed (t0 = t1). The queries do not perform
	// validation on the input intervals to test whether t0 <= t1.

	// The query tests overlap, whether a single point or an entire
	// interval.

	public class TIQueryIntervals
	{
		// Dynamic queries (intervals moving with constant speeds). If
		// 'intersect' is true, the contact times are valid and
		//     0 <= firstTime <= lastTime,  firstTime <= maxTime
		// If 'intersect' is false, there are two cases reported. If the
		// intervals will intersect at firstTime > maxTime, the contact
		// times are reported just as when 'intersect' is true. However,
		// if the intervals will not intersect, then firstTime and
		// lastTime are both set to zero (invalid because 'intersect' is
		// false).

		public bool Intersect { get; } = false;

		public double FirstTime { get; } = 0.0;

		public double LastTime { get; } = 0.0;

		private TIQueryIntervals()
		{
		}

		// Static query. The firstTime and lastTime values are set to zero by
		// the this constructor, but they are invalid for the static query
		// regardless of the value of 'intersect'.
		public TIQueryIntervals(double[] interval0, double[] interval1)
			: this()
		{
			this.Intersect = interval0[0] <= interval1[1] && interval0[1] >= interval1[0];
		}

		// Static queries where at least one interval is semiinfinite. The
		// two types of semiinfinite intervals are [a,+infinity), which I call
		// a positive-infinite interval, and (-infinity,a], which I call a
		// negative-infinite interval. The firstTime and lastTime values are
		// set to zero by the this constructor, but they are invalid for the
		// static query regardless of the value of 'intersect'.
		public TIQueryIntervals(double[] finite, double a, bool isPositiveInfinite)
			: this()
		{
			if (isPositiveInfinite)
			{
				this.Intersect = finite[1] >= a;
			}
			else // is negative-infinite
			{
				this.Intersect = finite[0] <= a;
			}

		}
		public TIQueryIntervals(double a0, bool isPositiveInfinite0, double a1, bool isPositiveInfinite1)
			: this()
		{
			if (isPositiveInfinite0)
			{
				if (isPositiveInfinite1)
				{
					this.Intersect = true;
				}
				else // interval1 is negative-infinite
				{
					this.Intersect = a0 <= a1;
				}
			}
			else // interval0 is negative-infinite
			{
				if (isPositiveInfinite1)
				{
					this.Intersect = a0 >= a1;
				}
				else // interval1 is negative-infinite
				{
					this.Intersect = true;
				}
			}
		}

		// Dynamic query. Current time is 0, maxTime > 0 is required.
		public TIQueryIntervals(double maxTime, double[] interval0, double speed0, double[] interval1, double speed1)
			: this()
		{
			double zero = 0.0;

			if (interval0[1] < interval1[0])
			{
				// interval0 initially to the left of interval1.
				double diffSpeed = speed0 - speed1;
				if (diffSpeed > zero)
				{
					// The intervals must move towards each other. 'intersect'
					// is true when the intervals will intersect by maxTime.
					double diffPos = interval1[0] - interval0[1];
					this.Intersect = (diffPos <= maxTime * diffSpeed);
					this.FirstTime = diffPos / diffSpeed;
					this.LastTime = (interval1[1] - interval0[0]) / diffSpeed;
				}
			}
			else if (interval0[0] > interval1[1])
			{
				// interval0 initially to the right of interval1.
				double diffSpeed = speed1 - speed0;
				if (diffSpeed > zero)
				{
					// The intervals must move towards each other. 'intersect'
					// is true when the intervals will intersect by maxTime.
					double diffPos = interval0[0] - interval1[1];
					this.Intersect = (diffPos <= maxTime * diffSpeed);
					this.FirstTime = diffPos / diffSpeed;
					this.LastTime = (interval0[1] - interval1[0]) / diffSpeed;
				}
			}
			else
			{
				// The intervals are initially intersecting.
				this.Intersect = true;
				this.FirstTime = zero;
				if (speed1 > speed0)
				{
					this.LastTime = (interval0[1] - interval1[0]) / (speed1 - speed0);
				}
				else if (speed1 < speed0)
				{
					this.LastTime = (interval1[1] - interval0[0]) / (speed0 - speed1);
				}
				else
				{
					this.LastTime = double.MaxValue;
				}
			}

			// The this constructor set 'intersect' to false and the
			// 'firstTime' and 'lastTime' to zero.
		}
	}

	public enum FIQueryIntervalsType
	{
		// No intersection.
		IsEmpty = 0,

		// Intervals touch at an endpoint, [t0,t0].
		IsPoint = 1,

		// Finite-length interval of intersection, [t0,t1].
		IsFinite = 2,

		// Smiinfinite interval of intersection, [t0,+infinity). The
		// this.Overlap[0] is t0 and this.Overlap[1] is +1 as a
		// message that the right endpoint is +infinity (you still need
		// the this.Type to know this interpretation).
		IsPositiveInfinite = 3,

		// Semiinfinite interval of intersection, (-infinity,t1]. The
		// this.Overlap[0] is -1 as a message that the left endpoint is
		// -infinity (you still need the this.Type to know this
		// interpretation). The this.Overlap[1] is t1.
		IsNegativeInfinite = 4,

		// The dynamic queries all set the type to isDynamicQuery because
		// the queries look for time of first and last contact.
		IsDynamicQuery = 5,
	}

	public class FIQueryIntervals
	{
		// The query finds overlap, whether a single point or an entire
		// interval.

		public bool Intersect { get; } = false;

		// Static queries (no motion of intervals over time). The number
		// of number of intersections is 0 (no overlap), 1 (intervals are
		// just touching), or 2 (intervals overlap in an interval). If
		// 'intersect' is false, numIntersections is 0 and 'overlap' is
		// set to [0,0]. If 'intersect' is true, numIntersections is
		// 1 or 2. When 1, 'overlap' is set to [x,x], which is degenerate
		// and represents the single intersection point x. When 2,
		// 'overlap' is the interval of intersection.
		public int NumIntersections { get; } = 0;

		public double[] Overlap { get; } = new[] { 0.0, 0.0 };

		// The type is one of isEmpty, isPoint, isFinite,
		// isPositiveInfinite, isNegativeInfinite or isDynamicQuery.
		public FIQueryIntervalsType Type { get; } = FIQueryIntervalsType.IsEmpty;

		// Dynamic queries (intervals moving with constant speeds). If
		// 'intersect' is true, the contact times are valid and
		//     0 <= firstTime <= lastTime,  firstTime <= maxTime
		// If 'intersect' is false, there are two cases reported. If the
		// intervals will intersect at firstTime > maxTime, the contact
		// times are reported just as when 'intersect' is true. However,
		// if the intervals will not intersect, then firstTime and
		// lastTime are both set to zero (invalid because 'intersect' is
		// false).
		public double FirstTime { get; } = 0.0;

		public double LastTime { get; } = 0.0;

		private FIQueryIntervals()
		{
		}
		// Static query.
		public FIQueryIntervals(double[] interval0, double[] interval1)
			: this()
		{
			if (interval0[1] < interval1[0] || interval0[0] > interval1[1])
			{
				this.NumIntersections = 0;
				this.Overlap[0] = 0.0;
				this.Overlap[1] = 0.0;
				this.Type = FIQueryIntervalsType.IsEmpty;
			}
			else if (interval0[1] > interval1[0])
			{
				if (interval0[0] < interval1[1])
				{
					this.Overlap[0] = (interval0[0] < interval1[0] ? interval1[0] : interval0[0]);
					this.Overlap[1] = (interval0[1] > interval1[1] ? interval1[1] : interval0[1]);
					if (this.Overlap[0] < this.Overlap[1])
					{
						this.NumIntersections = 2;
						this.Type = FIQueryIntervalsType.IsFinite;
					}
					else
					{
						this.NumIntersections = 1;
						this.Type = FIQueryIntervalsType.IsPoint;
					}
				}
				else // interval0[0] == interval1[1]
				{
					this.NumIntersections = 1;
					this.Overlap[0] = interval0[0];
					this.Overlap[1] = this.Overlap[0];
					this.Type = FIQueryIntervalsType.IsPoint;
				}
			}
			else // interval0[1] == interval1[0]
			{
				this.NumIntersections = 1;
				this.Overlap[0] = interval0[1];
				this.Overlap[1] = this.Overlap[0];
				this.Type = FIQueryIntervalsType.IsPoint;
			}

			this.Intersect = this.NumIntersections > 0;
		}
		// Static queries where at least one interval is semiinfinite. The
		// two types of semiinfinite intervals are [a,+infinity), which I call
		// a positive-infinite interval, and (-infinity,a], which I call a
		// negative-infinite interval.
		public FIQueryIntervals(double[] finite, double a, bool isPositiveInfinite)
			: this()
		{
			if (isPositiveInfinite)
			{
				if (finite[1] > a)
				{
					this.Overlap[0] = Math.Max(finite[0], a);
					this.Overlap[1] = finite[1];
					if (this.Overlap[0] < this.Overlap[1])
					{
						this.NumIntersections = 2;
						this.Type = FIQueryIntervalsType.IsFinite;
					}
					else
					{
						this.NumIntersections = 1;
						this.Type = FIQueryIntervalsType.IsPoint;
					}
				}
				else if (Math.Abs(finite[1] - a) < double.Epsilon)
				{
					this.NumIntersections = 1;
					this.Overlap[0] = a;
					this.Overlap[1] = this.Overlap[0];
					this.Type = FIQueryIntervalsType.IsPoint;
				}
				else
				{
					this.NumIntersections = 0;
					this.Overlap[0] = 0.0;
					this.Overlap[1] = 0.0;
					this.Type = FIQueryIntervalsType.IsEmpty;
				}
			}
			else // is negative-infinite
			{
				if (finite[0] < a)
				{
					this.Overlap[0] = finite[0];
					this.Overlap[1] = Math.Min(finite[1], a);
					if (this.Overlap[0] < this.Overlap[1])
					{
						this.NumIntersections = 2;
						this.Type = FIQueryIntervalsType.IsFinite;
					}
					else
					{
						this.NumIntersections = 1;
						this.Type = FIQueryIntervalsType.IsPoint;
					}
				}
				else if (Math.Abs(finite[0] - a) < double.Epsilon)
				{
					this.NumIntersections = 1;
					this.Overlap[0] = a;
					this.Overlap[1] = this.Overlap[0];
					this.Type = FIQueryIntervalsType.IsPoint;
				}
				else
				{
					this.NumIntersections = 0;
					this.Overlap[0] = 0.0;
					this.Overlap[1] = 0.0;
					this.Type = FIQueryIntervalsType.IsEmpty;
				}
			}

			this.Intersect = (this.NumIntersections > 0);
		}
		public FIQueryIntervals(double a0, bool isPositiveInfinite0, double a1, bool isPositiveInfinite1)
			: this()
		{
			if (isPositiveInfinite0)
			{
				if (isPositiveInfinite1)
				{
					// overlap[1] is +infinity, but set it to +1 because double
					// might not have a representation for +infinity. The
					// type indicates the interval is positive-infinite, so
					// the +1 is a reminder that overlap[1] is +infinity.
					this.NumIntersections = 1;
					this.Overlap[0] = Math.Max(a0, a1);
					this.Overlap[1] = 1.0;
					this.Type = FIQueryIntervalsType.IsPositiveInfinite;
				}
				else // interval1 is negative-infinite
				{
					if (a0 > a1)
					{
						this.NumIntersections = 0;
						this.Overlap[0] = 0.0;
						this.Overlap[1] = 0.0;
						this.Type = FIQueryIntervalsType.IsEmpty;
					}
					else if (a0 < a1)
					{
						this.NumIntersections = 2;
						this.Overlap[0] = a0;
						this.Overlap[1] = a1;
						this.Type = FIQueryIntervalsType.IsFinite;
					}
					else // a0 == a1
					{
						this.NumIntersections = 1;
						this.Overlap[0] = a0;
						this.Overlap[1] = this.Overlap[0];
						this.Type = FIQueryIntervalsType.IsPoint;
					}
				}
			}
			else // interval0 is negative-infinite
			{
				if (isPositiveInfinite1)
				{
					if (a0 < a1)
					{
						this.NumIntersections = 0;
						this.Overlap[0] = 0.0;
						this.Overlap[1] = 0.0;
						this.Type = FIQueryIntervalsType.IsEmpty;
					}
					else if (a0 > a1)
					{
						this.NumIntersections = 2;
						this.Overlap[0] = a1;
						this.Overlap[1] = a0;
						this.Type = FIQueryIntervalsType.IsFinite;
					}
					else
					{
						this.NumIntersections = 1;
						this.Overlap[0] = a1;
						this.Overlap[1] = this.Overlap[0];
						this.Type = FIQueryIntervalsType.IsPoint;
					}

					this.Intersect = a0 >= a1;
				}
				else // interval1 is negative-infinite
				{
					// overlap[0] is -infinity, but set it to -1 because double
					// might not have a representation for -infinity. The
					// type indicates the interval is negative-infinite, so
					// the -1 is a reminder that overlap[0] is -infinity.
					this.NumIntersections = 1;
					this.Overlap[0] = -1.0;
					this.Overlap[1] = Math.Min(a0, a1);
					this.Type = FIQueryIntervalsType.IsNegativeInfinite;
				}
			}

			this.Intersect = (this.NumIntersections > 0);
		}
		// Dynamic query. Current time is 0, maxTime > 0 is required.
		public FIQueryIntervals(double maxTime, double[] interval0, double speed0, double[] interval1, double speed1)
			: this()
		{
			this.Type = FIQueryIntervalsType.IsDynamicQuery;

			if (interval0[1] < interval1[0])
			{
				// interval0 initially to the left of interval1.
				double diffSpeed = speed0 - speed1;
				if (diffSpeed > 0.0)
				{
					// The intervals must move towards each other. 'intersect'
					// is true when the intervals will intersect by maxTime.
					double diffPos = interval1[0] - interval0[1];
					this.Intersect = (diffPos <= maxTime * diffSpeed);
					this.NumIntersections = 1;
					this.FirstTime = diffPos / diffSpeed;
					this.LastTime = (interval1[1] - interval0[0]) / diffSpeed;
					this.Overlap[0] = interval0[0] + this.FirstTime * speed0;
					this.Overlap[1] = this.Overlap[0];
				}
			}
			else if (interval0[0] > interval1[1])
			{
				// interval0 initially to the right of interval1.
				double diffSpeed = speed1 - speed0;
				if (diffSpeed > 0.0)
				{
					// The intervals must move towards each other. 'intersect'
					// is true when the intervals will intersect by maxTime.
					double diffPos = interval0[0] - interval1[1];
					this.Intersect = (diffPos <= maxTime * diffSpeed);
					this.NumIntersections = 1;
					this.FirstTime = diffPos / diffSpeed;
					this.LastTime = (interval0[1] - interval1[0]) / diffSpeed;
					this.Overlap[0] = interval1[1] + this.FirstTime * speed1;
					this.Overlap[1] = this.Overlap[0];
				}
			}
			else
			{
				// The intervals are initially intersecting.
				this.Intersect = true;
				this.FirstTime = 0.0;
				if (speed1 > speed0)
				{
					this.LastTime = (interval0[1] - interval1[0]) / (speed1 - speed0);
				}
				else if (speed1 < speed0)
				{
					this.LastTime = (interval1[1] - interval0[0]) / (speed0 - speed1);
				}
				else
				{
					this.LastTime = double.MaxValue;
				}

				if (interval0[1] > interval1[0])
				{
					if (interval0[0] < interval1[1])
					{
						this.NumIntersections = 2;
						this.Overlap[0] = (interval0[0] < interval1[0] ? interval1[0] : interval0[0]);
						this.Overlap[1] = (interval0[1] > interval1[1] ? interval1[1] : interval0[1]);
					}
					else // interval0[0] == interval1[1]
					{
						this.NumIntersections = 1;
						this.Overlap[0] = interval0[0];
						this.Overlap[1] = this.Overlap[0];
					}
				}
				else // interval0[1] == interval1[0]
				{
					this.NumIntersections = 1;
					this.Overlap[0] = interval0[1];
					this.Overlap[1] = this.Overlap[0];
				}
			}

			// The this constructor sets the correct state for no-intersection.
		}
	}
}