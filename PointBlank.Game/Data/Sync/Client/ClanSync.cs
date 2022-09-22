using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Data.Sync.Client
{
  public static class ClanSync
  {
    public static void Load(ReceiveGPacket p)
    {
      long id = p.readQ();
      int num1 = (int) p.readC();
      Account account = AccountManager.getAccount(id, true);
      if (account == null || num1 != 3)
        return;
      int num2 = p.readD();
      int num3 = (int) p.readC();
      account.clanId = num2;
      account.clanAccess = num3;
    }
  }
}
