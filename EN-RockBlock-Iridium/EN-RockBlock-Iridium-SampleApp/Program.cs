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

                var confs = isu.GetConfigurations();
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
