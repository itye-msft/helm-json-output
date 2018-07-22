using System;

namespace HelmJsonOutput
{
    class Program
    {
        static void Main(string[] args)
        {
            new ReponseParser().ProcessCmd(args);
        }
    }
}