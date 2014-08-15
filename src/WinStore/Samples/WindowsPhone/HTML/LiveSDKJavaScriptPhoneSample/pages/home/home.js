<!-- --------------------------------------------------------------------------
// Copyright (c) 2014 Microsoft Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
// ------------------------------------------------------------------------ -->

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

