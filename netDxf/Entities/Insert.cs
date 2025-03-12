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
using netDxf.Blocks;
using netDxf.Collections;
using netDxf.Tables;
using netDxf.Units;

namespace netDxf.Entities
{
	/// <summary>Represents a block insertion <see cref="EntityObject">entity</see>.</summary>
	public class Insert :
		EntityObject
	{
		#region delegates and events

		//public delegate void BlockChangedEventHandler(Insert sender, TableObjectChangedEventArgs<Block> e);
		//public event BlockChangedEventHandler BlockChanged;
		//protected virtual Block OnBlockChangedEvent(Block oldBlock, Block newBlock)
		//{
		//	BlockChangedEventHandler ae = this.BlockChanged;
		//	if (ae != null)
		//	{
		//		TableObjectChangedEventArgs<Block> eventArgs = new TableObjectChangedEventArgs<Block>(oldBlock, newBlock);
		//		ae(this, eventArgs);
		//		return eventArgs.NewValue;
		//	}
		//	return newBlock;
		//}

		public delegate void AttributeAddedEventHandler(Insert sender, AttributeChangeEventArgs e);
		public event AttributeAddedEventHandler AttributeAdded;
		protected virtual void OnAttributeAddedEvent(Attribute item)
		{
			AttributeAddedEventHandler ae = this.AttributeAdded;
			if (ae != null)
			{
				ae(this, new AttributeChangeEventArgs(item));
			}
		}

		public delegate void AttributeRemovedEventHandler(Insert sender, AttributeChangeEventArgs e);
		public event AttributeRemovedEventHandler AttributeRemoved;
		protected virtual void OnAttributeRemovedEvent(Attribute item)
		{
			AttributeRemovedEventHandler ae = this.AttributeRemoved;
			if (ae != null)
			{
				ae(this, new AttributeChangeEventArgs(item));
			}
		}

		#endregion

		#region private fields

		private Block block;
		private Vector3 scale;
		private double rotation;

		#endregion

		#region constructors

		internal Insert(List<Attribute> attributes)
			: base(EntityType.Insert, DxfObjectCode.Insert)
		{
			if (attributes == null)
			{
				throw new ArgumentNullException(nameof(attributes));
			}

			this.Attributes = new AttributeCollection(attributes);
			foreach (Attribute att in this.Attributes)
			{
				if (att.Owner != null)
				{
					throw new ArgumentException("The attributes list contains at least an attribute that already has an owner.", nameof(attributes));
				}
				att.Owner = this;
			}

			this.block = null;
			this.Position = Vector3.Zero;
			this.scale = new Vector3(1.0);
			this.rotation = 0.0;
		}

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="block">Insert <see cref="Block">block definition</see>.</param>
		public Insert(Block block)
			: this(block, Vector3.Zero)
		{
		}

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="block">Insert block definition.</param>
		/// <param name="position">Insert <see cref="Vector2">position</see> in world coordinates.</param>
		public Insert(Block block, Vector2 position)
			: this(block, new Vector3(position.X, position.Y, 0.0))
		{
		}

		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="block">Insert block definition.</param>
		/// <param name="position">Insert <see cref="Vector3">point</see> in world coordinates.</param>
		public Insert(Block block, Vector3 position)
			: base(EntityType.Insert, DxfObjectCode.Insert)
		{
			this.block = block ?? throw new ArgumentNullException(nameof(block));
			this.Position = position;
			this.scale = new Vector3(1.0);
			this.rotation = 0.0;

			List<Attribute> atts = new List<Attribute>(block.AttributeDefinitions.Count);
			foreach (AttributeDefinition attdef in block.AttributeDefinitions.Values)
			{
				Attribute att = new Attribute(attdef)
				{
					Position = attdef.Position + this.Position - this.block.Origin,
					Owner = this
				};
				atts.Add(att);
			}

			this.Attributes = new AttributeCollection(atts);
		}

		#endregion

		#region public properties

		/// <summary>Gets or sets the default drawing units to obtain the Insert transformation matrix, when the current Insert entity does not belong to a <b>DXF</b> document.</summary>
		public static DrawingUnits DefaultInsUnits { get; set; }

		/// <summary>Gets the insert list of <see cref="Attribute">attributes</see>.</summary>
		public AttributeCollection Attributes { get; private set; }

		/// <summary>Gets the insert <see cref="Block">block definition</see>.</summary>
		public Block Block
		{
			get => this.block;
			internal set
			{
				//if (value == null)
				//{
				//	throw new ArgumentNullException(nameof(value));
				//}

				//if (value.IsForInternalUseOnly)
				//{
				//	throw new ArgumentException("The block is for internal use only.");
				//}

				//this.block = this.OnBlockChangedEvent(this.block, value);

				//// remove all attributes in the actual insert
				//foreach (Attribute att in this.Attributes)
				//{
				//	this.OnAttributeRemovedEvent(att);
				//	att.Handle = null;
				//	att.Owner = null;
				//}
				//this.Attributes = new AttributeCollection();

				this.block = value;
			}
		}

		/// <summary>Gets or sets the <see cref="Vector3">position</see> in world coordinates.</summary>
		public Vector3 Position { get; set; }

		/// <summary>Gets or sets the insert <see cref="Vector3">scale</see>.</summary>
		/// <remarks>None of the vector scale components can be zero.</remarks>
		public Vector3 Scale
		{
			get => this.scale;
			set
			{
				if (MathHelper.IsZero(value.X) || MathHelper.IsZero(value.Y) || MathHelper.IsZero(value.Z))
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "None of the vector scale components can be zero.");
				}

				this.scale = value;
			}
		}

		/// <summary>Gets or sets the insert rotation along the normal vector in degrees.</summary>
		public double Rotation
		{
			get => this.rotation;
			set => this.rotation = MathHelper.NormalizeAngle(value);
		}

		#endregion

		#region public methods

		/// <summary>Updates the attribute list of the actual insert with the attribute definition list of the referenced block. This does not affect any value assigned to the Value property.</summary>
		/// <remarks>
		/// This method will automatically call the TransformAttributes method, to keep all attributes position and orientation up to date.<br />
		/// This method will does not change the values assigned to attributes in the actual insert, besides the ones modified by the TransformAttributes() method;
		/// position, normal, rotation, text height, width factor, oblique angle, is backwards, is upside down, and alignment values.
		/// </remarks>
		public void Sync()
		{
			List<Attribute> atts = new List<Attribute>();

			// remove all attributes that have no attribute definition in the block
			foreach (Attribute att in this.Attributes)
			{
				string tag = att.Tag;
				if (this.block.AttributeDefinitions.ContainsTag(tag))
				{
					atts.Add(att);
				}
				else
				{
					this.OnAttributeRemovedEvent(att);
					att.Handle = null;
					att.Owner = null;
				}
			}

			// add any new attributes from the attribute definitions of the block
			foreach (AttributeDefinition attdef in this.block.AttributeDefinitions.Values)
			{
				if (this.Attributes.AttributeWithTag(attdef.Tag) == null)
				{
					Attribute att = new Attribute(attdef)
					{
						Owner = this
					};

					atts.Add(att);
					this.OnAttributeAddedEvent(att);
				}
			}
			this.Attributes = new AttributeCollection(atts);

			this.TransformAttributes();
		}

		/// <summary>Gets the insert transformation matrix.</summary>
		/// <returns>The insert transformation matrix.</returns>
		/// <remarks>
		/// This method uses the DefaultInsUnits defined by the Insert class to obtain the scale between the drawing and the block units.
		/// Additionally, if the insert belongs to a block the units to use are the ones defined in the BlockRecord,
		/// and if the insert belongs to a layout the units to use are the ones defined in the document drawing variable InsUnits.
		/// </remarks>
		public Matrix3 GetTransformation()
		{
			DrawingUnits insUnits;
			if (this.Owner == null)
			{
				insUnits = DefaultInsUnits;
			}
			else
			{
				insUnits = this.Owner.Record.Layout == null ? this.Owner.Record.Units : this.Owner.Record.Owner.Owner.DrawingVariables.InsUnits;
			}

			double docScale = UnitHelper.ConversionFactor(this.Block.Record.Units, insUnits);
			Matrix3 trans = MathHelper.ArbitraryAxis(this.Normal);
			trans *= Matrix3.RotationZ(this.rotation * MathHelper.DegToRad);
			trans *= Matrix3.Scale(this.scale * docScale);

			return trans;
		}

		/// <summary>Calculates the insertion rotation matrix.</summary>
		/// <param name="insertionUnits">The insertion units.</param>
		/// <returns>The insert transformation matrix.</returns>
		public Matrix3 GetTransformation(DrawingUnits insertionUnits)
		{
			double docScale = UnitHelper.ConversionFactor(this.Block.Record.Units, insertionUnits);
			Matrix3 trans = MathHelper.ArbitraryAxis(this.Normal);
			trans *= Matrix3.RotationZ(this.rotation * MathHelper.DegToRad);
			trans *= Matrix3.Scale(this.scale * docScale);

			return trans;
		}

		/// <summary>Recalculate the attributes position, normal, rotation, height, width, width factor, oblique angle, backwards, and upside down properties from the transformation state of the insertion.</summary>
		/// <remarks>
		/// Making changes to the insert position, rotation, normal, and/or scale;
		/// when changing the block origin and/or units; or even the document insertion units will require this method to be called manually.<br />
		/// The attributes position, normal, rotation, text height, width factor, and oblique angle values includes the transformations applied to the insertion,
		/// if required this method will calculate the proper values according to the ones defined by the attribute definition.<br />
		/// All the attribute values can be changed manually independently to its definition,
		/// but, usually, you will want them to be transformed with the insert based on the local values defined by the attribute definition.<br />
		/// This method only applies to attributes that have a definition, some <b>DXF</b> files might generate attributes that have no definition in the block.
		/// </remarks>
		public void TransformAttributes()
		{
			// if the insert does not contain attributes there is nothing to do
			if (this.Attributes.Count == 0)
			{
				return;
			}

			Matrix3 transformation = this.GetTransformation();
			Vector3 translation = this.Position - transformation * this.block.Origin;

			foreach (Attribute att in this.Attributes)
			{
				AttributeDefinition attDef = att.Definition;
				if (attDef == null)
				{
					// do not modify attributes with no attribute definition.
					// They will need to be handle manually
					continue;
				}

				// reset the attribute to its default values
				att.Position = attDef.Position;
				att.Height = attDef.Height;
				att.Width = attDef.Width;
				att.WidthFactor = attDef.WidthFactor;
				att.ObliqueAngle = attDef.ObliqueAngle;
				att.Rotation = attDef.Rotation;
				att.Normal = attDef.Normal;
				att.IsBackward = attDef.IsBackward;
				att.IsUpsideDown = attDef.IsUpsideDown;

				att.TransformBy(transformation, translation);
			}
		}

		/// <summary>Explodes the current insert.</summary>
		/// <returns>A list of entities.</returns>
		public List<EntityObject> Explode()
		{
			List<EntityObject> entities = new List<EntityObject>();
			Matrix3 transformation = this.GetTransformation();
			Vector3 translation = this.Position - transformation * this.block.Origin;

			foreach (EntityObject entity in this.block.Entities)
			{
				Vector3 localScale = MathHelper.Transform(this.Scale, entity.Normal, CoordinateSystem.World, CoordinateSystem.Object);
				bool isUniformScale = MathHelper.IsEqual(localScale.X, localScale.Y);

				// entities with reactors are associated with other entities they will handle the transformation
				if (entity.Reactors.Count > 0)
				{
					continue;
				}

				if (!isUniformScale)
				{
					switch (entity.Type)
					{
						case EntityType.Circle:
							{
								Circle circle = (Circle)entity;
								Ellipse ellipse = new Ellipse(circle.Center, 2 * circle.Radius, 2 * circle.Radius)
								{
									//EntityObject properties
									Layer = (Layer)entity.Layer.Clone(),
									Linetype = (Linetype)entity.Linetype.Clone(),
									Color = (AciColor)entity.Color.Clone(),
									Lineweight = entity.Lineweight,
									Transparency = (Transparency)entity.Transparency.Clone(),
									LinetypeScale = entity.LinetypeScale,
									Normal = entity.Normal,
									IsVisible = entity.IsVisible,
									//Ellipse properties
									Thickness = circle.Thickness
								};

								ellipse.TransformBy(transformation, translation);
								entities.Add(ellipse);
								break;
							}
						case EntityType.Arc:
							{
								Arc arc = (Arc)entity;
								Ellipse ellipse = new Ellipse(arc.Center, 2 * arc.Radius, 2 * arc.Radius)
								{
									//EntityObject properties
									Layer = (Layer)entity.Layer.Clone(),
									Linetype = (Linetype)entity.Linetype.Clone(),
									Color = (AciColor)entity.Color.Clone(),
									Lineweight = entity.Lineweight,
									Transparency = (Transparency)entity.Transparency.Clone(),
									LinetypeScale = entity.LinetypeScale,
									Normal = entity.Normal,
									IsVisible = entity.IsVisible,
									//Ellipse properties
									StartAngle = arc.StartAngle,
									EndAngle = arc.EndAngle,
									Thickness = arc.Thickness
								};

								ellipse.TransformBy(transformation, translation);
								entities.Add(ellipse);
								break;
							}
						case EntityType.Polyline2D:
							{
								List<EntityObject> newEntities = ((Polyline2D)entity).Explode();
								foreach (EntityObject newEntity in newEntities)
								{
									if (newEntity.Type == EntityType.Arc)
									{
										Arc arc = (Arc)newEntity;
										Ellipse ellipse = new Ellipse(arc.Center, 2 * arc.Radius, 2 * arc.Radius)
										{
											//EntityObject properties
											Layer = (Layer)entity.Layer.Clone(),
											Linetype = (Linetype)entity.Linetype.Clone(),
											Color = (AciColor)entity.Color.Clone(),
											Lineweight = entity.Lineweight,
											Transparency = (Transparency)entity.Transparency.Clone(),
											LinetypeScale = entity.LinetypeScale,
											Normal = entity.Normal,
											IsVisible = entity.IsVisible,
											//Ellipse properties
											StartAngle = arc.StartAngle,
											EndAngle = arc.EndAngle,
											Thickness = arc.Thickness
										};

										ellipse.TransformBy(transformation, translation);
										entities.Add(ellipse);
									}
									else
									{
										newEntity.TransformBy(transformation, translation);
										entities.Add(newEntity);
									}
								}
								break;
							}
						case EntityType.MLine:
							{
								List<EntityObject> newEntities = ((MLine)entity).Explode();
								foreach (EntityObject newEntity in newEntities)
								{
									if (newEntity.Type == EntityType.Arc)
									{
										Arc arc = (Arc)newEntity;
										Ellipse ellipse = new Ellipse(arc.Center, 2 * arc.Radius, 2 * arc.Radius)
										{
											//EntityObject properties
											Layer = (Layer)entity.Layer.Clone(),
											Linetype = (Linetype)entity.Linetype.Clone(),
											Color = (AciColor)entity.Color.Clone(),
											Lineweight = entity.Lineweight,
											Transparency = (Transparency)entity.Transparency.Clone(),
											LinetypeScale = entity.LinetypeScale,
											Normal = entity.Normal,
											IsVisible = entity.IsVisible,
											//Ellipse properties
											StartAngle = arc.StartAngle,
											EndAngle = arc.EndAngle,
											Thickness = arc.Thickness
										};

										ellipse.TransformBy(transformation, translation);
										entities.Add(ellipse);
									}
									else
									{
										newEntity.TransformBy(transformation, translation);
										entities.Add(newEntity);
									}
								}
								break;
							}
						default:
							{
								EntityObject newEntity = (EntityObject)entity.Clone();
								newEntity.TransformBy(transformation, translation);
								entities.Add(newEntity);
								break;
							}
					}
				}
				else
				{
					EntityObject newEntity = (EntityObject)entity.Clone();
					newEntity.TransformBy(transformation, translation);
					entities.Add(newEntity);
				}
			}

			foreach (Attribute attribute in this.Attributes)
			{
				// the attributes will be exploded as a Text entity
				Text text = new Text
				{
					//Attribute properties
					Layer = (Layer)attribute.Layer.Clone(),
					Linetype = (Linetype)attribute.Linetype.Clone(),
					Color = (AciColor)attribute.Color.Clone(),
					Lineweight = attribute.Lineweight,
					Transparency = (Transparency)attribute.Transparency.Clone(),
					LinetypeScale = attribute.LinetypeScale,
					Normal = attribute.Normal,
					IsVisible = attribute.IsVisible,
					Height = attribute.Height,
					WidthFactor = attribute.WidthFactor,
					ObliqueAngle = attribute.ObliqueAngle,
					Value = attribute.Value,
					Style = (TextStyle)attribute.Style.Clone(),
					Position = attribute.Position,
					Rotation = attribute.Rotation,
					Alignment = attribute.Alignment,
					IsBackward = attribute.IsBackward,
					IsUpsideDown = attribute.IsUpsideDown
				};
				entities.Add(text);
			}

			return entities;
		}

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override void TransformBy(Matrix3 transformation, Vector3 translation)
		{
			Vector3 newPosition = transformation * this.Position + translation;
			Vector3 newNormal = transformation * this.Normal;
			if (Vector3.Equals(Vector3.Zero, newNormal))
			{
				newNormal = this.Normal;
			}

			Matrix3 transOW = MathHelper.ArbitraryAxis(this.Normal);
			transOW *= Matrix3.RotationZ(this.Rotation * MathHelper.DegToRad);

			Matrix3 transWO = MathHelper.ArbitraryAxis(newNormal);
			transWO = transWO.Transpose();

			Vector3 v = transOW * Vector3.UnitX;
			v = transformation * v;
			v = transWO * v;
			double newRotation = Vector2.Angle(new Vector2(v.X, v.Y));

			transWO = Matrix3.RotationZ(newRotation).Transpose() * transWO;

			Vector3 s = transOW * this.Scale;
			s = transformation * s;
			s = transWO * s;
			Vector3 newScale = new Vector3(
				MathHelper.IsZero(s.X) ? MathHelper.Epsilon : s.X,
				MathHelper.IsZero(s.Y) ? MathHelper.Epsilon : s.Y,
				MathHelper.IsZero(s.Z) ? MathHelper.Epsilon : s.Z);

			this.Normal = newNormal;
			this.Position = newPosition;
			this.Scale = newScale;
			this.Rotation = newRotation * MathHelper.RadToDeg;

			foreach (Attribute att in this.Attributes)
			{
				att.TransformBy(transformation, translation);
			}
		}

		/// <inheritdoc/>
		internal override long AssignHandle(long entityNumber)
		{
			foreach (Attribute attrib in this.Attributes)
			{
				entityNumber = attrib.AssignHandle(entityNumber);
			}
			return base.AssignHandle(entityNumber);
		}

		/// <inheritdoc/>
		public override object Clone()
		{
			// copy attributes
			List<Attribute> copyAttributes = new List<Attribute>();
			foreach (Attribute att in this.Attributes)
				copyAttributes.Add((Attribute)att.Clone());

			Insert entity = new Insert(copyAttributes)
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
				//Insert properties
				Position = this.Position,
				Block = (Block)this.block.Clone(),
				Scale = this.scale,
				Rotation = this.rotation,
			};

			// copy extended data
			foreach (XData data in this.XData.Values)
				entity.XData.Add((XData)data.Clone());

			return entity;
		}

		#endregion
	}
}