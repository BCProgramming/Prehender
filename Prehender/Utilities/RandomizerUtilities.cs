using System;
using System.Collections.Generic;
using System.Linq;

namespace Prehender.Utilities
{
    class RandomizerUtilities
    {
        public static T Choose<T>(IEnumerable<T> ChooseArray)
        {
            if (rgen == null) rgen = new Random();
            SortedList<double, T> sorttest = new SortedList<double, T>();
            foreach (T loopvalue in ChooseArray)
            {
                double rgg;
                do
                {
                    rgg = rgen.NextDouble();
                }
                while (sorttest.ContainsKey(rgg));
                sorttest.Add(rgg, loopvalue);

            }

            //return the first item.
            return sorttest.First().Value;
        }
        public static Random rgen = null;
        /// <summary>
        /// Selects numselect random items from Choosearray, returning those chosen items in a new array.
        /// If numselect is larger then the size of the array, the resulting array will be numselect, but any entries after the length of the selection array
        /// will be undefined.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ChooseArray"></param>
        /// <param name="numselect"></param>
        /// <returns></returns>
        public static T[] Choose<T>(IEnumerable<T> ChooseArray, int numselect)
        {
            if (rgen == null) rgen = new Random();
            T[] returnarray = new T[numselect];
            SortedList<double, T> sorttest = new SortedList<double, T>();
            foreach (T loopvalue in ChooseArray)
            {
                sorttest.Add(rgen.NextDouble(), loopvalue);
            }
            //Array.Copy(sorttest.ToArray(), returnarray, numselect);
            var usearray = sorttest.ToArray();
            for (int i = 0; i < numselect; i++)
            {
                returnarray[i] = usearray[i].Value;
            }
            return returnarray;
        }
    }
}
