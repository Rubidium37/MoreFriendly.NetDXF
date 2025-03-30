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
using System.Runtime.CompilerServices;
using netDxf.Blocks;
using netDxf.Collections;
using netDxf.Tables;

namespace netDxf.Entities
{
	/// <summary>Represents a leader <see cref="EntityObject">entity</see>.</summary>
	public class Leader :
		EntityObject
	{
		#region delegates and events

		/// <summary>Generated when a property of <see cref="DimensionStyle"/> type changes.</summary>
		public event BeforeValueChangeEventHandler<DimensionStyle> BeforeChangingDimensionStyleValue;
		/// <summary>Generates the <see cref="BeforeChangingDimensionStyleValue"/> event.</summary>
		/// <param name="oldValue">The old value, being changed.</param>
		/// <param name="newValue">The new value, that will replace the old one.</param>
		/// <param name="propertyName">(automatic) Name of the affected property.</param>
		protected virtual DimensionStyle OnBeforeChangingDimensionStyleValue(DimensionStyle oldValue, DimensionStyle newValue, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeChangingDimensionStyleValue is { } handler)
			{
				var e = new BeforeValueChangeEventArgs<DimensionStyle>(propertyName, oldValue, newValue);
				handler(this, e);
				return e.NewValue;
			}
			return newValue;
		}

