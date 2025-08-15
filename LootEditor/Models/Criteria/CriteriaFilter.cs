namespace LootEditor.Models;

public enum CriteriaFilterType
{
    Unstructured,
    Has
}

public sealed record CriteriaFilter(bool IsNegated, CriteriaFilterType Type, string[] Tokens);
