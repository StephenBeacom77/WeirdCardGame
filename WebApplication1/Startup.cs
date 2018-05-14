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
                AddCardData(context);
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

        /// <summary>
        /// todo: move to other class named GameDataSeeder - added via DI?
        /// </summary>
        private void AddCardData(GameContext context)
        {
            AddKindData(context);
            AddSuitData(context);
        }

        private void AddKindData(GameContext context)
        {
            context.Kinds.RemoveRange(context.Kinds);
            context.Kinds.Add(new Kind(Kinds.Any , "?"));
            context.Kinds.Add(new Kind(Kinds.Ace  , "A"));
            context.Kinds.Add(new Kind(Kinds.Two  , "2"));
            context.Kinds.Add(new Kind(Kinds.Three, "3"));
            context.Kinds.Add(new Kind(Kinds.Four , "4"));
            context.Kinds.Add(new Kind(Kinds.Five , "5"));
            context.Kinds.Add(new Kind(Kinds.Six  , "6"));
            context.Kinds.Add(new Kind(Kinds.Seven, "7"));
            context.Kinds.Add(new Kind(Kinds.Eight, "8"));
            context.Kinds.Add(new Kind(Kinds.Nine , "9"));
            context.Kinds.Add(new Kind(Kinds.Ten  , "10"));
            context.Kinds.Add(new Kind(Kinds.Jack , "J"));
            context.Kinds.Add(new Kind(Kinds.Queen, "Q"));
            context.Kinds.Add(new Kind(Kinds.King , "K"));
            context.SaveChanges();
        }

        private void AddSuitData(GameContext context)
        {
            context.Suits.Add(new Suit(Suits.Any    , "?"));
            context.Suits.Add(new Suit(Suits.Hearts  , "\u2665"));
            context.Suits.Add(new Suit(Suits.Clubs   , "\u2663"));
            context.Suits.Add(new Suit(Suits.Diamonds, "\u2666"));
            context.Suits.Add(new Suit(Suits.Spades  , "\u2660"));
            context.SaveChanges();
        }
    }
}
