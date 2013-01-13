using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using DataDynamics.PageFX.Common.IO;

namespace DataDynamics.PageFX.Ecma335.Metadata
{
	//ref: Partition II, 24.2.1 Metadata root

	/// <summary>
	/// Metadata reader.
	/// </summary>
	internal sealed class MetadataReader : IDisposable
	{
		private readonly BufferedBinaryReader _reader;
		private readonly Image _image = new Image();

		private StringHeap _strings;
		private UserStringHeap _userStrings;
		private GuidHeap _guids;
		private BlobHeap _blob;

		private byte _heapSizes;
		private MetadataTable[] _tables;
		internal const int MaxTableCount = 64;

		public MetadataReader(string path)
		{
			_reader = new BufferedBinaryReader(path);
			Load();
		}

		public MetadataReader(Stream s)
		{
			_reader = new BufferedBinaryReader(s);
			Load();
		}

		public void Dispose()
		{
		}

		#region Public Members

		private int _resourceOrigin = -1;

		public BufferedBinaryReader SeekResourceOffset(int offset)
		{
			if (_resourceOrigin < 0)
			{
				int pos = _image.ResolveVirtualAddress(_image.Resources.VirtualAddress);
				_resourceOrigin = pos;
			}
			if (offset >= _image.Resources.Size)
				throw new BadMetadataException();
			_reader.Position = _resourceOrigin + offset;
			return _reader;
		}

		public int SizeOfResources
		{
			get { return (int)_image.Resources.Size; }
		}

		public SimpleIndex EntryPointToken
		{
			get { return _image.EntryPointToken; }
		}

		public BufferedBinaryReader MoveToVirtualAddress(uint rva)
		{
			_reader.Position = _image.ResolveVirtualAddress(rva);
			return _reader;
		}

		public MetadataTable GetTable(TableId tableId)
		{
			int i = (int)tableId;
			if (i < 0 || i >= _tables.Length)
				throw new ArgumentOutOfRangeException("tableId");
			return _tables[i];
		}

		public IEnumerable<MetadataRow> GetRows(TableId tableId)
		{
			int n = GetRowCount(tableId);
			for (int i = 0; i < n; ++i)
				yield return GetRow(tableId, i);
		}

		public int GetRowCount(TableId id)
		{
			var table = GetTable(id);
			return table == null ? 0 : table.RowCount;
		}

		public MetadataRow GetRow(TableId tableId, int rowIndex)
		{
			var table = GetTable(tableId);
			if (table == null || table.RowCount <= 0) return null;
			
			var offset = table.Offset + rowIndex * table.RowSize;
			var reader = _reader.Slice(offset, table.RowSize);

			return new MetadataRow(this, table, reader, rowIndex);
		}

		#endregion

		private static int SizeOfIndex(int n)
		{
			return n >= 0x10000 ? 4 : 2;
		}

		internal int GetSimpleIndexSize(TableId id)
		{
			int n = GetRowCount(id);
			return SizeOfIndex(n);
		}

		private int[] _codedIndexSizes;

		internal int GetCodedIndexSize(CodedIndex codedIndex)
		{
			if (_codedIndexSizes == null)
			{
				_codedIndexSizes = CodedIndex.All.Select(x => -1).ToArray();
			}

			var i = codedIndex.Id;
			if (_codedIndexSizes[i] == -1)
			{
				int maxRowCount = codedIndex.Tables.Select(id => GetRowCount(id)).Concat(new[] {0}).Max();
				int count = maxRowCount << codedIndex.Bits;
				int size = SizeOfIndex(count);
				_codedIndexSizes[i] = size;
			}

			return _codedIndexSizes[i];
		}

		private int GetColumnSize(MetadataColumn c)
		{
			switch (c.Type)
			{
				case ColumnType.Int16: return 2;
				case ColumnType.Int32: return 4;
				case ColumnType.StringIndex: return StringIndexSize;
				case ColumnType.BlobIndex: return BlobIndexSize;
				case ColumnType.GuidIndex: return GuidIndexSize;
				case ColumnType.SimpleIndex: return GetSimpleIndexSize(c.SimpleIndex);
				case ColumnType.CodedIndex: return GetCodedIndexSize(c.CodedIndex);
				default: throw new ArgumentOutOfRangeException();
			}
		}

