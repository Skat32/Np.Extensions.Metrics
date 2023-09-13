# Np.Extensions.Metrics

Пакет добавления метрик в приложение
Собирает метрики по получаемым http сообщениям, по отправляемым http сообщениям, а так же по ресурсам приложения

## Установка

1) Подключение сбора метрик по входящим http сообщениям происходит данным образом:

``` C#
// Startup.cs
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseInHttpMetrics();
    }
```

И 

``` C#
// Startup.cs
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<MetricReporter>();
    }
```

2) Подключение сбора метрик по исходящим http сообщениям происходит данным образом:

При конфигурации любого вашего API клиента ноебходимо добавить `.AddHttpMessageHandler<HttpOutMetricsHandler>()`

``` C#
// ServiceCollectionExtensions.cs
    public static void ConfigureHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<IAnyApiClient, AnyApiClient>()
            .AddHttpMessageHandler<HttpOutMetricsHandler>();
    }
```

3) Подключение сбора метрик по ресурсам используемым сервисом необходимо:

в `ServiceCollectionExtensions` зарагестировать настройки `MetricsSettings`

``` C#
// ServiceCollectionExtensions.cs
 public static void ConfigureSettings(this IServiceCollection services, IConfiguration config)
    {
        services.ConfigureSettings<MetricsSettings>(config, nameof(MetricsSettings));
    }
```

Перед этим необходимо добавить настройки в env приложения:

``` json
    "MetricsSettings__EnableMetrics": "true",
    "MetricsSettings__UseDefaultMetrics": "false",
    "MetricsSettings__UseDebuggingMetrics": "false"
```

Далее в `ServiceCollectionExtensions` в методе `ConfigureApplicationServices` необходимо запустить сбор статистики:

``` C#
// ServiceCollectionExtensions.cs
public static void ConfigureApplicationServices(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        var metricsSettings = serviceProvider.GetRequiredService<IOptions<MetricsSettings>>().Value;

        MetricsExtensions.CreateCollector(_logger, metricsSettings);
    }
```

## Дополнительная конфигурация

В пакете есть класс контроллера `CollectorController`, он является абстрактным и представляет возможность управления состоянием сбора метрик сервиса
Для использования его, необходимо унаследовать ваш контроллер от него и по роуту `api-collector` вам станут доступны 2 метода `enable` и `disable` соответсвенно включающие и выключающие сбор метрик