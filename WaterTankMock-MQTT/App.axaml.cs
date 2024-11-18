using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using WaterTankMock_MQTT.Models;
using WaterTankMock_MQTT.Services;
using WaterTankMock_MQTT.ViewModels;
using WaterTankMock_MQTT.Views;

namespace WaterTankMock_MQTT
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                BindingPlugins.DataValidators.RemoveAt(0);


                var collection = new ServiceCollection();
                collection.AddServices();
                var services = collection.BuildServiceProvider();

                var vm = services.GetRequiredService<MainWindowViewModel>();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = vm,
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }


    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<SettingsTankViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddScoped<RecapViewModel>();

            services.AddTransient<RecapView>();
            services.AddTransient<MainWindow>();
            services.AddTransient<SettingsTankView>();


            services.AddSingleton<Sharedata>();
            services.AddSingleton<MqttInit>();
            services.AddSingleton<PagesController>();

        }
    }

}