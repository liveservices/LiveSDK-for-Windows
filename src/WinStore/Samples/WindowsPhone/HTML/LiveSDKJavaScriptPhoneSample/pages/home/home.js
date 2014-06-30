<!-- ------------------------------------------------------------------------------
// Copyright 2014 Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ------------------------------------------------------------------------------ -->

/// <reference path="/LiveSDKHTML/js/wl.js" />
(function () {
    "use strict";

    WinJS.UI.Pages.define("/pages/home/home.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            liveSdkSample.currentPage = liveSdkSample.HOMEPAGE;
            WL.getLoginStatus().then(
                    function (result) {
                        if (result.status == "connected") {
                            statusText.innerText = "You are connected! ";
                            liveSdkSample.displayMe();
                        }
                        else {
                            statusText.innerText = "You are not connected! ";
                            connectButton.onclick = function () {
                                WL.login({
                                    scope: liveSdkSample.defaultScopes
                                }).then(
                                    function (result) {
                                        if (result.status == "connected") {
                                            connectButton.style.display = "none";
                                            statusText.innerText = "You are connected!";
                                            liveSdkSample.displayMe();
                                        }
                                    }
                                );
                            };
                            connectButton.style.display = "block";
                        }
                    });
        },

        
    });
})();

