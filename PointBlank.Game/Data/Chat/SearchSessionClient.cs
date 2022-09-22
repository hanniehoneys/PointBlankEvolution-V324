namespace PointBlank.Game.Data.Chat
{
  public static class SearchSessionClient
  {
    public static string genCode1(string str)
    {
      GameManager.SearchActiveClient(uint.Parse(str.Substring(13)));
      return "";
    }
  }
}