		/// <summary>Generated when an <see cref="EntityObject"/> item has been added.</summary>
		public event AfterItemChangeEventHandler<EntityObject> AfterAddingEntityObject;
		/// <summary>Generates the <see cref="AfterAddingEntityObject"/> event.</summary>
		/// <param name="item">The item being added.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		protected virtual void OnAfterAddingEntityObject(EntityObject item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterAddingEntityObject is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Add, item));
		}

		/// <summary>Generated when an <see cref="EntityObject"/> item has been removed.</summary>
		public event AfterItemChangeEventHandler<EntityObject> AfterRemovingEntityObject;
		/// <summary>Generates the <see cref="AfterRemovingEntityObject"/> event.</summary>
		/// <param name="item">The item being removed.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		protected virtual void OnAfterRemovingEntityObject(EntityObject item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterRemovingEntityObject is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Remove, item));
		}

		#endregion

		#region delegates and events for style overrides

		/// <summary>Generated when an <see cref="DimensionStyleOverride"/> item has been added.</summary>
		public event AfterItemChangeEventHandler<DimensionStyleOverride> AfterAddingDimensionStyleOverride;
		/// <summary>Generates the <see cref="AfterAddingDimensionStyleOverride"/> event.</summary>
		/// <param name="item">The item being added.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		protected virtual void OnAfterAddingDimensionStyleOverride(DimensionStyleOverride item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterAddingDimensionStyleOverride is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Add, item));
		}

		/// <summary>Generated when an <see cref="DimensionStyleOverride"/> item has been removed.</summary>
		public event AfterItemChangeEventHandler<DimensionStyleOverride> AfterRemovingDimensionStyleOverride;
		/// <summary>Generates the <see cref="AfterRemovingDimensionStyleOverride"/> event.</summary>
		/// <param name="item">The item being removed.</param>
		/// <param name="propertyName">(automatic) Name of the affected collection property.</param>
		protected virtual void OnAfterRemovingDimensionStyleOverride(DimensionStyleOverride item, [CallerMemberName] string propertyName = "")
		{
			if (this.AfterRemovingDimensionStyleOverride is { } handler)
				handler(this, new(propertyName, ItemChangeAction.Remove, item));
		}

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="vertexes">List of leader vertexes in local coordinates.</param>
		public Leader(IEnumerable<Vector2> vertexes)
			: this(vertexes, DimensionStyle.Default)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="vertexes">List of leader vertexes in local coordinates.</param>
		/// <param name="style">Leader style.</param>
		public Leader(IEnumerable<Vector2> vertexes, DimensionStyle style)
			: this(vertexes, style, false)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="text">Leader text annotation.</param>
		/// <param name="vertexes">List of leader vertexes in local coordinates.</param>
		public Leader(string text, IEnumerable<Vector2> vertexes)
			: this(text, vertexes, DimensionStyle.Default)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="text">Leader text annotation.</param>
		/// <param name="vertexes">List of leader vertexes in local coordinates.</param>
		/// <param name="style">Leader style.</param>
		public Leader(string text, IEnumerable<Vector2> vertexes, DimensionStyle style)
			: this(vertexes, style)
		{
			this.Annotation = this.BuildAnnotation(text);
			this.CalculateAnnotationDirection();
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="tolerance">Leader tolerance annotation.</param>
		/// <param name="vertexes">List of leader vertexes in local coordinates.</param>
		public Leader(ToleranceEntry tolerance, IEnumerable<Vector2> vertexes)
			: this(tolerance, vertexes, DimensionStyle.Default)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="tolerance">Leader tolerance annotation.</param>
		/// <param name="vertexes">List of leader vertexes in local coordinates.</param>
		/// <param name="style">Leader style.</param>
		public Leader(ToleranceEntry tolerance, IEnumerable<Vector2> vertexes, DimensionStyle style)
			: this(vertexes, style)
		{
			this.Annotation = this.BuildAnnotation(tolerance);
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="block">Leader block annotation.</param>
		/// <param name="vertexes">List of leader vertexes in local coordinates.</param>
		public Leader(Block block, IEnumerable<Vector2> vertexes)
			: this(block, vertexes, DimensionStyle.Default)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="block">Leader block annotation.</param>
		/// <param name="vertexes">List of leader vertexes in local coordinates.</param>
		/// <param name="style">Leader style.</param>
		public Leader(Block block, IEnumerable<Vector2> vertexes, DimensionStyle style)
			: this(vertexes, style)
		{
			this.Annotation = this.BuildAnnotation(block);
		}
		internal Leader(IEnumerable<Vector2> vertexes, DimensionStyle style, bool hasHookline)
			: base(EntityType.Leader, DxfObjectCode.Leader)
		{
			if (vertexes == null)
			{
				throw new ArgumentNullException(nameof(vertexes));
			}

			this.Vertexes = new List<Vector2>(vertexes);
			if (this.Vertexes.Count < 2)
			{
				throw new ArgumentOutOfRangeException(nameof(vertexes), this.Vertexes.Count, "The leader vertexes list requires at least two points.");
			}

			_Style = style ?? throw new ArgumentNullException(nameof(style));
			_HasHookline = hasHookline;
			this.StyleOverrides.BeforeAddingItem += this.StyleOverrides_BeforeAddingItem;
			this.StyleOverrides.AfterAddingItem += this.StyleOverrides_AfterAddingItem;
			this.StyleOverrides.BeforeRemovingItem += this.StyleOverrides_BeforeRemovingItem;
			this.StyleOverrides.AfterRemovingItem += this.StyleOverrides_AfterRemovingItem;
		}

		#endregion

		#region public properties

		private DimensionStyle _Style;
		/// <summary>Gets or sets the leader style.</summary>
		public DimensionStyle Style
		{
			get => _Style;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_Style = this.OnBeforeChangingDimensionStyleValue(_Style, value);
			}
		}

		/// <summary>Gets the dimension style overrides list.</summary>
		/// <remarks>
		/// Any dimension style value stored in this list will override its corresponding value in the assigned style.
		/// </remarks>
		public DimensionStyleOverrideDictionary StyleOverrides { get; } = new DimensionStyleOverrideDictionary();

		/// <summary>Gets or sets if the arrowhead is drawn.</summary>
		public bool ShowArrowhead { get; set; } = true;

		/// <summary>Gets or sets the way the leader is drawn.</summary>
		public LeaderPathType PathType { get; set; } = LeaderPathType.StraightLineSegments;

		/// <summary>Gets the leader vertexes list in local coordinates.</summary>
		/// <remarks>
		/// The leader vertexes list must have at least two points.
		/// </remarks>
		public List<Vector2> Vertexes { get; }

		private EntityObject _Annotation;
		/// <summary>Gets or sets the leader annotation entity.</summary>
		/// <remarks>
		/// Only MText, Text, Tolerance, and Insert entities are supported as a leader annotation.
		/// Even if <b>AutoCAD</b> allows a Text entity to be part of a Leader it is not recommended, always use a MText entity instead.
		/// <br />
		/// Set the annotation property to <see langword="null"/> to create a Leader without annotation.
		/// </remarks>
		public EntityObject Annotation
		{
			get => _Annotation;
			set
			{
				if (value != null)
				{
					if (!(value.Type == EntityType.MText ||
							value.Type == EntityType.Text ||
							value.Type == EntityType.Insert ||
							value.Type == EntityType.Tolerance))
					{
						throw new ArgumentException("Only MText, Text, Insert, and Tolerance entities are supported as a leader annotation.", nameof(value));
					}
				}

				// nothing else to do if it is the same
				if (ReferenceEquals(_Annotation, value))
				{
					return;
				}

				// remove the previous annotation
				if (_Annotation != null)
				{
					_Annotation.RemoveReactor(this);
					this.OnAfterRemovingEntityObject(_Annotation);
				}

				// add the new annotation
				if (value != null)
				{
					value.AddReactor(this);
					this.OnAfterAddingEntityObject(value);
				}

				_Annotation = value;
			}
		}

		/// <summary>Gets or sets the leader hook position (last leader vertex).</summary>
		/// <remarks>
		/// This property allows easy access to the last leader vertex, aka leader hook position.
		/// </remarks>
		public Vector2 Hook
		{
			get => this.Vertexes[this.Vertexes.Count - 1];
			set => this.Vertexes[this.Vertexes.Count - 1] = value;
		}

		private bool _HasHookline;
		/// <summary>Gets if the leader has a hook line.</summary>
		/// <remarks>
		/// If set to <see langword="true"/> an additional vertex point (StartHookLine) will be created before the leader end point (hook).
		/// By default, only leaders with text annotation have hook lines.
		/// </remarks>
		public bool HasHookline
		{
			get => _HasHookline;
			set
			{
				if (this.Vertexes.Count < 2)
				{
					throw new Exception("The leader vertexes list requires at least two points.");
				}

				if (_HasHookline != value)
				{
					if (value)
					{
						this.Vertexes.Insert(this.Vertexes.Count - 1, this.CalculateHookLine());
					}
					else
					{
						this.Vertexes.RemoveAt(this.Vertexes.Count - 2);
					}
				}
				_HasHookline = value;
			}
		}

		private AciColor _LineColor = AciColor.ByLayer;
		/// <summary>Gets or sets the leader line color if the style parameter <b>DIMCLRD</b> is set as BYBLOCK.</summary>
		public AciColor LineColor
		{
			get => _LineColor;
			set => _LineColor = value ?? throw new ArgumentNullException(nameof(value));
		}

		/// <summary>Gets or sets the entity <see cref="Vector3">normal</see>.</summary>
		public new Vector3 Normal
		{
			get => base.Normal;
			set => base.Normal = value;
		}

		/// <summary>Gets or sets the leader elevation.</summary>
		/// <remarks>This is the distance from the origin to the plane of the leader.</remarks>
		public double Elevation { get; set; } = 0.0;

		/// <summary>Gets or sets the offset from the last leader vertex (hook) to the annotation position.</summary>
		public Vector2 Offset { get; set; } = Vector2.Zero;

		private Vector2 _Direction = Vector2.UnitX;
		/// <summary>Gets or sets the leader annotation direction.</summary>
		public Vector2 Direction
		{
			get => _Direction;
			set => _Direction = Vector2.Normalize(value);
		}

		#endregion

		#region public methods

		/// <summary>Updates the leader entity to reflect the latest changes made to its properties.</summary>
		/// <param name="resetAnnotationPosition">
		/// If <see langword="true"/> the annotation position will be modified according to the position of the leader hook (last leader vertex),
		/// otherwise the leader hook will be moved according to the actual annotation position.
		/// </param>
		/// <remarks>
		/// This method should be manually called if the annotation position is modified, or the leader properties like Style, Annotation, TextVerticalPosition, and/or Offset.
		/// </remarks>
		public void Update(bool resetAnnotationPosition)
		{
			if (this.Vertexes.Count < 2)
			{
				throw new Exception("The leader vertexes list requires at least two points.");
			}

			if (_Annotation == null)
			{
				return;
			}

			this.CalculateAnnotationDirection();

			if (resetAnnotationPosition)
			{
				this.ResetAnnotationPosition();
			}
			else
			{
				this.ResetHookPosition();
			}

			if (_HasHookline)
			{
				Vector2 vertex = this.CalculateHookLine();
				this.Vertexes[this.Vertexes.Count - 2] = vertex;
			}
		}

		#endregion

		#region private methods

		private void CalculateAnnotationDirection()
		{
			double angle = 0.0;

			if (_Annotation != null)
			{
				switch (_Annotation.Type)
				{
					case EntityType.MText:
						MText mText = (MText)_Annotation;
						angle = mText.Rotation;
						switch (mText.AttachmentPoint)
						{
							case MTextAttachmentPoint.TopRight:
							case MTextAttachmentPoint.MiddleRight:
							case MTextAttachmentPoint.BottomRight:
								angle += 180.0;
								break;
						}
						break;
					case EntityType.Text:
						Text text = (Text)_Annotation;
						angle = text.Rotation;
						switch (text.Alignment)
						{
							case TextAlignment.TopRight:
							case TextAlignment.MiddleRight:
							case TextAlignment.BottomRight:
							case TextAlignment.BaselineRight:
								angle += 180.0;
								break;
						}
						break;
					case EntityType.Insert:
						angle = ((Insert)_Annotation).Rotation;
						break;
					case EntityType.Tolerance:
						angle = ((Tolerance)_Annotation).Rotation;
						break;
					default:
						throw new ArgumentException("Only MText, Text, Insert, and Tolerance entities are supported as a leader annotation.", nameof(_Annotation));
				}
			}
			_Direction = Vector2.Rotate(Vector2.UnitX, angle * MathHelper.DegToRad);
		}

		private Vector2 CalculateHookLine()
		{
			DimensionStyleOverride styleOverride;

			double dimScale = this.Style.DimScaleOverall;
			if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.DimScaleOverall, out styleOverride))
			{
				dimScale = (double)styleOverride.Value;
			}

			double arrowSize = this.Style.ArrowSize;
			if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.ArrowSize, out styleOverride))
			{
				arrowSize = (double)styleOverride.Value;
			}

			return this.Hook - this.Direction * arrowSize * dimScale;
		}

		/// <summary>Resets the leader hook position according to the annotation position.</summary>
		private void ResetHookPosition()
		{
			DimensionStyleOverride styleOverride;

			DimensionStyleTextVerticalPlacement textVerticalPlacement = this.Style.TextVerticalPlacement;
			if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextVerticalPlacement, out styleOverride))
			{
				textVerticalPlacement = (DimensionStyleTextVerticalPlacement)styleOverride.Value;
			}

			double textGap = this.Style.TextOffset;
			if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextOffset, out styleOverride))
			{
				textGap = (double)styleOverride.Value;
			}

			double dimScale = this.Style.DimScaleOverall;
			if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.DimScaleOverall, out styleOverride))
			{
				dimScale = (double)styleOverride.Value;
			}

			double textHeight = this.Style.TextHeight;
			if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextHeight, out styleOverride))
			{
				textHeight = (double)styleOverride.Value;
			}

			AciColor textColor = this.Style.TextColor;
			if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextColor, out styleOverride))
			{
				textColor = (AciColor)styleOverride.Value;
			}

			Vector2 position;
			Vector2 textOffset;
			Vector2 dir = this.Direction;
			int side;
			textGap *= dimScale;

			switch (_Annotation.Type)
			{
				case EntityType.MText:
					MText mText = (MText)_Annotation;
					side = MathHelper.Sign(dir.X);
					if (side == 0) side = MathHelper.Sign(dir.Y);
					if (mText.Rotation > 90.0 && mText.Rotation <= 270.0) side *= -1;

					if (side >= 0)
					{
						switch (mText.AttachmentPoint)
						{
							case MTextAttachmentPoint.TopRight:
								mText.AttachmentPoint = MTextAttachmentPoint.TopLeft;
								break;
							case MTextAttachmentPoint.MiddleRight:
								mText.AttachmentPoint = MTextAttachmentPoint.MiddleLeft;
								break;
							case MTextAttachmentPoint.BottomRight:
								mText.AttachmentPoint = MTextAttachmentPoint.BottomLeft;
								break;
						}
					}
					else
					{
						switch (mText.AttachmentPoint)
						{
							case MTextAttachmentPoint.TopLeft:
								mText.AttachmentPoint = MTextAttachmentPoint.TopRight;
								break;
							case MTextAttachmentPoint.MiddleLeft:
								mText.AttachmentPoint = MTextAttachmentPoint.MiddleRight;
								break;
							case MTextAttachmentPoint.BottomLeft:
								mText.AttachmentPoint = MTextAttachmentPoint.BottomRight;
								break;
						}
					}

					textOffset = textVerticalPlacement == DimensionStyleTextVerticalPlacement.Centered ?
						new Vector2(side * textGap, 0.0) :
						new Vector2(side * textGap, textGap);

					position = MathHelper.Transform(mText.Position, this.Normal, out _);
					this.Hook = position - this.Offset - Vector2.Rotate(textOffset, mText.Rotation * MathHelper.DegToRad);

					mText.Height = textHeight * dimScale;
					mText.Color = textColor.IsByBlock ? AciColor.ByLayer : textColor;
					break;

				case EntityType.Text:
					Text text = (Text)_Annotation;
					side = MathHelper.Sign(dir.X);
					if (side == 0) side = MathHelper.Sign(dir.Y);
					if (text.Rotation > 90.0 && text.Rotation <= 270.0) side *= -1;

					if (side >= 0)
					{
						switch (text.Alignment)
						{
							case TextAlignment.TopRight:
								text.Alignment = TextAlignment.TopLeft;
								break;
							case TextAlignment.MiddleRight:
								text.Alignment = TextAlignment.MiddleLeft;
								break;
							case TextAlignment.BottomRight:
								text.Alignment = TextAlignment.BottomLeft;
								break;
							case TextAlignment.BaselineRight:
								text.Alignment = TextAlignment.BaselineLeft;
								break;
						}
					}
					else
					{
						switch (text.Alignment)
						{
							case TextAlignment.TopLeft:
								text.Alignment = TextAlignment.TopRight;
								break;
							case TextAlignment.MiddleLeft:
								text.Alignment = TextAlignment.MiddleRight;
								break;
							case TextAlignment.BottomLeft:
								text.Alignment = TextAlignment.BottomRight;
								break;
							case TextAlignment.BaselineLeft:
								text.Alignment = TextAlignment.BaselineRight;
								break;
						}
					}

					textOffset = textVerticalPlacement == DimensionStyleTextVerticalPlacement.Centered ?
						new Vector2(side * textGap, 0.0) :
						new Vector2(side * textGap, textGap);

					position = MathHelper.Transform(text.Position, this.Normal, out _);
					this.Hook = position - this.Offset - Vector2.Rotate(textOffset, text.Rotation * MathHelper.DegToRad);

					text.Height = textHeight * dimScale;
					text.Color = textColor.IsByBlock ? AciColor.ByLayer : textColor;
					break;

				case EntityType.Insert:
					Insert ins = (Insert)_Annotation;
					position = MathHelper.Transform(ins.Position, this.Normal, out _);
					this.Hook = position - this.Offset;
					ins.Color = textColor.IsByBlock ? AciColor.ByLayer : textColor;
					break;

				case EntityType.Tolerance:
					Tolerance tol = (Tolerance)_Annotation;
					position = MathHelper.Transform(tol.Position, this.Normal, out _);
					this.Hook = position - this.Offset;
					tol.Color = textColor.IsByBlock ? AciColor.ByLayer : textColor;
					break;

				default:
					throw new Exception(string.Format("The entity type: {0} not supported as a leader annotation.", _Annotation.Type));
			}
		}

		/// <summary>Resets the annotation position according to the leader hook.</summary>
		private void ResetAnnotationPosition()
		{
			DimensionStyleOverride styleOverride;

			DimensionStyleTextVerticalPlacement textVerticalPlacement = this.Style.TextVerticalPlacement;
			if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextVerticalPlacement, out styleOverride))
			{
				textVerticalPlacement = (DimensionStyleTextVerticalPlacement)styleOverride.Value;
			}

			double textGap = this.Style.TextOffset;
			if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextOffset, out styleOverride))
			{
				textGap = (double)styleOverride.Value;
			}

			double dimScale = this.Style.DimScaleOverall;
			if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.DimScaleOverall, out styleOverride))
			{
				dimScale = (double)styleOverride.Value;
			}

			double textHeight = this.Style.TextHeight;
			if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextHeight, out styleOverride))
			{
				textHeight = (double)styleOverride.Value;
			}

			AciColor textColor = this.Style.TextColor;
			if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextColor, out styleOverride))
			{
				textColor = (AciColor)styleOverride.Value;
			}

			Vector2 hook = this.Hook;
			Vector2 position;
			Vector2 textOffset;
			Vector2 dir = this.Direction;
			int side;
			textGap *= dimScale;

			switch (_Annotation.Type)
			{
				case EntityType.MText:
					MText mText = (MText)_Annotation;
					side = MathHelper.Sign(dir.X);
					if (side == 0) side = MathHelper.Sign(dir.Y);
					if (mText.Rotation > 90.0 && mText.Rotation <= 270.0) side *= -1;

					if (side >= 0)
					{
						switch (mText.AttachmentPoint)
						{
							case MTextAttachmentPoint.TopRight:
								mText.AttachmentPoint = MTextAttachmentPoint.TopLeft;
								break;
							case MTextAttachmentPoint.MiddleRight:
								mText.AttachmentPoint = MTextAttachmentPoint.MiddleLeft;
								break;
							case MTextAttachmentPoint.BottomRight:
								mText.AttachmentPoint = MTextAttachmentPoint.BottomLeft;
								break;
						}
					}
					else
					{
						switch (mText.AttachmentPoint)
						{
							case MTextAttachmentPoint.TopLeft:
								mText.AttachmentPoint = MTextAttachmentPoint.TopRight;
								break;
							case MTextAttachmentPoint.MiddleLeft:
								mText.AttachmentPoint = MTextAttachmentPoint.MiddleRight;
								break;
							case MTextAttachmentPoint.BottomLeft:
								mText.AttachmentPoint = MTextAttachmentPoint.BottomRight;
								break;
						}
					}

					textOffset = textVerticalPlacement == DimensionStyleTextVerticalPlacement.Centered ?
						new Vector2(side * textGap, 0.0) :
						new Vector2(side * textGap, textGap);

					position = hook + this.Offset + Vector2.Rotate(textOffset, mText.Rotation * MathHelper.DegToRad);

					mText.Position = MathHelper.Transform(position, this.Normal, this.Elevation);
					mText.Height = textHeight * dimScale;
					mText.Color = textColor.IsByBlock ? AciColor.ByLayer : textColor;
					break;

				case EntityType.Text:
					Text text = (Text)_Annotation;
					side = MathHelper.Sign(dir.X);
					if (side == 0) side = MathHelper.Sign(dir.Y);
					if (text.Rotation > 90.0 && text.Rotation <= 270.0) side *= -1;

					if (side >= 0)
					{
						switch (text.Alignment)
						{
							case TextAlignment.TopRight:
								text.Alignment = TextAlignment.TopLeft;
								break;
							case TextAlignment.MiddleRight:
								text.Alignment = TextAlignment.MiddleLeft;
								break;
							case TextAlignment.BottomRight:
								text.Alignment = TextAlignment.BottomLeft;
								break;
							case TextAlignment.BaselineRight:
								text.Alignment = TextAlignment.BaselineLeft;
								break;
						}
					}
					else
					{
						switch (text.Alignment)
						{
							case TextAlignment.TopLeft:
								text.Alignment = TextAlignment.TopRight;
								break;
							case TextAlignment.MiddleLeft:
								text.Alignment = TextAlignment.MiddleRight;
								break;
							case TextAlignment.BottomLeft:
								text.Alignment = TextAlignment.BottomRight;
								break;
							case TextAlignment.BaselineLeft:
								text.Alignment = TextAlignment.BaselineRight;
								break;
						}
					}

					textOffset = textVerticalPlacement == DimensionStyleTextVerticalPlacement.Centered ?
						new Vector2(side * textGap, 0.0) :
						new Vector2(side * textGap, textGap);

					position = hook + this.Offset + Vector2.Rotate(textOffset, text.Rotation * MathHelper.DegToRad);
					text.Position = MathHelper.Transform(position, this.Normal, this.Elevation);
					text.Height = textHeight * dimScale;
					text.Color = textColor.IsByBlock ? AciColor.ByLayer : textColor;
					break;

				case EntityType.Insert:
					Insert ins = (Insert)_Annotation;
					position = hook + this.Offset;
					ins.Position = MathHelper.Transform(position, this.Normal, this.Elevation);
					ins.Color = textColor.IsByBlock ? AciColor.ByLayer : textColor;
					break;

				case EntityType.Tolerance:
					Tolerance tol = (Tolerance)_Annotation;
					position = hook + this.Offset;
					tol.Position = MathHelper.Transform(position, this.Normal, this.Elevation);
					tol.Color = textColor.IsByBlock ? AciColor.ByLayer : textColor;
					break;

				default:
					throw new Exception(string.Format("The entity type: {0} not supported as a leader annotation.", _Annotation.Type));
			}
		}

		private MText BuildAnnotation(string text)
		{
			int side = Math.Sign(this.Vertexes[this.Vertexes.Count - 1].X - this.Vertexes[this.Vertexes.Count - 2].X);
			MTextAttachmentPoint attachment;
			Vector2 textOffset;
			if (_Style.TextVerticalPlacement == DimensionStyleTextVerticalPlacement.Centered)
			{
				textOffset = new Vector2(side * _Style.TextOffset * _Style.DimScaleOverall, 0.0);
				attachment = side >= 0 ? MTextAttachmentPoint.MiddleLeft : MTextAttachmentPoint.MiddleRight;
			}
			else
			{
				textOffset = new Vector2(side * _Style.TextOffset * _Style.DimScaleOverall, _Style.TextOffset * _Style.DimScaleOverall);
				attachment = side >= 0 ? MTextAttachmentPoint.BottomLeft : MTextAttachmentPoint.BottomRight;
			}

			Vector2 position = this.Hook + textOffset;
			Vector3 mTextPosition = MathHelper.Transform(position, this.Normal, this.Elevation);
			MText entity = new MText(text, mTextPosition, _Style.TextHeight * _Style.DimScaleOverall, 0.0, _Style.TextStyle)
			{
				Color = _Style.TextColor.IsByBlock ? AciColor.ByLayer : _Style.TextColor,
				AttachmentPoint = attachment
			};

			if (!MathHelper.IsZero(this.Vertexes[this.Vertexes.Count - 1].Y - this.Vertexes[this.Vertexes.Count - 2].Y))
			{
				this.HasHookline = true;
			}

			return entity;
		}
		private Insert BuildAnnotation(Block block)
			=> new Insert(block, this.Vertexes[this.Vertexes.Count - 1])
			{
				Color = _Style.TextColor.IsByBlock ? AciColor.ByLayer : _Style.TextColor
			};
		private Tolerance BuildAnnotation(ToleranceEntry tolerance)
			=> new Tolerance(tolerance, this.Vertexes[this.Vertexes.Count - 1])
			{
				Color = _Style.TextColor.IsByBlock ? AciColor.ByLayer : _Style.TextColor,
				Style = _Style
			};

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override void TransformBy(Matrix3 transformation, Vector3 translation)
		{
			Vector3 newNormal = transformation * this.Normal;
			if (Vector3.Equals(Vector3.Zero, newNormal))
			{
				newNormal = this.Normal;
			}
			double newElevation = this.Elevation;

			Matrix3 transOW = MathHelper.ArbitraryAxis(this.Normal);
			Matrix3 transWO = MathHelper.ArbitraryAxis(newNormal).Transpose();

			for (int i = 0; i < this.Vertexes.Count; i++)
			{
				Vector3 v = transOW * new Vector3(this.Vertexes[i].X, this.Vertexes[i].Y, this.Elevation);
				v = transformation * v + translation;
				v = transWO * v;
				this.Vertexes[i] = new Vector2(v.X, v.Y);
				newElevation = v.Z;
			}

			Vector3 newOffset = transOW * new Vector3(this.Offset.X, this.Offset.Y, this.Elevation);
			newOffset = transformation * newOffset;
			newOffset = transWO * newOffset;
			this.Offset = new Vector2(newOffset.X, newOffset.Y);

			this.Elevation = newElevation;
			this.Normal = newNormal;

			_Annotation?.TransformBy(transformation, translation);
		}

		/// <inheritdoc/>
		public override object Clone()
		{
			Leader entity = new Leader(this.Vertexes)
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
				//Leader properties
				Elevation = this.Elevation,
				Style = (DimensionStyle)_Style.Clone(),
				ShowArrowhead = this.ShowArrowhead,
				PathType = this.PathType,
				LineColor = _LineColor,
				Annotation = (EntityObject)_Annotation?.Clone(),
				Offset = this.Offset,
				HasHookline = _HasHookline
			};

			foreach (DimensionStyleOverride styleOverride in this.StyleOverrides.Values)
			{
				object copy = styleOverride.Value is ICloneable value ? value.Clone() : styleOverride.Value;
				entity.StyleOverrides.Add(new DimensionStyleOverride(styleOverride.Type, copy));
			}

			foreach (XData data in this.XData.Values)
			{
				entity.XData.Add((XData)data.Clone());
			}

			return entity;
		}

		#endregion

		#region Dimension style overrides events

		private void StyleOverrides_BeforeAddingItem(object sender, BeforeItemChangeEventArgs<DimensionStyleOverride> e)
		{
			if (sender is not DimensionStyleOverrideDictionary senderT)
				return;

			if (senderT.TryGetValue(e.Item.Type, out DimensionStyleOverride old))
			{
				if (ReferenceEquals(old.Value, e.Item.Value))
				{
					e.Cancel = true;
				}
			}
		}

		private void StyleOverrides_AfterAddingItem(object sender, AfterItemChangeEventArgs<DimensionStyleOverride> e)
			=> this.OnAfterAddingDimensionStyleOverride(e.Item, nameof(StyleOverrides));

		private void StyleOverrides_BeforeRemovingItem(object sender, BeforeItemChangeEventArgs<DimensionStyleOverride> e)
		{
		}

		private void StyleOverrides_AfterRemovingItem(object sender, AfterItemChangeEventArgs<DimensionStyleOverride> e)
			=> this.OnAfterRemovingDimensionStyleOverride(e.Item, nameof(StyleOverrides));

		#endregion
	}
}