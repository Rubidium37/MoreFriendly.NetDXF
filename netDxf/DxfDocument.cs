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
using System.IO;
using netDxf.Blocks;
using netDxf.Collections;
using netDxf.Entities;
using netDxf.Header;
using netDxf.IO;
using netDxf.Objects;
using netDxf.Tables;
using Attribute = netDxf.Entities.Attribute;

namespace netDxf
{
	/// <summary>Represents a document to read and write <b>DXF</b> files.</summary>
	/// <remarks>
	/// The <see cref="DxfDocument"/> class derives from <see cref="DxfObject"/> for convenience of this library not because of the <b>DXF</b> structure.
	/// It can contain external data (<see cref="XData"/>) information, but it is not saved in the <b>DXF</b>.
	/// </remarks>
	public sealed class DxfDocument :
		DxfObject
	{
		// DXF objects added to the document (key: handle, value: DXF object).
		internal ObservableDictionary<string, DxfObject> AddedObjects = new ObservableDictionary<string, DxfObject>(StringComparer.InvariantCultureIgnoreCase);
		// keeps track of the dimension blocks generated
		internal int DimensionBlocksIndex = -1;
		// keeps track of the group names generated (this groups have the isUnnamed set to true)
		internal int GroupNamesIndex = 0;

		#region constructor

		/// <summary>Initializes a new instance of the class.</summary>
		/// <remarks>The default <see cref="HeaderVariables">drawing variables</see> of the document will be used.</remarks>
		public DxfDocument()
			: this(new HeaderVariables())
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="supportFolders">List of the document support folders.</param>
		/// <remarks>The default <see cref="HeaderVariables">drawing variables</see> of the document will be used.</remarks>
		public DxfDocument(IEnumerable<string> supportFolders)
			: this(new HeaderVariables(), supportFolders)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="version">AutoCAD drawing database version number.</param>
		public DxfDocument(DxfVersion version)
			: this(new HeaderVariables { AcadVer = version })
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="version">AutoCAD drawing database version number.</param>
		/// <param name="supportFolders">List of the document support folders.</param>
		public DxfDocument(DxfVersion version, IEnumerable<string> supportFolders)
			: this(new HeaderVariables { AcadVer = version }, supportFolders)
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="drawingVariables"><see cref="HeaderVariables">Drawing variables</see> of the document.</param>
		public DxfDocument(HeaderVariables drawingVariables)
			: this(drawingVariables, new List<string>())
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="drawingVariables"><see cref="HeaderVariables">Drawing variables</see> of the document.</param>
		/// <param name="supportFolders">List of the document support folders.</param>
		public DxfDocument(HeaderVariables drawingVariables, IEnumerable<string> supportFolders)
			: this(drawingVariables, true, new SupportFolders(supportFolders))
		{
		}
		/// <summary>Initializes a new instance of the class.</summary>
		/// <param name="drawingVariables"><see cref="HeaderVariables">Drawing variables</see> of the document.</param>
		/// <param name="createDefaultObjects">Check if the default objects need to be created.</param>
		/// <param name="supportFolders">List of the document support folders.</param>
		internal DxfDocument(HeaderVariables drawingVariables, bool createDefaultObjects, SupportFolders supportFolders)
			: base("DOCUMENT")
		{
			this.SupportFolders = supportFolders;
			this.DrawingVariables = drawingVariables;
			this.NumHandles = this.AssignHandle(0);
			this.Entities = new DrawingEntities(this);

			this.AddedObjects.BeforeAddItem += this.AddedObjects_BeforeAddItem;
			this.AddedObjects.AddItem += this.AddedObjects_AddItem;
			this.AddedObjects.BeforeRemoveItem += this.AddedObjects_BeforeRemoveItem;
			this.AddedObjects.RemoveItem += this.AddedObjects_RemoveItem;
			this.AddedObjects.Add(this.Handle, this);

			if (createDefaultObjects)
			{
				this.AddDefaultObjects();
			}
		}

		#endregion

		#region internal properties

		private long _NumHandles;
		/// <summary>Gets or sets the number of handles generated, this value is saved as an hexadecimal in the drawing variables HandleSeed property.</summary>
		internal long NumHandles
		{
			get => _NumHandles;
			set
			{
				this.DrawingVariables.HandleSeed = value.ToString("X");
				_NumHandles = value;
			}
		}

		#endregion

		#region public properties

		/// <summary>Gets the list of folders where the drawing support files are present.</summary>
		/// <remarks>
		/// When shape linetype segments are used, the shape number will be obtained reading the .shp file equivalent to the .shx file,
		/// that file will be looked for in the same folder as the .shx file or one of the document support folders.
		/// </remarks>
		public SupportFolders SupportFolders { get; }

		//// <summary>
		//// Gets or sets if the blocks that represents dimension entities will be created when added to the document.
		//// </summary>
		/// <remarks>
		/// By default this value is set to <see langword="false"/>, no dimension blocks will be generated when adding dimension entities to the document.
		/// It will be the responsibility of the program importing the <b>DXF</b> to generate the drawing that represent the dimensions.<br />
		/// When set to <see langword="true"/> the block that represents the dimension will be generated,
		/// keep in mind that this process is limited and not all options available in the dimension style will be reflected in the final result.<br />
		/// When importing a file if the dimension block is present it will be read, regardless of this value.
		/// If, later, the dimension is modified all updates will be done with the limited dimension drawing capabilities of the library,
		/// in this case, if you want that the new modifications to be reflected when the file is saved again you can set the dimension block to <see langword="null"/>,
		/// and the program reading the resulting file will regenerate the block with the new modifications.
		/// </remarks>
		public bool BuildDimensionBlocks { get; set; } = false;

		/// <summary>Gets the document viewport.</summary>
		/// <remarks>
		/// This is the same as the <b>*Active VPort</b> in the <see cref="VPorts"/> list, it describes the current viewport.
		/// </remarks>
		public VPort Viewport => this.VPorts["*Active"];

		private RasterVariables _RasterVariables;
		/// <summary>Gets or sets the <see cref="RasterVariables"/> applied to image entities.</summary>
		public RasterVariables RasterVariables
		{
			get => _RasterVariables;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				if (string.IsNullOrEmpty(value.Handle))
				{
					this.NumHandles = value.AssignHandle(this.NumHandles);
				}
				this.AddedObjects.Add(value.Handle, value);
				_RasterVariables = value;
			}
		}

		#region header

		/// <summary>Gets or sets the name of the document, once a file is saved or loaded this field is equals the file name without extension.</summary>
		public List<string> Comments { get; } = new List<string> { "DXF file generated by netDxf https://github.com/haplokuon/netDxf, Copyright(C) Daniel Carvajal, Licensed under MIT" };

		/// <summary>Gets the <b>DXF</b> <see cref="HeaderVariables">drawing variables</see>.</summary>
		public HeaderVariables DrawingVariables { get; }

		/// <summary>Gets or sets the name of the document.</summary>
		/// <remarks>
		/// When a file is loaded this field is equals the file name without extension.
		/// </remarks>
		public string Name { get; set; }

		#endregion

		#region public collection properties

		/// <summary>Gets the <see cref="ApplicationRegistries">application registries</see> collection.</summary>
		public ApplicationRegistries ApplicationRegistries { get; internal set; }

		/// <summary>Gets the <see cref="Layers">layers</see> collection.</summary>
		public Layers Layers { get; internal set; }

		/// <summary>Gets the <see cref="Linetypes">line types</see> collection.</summary>
		public Linetypes Linetypes { get; internal set; }

		/// <summary>Gets the <see cref="TextStyles">text styles</see> collection.</summary>
		public TextStyles TextStyles { get; internal set; }

		/// <summary>Gets the <see cref="ShapeStyles">shape styles</see> collection.</summary>
		/// <remarks>
		/// The <b>DXF</b> stores the <see cref="TextStyles"/> and <see cref="ShapeStyles"/> in the same table list, here, they are separated since they serve a different role.
		/// Under normal circumstances you should not need to access this list.
		/// </remarks>
		public ShapeStyles ShapeStyles { get; internal set; }

