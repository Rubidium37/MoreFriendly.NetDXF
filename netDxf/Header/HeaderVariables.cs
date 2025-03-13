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
using System.Text;
using netDxf.Entities;
using netDxf.Tables;
using netDxf.Units;

namespace netDxf.Header
{
	/// <summary>Represents the header variables of a <b>DXF</b> document.</summary>
	public class HeaderVariables
	{
		private readonly Dictionary<string, HeaderVariable> StandardVariables = new Dictionary<string, HeaderVariable>(StringComparer.OrdinalIgnoreCase)
		{
			{HeaderVariableCode.AcadVer, new HeaderVariable(HeaderVariableCode.AcadVer, 1, DxfVersion.AutoCad2000)},
			{HeaderVariableCode.DwgCodePage, new HeaderVariable(HeaderVariableCode.DwgCodePage, 3, "ANSI_" + Encoding.ASCII.WindowsCodePage)},
			{HeaderVariableCode.LastSavedBy, new HeaderVariable(HeaderVariableCode.LastSavedBy, 1, Environment.UserName)},
			{HeaderVariableCode.HandleSeed, new HeaderVariable(HeaderVariableCode.HandleSeed, 5, "1")},
			{HeaderVariableCode.Angbase, new HeaderVariable(HeaderVariableCode.Angbase, 50, 0.0)},
			{HeaderVariableCode.Angdir, new HeaderVariable(HeaderVariableCode.Angdir, 70, AngleDirection.CCW)},
			{HeaderVariableCode.AttMode, new HeaderVariable(HeaderVariableCode.AttMode, 70, AttMode.Normal)},
			{HeaderVariableCode.AUnits, new HeaderVariable(HeaderVariableCode.AUnits, 70, AngleUnitType.DecimalDegrees)},
			{HeaderVariableCode.AUprec, new HeaderVariable(HeaderVariableCode.AUprec, 70, (short) 0)},
			{HeaderVariableCode.CeColor, new HeaderVariable(HeaderVariableCode.CeColor, 62, AciColor.ByLayer)},
			{HeaderVariableCode.CeLtScale, new HeaderVariable(HeaderVariableCode.CeLtScale, 40, 1.0)},
			{HeaderVariableCode.CeLtype, new HeaderVariable(HeaderVariableCode.CeLtype, 6, "ByLayer")},
			{HeaderVariableCode.CeLweight, new HeaderVariable(HeaderVariableCode.CeLweight, 370, Lineweight.ByLayer)},
			{HeaderVariableCode.CLayer, new HeaderVariable(HeaderVariableCode.CLayer, 8, "0")},
			{HeaderVariableCode.CMLJust, new HeaderVariable(HeaderVariableCode.CMLJust, 70, MLineJustification.Top)},
			{HeaderVariableCode.CMLScale, new HeaderVariable(HeaderVariableCode.CMLScale, 40, 20.0)},
			{HeaderVariableCode.CMLStyle, new HeaderVariable(HeaderVariableCode.CMLStyle, 2, "Standard")},
			{HeaderVariableCode.DimStyle, new HeaderVariable(HeaderVariableCode.DimStyle, 2, "Standard")},
			{HeaderVariableCode.TextSize, new HeaderVariable(HeaderVariableCode.TextSize, 40, 2.5)},
			{HeaderVariableCode.TextStyle, new HeaderVariable(HeaderVariableCode.TextStyle, 7, "Standard")},
			{HeaderVariableCode.LUnits, new HeaderVariable(HeaderVariableCode.LUnits, 70, LinearUnitType.Decimal)},
			{HeaderVariableCode.LUprec, new HeaderVariable(HeaderVariableCode.LUprec, 70, (short) 4)},
			{HeaderVariableCode.MirrText, new HeaderVariable(HeaderVariableCode.MirrText, 70, false)},
			{HeaderVariableCode.Extnames, new HeaderVariable(HeaderVariableCode.Extnames, 290, true)},
			{HeaderVariableCode.InsBase, new HeaderVariable(HeaderVariableCode.InsBase, 10, Vector3.Zero)},
			{HeaderVariableCode.InsUnits, new HeaderVariable(HeaderVariableCode.InsUnits, 70, DrawingUnits.Unitless)},
			{HeaderVariableCode.LtScale, new HeaderVariable(HeaderVariableCode.LtScale, 40, 1.0)},
			{HeaderVariableCode.LwDisplay, new HeaderVariable(HeaderVariableCode.LwDisplay, 290, false)},
			{HeaderVariableCode.PdMode, new HeaderVariable(HeaderVariableCode.PdMode, 70, PointShape.Dot)},
			{HeaderVariableCode.PdSize, new HeaderVariable(HeaderVariableCode.PdSize, 40, 0.0)},
			{HeaderVariableCode.PLineGen, new HeaderVariable(HeaderVariableCode.PLineGen, 70, (short) 0)},
			{HeaderVariableCode.PsLtScale, new HeaderVariable(HeaderVariableCode.PsLtScale, 70, (short) 1)},
			{HeaderVariableCode.SplineSegs, new HeaderVariable(HeaderVariableCode.SplineSegs, 70, (short) 8)},
			{HeaderVariableCode.SurfU, new HeaderVariable(HeaderVariableCode.SurfU, 70, (short) 6)},
			{HeaderVariableCode.SurfV, new HeaderVariable(HeaderVariableCode.SurfV, 70, (short) 6)},
			{HeaderVariableCode.TdCreate, new HeaderVariable(HeaderVariableCode.TdCreate, 40, DateTime.Now)},
			{HeaderVariableCode.TduCreate, new HeaderVariable(HeaderVariableCode.TduCreate, 40, DateTime.UtcNow)},
			{HeaderVariableCode.TdUpdate, new HeaderVariable(HeaderVariableCode.TdUpdate, 40, DateTime.Now)},
			{HeaderVariableCode.TduUpdate, new HeaderVariable(HeaderVariableCode.TduUpdate, 40, DateTime.UtcNow)},
			{HeaderVariableCode.TdinDwg, new HeaderVariable(HeaderVariableCode.TdinDwg, 40, new TimeSpan())}
		};
		private readonly Dictionary<string, HeaderVariable> CustomVariables = new Dictionary<string, HeaderVariable>(StringComparer.OrdinalIgnoreCase);

