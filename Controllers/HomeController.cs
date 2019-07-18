using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KeyValueTypesComprasion.Models;
using System.Collections;
using System.Collections.Concurrent;

namespace KeyValueTypesComprasion.Controllers
{
    public class HomeController : Controller
    {
        MyObject dummyValue;
        MyObject[] dummyObjects;

        class MyObject
        {

        }

        public IActionResult Index(int size = 1000)
        {
            var resultModel = new ResultModel
            {
                RequestCount = size
            };

            if (size > 1000001)
            {
                resultModel.HasError = true;
                return View(resultModel);
            }

            var sw = new Stopwatch();
            sw.Start();

            dummyValue = new MyObject();
            dummyObjects = new MyObject[resultModel.RequestCount];

            for (int i = 0; i < resultModel.RequestCount; i++)
                dummyObjects[i] = new MyObject();


            var taskArray = new Task<ResultItemModel>[] {
                Task.Run(() => GetHashtableResult(resultModel.RequestCount)),
                Task.Run(() => GetHashsetResult(resultModel.RequestCount)),
                Task.Run(() => GetDictionaryResult(resultModel.RequestCount)),
                Task.Run(() => GetConcurrentDictionaryResult(resultModel.RequestCount)),
                Task.Run(() => GetSortedDictionaryResult(resultModel.RequestCount)),
                Task.Run(() => GetListResult(resultModel.RequestCount)),
                Task.Run(() => GetSortedListResult(resultModel.RequestCount))
            };

            Task.WaitAll(taskArray);

            for (int i = 0; i < taskArray.Length; i++)
            {
                var taskItem = taskArray[i];
                if (taskItem.IsCompleted)
                {
                    resultModel.ResultItemList.Add(taskItem.Result);
                }
            }

            resultModel.ResponseTime = sw.Elapsed.TotalMilliseconds;
            sw.Stop();

            return View(resultModel);
        }

        public async Task<ResultItemModel> GetHashtableResult(int size)
        {
            var sw = new Stopwatch();
            sw.Start();

            var hashtable = new Hashtable();

            // Add
            for (int i = 0; i < size; i++)
            {
                hashtable.Add(dummyObjects[i], dummyValue);
            }
            var htAddTime = sw.Elapsed.TotalMilliseconds;

            // TryAdd
            var htTryAddTime = -1;

            // Contains
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                hashtable.ContainsKey(i);
            }
            var htContainsTime = sw.Elapsed.TotalMilliseconds;

            // FindIndex
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                var a = hashtable[dummyObjects[i]];
            }
            var htFindIndexTime = sw.Elapsed.TotalMilliseconds;

