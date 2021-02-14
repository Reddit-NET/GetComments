using System;

namespace GetRedditComments
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: GetRedditComments <Reddit App ID> <Reddit Refresh Token> [Reddit Access Token]");
            }
            else
            {
                Workflow workflow = new Workflow(args);
                workflow.Run();
            }
        }
    }
}
