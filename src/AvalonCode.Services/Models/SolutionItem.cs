using System.Collections;

namespace AvalonCode.Services.Models
{
    public abstract class SolutionItem : IReadOnlyList<SolutionItem>
    {
        protected SolutionItem(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public SolutionItem? Parent { get; private set; }

        private readonly List<SolutionItem> _children = new();

        public IReadOnlyList<SolutionItem> Children => _children;

        public int Count => _children.Count;

        public SolutionItem this[int index] => _children[index];

        public SolutionItem? this[string name] => _children.FirstOrDefault(x => x.Name == name);

        public void AddChild(SolutionItem item)
        {
            if (item.Parent != null)
            { 
                item.Parent.RemoveChild(item);
            }

            _children.Add(item);

            item.Parent = this;
        }

        public void RemoveChild(SolutionItem item)
        { 
            _children.Remove(item);

            item.Parent = null;
        }

        public IEnumerator<SolutionItem> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<SolutionItem> Descendents()
        {
            foreach (var item in _children)
            {
                foreach (var subItem in item.Descendents())
                    yield return subItem;

                yield return item;
            }
        }
    }
}