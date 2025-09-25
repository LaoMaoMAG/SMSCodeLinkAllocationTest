namespace WebAPI;

// ReSharper disable once ClassNeverInstantiated.Global
public class Program
{
    /// <summary>
    /// 启动时间
    /// </summary>
    public static DateTime StartTime = DateTime.Now;
    
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
            
        // 添加 CORS 服务
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowBlazorWasm", policy =>
            {
                policy.WithOrigins("https://jmfpht.zhan-hun.com") // Blazor WASM 地址
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials(); // 如果需要发送凭据（如 cookies）
            });
        });
        
        
        // Add services to the container.
        // builder.Services.AddAuthorization();
        
        // 添加控制器支持
        // builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        // builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        var app = builder.Build();
        
        // 使用 CORS 中间件
        app.UseCors("AllowBlazorWasm");
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // app.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");
            app.UseHsts();
        }


        app.UseHttpsRedirection();
        
        app.UseBlazorFrameworkFiles(); // 启用 Blazor 文件服务
        app.UseStaticFiles(); // 启用静态文件服务
        
        // 👇 启用路由（API 路由）
        app.UseRouting();

        // app.UseAuthorization();
        
       
        app.MapRazorPages(); // 启用页面路由
        app.MapControllers(); // 启用控制器路由
        app.MapFallbackToFile("index.html"); // 启用默认文件
        
        app.Run();
    }
}