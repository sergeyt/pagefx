﻿using System;
using System.Linq;
using DataDynamics.PageFX.CLI.Metadata;
using DataDynamics.PageFX.CodeModel;

namespace DataDynamics.PageFX.CLI.Tables
{
	internal sealed class ManifestResourceTable : MetadataTable<IManifestResource>, IManifestResourceCollection
	{
		public ManifestResourceTable(AssemblyLoader loader)
			: base(loader, MdbTableId.ManifestResource)
		{
		}

		public override MdbTableId Id
		{
			get { return MdbTableId.ManifestResource; }
		}

		public IManifestResource this[string name]
		{
			get { return this.FirstOrDefault(x => x.Name == name); }
		}

		protected override IManifestResource ParseRow(int index)
		{
			var row = Mdb.GetRow(MdbTableId.ManifestResource, index);

			string name = row[MDB.ManifestResource.Name].String;
			var offset = (int)row[MDB.ManifestResource.Offset].Value;
			var flags = (ManifestResourceAttributes)row[MDB.ManifestResource.Flags].Value;
			var isPublic = (flags & ManifestResourceAttributes.VisibilityMask) == ManifestResourceAttributes.Public;

			var resource = new ManifestResource
				{
					Name = name,
					Offset = offset,
					IsPublic = isPublic
				};

			MdbIndex impl = row[MDB.ManifestResource.Implementation].Value;
			if (impl == 0)
			{

			}
			else
			{
				switch (impl.Table)
				{
					case MdbTableId.File:
						{
							//if (offset != 0)
							//    throw new BadMetadataException(string.Format("Offset of manifest resource {0} shall be zero.", mr.Name));
							var reader = Mdb.SeekResourceOffset(offset);
							int size = reader.ReadInt32();
							resource.Data = reader.ReadBlock(size);
						}
						break;

					case MdbTableId.AssemblyRef:
						{
							throw new NotSupportedException();
						}
				}
			}

			return resource;
		}

		
	}
}