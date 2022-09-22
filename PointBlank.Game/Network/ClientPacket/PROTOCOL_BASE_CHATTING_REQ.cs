using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Game.Data.Chat;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Utils;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Text;

namespace PointBlank.Game.Network.ClientPacket
{
    public class PROTOCOL_BASE_CHATTING_REQ : ReceivePacket
    {
        private string text;
        private ChattingType type;
        private bool SendAll;
        private bool SendMe;

        public PROTOCOL_BASE_CHATTING_REQ(GameClient client, byte[] data)
        {
            makeme(client, data);
        }

        public override void read()
        {
            type = (ChattingType)readH();
            text = readUnicode(readH() * 2);
            SendAll = false;
        }

        public override void run()
        {
            try
            {
                Account player = _client._player;
                if (player == null || string.IsNullOrEmpty(text) || text.Length > 60 || player.player_name.Length == 0)
                {
                    return;
                }
                Room room = player._room;
                Slot sender;
                switch (type)
                {
                    case ChattingType.Team:
                        {
                            if (room == null)
                            {
                                return;
                            }

                            sender = room._slots[player._slotId];
                            int[] array = room.GetTeamArray(sender._team);
                            using (PROTOCOL_ROOM_CHATTING_ACK packet = new PROTOCOL_ROOM_CHATTING_ACK((int)type, sender._id, player.UseChatGM(), text))
                            {
                                byte[] data = packet.GetCompleteBytes("PROTOCOL_BASE_CHATTING_REQ-1");
                                lock (room._slots)
                                {
                                    for (int i = 0; i < array.Length; i++)
                                    {
                                        int slotIdx = array[i];
                                        Slot receiver = room._slots[slotIdx];
                                        Account pR = room.getPlayerBySlot(receiver);
                                        if (pR != null && SlotValidMessage(sender, receiver))
                                        {
                                            pR.SendCompletePacket(data);
                                        }
                                    }
                                }
                            }
                            break;
                        }

                    case ChattingType.All:
                    case ChattingType.Lobby:
                        {
                            if (room != null)
                            {
                                if (!ServerCommands(player, room))
                                {
                                    sender = room._slots[player._slotId];
                                    if (SendAll)
                                    {
                                        for (int slotIdx = 0; slotIdx < 16; ++slotIdx)
                                        {
                                            Slot receiver = room._slots[slotIdx];
                                            Account pR = room.getPlayerBySlot(receiver);
                                            if (pR != null && SlotValidMessage(sender, receiver))
                                            {
                                                pR.SendPacket(new PROTOCOL_LOBBY_CHATTING_ACK("Room", 0, 5, false, text));
                                            }
                                        }
                                    }
                                    else if (SendMe)
                                    {
                                        _client.SendPacket(new PROTOCOL_LOBBY_CHATTING_ACK("Room", 0, 5, false, text));
                                    }
                                    else
                                    {
                                        using (PROTOCOL_ROOM_CHATTING_ACK packet = new PROTOCOL_ROOM_CHATTING_ACK((int)type, sender._id, player.UseChatGM(), text))
                                        {
                                            byte[] data = packet.GetCompleteBytes("PROTOCOL_BASE_CHATTING_REQ-2");
                                            lock (room._slots)
                                            {
                                                for (int slotIdx = 0; slotIdx < 16; ++slotIdx)
                                                {
                                                    Slot receiver = room._slots[slotIdx];
                                                    Account pR = room.getPlayerBySlot(receiver);
                                                    if (pR != null && SlotValidMessage(sender, receiver))
                                                    {
                                                        pR.SendCompletePacket(data);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {

                                    _client.SendPacket(new PROTOCOL_ROOM_CHATTING_ACK((int)type, player._slotId, true, text));
                                }
                            }
                            else
                            {
                                Channel channel = player.getChannel();
                                if (channel == null)
                                {
                                    return;
                                }

                                if (!ServerCommands(player, room))
                                {
                                    using (PROTOCOL_LOBBY_CHATTING_ACK packet = new PROTOCOL_LOBBY_CHATTING_ACK(player, text))
                                    {
                                        channel.SendPacketToWaitPlayers(packet);
                                    }
                                }
                                else
                                {
                                    _client.SendPacket(new PROTOCOL_LOBBY_CHATTING_ACK(player, text, true));
                                }
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Logger.warning(ex.ToString());
            }
        }

        private bool ServerCommands(Account player, Room room)
        {
            try
            {
                string str = text.Substring(1);

                // Command Player
                if (!(text.StartsWith(";") || text.StartsWith(@"\") || text.StartsWith(".")))
                {

                    if (str.StartsWith("refill ") || str.StartsWith("refill"))
                    {
                        text = RefillManager.RefillPlayer(str);
                    }

                    if (room != null)
                    {
                        if (str.StartsWith("NS") && player.access >= 0 && !room.GameRuleActive)
                        {
                            if (room.getLeader() == player)
                            {
                                if (!room.SniperMode && !room.ShotgunMode)
                                {
                                    SendAll = true;
                                    if (room.ShotgunActive)
                                    {
                                        room.ShotgunActive = false;
                                        room.RuleFlag -= 2;
                                        text = "ปิดใช้งานโหมดห้ามใช้ ปืนลูกซอง";
                                    }
                                    else
                                    {
                                        room.ShotgunActive = true;
                                        room.RuleFlag += 2;
                                        text = "เปิดใช้งานโหมดห้ามใช้ ปืนลูกซอง";
                                    }
                                }
                                else
                                {
                                    SendMe = true;
                                    text = "ไม่สามารถใช้งานในโหมดนี้ได้";
                                }
                            }
                            else
                            {
                                SendMe = true;
                                text = "คุณไม่ใช่หัวห้องไม่สามารถใช้คำสั่งนี้ได้";
                            }
                        }
                        else if (str.StartsWith("NS") && player.access >= 0 && room.GameRuleActive)
                        {
                            if (room.getLeader() == player)
                            {
                                SendMe = true;
                                text = "คุณไม่สามารถใช้งานได้เนื่องจากคุณเปิดใช้กฎแข่งแล้ว";
                            }
                            else
                            {
                                SendMe = true;
                                text = "คุณไม่ใช่หัวห้องไม่สามารถใช้คำสั่งนี้ได้";
                            }
                        }
                        if (str.StartsWith("NB") && player.access >= 0 && !room.GameRuleActive)
                        {
                            if (room.getLeader() == player)
                            {
                                if (!room.ShotgunMode)
                                {
                                    SendAll = true;
                                    if (room.BarrettActive)
                                    {
                                        room.BarrettActive = false;
                                        room.RuleFlag -= 1;
                                        text = "ปิดใช้งานโหมดห้ามใช้ ปืน Barrett M82A1 ทุกชนิด";
                                    }
                                    else
                                    {
                                        room.BarrettActive = true;
                                        room.RuleFlag += 1;
                                        text = "เปิดใช้งานโหมดห้ามใช้ ปืน Barrett M82A1 ทุกชนิด";
                                    }
                                }
                                else
                                {
                                    SendMe = true;
                                    text = "ไม่สามารถใช้งานในโหมดนี้ได้";
                                }
                            }
                            else
                            {
                                SendMe = true;
                                text = "คุณไม่ใช่หัวห้องไม่สามารถใช้คำสั่งนี้ได้";
                            }
                        }
                        else if (str.StartsWith("NB") && player.access >= 0 && room.GameRuleActive)
                        {
                            if (room.getLeader() == player)
                            {
                                SendMe = true;
                                text = "คุณไม่สามารถใช้งานได้เนื่องจากคุณเปิดใช้กฎแข่งแล้ว";
                            }
                            else
                            {
                                SendMe = true;
                                text = "คุณไม่ใช่หัวห้องไม่สามารถใช้คำสั่งนี้ได้";
                            }
                        }
                        if (str.StartsWith("NM") && player.access >= 0 && !room.GameRuleActive)
                        {
                            if (room.getLeader() == player)
                            {
                                SendAll = true;
                                if (room.MaskActive)
                                {
                                    room.MaskActive = false;
                                    room.RuleFlag -= 4;
                                    text = "ปิดใช้งานโหมดห้ามใช้ หน้ากาก";
                                }
                                else
                                {
                                    room.MaskActive = true;
                                    room.RuleFlag += 4;
                                    text = "เปิดใช้งานโหมดห้ามใช้ หน้ากาก";
                                }
                            }
                            else
                            {
                                SendMe = true;
                                text = "คุณไม่ใช่หัวห้องไม่สามารถใช้คำสั่งนี้ได้";
                            }
                        }
                        else if (str.StartsWith("NM") && player.access >= 0 && room.GameRuleActive)
                        {
                            if (room.getLeader() == player)
                            {
                                SendMe = true;
                                text = "คุณไม่สามารถใช้งานได้เนื่องจากคุณเปิดใช้กฎแข่งแล้ว";
                            }
                            else
                            {
                                SendMe = true;
                                text = "คุณไม่ใช่หัวห้องไม่สามารถใช้คำสั่งนี้ได้";
                            }
                        }
                        if (str.StartsWith("GR") && player.access >= 0 && (!room.BarrettActive && !room.ShotgunActive && !room.MaskActive))
                        {
                            if (room.getLeader() == player)
                            {
                                SendAll = true;
                                if (room.GameRuleActive)
                                {
                                    room.GameRuleActive = false;
                                    room.RuleFlag -= 8;
                                    text = "ปิดใช้งานโหมดกฎแข่ง";
                                    room.updateSlotsInfo();
                                }
                                else
                                {
                                    room.GameRuleActive = true;
                                    room.RuleFlag += 8;
                                    text = "เปิดใช้งานโหมดกฎแข่ง";
                                    room.updateSlotsInfo();
                                }
                            }
                            else
                            {
                                SendMe = true;
                                text = "คุณไม่ใช่หัวห้องไม่สามารถใช้คำสั่งนี้ได้";
                            }
                        }
                        else if (str.StartsWith("GR") && player.access >= 0 && (room.BarrettActive || room.ShotgunActive || room.MaskActive) && !room.RuleFlag.HasFlag(GameRuleFlag.กฎแข่ง))
                        {
                            if (room.getLeader() == player)
                            {
                                SendMe = true;
                                text = "คุณไม่สามารถใช้งานได้เนื่องจากคุณเปิดใช้: " + room.RuleFlag.ToString();
                            }
                            else
                            {
                                SendMe = true;
                                text = "คุณไม่ใช่หัวห้องไม่สามารถใช้คำสั่งนี้ได้";
                            }
                        }
                    }
                }

                // Command Admin
                if (!player.HaveGMLevel() || !(text.StartsWith(";") || text.StartsWith(@"\") || text.StartsWith(".")))
                {
                    return false;
                }
                Logger.LogCMD("[" + text + "] PlayerId: " + player.player_id + " Nick: '" + player.player_name + "' Login: '" + player.login + "' Ip: '" + player.PublicIP.ToString() + "' Date: '" + DateTime.Now.ToString("dd/MM/yy HH:mm") + "'");
                if (str.StartsWith("help3") && (int)player.access >= 3)
                {
                    text = HelpCommandList.GetList3(player);
                }
                else if (str.StartsWith("help4") && (int)player.access >= 3)
                {
                    text = HelpCommandList.GetList4(player);
                }
                else if (str.StartsWith("help5") && (int)player.access >= 3)
                {
                    text = HelpCommandList.GetList5(player);
                }
                else if (str.StartsWith("help6") && (int)player.access >= 3)
                {
                    text = HelpCommandList.GetList6(player);
                }

                //.Help3
                else if (str.StartsWith("nickh1 ") && (int)player.access >= 3)
                {
                    text = NickHistory.GetHistoryById(str, player);
                }
                else if (str.StartsWith("nickh2 ") && (int)player.access >= 3)
                {
                    text = NickHistory.GetHistoryByNewNick(str, player);
                }
                else if (str.StartsWith("fakerank ") && (int)player.access >= 3)
                {
                    text = GMDisguises.SetFakeRank(str, player, room);
                }
                else if (str.StartsWith("changenick ") && (int)player.access >= 3)
                {
                    text = GMDisguises.SetFakeNick(str, player, room);
                }
                else if (str.StartsWith("kp ") && (int)player.access >= 3)
                {
                    text = KickPlayer.KickByNick(str, player);
                }
                else if (str.StartsWith("kp2 ") && (int)player.access >= 3)
                {
                    text = KickPlayer.KickById(str, player);
                }
                else if (str.StartsWith("hcn") && (int)player.access >= 3)
                {
                    text = GMDisguises.SetHideColor(player);
                }
                else if (str.StartsWith("antikick") && (int)player.access >= 3)
                {
                    text = GMDisguises.SetAntiKick(player);
                }
                else if (str.StartsWith("roomunlock ") && (int)player.access >= 3)
                {
                    text = ChangeRoomInfos.UnlockById(str, player);
                }
                else if (str.StartsWith("afkcount ") && (int)player.access >= 3)
                {
                    text = AFKInteraction.GetAFKCount(str);
                }
                else if (str.StartsWith("afkkick ") && (int)player.access >= 3)
                {
                    text = AFKInteraction.KickAFKPlayers(str);
                }
                else if (str.StartsWith("players1") && (int)player.access >= 3)
                {
                    text = PlayersCountInServer.GetMyServerPlayersCount();
                }
                else if (str.StartsWith("players2 ") && (int)player.access >= 3)
                {
                    text = PlayersCountInServer.GetServerPlayersCount(str);
                }
                else if (str.StartsWith("ping") && (int)player.access >= 3)
                {
                    text = LatencyAnalyze.StartAnalyze(player, room);
                }

                //.Help4
                else if (str.StartsWith("send ") && (int)player.access >= 4)
                {
                    text = SendMsgToPlayers.SendToAll(str);
                }
                else if (str.StartsWith("sendr ") && (int)player.access >= 4)
                {
                    text = SendMsgToPlayers.SendToRoom(str, room);
                }
                else if (str.StartsWith("map ") && (int)player.access >= 4)
                {
                    text = ChangeRoomInfos.ChangeMap(str, room);
                }
                else if (str.StartsWith("t ") && (int)player.access >= 4)
                {
                    text = ChangeRoomInfos.ChangeTime(str, room);
                }
                else if (str.StartsWith("cp ") && (int)player.access >= 4)
                {
                    text = SendCashToPlayer.SendByNick(str);
                }
                else if (str.StartsWith("cp2 ") && (int)player.access >= 4)
                {
                    text = SendCashToPlayer.SendById(str);
                }
                else if (str.StartsWith("gp ") && (int)player.access >= 4)
                {
                    text = SendGoldToPlayer.SendByNick(str);
                }
                else if (str.StartsWith("gp2 ") && (int)player.access >= 4)
                {
                    text = SendGoldToPlayer.SendById(str);
                }
                else if (str.StartsWith("ka") && (int)player.access >= 4)
                {
                    text = KickAllPlayers.KickPlayers();
                }
                else if (str.StartsWith("gift ") && (int)player.access >= 4)
                {
                    text = SendGiftToPlayer.SendGiftById(str);
                }
                else if (str.StartsWith("goods ") && (int)player.access >= 4)
                {
                    text = ShopSearch.SearchGoods(str, player);
                }
                else if (str.StartsWith("banS ") && (int)player.access >= 4)
                {
                    text = Ban.BanNormalNick(str, player, true);
                }
                else if (str.StartsWith("banS2 ") && (int)player.access >= 4)
                {
                    text = Ban.BanNormalId(str, player, true);
                }
                else if (str.StartsWith("banA ") && (int)player.access >= 4)
                {
                    text = Ban.BanNormalNick(str, player, false);
                }
                else if (str.StartsWith("banA2 ") && (int)player.access >= 4)
                {
                    text = Ban.BanNormalId(str, player, false);
                }
                else if (str.StartsWith("unb ") && (int)player.access >= 4)
                {
                    text = UnBan.UnbanByNick(str, player);
                }
                else if (str.StartsWith("unb2 ") && (int)player.access >= 4)
                {
                    text = UnBan.UnbanById(str, player);
                }
                else if (str.StartsWith("reason ") && (int)player.access >= 4)
                {
                    text = Ban.UpdateReason(str);
                }
                else if (str.StartsWith("getip ") && (int)player.access >= 4)
                {
                    text = GetAccountInfo.getByIPAddress(str, player);
                }
                else if (str.StartsWith("get1 ") && (int)player.access >= 4)
                {
                    text = GetAccountInfo.getById(str, player);
                }
                else if (str.StartsWith("get2 ") && (int)player.access >= 4)
                {
                    text = GetAccountInfo.getByNick(str, player);
                }
                else if (str.StartsWith("open1 ") && (int)player.access >= 4)
                {
                    text = OpenRoomSlot.OpenSpecificSlot(str, player, room);
                }
                else if (str.StartsWith("open2 ") && (int)player.access >= 4)
                {
                    text = OpenRoomSlot.OpenRandomSlot(str, player);
                }
                else if (str.StartsWith("open3 ") && (int)player.access >= 4)
                {
                    text = OpenRoomSlot.OpenAllSlots(str, player);
                }
                else if (str.StartsWith("taketitles") && (int)player.access >= 4)
                {
                    text = TakeTitles.GetAllTitles(player);
                }

                //.Help5
                else if (str.StartsWith("changerank ") && (int)player.access >= 5)
                {
                    text = ChangePlayerRank.SetPlayerRank(str);
                }
                else if (str.StartsWith("banSE ") && (int)player.access >= 5)
                {
                    text = Ban.BanForeverNick(str, player, true);
                }
                else if (str.StartsWith("banSE2 ") && (int)player.access >= 5)
                {
                    text = Ban.BanForeverId(str, player, true);
                }
                else if (str.StartsWith("banAE ") && (int)player.access >= 5)
                {
                    text = Ban.BanForeverNick(str, player, false);
                }
                else if (str.StartsWith("banAE2 ") && (int)player.access >= 5)
                {
                    text = Ban.BanForeverId(str, player, false);
                }
                else if (str.StartsWith("getban ") && (int)player.access >= 5)
                {
                    text = Ban.GetBanData(str, player);
                }
                else if (str.StartsWith("sunb ") && (int)player.access >= 5)
                {
                    text = UnBan.SuperUnbanByNick(str, player);
                }
                else if (str.StartsWith("sunb2 ") && (int)player.access >= 5)
                {
                    text = UnBan.SuperUnbanById(str, player);
                }
                else if (str.StartsWith("ci ") && (int)player.access >= 5)
                {
                    text = CreateItem.CreateItemYourself(str, player);
                }
                else if (str.StartsWith("cia ") && (int)player.access >= 5)
                {
                    text = CreateItem.CreateItemByNick(str, player);
                }
                else if (str.StartsWith("cid ") && (int)player.access >= 5)
                {
                    text = CreateItem.CreateItemById(str, player);
                }
                else if (str.StartsWith("cgid ") && (int)player.access >= 5)
                {
                    text = CreateItem.CreateGoldCupom(str);
                }
                else if (str.StartsWith("refillshop") && (int)player.access >= 5)
                {
                    text = RefillShop.SimpleRefill(player);
                }
                else if (str.StartsWith("refill2shop") && (int)player.access >= 5)
                {
                    text = RefillShop.InstantRefill(player);
                }
                /*else if (str.StartsWith("upchan ") && (int)player.access >= 5)
                {
                    text = ChangeChannelNotice.SetChannelNotice(str);
                }
                else if (str.StartsWith("upach ") && (int)player.access >= 5)
                {
                    text = ChangeChannelNotice.SetAllChannelsNotice(str);
                }
                else if (str.StartsWith("setgold ") && (int)player.access >= 5)
                {
                    text = SetGoldToPlayer.SetGdToPlayer(str);
                }
                else if (str.StartsWith("setcash ") && (int)player.access >= 5)
                {
                    text = SetCashToPlayer.SetCashPlayer(str);
                }
                else if (str.StartsWith("gpd ") && (int)player.access >= 5)
                {
                    text = SendGoldToPlayerDev.SendGoldToPlayer(str);
                }
                else if (str.StartsWith("cpd ") && (int)player.access >= 5)
                {
                    text = SendCashToPlayerDev.SendCashToPlayer(str);
                }
                else if (str.StartsWith("setvip ") && (int)player.access >= 5)
                {
                    text = SetVipToPlayer.SetVipPlayer(str);
                }
                else if (str.StartsWith("setacess ") && (int)player.access >= 5)
                {
                    text = SetAcessToPlayer.SetAcessPlayer(str);
                }*/
                /*else if (str.StartsWith("sendtitles") && (int)player.access >= 5)
                {
                    text = SendTitleToPlayer.SendTitlePlayer(str);
                }*/

                //.Help 6               
                else if (str.StartsWith("end") && (int)player.access >= 6)
                {
                    if (room != null)
                    {
                        if (room.isPreparing())
                        {
                            AllUtils.EndBattle(room);
                            text = Translation.GetLabel("EndRoomSuccess");
                        }
                        else
                        {
                            text = Translation.GetLabel("EndRoomFail1");
                        }
                    }
                    else
                    {
                        text = Translation.GetLabel("GeneralRoomInvalid");
                    }
                }
                else if (str.StartsWith("newroomtype ") && (int)player.access >= 6)
                {
                    text = ChangeRoomInfos.ChangeStageType(str, room);
                }
                else if (str.StartsWith("newroomweap ") && (int)player.access >= 6)
                {
                    text = ChangeRoomInfos.ChangeWeaponsFlag(str, room);
                }
                /*else if (str.StartsWith("udp ") && (int)player.access >= 6)
                {
                    text = ChangeUdpType.SetUdpType(str);
                }
                else if (str.StartsWith("testmode") && (int)player.access >= 6)
                {
                    text = ChangeServerMode.EnableTestMode();
                }
                else if (str.StartsWith("publicmode") && (int)player.access >= 6)
                {
                    text = ChangeServerMode.EnablePublicMode();
                }
                else if (str.StartsWith("activeM ") && (int)player.access >= 6)
                {
                    text = EnableMissions.genCode1(str, player);
                }*/
                else
                {
                    text = Translation.GetLabel("UnknownCmd");
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.warning("PROTOCOL_BASE_CHATTING_REQ: " + ex.ToString());
                text = Translation.GetLabel("CrashProblemCmd");
                return true;
            }
        }

        private bool SlotValidMessage(Slot sender, Slot receiver)
        {
            return (((int)sender.state == 8 || (int)sender.state == 9) && ((int)receiver.state == 8 || (int)receiver.state == 9) ||
                    ((int)sender.state >= 10 && (int)receiver.state >= 10) && (receiver.specGM ||
                    sender.specGM ||
                    sender._deathState.HasFlag(DeadEnum.UseChat) ||
                    sender._deathState.HasFlag(DeadEnum.Dead) && receiver._deathState.HasFlag(DeadEnum.Dead) ||
                    sender.espectador && receiver.espectador ||
                    sender._deathState.HasFlag(DeadEnum.Alive) && receiver._deathState.HasFlag(DeadEnum.Alive) && (sender.espectador && receiver.espectador || !sender.espectador && !receiver.espectador)));
        }
    }
}

namespace PointBlank.Game
{
    class RefillManager
    {
        internal static string RefillPlayer(string str)
        {
            throw new NotImplementedException();
        }
    }
}