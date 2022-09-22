using PointBlank.Core;
using PointBlank.Core.Filters;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using PointBlank.Game.Data.Configs;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BASE_CREATE_NICK_REQ : ReceivePacket
  {
    private string name;

    public PROTOCOL_BASE_CREATE_NICK_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.name = this.readUnicode((int) this.readC() * 2);
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null || player.player_name.Length > 0 || (string.IsNullOrEmpty(this.name) || this.name.Length < GameConfig.minNickSize) || this.name.Length > GameConfig.maxNickSize)
        {
          this._client.SendPacket((SendPacket) new PROTOCOL_BASE_CREATE_NICK_ACK(2147487763U, ""));
        }
        else
        {
          foreach (string str in NickFilter._filter)
          {
            if (this.name.Contains(str))
            {
              this._client.SendPacket((SendPacket) new PROTOCOL_BASE_CREATE_NICK_ACK(2147487763U, ""));
              return;
            }
          }
          if (!PlayerManager.isPlayerNameExist(this.name))
          {
            if (AccountManager.updatePlayerName(this.name, player.player_id))
            {
              NickHistoryManager.CreateHistory(player.player_id, player.player_name, this.name, "สร้างตัวละคร");
              player.player_name = this.name;
              List<ItemsModel> creationAwards = BasicInventoryXml.creationAwards;
              List<ItemsModel> characters = BasicInventoryXml.Characters;
              if (creationAwards.Count > 0)
              {
                foreach (ItemsModel itemsModel in creationAwards)
                {
                  if (itemsModel._id != 0)
                    this._client.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, player, itemsModel));
                }
              }
              this._client.SendPacket((SendPacket) new PROTOCOL_SHOP_PLUS_POINT_ACK(player._gp, player._gp, 4));
              this._client.SendPacket((SendPacket) new PROTOCOL_BASE_QUEST_GET_INFO_ACK(player));
              this._client.SendPacket((SendPacket) new PROTOCOL_BASE_CREATE_NICK_ACK(0U, this.name));
              if (characters.Count > 0)
              {
                foreach (ItemsModel itemsModel in characters)
                {
                  if (itemsModel._id != 0)
                  {
                    int count = player.Characters.Count;
                    Character character1 = new Character();
                    character1.Id = itemsModel._id;
                    character1.Name = itemsModel._name;
                    character1.PlayTime = 0;
                    Character character2 = character1;
                    int num1 = count;
                    int num2 = num1 + 1;
                    character2.Slot = num1;
                    character1.CreateDate = int.Parse(DateTime.Now.ToString("yyMMddHHmm"));
                    player.Characters.Add(character1);
                    this._client.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, player, itemsModel));
                    if (CharacterManager.Create(character1, player.player_id))
                      this._client.SendPacket((SendPacket) new PROTOCOL_CHAR_CREATE_CHARA_ACK(0U, 3, character1, player));
                    else
                      this._client.SendPacket((SendPacket) new PROTOCOL_CHAR_CREATE_CHARA_ACK(2147483648U, 0, (Character) null, (PointBlank.Game.Data.Model.Account) null));
                  }
                }
              }
              this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INFO_ACK(player.FriendSystem._friends));
            }
            else
              this._client.SendPacket((SendPacket) new PROTOCOL_BASE_CREATE_NICK_ACK(2147487763U, ""));
          }
          else
            this._client.SendPacket((SendPacket) new PROTOCOL_BASE_CREATE_NICK_ACK(2147483923U, ""));
        }
      }
      catch (Exception ex)
      {
        Logger.warning("PROTOCOL_LOBBY_CREATE_NICK_NAME_REQ: " + ex.ToString());
      }
    }
  }
}
