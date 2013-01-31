﻿using System;

namespace CSSParser.ContentProcessors.StringProcessors
{
	public class CategorisedCharacterString
	{
		public CategorisedCharacterString(string value, int indexInSource, CharacterCategorisationOptions characterCategorisation)
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentException("Null/blank value specified");
			if (indexInSource < 0)
				throw new ArgumentOutOfRangeException("indexInSource", "must be zero or greater");
			if (!Enum.IsDefined(typeof(CharacterCategorisationOptions), characterCategorisation))
				throw new ArgumentOutOfRangeException("characterCategorisation");

			Value = value;
			IndexInSource = indexInSource;
			CharacterCategorisation = characterCategorisation;
		}

		/// <summary>
		/// This will never be null or an empty string
		/// </summary>
		public string Value { get; private set; }

		/// <summary>
		/// This is the location of the start of the string in the source data
		/// </summary>
		public int IndexInSource { get; private set; }

		public CharacterCategorisationOptions CharacterCategorisation { get; private set; }
	}
}
