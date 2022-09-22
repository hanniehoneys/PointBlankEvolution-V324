using PointBlank.Auth.Data.Configs;
using PointBlank.Auth.Data.Managers;
using PointBlank.Auth.Data.Model;
using PointBlank.Auth.Data.Sync;
using PointBlank.Auth.Data.Sync.Server;
using PointBlank.Auth.Network.ServerPacket;
using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Managers.Server;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using System;
using System.Net.NetworkInformation;

namespace PointBlank.Auth.Network.ClientPacket
{
  public class PROTOCOL_BASE_LOGIN_REQ : ReceivePacket
  {
    private string Token;
    private string GameVersion;
    private string PublicIP;
    private int TokenSize;
    private ClientLocale GameLocale;
    private PhysicalAddress MacAddress;

    public PROTOCOL_BASE_LOGIN_REQ(AuthClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.readB((int) this.readC());
      this.readB(16);
      this.readS(32);
      this.readB(26);
      int num1 = (int) this.readC();
      this.GameVersion = ((int) this.readC()).ToString() + "." + (object) this.readH();
      this.TokenSize = (int) this.readH();
      this.Token = this.readS(this.TokenSize);
      int num2 = (int) this.readC();
      int num3 = (int) this.readH();
      this.PublicIP = this._client.GetIPAddress();
      this.GameLocale = ClientLocale.Thai;
      this.MacAddress = new PhysicalAddress(new byte[6]);
    }

