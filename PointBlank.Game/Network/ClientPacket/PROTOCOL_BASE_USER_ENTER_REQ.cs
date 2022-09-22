using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Managers.Events;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Configs;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BASE_USER_ENTER_REQ : ReceivePacket
  {
    private long pId;
    private uint erro;

    public PROTOCOL_BASE_USER_ENTER_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      int num = (int) this.readC();
      this.pId = this.readQ();
    }

    public override void run()
    {
      if (this._client == null)
        return;
      try
      {
        if (this._client._player != null)
        {
          this.erro = 2147483648U;
        }
        else
        {
          PointBlank.Game.Data.Model.Account accountDb = AccountManager.getAccountDB((object) this.pId, 2, 0);
          if (accountDb != null && accountDb._status.serverId == (byte) 0)
          {
            this._client.player_id = accountDb.player_id;
            accountDb._connection = this._client;
            accountDb.GetAccountInfos(29);
            accountDb.LoadInventory();
            accountDb.LoadMissionList();
            accountDb.LoadPlayerBonus();
            this.EnableQuestMission(accountDb);
            accountDb._inventory.LoadBasicItems();
            accountDb.SetPublicIP(this._client.GetAddress());
            accountDb.Session = new PlayerSession()
            {
              _sessionId = this._client.SessionId,
              _playerId = this._client.player_id
            };
            accountDb.updateCacheInfo();
            accountDb._status.updateServer((byte) GameConfig.serverId);
            this._client._player = accountDb;
            ComDiv.updateDB("accounts", "lastip", (object) accountDb.PublicIP.ToString(), "player_id", (object) accountDb.player_id);
            if (accountDb._topups.Count > 0)
            {
              for (int index = 0; index < accountDb._topups.Count; ++index)
              {
                PlayerItemTopup topup = accountDb._topups[index];
                if (topup.ItemId != 0)
                {
                  this._client.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, accountDb, new ItemsModel(topup.ItemId, topup.ItemName, topup.Equip, topup.Count, 0L)));
                  PlayerManager.DeletePlayerTopup(topup.ObjectId, accountDb.player_id);
                }
              }
            }
          }
          else
            this.erro = 2147483648U;
        }
        this._client.SendPacket((SendPacket) new PROTOCOL_BASE_USER_ENTER_ACK(this.erro));
        if (this.erro <= 0U)
          return;
        this._client.Close(500, false);
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_BASE_USER_ENTER_REQ: " + ex.ToString());
        this._client.Close(0, false);
      }
    }

    private void EnableQuestMission(PointBlank.Game.Data.Model.Account player)
    {
      PlayerEvent playerEvent = player._event;
      if (playerEvent == null || playerEvent.LastQuestFinish != 0 || EventQuestSyncer.getRunningEvent() == null)
        return;
      player._mission.mission4 = 13;
    }
  }
}
