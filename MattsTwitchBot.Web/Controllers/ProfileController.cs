﻿using System.Threading.Tasks;
using MattsTwitchBot.Core.RequestHandlers.Profile;
using MattsTwitchBot.Web.Filters;
using MattsTwitchBot.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MattsTwitchBot.Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IMediator _mediator;

        public ProfileController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet]
        [Route("/profile")]
        [BearerToken]
        public IActionResult ProfileRedirect(string twitchUsername)
        {
            return RedirectToAction("ProfileEditor", new {twitchUsername = twitchUsername });
        }

        [HttpGet]
        [Route("/profile/{twitchUsername}")]
        [BearerToken]
        public async Task<IActionResult> ProfileEditor(string twitchUsername)
        {
            if (twitchUsername != twitchUsername.ToLower())
                return RedirectToAction("ProfileEditor", new {twitchUsername = twitchUsername.ToLower()});

            await _mediator.Send(new CreateProfileIfNotExists(twitchUsername));

            var twitcherProfile = await _mediator.Send(new GetProfile(twitchUsername));

            var model = new ProfileEditorViewModel();
            model.Map(twitcherProfile, twitchUsername);

            return View("ProfileEditor", model);
        }

        [HttpPost]
        [Route("/profile/{twitchUsername}")]
        [BearerToken]
        public async Task<IActionResult> ProfileEditorPost([FromRoute] string twitchUsername, [FromForm] ProfileEditorViewModel form)
        {
            form.Validate(ModelState);

            if (!ModelState.IsValid)
                return View("ProfileEditor", form);

            var request = new UpdateProfile();
            request.TwitchUsername = twitchUsername;
            request.ShoutMessage = form.ShoutMessage;
            request.FanfareEnabled = form.FanfareEnabled;
            request.FanfareMessage = form.FanfareMessage;
            request.FanfareTimeout = form.FanfareTimeout;
            request.FanfareYouTubeCode = form.FanfareYouTubeCode;
            request.FanfareYouTubeEndTime = form.FanfareYouTubeEndTime;
            request.FanfareYouTubeStartTime = form.FanfareYouTubeStartTime;

            await _mediator.Send(request);

            return Redirect("/profile/" + twitchUsername);
        }
    }
}