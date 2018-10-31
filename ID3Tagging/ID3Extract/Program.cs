using ID3Tagging.ID3Extract;
using System;
using System.Data.SqlClient;
//using System.Data.SqlServerCe;
using System.IO;

namespace ID3Tagging.ED3Extract
{
    /// <summary>
    /// The program.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            DirScan dirScan = new DirScan();
            string[] files = dirScan.Browse(@"C:\Users\david.waidmann\Downloads\Productivity\");

            //using (SqlCeConnection connection = new SqlCeConnection("Data Source=Repository.sdf; Persist Security Info=False; Max Database Size=2000"))
            using (SqlConnection connection = new SqlConnection(""))
            {
                connection.Open();
                var repositoryAdapter = new ReopositoryAdapter(connection);
                foreach (string file in files)
                {
                    try
                    {
                        repositoryAdapter.PublishFrames(file);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Error Scanning:{0}", Path.GetFileName(file));
                        Console.WriteLine(e.Message);
                        Console.WriteLine();
                    }

                    Console.Write('.');
                }
            }
        }
    }
}
