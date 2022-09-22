using PointBlank.Auth.Data.Managers;
using PointBlank.Auth.Data.Model;
using PointBlank.Auth.Data.Sync.Update;
using PointBlank.Core.Network;

namespace PointBlank.Auth.Data.Sync.Client
{
  public static class ClanSync
  {
    public static void Load(ReceiveGPacket p)
    {
      long id1 = p.readQ();
      int num1 = (int) p.readC();
      Account account = AccountManager.getInstance().getAccount(id1, true);
      if (account == null)
        return;
      switch (num1)
      {
        case 0:
          ClanInfo.ClearList(account);
          break;
        case 1:
          long pId = p.readQ();
          string str = p.readS((int) p.readC());
          byte[] buffer = p.readB(4);
          byte num2 = p.readC();
          Account member = new Account() { player_id = pId, player_name = str, _rank = (int) num2 };
          member._status.SetData(buffer, pId);
          ClanInfo.AddMember(account, member);
          break;
        case 2:
          long id2 = p.readQ();
          ClanInfo.RemoveMember(account, id2);
          break;
        case 3:
          int num3 = p.readD();
          int num4 = (int) p.readC();
          account.clan_id = num3;
          account.clanAccess = num4;
          break;
      }
    }
  }
}
