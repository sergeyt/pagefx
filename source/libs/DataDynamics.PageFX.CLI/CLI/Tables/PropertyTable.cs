﻿using DataDynamics.PageFX.CLI.Metadata;
using DataDynamics.PageFX.CodeModel;

namespace DataDynamics.PageFX.CLI.Tables
{
	internal sealed class PropertyTable : MetadataTable<IProperty>
	{
		public PropertyTable(AssemblyLoader loader)
			: base(loader)
		{
		}

		public override MdbTableId Id
		{
			get { return MdbTableId.Property; }
		}

		protected override IProperty ParseRow(MdbRow row, int index)
		{
			var flags = (PropertyAttributes)row[MDB.Property.Flags].Value;
			var name = row[MDB.Property.Name].String;
			var token = MdbIndex.MakeToken(MdbTableId.Property, index + 1);
			var value = Loader.Const[token];

			return new Property
				{
					MetadataToken = token,
					Name = name,
					IsSpecialName = ((flags & PropertyAttributes.SpecialName) != 0),
					IsRuntimeSpecialName = ((flags & PropertyAttributes.RTSpecialName) != 0),
					HasDefault = ((flags & PropertyAttributes.HasDefault) != 0),
					Value = value
				};
		}
	}
}
