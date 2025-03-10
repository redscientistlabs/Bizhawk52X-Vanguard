﻿namespace ISOParser
{
	/// <summary>
	/// Representation of a file in the file system.
	/// </summary>
	public class ISOFileNode : ISONode
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="record">The record to construct from.</param>
		public ISOFileNode(ISONodeRecord record)
			: base(record)
		{
			// Do Nothing
		}
	}
}
