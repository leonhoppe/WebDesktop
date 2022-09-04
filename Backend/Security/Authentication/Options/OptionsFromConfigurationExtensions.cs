namespace Backend.Options {
    public static class OptionsFromConfigurationExtensions {
        public static T AddOptionsFromConfiguration<T>(this IServiceCollection services, IConfiguration configuration)
            where T : OptionsFromConfiguration {
            T optionsInstance = (T)Activator.CreateInstance(typeof(T));
            if (optionsInstance == null) return null;
            string position = optionsInstance.Position;
            services.Configure((Action<T>)(options => {
                IConfigurationSection section = configuration.GetSection(position);
                if (section != null) {
                    section.Bind(options);
                }
            }));
            return optionsInstance;
        }
    }
}