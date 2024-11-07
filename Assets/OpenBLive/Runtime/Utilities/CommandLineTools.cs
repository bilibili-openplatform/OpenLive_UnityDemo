using System;


namespace OpenBLive.Runtime.Utilities
{
    public static class CommandLineTools
    {
        private const string k_CodeIdArgs = "code=";

        public static string GetCodeViaCmdLineArgs()
        {
            var code = "";
            var args = Environment.GetCommandLineArgs();
            foreach (var arg in args)
            {
                if (arg.Contains(k_CodeIdArgs))
                {
                    code = arg.Substring(k_CodeIdArgs.Length, arg.Length - k_CodeIdArgs.Length);
                }
            }
            return code;
        }
    }
}