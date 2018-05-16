using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeirdCardGame.Data;
using WeirdCardGame.Services;

namespace WeirdCardGame
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<GameContext>(
                optionsAction: opt => opt.UseInMemoryDatabase("WeirdCardGame"),
                contextLifetime: ServiceLifetime.Singleton);

            services.AddTransient<ICardDrawingService>(
                sp => new CardDrawingService());
            services.AddTransient<ICardScoringService>(
                sp => new CardScoringService());
            services.AddTransient<IGamePlayingService>(
                sp => new GamePlayingService(
                    sp.GetService<ICardDrawingService>(),
                    sp.GetService<ICardScoringService>()));
            services.AddTransient<IGameHistoryService>(
                sp => new GameHistoryService(
                    sp.GetService<GameContext>()));

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<GameContext>();
                var seeder = new GameContextSeeder(context);
                seeder.AddCardData(context);
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
