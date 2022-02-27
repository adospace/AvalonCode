namespace AvalonCode.Services
{
    public interface ISolutionItem
    {
        ISolutionItem this[int index] { get; }

        ISolutionItem? this[string name] { get; }

        IReadOnlyList<ISolutionItem> Children { get; }

        int Count { get; }

        string Name { get; }

        ISolutionItem? Parent { get; }

        IEnumerable<ISolutionItem> Descendents();

        IEnumerator<ISolutionItem> GetEnumerator();

        T? GetParent<T>();
    }
}