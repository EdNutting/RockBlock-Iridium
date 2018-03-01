using System;
using System.Collections.Generic;


namespace EN.RockBlockIridium.SampleApp
{
    class Program
    {
        static ISU isu;

        static void Main(string[] args)
        {
            isu = new ISU("COM3");

            isu.OnAutoRegistration += Isu_OnAutoRegistration;
            isu.OnSBDRing += Isu_OnSBDRing;
            isu.OnServiceAvailabilityChange += Isu_OnServiceAvailabilityChange;

            try
            {
                isu.Open();
                
                isu.SetIndicatorEventReporting(new IndicatorEventReporting(true, true, true));
                var data = isu.GetIndicatorEventReporting().Data;
                Console.WriteLine(data.Enable);
                Console.WriteLine(data.ReportServiceAvailabilityChanges);
                Console.WriteLine(data.ReportSignalQuality);
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

        private static void Isu_OnServiceAvailabilityChange(object sender, List<string> e)
        {
            isu.PopOldestLines();

            Console.WriteLine("Service availability: " + e[0]);
        }

        private static void Isu_OnSBDRing(object sender, List<string> e)
        {
            isu.PopOldestLines();

            Console.WriteLine("SBD Ring: " + e[0]);
        }

        private static void Isu_OnAutoRegistration(object sender, List<string> e)
        {
            isu.PopOldestLines();

            Console.WriteLine("Auto registration: " + e[0]);
        }
    }
}
