using System;
using System.Collections.Generic;

namespace Uzil.Indices {

	public struct Pair<T1, T2> {
		public T1 t1;
		public T2 t2;
		public Pair (T1 t1, T2 t2) {
			this.t1 = t1;
			this.t2 = t2;
		}
	}

	public class SortDict {
		public static List<T2> GetSortValues <T1, T2> (Dictionary<T1, T2> dict, List<T1> sort) {
			
			List<T2> res = new List<T2>();

			List<Pair<int, T2>> temp = new List<Pair<int, T2>>();
			
			foreach (KeyValuePair<T1, T2> pair in dict) {
				temp.Add(new Pair<int, T2>(sort.IndexOf(pair.Key), pair.Value));
			}
			
			temp.Sort((a, b)=>{
				return a.t1 - b.t1;
			});
			
			for (int idx = 0; idx < temp.Count; idx++) {
				res.Add(temp[idx].t2);
			}

			return res;
		}
	
		public static List<T3> GetSortValuesTo <T1, T2, T3> (Dictionary<T1, T2> dict, List<T1> sort, Func<T2, T3> convert) {
				
			List<T3> res = new List<T3>();

			List<Pair<int, T2>> temp = new List<Pair<int, T2>>();
			
			foreach (KeyValuePair<T1, T2> pair in dict) {
				temp.Add(new Pair<int, T2>(sort.IndexOf(pair.Key), pair.Value));
			}
			
			temp.Sort((a, b)=>{
				return a.t1 - b.t1;
			});
			
			for (int idx = 0; idx < temp.Count; idx++) {
				res.Add(convert(temp[idx].t2));
			}

			return res;
		}
	}

}