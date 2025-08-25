using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DatabaseCore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("开始并发测试...");

            var url = "http://45.207.211.227:8001/UserRequest/RequestSingleSMS/";
            var concurrentRequests = 300;
            var results = new Dictionary<string, List<int>>();
            var httpClient = new HttpClient();

            // 创建并发任务
            var tasks = new Task[concurrentRequests];
            for (int i = 0; i < concurrentRequests; i++)
            {
                int requestIndex = i + 1;
                tasks[i] = Task.Run(async () =>
                {
                    try
                    {
                        var response = await httpClient.GetStringAsync(url);
                        
                        // 线程安全地添加结果
                        lock (results)
                        {
                            if (!results.ContainsKey(response))
                            {
                                results[response] = new List<int>();
                            }
                            results[response].Add(requestIndex);
                            
                            // 输出当前请求的访问次序和是否重复
                            bool isDuplicate = results[response].Count > 1;
                            Console.WriteLine($"第{requestIndex}次请求，返回数据: \"{response}\"，{(isDuplicate ? "重复" : "首次出现")}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"第{requestIndex}次访问出错: {ex.Message}");
                    }
                });
            }

            // 等待所有任务完成
            await Task.WhenAll(tasks);

            // 最后总结重复数据情况
            Console.WriteLine("\n=== 重复数据总结 ===");
            bool hasDuplicates = false;
            foreach (var kvp in results)
            {
                if (kvp.Value.Count > 1)
                {
                    hasDuplicates = true;
                    Console.WriteLine($"发现重复数据: \"{kvp.Key}\"");
                    Console.WriteLine($"重复出现在第 {string.Join(", ", kvp.Value)} 次访问");
                }
            }

            if (!hasDuplicates)
            {
                Console.WriteLine("未发现重复数据");
            }

            Console.WriteLine("测试完成");
        }
    }
}
