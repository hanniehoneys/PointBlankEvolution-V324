using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CHAR_DELETE_CHARA_REQ : ReceivePacket
  {
    private int Slot;

    public PROTOCOL_CHAR_DELETE_CHARA_REQ(GameClient Client, byte[] Buffer)
    {
      this.makeme(Client, Buffer);
    }

    public override void read()
    {
      this.Slot = (int) this.readC();
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player != null)
        {
          Character characterSlot = player.getCharacterSlot(this.Slot);
          if (characterSlot != null)
          {
            ItemsModel itemsModel = player._inventory.getItem(characterSlot.Id);
            if (itemsModel != null)
            {
              int Slot = 0;
              for (int index = 0; index < player.Characters.Count; ++index)
              {
                Character character = player.Characters[index];
                if (character.Slot != characterSlot.Slot)
                {
                  character.Slot = Slot;
                  CharacterManager.Update(Slot, character.ObjId);
                  ++Slot;
                }
              }
              this._client.SendPacket((SendPacket) new PROTOCOL_CHAR_DELETE_CHARA_ACK(0U, this.Slot, player, itemsModel));
              if (CharacterManager.Delete(characterSlot.ObjId, player.player_id))
                player.Characters.Remove(characterSlot);
              if (!PlayerManager.DeleteItem(itemsModel._objId, player.player_id))
                return;
              player._inventory.RemoveItem(itemsModel);
            }
            else
              this._client.SendPacket((SendPacket) new PROTOCOL_CHAR_DELETE_CHARA_ACK(2147487911U, 0, (PointBlank.Game.Data.Model.Account) null, (ItemsModel) null));
          }
          else
            this._client.SendPacket((SendPacket) new PROTOCOL_CHAR_DELETE_CHARA_ACK(2147487911U, 0, (PointBlank.Game.Data.Model.Account) null, (ItemsModel) null));
        }
        else
          this._client.SendPacket((SendPacket) new PROTOCOL_CHAR_DELETE_CHARA_ACK(2147487911U, 0, (PointBlank.Game.Data.Model.Account) null, (ItemsModel) null));
      }
      catch (Exception ex)
      {
        Logger.error("PROTOCOL_CHAR_DELETE_CHARA_REQ: " + ex.ToString());
      }
    }
  }
}
