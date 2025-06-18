using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Options;

public  class GoogleOAuthOptions
{
    public const string GoogleOptionsKey = "Google"; // ## name in appsettings.json

    public string ClientId { get; set; }
    public string ClientSecret { get; set; }

}
