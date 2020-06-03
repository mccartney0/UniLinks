﻿using Blazored.SessionStorage;

using Dependencies.Services;

using RestSharp;
using RestSharp.Authenticators;

using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

using UniLinks.Dependencies.Data.VO.Student;
using UniLinks.Dependencies.Helper;

namespace UniLinks.Client.Site.Services.Coordinator
{
	public class StudentService
	{
		private readonly ISessionStorageService _sessionStorage;

		public StudentService(ISessionStorageService sessionStorage)
		{
			_sessionStorage = sessionStorage;
		}

		public async Task<StudentDisciplineVO> AddStudentTaskAsync(StudentVO student)
		{
			student.CourseId = Guid.Parse(await _sessionStorage.GetItemAsync<string>("courseId"));
			IRestResponse response = await SendRequestTaskAsync(await _sessionStorage.GetItemAsync<string>("token"), student);

			if (response.StatusCode == HttpStatusCode.Created)
				return JsonSerializer.Deserialize<StudentDisciplineVO>(response.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			return default;

			static async Task<IRestResponse> SendRequestTaskAsync(string token, StudentVO student)
			{
				return await new RequestService()
				{
					Method = Method.POST,
					URL = DataHelper.URLBase,
					URN = $"Students",
					Body = student,
					Authenticator = new JwtAuthenticator(token)
				}.ExecuteTaskAsync();
			}
		}

		public async Task<List<StudentDisciplineVO>> GetStudentsTaskAsync()
		{
			IRestResponse response = await SendRequestTaskAsync(await _sessionStorage.GetItemAsync<string>("token"));

			if (response.StatusCode == HttpStatusCode.OK)
				return JsonSerializer.Deserialize<List<StudentDisciplineVO>>(response.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
			else
				return new List<StudentDisciplineVO>();

			return default;

			async Task<IRestResponse> SendRequestTaskAsync(string token)
			{
				return await new RequestService()
				{
					Method = Method.GET,
					URL = DataHelper.URLBase,
					URN = $"Students/all",
					Authenticator = new JwtAuthenticator(token)
				}.ExecuteTaskAsync();
			}
		}

		public async Task<StudentVO> UpdateStudentTaskAsync(StudentVO student)
		{
			student.CourseId = Guid.Parse(await _sessionStorage.GetItemAsync<string>("courseId"));
			IRestResponse response = await SendRequestTaskAsync(await _sessionStorage.GetItemAsync<string>("token"), student);

			if (response.StatusCode == HttpStatusCode.OK)
				return JsonSerializer.Deserialize<StudentVO>(response.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			return default;

			static async Task<IRestResponse> SendRequestTaskAsync(string token, StudentVO student)
			{
				return await new RequestService()
				{
					Method = Method.PUT,
					URL = DataHelper.URLBase,
					URN = $"Students",
					Body = student,
					Authenticator = new JwtAuthenticator(token)
				}.ExecuteTaskAsync();
			}
		}

		public async Task<bool> RemoveStudentTaskAsync(Guid studentId)
		{
			IRestResponse response = await SendRequestTaskAsync(await _sessionStorage.GetItemAsync<string>("token"), studentId);

			return response.StatusCode == HttpStatusCode.NoContent;

			static async Task<IRestResponse> SendRequestTaskAsync(string token, Guid studentId)
			{
				return await new RequestService()
				{
					Method = Method.DELETE,
					URL = DataHelper.URLBase,
					URN = $"Students/{studentId}",
					Authenticator = new JwtAuthenticator(token)
				}.ExecuteTaskAsync();
			}
		}
	}
}