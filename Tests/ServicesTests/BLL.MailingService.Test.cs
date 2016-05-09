﻿using BLL.Services.MailingService;
using System.Collections.Generic;
using System.Net.Mail;
using Xunit;

namespace Tests.ServicesTests
{
    /// <summary>
    /// Mailing service tests
    /// NOTE:
    /// even if only one paramether in constructor is invalid -> exception will be thrown
    /// This situation wasn't tested
    /// </summary>
    public class BLL_MailingService_Test
    {
        MailingService _service;

        public BLL_MailingService_Test()
        {
            _service = new MailingService("bu.translion@gmail.com", "superPass", "smtp.gmail.com");
            _service.IgnoreQueue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("  ")]
        public void SendMail_with_invalid_body_failed(string body)
        {
            //arrange
            //act
            var fakeRes = _service.SendMail(body, "Normal", "yegor.sergeev@gmail.com");
            var asyncFakeRes = _service.SendMailAsync(body, "Normal", "yegor.sergeev@gmail.com").Result;
            //assert
            Assert.True(fakeRes.HasError);
            Assert.True(asyncFakeRes.HasError);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("  ")]
        public void SendMail_with_invalid_subject_failed(string subject)
        {
            //arrange
            //act
            var fakeRes = _service.SendMail("Normal", subject, "yegor.sergeev@gmail.com");
            var asyncFakeRes = _service.SendMailAsync("Normal", subject, "yegor.sergeev@gmail.com").Result;
            //assert
            Assert.True(fakeRes.HasError);
            Assert.True(asyncFakeRes.HasError);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("  ")]
        [InlineData("asdasd")]
        [InlineData("UNKNOWNMAIL")]
        [InlineData(")))___)+|")]
        public void SendMail_with_invalid_reciever_failed(string reciever)
        {
            //arrange
            //act
            var fakeRes = _service.SendMail("Body", "Normal", reciever);
            var asyncFakeRes = _service.SendMailAsync("Body", "Normal", reciever).Result;
            //assert
            Assert.True(fakeRes.HasError);
            Assert.True(asyncFakeRes.HasError);
        }

        [Theory]
        [InlineData("yegor.sergeev@gmail.com")]
        [InlineData("www.egor.sergeew@mail.ru")]
        [InlineData("yegor.sergeev@mail.ru")]
        public void SendMail_with_valid_data_succeeded(string reciever)
        {
            //arrange
            //act
            var fakeRes = _service.SendMail("Test", "Normal", reciever);
            var asyncFakeRes = _service.SendMailAsync("Test", "Normal", reciever).Result;
            //assert
            Assert.False(fakeRes.HasError);
            Assert.False(asyncFakeRes.HasError);
        }

        [Fact]
        public void SendMail_with_invalid_recievers_collection_failed()
        {
            //arrange
            var fakeRecievers = new[] { null, "", "yegor.sergeev@mail.ru" };
            var fakeRecievers1 = new[] { "yegor.sergeev@gmail.com", "aaaa" };
            var fakeRecievers2 = new[] { "yegor.sergeev@gmail.com", "yegor.sergeev@mail.ru", "" };
            var fakeRecievers3 = new [] { "yegor.sergeev@gmail.com", " ", "yegor.sergeev@mail.ru" };
            //act
            var fakeRes = _service.SendMail("Good", "Good", fakeRecievers);
            var asyncFakeRes = _service.SendMailAsync("Good", "Good", fakeRecievers).Result;
            var fakeRes1 = _service.SendMail("Good", "Good", fakeRecievers1);
            var asyncFakeRes1 = _service.SendMailAsync("Good", "Good", fakeRecievers1).Result;
            var fakeRes2 = _service.SendMail("Good", "Good", fakeRecievers2);
            var asyncFakeRes2 = _service.SendMailAsync("Good", "Good", fakeRecievers2).Result;
            var fakeRes3 = _service.SendMail("Good", "Good", fakeRecievers3);
            var asyncFakeRes3 = _service.SendMailAsync("Good", "Good", fakeRecievers3).Result;
            //assert
            Assert.True(fakeRes.HasError);
            Assert.True(asyncFakeRes.HasError);
            Assert.True(fakeRes1.HasError);
            Assert.True(asyncFakeRes1.HasError);
            Assert.True(fakeRes2.HasError);
            Assert.True(asyncFakeRes2.HasError);
            Assert.True(fakeRes3.HasError);
            Assert.True(asyncFakeRes3.HasError);
        }

        [Fact]
        public void SendMail_with_valid_recievers_collection_succeeded()
        {
            //arrange
            var recievers = new[] { "yegor.sergeev@mail.ru", "bu.translion@gmail.com", "yegor.sergeev@gmail.com" };
            //act
            var result = _service.SendMail("Good", "Good", recievers);
            var asyncResult = _service.SendMailAsync("Good", "Good", recievers).Result;
            //assert
            Assert.False(result.HasError);
            Assert.False(asyncResult.HasError);
        }
    }
}
