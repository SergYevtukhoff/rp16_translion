﻿using IDAL.Interfaces.Repositories;
using IDAL.Models;

namespace DAL.Repositories
{
    internal class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        internal QuestionRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
