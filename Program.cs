using System;

namespace Databaseopgave
{
    class Program
    {
        static void Main(string[] args)
        {
            DBClient dbc = new DBClient();
            dbc.Start();
        }
    }
}
