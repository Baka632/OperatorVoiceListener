namespace OperatorVoiceListener.Main.Models;

public readonly record struct OperatorIdTitleInfo : IComparable<OperatorIdTitleInfo>
{
    public OperatorIdTitleInfo(string id, string title)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Title = title ?? throw new ArgumentNullException(nameof(title));
    }

    public string Id { get; init; }
    public string Title { get; init; }

    public int CompareTo(OperatorIdTitleInfo other)
    {
        return string.CompareOrdinal(Id, other.Id);
    }

    public override string ToString()
    {
        return Id;
    }
}
