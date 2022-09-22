using System.Text;

namespace PointBlank.Core.Network
{
  public class StringUtil
  {
    private static StringBuilder builder;

    public StringUtil()
    {
      StringUtil.builder = new StringBuilder();
    }

    public void AppendLine(string text)
    {
      StringUtil.builder.AppendLine(text);
    }

    public string getString()
    {
      if (StringUtil.builder.Length == 0)
        return StringUtil.builder.ToString();
      return StringUtil.builder.Remove(StringUtil.builder.Length - 1, 1).ToString();
    }
  }
}
