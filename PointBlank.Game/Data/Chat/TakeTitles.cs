using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Title;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using PointBlank.Game.Network.ServerPacket;

namespace PointBlank.Game.Data.Chat
{
  public static class TakeTitles
  {
    public static string GetAllTitles(PointBlank.Game.Data.Model.Account p)
    {
      if (p._titles.ownerId == 0L)
      {
        TitleManager.getInstance().CreateTitleDB(p.player_id);
        p._titles = new PlayerTitles()
        {
          ownerId = p.player_id
        };
      }
      PlayerTitles titles = p._titles;
      int num = 0;
      for (int titleId = 1; titleId <= 44; ++titleId)
      {
        TitleQ title = TitlesXml.getTitle(titleId, true);
        if (title != null && !titles.Contains(title._flag))
        {
          ++num;
          titles.Add(title._flag);
          if (titles.Slots < title._slot)
            titles.Slots = title._slot;
        }
      }
      if (num > 0)
      {
        ComDiv.updateDB("player_titles", "titleslots", (object) titles.Slots, "owner_id", (object) p.player_id);
        TitleManager.getInstance().updateTitlesFlags(p.player_id, titles.Flags);
        p.SendPacket((SendPacket) new PROTOCOL_BASE_USER_TITLE_INFO_ACK(p));
      }
      return Translation.GetLabel("TitleAcquisiton", (object) num);
    }
  }
}
