namespace QuickMafs
{
    public delegate void BoardEventHandler();
    public interface IBoardController
    {
        event BoardEventHandler TileSelected;
        event BoardEventHandler MatchMade;
    }
}
