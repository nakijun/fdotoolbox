using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core;
using FdoToolbox.Core.ClientServices;

namespace FdoQuery
{
    public class FdoQueryApp : ConsoleApplication
    {
        public override void ParseArguments(string[] args)
        {
            string provider = GetArgument("-provider", args);
            string connStr = GetArgument("-connection", args);
            ThrowIfEmpty(provider, "-provider");
            ThrowIfEmpty(connStr, "-connection");

            if (IsSwitchDefined("-sql", args))
            {
                string sql = GetArgument("-sql", args);
                ThrowIfEmpty(sql, "-sql");
                _Command = new SqlQueryCommand(provider, connStr, sql);
            }
            else if (IsSwitchDefined("-class", args))
            {
                string className = GetArgument("-class", args);
                string filter = GetArgument("-filter", args);
                string propertyList = GetArgument("-properties", args);
                string strlimit = GetArgument("-limit", args);
                ThrowIfEmpty(className, "-class");
                List<string> propNames = new List<string>();
                if (!string.IsNullOrEmpty(propertyList))
                {
                    string[] names = propertyList.Split(',');
                    foreach (string n in names)
                    {
                        propNames.Add(n.Trim());
                    }
                }
                int limit = -1;
                if (!string.IsNullOrEmpty(strlimit) && int.TryParse(strlimit, out limit))
                {
                    limit = Convert.ToInt32(strlimit);
                }
                else
                {
                    if (!string.IsNullOrEmpty(strlimit))
                        throw new ArgumentException("-limit value is not a number");
                }
                _Command = new FeatureQueryCommand(provider, connStr, className, filter, propNames.AsReadOnly(), limit);
            }
            else
            {
                throw new ArgumentException("One or more required parameters are missing");
            }
        }

        public override void ShowUsage()
        {
            string usage =
@"Usage: FdoQuery.exe -provider:<provider name> -connection:<connection string>
<feature query parameters|sql query parameters>

Additional parameters for feature query:

-class:<class name> -filter:<filter> [-properties:<property names
(comma-separated)>] [-limit:<number of features to return>]

Additional parameters for sql query:

-sql:<sql text>

If the user wishes to save the feature query results to another source, the
following additional parameters can be supplied:

-outfmt:<format>
-out_connection:<connection string>

<format> can be any of the following:

- CSV
- Any FDO provider (use provider name, eg. OSGeo.SDF)

<connection string> can be any of the following:

- Path to the CSV file (if -outfmt:CSV)
- The FDO connection string (if -outfmt:<FDO Provider name>)

In CSV mode, geometry properties will be outputted as FGF text

For more help. Consult the help file cmd_readme.txt";
            AppConsole.WriteLine(usage);
        }
    }
}
