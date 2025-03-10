using Dapper;
using DirectInserttoDB.Models;
using Npgsql;



namespace DirectInserttoDB
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            Console.Write("number of items to be inserted : ");
            int input = int.Parse(Console.ReadLine());


            Console.Write("start?");
            Console.ReadLine();


            //Dbinserttest dbinserttest = new Dbinserttest();
            //await dbinserttest.Start();

            DBBulkinsert dBinsert = new DBBulkinsert();
            await dBinsert.Start(input);
          
            

            Console.WriteLine("done");

           










        }
    }
}
