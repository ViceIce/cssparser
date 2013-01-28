﻿using System;
using System.Collections.Generic;
using CSSParser.ContentProcessors.CharacterProcessors;
using CSSParser.ContentProcessors.CharacterProcessors.Factories;
using CSSParser.ContentProcessors.StringProcessors;
using CSSParser.StringNavigators;

namespace CSSParser
{
	public static class Parser
	{
		/// <summary>
		/// This will never return null nor a set containing any null references. It will throw an exception for a null content reference.
		/// CSS does not support single line comment, unlike LESS CSS.
		/// </summary>
		public static IEnumerable<CategorisedCharacterString> ParseCSS(string content)
		{
			if (content == null)
				throw new ArgumentNullException("content");

			return Parse(content, false);
		}

		/// <summary>
		/// This will never return null nor a set containing any null references. It will throw an exception for a null content reference.
		/// LESS CSS supports single line comments as well the multiline comment format supported by standard CSS.
		/// </summary>
		public static IEnumerable<CategorisedCharacterString> ParseLESS(string content)
		{
			if (content == null)
				throw new ArgumentNullException("content");

			return Parse(content, true);
		}

		private static IEnumerable<CategorisedCharacterString> Parse(string content, bool supportSingleLineComments)
		{
			if (content == null)
				throw new ArgumentNullException("content");

			var processorFactory = new CachingCharacterProcessorsFactory(
				new CharacterProcessorsFactory()
			);
			return (new ProcessedCharactersGrouper()).GetStrings(
				new StringNavigator(content),
				processorFactory.Get<SelectorOrStylePropertySegment>(
					supportSingleLineComments
						? SelectorOrStyleSegment.SingleLineCommentsSupportOptions.Support
						: SelectorOrStyleSegment.SingleLineCommentsSupportOptions.DoNotSupport,
					processorFactory
				)
			);
		}
	}
}
