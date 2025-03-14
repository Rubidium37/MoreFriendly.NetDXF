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
using netDxf.Tables;

namespace netDxf.Entities
{
	/// <summary>Represents a spline curve <see cref="EntityObject">entity</see> (NURBS Non-Uniform Rational B-Splines).</summary>
	public class Spline :
		EntityObject
	{
		public const short MaxDegree = 10;

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="fitPoints">Spline fit points.</param>
		/// <remarks>
		/// The resulting spline curve will be created from a list of cubic bezier curves that passes through the specified fit points.
		/// </remarks>
		public Spline(IEnumerable<Vector3> fitPoints)
			: this(BezierCurveCubic.CreateFromFitPoints(fitPoints))
		{
			this.CreationMethod = SplineCreationMethod.FitPoints;
			_FitPoints = fitPoints.ToArray();
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="curves">List of cubic bezier curves.</param>
		public Spline(IEnumerable<BezierCurveQuadratic> curves)
			: this(curves, 2)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="curves">List of cubic bezier curves.</param>
		public Spline(IEnumerable<BezierCurveCubic> curves)
			: this(curves, 3)
		{
		}
		private Spline(IEnumerable<BezierCurve> curves, short degree)
			: base(EntityType.Spline, DxfObjectCode.Spline)
		{
			// control points and fit points
			List<Vector3> ctrList = new List<Vector3>();
			List<double> wList = new List<double>();

			foreach (BezierCurve curve in curves)
			{
				foreach (Vector3 point in curve.ControlPoints)
				{
					ctrList.Add(point);
					wList.Add(1.0);
				}
			}

			this.ControlPoints = ctrList.ToArray();
			this.Weights = wList.ToArray();
			this.Degree = degree;
			_IsClosedPeriodic = false;
			_FitPoints = new Vector3[0];
			this.CreationMethod = SplineCreationMethod.ControlPoints;
			this.Knots = ÇreateBezierKnotVector(this.ControlPoints.Length, this.Degree);
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="controlPoints">Spline control points.</param>
		/// <param name="weights">Spline control weights. Pass <see langword="null"/> to set the default weights as 1.0.</param>
		/// <remarks>By default the degree of the spline is equal three.</remarks>
		public Spline(IEnumerable<Vector3> controlPoints, IEnumerable<double> weights)
			: this(controlPoints, weights, 3, false)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="controlPoints">Spline control points.</param>
		/// <param name="weights">Spline control weights. Pass <see langword="null"/> to set the default weights as 1.0.</param>
		/// <param name="degree">Degree of the spline curve. Valid values are 1 (linear), degree 2 (quadratic), degree 3 (cubic), and so on up to degree 10.</param>
		public Spline(IEnumerable<Vector3> controlPoints, IEnumerable<double> weights, short degree)
			: this(controlPoints, weights, degree, false)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="controlPoints">Spline control points.</param>
		/// <param name="weights">Spline control weights. If <see langword="null"/> the weights vector will be automatically initialized with 1.0.</param>
		/// <param name="closedPeriodic">Sets if the spline as periodic closed (default: <see langword="false"/>).</param>
		/// <remarks>By default the degree of the spline is equal three.</remarks>
		public Spline(IEnumerable<Vector3> controlPoints, IEnumerable<double> weights, bool closedPeriodic)
			: this(controlPoints, weights, 3, closedPeriodic)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="controlPoints">Spline control points.</param>
		/// <param name="weights">Spline control weights. If <see langword="null"/> the weights vector will be automatically initialized with 1.0.</param>
		/// <param name="degree">Degree of the spline curve. Valid values are 1 (linear), degree 2 (quadratic), degree 3 (cubic), and so on up to degree 10.</param>
		/// <param name="closedPeriodic">Sets if the spline as periodic closed (default: <see langword="false"/>).</param>
		public Spline(IEnumerable<Vector3> controlPoints, IEnumerable<double> weights, short degree, bool closedPeriodic)
			: base(EntityType.Spline, DxfObjectCode.Spline)
		{
			// spline degree
			if (degree < 1 || degree > MaxDegree)
			{
				throw new ArgumentOutOfRangeException(nameof(degree), degree, "The spline degree valid values range from 1 to 10.");
			}
			this.Degree = degree;

			// control points
			if (controlPoints == null)
			{
				throw new ArgumentNullException(nameof(controlPoints));
			}

			// create control points
			this.ControlPoints = controlPoints.ToArray();
			if (this.ControlPoints.Length < 2)
			{
				throw new ArgumentException("The number of control points must be equal or greater than 2.");
			}

			if (this.ControlPoints.Length < degree + 1)
			{
				throw new ArgumentException("The number of control points must be equal or greater than the spline degree + 1.");
			}

			// create weights
			if (weights == null)
			{
				this.Weights = new double[this.ControlPoints.Length];
				for (int i = 0; i < this.ControlPoints.Length; i++)
				{
					this.Weights[i] = 1.0;
				}
			}
			else
			{
				this.Weights = weights.ToArray();
				if (this.Weights.Length != this.ControlPoints.Length)
				{
					throw new ArgumentException("The number of control points must be the same as the number of weights.", nameof(weights));
				}
			}

			_IsClosedPeriodic = closedPeriodic;
			this.CreationMethod = SplineCreationMethod.ControlPoints;
			_FitPoints = new Vector3[0];
			this.Knots = CreateKnotVector(this.ControlPoints.Length, this.Degree, _IsClosedPeriodic);
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="controlPoints">Spline control points.</param>
		/// <param name="weigths">Spline control weights. If <see langword="null"/> the weights vector will be automatically initialized with 1.0.</param>
		/// <param name="knots">Spline knot vector.</param>
		/// <param name="degree">Degree of the spline curve. Valid values are 1 (linear), degree 2 (quadratic), degree 3 (cubic), and so on up to degree 10.</param>
		/// <param name="closedPeriodic">Sets if the spline as periodic closed (default: <see langword="false"/>).</param>
		public Spline(IEnumerable<Vector3> controlPoints, IEnumerable<double> weigths, IEnumerable<double> knots, short degree, bool closedPeriodic)
			: this(controlPoints, weigths, knots, degree, null, SplineCreationMethod.ControlPoints, closedPeriodic)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="controlPoints">Spline control points.</param>
		/// <param name="weights">Spline control weights. If <see langword="null"/> the weights vector will be automatically initialized with 1.0.</param>
		/// <param name="knots">Spline knot vector.</param>
		/// <param name="degree">Degree of the spline curve. Valid values are 1 (linear), degree 2 (quadratic), degree 3 (cubic), and so on up to degree 10.</param>
		/// <param name="fitPoints">Spine fit points.</param>
		/// <param name="method">Spline creation method.</param>
		/// <param name="closedPeriodic">Sets if the spline as periodic closed (default: <see langword="false"/>).</param>
		internal Spline(IEnumerable<Vector3> controlPoints, IEnumerable<double> weights, IEnumerable<double> knots, short degree, IEnumerable<Vector3> fitPoints, SplineCreationMethod method, bool closedPeriodic)
			: base(EntityType.Spline, DxfObjectCode.Spline)
		{
			// spline degree
			if (degree < 1 || degree > MaxDegree)
			{
				throw new ArgumentOutOfRangeException(nameof(degree), degree, "The spline degree valid values range from 1 to 10.");
			}

			this.Degree = degree;

			// control points
			if (controlPoints == null)
			{
				if (method == SplineCreationMethod.ControlPoints)
				{
					throw new ArgumentNullException(nameof(controlPoints), "Cannot create a spline without control points if its creation method is with control points.");
				}

				this.ControlPoints = new Vector3[0];
			}
			else
			{
				this.ControlPoints = controlPoints.ToArray();
				int numControlPoints = this.ControlPoints.Length;
				if (numControlPoints < 2)
				{
					throw new ArgumentOutOfRangeException(nameof(controlPoints), numControlPoints, "The number of control points must be equal or greater than 2.");
				}

				if (numControlPoints < degree + 1)
				{
					throw new ArgumentOutOfRangeException(nameof(controlPoints), numControlPoints, "The number of control points must be equal or greater than the spline degree + 1.");
				}

				// create weights
				if (weights == null)
				{
					this.Weights = new double[numControlPoints];
					for (int i = 0; i < numControlPoints; i++)
					{
						this.Weights[i] = 1.0;
					}
				}
				else
				{
					this.Weights = weights.ToArray();
					int numWeights = this.Weights.Length;
					if (numWeights != numControlPoints)
					{
						throw new ArgumentException("The number of control points must be the same as the number of weights.", nameof(weights));
					}
				}

				// knots
				if (knots == null)
				{
					throw new ArgumentNullException(nameof(knots));
				}

				this.Knots = knots.ToArray();
				int numKnots;
				if (closedPeriodic)
				{
					numKnots = numControlPoints + 2 * degree + 1;
				}
				else
				{
					numKnots = numControlPoints + degree + 1;
				}
				if (this.Knots.Length != numKnots)
				{
					throw new ArgumentException("Invalid number of knots.");
				}
			}

			// fit points
			if (fitPoints == null)
			{
				if (method == SplineCreationMethod.FitPoints)
				{
					throw new ArgumentNullException(nameof(fitPoints), "Cannot create a spline without fit points if its creation method is with fit points.");
				}
				_FitPoints = new Vector3[0];
			}
			else
			{
				_FitPoints = fitPoints.ToArray();
			}

			this.CreationMethod = method;
			_IsClosedPeriodic = closedPeriodic;
		}

		#endregion

		#region public properties

		private readonly Vector3[] _FitPoints;
		/// <summary>Gets the spline <see cref="Vector3">fit points</see> list.</summary>
		public IReadOnlyList<Vector3> FitPoints => _FitPoints;

		/// <summary>Gets or sets the spline curve start tangent.</summary>
		/// <remarks>Only applicable to splines created with fit points.</remarks>
		public Vector3? StartTangent { get; set; }

		/// <summary>Gets or sets the spline curve end tangent.</summary>
		/// <remarks>Only applicable to splines created with fit points.</remarks>
		public Vector3? EndTangent { get; set; }

		/// <summary>Gets or set the knot parameterization computational method.</summary>
		/// <remarks>
		/// Not usable. When initializing a Spline through a set of fit points, the resulting spline is approximated creating a list of cubic bezier curves.
		/// It is only informative for splines that has been loaded from a <b>DXF</b> file.
		/// </remarks>
		public SplineKnotParameterization KnotParameterization { get; set; } = SplineKnotParameterization.FitChord;

		/// <summary>Gets the spline creation method.</summary>
		public SplineCreationMethod CreationMethod { get; }

		private double _KnotTolerance = 0.0000001;
		/// <summary>Gets or sets the knot tolerance.</summary>
		public double KnotTolerance
		{
			get => _KnotTolerance;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The knot tolerance must be greater than zero.");
				}

				_KnotTolerance = value;
			}
		}

		private double _CtrlPointTolerance = 0.0000001;
		/// <summary>Gets or sets the control point tolerance.</summary>
		public double CtrlPointTolerance
		{
			get => _CtrlPointTolerance;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The control point tolerance must be greater than zero.");
				}

				_CtrlPointTolerance = value;
			}
		}

