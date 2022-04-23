﻿using LiteNetwork.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace LiteNetwork.Client.Hosting
{
    public static class LiteClientBuilderExtensions
    {
        /// <summary>
        /// Initializes a custom <see cref="ILiteClient"/>.
        /// </summary>
        /// <typeparam name="TLiteClient">Custom client type.</typeparam>
        /// <param name="builder">A <see cref="ILiteBuilder"/> to add the client.</param>
        /// <param name="configure">Delegate to configure a <see cref="LiteClientOptions"/>.</param>
        /// <returns>The <see cref="ILiteBuilder"/>.</returns>
        public static ILiteBuilder AddLiteClient<TLiteClient>(this ILiteBuilder builder, Action<LiteClientOptions> configure)
            where TLiteClient : LiteClient
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddSingleton(serviceProvider =>
            {
                LiteClientOptions options = new();
                configure(options);

                return ActivatorUtilities.CreateInstance<TLiteClient>(serviceProvider, options);
            });

            builder.Services.AddLiteClientHostedService<TLiteClient>();

            return builder;
        }

        /// <summary>
        /// Initializes a custom <see cref="ILiteClient"/> with a custom interface.
        /// </summary>
        /// <typeparam name="TLiteClient">LiteClient abstraction.</typeparam>
        /// <typeparam name="TLiteClientImplementation">LiteClient implementation.</typeparam>
        /// <param name="builder">A <see cref="ILiteBuilder"/> to add the client.</param>
        /// <param name="configure">Delegate to configure a <see cref="LiteClientOptions"/>.</param>
        /// <returns>The <see cref="ILiteBuilder"/>.</returns>
        public static ILiteBuilder AddLiteClient<TLiteClient, TLiteClientImplementation>(this ILiteBuilder builder, Action<LiteClientOptions> configure)
            where TLiteClient : class
            where TLiteClientImplementation : LiteClient, TLiteClient
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddSingleton<TLiteClient, TLiteClientImplementation>(serviceProvider =>
            {
                LiteClientOptions options = new();
                configure(options);

                return ActivatorUtilities.CreateInstance<TLiteClientImplementation>(serviceProvider, options);
            });

            builder.Services.AddLiteClientHostedService<TLiteClientImplementation>();

            return builder;
        }

        private static void AddLiteClientHostedService<TLiteClient>(this IServiceCollection services)
            where TLiteClient : LiteClient
        {
            services.AddSingleton<IHostedService>(serviceProvider => new LiteClientHostedService(serviceProvider.GetRequiredService<TLiteClient>()));
        }
    }
}
