﻿using Money.Events;
using Money.Models;
using Money.Queries;
using Neptuo.Events.Handlers;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    internal class UserMiddleware : HttpQueryDispatcher.IMiddleware,
        IEventHandler<EmailChanged>,
        IEventHandler<UserSignedOut>
    {
        private ProfileModel profile;
        private Task getProfileTask;

        public async Task<object> ExecuteAsync(object query, HttpQueryDispatcher dispatcher, HttpQueryDispatcher.Next next)
        {
            if (query is GetProfile getProfile)
            {
                if (profile == null)
                {
                    if (getProfileTask == null)
                        getProfileTask = LoadProfileAsync(getProfile, next).ContinueWith(t => getProfileTask = null);

                    await getProfileTask;
                }

                return profile;
            }

            return await next(query);
        }

        private async Task LoadProfileAsync(GetProfile query, HttpQueryDispatcher.Next next)
        {
            profile = (ProfileModel)await next(query);
        }

        Task IEventHandler<EmailChanged>.HandleAsync(EmailChanged payload)
        {
            if (profile != null)
                profile.Email = payload.Email;

            return Task.CompletedTask;
        }

        Task IEventHandler<UserSignedOut>.HandleAsync(UserSignedOut payload)
        {
            profile = null;

            return Task.CompletedTask;
        }
    }
}
