using System.Collections;

namespace AvalonCode.Services.Models
{
    public abstract class SolutionItem : IReadOnlyList<ISolutionItem>, ISolutionItem
    {
        protected SolutionItem(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public ISolutionItem? Parent { get; private set; }

        private readonly List<ISolutionItem> _children = new();

        public IReadOnlyList<ISolutionItem> Children => _children;

        public int Count => _children.Count;

        public ISolutionItem this[int index] => _children[index];

        public ISolutionItem? this[string name] => _children.FirstOrDefault(x => x.Name == name);

        public void AddChild(SolutionItem item)
        {
            if (item.Parent is SolutionItem solutionItem)
            {
                solutionItem.RemoveChild(item);
            }

            _children.Add(item);

            item.Parent = this;
        }

        public void RemoveChild(SolutionItem item)
        {
            _children.Remove(item);

            item.Parent = null;
        }

        public IEnumerator<ISolutionItem> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<ISolutionItem> Descendents()
        {
            foreach (var item in _children)
            {
                foreach (var subItem in item.Descendents())
                    yield return subItem;

                yield return item;
            }
        }

        public T? GetParent<T>()
        {
            var parent = Parent;
            while (parent != null)
            {
                if (parent is T parentAsT)
                    return parentAsT;
                parent = parent.Parent;
            }

            return default;
        }
    }
}