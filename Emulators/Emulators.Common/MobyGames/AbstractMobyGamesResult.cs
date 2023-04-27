using Emulators.Common.WebRequests;
using System.Net;

namespace Emulators.Common.MobyGames
{
  abstract class AbstractMobyGamesResult : IHtmlDeserializable
  {
    public abstract bool Deserialize(string response);
        
    protected string Decode(string input)
    {
      return WebUtility.HtmlDecode(input).Trim();
    }
  }
}
