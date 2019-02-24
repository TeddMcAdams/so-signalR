using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using server.Models;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private static ConcurrentBag<Question> questions = new ConcurrentBag<Question> {
            new Question {
                Id = Guid.Parse("b00c58c0-df00-49ac-ae85-0a135f75e01b"),
                Title = "Welcome",
                Body = "Welcome to the _mini Stack Overflow_ rip-off!\nThis will help showcasing **SignalR** and its integration with **Vue**",
                Answers = new List<Answer>{ new Answer { Body = "Sample answer" }}
            }
        };

        [HttpGet]
        public IEnumerable GetQuestions()
        {
            return questions.Select(q => new {
                Id = q.Id,
                Title = q.Title,
                Body = q.Body,
                Score = q.Score,
                AnswerCount = q.Answers.Count
            });
        }

        [HttpGet("{id}")]
        public ActionResult GetQuestion(Guid id)
        {
            Question q = questions.SingleOrDefault(m => m.Id == id);

            if (q == null) return NotFound();

            return new JsonResult(q);
        }

        [HttpPost]
        public Question AddQuestion([FromBody]Question question)
        {
            question.Id = Guid.NewGuid();
            question.Answers = new List<Answer>();

            questions.Add(question);

            return question;
        }

        [HttpPost("{id}/answer")]
        public ActionResult AddAnswerAsync(Guid id, [FromBody]Answer answer)
        {
            Question q = questions.SingleOrDefault(m => m.Id == id);
            
            if (q == null) return NotFound();

            answer.Id = Guid.NewGuid();
            answer.QuestionId = id;

            q.Answers.Add(answer);

            return new JsonResult(answer);
        }

        [HttpPatch("{id}/upvote")]
        public ActionResult UpvoteQuestionAsync(Guid id)
        {
            Question q = questions.SingleOrDefault(m => m.Id == id);

            if (q == null) return NotFound();

            // TODO Fix thread safety
            q.Score++;

            return new JsonResult(q);
        }
    }
}
