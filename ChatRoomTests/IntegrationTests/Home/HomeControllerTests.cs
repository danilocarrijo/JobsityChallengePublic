using ChatRoomChallenge.Controllers;
using ChatRoomChallenge.Hubs.ChatCenter;
using Entities;
using Events;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Repository;
using RepositoryInterface;
using Services;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatRoomTests.IntegrationTests.Home
{

    class HomeControllerTests
    {
        public IChatCenterController _chatCenterController;
        public IMessageService _messageService;

        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<SignInManager<AppUser>> _signInManagerMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ChatDbContext>()
                   .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                   .Options;

            EventDispacher dispacher = new EventDispacher();

            _chatCenterController = new ChatCenterController(dispacher);

            _messageService = new MessageService(new ChatDbContext(options));

            var mockStore = Mock.Of<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(mockStore, null, null, null, null, null, null, null, null);

            var _contextAccessor = new Mock<IHttpContextAccessor>();
            var _userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<AppUser>>();

            _signInManagerMock = new Mock<SignInManager<AppUser>>(_userManagerMock.Object,
               _contextAccessor.Object, _userPrincipalFactory.Object, null, null, null, null);


        }

        [Test]
        public async Task Register_InvalidUser_ShouldReturnView()
        {
            var dummyUser = new AppUser()
            {
                Name = "teste",
                Password = "!Aa123456",
                Email = "teste@gmail.com"
            };

            _signInManagerMock
                .Setup(x => x.PasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError()));

            HomeController _controller = new HomeController(_userManagerMock.Object, _signInManagerMock.Object, _chatCenterController, _messageService);


            var result = await _controller.Register(dummyUser);

            Assert.IsAssignableFrom(typeof(ViewResult),result);


        }

        [Test]
        public async Task Register_ValidUser_ShouldredirectToIndex()
        {
            var dummyUser = new AppUser()
            {
                Name = "teste",
                Password = "!Aa123456",
                Email = "teste@gmail.com"
            };

            _signInManagerMock
                .Setup(x => x.PasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            HomeController _controller = new HomeController(_userManagerMock.Object, _signInManagerMock.Object, _chatCenterController, _messageService);


            var result = (RedirectToActionResult)await _controller.Register(dummyUser) ;

            Assert.AreEqual(result.ActionName, "Index");


        }

        [Test]
        public async Task LogOut_ShouldredirectToIndex()
        {
            _signInManagerMock
                .Setup(x => x.SignOutAsync())
                .Returns(Task.FromResult(default(object)));

            HomeController _controller = new HomeController(_userManagerMock.Object, _signInManagerMock.Object, _chatCenterController, _messageService);


            var result = (RedirectToActionResult)await _controller.LogOut();

            Assert.AreEqual(result.ActionName, "Index");


        }

        [Test]
        public async Task Index_ValidUser_ShuldReturnViewWithMessages()
        {

            var httpContext = new DefaultHttpContext();
            var claims = new Claim[1];
            claims[0] = new Claim(ClaimTypes.Name, "foo");
            var claimsIdentity = new ClaimsIdentity(claims);

            httpContext.User = new ClaimsPrincipal(claimsIdentity);

            _userManagerMock
                .Setup(x => x.GetUserAsync(httpContext.User))
                .ReturnsAsync(new AppUser());

            HomeController _controller = new HomeController(_userManagerMock.Object, _signInManagerMock.Object, _chatCenterController, _messageService);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            _messageService.Add(new Message
            {
                User = "teste",
                MessageStrin = "teste",
                MessageDateMessage = DateTime.Now
            });

            var result = (ViewResult)await _controller.Index();

            var res = ((IEnumerable<Message>)_controller.ViewBag.MessageHIstory).ToList() ;

            Assert.IsNotNull(result);
            Assert.AreEqual(res.Count, 1);
        }

        [Test]
        public async Task Index_InvalidUser_ShuldRedirectToLogin()
        {

            var httpContext = new DefaultHttpContext();
            var claims = new Claim[1];
            claims[0] = new Claim(ClaimTypes.Name, "foo");
            var claimsIdentity = new ClaimsIdentity(claims);

            httpContext.User = new ClaimsPrincipal(claimsIdentity);

            _userManagerMock
                .Setup(x => x.GetUserAsync(httpContext.User))
                .ReturnsAsync(() => null);

            HomeController _controller = new HomeController(_userManagerMock.Object, _signInManagerMock.Object, _chatCenterController, _messageService);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
            var result = (RedirectToActionResult)await _controller.Index();

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ActionName, "Login");
        }
    }
}