		private void Load()
		{
			_image.Load(_reader);

			uint mdbOffset = (uint)_reader.Position;

			_reader.Position = mdbOffset;

			// Metadata Header
			// Signature
			if (_reader.ReadUInt32() != 0x424A5342)
			{
				throw new BadMetadataException("Invalid metadata header.");
			}

			// MajorVersion			2
			// MinorVersion			2
			// Reserved				4
			_reader.Advance(8);

			var runtimeVersion = _reader.ReadZeroTerminatedString(_reader.ReadInt32());

			// align for dword boundary
			_reader.Align4();

			// Flags		2
			_reader.Advance(2);

			LoadHeaps(mdbOffset);
		}

		private void LoadHeaps(uint startOffset)
		{
			// heap headers
			int n = _reader.ReadUInt16();
			var headers = Enumerable
				.Range(0, n)
				.Select(x =>
				        new
					        {
						        Offset = startOffset + _reader.ReadUInt32(),
						        Size = _reader.ReadUInt32(),
						        Name = _reader.ReadAlignedString(16)
					        })
				.ToList();

			var creators = new Dictionary<string, Action<long, long>>
				{
					{"#-", CreateTables},
					{"#~", CreateTables},
					{"#Strings", CreateStringHeap},
					{"#US", CreateUserStringHeap},
					{"#GUID", CreateGuidHeap},
					{"#Blob", CreateBlobHeap}
				};

			headers.ForEach(header =>
				{
					Action<long, long> create;
					if (!creators.TryGetValue(header.Name, out create))
						throw new BadMetadataException(string.Format("Unknown meta-data stream '{0}'.", header.Name));
					create(header.Offset, header.Size);
				});
		}

		internal int StringIndexSize
		{
			get { return (_heapSizes & 1) == 0 ? 2 : 4; }
		}

		internal int GuidIndexSize
		{
			get { return (_heapSizes & 2) == 0 ? 2 : 4; }
		}

		internal int BlobIndexSize
		{
			get { return (_heapSizes & 4) == 0 ? 2 : 4; }
		}

		private void CreateTables(long offset, long heapSize)
		{
			if (_tables != null)
				throw new BadMetadataException("Multiple table heaps.");
			_reader.Position = offset;

			_reader.Advance(4); //reserved: 4, always 0

			var header = new
				{
					MajorVersion = _reader.ReadUInt8(),
					MinorVersion = _reader.ReadUInt8(),
					HeapSizes = _reader.ReadUInt8(),
					_ = _reader.Advance(1), //reserved: 1, always 1
					Valid = _reader.ReadUInt64(),
					Sorted = _reader.ReadUInt64(),
				};

			_heapSizes = header.HeapSizes;

			// read table row nums
			ulong present = header.Valid; // if bit is set table is presented, otherwise it is empty
			_tables = new MetadataTable[MaxTableCount];
			for (int i = 0; i < MaxTableCount; i++)
			{
				//NOTE: If flag set table is presented
				if (((present >> i) & 1) != 0)
				{
					int rowCount = _reader.ReadInt32();
					var id = (TableId)i;
					var table = Schema.CreateTable(id);
					table.RowCount = rowCount;
					table.IsSorted = ((header.Sorted >> i) & 1) != 0;
					_tables[i] = table;
				}
			}

			//Setup tables (Offset, RowSize, Size)
			long pos = _reader.Position;
			for (int i = 0; i < MaxTableCount; i++)
			{
				var table = _tables[i];
				if (table != null)
				{
					table.Offset = pos;
					int rowSize = 0;
					foreach (var column in table.Columns)
					{
						column.Size = GetColumnSize(column);
						rowSize += column.Size;
					}
					table.RowSize = rowSize;
					table.Size = table.RowCount*rowSize;
					pos += table.Size;
				}
			}
		}

		#region Heaps

		private void CreateStringHeap(long offset, long size)
		{
			if (_strings != null)
				throw new BadMetadataException("Multiple #Strings heaps.");
			_strings = new StringHeap(_reader.Slice(offset, size));
		}

		internal string FetchString(uint offset)
		{
			if (_strings == null)
				throw new BadMetadataException("#Strings heap not found.");
			return _strings.Fetch(offset);
		}

		private void CreateUserStringHeap(long offset, long size)
		{
			if (_userStrings != null)
				throw new BadMetadataException("Multiple #US heaps.");
			_userStrings = new UserStringHeap(_reader.Slice(offset, size));
		}

		public string GetUserString(uint offset)
		{
			if (_userStrings == null)
				throw new BadMetadataException("#US heap not found.");
			return _userStrings.Fetch(offset);
		}

		private void CreateGuidHeap(long offset, long size)
		{
			if (_guids != null)
				throw new BadMetadataException("Multiple #GUID heaps.");
			_guids = new GuidHeap(_reader.Slice(offset, size));
		}

