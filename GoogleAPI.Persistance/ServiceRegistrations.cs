using GoogleAPI.Persistance;
using GoogleAPI.Persistance.Concreates;
using GoogleAPI.Persistance.Concreates.Services.Helper;
using GoogleAPI.Persistance.Concreates.Services.Mail;
using GoogleAPI.Persistance.Concreates.User_Token;
using GoogleAPI.Persistance.Concretes;
using GoogleAPI.Persistance.Contexts;
using GoogleAPI.Persistance.Repositories.UserAndCommunication;
using GooleAPI.Application.Abstractions;
using GooleAPI.Application.Abstractions.IServices.IHelper;
using GooleAPI.Application.Abstractions.IServices.IMail;
using GooleAPI.Application.IRepositories;
using GooleAPI.Application.IRepositories.Log;
using GooleAPI.Application.IRepositories.UserAndCommunication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GooleAPI.Infrastructure
{
    public static class ServiceRegistrations
    {
        public static void AddPersistanceServices(this IServiceCollection services)
        {
            //Dependency Injection (DI) konteynerı, bir ASP.NET Core uygulamasında, hizmetleri yönetmek ve bunlara erişmek için kullanılan bir tasarım desenidir. DI konteynerı, hizmetlerin oluşturulması, kaydedilmesi ve yönetilmesi için gereken tüm işlemleri yapar.DI konteynerı, IServiceCollection arayüzü üzerinden hizmetlerin kaydını yapar. Bu hizmetler, AddTransient, AddScoped, AddSingleton gibi yöntemler kullanılarak kaydedilir. DI konteynerı, uygulama ihtiyacı doğrultusunda, kaydedilen hizmetleri yaşam döngüsü boyunca yönetir.

            //IServiceCollection, ASP.NET Core uygulamalarında kullanılan bir arayüz (interface) ve Dependency Injection (DI) konteynerinin bir parçasıdır.IServiceCollection, DI konteynerine kaydedilecek olan hizmetlerin (services) kaydını tutar. Bu arayüz üzerinden, uygulama içinde kullanılacak olan hizmetlerin kaydı yapılarak, bu hizmetlere DI konteyneri üzerinden erişilebilir hale getirilir.IServiceCollection, uygulama hizmetlerinin kaydını yapmak için kullanılan AddTransient, AddScoped, AddSingleton gibi yöntemleri sağlar. Bu yöntemler, uygulama hizmetleri için farklı ömür biçimlerini tanımlar ve DI konteyneri üzerinden bu hizmetlere erişim sağlanır.IServiceCollection arayüzü, uygulama hizmetlerinin kaydı için önemlidir çünkü hizmetlerin DI konteynerine doğru bir şekilde kaydedilmesini sağlar. Bu da uygulamanın performansını artırır ve kodun daha okunaklı ve yönetilebilir olmasını sağlar. Ayrıca, IServiceCollection, ASP.NET Core uygulamalarında genişletilebilirlik sağlayarak, özel hizmetlerin kolayca kaydedilmesine ve kullanılmasına olanak tanır.

            //AddTransient, AddScoped ve AddSingleton, ASP.NET Core DI konteynerı aracılığıyla hizmetlerin kaydedilmesi için kullanılan üç farklı yöntemdir. Bu yöntemler, hizmetlerin yaşam döngüsü boyunca nasıl kullanılacağını belirler ve hizmetin örnekleme ve paylaşım şeklini tanımlar.

            //AddTransient:AddTransient yöntemi, her istekte yeni bir hizmet örneği oluşturur. Yani, her istekte ayrı bir örnek oluşturarak, hizmetlerin kullanım ömrü kısadır ve hizmetler, istek başına bir kopya olarak oluşturulur. Bu yöntem genellikle, hizmetin her kullanıcı isteği için ayrı bir örneğinin oluşturulması gerektiği durumlarda kullanılır.Örnek kullanım:services.AddTransient<ISampleService, SampleService>();

            //AddScoped:AddScoped yöntemi, her istekte aynı örnekten kullanır. Yani, her istek kapsamı (scope) için ayrı bir örnek oluşturarak, hizmetlerin kullanım ömrü orta düzeyde bir süre ile sınırlıdır. Bu yöntem genellikle, hizmetlerin her istekte aynı örneğinin kullanılması gerektiği durumlarda kullanılır.Örnek kullanım:services.AddScoped<ISampleService, SampleService>();

            //AddSingleton:AddSingleton yöntemi, uygulama yaşam döngüsü boyunca sadece bir örnek oluşturur ve bu örnek, tüm istekler tarafından paylaşılır. Yani, örnek sadece bir kez oluşturularak, hizmetlerin kullanım ömrü uygulama yaşam döngüsü ile aynıdır. Bu yöntem genellikle, uygulama düzeyindeki hizmetlerin kullanılması gerektiği durumlarda kullanılır.



            //AddScoped, ASP.NET Core Dependency Injection (DI) container'da hizmetlerin kaydedilmesi için kullanılan bir yöntemdir.AddScoped metodu, uygulamanın yaşam döngüsü boyunca bir kez oluşturulan bir hizmet örneği (service instance) oluşturur ve bu örnek, istek başına bir kopya olarak istekin ömrü boyunca kullanılabilir.Yani, bir hizmet sınıfını AddScoped ile kaydedersek, her istek için yeni bir hizmet örneği oluşturulur. İsteğin kapsamı (scope) içinde farklı bileşenler, farklı örnekleri kullanabilirler. Bu sayede, herhangi bir istek sırasında aynı hizmet örneği birden fazla istek tarafından paylaşılır.
            services.AddDbContext<GooleAPIDbContext>(
                options => options.UseSqlServer(Configuration.ConnectionString)
            );

            services.AddScoped<ILogReadRepository, LogReadRepository>();
            services.AddScoped<ILogWriteRepository, LogWriteRepository>();
            services.AddScoped<IUserWriteRepository, UserWriteRepository>();
            services.AddScoped<IUserReadRepository, UserReadRepository>();
            services.AddScoped<ICargoBarcodeReadRepository, CargoBarcodeReadRepository>();
            services.AddScoped<ICargoBarcodeWriteRepository, CargoBarcodeWriteRepository>();

            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ILogService, LogService>();

            services.AddScoped<IGeneralService, GeneralService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<ITransferService, TransferService>();
            services.AddScoped<ICountService, CountService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IFileUploadService, FileUploadService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IHelperService, HelperService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IMNGCargoService, MngCargoService>();



        }
        //extention fonksiyonları
    }
}
