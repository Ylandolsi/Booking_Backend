using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Options;

public sealed class EmailOptions
{
    public const string EmailOptionsKey = "Email";
    public string SenderEmail { get; set; } = string.Empty;
}
