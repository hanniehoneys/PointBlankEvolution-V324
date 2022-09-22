using System.Collections.Generic;
using System.Linq;

namespace PointBlank.Core.Models.Map
{
  public static class MapModel
  {
    public static List<MapRule> Rules = new List<MapRule>();
    public static List<MapMatch> Matchs = new List<MapMatch>();

    public static IEnumerable<IEnumerable<T>> Split<T>(
      this IEnumerable<T> list,
      int limit)
    {
      return list.Select((item, inx) => new { item, inx }).GroupBy(x => x.inx / limit).Select(g => g.Select(x => x.item));
    }

    public static MapRule getRule(int Mode)
    {
      for (int index = 0; index < MapModel.Rules.Count; ++index)
      {
        MapRule rule = MapModel.Rules[index];
        if (rule != null && rule.Id == Mode)
          return rule;
      }
      return (MapRule) null;
    }
  }
}
