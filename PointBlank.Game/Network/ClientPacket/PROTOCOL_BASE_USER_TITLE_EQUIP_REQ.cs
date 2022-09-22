using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Title;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BASE_USER_TITLE_EQUIP_REQ : ReceivePacket
  {
    private byte slotIdx;
    private byte titleId;
    private uint erro;

    public PROTOCOL_BASE_USER_TITLE_EQUIP_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.slotIdx = this.readC();
      this.titleId = this.readC();
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null)
          return;
        PlayerTitles titles = player._titles;
        TitleQ title = TitlesXml.getTitle((int) this.titleId, true);
        TitleQ title1;
        TitleQ title2;
        TitleQ title3;
        TitlesXml.get3Titles(titles.Equiped1, titles.Equiped2, titles.Equiped3, out title1, out title2, out title3, false);
        if (this.slotIdx >= (byte) 3 || this.titleId >= (byte) 45 || (titles == null || title == null) || (title._classId == title1._classId && this.slotIdx != (byte) 0 || title._classId == title2._classId && this.slotIdx != (byte) 1) || (title._classId == title3._classId && this.slotIdx != (byte) 2 || (!titles.Contains(title._flag) || titles.Equiped1 == (int) this.titleId) || (titles.Equiped2 == (int) this.titleId || titles.Equiped3 == (int) this.titleId)))
          this.erro = 2147483648U;
        else if (TitleManager.getInstance().updateEquipedTitle(titles.ownerId, (int) this.slotIdx, (int) this.titleId))
          titles.SetEquip((int) this.slotIdx, (int) this.titleId);
        else
          this.erro = 2147483648U;
        this._client.SendPacket((SendPacket) new PROTOCOL_BASE_USER_TITLE_EQUIP_ACK(this.erro));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_BASE_USER_TITLE_EQUIP_REQ: " + ex.ToString());
      }
    }
  }
}
