﻿using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using AD.IO;

namespace AD.PartialEquilibriumApi.Example
{
    public static class Program
    {
        public static void Main()
        {
            Example.Example1();
            return;

            XmlFilePath structureFile = CreateTempXmlFile();
            DelimitedFilePath dataFile = CreateTempCsvFile();

            XElement[] versions = new XElement[]
            {
                ExampleFromFiles(structureFile, dataFile),
                ExampleFromXElement(dataFile)
            };

            foreach (XElement model in versions)
            {
                // Print the results.
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    NewLineOnAttributes = true
                };
                using (XmlWriter writer = XmlWriter.Create(Console.Out, settings))
                {
                    model.WriteTo(writer);
                }
                Console.WriteLine();
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.ReadLine();
            }
            Console.ReadLine();
        }

        private static XElement ExampleFromFiles(XmlFilePath structureFile, DelimitedFilePath dataFile)
        {
            return XElement.Load(structureFile)
                           .DefineAttributeData(dataFile)
                           .ShockAllPrices()
                           .CalculatePriceIndex()
                           .CalculateRootMarketEquilibrium();
        }

        private static XElement ExampleFromXElement(DelimitedFilePath dataFile)
        {
            // Define the product market.
            XElement usaBeef = new XElement("usaBeef").DefineAttributeData(dataFile, 0);

            // Define the supplier markets and the initial prices of purchase from those markets.
            XElement usa = new XElement("usa").DefineAttributeData(dataFile, 1);
            XElement can = new XElement("can").DefineAttributeData(dataFile, 2);
            XElement mex = new XElement("mex").DefineAttributeData(dataFile, 3);
            XElement aus = new XElement("aus").DefineAttributeData(dataFile, 4);

            // Add the supplier markets to the product market. This has the effect of splitting the product market into product supplied by the supplier markets.
            usaBeef.Add(usa, can, mex, aus);

            // Set the current prices
            usaBeef.SetCurrentPrices(usaBeef.DescendantsAndSelf().Select(x => x.InitialPrice()).ToArray());

            // Apply the price shocks
            usaBeef.ShockAllPrices();

            // Calculate the price indices 
            usaBeef.CalculatePriceIndex();

            // Calculate the market equilibrium
            usaBeef.CalculateRootMarketEquilibrium();

            bool[] variables = 
                new bool[]
                {
                    false,
                    true,
                    true,
                    true,
                    true
                };

            // Optimize
            Func<double[], double> function =
                x =>
                {
                    usaBeef.SetCurrentPrices(x, variables);
                    usaBeef.ShockAllPrices();
                    usaBeef.CalculatePriceIndex();
                    usaBeef.CalculateRootMarketEquilibrium();
                    return usaBeef.MarketEquilibrium();
                };

            Simplex simplex = new Simplex(5, 5, 0, 100, 1000, x => function(x));

            double[] result = simplex.Minimize().Vector;

            usaBeef.SetCurrentPrices(result, variables);
            usaBeef.ShockAllPrices();
            usaBeef.CalculatePriceIndex();
            usaBeef.CalculateRootMarketEquilibrium();
            
            return usaBeef;
        }

        private static XmlFilePath CreateTempXmlFile()
        {
            string xml = Path.ChangeExtension(Path.GetTempFileName(), ".xml");
            using (StreamWriter writer = new StreamWriter(xml))
            {
                writer.WriteLine(
                    @"<usaBeef>
                        <usa/>
                        <can/>
                        <mex/>
                        <aus/>
                      </usaBeef>");
            }
            return new XmlFilePath(xml);
        }

        private static DelimitedFilePath CreateTempCsvFile()
        {
            string csv = Path.ChangeExtension(Path.GetTempFileName(), ".csv");
            using (StreamWriter writer = new StreamWriter(csv))
            {
                writer.WriteLine("ElasticityOfSubstitution,ElasticityOfSupply,ElasticityOfDemand,InitialPrice,CurrentPrice,MarketShare,Shock");
                writer.WriteLine("4,5,-1,1.0000000000000000,1.0000000000000000,1.0000000000000000,0.0000000000000000");
                writer.WriteLine("4,5,-1,1.0000000000000000,0.9764852913975930,0.0164876157540142,0.0435080979193930");
                writer.WriteLine("4,5,-1,1.0000000000000000,1.0000000000000000,0.1826886798599640,0.0000000000000000");
                writer.WriteLine("4,5,-1,1.0000000000000000,1.0000000000000000,0.0747428059044746,0.0000000000000000");
                writer.WriteLine("4,5,-1,1.0000000000000000,1.0000000000000000,0.7260808984815470,0.0000000000000000");
            }
            return new DelimitedFilePath(csv, ',');
        }
    }
}