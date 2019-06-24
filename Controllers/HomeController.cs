using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KeyValueTypesComprasion.Models;
using System.Collections;

namespace KeyValueTypesComprasion.Controllers
{
    public class HomeController : Controller
    {
        MyObject dummyValue;
        MyObject[] dummyObjects;

        class MyObject
        {

        }

        public IActionResult Index(int size = 100000)
        {
            var sw = new Stopwatch();
            sw.Start();

            var resultModel = new ResultModel
            {
                RequestCount = size
            };

            dummyValue = new MyObject();
            dummyObjects = new MyObject[resultModel.RequestCount];

            for (int i = 0; i < resultModel.RequestCount; i++)
                dummyObjects[i] = new MyObject();


            var taskArray = new Task<ResultItemModel>[] {
                Task.Run(() => GetHashtableResult(resultModel.RequestCount)),
                Task.Run(() => GetHashsetResult(resultModel.RequestCount)),
                Task.Run(() => GetDictionaryResult(resultModel.RequestCount))
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

            // Contains
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                hashtable.Contains(dummyObjects[i]);
            }
            var htContainsTime = sw.Elapsed.TotalMilliseconds;

            // Remove
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                hashtable.Remove(dummyObjects[i]);
            }
            var htRemoveTime = sw.Elapsed.TotalMilliseconds;

            sw.Stop();
            // Result
            return await Task.Run(() => new ResultItemModel
            {
                TypeName = "Hashtable",
                AddTime = htAddTime,
                ContainsTime = htContainsTime,
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

            // Contains
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                hashset.Contains(dummyObjects[i]);
            }
            var hsContainsTime = sw.Elapsed.TotalMilliseconds;

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
                ContainsTime = hsContainsTime,
                RemoveTime = hsRemoveTime
            });
        }

        public async Task<ResultItemModel> GetDictionaryResult(int size)
        {
            var sw = new Stopwatch();
            sw.Start();

            var dictionary = new Dictionary<MyObject, bool>();

            // Add
            for (int i = 0; i < size; i++)
            {
                dictionary.Add(dummyObjects[i], true);
            }
            var dAddTime = sw.Elapsed.TotalMilliseconds;

            // Contains
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                dictionary.ContainsKey(dummyObjects[i]);
            }
            var dContainsTime = sw.Elapsed.TotalMilliseconds;

            // Remove
            sw.Reset();
            sw.Start();
            for (int i = 0; i < size; i++)
            {
                dictionary.Remove(dummyObjects[i]);
            }
            var dRemoveTime = sw.Elapsed.TotalMilliseconds;

            sw.Stop();
            // Result
            return await Task.Run(() => new ResultItemModel
            {
                TypeName = "Dictionary",
                AddTime = dAddTime,
                ContainsTime = dContainsTime,
                RemoveTime = dRemoveTime
            });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    }
}
