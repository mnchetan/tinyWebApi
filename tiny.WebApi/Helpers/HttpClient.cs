// <copyright file="HttpClient.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Implements the HTTP client class.
// </summary>
using tiny.WebApi.DataObjects;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mime;
using System.IO;
using System.Threading;
// namespace: tiny.WebApi.Helpers
//
// summary:	.
namespace tiny.WebApi.Helpers
{
    /// <summary>
    ///     A HTTP client.
    /// </summary>
    [DebuggerStepThrough]
    public class HttpClient
    {
        /// <summary>
        ///     (Immutable) the HTTP client.
        /// </summary>
        private readonly System.Net.Http.HttpClient _httpClient;
        /// <summary>
        ///     Initializes a new instance of the tiny.WebApi.Helpers.HttpClient class.
        /// </summary>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public HttpClient() => _httpClient = new System.Net.Http.HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
        /// <summary>
        ///     Gets a t using the given request URI.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="requestUri"> URI of the request. </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public T Get<T>(Uri requestUri, CancellationToken cancellationToken)
        {
            Global.LogInformation("Inside Get, get data using httpclient synchronously.");
            var response = _httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken).GetAwaiter().GetResult();
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
            }
            return JsonConvert.DeserializeObject<T>(data);
        }
        /// <summary>
        ///     Gets a byte[] using the given request URI.
        /// </summary>
        /// <param name="requestUri"> URI of the request. </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     A byte[].
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public byte[] GetAsBytes<T>(Uri requestUri, CancellationToken cancellationToken)
        {
            Global.LogInformation("Inside GetAsBytes, get byte array using httpclient synchronously.");
            return _httpClient.GetByteArrayAsync(requestUri, cancellationToken).GetAwaiter().GetResult();
        }
        /// <summary>
        ///     Gets a System.IO.Stream using the given request URI.
        /// </summary>
        /// <param name="requestUri"> URI of the request. </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     A System.IO.Stream.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public Stream GetAsStream<T>(Uri requestUri, CancellationToken cancellationToken)
        {
            Global.LogInformation("Inside GetAsStream, get data stream using httpclient synchronously.");
            return _httpClient.GetStreamAsync(requestUri, cancellationToken).GetAwaiter().GetResult();
        }
        /// <summary>
        ///     Gets an asynchronous.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="requestUri"> URI of the request. </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     The async&lt; t&gt;
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public async Task<T> GetAsync<T>(Uri requestUri, CancellationToken cancellationToken)
        {
            Global.LogInformation("Inside Get, get data using httpclient asynchronously.");
            var response = _httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken).GetAwaiter().GetResult();
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            return JsonConvert.DeserializeObject<T>(data);
        }
        /// <summary>
        ///     Post this message.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="requestUri"> URI of the request. </param>
        /// <param name="content">    The content. </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public T Post<T>(Uri requestUri, T content, CancellationToken cancellationToken)
        {
            Global.LogInformation("Inside Post, post data using httpclient synchronously.");
            var response = _httpClient.PostAsync(requestUri, CreateHttpContent(content), cancellationToken).GetAwaiter().GetResult();
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
            }
            return JsonConvert.DeserializeObject<T>(data);
        }
        /// <summary>
        ///     Post this message.
        /// </summary>
        /// <typeparam name="T1"> Generic type parameter. </typeparam>
        /// <typeparam name="T2"> Generic type parameter. </typeparam>
        /// <param name="requestUri"> URI of the request. </param>
        /// <param name="content">    The content. </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public T1 Post<T1, T2>(Uri requestUri, T2 content, CancellationToken cancellationToken)
        {
            Global.LogInformation("Inside Post, post data using httpclient synchronously.");
            var response = _httpClient.PostAsync(requestUri, CreateHttpContent(content), cancellationToken).GetAwaiter().GetResult();
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
            }
            return JsonConvert.DeserializeObject<T1>(data);
        }
        /// <summary>
        ///     Posts an asynchronous.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="requestUri"> URI of the request. </param>
        /// <param name="content">    The content. </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public async Task<T> PostAsync<T>(Uri requestUri, T content, CancellationToken cancellationToken)
        {
            Global.LogInformation("Inside Post, post data using httpclient asynchronously.");
            var response = await _httpClient.PostAsync(requestUri, CreateHttpContent(content), cancellationToken);
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            return JsonConvert.DeserializeObject<T>(data);
        }
        /// <summary>
        ///     Posts an asynchronous.
        /// </summary>
        /// <typeparam name="T1"> Generic type parameter. </typeparam>
        /// <typeparam name="T2"> Generic type parameter. </typeparam>
        /// <param name="requestUri"> URI of the request. </param>
        /// <param name="content">    The content. </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public async Task<T1> PostAsync<T1, T2>(Uri requestUri, T2 content, CancellationToken cancellationToken)
        {
            Global.LogInformation("Inside Post, post data using httpclient asynchronously.");
            var response = await _httpClient.PostAsync(requestUri, CreateHttpContent(content), cancellationToken);
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            return JsonConvert.DeserializeObject<T1>(data);
        }
        /// <summary>
        ///     Put this message.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="requestUri"> URI of the request. </param>
        /// <param name="content">    The content. </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public T Put<T>(Uri requestUri, T content, CancellationToken cancellationToken)
        {
            Global.LogInformation("Inside Put, Put data using httpclient synchronously.");
            var response = _httpClient.PutAsync(requestUri, CreateHttpContent(content), cancellationToken).GetAwaiter().GetResult();
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
            }
            return JsonConvert.DeserializeObject<T>(data);
        }
        /// <summary>
        ///     Put this message.
        /// </summary>
        /// <typeparam name="T1"> Generic type parameter. </typeparam>
        /// <typeparam name="T2"> Generic type parameter. </typeparam>
        /// <param name="requestUri"> URI of the request. </param>
        /// <param name="content">    The content. </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public T1 Put<T1, T2>(Uri requestUri, T2 content, CancellationToken cancellationToken)
        {
            Global.LogInformation("Inside Put, Put data using httpclient synchronously.");
            var response = _httpClient.PutAsync(requestUri, CreateHttpContent(content), cancellationToken).GetAwaiter().GetResult();
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
            }
            return JsonConvert.DeserializeObject<T1>(data);
        }
        /// <summary>
        ///     Puts an asynchronous.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="requestUri"> URI of the request. </param>
        /// <param name="content">    The content. </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public async Task<T> PutAsync<T>(Uri requestUri, T content, CancellationToken cancellationToken)
        {
            Global.LogInformation("Inside Put, Put data using httpclient asynchronously.");
            var response = await _httpClient.PutAsync(requestUri, CreateHttpContent(content), cancellationToken);
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            return JsonConvert.DeserializeObject<T>(data);
        }
        /// <summary>
        ///     Puts an asynchronous.
        /// </summary>
        /// <typeparam name="T1"> Generic type parameter. </typeparam>
        /// <typeparam name="T2"> Generic type parameter. </typeparam>
        /// <param name="requestUri"> URI of the request. </param>
        /// <param name="content">    The content. </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public async Task<T1> PutAsync<T1, T2>(Uri requestUri, T2 content, CancellationToken cancellationToken)
        {
            Global.LogInformation("Inside Put, Put data using httpclient asynchronously.");
            var response = await _httpClient.PutAsync(requestUri, CreateHttpContent(content), cancellationToken);
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            return JsonConvert.DeserializeObject<T1>(data);
        }
        /// <summary>
        ///     Deletes a t using the given request URI.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="requestUri"> URI of the request. </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public T Delete<T>(Uri requestUri, CancellationToken cancellationToken)
        {
            Global.LogInformation("Inside Delete, Delete data using httpclient synchronously.");
            var response = _httpClient.DeleteAsync(requestUri, cancellationToken).GetAwaiter().GetResult();
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
            }
            return JsonConvert.DeserializeObject<T>(data);
        }
        /// <summary>
        ///     Deletes an asynchronous.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="requestUri"> URI of the request. </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     The async&lt; t&gt;
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public async Task<T> DeleteAsync<T>(Uri requestUri, CancellationToken cancellationToken)
        {
            Global.LogInformation("Inside Delete, Delete data using httpclient asynchronously.");
            var response = _httpClient.DeleteAsync(requestUri, cancellationToken).GetAwaiter().GetResult();
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            return JsonConvert.DeserializeObject<T>(data);
        }
        /// <summary>
        ///     Patch this message.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="requestUri"> URI of the request. </param>
        /// <param name="content">    The content. </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public T Patch<T>(Uri requestUri, T content, CancellationToken cancellationToken)
        {
            Global.LogInformation("Inside Patch, Patch data using httpclient synchronously.");
            var response = _httpClient.PatchAsync(requestUri, CreateHttpContent(content), cancellationToken).GetAwaiter().GetResult();
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
            }
            return JsonConvert.DeserializeObject<T>(data);
        }
        /// <summary>
        ///     Patch this message.
        /// </summary>
        /// <typeparam name="T1"> Generic type parameter. </typeparam>
        /// <typeparam name="T2"> Generic type parameter. </typeparam>
        /// <param name="requestUri"> URI of the request. </param>
        /// <param name="content">    The content. </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public T1 Patch<T1, T2>(Uri requestUri, T2 content, CancellationToken cancellationToken)
        {
            Global.LogInformation("Inside Patch, Patch data using httpclient synchronously.");
            var response = _httpClient.PatchAsync(requestUri, CreateHttpContent(content), cancellationToken).GetAwaiter().GetResult();
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
            }
            return JsonConvert.DeserializeObject<T1>(data);
        }
        /// <summary>
        ///     Patchs an asynchronous.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="requestUri"> URI of the request. </param>
        /// <param name="content">    The content. </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public async Task<T> PatchAsync<T>(Uri requestUri, T content, CancellationToken cancellationToken)
        {
            Global.LogInformation("Inside Patch, Patch data using httpclient asynchronously.");
            var response = await _httpClient.PatchAsync(requestUri, CreateHttpContent(content), cancellationToken);
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            return JsonConvert.DeserializeObject<T>(data);
        }
        /// <summary>
        ///     Patchs an asynchronous.
        /// </summary>
        /// <typeparam name="T1"> Generic type parameter. </typeparam>
        /// <typeparam name="T2"> Generic type parameter. </typeparam>
        /// <param name="requestUri"> URI of the request. </param>
        /// <param name="content">    The content. </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public async Task<T1> PatchAsync<T1, T2>(Uri requestUri, T2 content, CancellationToken cancellationToken)
        {
            Global.LogInformation("Inside Patch, Patch data using httpclient asynchronously.");
            var response = await _httpClient.PatchAsync(requestUri, CreateHttpContent(content), cancellationToken);
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            return JsonConvert.DeserializeObject<T1>(data);
        }
        /// <summary>
        ///     Creates HTTP content.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="content"> The content. </param>
        /// <returns>
        ///     The new HTTP content.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static HttpContent CreateHttpContent<T>(T content) => new StringContent(JsonConvert.SerializeObject(content, new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat }), Encoding.UTF8, MediaTypeNames.Application.Json);
    }
}
