namespace Assets.Scripts.Framework.Editor.Grid
{
    public interface IGridItemContent
    {
        string Name { get; }
        bool IsValid { get; }
        void Draw(bool isSelected);
    }
}