            // Remove
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                hashtable.Remove(dummyObjects[i]);
            }
            var htRemoveTime = sw.Elapsed.TotalMilliseconds;

            // TryGetValue
            var htTryGetValueTime = -1;

            sw.Stop();
            // Result
            return await Task.Run(() => new ResultItemModel
            {
                TypeName = "Hashtable",
                AddTime = htAddTime,
                TryAddTime = htTryAddTime,
                ContainsTime = htContainsTime,
                FindIndexTime = htFindIndexTime,
                TryGetValueTime = htTryGetValueTime,
                RemoveTime = htRemoveTime
            });
        }


        public async Task<ResultItemModel> GetHashsetResult(int size)
        {
            var sw = new Stopwatch();
            sw.Start();

            var hashset = new HashSet<MyObject>();

            // Add
            for (int i = 0; i < size; i++)
            {
                hashset.Add(dummyObjects[i]);
            }
            var hsAddTime = sw.Elapsed.TotalMilliseconds;

            // TryAdd
            var hsTryAddTime = -1;

            // Contains
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                hashset.Contains(dummyObjects[i]);
            }
            var hsContainsTime = sw.Elapsed.TotalMilliseconds;

            // FindIndex
            var hsFindIndexTime = -1;

            // TryGetValue
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                MyObject a;
                hashset.TryGetValue(dummyObjects[i], out a);
            }
            var hsTryGetValueTime = sw.Elapsed.TotalMilliseconds;

            // Remove
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                hashset.Remove(dummyObjects[i]);
            }
            var hsRemoveTime = sw.Elapsed.TotalMilliseconds;

            sw.Stop();
            // Result
            return await Task.Run(() => new ResultItemModel
            {
                TypeName = "HashSet",
                AddTime = hsAddTime,
                TryAddTime = hsTryAddTime,
                ContainsTime = hsContainsTime,
                FindIndexTime = hsFindIndexTime,
                TryGetValueTime = hsTryGetValueTime,
                RemoveTime = hsRemoveTime
            });
        }

        public async Task<ResultItemModel> GetDictionaryResult(int size)
        {
            var sw = new Stopwatch();
            sw.Start();

            var dictionary = new Dictionary<int, MyObject>();

            // Add
            for (int i = 0; i < size; i++)
            {
                dictionary.Add(i, dummyObjects[i]);
            }
            var dAddTime = sw.Elapsed.TotalMilliseconds;

            // TryAdd
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                dictionary.TryAdd(i, dummyObjects[i]);
            }
            var dTryAddTime = sw.Elapsed.TotalMilliseconds;

            // Contains
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                dictionary.ContainsKey(i);
            }
            var dContainsTime = sw.Elapsed.TotalMilliseconds;

            // FindIndex
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                var a = dictionary[i];
            }
            var dFindIndexTime = sw.Elapsed.TotalMilliseconds;

            // TryGetValue
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                MyObject a;
                dictionary.TryGetValue(i, out a);
            }
            var dTryGetValueTime = sw.Elapsed.TotalMilliseconds;

            // Remove
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                dictionary.Remove(i);
            }
            var dRemoveTime = sw.Elapsed.TotalMilliseconds;

            sw.Stop();
            // Result
            return await Task.Run(() => new ResultItemModel
            {
                TypeName = "Dictionary",
                AddTime = dAddTime,
                TryAddTime = dTryAddTime,
                ContainsTime = dContainsTime,
                FindIndexTime = dFindIndexTime,
                TryGetValueTime = dTryGetValueTime,
                RemoveTime = dRemoveTime
            });
        }

        public async Task<ResultItemModel> GetConcurrentDictionaryResult(int size)
        {
            var sw = new Stopwatch();
            sw.Start();

            var cDictionary = new ConcurrentDictionary<int, MyObject>();

            // Add
            var cdAddTime = -1;

            // TryAdd
            for (int i = 0; i < size; i++)
            {
                cDictionary.TryAdd(i, dummyObjects[i]);
            }
            var cdTryAddTime = sw.Elapsed.TotalMilliseconds;

            // Contains
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                cDictionary.ContainsKey(i);
            }
            var cdContainsTime = sw.Elapsed.TotalMilliseconds;

            // FindIndex
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                var a = cDictionary[i];
            }
            var cdFindIndexTime = sw.Elapsed.TotalMilliseconds;

            // TryGetValue
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                MyObject a;
                cDictionary.TryGetValue(i, out a);
            }
            var cdTryGetValueTime = sw.Elapsed.TotalMilliseconds;

            // Remove
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                MyObject obj;
                cDictionary.TryRemove(i, out obj);
            }
            var cdRemoveTime = sw.Elapsed.TotalMilliseconds;

            sw.Stop();
            // Result
            return await Task.Run(() => new ResultItemModel
            {
                TypeName = "ConcurrentDictionary",
                AddTime = cdAddTime,
                TryAddTime = cdTryAddTime,
                ContainsTime = cdContainsTime,
                FindIndexTime = cdFindIndexTime,
                TryGetValueTime = cdTryGetValueTime,
                RemoveTime = cdRemoveTime
            });
        }

        public async Task<ResultItemModel> GetSortedDictionaryResult(int size)
        {
            var sw = new Stopwatch();
            sw.Start();

            var sDictionary = new SortedDictionary<int, MyObject>();

            // Add
            for (int i = 0; i < size; i++)
            {
                sDictionary.Add(i, dummyObjects[i]);
            }
            var sdAddTime = sw.Elapsed.TotalMilliseconds;

            // TryAdd
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                sDictionary.TryAdd(i, dummyObjects[i]);
            }
            var sdTryAddTime = sw.Elapsed.TotalMilliseconds;

            // Contains
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                sDictionary.ContainsKey(i);
            }
            var sdContainsTime = sw.Elapsed.TotalMilliseconds;

            // FindIndex
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                var a = sDictionary[i];
            }
            var sdFindIndexTime = sw.Elapsed.TotalMilliseconds;

            // TryGetValue
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                MyObject a;
                sDictionary.TryGetValue(i, out a);
            }
            var sdTryGetValueTime = sw.Elapsed.TotalMilliseconds;

            // Remove
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                sDictionary.Remove(i);
            }
            var sdRemoveTime = sw.Elapsed.TotalMilliseconds;

            sw.Stop();
            // Result
            return await Task.Run(() => new ResultItemModel
            {
                TypeName = "SortedDictionary",
                AddTime = sdAddTime,
                TryAddTime = sdTryAddTime,
                ContainsTime = sdContainsTime,
                FindIndexTime = sdFindIndexTime,
                TryGetValueTime = sdTryGetValueTime,
                RemoveTime = sdRemoveTime
            });
        }

        public async Task<ResultItemModel> GetListResult(int size)
        {
            var sw = new Stopwatch();
            sw.Start();

            var list = new List<MyObject>();

            // Add
            for (int i = 0; i < size; i++)
            {
                list.Add(dummyObjects[i]);
            }
            var lAddTime = sw.Elapsed.TotalMilliseconds;

            // TryAdd
            var lTryAddTime = -1;

            // Contains
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                list.Contains(dummyObjects[i]);
            }
            var lContainsTime = sw.Elapsed.TotalMilliseconds;

            // FindIndex
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                var a = list[i];
            }
            var lFindIndexTime = sw.Elapsed.TotalMilliseconds;

            // TryGetValue
            var lTryGetValueTime = -1;

            // Remove
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                list.Remove(dummyObjects[i]);
            }
            var lRemoveTime = sw.Elapsed.TotalMilliseconds;

            sw.Stop();
            // Result
            return await Task.Run(() => new ResultItemModel
            {
                TypeName = "List",
                AddTime = lAddTime,
                TryAddTime = lTryAddTime,
                ContainsTime = lContainsTime,
                FindIndexTime = lFindIndexTime,
                TryGetValueTime = lTryGetValueTime,
                RemoveTime = lRemoveTime
            });
        }

        public async Task<ResultItemModel> GetSortedListResult(int size)
        {
            var sw = new Stopwatch();
            sw.Start();

            var sortedList = new SortedList<int, MyObject>();

            // Add
            for (int i = 0; i < size; i++)
            {
                sortedList.Add(i, dummyObjects[i]);
            }
            var lAddTime = sw.Elapsed.TotalMilliseconds;

            // TryAdd
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                sortedList.TryAdd(i, dummyObjects[i]);
            }
            var lTryAddTime = sw.Elapsed.TotalMilliseconds;

            // Contains
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                sortedList.ContainsKey(i);
            }
            var lContainsTime = sw.Elapsed.TotalMilliseconds;

            // FindIndex
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                var a = sortedList[i];
            }
            var lFindIndexTime = sw.Elapsed.TotalMilliseconds;

            // TryGetValue
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                MyObject a;
                sortedList.TryGetValue(i, out a);
            }
            var lTryGetValueTime = sw.Elapsed.TotalMilliseconds;

            // Remove
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                sortedList.Remove(i);
            }
            var lRemoveTime = sw.Elapsed.TotalMilliseconds;

            sw.Stop();
            // Result
            return await Task.Run(() => new ResultItemModel
            {
                TypeName = "SortedList",
                AddTime = lAddTime,
                TryAddTime = lTryAddTime,
                ContainsTime = lContainsTime,
                FindIndexTime = lFindIndexTime,
                TryGetValueTime = lTryGetValueTime,
                RemoveTime = lRemoveTime
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    }

}
