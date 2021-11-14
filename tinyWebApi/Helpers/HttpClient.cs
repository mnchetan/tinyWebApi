/// <copyright file="HttpClient.cs" company="tiny">
///     Copyright (c) 2021 tiny. All rights reserved.
/// </copyright>
/// <summary>
///     Implements the HTTP client class.
/// </summary>
using tinyWebApi.Common.DataObjects;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mime;
// namespace: tinyWebApi.Common.Helpers
//
// summary:	.
namespace tinyWebApi.Common.Helpers
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
        ///     Initializes a new instance of the tinyWebApi.Common.Helpers.HttpClient class.
        /// </summary>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public HttpClient() => _httpClient = new System.Net.Http.HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
        /// <summary>
        ///     Gets a t using the given request URI.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="requestUri"> URI of the request. </param>
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public T Get<T>(Uri requestUri)
        {
            var response = _httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            catch(Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            return JsonConvert.DeserializeObject<T>(data);
        }
        /// <summary>
        ///     Gets an asynchronous.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="requestUri"> URI of the request. </param>
        /// <returns>
        ///     The async&lt; t&gt;
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public async Task<T> GetAsync<T>(Uri requestUri)
        {
            var response = _httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = await response.Content.ReadAsStringAsync();
            }
            return JsonConvert.DeserializeObject<T>(data);
        }
        /// <summary>
        ///     Post this message.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="requestUri"> URI of the request. </param>
        /// <param name="content">    The content. </param>
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public T Post<T>(Uri requestUri, T content)
        {
            var response = _httpClient.PostAsync(requestUri, CreateHttpContent(content)).GetAwaiter().GetResult();
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
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
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public T1 Post<T1, T2>(Uri requestUri, T2 content)
        {
            var response = _httpClient.PostAsync(requestUri, CreateHttpContent(content)).GetAwaiter().GetResult();
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            return JsonConvert.DeserializeObject<T1>(data);
        }
        /// <summary>
        ///     Posts an asynchronous.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="requestUri"> URI of the request. </param>
        /// <param name="content">    The content. </param>
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public async Task<T> PostAsync<T>(Uri requestUri, T content)
        {
            var response = await _httpClient.PostAsync(requestUri, CreateHttpContent(content));
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = await response.Content.ReadAsStringAsync();
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
        /// <returns>
        ///     A T.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public async Task<T1> PostAsync<T1, T2>(Uri requestUri, T2 content)
        {
            var response = await _httpClient.PostAsync(requestUri, CreateHttpContent(content));
            string data;
            try
            {
                response.EnsureSuccessStatusCode();
                data = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Global.LogError($"Error while triggering call. {Environment.NewLine} Error : {ex.Message}", ex);
                data = await response.Content.ReadAsStringAsync();
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
