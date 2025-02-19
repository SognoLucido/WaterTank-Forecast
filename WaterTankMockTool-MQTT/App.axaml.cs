using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using WaterTankMock_MQTT.Models;
using WaterTankMock_MQTT.Services;
using WaterTankMock_MQTT.Services.Mqtt;
using WaterTankMock_MQTT.ViewModels;
using WaterTankMock_MQTT.Views;



namespace WaterTankMock_MQTT
{
    public partial class App : Application
    {
        public IServiceProvider Services =  ConfigureServices();

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
                DisableAvaloniaDataAnnotationValidation();

                var vm = Services.GetRequiredService<MainWindowViewModel>();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = vm,
                };
            }

            base.OnFrameworkInitializationCompleted();
        }


        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }


        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddTransient<SimViewModel>();
            services.AddSingleton<SettingsTankViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddTransient<RecapViewModel>();
            services.AddTransient<OptionsViewModel>();
            services.AddTransient<StartViewModel>();
           


            services.AddTransient<StartView>();
            services.AddTransient<OptionsView>();
            services.AddTransient<RecapView>();
            services.AddTransient<MainWindow>();
            services.AddTransient<SettingsTankView>();
            services.AddTransient<SimView>();
           


            services.AddSingleton<Sharedata>();
            services.AddSingleton<MqttInit>();
            services.AddSingleton<PagesController>();

            return services.BuildServiceProvider();
        }



    }



}