		/// <summary>Gets the <see cref="DimensionStyles">dimension styles</see> collection.</summary>
		public DimensionStyles DimensionStyles { get; internal set; }

		/// <summary>Gets the <see cref="MLineStyles">MLine styles</see> collection.</summary>
		public MLineStyles MlineStyles { get; internal set; }

		/// <summary>Gets the <see cref="UCSs">User coordinate systems</see> collection.</summary>
		public UCSs UCSs { get; internal set; }

		/// <summary>Gets the <see cref="BlockRecords">block</see> collection.</summary>
		public BlockRecords Blocks { get; internal set; }

		/// <summary>Gets the <see cref="ImageDefinitions">image definitions</see> collection.</summary>
		public ImageDefinitions ImageDefinitions { get; internal set; }

		/// <summary>Gets the <see cref="UnderlayDgnDefinitions">dgn underlay definitions</see> collection.</summary>
		public UnderlayDgnDefinitions UnderlayDgnDefinitions { get; internal set; }

		/// <summary>Gets the <see cref="UnderlayDwfDefinitions">dwf underlay definitions</see> collection.</summary>
		public UnderlayDwfDefinitions UnderlayDwfDefinitions { get; internal set; }

		/// <summary>Gets the <see cref="UnderlayPdfDefinitions">pdf underlay definitions</see> collection.</summary>
		public UnderlayPdfDefinitions UnderlayPdfDefinitions { get; internal set; }

		/// <summary>Gets the <see cref="Groups">groups</see> collection.</summary>
		public Groups Groups { get; internal set; }

		/// <summary>Gets the <see cref="Layouts">layouts</see> collection.</summary>
		public Layouts Layouts { get; internal set; }

		/// <summary>Gets the <see cref="VPorts">viewports</see> collection.</summary>
		public VPorts VPorts { get; internal set; }

		/// <summary>Gets the <see cref="Views">views</see> collection.</summary>
		internal Views Views { get; set; }

		/// <summary>Gets the <see cref="DrawingEntities">entities</see> shortcuts.</summary>
		public DrawingEntities Entities { get; }

		#endregion

		#endregion

		#region public methods

		/// <summary>Loads a <b>DXF</b> file.</summary>
		/// <param name="file">File name.</param>
		/// <returns>Returns a <see cref="DxfDocument"/>. It will return <see langword="null"/> if the file has not been able to load.</returns>
		/// <exception cref="DxfVersionNotSupportedException"></exception>
		/// <remarks>
		/// Loading <b>DXF</b> files prior to <b>AutoCAD</b> 2000 is not supported.<br />
		/// The Load method will still raise an exception if they are unable to create the <see cref="FileStream"/>.<br />
		/// On Debug mode it will raise any exception that might occur during the whole process.
		/// </remarks>
		public static DxfDocument Load(string file) => Load(file, new List<string>());
		/// <summary>Loads a <b>DXF</b> file.</summary>
		/// <param name="file">File name.</param>
		/// <param name="supportFolders">List of the document support folders.</param>
		/// <returns>Returns a <see cref="DxfDocument"/>. It will return <see langword="null"/> if the file has not been able to load.</returns>
		/// <exception cref="DxfVersionNotSupportedException"></exception>
		/// <remarks>
		/// Loading <b>DXF</b> files prior to <b>AutoCAD</b> 2000 is not supported.<br />
		/// The Load method will still raise an exception if they are unable to create the <see cref="FileStream"/>.<br />
		/// On Debug mode it will raise any exception that might occur during the whole process.
		/// </remarks>
		public static DxfDocument Load(string file, IEnumerable<string> supportFolders)
		{
			Stream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

			FileInfo fileInfo = new FileInfo(file);
			SupportFolders folders = new SupportFolders(supportFolders) { WorkingFolder = fileInfo.DirectoryName };

			DxfReader dxfReader = new DxfReader();

#if DEBUG
			DxfDocument document = dxfReader.Read(stream, folders);
			stream.Close();
#else
			DxfDocument document;
			try
			{
				document = dxfReader.Read(stream, folders);
			}
			catch (DxfVersionNotSupportedException)
			{
				throw;
			}
			catch
			{
				return null;
			}
			finally
			{
				stream.Close();
			}

#endif
			document.Name = Path.GetFileNameWithoutExtension(file);
			return document;
		}
		/// <summary>Loads a <b>DXF</b> file.</summary>
		/// <param name="stream">Stream.</param>
		/// <returns>Returns a <see cref="DxfDocument"/>. It will return <see langword="null"/> if the file has not been able to load.</returns>
		/// <exception cref="DxfVersionNotSupportedException"></exception>
		/// <remarks>
		/// Loading <b>DXF</b> files prior to <b>AutoCAD</b> 2000 is not supported.<br />
		/// On Debug mode it will raise any exception that might occur during the whole process.<br />
		/// The caller will be responsible of closing the stream.
		/// </remarks>
		public static DxfDocument Load(Stream stream) => Load(stream, new List<string>());
		/// <summary>Loads a <b>DXF</b> file.</summary>
		/// <param name="stream">Stream.</param>
		/// <param name="supportFolders">List of the document support folders.</param>
		/// <returns>Returns a <see cref="DxfDocument"/>. It will return <see langword="null"/> if the file has not been able to load.</returns>
		/// <exception cref="DxfVersionNotSupportedException"></exception>
		/// <remarks>
		/// Loading <b>DXF</b> files prior to <b>AutoCAD</b> 2000 is not supported.<br />
		/// On Debug mode it will raise any exception that might occur during the whole process.<br />
		/// The caller will be responsible of closing the stream.
		/// </remarks>
		public static DxfDocument Load(Stream stream, IEnumerable<string> supportFolders)
		{
			DxfReader dxfReader = new DxfReader();

#if DEBUG
			DxfDocument document = dxfReader.Read(stream, new SupportFolders(supportFolders));
#else
			DxfDocument document;
			try
			{
				 document = dxfReader.Read(stream, new SupportFolders(supportFolders));
			}
			catch (DxfVersionNotSupportedException)
			{
				throw;
			}
			catch
			{
				return null;
			}

#endif
			return document;
		}

		/// <summary>Saves the database of the actual <see cref="DxfDocument"/> to a text <b>DXF</b> file.</summary>
		/// <param name="file">File name.</param>
		/// <returns>Return <see langword="true"/> if the file has been successfully save; otherwise, <see langword="false"/>.</returns>
		/// <exception cref="DxfVersionNotSupportedException"></exception>
		/// <remarks>
		/// If the file already exists it will be overwritten.<br />
		/// The Save method will still raise an exception if they are unable to create the <see cref="FileStream"/>.<br />
		/// On Debug mode they will raise any exception that might occur during the whole process.
		/// </remarks>
		public bool Save(string file) => this.Save(file, false);
		/// <summary>Saves the database of the actual <see cref="DxfDocument"/> to a <b>DXF</b> file.</summary>
		/// <param name="file">File name.</param>
		/// <param name="isBinary">Defines if the file will be saved as binary.</param>
		/// <returns>Returns <see langword="true"/> if the file has been successfully saved; otherwise, <see langword="false"/>.</returns>
		/// <exception cref="DxfVersionNotSupportedException"></exception>
		/// <remarks>
		/// If the file already exists it will be overwritten.<br />
		/// The Save method will still raise an exception if they are unable to create the <see cref="FileStream"/>.<br />
		/// On Debug mode they will raise any exception that might occur during the whole process.
		/// </remarks>
		public bool Save(string file, bool isBinary)
		{
			FileInfo fileInfo = new FileInfo(file);
			this.Name = Path.GetFileNameWithoutExtension(fileInfo.FullName);


			DxfWriter dxfWriter = new DxfWriter();

			Stream stream = File.Create(file);

			string workingFolder = fileInfo.DirectoryName;
			if (!string.IsNullOrEmpty(workingFolder))
			{
				this.SupportFolders.WorkingFolder = workingFolder;
			}

#if DEBUG
			dxfWriter.Write(stream, this, isBinary);
			stream.Close();
#else
			try
			{
				dxfWriter.Write(stream, this, isBinary);
			}
			catch (DxfVersionNotSupportedException)
			{
				throw;
			}
			catch
			{
				return false;
			}
			finally
			{
				stream.Close();
			}

#endif
			return true;
		}
		/// <summary>Saves the database of the actual <see cref="DxfDocument"/> to a text stream.</summary>
		/// <param name="stream">Stream.</param>
		/// <returns>Return <see langword="true"/> if the stream has been successfully saved; otherwise, <see langword="false"/>.</returns>
		/// <exception cref="DxfVersionNotSupportedException"></exception>
		/// <remarks>
		/// On Debug mode it will raise any exception that might occur during the whole process.<br />
		/// The caller will be responsible of closing the stream.
		/// </remarks>
		public bool Save(Stream stream) => this.Save(stream, false);
		/// <summary>Saves the database of the actual <see cref="DxfDocument"/> to a stream.</summary>
		/// <param name="stream">Stream.</param>
		/// <param name="isBinary">Defines if the file will be saved as binary.</param>
		/// <returns>Return <see langword="true"/> if the stream has been successfully saved; otherwise, <see langword="false"/>.</returns>
		/// <exception cref="DxfVersionNotSupportedException"></exception>
		/// <remarks>
		/// On Debug mode it will raise any exception that might occur during the whole process.<br />
		/// The caller will be responsible of closing the stream.
		/// </remarks>
		public bool Save(Stream stream, bool isBinary)
		{
			DxfWriter dxfWriter = new DxfWriter();

#if DEBUG
			dxfWriter.Write(stream, this, isBinary);
#else
			try
			{
				dxfWriter.Write(stream, this, isBinary);
			}
			catch (DxfVersionNotSupportedException)
			{
				throw;
			}
			catch
			{
				return false;
			}

#endif
			return true;
		}