		private double _FitTolerance = 0.0000000001;
		/// <summary>Gets or sets the fit point tolerance.</summary>
		public double FitTolerance
		{
			get => _FitTolerance;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The fit tolerance must be greater than zero.");
				}

				_FitTolerance = value;
			}
		}

		/// <summary>Gets or sets the polynomial degree of the resulting spline.</summary>
		/// <remarks>
		/// Valid values are 1 (linear), degree 2 (quadratic), degree 3 (cubic), and so on up to degree 10.
		/// </remarks>
		public short Degree { get; }

		/// <summary>Gets if the spline is closed.</summary>
		/// <remarks>
		/// An Spline is closed when the start and end control points are the same.
		/// </remarks>
		public bool IsClosed
		{
			get
			{
				return this.ControlPoints[0].Equals(this.ControlPoints[this.ControlPoints.Length - 1]);
			}
		}

		private bool _IsClosedPeriodic;
		/// <summary>Gets or sets if the spline is closed and periodic.</summary>
		/// <remarks>
		/// A periodic spline is always closed creating a smooth continuity at the end points. <br />
		/// Changing the property will rebuild the knot vector.
		/// </remarks>
		public bool IsClosedPeriodic
		{
			get => _IsClosedPeriodic;
			set
			{
				this.Knots = CreateKnotVector(this.ControlPoints.Length, this.Degree, value);
				_IsClosedPeriodic = value;
			}
		}

		/// <summary>Gets the spline <see cref="Vector3">control points</see> list.</summary>
		public Vector3[] ControlPoints { get; }

		/// <summary>Gets the spline control points weights list.</summary>
		public double[] Weights { get; }

		/// <summary>Gets the spline knot vector.</summary>
		/// <remarks>By default a uniform knot vector is created.</remarks>
		public double[] Knots { get; private set; }

		#endregion

		#region public methods

		/// <summary>Switch the spline direction.</summary>
		public void Reverse()
		{
			Array.Reverse(_FitPoints);
			Array.Reverse(this.ControlPoints);
			Array.Reverse(this.Weights);
			Vector3? tmp = this.StartTangent;
			this.StartTangent = -this.EndTangent;
			this.EndTangent = -tmp;
		}

		/// <summary>Sets all control point weights to the specified number.</summary>
		/// <param name="weight">Control point weight.</param>
		public void SetUniformWeights(double weight)
		{
			for (int i = 0; i < this.Weights.Length; i++)
			{
				this.Weights[i] = weight;
			}
		}

		/// <summary>Converts the spline in a list of vertexes.</summary>
		/// <param name="precision">Number of vertexes generated.</param>
		/// <returns>A list vertexes that represents the spline.</returns>
		public List<Vector3> PolygonalVertexes(int precision)
			=> NurbsEvaluator(this.ControlPoints.ToArray(), this.Weights.ToArray(), this.Knots, this.Degree, this.IsClosed, _IsClosedPeriodic, precision);

		/// <summary>Converts the spline in a Polyline3D.</summary>
		/// <param name="precision">Number of vertexes generated.</param>
		/// <returns>A new instance of <see cref="Polyline3D"/> that represents the spline.</returns>
		public Polyline3D ToPolyline3D(int precision)
		{
			IEnumerable<Vector3> vertexes = this.PolygonalVertexes(precision);
			bool closed = this.IsClosed || this.IsClosedPeriodic;
			Polyline3D poly = new Polyline3D(vertexes)
			{
				Layer = (Layer)this.Layer.Clone(),
				Linetype = (Linetype)this.Linetype.Clone(),
				Color = (AciColor)this.Color.Clone(),
				Lineweight = this.Lineweight,
				Transparency = (Transparency)this.Transparency.Clone(),
				LinetypeScale = this.LinetypeScale,
				Normal = this.Normal,
				IsClosed = closed
			};

			return poly;
		}

		/// <summary>Converts the spline in a Polyline2D.</summary>
		/// <param name="precision">Number of vertexes generated.</param>
		/// <returns>A new instance of <see cref="Polyline2D"/> that represents the spline.</returns>
		/// <remarks>
		/// The resulting polyline will be a projection of the actual spline into the plane defined by its normal vector.
		/// </remarks>
		public Polyline2D ToPolyline2D(int precision)
		{
			List<Vector3> vertexes3D = this.PolygonalVertexes(precision);
			List<Vector2> vertexes2D = MathHelper.Transform(vertexes3D, this.Normal, out double _);
			bool closed = this.IsClosed || this.IsClosedPeriodic;
			Polyline2D polyline2D = new Polyline2D(vertexes2D)
			{
				Layer = (Layer)this.Layer.Clone(),
				Linetype = (Linetype)this.Linetype.Clone(),
				Color = (AciColor)this.Color.Clone(),
				Lineweight = this.Lineweight,
				Transparency = (Transparency)this.Transparency.Clone(),
				LinetypeScale = this.LinetypeScale,
				Normal = this.Normal,
				IsClosed = closed
			};

			return polyline2D;
		}

		/// <summary>Calculate points along a <b>NURBS</b> curve.</summary>
		/// <param name="controls">List of spline control points.</param>
		/// <param name="weights">Spline control weights. If <see langword="null"/> the weights vector will be automatically initialized with 1.0.</param>
		/// <param name="knots">List of spline knot points. If <see langword="null"/> the knot vector will be automatically generated.</param>
		/// <param name="degree">Spline degree.</param>
		/// <param name="isClosed">Specifies if the spline is closed.</param>
		/// <param name="isClosedPeriodic">Specifies if the spline is closed and periodic.</param>
		/// <param name="precision">Number of vertexes generated.</param>
		/// <returns>A list vertexes that represents the spline.</returns>
		/// <remarks>
		/// NURBS evaluator provided by mikau16 based on Michael V. implementation, roughly follows the notation of http://cs.mtu.edu/~shene/PUBLICATIONS/2004/NURBS.pdf
		/// Added a few modifications to make it work for open, closed, and periodic closed splines.
		/// </remarks>
		public static List<Vector3> NurbsEvaluator(Vector3[] controls, double[] weights, double[] knots, int degree, bool isClosed, bool isClosedPeriodic, int precision)
		{
			if (precision < 2)
			{
				throw new ArgumentOutOfRangeException(nameof(precision), precision, "The precision must be equal or greater than two.");
			}

			// control points
			if (controls == null)
			{
				throw new ArgumentNullException(nameof(controls), "A spline entity with control points is required.");
			}

			int numCtrlPoints = controls.Length;

			if (numCtrlPoints == 0)
			{
				throw new ArgumentException("A spline entity with control points is required.", nameof(controls));
			}

			// weights
			if (weights == null)
			{
				// give the default 1.0 to the control points weights
				weights = new double[numCtrlPoints];
				for (int i = 0; i < numCtrlPoints; i++)
				{
					weights[i] = 1.0;
				}
			}
			else if (weights.Length != numCtrlPoints)
			{
				throw new ArgumentException("The number of control points must be the same as the number of weights.", nameof(weights));
			}

			// knots
			if (knots == null)
			{
				knots = CreateKnotVector(numCtrlPoints, degree, isClosedPeriodic);
			}
			else
			{
				int numKnots;
				if (isClosedPeriodic)
				{
					numKnots = numCtrlPoints + 2 * degree + 1;
				}
				else
				{
					numKnots = numCtrlPoints + degree + 1;
				}
				if (knots.Length != numKnots)
				{
					throw new ArgumentException("Invalid number of knots.");
				}
			}

			Vector3[] ctrl;
			double[] w;
			if (isClosedPeriodic)
			{
				ctrl = new Vector3[numCtrlPoints + degree];
				w = new double[numCtrlPoints + degree];
				for (int i = 0; i < degree; i++)
				{
					int index = numCtrlPoints - degree + i;
					ctrl[i] = controls[index];
					w[i] = weights[index];
				}

				controls.CopyTo(ctrl, degree);
				weights.CopyTo(w, degree);
			}
			else
			{
				ctrl = controls;
				w = weights;
			}

			double uStart;
			double uEnd;
			List<Vector3> vertexes = new List<Vector3>();

			if (isClosed)
			{
				uStart = knots[0];
				uEnd = knots[knots.Length - 1];
			}
			else if (isClosedPeriodic)
			{
				uStart = knots[degree];
				uEnd = knots[knots.Length - degree - 1];
			}
			else
			{
				precision -= 1;
				uStart = knots[0];
				uEnd = knots[knots.Length - 1];
			}

			double uDelta = (uEnd - uStart) / precision;

			for (int i = 0; i < precision; i++)
			{
				double u = uStart + uDelta * i;
				vertexes.Add(C(ctrl, w, knots, degree, u));
			}

			if (!(isClosed || isClosedPeriodic))
			{
				vertexes.Add(ctrl[ctrl.Length - 1]);
			}

			return vertexes;
		}

		#endregion

		#region private methods

		private static double[] CreateKnotVector(int numControlPoints, int degree, bool isPeriodic)
		{
			// create knot vector
			int numKnots;
			double[] knots;

			if (!isPeriodic)
			{
				numKnots = numControlPoints + degree + 1;
				knots = new double[numKnots];

				int i;
				for (i = 0; i <= degree; i++)
				{
					knots[i] = 0.0;
				}

				for (; i < numControlPoints; i++)
				{
					knots[i] = i - degree;
				}

				for (; i < numKnots; i++)
				{
					knots[i] = numControlPoints - degree;
				}
			}
			else
			{
				numKnots = numControlPoints + 2 * degree + 1;
				knots = new double[numKnots];

				double factor = 1.0 / (numControlPoints - degree);
				for (int i = 0; i < numKnots; i++)
				{
					knots[i] = (i - degree) * factor;
				}
			}

			return knots;
		}

		private static double[] ÇreateBezierKnotVector(int numControlPoints, int degree)
		{
			// create knot vector
			int numKnots = numControlPoints + degree + 1;
			double[] knots = new double[numKnots];

			int np = degree + 1;
			int nc = numKnots / np;
			double fact = 1.0 / nc;
			int index = 1;

			for (int i = 0; i < numKnots;)
			{
				double knot;

				if (i < np)
				{
					knot = 0.0;
				}
				else if (i >= numKnots - np)
				{
					knot = 1.0;
				}
				else
				{
					knot = fact * index;
					index += 1;
				}

				for (int j = 0; j < np; j++)
				{
					knots[i] = knot;
					i += 1;
				}
			}

			return knots;
		}

		private static Vector3 C(Vector3[] ctrlPoints, double[] weights, double[] knots, int degree, double u)
		{
			Vector3 vectorSum = Vector3.Zero;
			double denominatorSum = 0.0;

			// optimization suggested by ThVoss
			for (int i = 0; i < ctrlPoints.Length; i++)
			{
				double n = N(knots, i, degree, u);
				denominatorSum += n * weights[i];
				vectorSum += weights[i] * n * ctrlPoints[i];
			}

			// avoid possible divided by zero error, this should never happen
			if (Math.Abs(denominatorSum) < double.Epsilon)
			{
				return Vector3.Zero;
			}

			return (1.0 / denominatorSum) * vectorSum;
		}

		private static double N(double[] knots, int i, int p, double u)
		{
			if (p <= 0)
			{
				if (knots[i] <= u && u < knots[i + 1])
				{
					return 1;
				}

				return 0.0;
			}

			double leftCoefficient = 0.0;
			if (!(Math.Abs(knots[i + p] - knots[i]) < double.Epsilon))
			{
				leftCoefficient = (u - knots[i]) / (knots[i + p] - knots[i]);
			}

			double rightCoefficient = 0.0; // article contains error here, denominator is Knots[i + p + 1] - Knots[i + 1]
			if (!(Math.Abs(knots[i + p + 1] - knots[i + 1]) < double.Epsilon))
			{
				rightCoefficient = (knots[i + p + 1] - u) / (knots[i + p + 1] - knots[i + 1]);
			}

			return leftCoefficient * N(knots, i, p - 1, u) + rightCoefficient * N(knots, i + 1, p - 1, u);
		}

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override void TransformBy(Matrix3 transformation, Vector3 translation)
		{
			for (int i = 0; i < this.ControlPoints.Length; i++)
			{
				this.ControlPoints[i] = transformation * this.ControlPoints[i] + translation;
			}

			for (int i = 0; i < _FitPoints.Length; i++)
			{
				_FitPoints[i] = transformation * _FitPoints[i] + translation;
			}

			Vector3 newNormal = transformation * this.Normal;
			if (Vector3.Equals(Vector3.Zero, newNormal))
			{
				newNormal = this.Normal;
			}
			this.Normal = newNormal;
		}

		/// <inheritdoc/>
		public override object Clone()
		{
			Spline entity;
			if (this.CreationMethod == SplineCreationMethod.FitPoints)
			{
				entity = new Spline(new List<Vector3>(_FitPoints))
				{
					//EntityObject properties
					Layer = (Layer)this.Layer.Clone(),
					Linetype = (Linetype)this.Linetype.Clone(),
					Color = (AciColor)this.Color.Clone(),
					Lineweight = this.Lineweight,
					Transparency = (Transparency)this.Transparency.Clone(),
					LinetypeScale = this.LinetypeScale,
					Normal = this.Normal,
					IsVisible = this.IsVisible,
					//Spline properties
					KnotParameterization = this.KnotParameterization,
					StartTangent = this.StartTangent,
					EndTangent = this.EndTangent
				};
			}
			else
			{
				entity = new Spline(this.ControlPoints, this.Weights, this.Knots, this.Degree, _FitPoints, this.CreationMethod, _IsClosedPeriodic)
				{
					//EntityObject properties
					Layer = (Layer)this.Layer.Clone(),
					Linetype = (Linetype)this.Linetype.Clone(),
					Color = (AciColor)this.Color.Clone(),
					Lineweight = this.Lineweight,
					Transparency = (Transparency)this.Transparency.Clone(),
					LinetypeScale = this.LinetypeScale,
					Normal = this.Normal,
					//Spline properties
					KnotParameterization = this.KnotParameterization,
					StartTangent = this.StartTangent,
					EndTangent = this.EndTangent
				};
			}

			foreach (XData data in this.XData.Values)
			{
				entity.XData.Add((XData)data.Clone());
			}

			return entity;
		}

		#endregion
	}
}