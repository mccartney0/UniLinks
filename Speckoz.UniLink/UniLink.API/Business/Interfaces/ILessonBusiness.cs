﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UniLink.Dependencies.Data.VO;
using UniLink.Dependencies.Enums;

namespace UniLink.API.Business.Interfaces
{
	public interface ILessonBusiness
	{
		Task<LessonVO> AddTaskAsync(LessonVO lesson);

		Task<LessonVO> FindByIdTaskAsync(Guid lessonId);

		Task<LessonVO> FindByURITaskAsync(string uri);

		Task<LessonVO> FindByDateTaskAsync(DateTime dateTime, LessonShiftEnum LessonShift);

		Task<IList<LessonVO>> FindAllByDisciplinesIdTaskASync(string disciplines);

		Task<LessonVO> UpdateTaskAsync(LessonVO newLesson);

		Task<bool> DeleteTaskAsync(Guid lessonId);
	}
}