		#region constructors

		/// <summary>Initializes a new instance of the class.</summary>
		/// <remarks>The default values are the same ones that are apply to a new <b>AutoCAD</b> drawing.</remarks>
		public HeaderVariables()
		{
		}

		#endregion

		#region public properties

		/// <summary>The <b>AutoCAD</b> drawing database version number.</summary>
		/// <remarks>Only <b>AutoCad2000</b>AutoCad2000 and higher <b>DXF</b> versions are supported.</remarks>
		/// <exception cref="NotSupportedException">Only <b>AutoCad2000</b> and higher <b>DXF</b> versions are supported.</exception>
		public DxfVersion AcadVer
		{
			get => (DxfVersion)this.StandardVariables[HeaderVariableCode.AcadVer].Value;
			set
			{
				if (value < DxfVersion.AutoCad2000)
				{
					throw new NotSupportedException("Only AutoCad2000 and newer DXF versions are supported.");
				}
				this.StandardVariables[HeaderVariableCode.AcadVer].Value = value;
			}
		}

		/// <summary>Next available handle.</summary>
		public string HandleSeed
		{
			get => (string)this.StandardVariables[HeaderVariableCode.HandleSeed].Value;
			internal set => this.StandardVariables[HeaderVariableCode.HandleSeed].Value = value;
		}

		/// <summary>Angle 0 base.</summary>
		/// <remarks>Default value: 0.</remarks>
		public double Angbase
		{
			get => (double)this.StandardVariables[HeaderVariableCode.Angbase].Value;
			internal set => this.StandardVariables[HeaderVariableCode.Angbase].Value = value;
		}

		/// <summary>The angle direction.</summary>
		/// <remarks>Default value: CCW.</remarks>
		public AngleDirection Angdir
		{
			get => (AngleDirection)this.StandardVariables[HeaderVariableCode.Angdir].Value;
			internal set => this.StandardVariables[HeaderVariableCode.Angdir].Value = value;
		}