		/// <summary>Checks the <b>AutoCAD</b> <b>DXF</b> file database version.</summary>
		/// <param name="file">File name.</param>
		/// <returns>String that represents the <b>DXF</b> file version.</returns>
		public static DxfVersion CheckDxfFileVersion(string file) => CheckDxfFileVersion(file, out bool _);

		/// <summary>Checks the <b>AutoCAD</b> <b>DXF</b> file database version.</summary>
		/// <param name="file">File name.</param>
		/// <param name="isBinary">Returns <see langword="true"/> if the <b>DXF</b> is a binary file.</param>
		/// <returns>String that represents the <b>DXF</b> file version.</returns>
		public static DxfVersion CheckDxfFileVersion(string file, out bool isBinary)
		{
			Stream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			DxfVersion version = CheckDxfFileVersion(stream, out isBinary);
			stream.Close();
			return version;
		}

		/// <summary>Checks the <b>AutoCAD</b> <b>DXF</b> file database version.</summary>
		/// <param name="stream">Stream</param>
		/// <returns>String that represents the <b>DXF</b> file version.</returns>
		/// <remarks>The caller will be responsible of closing the stream.</remarks>
		public static DxfVersion CheckDxfFileVersion(Stream stream) => CheckDxfFileVersion(stream, out bool _);

