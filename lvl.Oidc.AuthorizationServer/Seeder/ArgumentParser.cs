using System;
using System.Collections.Generic;
using System.Linq;

namespace lvl.Oidc.AuthorizationServer.Seeder
{
    public class ArgumentParser
    {
        public OidcAuthorizationServerOptions Parse(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var connectionString = GetValue(args, "--connection-string");
            var seedTestData = GetFlag(args, "--seed-test-data");
            return new OidcAuthorizationServerOptions
            {
                ConnectionString = connectionString,
                SeedTestData = seedTestData
            };
        }

        public string GetValue(IReadOnlyList<string> args, string key)
        {
            for (var i = 0; i < args.Count; i++)
            {
                if (!args[i].Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (i == args.Count - 1 || args[i + 1].StartsWith("--"))
                {
                    throw new ArgumentException($"{key} was specified but had no value.");
                }

                return args[i + 1];
            }

            return null;
        }

        public bool GetFlag(IEnumerable<string> args, string key)
        {
            return args.Contains(key);
        }
    }
}
