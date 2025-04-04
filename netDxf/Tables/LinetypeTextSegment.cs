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
using System.Runtime.CompilerServices;

namespace netDxf.Tables
{
	/// <summary>Represents a text linetype segment.</summary>
	public class LinetypeTextSegment :
		LinetypeSegment
	{
		#region delegates and events

		/// <summary>Generated when a property of <see cref="TextStyle"/> type changes.</summary>
		public event BeforeValueChangeEventHandler<TextStyle> BeforeChangingTextStyleValue;
		/// <summary>Generates the <see cref="BeforeChangingTextStyleValue"/> event.</summary>
		/// <param name="oldValue">The old value, being changed.</param>
		/// <param name="newValue">The new value, that will replace the old one.</param>
		/// <param name="propertyName">(automatic) Name of the affected property.</param>
		protected virtual TextStyle OnBeforeChangingBeforeValueTextStyle(TextStyle oldValue, TextStyle newValue, [CallerMemberName] string propertyName = "")
		{
			if (this.BeforeChangingTextStyleValue is { } handler)
			{
				var e = new BeforeValueChangeEventArgs<TextStyle>(propertyName, oldValue, newValue);
				handler(this, e);
				return e.NewValue;
			}
			return newValue;
		}

		#endregion

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		public LinetypeTextSegment()
			: this(string.Empty, TextStyle.Default, 1.0, Vector2.Zero, LinetypeSegmentRotationType.Relative, 0.0, 1.0)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="text">Text to display on the linetype segment.</param>
		/// <param name="style">Name of the TextStyle.</param>
		/// <param name="length">Dash, dot, or space length of the linetype segment.</param>
		public LinetypeTextSegment(string text, TextStyle style, double length)
			: this(text, style, length, Vector2.Zero, LinetypeSegmentRotationType.Relative, 0.0, 1.0)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="text">Text to display on the linetype segment.</param>
		/// <param name="style">Name of the TextStyle.</param>
		/// <param name="length">Dash, dot, or space length of the linetype segment.</param>
		/// <param name="offset">Shift of the shape along the line.</param>
		/// <param name="rotationType">Type of rotation defined by the rotation value.</param>
		/// <param name="rotation">Rotation of the text.</param>
		/// <param name="scale">Scale of the text.</param>
		public LinetypeTextSegment(string text, TextStyle style, double length, Vector2 offset, LinetypeSegmentRotationType rotationType, double rotation, double scale)
			: base(LinetypeSegmentType.Text, length)
		{
			_Text = string.IsNullOrEmpty(text) ? string.Empty : text;
			_Style = style ?? throw new ArgumentNullException(nameof(style), "The style must be a valid TextStyle.");
			this.Offset = offset;
			this.RotationType = rotationType;
			_Rotation = MathHelper.NormalizeAngle(rotation);
			this.Scale = scale;
		}

		#endregion

		#region public properties

		private string _Text;
		/// <summary>Gets or sets the text displayed by the linetype.</summary>
		public string Text
		{
			get => _Text;
			set => _Text = string.IsNullOrEmpty(value) ? string.Empty : value;
		}

		private TextStyle _Style;
		/// <summary>Gets or sets the TextStyle of the text to be displayed by the linetype.</summary>
		public TextStyle Style
		{
			get => _Style;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_Style = this.OnBeforeChangingBeforeValueTextStyle(_Style, value);
			}
		}

		/// <summary>Gets or sets the shift of the text along the line.</summary>
		public Vector2 Offset { get; set; }

		/// <summary>Gets or sets the type of rotation defined by the rotation value upright, relative, or absolute.</summary>
		public LinetypeSegmentRotationType RotationType { get; set; }

		private double _Rotation;
		/// <summary>Gets or sets the angle in degrees of the text.</summary>
		public double Rotation
		{
			get => _Rotation;
			set => _Rotation = MathHelper.NormalizeAngle(value);
		}

		/// <summary>Gets or sets the scale of the text relative to the scale of the linetype.</summary>
		public double Scale { get; set; }

		#endregion

		#region overrides

		/// <inheritdoc/>
		public override object Clone()
			=> new LinetypeTextSegment(_Text, (TextStyle)_Style.Clone(), this.Length, this.Offset, this.RotationType, _Rotation, this.Scale);

		#endregion
	}
}