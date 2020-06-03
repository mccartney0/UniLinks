﻿using Blazored.SessionStorage;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Linq;
using System.Threading.Tasks;

namespace UniLinks.Client.Site.Pages
{
    public partial class IndexPage
    {
        private string name;

        [Inject]
        private AuthenticationStateProvider Authentication { get; set; }

        [Inject]
        private ISessionStorageService SessionStorage { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            string fullName = await SessionStorage.GetItemAsync<string>("name");
            if (fullName is string)
            {
                if (fullName.Contains(' '))
                    name = new string(fullName.Take(fullName.IndexOf(' ')).ToArray());
                else
                    name = fullName;
            }
        }

        private async Task GoToAsync(string id)
        {
            await jSRuntime.InvokeVoidAsync("GoTo", id);
        }
    }
}