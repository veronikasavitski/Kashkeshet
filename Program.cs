using System;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientBootstrapper bootstrapper = new ClientBootstrapper();
            bootstrapper.Start();
        }
    }
}
