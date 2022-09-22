using System.Collections.Generic;

namespace PointBlank.Battle.Data.Models
{
  public class MapModel
  {
    public List<ObjectModel> Objects = new List<ObjectModel>();
    public List<BombPosition> Bombs = new List<BombPosition>();
    public int Id;

    public BombPosition GetBomb(int BombId)
    {
      try
      {
        return this.Bombs[BombId];
      }
      catch
      {
        return (BombPosition) null;
      }
    }
  }
}
