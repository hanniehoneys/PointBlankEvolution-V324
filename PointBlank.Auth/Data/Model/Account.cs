using PointBlank.Auth.Data.Configs;
using PointBlank.Auth.Data.Managers;
using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Account.Title;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace PointBlank.Auth.Data.Model
{
  public class Account
  {
    public string player_name = "";
    public PlayerEquipedItems _equip = new PlayerEquipedItems();
    public PlayerInventory _inventory = new PlayerInventory();
    public PlayerMissions _mission = new PlayerMissions();
    public PlayerStats _statistic = new PlayerStats();
    public AccountStatus _status = new AccountStatus();
    public FriendSystem FriendSystem = new FriendSystem();
    public PlayerDailyRecord Daily = new PlayerDailyRecord();
    public List<PointBlank.Auth.Data.Model.Account> _clanPlayers = new List<PointBlank.Auth.Data.Model.Account>();
    public List<Character> Characters = new List<Character>();
    public List<PlayerItemTopup> _topups = new List<PlayerItemTopup>();
    public bool _myConfigsLoaded;
    public bool _isOnline;
    public bool ICafe;
    public CouponEffects effects;
    public uint LastRankUpDate;
    public string password;
    public string login;
    public string token;
    public string hwid;
    public int tourneyLevel;
    public int _exp;
    public int _gp;
    public int clan_id;
    public int clanAccess;
    public int _money;
    public int pc_cafe;
    public int _rank;
    public int brooch;
    public int insignia;
    public int medal;
    public int blue_order;
    public int name_color;
    public int access;
    public int age;
    public long player_id;
    public long ban_obj_id;
    public PhysicalAddress MacAddress;
    public AuthClient _connection;
    public PlayerBonus _bonus;
    public PlayerConfig _config;
    public PlayerTitles _titles;
    public PlayerEvent _event;

    public void SimpleClear()
    {
      this._config = (PlayerConfig) null;
      this._titles = (PlayerTitles) null;
      this._bonus = (PlayerBonus) null;
      this._event = (PlayerEvent) null;
      this._connection = (AuthClient) null;
      this._inventory = new PlayerInventory();
      this.Characters = new List<Character>();
      this.FriendSystem = new FriendSystem();
      this._clanPlayers = new List<PointBlank.Auth.Data.Model.Account>();
      this._equip = new PlayerEquipedItems();
      this._mission = new PlayerMissions();
      this._status = new AccountStatus();
      this._topups = new List<PlayerItemTopup>();
      this.Daily = new PlayerDailyRecord();
    }

    public void setOnlineStatus(bool online)
    {
      if (this._isOnline == online || !ComDiv.updateDB("accounts", nameof (online), (object) online, "player_id", (object) this.player_id))
        return;
      this._isOnline = online;
    }

    public void updateCacheInfo()
    {
      if (this.player_id == 0L)
        return;
      lock (AccountManager.getInstance()._accounts)
        AccountManager.getInstance()._accounts[this.player_id] = this;
    }

    public void Close(int time)
    {
      if (this._connection == null)
        return;
      this._connection.Close(time, true);
    }

    public void SendPacket(SendPacket sp)
    {
      if (this._connection == null)
        return;
      this._connection.SendPacket(sp);
    }

    public void SendPacket(byte[] data)
    {
      if (this._connection == null)
        return;
      this._connection.SendPacket(data);
    }

    public void SendCompletePacket(byte[] data)
    {
      if (this._connection == null)
        return;
      this._connection.SendCompletePacket(data);
    }

    public void LoadInventory()
    {
      lock (this._inventory._items)
        this._inventory._items.AddRange((IEnumerable<ItemsModel>) PlayerManager.getInventoryItems(this.player_id));
    }

    public void LoadMissionList()
    {
      PlayerMissions mission = MissionManager.getInstance().getMission(this.player_id, this._mission.mission1, this._mission.mission2, this._mission.mission3, this._mission.mission4);
      if (mission == null)
        MissionManager.getInstance().addMissionDB(this.player_id);
      else
        this._mission = mission;
    }

    public long Status()
    {
      return !string.IsNullOrEmpty(this.player_name) ? 1L : 0L;
    }

    public void SetPlayerId(long id, int LoadType)
    {
      this.player_id = id;
      this.GetAccountInfos(LoadType);
    }

    public void SetPlayerId()
    {
      this._titles = new PlayerTitles();
      this._bonus = new PlayerBonus();
      this.GetAccountInfos(8);
    }

    public void GetAccountInfos(int LoadType)
    {
      if (LoadType == 0 || this.player_id == 0L)
        return;
      if ((LoadType & 1) == 1)
      {
        this._titles = TitleManager.getInstance().getTitleDB(this.player_id);
        this._topups = PlayerManager.getPlayerTopups(this.player_id);
        this.Characters = CharacterManager.getCharacters(this.player_id);
        this.Daily = PlayerManager.getPlayerDailyRecord(this.player_id);
        if (this.Daily == null)
          PlayerManager.CreatePlayerDailyRecord(this.player_id);
      }
      if ((LoadType & 2) == 2)
        this._bonus = PlayerManager.getPlayerBonusDB(this.player_id);
      if ((LoadType & 4) == 4)
      {
        List<Friend> friendList = PlayerManager.getFriendList(this.player_id);
        if (friendList.Count > 0)
          this.FriendSystem._friends = friendList;
      }
      if ((LoadType & 8) == 8)
      {
        this._event = PlayerManager.getPlayerEventDB(this.player_id);
        if (this._event == null)
        {
          PlayerManager.addEventDB(this.player_id);
          this._event = new PlayerEvent();
        }
      }
      if ((LoadType & 16) != 16)
        return;
      this._config = PlayerManager.getConfigDB(this.player_id);
      if (this._config == null)
        PlayerManager.CreateConfigDB(this.player_id);
      if (this._connection == null)
        return;
      this.ICafe = ICafeManager.GetCafe(this._connection.GetIPAddress());
      if (!this.ICafe)
        return;
      this.pc_cafe = 1;
    }

    public Character getCharacter(int ItemId)
    {
      for (int index = 0; index < this.Characters.Count; ++index)
      {
        Character character = this.Characters[index];
        if (character.Id == ItemId)
          return character;
      }
      return (Character) null;
    }

    public bool CompareToken(string Token)
    {
      if (!AuthConfig.isTestMode)
        return Token == this.token;
      return true;
    }

    public bool DiscountPlayerItems()
    {
      try
      {
        bool flag = false;
        long int64 = Convert.ToInt64(DateTime.Now.ToString("yyMMddHHmm"));
        List<object> objectList = new List<object>();
        int num1 = this._bonus != null ? this._bonus.bonuses : 0;
        int num2 = this._bonus != null ? this._bonus.freepass : 0;
        lock (this._inventory._items)
        {
          for (int index1 = 0; index1 < this._inventory._items.Count; ++index1)
          {
            ItemsModel itemsModel = this._inventory._items[index1];
            if (itemsModel._count <= int64 & itemsModel._equip == 2)
            {
              if (itemsModel._category == 2 && ComDiv.getIdStatics(itemsModel._id, 1) == 6)
              {
                Character character1 = this.getCharacter(itemsModel._id);
                if (character1 != null)
                {
                  int Slot = 0;
                  for (int index2 = 0; index2 < this.Characters.Count; ++index2)
                  {
                    Character character2 = this.Characters[index2];
                    if (character2.Slot != character1.Slot)
                    {
                      character2.Slot = Slot;
                      CharacterManager.Update(Slot, character2.ObjId);
                      ++Slot;
                    }
                  }
                  if (CharacterManager.Delete(character1.ObjId, this.player_id))
                    this.Characters.Remove(character1);
                }
              }
              else if (itemsModel._category == 3)
              {
                if (this._bonus != null)
                {
                  if (!this._bonus.RemoveBonuses(itemsModel._id))
                  {
                    if (itemsModel._id == 1600014)
                    {
                      ComDiv.updateDB("player_bonus", "sightcolor", (object) 4, "player_id", (object) this.player_id);
                      this._bonus.sightColor = 4;
                    }
                    else if (itemsModel._id == 1600006)
                    {
                      ComDiv.updateDB("accounts", "name_color", (object) 0, "player_id", (object) this.player_id);
                      this.name_color = 0;
                    }
                    else if (itemsModel._id == 1600009)
                    {
                      ComDiv.updateDB("player_bonus", "fakerank", (object) 55, "player_id", (object) this.player_id);
                      this._bonus.fakeRank = 55;
                    }
                    else if (itemsModel._id == 1600010)
                    {
                      if (this._bonus.fakeNick.Length > 0)
                      {
                        ComDiv.updateDB("player_bonus", "fakenick", (object) "", "player_id", (object) this.player_id);
                        ComDiv.updateDB("accounts", "player_name", (object) this._bonus.fakeNick, "player_id", (object) this.player_id);
                        this.player_name = this._bonus.fakeNick;
                        this._bonus.fakeNick = "";
                      }
                    }
                    else if (itemsModel._id == 1600187)
                    {
                      ComDiv.updateDB("player_bonus", "muzzle", (object) 0, "player_id", (object) this.player_id);
                      this._bonus.muzzle = 0;
                    }
                  }
                  CouponFlag couponEffect = CouponEffectManager.getCouponEffect(itemsModel._id);
                  if (couponEffect != null && couponEffect.EffectFlag > (CouponEffects) 0 && this.effects.HasFlag((Enum) couponEffect.EffectFlag))
                  {
                    this.effects -= couponEffect.EffectFlag;
                    flag = true;
                  }
                }
                else
                  continue;
              }
              objectList.Add((object) itemsModel._objId);
              this._inventory._items.RemoveAt(index1--);
            }
            else if (itemsModel._count == 0L)
            {
              objectList.Add((object) itemsModel._objId);
              this._inventory._items.RemoveAt(index1--);
            }
          }
          ComDiv.deleteDB("player_items", "object_id", objectList.ToArray(), "owner_id", (object) this.player_id);
        }
        if (this._bonus != null && (this._bonus.bonuses != num1 || this._bonus.freepass != num2))
          PlayerManager.updatePlayerBonus(this.player_id, this._bonus.bonuses, this._bonus.freepass);
        if (this.effects < (CouponEffects) 0)
          this.effects = (CouponEffects) 0;
        if (flag)
          PlayerManager.updateCupomEffects(this.player_id, this.effects);
        this._inventory.LoadBasicItems();
        int num3 = PlayerManager.CheckEquipedItems(this._equip, this._inventory._items, false);
        if (num3 > 0)
        {
          DBQuery query = new DBQuery();
          if ((num3 & 2) == 2)
            PlayerManager.updateWeapons(this._equip, query);
          if ((num3 & 1) == 1)
            PlayerManager.updateChars(this._equip, query);
          ComDiv.updateDB("accounts", "player_id", (object) this.player_id, query.GetTables(), query.GetValues());
        }
        return true;
      }
      catch (Exception ex)
      {
        Logger.error("DiscountPlayerItems: " + ex.ToString());
        return false;
      }
    }

    public bool IsGM()
    {
      if (this._rank != 53 && this._rank != 54)
        return this.access > 2;
      return true;
    }

    public bool HaveGMLevel()
    {
      return this.access > 2;
    }

    public bool HaveAcessLevel()
    {
      return this.access > 0;
    }
  }
}
