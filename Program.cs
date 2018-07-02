using System;

namespace jsonplugin
{
    class Program
    {
        static void Main(string[] args)
        {
            new ReponseParser().HandleCmd(args);
        }
    }
}