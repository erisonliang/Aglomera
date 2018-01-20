﻿// ------------------------------------------
// <copyright file="Program.cs" company="Pedro Sequeira">
//     Some copyright
// </copyright>
// <summary>
//    Project: NumericClustering
//    Last updated: 2018/01/05
// 
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using Agnes;
using Agnes.D3;
using Agnes.Linkage;
using ExamplesUtil;
using Newtonsoft.Json;

namespace NumericClustering
{
    internal class Program
    {
        #region Static Fields & Constants

        private const string DATASET_FILE = "../../../../datasets/seeds.csv";

        #endregion

        #region Private & Protected Methods

        private static void Main(string[] args)
        {
            var dataSetFile = args.Length > 0 ? args[0] : DATASET_FILE;
            var parser = new CsvParser();
            var instances = parser.Load(Path.GetFullPath(dataSetFile));

            var metric = new DataPoint(null, null);
            var linkages = new Dictionary<ILinkageCriterion<DataPoint>, string>
                           {
                               {new AverageLinkage<DataPoint>(metric), "average"},
                               {new CompleteLinkage<DataPoint>(metric), "complete"},
                               {new SingleLinkage<DataPoint>(metric), "single"},
                               {new MinimumEnergyLinkage<DataPoint>(metric), "min-energy"},
                               {new CentroidLinkage<DataPoint>(metric, DataPoint.GetCentroid), "centroid"},
                               {new WardsMinimumVarianceLinkage<DataPoint>(metric, DataPoint.GetCentroid), "ward"}
                           };
            foreach (var linkage in linkages)
                PrintClusters(instances, linkage.Key, linkage.Value);

            Console.WriteLine("\nDone!");
            Console.ReadKey();
        }

        private static void PrintClusters(ISet<DataPoint> instances, ILinkageCriterion<DataPoint> linkage, string name)
        {
            var perfMeasure = new PerformanceMeasure();
            perfMeasure.Start();
            var clusteringAlg = new ClusteringAlgorithm<DataPoint>(linkage);
            var clustering = clusteringAlg.GetClustering(instances);
            perfMeasure.Stop();

            Console.WriteLine("_____________________________________________");
            Console.WriteLine(name);
            Console.WriteLine(perfMeasure);
            foreach (var clusterSet in clustering)
            {
                Console.WriteLine($"Clusters at distance: {clusterSet.Dissimilarity:0.00} ({clusterSet.Count})");
                foreach (var cluster in clusterSet)
                    Console.WriteLine($" - {cluster}");
            }

            clustering.SaveD3DendrogramFile(Path.GetFullPath($"{name}.json"), formatting: Formatting.Indented);
        }

        #endregion
    }
}