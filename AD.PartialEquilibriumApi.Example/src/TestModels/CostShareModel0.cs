﻿using System.IO;
using System.Xml.Linq;
using AD.IO;
using JetBrains.Annotations;

#pragma warning disable 1591

namespace AD.PartialEquilibriumApi.Example.TestModels
{
    public class CostShareModel0 : IModel
    {
        [UsedImplicitly]
        public XmlFilePath Model()
        {
            string xml = Path.ChangeExtension(Path.GetTempFileName(), ".xml");
            using (StreamWriter writer = new StreamWriter(xml))
            {
                writer.WriteLine(
                    @"<A0>
                          <B0/>
                          <B1>
                              <C0/>
                              <C1>
                                  <D0/>
                                  <D1>
                                      <E0/>
                                      <E1>
                                          <F0/>
                                          <F1/>
                                      </E1>
                                  </D1>
                              </C1>    
                          </B1>
                      </A0>");
            }
            return new XmlFilePath(xml);
        }

        [UsedImplicitly]
        public DelimitedFilePath Data()
        {
            string csv = Path.ChangeExtension(Path.GetTempFileName(), ".csv");
            using (StreamWriter writer = new StreamWriter(csv))
            {
                writer.WriteLine("ElasticityOfSubstitution,ElasticityOfSupply,ElasticityOfDemand,InitialPrice,InitialMarketShare,Shock,UpstreamCostShare");
                writer.WriteLine("4,5,-1,1.0,1.00,0.00,1");
                writer.WriteLine("4,5,-1,1.0,0.50,0.00,1");
                writer.WriteLine("4,5,-1,1.0,0.50,0.00,1");
                writer.WriteLine("4,5,-1,1.0,0.50,0.00,1");
                writer.WriteLine("4,5,-1,1.0,0.50,0.00,1");
                writer.WriteLine("4,5,-1,1.0,0.50,0.00,1");
                writer.WriteLine("4,5,-1,1.0,0.50,0.00,1");
                writer.WriteLine("4,5,-1,1.0,0.50,0.00,1");
                writer.WriteLine("4,5,-1,1.0,0.50,0.00,1");
                writer.WriteLine("4,5,-1,1.0,0.50,0.05,1");
                writer.WriteLine("4,5,-1,1.0,0.50,0.05,1");
            }
            return new DelimitedFilePath(csv, ',');
        }

        [UsedImplicitly]
        public XName[] Variables()
        {
            return new XName[]
            {
                "A0",
                    "B0",
                    "B1",
                        "C0",
                        "C1",
                            "D0",
                            "D1",
                                "E0",
                                "E1",
                                    "F0",
                                     "F1"
            };
        }
    }
}