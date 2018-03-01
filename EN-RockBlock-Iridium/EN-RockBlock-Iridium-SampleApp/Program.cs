using System;
using System.Collections.Generic;


namespace EN.RockBlockIridium.SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ISU isu = new ISU("COM3");
            
            try
            {
                isu.Open();
                Console.WriteLine(isu.SerialNumber);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:\n" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                isu.Close();
            }
            Console.ReadKey();
        }
    }
}
