using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphManager.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<string> SortStringAsInt(this IEnumerable<string> sequence)
        {
            try
            {
                var intSequence = StringToIntSequence(sequence).ToList();

                intSequence.Sort();

                return IntToStringSequence(intSequence);                
            }
            catch (Exception) 
            {
                return sequence;
            }
            
        }

        private static IEnumerable<string> IntToStringSequence(List<int> sequence)
        {
            foreach (int item in sequence)
            {
                yield return item.ToString();
            }
        }

        private static IEnumerable<int> StringToIntSequence(IEnumerable<string> sequence)
        {
            foreach (string item in sequence)
            {
                if (int.TryParse(item, out int result))
                    yield return result;
                else
                    throw new ArgumentException($"Cannot parse {item} item.");
            }
        }

        public static IEnumerable<T> Flatten<T, U>(this IEnumerable<U> sequence) where U: IEnumerable<T>
        {
            var res = new List<T>();

            foreach (var item in sequence)
            {
                if (item is IEnumerable<T> seq)
                {
                    res.AddRange(seq);
                }
            }

            return res;
        }

        public static bool ContainsAny<T>(this IEnumerable<T> sequence1, IEnumerable<T> sequence2, IEqualityComparer<T> equalityComparer = null)
        {
            if (sequence1.Count() > sequence2.Count())
                return sequence2.ContainsAny(sequence1);

            foreach (var item in sequence1)
            {
                if (equalityComparer == null)
                    if (sequence2.Contains(item))
                        return true;
                else
                    if (sequence2.Contains(item, equalityComparer))
                        return true; 
            }

            return false;
        }

        public static IEnumerable<T> RemoveValues<T>(this IEnumerable<T> baseSequence, IEnumerable<T> values)
        {
            var temp = baseSequence.ToList();

            foreach (var item in values)
            {
                if (baseSequence.Contains(item))
                    temp.Remove(item);
            }

            return temp;
        }

        public static IEnumerable<T> Add<T>(this IEnumerable<T> baseSequence, params IEnumerable<T>[] sequences)
        {
            var res = new List<T>(baseSequence);

            foreach (var item in sequences)
            {
                res = res
                    .Add(item)
                    .ToList();
            }

            return res;
        }

        public static IEnumerable<T> Add<T>(this IEnumerable<T> col1, IEnumerable<T> col2)
        {
            var res = new List<T>();

            res.AddRange(col1);

            res.AddRange(col2);

            return res;
        }
    }
}