		/// <summary>Attribute visibility.</summary>
		/// <remarks>Default value: Normal.</remarks>
		public AttMode AttMode
		{
			get => (AttMode)this.StandardVariables[HeaderVariableCode.AttMode].Value;
			set => this.StandardVariables[HeaderVariableCode.AttMode].Value = value;
		}

		/// <summary>Units format for angles.</summary>
		/// <remarks>Default value: Decimal degrees.</remarks>
		public AngleUnitType AUnits
		{
			get => (AngleUnitType)this.StandardVariables[HeaderVariableCode.AUnits].Value;
			set => this.StandardVariables[HeaderVariableCode.AUnits].Value = value;
		}

		/// <summary>Units precision for angles.</summary>
		/// <remarks>Valid values are integers from 0 to 8. Default value: 0.</remarks>
		public short AUprec
		{
			get => (short)this.StandardVariables[HeaderVariableCode.AUprec].Value;
			set
			{
				if (value < 0 || value > 8)
				{
					throw new ArgumentOutOfRangeException(nameof(value), "Valid values are integers from 0 to 8.");
				}
				this.StandardVariables[HeaderVariableCode.AUprec].Value = value;
			}
		}

		/// <summary>Current entity color.</summary>
		/// <remarks>Default value: 256 (<b>ByLayer</b>). This header variable only supports indexed colors.</remarks>
		public AciColor CeColor
		{
			get => (AciColor)this.StandardVariables[HeaderVariableCode.CeColor].Value;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				this.StandardVariables[HeaderVariableCode.CeColor].Value = value;
			}
		}

