﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

using UniLinks.API.Business.Interfaces;
using UniLinks.Dependencies.Attributes;
using UniLinks.Dependencies.Data.VO;
using UniLinks.Dependencies.Data.VO.Lesson;
using UniLinks.Dependencies.Enums;

namespace UniLinks.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LessonsController : ControllerBase
	{
		private readonly ILessonBusiness _lessonBusiness;
		private readonly ICourseBusiness _courseBusiness;

		public LessonsController(ILessonBusiness lessonBusiness, ICourseBusiness courseBusiness)
		{
			_lessonBusiness = lessonBusiness;
			_courseBusiness = courseBusiness;
		}

		// POST: /Lessons
		[HttpPost("add")]
		[Authorizes(UserTypeEnum.Coordinator)]
		public async Task<IActionResult> AddClassTaskAsync([FromBody] LessonVO lesson)
		{
			if (ModelState.IsValid)
			{
				var coordId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

				if (await _courseBusiness.FindByCoordIdTaskAsync(coordId) is CourseVO course)
					if (course.CourseId != lesson.CourseId)
						return Unauthorized("Voce nao tem permissao para adicionar aulas em outro curso!");

				if (await _lessonBusiness.FindByURITaskAsync(lesson.URI) is LessonVO)
					return Conflict("A aula informada ja existe, verifique se o link está correto");

				if (!(await _lessonBusiness.GetRecordingInfoTaskAsync(lesson) is LessonVO lessonCollab))
					return NotFound("Nao foi possivel encontrar as informaçoes da aula informada, verifique se o link está correto!");

				if (await _lessonBusiness.AddTaskAsync(lessonCollab) is LessonVO addedClass)
					return Created($"/lessons/{addedClass.LessonId}", addedClass);
			}

			return BadRequest();
		}

		// GET: /Lessons/:id
		[HttpGet("{id}")]
		[Authorize]
		public async Task<IActionResult> FindByDisciplineIdTaskAsync(Guid id)
		{
			if (ModelState.IsValid)
			{
				if (await _lessonBusiness.FindByIdTaskAsync(id) is LessonVO lesson)
					return Ok(lesson);

				return NotFound("A aula informada nao existe!");
			}

			return BadRequest();
		}

		[HttpPost]
		[Authorizes]
		public async Task<IActionResult> FindAllByDisciplinesTaskAsync([FromBody] List<Guid> disciplines)
		{
			if (ModelState.IsValid)
			{
				if ((await _lessonBusiness.FindAllByRangeDisciplinesIdTaskASync(disciplines)) is List<LessonDisciplineVO> lessons)
					return Ok(lessons);

				return NotFound("Nao foi possivel encontrar as aulas requisitadas.");
			}

			return BadRequest();
		}

		// PUT: /Lessons
		[HttpPut]
		[Authorizes(UserTypeEnum.Coordinator)]
		public async Task<IActionResult> UpdateTaskAsync([FromBody] LessonVO newLesson)
		{
			if (ModelState.IsValid)
			{
				var coordId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

				if (await _courseBusiness.FindByCoordIdTaskAsync(coordId) is CourseVO course)
					if (course.CourseId != newLesson.CourseId)
						return Unauthorized("Voce nao tem permissao para adicionar aulas em outro curso!");

				if (await _lessonBusiness.FindByURITaskAsync(newLesson.URI) is LessonVO)
					return Conflict("A aula informada ja existe, verifique se o link está correto");

				if (await _lessonBusiness.UpdateTaskAsync(newLesson) is LessonVO lesson)
					return Ok(lesson);

				return NotFound("Nao foi possivel atualizar os dados, verifique se a aula realmente existe!");
			}

			return BadRequest();
		}

		// DELETE: /Lessons/:id
		[HttpDelete("{lessonId}")]
		[Authorizes(UserTypeEnum.Coordinator)]
		public async Task<IActionResult> DeleteTaskAsync([Required] Guid lessonId)
		{
			if (ModelState.IsValid)
			{
				if (await _lessonBusiness.FindByIdTaskAsync(lessonId) is LessonVO lesson)
				{
					var coordId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

					if (await _courseBusiness.FindByCoordIdTaskAsync(coordId) is CourseVO course)
						if (coordId != course.CoordinatorId)
							return Unauthorized("Voce nao tem permissao para remover aulas em outro curso!");

					await _lessonBusiness.DeleteAsync(lessonId);
					return NoContent();
				}

				return BadRequest("Aula informada nao existe");
			}

			return BadRequest();
		}
	}
}