		internal Guid FetchGuid(uint index)
		{
			if (index == 0)
				return Guid.Empty;
			if (_guids == null)
				throw new BadMetadataException("#GUID heap not found.");
			//guid index is 1 based
			return _guids.Fetch((int)(index - 1));
		}

		private void CreateBlobHeap(long offset, long size)
		{
			if (_blob != null)
				throw new BadMetadataException("Multiple #Blob heaps.");
			_blob = new BlobHeap(_reader.Slice(offset, size));
		}

		internal BufferedBinaryReader FetchBlob(uint offset)
		{
			if (_blob == null)
				throw new BadMetadataException("#Blob heap not found.");
			return _blob.Fetch(offset);
		}

		#endregion

		#region Dump

		internal void Dump(string path)
		{
			var xws = new XmlWriterSettings {Indent = true, IndentChars = "  "};
			using (var writer = XmlWriter.Create(path, xws))
			{
				writer.WriteStartDocument();
				Dump(writer);
				writer.WriteEndDocument();
			}
		}

		internal void Dump(XmlWriter writer)
		{
			writer.WriteStartElement("mdb");
			foreach (var table in _tables)
			{
				if (table != null)
					Dump(writer, table);
			}
			writer.WriteEndElement();
		}

		private static void Dump(XmlWriter writer, MetadataTable table)
		{
			writer.WriteStartElement("table");
			writer.WriteAttributeString("name", table.Name);
			writer.WriteAttributeString("id", string.Format("{0} (0x{0:X2})", (int)table.Id));
			writer.WriteAttributeString("offset", XmlConvert.ToString(table.Offset));
			writer.WriteAttributeString("size", XmlConvert.ToString(table.Size));
			writer.WriteAttributeString("aligned", XmlConvert.ToString((table.Size%4) == 0));
			writer.WriteAttributeString("row-count", XmlConvert.ToString(table.RowCount));
			writer.WriteAttributeString("row-size", XmlConvert.ToString(table.RowSize));
			writer.WriteAttributeString("sorted", XmlConvert.ToString(table.IsSorted));
			writer.WriteEndElement();
		}

		#endregion

		#region LookupRows

		public IEnumerable<MetadataRow> LookupRows(TableId tableId, MetadataColumn column, int target, bool simple)
		{
			var table = GetTable(tableId);
			if (table == null)
				return Enumerable.Empty<MetadataRow>();

			var lookup = table.GetLookup(column);

			IList<int> list;
			var rowCount = GetRowCount(tableId);
			while (lookup.LastIndex < rowCount)
			{
				var row = GetRow(tableId, lookup.LastIndex);
				int key = simple ? row[column].Index - 1 : (int)((SimpleIndex)row[column].Value);

				if (!lookup.TryGetValue(key, out list))
				{
					list = new List<int>();
					lookup.Add(key, list);
				}

				list.Add(lookup.LastIndex);

				for (lookup.LastIndex++; lookup.LastIndex < rowCount; lookup.LastIndex++)
				{
					row = GetRow(tableId, lookup.LastIndex);
					int key2 = simple ? row[column].Index - 1 : (int)((SimpleIndex)row[column].Value);
					if (key != key2) break;
					list.Add(lookup.LastIndex);
				}
			}

			if (lookup.TryGetValue(target, out list))
			{
				return GetRows(tableId, list);
			}

			return Enumerable.Empty<MetadataRow>();
		}

		private IEnumerable<MetadataRow> GetRows(TableId tableId, IEnumerable<int> rows)
		{
			return rows.Select(i => GetRow(tableId, i));
		}

		public MetadataRow LookupRow(TableId tableId, MetadataColumn column, int target, bool simple)
		{
			var table = GetTable(tableId);
			if (table == null)
				return null;

			var lookup = table.GetLookup(column);

			IList<int> list;
			if (lookup.TryGetValue(target, out list))
			{
				return GetRow(tableId, list[0]);
			}

			var rowCount = GetRowCount(tableId);
			for (; lookup.LastIndex < rowCount; lookup.LastIndex++)
			{
				var row = GetRow(tableId, lookup.LastIndex);
				int index = simple ? row[column].Index - 1 : (int)((SimpleIndex)row[column].Value);

				if (index == target)
				{
					lookup.Add(target, new List<int> {lookup.LastIndex});
					lookup.LastIndex++;
					return row;
				}

				if (!lookup.TryGetValue(index, out list))
				{
					list = new List<int>();
					lookup.Add(index, list);
				}

				list.Add(lookup.LastIndex);
			}

			return null;
		}

		#endregion
	}
}