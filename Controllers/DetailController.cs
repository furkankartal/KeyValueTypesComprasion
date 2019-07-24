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
    public class DetailController : Controller
    {
        MyObject dummyValue;

        ConcurrentDictionary<string, MyObject> cDummyObjects;
        Dictionary<string, MyObject> dDummyObjects;

        class MyObject
        {
            public int Id { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public IActionResult Index(int size = 1000)
        {
            var resultModel = new DetailResultModel
            {
                RequestCount = size
            };

            var sw = new Stopwatch();
            sw.Start();

            cDummyObjects = new ConcurrentDictionary<string, MyObject>();

            for (int i = 0; i < resultModel.RequestCount; i++)
            {
                dummyValue = new MyObject
                {
                    Id = i,
                    Key = string.Concat("key-", i),
                    Value = string.Concat("value-", i)
                };

                cDummyObjects.TryAdd(dummyValue.Key, dummyValue);
            }

            var cdTryAddTime = sw.Elapsed.TotalMilliseconds;
            resultModel.ResultItemList.Add(new DetailResultItemModel
            {
                Type = "ConcurrentDictionary",
                Step = "TryAddResult",
                Time = cdTryAddTime
            });

            sw.Reset();
            sw.Start();

            dDummyObjects = new Dictionary<string, MyObject>();

            for (int i = 0; i < resultModel.RequestCount; i++)
            {
                dummyValue = new MyObject
                {
                    Id = i,
                    Key = string.Concat("key-", i),
                    Value = string.Concat("value-", i)
                };

                dDummyObjects.TryAdd(dummyValue.Key, dummyValue);
            }

            var dTryAddTime = sw.Elapsed.TotalMilliseconds;
            resultModel.ResultItemList.Add(new DetailResultItemModel
            {
                Type = "Dictionary",
                Step = "TryAddResult",
                Time = dTryAddTime
            });

            sw.Reset();
            sw.Start();

            var taskArray = new Task<List<DetailResultItemModel>>[] {
                Task.Run(() => GetData("value-5")),
                Task.Run(() => GetData("value-10")),
                Task.Run(() => GetData("value-25")),
                Task.Run(() => GetData("value-50")),
                Task.Run(() => GetData("value-75")),
                Task.Run(() => GetData("value-100")),
                Task.Run(() => GetData("value-250")),
                Task.Run(() => GetData("value-500")),
                Task.Run(() => GetData("value-750")),
                Task.Run(() => GetData("value-1000"))
            };

            Task.WaitAll(taskArray);

            for (int i = 0; i < taskArray.Length; i++)
            {
                var taskItem = taskArray[i];
                if (taskItem.IsCompleted)
                {
                    for (int j = 0; j < taskItem.Result.Count; j++)
                    {
                        var detailTaskItem = taskItem.Result[j];
                        resultModel.ResultItemList.Add(detailTaskItem);
                    }
                }
            }

            var taskResult = sw.Elapsed.TotalMilliseconds;
            sw.Stop();

            resultModel.ResultItemList.Add(new DetailResultItemModel
            {
                Type = "All",
                Step = "Result",
                Time = taskResult
            });

            resultModel.ResponseTime = taskResult;

            return View(resultModel);
        }

        public async Task<List<DetailResultItemModel>> GetData(string key)
        {
            var resultModel = new DetailResultModel();
            var taskArray = new Task<DetailResultItemModel>[] {
                Task.Run(() => GetDictionaryData(key)),
                Task.Run(() => GetConcurrentDictionaryData(key)),
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

            // Result
            return await Task.Run(() => resultModel.ResultItemList);
        }

        public async Task<DetailResultItemModel> GetDictionaryData(string key)
        {
            var sw = new Stopwatch();
            sw.Start();

            MyObject obj;
            dDummyObjects.TryGetValue(key, out obj);

            var dTryGetValueTime = sw.Elapsed.TotalMilliseconds;
            sw.Stop();
            sw.Reset();

            // Result
            return await Task.Run(() => new DetailResultItemModel
            {
                Type = "Dictionary",
                Step = "TryGetValue",
                Value = key,
                Time = dTryGetValueTime
            });
        }

        public async Task<DetailResultItemModel> GetConcurrentDictionaryData(string key)
        {
            var sw = new Stopwatch();
            sw.Start();

            MyObject obj;
            cDummyObjects.TryGetValue(key, out obj);

            var cdTryGetValueTime = sw.Elapsed.TotalMilliseconds;
            sw.Stop();

            // Result
            return await Task.Run(() => new DetailResultItemModel
            {
                Type = "ConcurrentDictionary",
                Step = "TryGetValue",
                Value = key,
                Time = cdTryGetValueTime
            });
        }

    }

}
