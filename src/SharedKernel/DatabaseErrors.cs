using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel;

public static class DatabaseErrors
{
    public static Error SaveChangeError( string details ) => Error.Failure("Database.SaveChanges", details);
}
