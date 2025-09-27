using System.Text.Json;

namespace DebugTest;

internal static class Program
{
    static async Task Main(string[] args)
        {
            Console.WriteLine("开始并发测试...");

            const string url = "http://localhost:5232/UserRequest/RequestSingleSMS";
            const int concurrentRequests = 10;
            var results = new Dictionary<string, List<int>>();
            var httpClient = new HttpClient();

            // 创建并发任务
            var tasks = new Task[concurrentRequests];
            for (var i = 0; i < concurrentRequests; i++)
            {
                var requestIndex = i + 1;
                tasks[i] = Task.Run(async () =>
                {
                    try
                    {
                        //var response = await httpClient.GetStringAsync(url);
                        
                        var response = await httpClient.PostAsync(url, new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new HttpRequestException($"请求失败，状态码: {response.StatusCode}");
                        }
                        var responseBody = await response.Content.ReadAsStringAsync();
                        // Console.WriteLine(JsonDocument.Parse(responseBody));
                        
                        var contentType = "APPLICATION/JSON; CHARSET=UTF-8";
                        var containsJson = contentType.Contains(responseBody, StringComparison.OrdinalIgnoreCase);

                        if (containsJson)
                        {
                            throw new Exception("我是 SB 请求头");
                        }

                        
                        // 线程安全地添加结果
                        lock (results)
                        {
                            if (!results.TryGetValue(responseBody, out var value))
                            {
                                value = [];
                                results[responseBody] = value;
                            }

                            value.Add(requestIndex);
                            
                            // 输出当前请求的访问次序和是否重复
                            var isDuplicate = value.Count > 1;
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
            var hasDuplicates = false;
            foreach (var kvp in results.Where(kvp => kvp.Value.Count > 1))
            {
                hasDuplicates = true;
                Console.WriteLine($"发现重复数据: \"{kvp.Key}\"");
                Console.WriteLine($"重复出现在第 {string.Join(", ", kvp.Value)} 次访问");
            }

            if (!hasDuplicates)
            {
                Console.WriteLine("未发现重复数据");
            }

            Console.WriteLine("测试完成");
        }
}