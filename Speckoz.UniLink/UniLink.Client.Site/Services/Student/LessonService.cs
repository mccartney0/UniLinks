﻿using Dependencies.Services;

using RestSharp;
using RestSharp.Authenticators;

using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

using UniLink.Client.Site.Helper;
using UniLink.Dependencies.Data.VO;

namespace UniLink.Client.Site.Services.Student
{
	public class LessonService
	{
		public async Task<IList<LessonVO>> GetAllLessonsTaskAync(string token, string disciplines)
		{
			IRestResponse response = await SendRequestTaskAsync(token, disciplines);

			if (response.StatusCode == HttpStatusCode.OK)
				return JsonSerializer.Deserialize<List<LessonVO>>(response.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			return default;
		}

		private async Task<IRestResponse> SendRequestTaskAsync(string token, string disciplines)
		{
			return await new RequestService()
			{
				Method = Method.GET,
				URL = DataHelper.URLBase,
				URN = $"lessons/all/{disciplines}",
				Authenticator = new JwtAuthenticator(token)
			}.ExecuteTaskAsync();
		}
	}
}