    public override void run()
    {
      try
      {
        if (this.PublicIP == null)
          this._client.Close(0, true);
                _client._player = AccountManager.getInstance().getAccountDB(Token, null, 0, 0);
                Account Player = _client._player;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("***************************************************");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Player_ID: " + Player.player_id);
                Console.WriteLine("Player_Name: " + Player.player_name);
                Console.WriteLine("Player_Rank: " + Player._rank);
                Console.WriteLine("IP: " + PublicIP);
                Console.WriteLine("Version: " + GameVersion);
                Console.WriteLine("Date: " + DateTime.Now);
                Console.WriteLine("GameLocale: " + GameLocale);
                Console.WriteLine("***************************************************");
                ServerConfig cfg = AuthManager.Config;
                ServerConfig config = AuthManager.Config;
        if (config == null || !AuthConfig.isTestMode && !AuthConfig.GameLocales.Contains(this.GameLocale) || (this.Token.Length < AuthConfig.minTokenSize || this.GameVersion != config.ClientVersion))
        {
          string text = "";
          if (config == null)
            text = "Invalid server setting [" + this.Token + "]";
          else if (!AuthConfig.isTestMode && !AuthConfig.GameLocales.Contains(this.GameLocale))
            text = ((int) this.GameLocale).ToString() + " blocked [" + this.Token + "]";
          else if (this.Token.Length < AuthConfig.minTokenSize)
            text = "Token < Size [" + this.Token + "]";
          else if (this.GameVersion != config.ClientVersion)
            text = "Version: " + this.GameVersion + " not compatible [" + this.Token + "]";
          this._client.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_DISCONNECTIONSUCCESS_ACK(2147483904U, false));
          Logger.LogLogin(text);
          this._client.Close(1000, true);
        }
        else
        {
          this._client._player = AccountManager.getInstance().getAccountDB((object) this.Token, (object) null, 0, 0);
          if (this._client._player == null && AuthConfig.AUTO_ACCOUNTS && !AccountManager.getInstance().CreateAccount(out this._client._player, this.Token))
          {
            this._client.SendPacket((SendPacket) new PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum.LOGIN_DELETE_ACCOUNT, "", 0L));
            Logger.LogLogin("Failed to create account automatically");
            this._client.Close(1000, false);
          }
          else
          {
            Account player = this._client._player;
            if (player == null || !player.CompareToken(this.Token))
            {
              string str = "";
              if (player == null)
                str = "Invaild Account";
              else if (!player.CompareToken(this.Token))
                str = "Invalid Token";
              this._client.SendPacket((SendPacket) new PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum.LOGIN_INVALID_ACCOUNT, "", 0L));
              Logger.LogLogin(str + " [" + this.Token + "]");
              this._client.Close(1000, false);
            }
            else if (player.access >= 0)
            {
              if (player.MacAddress != this.MacAddress)
                ComDiv.updateDB("accounts", "last_mac", (object) this.MacAddress, "player_id", (object) player.player_id);
              bool validMac;
              bool validIp;
              BanManager.GetBanStatus(this.MacAddress.ToString(), this.PublicIP, out validMac, out validIp);
              if (validMac | validIp)
              {
                if (validMac)
                  Logger.LogLogin("Mac banned [" + player.login + "]");
                else
                  Logger.LogLogin("Ip banned [" + player.login + "]");
                this._client.SendPacket((SendPacket) new PROTOCOL_BASE_LOGIN_ACK(validIp ? EventErrorEnum.LOGIN_BLOCK_IP : EventErrorEnum.LOGIN_BLOCK_ACCOUNT, "", 0L));
                this._client.Close(1000, false);
              }
              else if (player.IsGM() && config.onlyGM || player.access >= 0 && !config.onlyGM)
              {
                Account account = AccountManager.getInstance().getAccount(player.player_id, true);
                if (!player._isOnline)
                {
                  if (BanManager.GetAccountBan(player.ban_obj_id).endDate > DateTime.Now)
                  {
                    this._client.SendPacket((SendPacket) new PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum.LOGIN_BLOCK_ACCOUNT, "", 0L));
                    Logger.LogLogin("Account with ban active [" + player.login + "]");
                    this._client.Close(1000, false);
                  }
                  else if (this.CheckHwId(account.hwid))
                  {
                    this._client.SendPacket((SendPacket) new PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum.LOGIN_BLOCK_ACCOUNT, "", 0L));
                    Logger.LogLogin("Ban HwId [" + player.login + "]");
                    this._client.Close(1000, false);
                  }
                  else
                  {
                    if (account != null)
                      account._connection = this._client;
                    player.SetPlayerId(player.player_id, 31);
                    player._clanPlayers = ClanManager.getClanPlayers(player.clan_id, player.player_id);
                    this._client.SendPacket((SendPacket) new PROTOCOL_BASE_LOGIN_ACK(0, player.login, player.player_id));
                    this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_GET_POINT_CASH_ACK(0, player._gp, player._money));
                    if (player.clan_id > 0)
                      this._client.SendPacket((SendPacket) new PROTOCOL_CS_MEMBER_INFO_ACK(player._clanPlayers));
                    player._status.SetData(uint.MaxValue, player.player_id);
                    player._status.updateServer((byte) 0);
                    player.setOnlineStatus(true);
                    SendRefresh.RefreshAccount(player, true);
                  }
                }
                else
                {
                  this._client.SendPacket((SendPacket) new PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum.LOGIN_ALREADY_LOGIN_WEB, "", 0L));
                  Logger.LogLogin("Online account [" + player.login + "]");
                  if (account != null && account._connection != null)
                  {
                    account.SendPacket((SendPacket) new PROTOCOL_AUTH_ACCOUNT_KICK_ACK(1));
                    account.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_ERROR_ACK(2147487744U));
                    account.Close(1000);
                  }
                  else
                    AuthSync.SendLoginKickInfo(player);
                  this._client.Close(1000, false);
                }
              }
              else
              {
                this._client.SendPacket((SendPacket) new PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum.LOGIN_TIME_OUT_2, "", 0L));
                Logger.LogLogin("Invalid Access [" + player.login + "]");
                this._client.Close(1000, false);
              }
            }
            else
            {
              this._client.SendPacket((SendPacket) new PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum.LOGIN_BLOCK_ACCOUNT, "", 0L));
              Logger.LogLogin("Permanent ban [" + player.login + "]");
              this._client.Close(1000, false);
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.warning("PROTOCOL_BASE_LOGIN_REQ: " + ex.ToString());
      }
    }

    public bool CheckHwId(string PlayerHwId)
    {
      foreach (string hwId in BanManager.GetHwIdList())
      {
        if ((PlayerHwId.Length != 0 || hwId.Length != 0 || (hwId != null || hwId == "")) && PlayerHwId == hwId)
          return true;
      }
      return false;
    }
  }
}
