using System;
using System.Collections.Generic;

namespace CssUI.HTML
{
    /// <summary>
    /// Defines a function which creates a <see cref="DataRequest"/> that can provide access to the data for a specific request.
    /// </summary>
    /// <param name="requestURI">Url of the resource requested</param>
    /// <param name="outRequest">The request object returned</param>
    /// <returns><c>True</c> if the given resource can be resolved by this function</returns>
    public delegate bool DataRequestResolver(Url requestURI, out DataRequest outRequest);
    public static class DataRequestFactory
    {
        /// <summary>
        /// List of resolver functions used to provide special <see cref="DataRequest"/> instances for special resources
        /// </summary>
        public static ConcurrentHashSet<DataRequestResolver> Resolvers = new ConcurrentHashSet<DataRequestResolver>();

        public static DataRequest Create(string requestURI)
        {
            Url url = new Url(requestURI);
            foreach (DataRequestResolver resolver in Resolvers)
            {
                if (resolver(url, out DataRequest outRequest))
                {
                    return outRequest;
                }
            }

            switch (url.Scheme.EnumValue)
            {
                case EUrlScheme.File:
                    return new FileDataRequest(requestURI);
                case EUrlScheme.Ftp:
                    ...
                case EUrlScheme.Http:
                case EUrlScheme.Https:
                    ...
                default:
                    throw new NotImplementedException($"Unable to provide {nameof(DataRequest)} to handle the requested resource: \"{requestURI}\"");
            }
        }
    }
}
