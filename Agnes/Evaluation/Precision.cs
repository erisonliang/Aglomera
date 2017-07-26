// ------------------------------------------
// <copyright file="Precision.cs" company="Pedro Sequeira">
//     Some copyright
// </copyright>
// <summary>
//    Project: Agnes
//    Last updated: 2017/07/26
// 
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Agnes.Evaluation
{
    /// <summary>
    ///     Evaluates the given partition according to the precision criterion, given by the percentage of true positives over
    ///     all positives.
    /// </summary>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    /// <typeparam name="TClass">The type of class considered.</typeparam>
    /// <remarks>
    ///     see: <a href="https://nlp.stanford.edu/IR-book/html/htmledition/evaluation-of-clustering-1.html" />
    /// </remarks>
    public class Precision<TInstance, TClass> : IExternalCriterion<TInstance, TClass>
        where TInstance : IComparable<TInstance>
    {
        #region Public Methods

        public double Evaluate(ClusterSet<TInstance> clusterSet, IDictionary<TInstance, TClass> instanceClasses)
        {
            // counts the positives for each cluster 
            var truePositives = 0L;
            var positives = 0L;
            foreach (var cluster in clusterSet)
            {
                // gets class counts
                var clusterClassCounts = new Dictionary<TClass, int>();
                foreach (var instance in cluster)
                {
                    var instanceClass = instanceClasses[instance];
                    if (clusterClassCounts.ContainsKey(instanceClass))
                        clusterClassCounts[instanceClass]++;
                    else clusterClassCounts[instanceClass] = 1;
                }

                // updates positives
                positives += Combinatorics.GetCombinations(cluster.Count, 2);

                // updates true positives (pairs of same class within cluster)
                truePositives += clusterClassCounts.Values
                    .Where(count => count > 1)
                    .Sum(count => Combinatorics.GetCombinations(count, 2));
            }

            // returns precision
            return (double) truePositives / positives;
        }

        #endregion
    }
}