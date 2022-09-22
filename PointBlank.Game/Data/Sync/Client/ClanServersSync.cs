using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;

namespace PointBlank.Game.Data.Sync.Client
{
  public class ClanServersSync
  {
    public static void Load(ReceiveGPacket p)
    {
      int num1 = (int) p.readC();
      int id = p.readD();
      PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(id);
      if (num1 == 0)
      {
        if (clan != null)
          return;
        long num2 = p.readQ();
        int num3 = p.readD();
        string str1 = p.readS((int) p.readC());
        string str2 = p.readS((int) p.readC());
        ClanManager.AddClan(new PointBlank.Core.Models.Account.Clan.Clan()
        {
          _id = id,
          _name = str1,
          owner_id = num2,
          _logo = 0U,
          _info = str2,
          creationDate = num3
        });
      }
      else
      {
        if (clan == null)
          return;
        ClanManager.RemoveClan(clan);
      }
    }
  }
}
