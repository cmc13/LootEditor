using LootEditor.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LootEditor.Services;

public class FilterParser
{
    private readonly string _input;
    private int _i;

    public FilterParser(string input) => _input = input;

    public List<CriteriaFilter> ParseAll()
    {
        var tokens = new List<CriteriaFilter>();
        while (!IsEof)
        {
            SkipWhitespace();
            if (IsEof) break;

            bool neg = false;
            if (PeekStarts("!"))
            {
                neg = true;
                Expect("!");
            }

            if (PeekStarts("has:"))
                tokens.Add(ParseHasClause(neg));
            else if (_input[_i] == '"')
            {
                // guard: is there at least one more '"' later?
                if (_i + 1 >= _input.Length || _input.IndexOf('"', _i + 1) == -1)
                    throw new FormatException($"Unterminated quoted string at pos {_i}");

                tokens.Add(new(neg, CriteriaFilterType.Unstructured, [ParseQuoted()]));
            }
            else
                tokens.Add(new(neg, CriteriaFilterType.Unstructured, [ParseUnquoted()]));
        }
        return tokens;
    }

    private CriteriaFilter ParseHasClause(bool neg)
    {
        Expect("has:");
        var segments = new List<string>();
        segments.Add(ParseSegment());
        while (!IsEof && _input[_i] == ':')
        {
            _i++;
            segments.Add(ParseSegment());
        }
        return new(neg, CriteriaFilterType.Has, segments.ToArray());
    }

    private string ParseSegment()
    {
        return _input[_i] == '"'
            ? ParseQuoted()
            : ParseUnquoted(allowColons: false);
    }

    private string ParseQuoted()
    {
        // we know _input[_i] == '"'
        var sb = new StringBuilder();
        sb.Append(_input[_i++]);  // consume opening "

        while (!IsEof)
        {
            char c = _input[_i++];
            sb.Append(c);

            // if this is a non-escaped ", we’re done
            if (c == '"' && sb.Length >= 2 && sb[sb.Length - 2] != '\\')
                return Regex.Unescape(sb.ToString()[1..^1]);
        }

        // should never get here because we pre-checked for a closing quote
        throw new FormatException($"Unterminated quoted string starting at pos {_i - sb.Length}");
    }

    private string ParseUnquoted(bool allowColons = true)
    {
        var sb = new StringBuilder();
        while (!IsEof)
        {
            char c = _input[_i];
            if (char.IsWhiteSpace(c) || (!allowColons && c == ':')) break;
            sb.Append(c);
            _i++;
        }
        return sb.ToString();
    }

    private void SkipWhitespace()
    {
        while (!IsEof && char.IsWhiteSpace(_input[_i])) _i++;
    }

    private bool PeekStarts(string s)
    {
        return _input.Substring(_i).StartsWith(s, StringComparison.Ordinal);
    }

    private void Expect(string s)
    {
        if (!PeekStarts(s)) throw new Exception($"Expected '{s}'");
        _i += s.Length;
    }

    private bool IsEof => _i >= _input.Length;
}