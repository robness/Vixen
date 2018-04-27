﻿using System.Drawing;
using System.Runtime.Serialization;

namespace Vixen.Sys.Marks
{
	[DataContract]
	public class MarkDecorator
	{
		public MarkDecorator()
		{
			Color = Color.Black;
			IsBold = false;
			IsSolidLine = false;
		}
		[DataMember]
		public bool IsBold { get; set; }

		[DataMember]
		public bool IsSolidLine { get; set; }

		[DataMember]
		public Color Color { get; set; }
	}
}
