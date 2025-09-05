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
                policy.WithOrigins("https://zhfpht.zhan-hun.com") // Blazor WASM 地址
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials(); // 如果需要发送凭据（如 cookies）
            });
        });

        // Add services to the container.
        builder.Services.AddAuthorization();
        
        // 添加控制器支持
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        
        // 使用 CORS 中间件
        app.UseCors("AllowBlazorWasm");
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        // 启用控制器路由
        app.MapControllers();

        app.Run();
    }
}