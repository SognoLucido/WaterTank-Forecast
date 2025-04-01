using System.Text;



namespace DirectInserttoDB
{
    internal class Program
    {

        enum DbMode
        {
            MultipleInsertions,
            Custom
        }


        static async Task<int> Main()
        {
            DbMode? mode = null;
            StringBuilder sb = new();
            int counter = 1;

            var modes = Enum.GetValues<DbMode>();
            if (modes is null || modes.Length == 0) return 0;

            foreach (var item in modes)
            {
                sb.Append($".{counter++} {item} ");
            }

            sb.Length--;

            while (true)
            {

                Console.Write($"Choose mode ({sb}): ");

                string? inputMode = Console.ReadLine();
                if (int.TryParse(inputMode, out int result))
                {
                    --result;
                    if(result <= modes.Length)
                    {
                        mode = (DbMode)result;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("invalid mode");
                    }
                }
                else Console.WriteLine("invalid mode");


            }
            Console.Clear();


            switch (mode)
            {
                case DbMode.MultipleInsertions: await new DBBulkinsert().Start(); break;
                case DbMode.Custom: await new DbCustomInsert().Start(); break;
                default: throw new NotImplementedException();
            }


            //Console.Write("number of items to be inserted : ");
            //int input = int.Parse(Console.ReadLine());


            //Console.Write("start?");
            //Console.ReadLine();


            ////DbCustomInsert dbinserttest = new DbCustomInsert();
            ////await dbinserttest.Start();

            //DBBulkinsert dBinsert = new DBBulkinsert();
            //await dBinsert.Start(input);



            Console.WriteLine("done");


            return 0;



        }


       




    }
}