		/// <summary>Checks the <b>AutoCAD</b> <b>DXF</b> file database version.</summary>
		/// <param name="stream">Stream</param>
		/// <param name="isBinary">Returns <see langword="true"/> if the <b>DXF</b> is a binary file.</param>
		/// <returns>String that represents the <b>DXF</b> file version.</returns>
		/// <remarks>The caller will be responsible of closing the stream.</remarks>
		public static DxfVersion CheckDxfFileVersion(Stream stream, out bool isBinary)
		{
			string value;
			isBinary = false;

			try
			{
				value = DxfReader.CheckHeaderVariable(stream, HeaderVariableCode.AcadVer, out isBinary);
			}
			catch
			{
				return DxfVersion.Unknown;
			}

			return StringEnum<DxfVersion>.Parse(value, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>Gets a <b>DXF</b> object by its handle.</summary>
		/// <param name="objectHandle">DxfObject handle.</param>
		/// <returns>The DxfObject that has the provided handle, <see langword="null"/> otherwise.</returns>
		public DxfObject GetObjectByHandle(string objectHandle)
		{
			if (string.IsNullOrEmpty(objectHandle))
			{
				return null;
			}

			this.AddedObjects.TryGetValue(objectHandle, out DxfObject o);
			return o;
		}

		#endregion

		#region internal methods

		internal void AddEntityToDocument(EntityObject entity, bool assignHandle)
		{
			// null entities are not allowed
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			// assign a handle
			if (assignHandle || string.IsNullOrEmpty(entity.Handle))
			{
				this.NumHandles = entity.AssignHandle(this.NumHandles);
			}

			// the entities that are part of a block do not belong to any of the entities lists but to the block definition.
			switch (entity.Type)
			{
				case EntityType.Arc:
					break;
				case EntityType.Circle:
					break;
				case EntityType.Dimension:
					Dimension dim = (Dimension)entity;
					dim.Style = this.DimensionStyles.Add(dim.Style, assignHandle);
					this.DimensionStyles.References[dim.Style.Name].Add(dim);
					this.AddDimensionStyleOverridesReferencedDxfObjects(dim, dim.StyleOverrides, assignHandle);
					if (this.BuildDimensionBlocks)
					{
						Block dimBlock = DimensionBlock.Build(dim, "DimBlock");
						dimBlock.SetName("*D" + ++this.DimensionBlocksIndex, false);
						dim.Block = this.Blocks.Add(dimBlock);
						this.Blocks.References[dimBlock.Name].Add(dim);
					}
					else if (dim.Block != null)
					{
						if (!this.Blocks.Contains(dim.Block) || !dim.Block.Name.StartsWith("*D", StringComparison.InvariantCultureIgnoreCase))
						{
							// if a block is not present or have a wrong name give it a proper name
							dim.Block.SetName("*D" + ++this.DimensionBlocksIndex, false);
						}
						//dim.Block.SetName("*D" + ++this.DimensionBlocksIndex, false);
						dim.Block = this.Blocks.Add(dim.Block);
						this.Blocks.References[dim.Block.Name].Add(dim);
					}
					dim.DimensionStyleChanged += this.Dimension_DimStyleChanged;
					dim.DimensionBlockChanged += this.Dimension_DimBlockChanged;
					dim.DimensionStyleOverrideAdded += this.Dimension_DimStyleOverrideAdded;
					dim.DimensionStyleOverrideRemoved += this.Dimension_DimStyleOverrideRemoved;
					break;
				case EntityType.Leader:
					Leader leader = (Leader)entity;
					leader.Style = this.DimensionStyles.Add(leader.Style, assignHandle);
					this.DimensionStyles.References[leader.Style.Name].Add(leader);
					leader.LeaderStyleChanged += this.Leader_DimStyleChanged;
					this.AddDimensionStyleOverridesReferencedDxfObjects(leader, leader.StyleOverrides, assignHandle);
					leader.DimensionStyleOverrideAdded += this.Leader_DimStyleOverrideAdded;
					leader.DimensionStyleOverrideRemoved += this.Leader_DimStyleOverrideRemoved;
					leader.AnnotationAdded += this.Leader_AnnotationAdded;
					leader.AnnotationRemoved += this.Leader_AnnotationRemoved;
					break;
				case EntityType.Tolerance:
					Tolerance tol = (Tolerance)entity;
					tol.Style = this.DimensionStyles.Add(tol.Style, assignHandle);
					this.DimensionStyles.References[tol.Style.Name].Add(tol);
					tol.ToleranceStyleChanged += this.Tolerance_DimStyleChanged;
					break;
				case EntityType.Ellipse:
					break;
				case EntityType.Face3D:
					break;
				case EntityType.Hatch:
					Hatch hatch = (Hatch)entity;
					hatch.HatchBoundaryPathAdded += this.Hatch_BoundaryPathAdded;
					hatch.HatchBoundaryPathRemoved += this.Hatch_BoundaryPathRemoved;
					break;
				case EntityType.Insert:
					Insert insert = (Insert)entity;
					insert.Block = this.Blocks.Add(insert.Block, assignHandle);
					this.Blocks.References[insert.Block.Name].Add(insert);
					//DrawingUnits insUnits = this.DrawingVariables.InsUnits;
					//double docScale = UnitHelper.ConversionFactor(insert.Block.Record.Units, insUnits);
					foreach (Attribute attribute in insert.Attributes)
					{
						//if (assignHandle && attribute.Definition != null)
						//{
						//	attribute.Height = docScale * attribute.Definition.Height;
						//	attribute.Position = docScale * attribute.Definition.Position + insert.Position - insert.Block.Origin;
						//}

						attribute.Layer = this.Layers.Add(attribute.Layer, assignHandle);
						this.Layers.References[attribute.Layer.Name].Add(attribute);
						attribute.LayerChanged += this.Entity_LayerChanged;

						attribute.Linetype = this.Linetypes.Add(attribute.Linetype, assignHandle);
						this.Linetypes.References[attribute.Linetype.Name].Add(attribute);
						attribute.LinetypeChanged += this.Entity_LinetypeChanged;

						attribute.Style = this.TextStyles.Add(attribute.Style, assignHandle);
						this.TextStyles.References[attribute.Style.Name].Add(attribute);
						attribute.TextStyleChanged += this.Entity_TextStyleChanged;
					}
					insert.AttributeAdded += this.Insert_AttributeAdded;
					insert.AttributeRemoved += this.Insert_AttributeRemoved;
					//insert.BlockChanged += this.Insert_BlockChanged;
					break;
				case EntityType.Line:
					break;
				case EntityType.Shape:
					Shape shape = (Shape)entity;
					shape.Style = this.ShapeStyles.Add(shape.Style, assignHandle);
					this.ShapeStyles.References[shape.Style.Name].Add(shape);
					//TODO: shape names and indexes, require check to external SHX file
					//if (!shape.Style.ContainsShapeName(shape.Name))
					//{
					//	throw new ArgumentException("The shape name " + shape.Name + " is not defined in the associated shape style " + shape.Style.Name + ".");
					//}
					shape.StyleChanged += this.Shape_StyleChanged;
					break;
				case EntityType.Point:
					break;
				case EntityType.PolyfaceMesh:
					PolyfaceMesh mesh = (PolyfaceMesh)entity;
					foreach (PolyfaceMeshFace face in mesh.Faces)
					{
						if (face.Layer != null)
						{
							face.Layer = this.Layers.Add(face.Layer, assignHandle);
							this.Layers.References[face.Layer.Name].Add(mesh);
						}
					}
					mesh.PolyfaceMeshFaceLayerChanged += this.Entity_LayerChanged;
					break;
				case EntityType.PolygonMesh:
					break;
				case EntityType.Polyline2D:
					break;
				case EntityType.Polyline3D:
					break;
				case EntityType.Solid:
					break;
				case EntityType.Spline:
					break;
				case EntityType.Trace:
					break;
				case EntityType.Mesh:
					break;
				case EntityType.Text:
					Text text = (Text)entity;
					text.Style = this.TextStyles.Add(text.Style, assignHandle);
					this.TextStyles.References[text.Style.Name].Add(text);
					text.TextStyleChanged += this.Entity_TextStyleChanged;
					break;
				case EntityType.MText:
					MText mText = (MText)entity;
					mText.Style = this.TextStyles.Add(mText.Style, assignHandle);
					this.TextStyles.References[mText.Style.Name].Add(mText);
					mText.TextStyleChanged += this.Entity_TextStyleChanged;
					break;
				case EntityType.Image:
					Image image = (Image)entity;
					image.Definition = this.ImageDefinitions.Add(image.Definition, assignHandle);
					this.ImageDefinitions.References[image.Definition.Name].Add(image);
					image.ImageDefinitionChanged += this.Image_ImageDefinitionChanged;
					break;
				case EntityType.MLine:
					MLine mline = (MLine)entity;
					mline.Style = this.MlineStyles.Add(mline.Style, assignHandle);
					this.MlineStyles.References[mline.Style.Name].Add(mline);
					mline.MLineStyleChanged += this.MLine_MLineStyleChanged;
					break;
				case EntityType.Ray:
					break;
				case EntityType.XLine:
					break;
				case EntityType.Underlay:
					Underlay underlay = (Underlay)entity;
					switch (underlay.Definition.Type)
					{
						case UnderlayType.DGN:
							underlay.Definition = this.UnderlayDgnDefinitions.Add((UnderlayDgnDefinition)underlay.Definition, assignHandle);
							this.UnderlayDgnDefinitions.References[underlay.Definition.Name].Add(underlay);
							break;
						case UnderlayType.DWF:
							underlay.Definition = this.UnderlayDwfDefinitions.Add((UnderlayDwfDefinition)underlay.Definition, assignHandle);
							this.UnderlayDwfDefinitions.References[underlay.Definition.Name].Add(underlay);
							break;
						case UnderlayType.PDF:
							underlay.Definition = this.UnderlayPdfDefinitions.Add((UnderlayPdfDefinition)underlay.Definition, assignHandle);
							this.UnderlayPdfDefinitions.References[underlay.Definition.Name].Add(underlay);
							break;
					}
					underlay.UnderlayDefinitionChanged += this.Underlay_UnderlayDefinitionChanged;
					break;
				case EntityType.Wipeout:
					break;
				case EntityType.Viewport:
					Viewport viewport = (Viewport)entity;
					for (int i = 0; i < viewport.FrozenLayers.Count; i++)
					{
						viewport.FrozenLayers[i] = this.Layers.Add(viewport.FrozenLayers[i], assignHandle);
					}
					viewport.ClippingBoundaryAdded += this.Viewport_ClippingBoundaryAdded;
					viewport.ClippingBoundaryRemoved += this.Viewport_ClippingBoundaryRemoved;
					break;
				default:
					throw new ArgumentException("The entity " + entity.Type + " is not implemented or unknown.");
			}

			entity.Layer = this.Layers.Add(entity.Layer, assignHandle);
			this.Layers.References[entity.Layer.Name].Add(entity);

			entity.Linetype = this.Linetypes.Add(entity.Linetype, assignHandle);
			this.Linetypes.References[entity.Linetype.Name].Add(entity);

			this.AddedObjects.Add(entity.Handle, entity);

			entity.LayerChanged += this.Entity_LayerChanged;
			entity.LinetypeChanged += this.Entity_LinetypeChanged;
		}

		internal void AddAttributeDefinitionToDocument(AttributeDefinition attDef, bool assignHandle)
		{
			// null entities are not allowed
			if (attDef == null)
			{
				throw new ArgumentNullException(nameof(attDef));
			}

			// assign a handle
			if (assignHandle || string.IsNullOrEmpty(attDef.Handle))
			{
				this.NumHandles = attDef.AssignHandle(this.NumHandles);
			}

			attDef.Style = this.TextStyles.Add(attDef.Style, assignHandle);
			this.TextStyles.References[attDef.Style.Name].Add(attDef);
			attDef.TextStyleChange += this.Entity_TextStyleChanged;

			attDef.Layer = this.Layers.Add(attDef.Layer, assignHandle);
			this.Layers.References[attDef.Layer.Name].Add(attDef);

			attDef.Linetype = this.Linetypes.Add(attDef.Linetype, assignHandle);
			this.Linetypes.References[attDef.Linetype.Name].Add(attDef);

			this.AddedObjects.Add(attDef.Handle, attDef);

			attDef.LayerChanged += this.Entity_LayerChanged;
			attDef.LinetypeChanged += this.Entity_LinetypeChanged;

		}

		internal bool RemoveEntityFromDocument(EntityObject entity)
		{
			// the entities that are part of a block do not belong to any of the entities lists but to the block definition
			// and they will not be removed from the drawing database
			switch (entity.Type)
			{
				case EntityType.Arc:
					break;
				case EntityType.Circle:
					break;
				case EntityType.Dimension:
					Dimension dim = (Dimension)entity;
					if (dim.Block != null)
					{
						this.Blocks.References[dim.Block.Name].Remove(entity);
						dim.Block = null;
					}

					dim.DimensionBlockChanged -= this.Dimension_DimBlockChanged;
					this.DimensionStyles.References[dim.Style.Name].Remove(entity);
					dim.DimensionStyleChanged -= this.Dimension_DimStyleChanged;

					this.RemoveDimensionStyleOverridesReferencedDxfObjects(dim, dim.StyleOverrides);
					dim.DimensionStyleOverrideAdded -= this.Dimension_DimStyleOverrideAdded;
					dim.DimensionStyleOverrideRemoved -= this.Dimension_DimStyleOverrideRemoved;
					break;
				case EntityType.Leader:
					Leader leader = (Leader)entity;
					this.DimensionStyles.References[leader.Style.Name].Remove(entity);
					leader.LeaderStyleChanged -= this.Leader_DimStyleChanged;
					if (leader.Annotation != null)
					{
						leader.Annotation.RemoveReactor(leader);
						this.Entities.Remove(leader.Annotation);
					}
					this.RemoveDimensionStyleOverridesReferencedDxfObjects(leader, leader.StyleOverrides);
					leader.DimensionStyleOverrideAdded -= this.Leader_DimStyleOverrideAdded;
					leader.DimensionStyleOverrideRemoved -= this.Leader_DimStyleOverrideRemoved;
					leader.AnnotationAdded -= this.Leader_AnnotationAdded;
					leader.AnnotationRemoved -= this.Leader_AnnotationRemoved;
					break;
				case EntityType.Tolerance:
					Tolerance tolerance = (Tolerance)entity;
					this.DimensionStyles.References[tolerance.Style.Name].Remove(entity);
					tolerance.ToleranceStyleChanged -= this.Tolerance_DimStyleChanged;
					break;
				case EntityType.Ellipse:
					break;
				case EntityType.Face3D:
					break;
				case EntityType.Spline:
					break;
				case EntityType.Hatch:
					Hatch hatch = (Hatch)entity;
					hatch.UnLinkBoundary(); // remove reactors, the entities that made the hatch boundary will not be automatically deleted
					hatch.HatchBoundaryPathAdded -= this.Hatch_BoundaryPathAdded;
					hatch.HatchBoundaryPathRemoved -= this.Hatch_BoundaryPathRemoved;
					break;
				case EntityType.Insert:
					Insert insert = (Insert)entity;
					this.Blocks.References[insert.Block.Name].Remove(entity);
					foreach (Attribute att in insert.Attributes)
					{
						this.Layers.References[att.Layer.Name].Remove(att);
						att.LayerChanged -= this.Entity_LayerChanged;
						this.Linetypes.References[att.Linetype.Name].Remove(att);
						att.LinetypeChanged -= this.Entity_LinetypeChanged;
						this.TextStyles.References[att.Style.Name].Remove(att);
						att.TextStyleChanged -= this.Entity_TextStyleChanged;
					}
					insert.AttributeAdded -= this.Insert_AttributeAdded;
					insert.AttributeRemoved -= this.Insert_AttributeRemoved;
					//insert.BlockChanged -= this.Insert_BlockChanged;
					break;
				case EntityType.Line:
					break;
				case EntityType.Shape:
					Shape shape = (Shape)entity;
					this.ShapeStyles.References[shape.Style.Name].Remove(entity);
					shape.StyleChanged -= this.Shape_StyleChanged;
					break;
				case EntityType.Point:
					break;
				case EntityType.PolyfaceMesh:
					PolyfaceMesh mesh = (PolyfaceMesh)entity;
					foreach (PolyfaceMeshFace face in mesh.Faces)
					{
						this.Layers.References[face.Layer.Name].Remove(mesh);
					}
					mesh.PolyfaceMeshFaceLayerChanged -= this.Entity_LayerChanged;
					break;
				case EntityType.PolygonMesh:
					break;
				case EntityType.Polyline2D:
					break;
				case EntityType.Polyline3D:
					break;
				case EntityType.Solid:
					break;
				case EntityType.Trace:
					break;
				case EntityType.Mesh:
					break;
				case EntityType.Text:
					Text text = (Text)entity;
					this.TextStyles.References[text.Style.Name].Remove(entity);
					text.TextStyleChanged -= this.Entity_TextStyleChanged;
					break;
				case EntityType.MText:
					MText mText = (MText)entity;
					this.TextStyles.References[mText.Style.Name].Remove(entity);
					mText.TextStyleChanged -= this.Entity_TextStyleChanged;
					break;
				case EntityType.Image:
					Image image = (Image)entity;
					this.ImageDefinitions.References[image.Definition.Name].Remove(image);
					image.ImageDefinitionChanged -= this.Image_ImageDefinitionChanged;
					break;
				case EntityType.MLine:
					MLine mline = (MLine)entity;
					this.MlineStyles.References[mline.Style.Name].Remove(entity);
					mline.MLineStyleChanged -= this.MLine_MLineStyleChanged;
					break;
				case EntityType.Ray:
					break;
				case EntityType.XLine:
					break;
				case EntityType.Underlay:
					Underlay underlay = (Underlay)entity;
					switch (underlay.Definition.Type)
					{
						case UnderlayType.DGN:
							this.UnderlayDgnDefinitions.References[underlay.Definition.Name].Remove(underlay);
							break;
						case UnderlayType.DWF:
							this.UnderlayDwfDefinitions.References[underlay.Definition.Name].Remove(underlay);
							break;
						case UnderlayType.PDF:
							this.UnderlayPdfDefinitions.References[underlay.Definition.Name].Remove(underlay);
							break;
					}
					underlay.UnderlayDefinitionChanged -= this.Underlay_UnderlayDefinitionChanged;
					break;
				case EntityType.Wipeout:
					break;
				case EntityType.Viewport:
					Viewport viewport = (Viewport)entity;
					// delete the viewport boundary entity in case there is one
					if (viewport.ClippingBoundary != null)
					{
						viewport.ClippingBoundary.RemoveReactor(viewport);
						this.Entities.Remove(viewport.ClippingBoundary);
					}
					viewport.ClippingBoundaryAdded -= this.Viewport_ClippingBoundaryAdded;
					viewport.ClippingBoundaryRemoved -= this.Viewport_ClippingBoundaryRemoved;

					break;
				default:
					throw new ArgumentException("The entity " + entity.Type + " is not implemented or unknown");
			}

			this.Layers.References[entity.Layer.Name].Remove(entity);
			this.Linetypes.References[entity.Linetype.Name].Remove(entity);
			this.AddedObjects.Remove(entity.Handle);

			entity.LayerChanged -= this.Entity_LayerChanged;
			entity.LinetypeChanged -= this.Entity_LinetypeChanged;

			entity.Handle = null;
			entity.Owner = null;

			return true;
		}

		internal bool RemoveAttributeDefinitionFromDocument(AttributeDefinition attDef)
		{
			this.TextStyles.References[attDef.Style.Name].Remove(attDef);
			attDef.TextStyleChange -= this.Entity_TextStyleChanged;

			this.Layers.References[attDef.Layer.Name].Remove(attDef);
			this.Linetypes.References[attDef.Linetype.Name].Remove(attDef);
			this.AddedObjects.Remove(attDef.Handle);

			attDef.LayerChanged -= this.Entity_LayerChanged;
			attDef.LinetypeChanged -= this.Entity_LinetypeChanged;

			attDef.Handle = null;
			attDef.Owner = null;

			return true;
		}

		#endregion

		#region private methods

		private void AddDimensionStyleOverridesReferencedDxfObjects(EntityObject entity, DimensionStyleOverrideDictionary overrides, bool assignHandle)
		{
			// add the style override referenced DxfObjects

			// add referenced text style
			if (overrides.TryGetValue(DimensionStyleOverrideType.TextStyle, out DimensionStyleOverride styleOverride))
			{
				TextStyle txtStyle = (TextStyle)styleOverride.Value;
				overrides[styleOverride.Type] = new DimensionStyleOverride(styleOverride.Type, this.TextStyles.Add(txtStyle, assignHandle));
				this.TextStyles.References[txtStyle.Name].Add(entity);
			}

			// add referenced blocks
			if (overrides.TryGetValue(DimensionStyleOverrideType.LeaderArrow, out styleOverride))
			{
				Block block = (Block)styleOverride.Value;
				if (block != null)
				{
					overrides[styleOverride.Type] = new DimensionStyleOverride(styleOverride.Type, this.Blocks.Add(block, assignHandle));
					this.Blocks.References[block.Name].Add(entity);
				}
			}

			if (overrides.TryGetValue(DimensionStyleOverrideType.DimArrow1, out styleOverride))
			{
				Block block = (Block)styleOverride.Value;
				if (block != null)
				{
					overrides[styleOverride.Type] = new DimensionStyleOverride(styleOverride.Type, this.Blocks.Add(block, assignHandle));
					this.Blocks.References[block.Name].Add(entity);
				}
			}

			if (overrides.TryGetValue(DimensionStyleOverrideType.DimArrow2, out styleOverride))
			{
				Block block = (Block)styleOverride.Value;
				if (block != null)
				{
					overrides[styleOverride.Type] = new DimensionStyleOverride(styleOverride.Type, this.Blocks.Add(block, assignHandle));
					this.Blocks.References[block.Name].Add(entity);
				}
			}

			// add referenced line types
			if (overrides.TryGetValue(DimensionStyleOverrideType.DimLineLinetype, out styleOverride))
			{
				Linetype linetype = (Linetype)styleOverride.Value;
				overrides[styleOverride.Type] = new DimensionStyleOverride(styleOverride.Type, this.Linetypes.Add(linetype, assignHandle));
				this.Linetypes.References[linetype.Name].Add(entity);
			}

			if (overrides.TryGetValue(DimensionStyleOverrideType.ExtLine1Linetype, out styleOverride))
			{
				Linetype linetype = (Linetype)styleOverride.Value;
				overrides[styleOverride.Type] = new DimensionStyleOverride(styleOverride.Type, this.Linetypes.Add(linetype, assignHandle));
				this.Linetypes.References[linetype.Name].Add(entity);
			}

			if (overrides.TryGetValue(DimensionStyleOverrideType.ExtLine2Linetype, out styleOverride))
			{
				Linetype linetype = (Linetype)styleOverride.Value;
				overrides[styleOverride.Type] = new DimensionStyleOverride(styleOverride.Type, this.Linetypes.Add(linetype, assignHandle));
				this.Linetypes.References[linetype.Name].Add(entity);
			}
		}

		private void RemoveDimensionStyleOverridesReferencedDxfObjects(EntityObject entity, DimensionStyleOverrideDictionary overrides)
		{
			// remove the style override referenced DxfObjects
			DimensionStyleOverride styleOverride;

			// remove referenced text style
			overrides.TryGetValue(DimensionStyleOverrideType.TextStyle, out styleOverride);
			if (styleOverride != null)
			{
				TextStyle txtStyle = (TextStyle)styleOverride.Value;
				this.TextStyles.References[txtStyle.Name].Remove(entity);
			}

			// remove referenced blocks
			overrides.TryGetValue(DimensionStyleOverrideType.LeaderArrow, out styleOverride);
			if (styleOverride != null)
			{
				Block block = (Block)styleOverride.Value;
				if (block != null)
				{
					this.Blocks.References[block.Name].Remove(entity);
				}
			}

			overrides.TryGetValue(DimensionStyleOverrideType.DimArrow1, out styleOverride);
			if (styleOverride != null)
			{
				Block block = (Block)styleOverride.Value;
				if (block != null)
				{
					this.Blocks.References[block.Name].Remove(entity);
				}
			}

			overrides.TryGetValue(DimensionStyleOverrideType.DimArrow2, out styleOverride);
			if (styleOverride != null)
			{
				Block block = (Block)styleOverride.Value;
				if (block != null)
				{
					this.Blocks.References[block.Name].Remove(entity);
				}
			}

			// remove referenced line types
			overrides.TryGetValue(DimensionStyleOverrideType.DimLineLinetype, out styleOverride);
			if (styleOverride != null)
			{
				Linetype linetype = (Linetype)styleOverride.Value;
				this.Linetypes.References[linetype.Name].Remove(entity);
			}

			overrides.TryGetValue(DimensionStyleOverrideType.ExtLine1Linetype, out styleOverride);
			if (styleOverride != null)
			{
				Linetype linetype = (Linetype)styleOverride.Value;
				this.Linetypes.References[linetype.Name].Remove(entity);
			}

			overrides.TryGetValue(DimensionStyleOverrideType.ExtLine2Linetype, out styleOverride);
			if (styleOverride != null)
			{
				Linetype linetype = (Linetype)styleOverride.Value;
				this.Linetypes.References[linetype.Name].Remove(entity);
			}
		}

		private void AddDefaultObjects()
		{
			// collections
			this.VPorts = new VPorts(this);
			this.Views = new Views(this);
			this.ApplicationRegistries = new ApplicationRegistries(this);
			this.Layers = new Layers(this);
			this.Linetypes = new Linetypes(this);
			this.TextStyles = new TextStyles(this);
			this.ShapeStyles = new ShapeStyles(this);
			this.DimensionStyles = new DimensionStyles(this);
			this.MlineStyles = new MLineStyles(this);
			this.UCSs = new UCSs(this);
			this.Blocks = new BlockRecords(this);
			this.ImageDefinitions = new ImageDefinitions(this);
			this.UnderlayDgnDefinitions = new UnderlayDgnDefinitions(this);
			this.UnderlayDwfDefinitions = new UnderlayDwfDefinitions(this);
			this.UnderlayPdfDefinitions = new UnderlayPdfDefinitions(this);
			this.Groups = new Groups(this);
			this.Layouts = new Layouts(this);

			//add default viewport (the active viewport is automatically added when the collection is created, is the only one supported)
			//this.vports.Add(VPort.Active);

			//add default layer
			this.Layers.Add(Layer.Default);

			// add default line types
			this.Linetypes.Add(Linetype.ByLayer);
			this.Linetypes.Add(Linetype.ByBlock);
			this.Linetypes.Add(Linetype.Continuous);

			// add default text style
			this.TextStyles.Add(TextStyle.Default);

			// add default application registry
			this.ApplicationRegistries.Add(ApplicationRegistry.Default);

			// add default dimension style
			this.DimensionStyles.Add(DimensionStyle.Default);

			// add default MLine style
			this.MlineStyles.Add(MLineStyle.Default);

			// add ModelSpace layout
			this.Layouts.Add(Layout.ModelSpace);

			// raster variables
			this.RasterVariables = new RasterVariables(this);
		}

		#endregion

		#region EntityObject events

		private void MLine_MLineStyleChanged(MLine sender, TableObjectChangedEventArgs<MLineStyle> e)
		{
			this.MlineStyles.References[e.OldValue.Name].Remove(sender);

			e.NewValue = this.MlineStyles.Add(e.NewValue);
			this.MlineStyles.References[e.NewValue.Name].Add(sender);
		}

		private void Dimension_DimStyleChanged(Dimension sender, TableObjectChangedEventArgs<DimensionStyle> e)
		{
			this.DimensionStyles.References[e.OldValue.Name].Remove(sender);

			e.NewValue = this.DimensionStyles.Add(e.NewValue);
			this.DimensionStyles.References[e.NewValue.Name].Add(sender);
		}

		private void Dimension_DimBlockChanged(Dimension sender, TableObjectChangedEventArgs<Block> e)
		{
			if (e.OldValue != null)
			{
				this.Blocks.References[e.OldValue.Name].Remove(sender);
				this.Blocks.Remove(e.OldValue);
			}

			if (e.NewValue != null)
			{
				if (!e.NewValue.Name.StartsWith("*D")) e.NewValue.SetName("*D" + ++this.DimensionBlocksIndex, false);
				e.NewValue = this.Blocks.Add(e.NewValue);
				this.Blocks.References[e.NewValue.Name].Add(sender);
			}
		}

		private void Dimension_DimStyleOverrideAdded(Dimension sender, DimensionStyleOverrideChangeEventArgs e)
		{
			switch (e.Item.Type)
			{
				case DimensionStyleOverrideType.DimLineLinetype:
				case DimensionStyleOverrideType.ExtLine1Linetype:
				case DimensionStyleOverrideType.ExtLine2Linetype:
					Linetype linetype = (Linetype)e.Item.Value;
					sender.StyleOverrides[e.Item.Type] = new DimensionStyleOverride(e.Item.Type, this.Linetypes.Add(linetype));
					this.Linetypes.References[linetype.Name].Add(sender);
					break;
				case DimensionStyleOverrideType.LeaderArrow:
				case DimensionStyleOverrideType.DimArrow1:
				case DimensionStyleOverrideType.DimArrow2:
					Block block = (Block)e.Item.Value;
					if (block == null)
						return; // the block might be defined as null to indicate that the default arrowhead will be used
					sender.StyleOverrides[e.Item.Type] = new DimensionStyleOverride(e.Item.Type, this.Blocks.Add(block));
					this.Blocks.References[block.Name].Add(sender);
					break;
				case DimensionStyleOverrideType.TextStyle:
					TextStyle style = (TextStyle)e.Item.Value;
					sender.StyleOverrides[e.Item.Type] = new DimensionStyleOverride(e.Item.Type, this.TextStyles.Add(style));
					this.TextStyles.References[style.Name].Add(sender);
					break;
			}
		}

		private void Dimension_DimStyleOverrideRemoved(Dimension sender, DimensionStyleOverrideChangeEventArgs e)
		{
			switch (e.Item.Type)
			{
				case DimensionStyleOverrideType.DimLineLinetype:
				case DimensionStyleOverrideType.ExtLine1Linetype:
				case DimensionStyleOverrideType.ExtLine2Linetype:
					Linetype linetype = (Linetype)e.Item.Value;
					this.Linetypes.References[linetype.Name].Remove(sender);
					break;
				case DimensionStyleOverrideType.LeaderArrow:
				case DimensionStyleOverrideType.DimArrow1:
				case DimensionStyleOverrideType.DimArrow2:
					Block block = (Block)e.Item.Value;
					if (block == null)
						return; // the block might be defined as null to indicate that the default arrowhead will be used
					this.Blocks.References[block.Name].Remove(sender);
					break;
				case DimensionStyleOverrideType.TextStyle:
					TextStyle style = (TextStyle)e.Item.Value;
					this.TextStyles.References[style.Name].Remove(sender);
					break;
			}
		}

		private void Leader_DimStyleChanged(Leader sender, TableObjectChangedEventArgs<DimensionStyle> e)
		{
			this.DimensionStyles.References[e.OldValue.Name].Remove(sender);

			e.NewValue = this.DimensionStyles.Add(e.NewValue);
			this.DimensionStyles.References[e.NewValue.Name].Add(sender);
		}

		private void Leader_DimStyleOverrideAdded(Leader sender, DimensionStyleOverrideChangeEventArgs e)
		{
			switch (e.Item.Type)
			{
				case DimensionStyleOverrideType.DimLineLinetype:
				case DimensionStyleOverrideType.ExtLine1Linetype:
				case DimensionStyleOverrideType.ExtLine2Linetype:
					Linetype linetype = (Linetype)e.Item.Value;
					sender.StyleOverrides[e.Item.Type] = new DimensionStyleOverride(e.Item.Type, this.Linetypes.Add(linetype));
					this.Linetypes.References[linetype.Name].Add(sender);
					break;
				case DimensionStyleOverrideType.LeaderArrow:
				case DimensionStyleOverrideType.DimArrow1:
				case DimensionStyleOverrideType.DimArrow2:
					Block block = (Block)e.Item.Value;
					if (block == null)
					{
						// the block might be defined as null to indicate that the default arrowhead will be used
						return;
					}
					sender.StyleOverrides[e.Item.Type] = new DimensionStyleOverride(e.Item.Type, this.Blocks.Add(block));
					this.Blocks.References[block.Name].Add(sender);
					break;
				case DimensionStyleOverrideType.TextStyle:
					TextStyle style = (TextStyle)e.Item.Value;
					sender.StyleOverrides[e.Item.Type] = new DimensionStyleOverride(e.Item.Type, this.TextStyles.Add(style));
					this.TextStyles.References[style.Name].Add(sender);
					break;
			}
		}

		private void Leader_DimStyleOverrideRemoved(Leader sender, DimensionStyleOverrideChangeEventArgs e)
		{
			switch (e.Item.Type)
			{
				case DimensionStyleOverrideType.DimLineLinetype:
				case DimensionStyleOverrideType.ExtLine1Linetype:
				case DimensionStyleOverrideType.ExtLine2Linetype:
					Linetype linetype = (Linetype)e.Item.Value;
					this.Linetypes.References[linetype.Name].Remove(sender);
					break;
				case DimensionStyleOverrideType.LeaderArrow:
				case DimensionStyleOverrideType.DimArrow1:
				case DimensionStyleOverrideType.DimArrow2:
					Block block = (Block)e.Item.Value;
					if (block == null)
						return; // the block might be defined as null to indicate that the default arrowhead will be used
					this.Blocks.References[block.Name].Remove(sender);
					break;
				case DimensionStyleOverrideType.TextStyle:
					TextStyle style = (TextStyle)e.Item.Value;
					this.TextStyles.References[style.Name].Remove(sender);
					break;
			}
		}

		private void Leader_AnnotationAdded(Leader sender, EntityChangeEventArgs e)
		{
			Layout layout = sender.Owner.Record.Layout;
			// the viewport belongs to a layout
			if (e.Item.Owner != null)
			{
				// the viewport clipping boundary and its entities must belong to the same document or block
				if (!ReferenceEquals(e.Item.Owner.Record.Layout, layout))
					throw new ArgumentException("The leader annotation entity and the Leader entity must belong to the same layout and document. Clone it instead.");
				// there is no need to do anything else we will not add the same entity twice
			}
			else
			{
				// we will add the new entity to the same document and layout of the hatch
				this.Blocks[layout.AssociatedBlock.Name].Entities.Add(e.Item);
			}
		}

		private void Leader_AnnotationRemoved(Leader sender, EntityChangeEventArgs e) => this.Entities.Remove(e.Item);

		private void Tolerance_DimStyleChanged(Tolerance sender, TableObjectChangedEventArgs<DimensionStyle> e)
		{
			if (e.OldValue != null)
			{
				this.DimensionStyles.References[e.OldValue.Name].Remove(sender);
			}
			if (e.NewValue == null)
			{
				return;
			}
			e.NewValue = this.DimensionStyles.Add(e.NewValue);
			this.DimensionStyles.References[e.NewValue.Name].Add(sender);
		}

		private void Entity_TextStyleChanged(DxfObject sender, TableObjectChangedEventArgs<TextStyle> e)
		{
			if (e.OldValue != null)
			{
				this.TextStyles.References[e.OldValue.Name].Remove(sender);
			}
			if (e.NewValue == null)
			{
				return;
			}
			e.NewValue = this.TextStyles.Add(e.NewValue);
			this.TextStyles.References[e.NewValue.Name].Add(sender);
		}

		private void Entity_LinetypeChanged(DxfObject sender, TableObjectChangedEventArgs<Linetype> e)
		{
			if (e.OldValue != null)
			{
				this.Linetypes.References[e.OldValue.Name].Remove(sender);
			}
			if (e.NewValue == null)
			{
				return;
			}
			e.NewValue = this.Linetypes.Add(e.NewValue);
			this.Linetypes.References[e.NewValue.Name].Add(sender);
		}

		private void Entity_LayerChanged(DxfObject sender, TableObjectChangedEventArgs<Layer> e)
		{
			if (e.OldValue != null)
			{
				this.Layers.References[e.OldValue.Name].Remove(sender);
			}
			if (e.NewValue == null)
			{
				return;
			}
			e.NewValue = this.Layers.Add(e.NewValue);
			this.Layers.References[e.NewValue.Name].Add(sender);
		}

		private void Insert_AttributeAdded(Insert sender, AttributeChangeEventArgs e)
		{
			this.NumHandles = e.Item.AssignHandle(this.NumHandles);

			e.Item.Layer = this.Layers.Add(e.Item.Layer);
			this.Layers.References[e.Item.Layer.Name].Add(e.Item);
			e.Item.LayerChanged += this.Entity_LayerChanged;

			e.Item.Linetype = this.Linetypes.Add(e.Item.Linetype);
			this.Linetypes.References[e.Item.Linetype.Name].Add(e.Item);
			e.Item.LinetypeChanged += this.Entity_LinetypeChanged;

			e.Item.Style = this.TextStyles.Add(e.Item.Style);
			this.TextStyles.References[e.Item.Style.Name].Add(e.Item);
			e.Item.TextStyleChanged += this.Entity_TextStyleChanged;
		}

		private void Insert_AttributeRemoved(Insert sender, AttributeChangeEventArgs e)
		{
			this.Layers.References[e.Item.Layer.Name].Remove(e.Item);
			e.Item.LayerChanged -= this.Entity_LayerChanged;

			this.Linetypes.References[e.Item.Linetype.Name].Remove(e.Item);
			e.Item.LinetypeChanged -= this.Entity_LinetypeChanged;

			this.TextStyles.References[e.Item.Style.Name].Remove(e.Item);
			e.Item.TextStyleChanged -= this.Entity_TextStyleChanged;
		}

		//private void Insert_BlockChanged(Insert sender, TableObjectChangedEventArgs<Block> e)
		//{
		//	this.Blocks.References[e.OldValue.Name].Remove(sender);

		//	e.NewValue = this.Blocks.Add(e.NewValue);
		//	this.Blocks.References[e.NewValue.Name].Add(sender);
		//}

		private void Hatch_BoundaryPathAdded(Hatch sender, ObservableCollectionEventArgs<HatchBoundaryPath> e)
		{
			Layout layout = sender.Owner.Record.Layout;
			foreach (EntityObject entity in e.Item.Entities)
			{
				// the hatch belongs to a layout
				if (entity.Owner != null)
				{
					// the hatch and its entities must belong to the same document or block
					if (!ReferenceEquals(entity.Owner.Record.Layout, layout))
					{
						throw new ArgumentException("The HatchBoundaryPath entity and the Hatch entity must belong to the same layout and document. Clone it instead.");
					}
					// there is no need to do anything else we will not add the same entity twice
				}
				else
				{
					// we will add the new entity to the same document and layout of the hatch
					this.Blocks[layout.AssociatedBlock.Name].Entities.Add(entity);
				}
			}
		}

		private void Hatch_BoundaryPathRemoved(Hatch sender, ObservableCollectionEventArgs<HatchBoundaryPath> e)
		{
			foreach (EntityObject entity in e.Item.Entities)
			{
				this.Entities.Remove(entity);
			}
		}

		private void Viewport_ClippingBoundaryAdded(Viewport sender, EntityChangeEventArgs e)
		{
			Layout layout = sender.Owner.Record.Layout;
			// the viewport belongs to a layout
			if (e.Item.Owner != null)
			{
				// the viewport clipping boundary and its entities must belong to the same document or block
				if (!ReferenceEquals(e.Item.Owner.Record.Layout, layout))
				{
					throw new ArgumentException("The viewport clipping boundary entity and the Viewport entity must belong to the same layout and document. Clone it instead.");
				}
				// there is no need to do anything else we will not add the same entity twice
			}
			else
			{
				// we will add the new entity to the same document and layout of the hatch
				this.Blocks[layout.AssociatedBlock.Name].Entities.Add(e.Item);
			}
		}

		private void Viewport_ClippingBoundaryRemoved(Viewport sender, EntityChangeEventArgs e) => this.Entities.Remove(e.Item);

		private void Image_ImageDefinitionChanged(Image sender, TableObjectChangedEventArgs<ImageDefinition> e)
		{
			this.ImageDefinitions.References[e.OldValue.Name].Remove(sender);

			e.NewValue = this.ImageDefinitions.Add(e.NewValue);
			this.ImageDefinitions.References[e.OldValue.Name].Add(sender);
		}

		private void Underlay_UnderlayDefinitionChanged(Underlay sender, TableObjectChangedEventArgs<UnderlayDefinition> e)
		{
			switch (e.OldValue.Type)
			{
				case UnderlayType.DGN:
					this.UnderlayDgnDefinitions.References[e.OldValue.Name].Remove(sender);
					break;
				case UnderlayType.DWF:
					this.UnderlayDwfDefinitions.References[e.OldValue.Name].Remove(sender);
					break;
				case UnderlayType.PDF:
					this.UnderlayPdfDefinitions.References[e.OldValue.Name].Remove(sender);
					break;
			}


			switch (e.NewValue.Type)
			{
				case UnderlayType.DGN:
					e.NewValue = this.UnderlayDgnDefinitions.Add((UnderlayDgnDefinition)e.NewValue);
					this.UnderlayDgnDefinitions.References[e.NewValue.Name].Add(sender);
					break;
				case UnderlayType.DWF:
					e.NewValue = this.UnderlayDwfDefinitions.Add((UnderlayDwfDefinition)e.NewValue);
					this.UnderlayDwfDefinitions.References[e.NewValue.Name].Add(sender);
					break;
				case UnderlayType.PDF:
					e.NewValue = this.UnderlayPdfDefinitions.Add((UnderlayPdfDefinition)e.NewValue);
					this.UnderlayPdfDefinitions.References[e.NewValue.Name].Add(sender);
					break;
			}
		}

		private void Shape_StyleChanged(Shape sender, TableObjectChangedEventArgs<ShapeStyle> e)
		{
			this.ShapeStyles.References[e.OldValue.Name].Remove(sender);

			e.NewValue = this.ShapeStyles.Add(e.NewValue);
			this.ShapeStyles.References[e.NewValue.Name].Add(sender);
		}

		#endregion

		#region DxfObject events

		private void AddedObjects_BeforeAddItem(ObservableDictionary<string, DxfObject> sender, ObservableDictionaryEventArgs<string, DxfObject> e)
		{
		}

		private void AddedObjects_AddItem(ObservableDictionary<string, DxfObject> sender, ObservableDictionaryEventArgs<string, DxfObject> e)
		{
			DxfObject o = e.Item.Value;
			if (o != null)
			{
				foreach (string appReg in o.XData.AppIds)
				{
					o.XData[appReg].ApplicationRegistry = this.ApplicationRegistries.Add(o.XData[appReg].ApplicationRegistry);
					this.ApplicationRegistries.References[appReg].Add(e.Item.Value);
				}

				o.XDataAddAppReg += this.DxfObject_XDataAddAppReg;
				o.XDataRemoveAppReg += this.DxfObject_XDataRemoveAppReg;
			}
		}

		private void AddedObjects_BeforeRemoveItem(ObservableDictionary<string, DxfObject> sender, ObservableDictionaryEventArgs<string, DxfObject> e)
		{
		}

		private void AddedObjects_RemoveItem(ObservableDictionary<string, DxfObject> sender, ObservableDictionaryEventArgs<string, DxfObject> e)
		{
			DxfObject o = e.Item.Value;
			if (o != null)
			{
				foreach (string appReg in o.XData.AppIds)
				{
					this.ApplicationRegistries.References[appReg].Remove(e.Item.Value);
				}
				o.XDataAddAppReg -= this.DxfObject_XDataAddAppReg;
				o.XDataRemoveAppReg -= this.DxfObject_XDataRemoveAppReg;
			}
		}

		private void DxfObject_XDataAddAppReg(DxfObject sender, ObservableCollectionEventArgs<ApplicationRegistry> e)
		{
			sender.XData[e.Item.Name].ApplicationRegistry = this.ApplicationRegistries.Add(sender.XData[e.Item.Name].ApplicationRegistry);
			this.ApplicationRegistries.References[e.Item.Name].Add(sender);
		}

		private void DxfObject_XDataRemoveAppReg(DxfObject sender, ObservableCollectionEventArgs<ApplicationRegistry> e)
			=> this.ApplicationRegistries.References[e.Item.Name].Remove(sender);

		#endregion
	}
}