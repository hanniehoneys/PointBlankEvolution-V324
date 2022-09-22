using Npgsql;
using PointBlank.Auth.Data.Model;
using PointBlank.Core;
using PointBlank.Core.Network;
using PointBlank.Core.Sql;
using System;
using System.Collections.Generic;

namespace PointBlank.Auth.Data.Xml
{
  public static class ChannelsXml
  {
    public static List<Channel> _channels = new List<Channel>();

    public static void Load(int serverId)
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@server", (object) serverId);
          command.CommandText = "SELECT * FROM info_channels WHERE server_id=@server ORDER BY channel_id ASC";
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
            ChannelsXml._channels.Add(new Channel()
            {
              serverId = npgsqlDataReader.GetInt32(0),
              _id = npgsqlDataReader.GetInt32(1),
              _type = npgsqlDataReader.GetInt32(2)
            });
          command.Dispose();
          npgsqlDataReader.Close();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
    }

    public static Channel getChannel(int id)
    {
      try
      {
        return ChannelsXml._channels[id];
      }
      catch
      {
        return (Channel) null;
      }
    }

    public static List<Channel> getChannels(int ServerId)
    {
      List<Channel> channelList = new List<Channel>();
      for (int index = 0; index < ChannelsXml._channels.Count; ++index)
      {
        Channel channel = ChannelsXml._channels[index];
        if (channel.serverId == ServerId)
          channelList.Add(channel);
      }
      return channelList;
    }

    public static bool updateNotice(int serverId, int channelId, string text)
    {
      return ComDiv.updateDB("info_channels", "announce", (object) text, "server_id", (object) serverId, "channel_id", (object) channelId);
    }

    public static bool updateNotice(string text)
    {
      return ComDiv.updateDB("info_channels", "announce", (object) text);
    }
  }
}
