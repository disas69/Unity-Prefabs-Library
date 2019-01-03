namespace Assets.Scripts.Framework.Editor.Grid
{
    public class GridNameFilter : IGridFilter
    {
        private string _filter;
        private readonly string _title;
        private readonly float _labelWidth;

        public GridNameFilter(string title = "Name", float labelWidth = 100f)
        {
            _title = title;
            _labelWidth = labelWidth;
        }

        public bool Match(IGridItemContent entry)
        {
            if (string.IsNullOrEmpty(_filter))
            {
                return true;
            }

            if (string.IsNullOrEmpty(entry.Name))
            {
                return false;
            }

            return entry.Name.ToLowerInvariant().Contains(_filter.ToLowerInvariant());
        }

        public void Draw()
        {
            _filter = Filter.Draw(_filter, _title, _labelWidth);
        }

        public void Reset()
        {
            _filter = string.Empty;
        }
    }
}