		/// <summary>Current entity line type scale.</summary>
		/// <remarks>Default value: 1.0.</remarks>
		public double CeLtScale
		{
			get => (double)this.StandardVariables[HeaderVariableCode.CeLtScale].Value;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The current entity line type scale must be greater than zero.");
				}
				this.StandardVariables[HeaderVariableCode.CeLtScale].Value = value;
			}
		}

		/// <summary>Current entity line weight.</summary>
		/// <remarks>Default value: -1 (ByLayer).</remarks>
		public Lineweight CeLweight
		{
			get => (Lineweight)this.StandardVariables[HeaderVariableCode.CeLweight].Value;
			set => this.StandardVariables[HeaderVariableCode.CeLweight].Value = value;
		}

		/// <summary>Current entity line type name.</summary>
		/// <remarks>Default value: ByLayer.</remarks>
		public string CeLtype
		{
			get => (string)this.StandardVariables[HeaderVariableCode.CeLtype].Value;
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException(nameof(value), "The current entity line type name should be at least one character long.");
				}
				this.StandardVariables[HeaderVariableCode.CeLtype].Value = value;
			}
		}

		/// <summary>Current layer name.</summary>
		/// <remarks>Default value: 0.</remarks>
		public string CLayer
		{
			get => (string)this.StandardVariables[HeaderVariableCode.CLayer].Value;
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException(nameof(value), "The current layer name should be at least one character long.");
				}
				this.StandardVariables[HeaderVariableCode.CLayer].Value = value;
			}
		}

		/// <summary>Current multiline justification.</summary>
		/// <remarks>Default value: 0 (Top).</remarks>
		public MLineJustification CMLJust
		{
			get => (MLineJustification)this.StandardVariables[HeaderVariableCode.CMLJust].Value;
			set => this.StandardVariables[HeaderVariableCode.CMLJust].Value = value;
		}

		/// <summary>Current multiline scale.</summary>
		/// <remarks>Default value: 20.</remarks>
		public double CMLScale
		{
			get => (double)this.StandardVariables[HeaderVariableCode.CMLScale].Value;
			set => this.StandardVariables[HeaderVariableCode.CMLScale].Value = value;
		}

		/// <summary>Current multiline style.</summary>
		/// <remarks>Default value: Standard.</remarks>
		public string CMLStyle
		{
			get => (string)this.StandardVariables[HeaderVariableCode.CMLStyle].Value;
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException(nameof(value), "The current multiline style name should be at least one character long.");
				}
				this.StandardVariables[HeaderVariableCode.CMLStyle].Value = value;
			}
		}

		/// <summary>Current dimension style.</summary>
		/// <remarks>Default value: Standard.</remarks>
		public string DimStyle
		{
			get => (string)this.StandardVariables[HeaderVariableCode.DimStyle].Value;
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException(nameof(value), "The current dimension style name should be at least one character long.");
				}
				this.StandardVariables[HeaderVariableCode.DimStyle].Value = value;
			}
		}

		/// <summary>Default text height.</summary>
		/// <remarks>Default value: 2.5.</remarks>
		public double TextSize
		{
			get => (double)this.StandardVariables[HeaderVariableCode.TextSize].Value;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The default text height must be greater than zero.");
				}
				this.StandardVariables[HeaderVariableCode.TextSize].Value = value;
			}
		}

		/// <summary>Current text style.</summary>
		/// <remarks>Default value: Standard.</remarks>
		public string TextStyle
		{
			get => (string)this.StandardVariables[HeaderVariableCode.TextStyle].Value;
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException(nameof(value), "The current text style name should be at least one character long.");
				}
				this.StandardVariables[HeaderVariableCode.TextStyle].Value = value;
			}
		}

		/// <summary>Units format for coordinates and distances.</summary>
		/// <remarks>
		/// Default value: Decimal.<br />
		/// If the LUnits is set to <see cref="LinearUnitType.Architectural"/> or <see cref="LinearUnitType.Engineering"/> the InsUnits variable will be set to Inches automatically.
		/// </remarks>
		public LinearUnitType LUnits
		{
			get => (LinearUnitType)this.StandardVariables[HeaderVariableCode.LUnits].Value;
			set
			{
				if (value == LinearUnitType.Architectural || value == LinearUnitType.Engineering)
				{
					this.InsUnits = DrawingUnits.Inches;
				}
				this.StandardVariables[HeaderVariableCode.LUnits].Value = value;
			}
		}

		/// <summary>Units precision for coordinates and distances.</summary>
		/// <remarks>Valid values are integers from 0 to 8. Default value: 4.</remarks>
		public short LUprec
		{
			get => (short)this.StandardVariables[HeaderVariableCode.LUprec].Value;
			set
			{
				if (value < 0 || value > 8)
				{
					throw new ArgumentOutOfRangeException(nameof(value), "Valid values are integers from 0 to 8.");
				}
				this.StandardVariables[HeaderVariableCode.LUprec].Value = value;
			}
		}

		/// <summary>Drawing code page; set to the system code page when a new drawing is created, but not otherwise maintained by AutoCAD.</summary>
		public string DwgCodePage
		{
			get => (string)this.StandardVariables[HeaderVariableCode.DwgCodePage].Value;
			internal set => this.StandardVariables[HeaderVariableCode.DwgCodePage].Value = value;
		}

		/// <summary>Controls symbol table naming.</summary>
		/// <remarks>
		/// Default value: 1.<br />
		/// Controls symbol table naming:<br />
		/// 0 = Release 14 compatibility. Limits names to 31 characters in length.<br />
		/// Names can include the letters A to Z, the numerals 0 to 9,
		/// and the special characters dollar sign ($), underscore (_), and hyphen (-).<br />
		/// 1 = <b>AutoCAD</b> 2000.<br />
		/// Names can be up to 255 characters in length,
		/// and can include the letters A to Z, the numerals 0 to 9, spaces,
		/// and any special characters not used for other purposes by Microsoft Windows and <b>AutoCAD</b>.
		/// </remarks>
		public bool Extnames
		{
			get => (bool)this.StandardVariables[HeaderVariableCode.Extnames].Value;
			internal set => this.StandardVariables[HeaderVariableCode.Extnames].Value = value;
		}

		/// <summary>Insertion base point for the current drawing.</summary>
		/// <remarks>
		/// When you insert or externally reference the current drawing into other drawings, this base point is used as the insertion base point.
		/// </remarks>
		public Vector3 InsBase
		{
			get => (Vector3)this.StandardVariables[HeaderVariableCode.InsBase].Value;
			set => this.StandardVariables[HeaderVariableCode.InsBase].Value = value;
		}

		/// <summary>Specifies a drawing units value for automatic scaling of blocks, images, or xRefs when inserted or attached to a drawing.</summary>
		/// <remarks>
		/// Default value: Unitless.<br />
		/// It is not recommend to change this value, if the LUnits variable has been set to <see cref="LinearUnitType.Architectural"/> or <see cref="LinearUnitType.Engineering"/>, they require the InsUnits to be set at Inches.
		/// </remarks>
		public DrawingUnits InsUnits
		{
			get => (DrawingUnits)this.StandardVariables[HeaderVariableCode.InsUnits].Value;
			set => this.StandardVariables[HeaderVariableCode.InsUnits].Value = value;
		}

		/// <summary>User name that saved the file.</summary>
		/// <remarks>
		/// By default it uses the user name of the person who is currently logged on to the Windows operating system.<br />
		/// This header variable is not compatible with <b>AutoCad2000</b> or lower versions.
		/// </remarks>
		public string LastSavedBy
		{
			get => (string)this.StandardVariables[HeaderVariableCode.LastSavedBy].Value;
			set => this.StandardVariables[HeaderVariableCode.LastSavedBy].Value = value;
		}

		/// <summary>Global line type scale.</summary>
		/// <remarks>Default value: 1.0.</remarks>
		public double LtScale
		{
			get => (double)this.StandardVariables[HeaderVariableCode.LtScale].Value;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "The global line type scale must be greater than zero.");
				}
				this.StandardVariables[HeaderVariableCode.LtScale].Value = value;
			}
		}

		/// <summary>Controls the display of line weights on the Model or Layout tab.</summary>
		/// <remarks>
		/// Default value: <see langword="false"/>.<br />
		/// <see langword="false"/> = Line weight is not displayed.<br />
		/// <see langword="true"/> = Line weight is displayed.<br />
		/// </remarks>
		public bool LwDisplay
		{
			get => (bool)this.StandardVariables[HeaderVariableCode.LwDisplay].Value;
			set => this.StandardVariables[HeaderVariableCode.LwDisplay].Value = value;
		}

		/// <summary>Controls if the text will be mirrored during a symmetry.</summary>
		public bool MirrText
		{
			get => (bool)this.StandardVariables[HeaderVariableCode.MirrText].Value;
			set => this.StandardVariables[HeaderVariableCode.MirrText].Value = value;
		}

		/// <summary>Controls the <see cref="PointShape">shape</see> to draw a point entity.</summary>
		/// <remarks>Default value: PointShape.Dot.</remarks>
		public PointShape PdMode
		{
			get => (PointShape)this.StandardVariables[HeaderVariableCode.PdMode].Value;
			set => this.StandardVariables[HeaderVariableCode.PdMode].Value = value;
		}

		/// <summary>Controls the size of the point figures, except for <b>PDMODE</b> values 0 (Dot) and 1 (Empty).</summary>
		/// <remarks>
		/// Default value: 0.<br />
		/// A setting of 0 generates the point at 5 percent of the drawing area height.<br />
		/// A positive <b>PDSIZE</b> value specifies an absolute size for the point figures.<br />
		/// A negative value is interpreted as a percentage of the viewport size. <br />
		/// </remarks>
		public double PdSize
		{
			get => (double)this.StandardVariables[HeaderVariableCode.PdSize].Value;
			set => this.StandardVariables[HeaderVariableCode.PdSize].Value = value;
		}

		/// <summary>Governs the generation of line type patterns around the vertexes of a 2D polyline.</summary>
		/// <remarks>
		/// Default value: 0.<br />
		/// 1 = Line type is generated in a continuous pattern around vertexes of the polyline.<br />
		/// 0 = Each segment of the polyline starts and ends with a dash.
		/// </remarks>
		public short PLineGen
		{
			get => (short)this.StandardVariables[HeaderVariableCode.PLineGen].Value;
			set
			{
				if (value != 0 && value != 1)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "Accepted values are 0 or 1.");
				}
				this.StandardVariables[HeaderVariableCode.PLineGen].Value = value;
			}
		}

		/// <summary>Controls paper space line type scaling.</summary>
		/// <remarks>
		/// Default value: 1.<br />
		/// 1 = No special line type scaling.<br />
		/// 0 = Viewport scaling governs line type scaling.
		/// </remarks>
		public short PsLtScale
		{
			get => (short)this.StandardVariables[HeaderVariableCode.PsLtScale].Value;
			set
			{
				if (value != 0 && value != 1)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "Accepted values are 0 or 1.");
				}
				this.StandardVariables[HeaderVariableCode.PsLtScale].Value = value;
			}
		}

		/// <summary>Defines number of line segments generated for smoothed polylines.</summary>
		/// <remarks>
		/// Accepted values must be greater than 0. Default value: 6.<br />
		/// Even thought <b>AutoCAD</b> accepts negative values for the SplineSegs header values only positive ones are supported.
		/// </remarks>
		public short SplineSegs
		{
			get => (short)this.StandardVariables[HeaderVariableCode.SplineSegs].Value;
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "Values must be greater than 0.");
				}
				this.StandardVariables[HeaderVariableCode.SplineSegs].Value = value;
			}
		}

		/// <summary>Define the number of segments generated for smoothed polygon meshes in U direction (local X axis).</summary>
		/// <remarks>
		/// Accepted value range from 0 to 200. Default value: 6.<br />
		/// Although in <b>AutoCAD</b> the header variable SurfU accepts values less than 2, the minimum vertexes generated is 3 equivalent to a SurfV value of 2.
		/// </remarks>
		public short SurfU
		{
			get => (short)this.StandardVariables[HeaderVariableCode.SurfU].Value;
			set
			{
				if (value < 0 || value > 200)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "Values must be between 0 and 200.");
				}
				this.StandardVariables[HeaderVariableCode.SurfU].Value = value;
			}
		}

		/// <summary>Define the number of segments generated for smoothed polygon meshes in V direction (local Y axis).</summary>
		/// <remarks>
		/// Accepted value range from 0 to 200. Default value: 6.<br />
		/// Although in <b>AutoCAD</b> the header variable SurfV accepts values less than 2, the minimum vertexes generated is 3 equivalent to a SurfV value of 2.
		/// </remarks>
		public short SurfV
		{
			get => (short)this.StandardVariables[HeaderVariableCode.SurfV].Value;
			set
			{
				if (value < 0 || value > 200)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "Values must be between 0 and 200.");
				}
				this.StandardVariables[HeaderVariableCode.SurfV].Value = value;
			}
		}

		/// <summary>Local date/time of drawing creation.</summary>
		/// <remarks>
		/// This date/time is local to the time zone where the file was created.
		/// </remarks>
		public DateTime TdCreate
		{
			get => (DateTime)this.StandardVariables[HeaderVariableCode.TdCreate].Value;
			set => this.StandardVariables[HeaderVariableCode.TdCreate].Value = value;
		}

		/// <summary>Universal date/time the drawing was created.</summary>
		public DateTime TduCreate
		{
			get => (DateTime)this.StandardVariables[HeaderVariableCode.TduCreate].Value;
			set => this.StandardVariables[HeaderVariableCode.TduCreate].Value = value;
		}

		/// <summary>Local date/time of last drawing update.</summary>
		/// <remarks>This date/time is local to the time zone where the file was created.</remarks>
		public DateTime TdUpdate
		{
			get => (DateTime)this.StandardVariables[HeaderVariableCode.TdUpdate].Value;
			set => this.StandardVariables[HeaderVariableCode.TdUpdate].Value = value;
		}

		/// <summary>Universal date/time the drawing was created.</summary>
		public DateTime TduUpdate
		{
			get => (DateTime)this.StandardVariables[HeaderVariableCode.TduUpdate].Value;
			set => this.StandardVariables[HeaderVariableCode.TduUpdate].Value = value;
		}

		/// <summary>Cumulative editing time for this drawing.</summary>
		public TimeSpan TdinDwg
		{
			get => (TimeSpan)this.StandardVariables[HeaderVariableCode.TdinDwg].Value;
			set => this.StandardVariables[HeaderVariableCode.TdinDwg].Value = value;
		}

		private UCS _CurrentUCS = new UCS("Unnamed");
		/// <summary>Gets ore sets the current/active <b>UCS</b> of the drawing.</summary>
		/// <remarks>
		/// This field encapsulates the three drawing variables UcsOrg, UcsXDir, and UcsYDir.
		/// </remarks>
		public UCS CurrentUCS
		{
			get => _CurrentUCS;
			set => _CurrentUCS = value ?? throw new ArgumentNullException(nameof(value));
		}

		#endregion

		#region public methods

		/// <summary>Gets a collection of the known header variables.</summary>
		/// <returns>A list with the known header variables.</returns>
		public List<HeaderVariable> KnownValues() => new List<HeaderVariable>(this.StandardVariables.Values);

		/// <summary>Gets a collection of the known header variables names.</summary>
		/// <returns>A list with the known header variables names.</returns>
		public List<string> KnownNames() => new List<string>(this.StandardVariables.Keys);

		/// <summary>Gets a collection of the custom header variables.</summary>
		/// <returns>A list with the custom header variables.</returns>
		public List<HeaderVariable> CustomValues() => new List<HeaderVariable>(this.CustomVariables.Values);

		/// <summary>Gets a collection of the custom header variables names.</summary>
		/// <returns>A list with the custom header variables names.</returns>
		public List<string> CustomNames() => new List<string>(this.CustomVariables.Keys);

		/// <summary>Adds a custom <see cref="HeaderVariable"/> to the list.</summary>
		/// <param name="variable">Header variable to add to the list.</param>
		/// <remarks>
		/// All header variable names must start with the character '$'.<br />
		/// Header variable names that already exists in the known list cannot be added.
		/// </remarks>
		public void AddCustomVariable(HeaderVariable variable)
		{
			if (variable == null)
			{
				throw new ArgumentNullException(nameof(variable), "A custom header variable cannot be null.");
			}

			if (!variable.Name.StartsWith("$"))
			{
				throw new ArgumentException("A header variable name must start with '$'.", nameof(variable));
			}

			if (this.StandardVariables.ContainsKey(variable.Name))
			{
				throw new ArgumentException("A known header variable with the same name already exists.", nameof(variable));
			}

			this.CustomVariables.Add(variable.Name, variable);
		}

		/// <summary>Checks if a custom <see cref="HeaderVariable"/> name exits in the list.</summary>
		/// <param name="name">Header variable name.</param>
		/// <returns><see langword="true"/> if a header variable name exits in the list; otherwise, <see langword="false"/>.</returns>
		/// <remarks>The header variable name is case insensitive.</remarks>
		public bool ContainsCustomVariable(string name) => this.CustomVariables.ContainsKey(name);

		/// <summary>Gets the header variable associated with the specified name.</summary>
		/// <param name="name">The name of the header variable to get.</param>
		/// <param name="variable">When this method returns, contains the header variable associated with the specified name, if the name is found; otherwise, it contains <see langword="null"/>.</param>
		/// <returns><see langword="true"/> if the list contains a header variable with the specified name; otherwise, <see langword="false"/>.</returns>
		public bool TryGetCustomVariable(string name, out HeaderVariable variable)
			=> this.CustomVariables.TryGetValue(name, out variable);

		/// <summary>Removes a custom <see cref="HeaderVariable"/> from the list.</summary>
		/// <param name="name">Header variable to add to the list.</param>
		/// <returns><see langword="true"/> if the element is successfully found and removed; otherwise, <see langword="false"/>.</returns>
		/// <remarks>The header variable name is case insensitive.</remarks>
		public bool RemoveCustomVariable(string name) => this.CustomVariables.Remove(name);

		/// <summary>Removes all custom <see cref="HeaderVariable"/> from the list.</summary>
		public void ClearCustomVariables() => this.CustomVariables.Clear();

		#endregion
	}
}