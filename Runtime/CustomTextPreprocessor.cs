using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;

public class CustomTextPreprocessor : ITextPreprocessor
{
	private Dictionary<string, string> tagReplacements = new Dictionary<string, string>
	{
		{ "g1", "<gradient=\"Color Gradient_ MAGE\">" },
		{ "/g1", "</gradient>" },
		{ "g2", "<gradient=\"Color Gradient_ Objective\"><b>" },
		{ "/g2", "</b></gradient>" },
		{ "g3", "<gradient=\"Color Gradient_ Green\">" },
		{ "/g3", "</gradient>" },
		{ "g4", "<gradient=\"Color Gradient_ Purple\">" },
		{ "/g4", "</gradient>" },
	};

	public string PreprocessText(string text)
	{
		foreach (var tag in tagReplacements)
		{
			text = text.Replace($"<{tag.Key}>", tag.Value);
		}

		return text;
	}
}